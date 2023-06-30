using LlamAcademy.Guns.ImpactEffects;
using LlamAcademy.ImpactSystem;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace LlamAcademy.Guns
{
    [CreateAssetMenu(fileName = "Gun", menuName = "Guns/Gun", order = 0)]
    public class GunScriptableObject : ScriptableObject, ICloneable
    {
        public ImpactType ImpactType;
        public GunType Type;
        public string Name;
        public GameObject ModelPrefab;
        public Vector3 SpawnPoint;
        public Vector3 SpawnRotation;

        public DamageConfigScriptableObject DamageConfig;
        public ShootConfigScriptableObject ShootConfig;
        public AmmoConfigScriptableObject AmmoConfig;
        public TrailConfigScriptableObject TrailConfig;
        public AudioConfigScriptableObject AudioConfig;

        public ICollisionHandler[] BulletImpactEffects = new ICollisionHandler[0];

        private MonoBehaviour ActiveMonoBehaviour;
        private AudioSource ShootingAudioSource;
        private GameObject Model;
        private Camera ActiveCamera;
        private float LastShootTime;
        private float InitialClickTime;
        private float StopShootingTime;

        private ParticleSystem ShootSystem;
        private ObjectPool<TrailRenderer> TrailPool;
        private ObjectPool<Bullet> BulletPool;
        private bool LastFrameWantedToShoot;

        /// <summary>
        /// Spawns the Gun Model into the scene
        /// </summary>
        /// <param name="Parent">Parent for the gun model</param>
        /// <param name="ActiveMonoBehaviour">An Active MonoBehaviour that can have Coroutines attached to them.
        /// <param name="Camera">The camera to raycast from. Required if <see cref="ShootConfigScriptableObject.ShootType"/> = <see cref="ShootType.FromCamera"/></paramref>
        /// The input handling script is a good candidate for this.
        /// </param>
        public void Spawn(Transform Parent, MonoBehaviour ActiveMonoBehaviour, Camera Camera = null)
        {
            this.ActiveMonoBehaviour = ActiveMonoBehaviour;

            TrailPool = new ObjectPool<TrailRenderer>(CreateTrail);
            if (!ShootConfig.IsHitscan)
            {
                BulletPool = new ObjectPool<Bullet>(CreateBullet);
            }

            Model = Instantiate(ModelPrefab);
            Model.transform.SetParent(Parent, false);
            Model.transform.localPosition = SpawnPoint;
            Model.transform.localRotation = Quaternion.Euler(SpawnRotation);

            ActiveCamera = Camera;

            ShootingAudioSource = Model.GetComponent<AudioSource>();
            ShootSystem = Model.GetComponentInChildren<ParticleSystem>();
        }

        /// <summary>
        /// Despawns the active gameobjects and cleans up pools.
        /// </summary>
        public void Despawn()
        {
            // We do a bunch of other stuff on the same frame, so we really want it to be immediately destroyed, not at Unity's convenience.
            Model.SetActive(false);
            Destroy(Model);
            TrailPool.Clear();
            if (BulletPool != null)
            {
                BulletPool.Clear();
            }

            ShootingAudioSource = null;
            ShootSystem = null;
        }

        /// <summary>
        /// Used to override the Camera provided in <see cref="Spawn(Transform, MonoBehaviour, Camera)"/>.
        /// Cameras are only required if 
        /// </summary>
        /// <param name="Camera"></param>
        public void UpdateCamera(Camera Camera)
        {
            ActiveCamera = Camera;
        }

        /// <summary>
        /// Expected to be called every frame
        /// </summary>
        /// <param name="WantsToShoot">Whether or not the player is trying to shoot</param>
        public void Tick(bool WantsToShoot)
        {
            Model.transform.localRotation = Quaternion.Lerp(
                Model.transform.localRotation,
                Quaternion.Euler(SpawnRotation),
                Time.deltaTime * ShootConfig.RecoilRecoverySpeed
            );

            if (WantsToShoot)
            {
                LastFrameWantedToShoot = true;
                TryToShoot();
            }

            if (!WantsToShoot && LastFrameWantedToShoot)
            {
                StopShootingTime = Time.time;
                LastFrameWantedToShoot = false;
            }
        }

        /// <summary>
        /// Plays the reloading audio clip if assigned.
        /// Expected to be called on the first frame that reloading begins
        /// </summary>
        public void StartReloading()
        {
            AudioConfig.PlayReloadClip(ShootingAudioSource);
        }

        /// <summary>
        /// Handle ammo after a reload animation.
        /// ScriptableObjects can't catch Animation Events, which is how we're determining when the
        /// reload has completed, instead of using a timer
        /// </summary>
        public void EndReload()
        {
            AmmoConfig.Reload();
        }

        /// <summary>
        /// Whether or not this gun can be reloaded
        /// </summary>
        /// <returns>Whether or not this gun can be reloaded</returns>
        public bool CanReload()
        {
            return AmmoConfig.CanReload();
        }

        /// <summary>
        /// Performs the shooting raycast if possible based on gun rate of fire. Also applies bullet spread and plays sound effects based on the AudioConfig.
        /// </summary>
        private void TryToShoot()
        {
            if (Time.time - LastShootTime - ShootConfig.FireRate > Time.deltaTime)
            {
                float lastDuration = Mathf.Clamp(
                    0,
                    (StopShootingTime - InitialClickTime),
                    ShootConfig.MaxSpreadTime
                );
                float lerpTime = (ShootConfig.RecoilRecoverySpeed - (Time.time - StopShootingTime))
                    / ShootConfig.RecoilRecoverySpeed;

                InitialClickTime = Time.time - Mathf.Lerp(0, lastDuration, Mathf.Clamp01(lerpTime));
            }

            if (Time.time > ShootConfig.FireRate + LastShootTime)
            {
                LastShootTime = Time.time;
                if (AmmoConfig.CurrentClipAmmo == 0)
                {
                    AudioConfig.PlayOutOfAmmoClip(ShootingAudioSource);
                    return;
                }

                ShootSystem.Play();
                AudioConfig.PlayShootingClip(ShootingAudioSource, AmmoConfig.CurrentClipAmmo == 1);

                Vector3 spreadAmount = ShootConfig.GetSpread(Time.time - InitialClickTime);

                Vector3 shootDirection = Vector3.zero;
                Model.transform.forward += Model.transform.TransformDirection(spreadAmount);
                if (ShootConfig.ShootType == ShootType.FromGun)
                {
                    shootDirection = ShootSystem.transform.forward;
                }
                else
                {
                    shootDirection = ActiveCamera.transform.forward + ActiveCamera.transform.TransformDirection(spreadAmount);
                }

                AmmoConfig.CurrentClipAmmo--;

                if (ShootConfig.IsHitscan)
                {
                    DoHitscanShoot(shootDirection);
                }
                else
                {
                    DoProjectileShoot(shootDirection);
                }
            }
        }

        /// <summary>
        /// Generates a live Bullet instance that is launched in the <paramref name="ShootDirection"/> direction
        /// with velocity from <see cref="ShootConfigScriptableObject.BulletSpawnForce"/>.
        /// </summary>
        /// <param name="ShootDirection"></param>
        private void DoProjectileShoot(Vector3 ShootDirection)
        {
            Bullet bullet = BulletPool.Get();
            bullet.gameObject.SetActive(true);
            bullet.OnCollsion += HandleBulletCollision;

            // We have to ensure if shooting from the camera, but shooting real proejctiles, that we aim the gun at the hit point
            // of the raycast from the camera. Otherwise the aim is off.
            // When shooting from the gun, there's no need to do any of this because the recoil is already handled in TryToShoot
            if (ShootConfig.ShootType == ShootType.FromCamera
                && Physics.Raycast(
                    GetRaycastOrigin(),
                    ShootDirection,
                    out RaycastHit hit,
                    float.MaxValue,
                    ShootConfig.HitMask
                ))
            {
                Vector3 directionToHit = (hit.point - ShootSystem.transform.position).normalized;
                Model.transform.forward = directionToHit;
                ShootDirection = directionToHit;
            }

            bullet.transform.position = ShootSystem.transform.position;
            bullet.Spawn(ShootDirection * ShootConfig.BulletSpawnForce);

            TrailRenderer trail = TrailPool.Get();
            if (trail != null)
            {
                trail.transform.SetParent(bullet.transform, false);
                trail.transform.localPosition = Vector3.zero;
                trail.emitting = true;
                trail.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Performs a Raycast to determine if a shot hits something. Spawns a TrailRenderer
        /// and will apply impact effects and damage after the TrailRenderer simulates moving to the
        /// hit point. 
        /// See <see cref="PlayTrail(Vector3, Vector3, RaycastHit)"/> for impact logic.
        /// </summary>
        /// <param name="ShootDirection"></param>
        private void DoHitscanShoot(Vector3 ShootDirection)
        {
            if (Physics.Raycast(
                    GetRaycastOrigin(),
                    ShootDirection,
                    out RaycastHit hit,
                    float.MaxValue,
                    ShootConfig.HitMask
                ))
            {
                ActiveMonoBehaviour.StartCoroutine(
                    PlayTrail(
                        ShootSystem.transform.position,
                        hit.point,
                        hit
                    )
                );
            }
            else
            {
                ActiveMonoBehaviour.StartCoroutine(
                    PlayTrail(
                        ShootSystem.transform.position,
                        ShootSystem.transform.position + (ShootDirection * TrailConfig.MissDistance),
                        new RaycastHit()
                    )
                );
            }
        }

        /// <summary>
        /// Returns the proper Origin point for raycasting based on <see cref="ShootConfigScriptableObject.ShootType"/>
        /// </summary>
        /// <returns></returns>
        public Vector3 GetRaycastOrigin()
        {
            Vector3 origin = ShootSystem.transform.position;

            if (ShootConfig.ShootType == ShootType.FromCamera)
            {
                origin = ActiveCamera.transform.position
                    + ActiveCamera.transform.forward * Vector3.Distance(
                            ActiveCamera.transform.position,
                            ShootSystem.transform.position
                        );
            }

            return origin;
        }

        /// <summary>
        /// Returns the forward of the spawned gun model
        /// </summary>
        /// <returns></returns>
        public Vector3 GetGunForward()
        {
            return Model.transform.forward;
        }

        /// <summary>
        /// Plays a bullet trail/tracer from start/end point. 
        /// If <paramref name="Hit"/> is not an empty hit, it will also play an impact using the <see cref="SurfaceManager"/>.
        /// </summary>
        /// <param name="StartPoint">Starting point for the trail</param>
        /// <param name="EndPoint">Ending point for the trail</param>
        /// <param name="Hit">The hit object. If nothing is hit, simply pass new RaycastHit()</param>
        /// <returns>Coroutine</returns>
        private IEnumerator PlayTrail(Vector3 StartPoint, Vector3 EndPoint, RaycastHit Hit)
        {
            TrailRenderer instance = TrailPool.Get();
            instance.gameObject.SetActive(true);
            instance.transform.position = StartPoint;
            yield return null; // avoid position carry-over from last frame if reused

            instance.emitting = true;

            float distance = Vector3.Distance(StartPoint, EndPoint);
            float remainingDistance = distance;
            while (remainingDistance > 0)
            {
                instance.transform.position = Vector3.Lerp(
                    StartPoint,
                    EndPoint,
                    Mathf.Clamp01(1 - (remainingDistance / distance))
                );
                remainingDistance -= TrailConfig.SimulationSpeed * Time.deltaTime;

                yield return null;
            }

            instance.transform.position = EndPoint;

            if (Hit.collider != null)
            {
                HandleBulletImpact(distance, EndPoint, Hit.normal, Hit.collider);
            }

            yield return new WaitForSeconds(TrailConfig.Duration);
            yield return null;
            instance.emitting = false;
            instance.gameObject.SetActive(false);
            TrailPool.Release(instance);
        }

        /// <summary>
        /// Callback handler for <see cref="Bullet.OnCollsion"/>. Disables TrailRenderer, releases the 
        /// Bullet from the BulletPool, and applies impact effects if <paramref name="Collision"/> is not null.
        /// </summary>
        /// <param name="Bullet"></param>
        /// <param name="Collision"></param>
        private void HandleBulletCollision(Bullet Bullet, Collision Collision)
        {
            TrailRenderer trail = Bullet.GetComponentInChildren<TrailRenderer>();
            if (trail != null)
            {
                trail.transform.SetParent(null, true);
                ActiveMonoBehaviour.StartCoroutine(DelayedDisableTrail(trail));
            }

            Bullet.gameObject.SetActive(false);
            BulletPool.Release(Bullet);

            if (Collision != null)
            {
                ContactPoint contactPoint = Collision.GetContact(0);

                HandleBulletImpact(
                    Vector3.Distance(contactPoint.point, Bullet.SpawnLocation),
                    contactPoint.point,
                    contactPoint.normal,
                    contactPoint.otherCollider
                );
            }
        }

        /// <summary>
        /// Disables the trail renderer based on the <see cref="TrailConfigScriptableObject.Duration"/> provided
        ///and releases it from the<see cref="TrailPool"/>
        /// </summary>
        /// <param name="Trail"></param>
        /// <returns></returns>
        private IEnumerator DelayedDisableTrail(TrailRenderer Trail)
        {
            yield return new WaitForSeconds(TrailConfig.Duration);
            yield return null;
            Trail.emitting = false;
            Trail.gameObject.SetActive(false);
            TrailPool.Release(Trail);
        }

        /// <summary>
        /// Calls <see cref="SurfaceManager.HandleImpact(GameObject, Vector3, Vector3, ImpactType, int)"/> and applies damage
        /// if a damagable object was hit
        /// </summary>
        /// <param name="DistanceTraveled"></param>
        /// <param name="HitLocation"></param>
        /// <param name="HitNormal"></param>
        /// <param name="HitCollider"></param>
        private void HandleBulletImpact(
            float DistanceTraveled,
            Vector3 HitLocation,
            Vector3 HitNormal,
            Collider HitCollider)
        {
            SurfaceManager.Instance.HandleImpact(
                    HitCollider.gameObject,
                    HitLocation,
                    HitNormal,
                    ImpactType,
                    0
                );

            if (HitCollider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(DamageConfig.GetDamage(DistanceTraveled));
            }

            foreach (ICollisionHandler collisionHandler in BulletImpactEffects)
            {
                collisionHandler.HandleImpact(HitCollider, HitLocation, HitNormal, this);
            }
        }

        /// <summary>
        /// Creates a trail Renderer for use in the object pool.
        /// </summary>
        /// <returns>A live TrailRenderer GameObject</returns>
        private TrailRenderer CreateTrail()
        {
            GameObject instance = new GameObject("Bullet Trail");
            TrailRenderer trail = instance.AddComponent<TrailRenderer>();
            trail.colorGradient = TrailConfig.Color;
            trail.material = TrailConfig.Material;
            trail.widthCurve = TrailConfig.WidthCurve;
            trail.time = TrailConfig.Duration;
            trail.minVertexDistance = TrailConfig.MinVertexDistance;

            trail.emitting = false;
            trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            return trail;
        }

        /// <summary>
        /// Creates a Bullet for use in the object pool.
        /// </summary>
        /// <returns>A live Bullet GameObject</returns>
        private Bullet CreateBullet()
        {
            Bullet bullet = Instantiate(ShootConfig.BulletPrefab);
            Rigidbody rigidbody = bullet.GetComponent<Rigidbody>();
            rigidbody.mass = ShootConfig.BulletWeight;

            return bullet;
        }

        public object Clone()
        {
            GunScriptableObject config = CreateInstance<GunScriptableObject>();

            config.ImpactType = ImpactType;
            config.Type = Type;
            config.Name = Name;
            config.name = name;
            config.DamageConfig = DamageConfig.Clone() as DamageConfigScriptableObject;
            config.ShootConfig = ShootConfig.Clone() as ShootConfigScriptableObject;
            config.AmmoConfig = AmmoConfig.Clone() as AmmoConfigScriptableObject;
            config.TrailConfig = TrailConfig.Clone() as TrailConfigScriptableObject;
            config.AudioConfig = AudioConfig.Clone() as AudioConfigScriptableObject;

            config.ModelPrefab = ModelPrefab;
            config.SpawnPoint = SpawnPoint;
            config.SpawnRotation = SpawnRotation;

            return config;
        }
    }
}