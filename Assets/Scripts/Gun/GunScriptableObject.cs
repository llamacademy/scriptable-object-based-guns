using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "Gun", menuName = "Guns/Gun", order = 0)]
public class GunScriptableObject : ScriptableObject
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

    private MonoBehaviour ActiveMonoBehaviour;
    private GameObject Model;
    private float LastShootTime;
    private float InitialClickTime;
    private float StopShootingTime;

    private ParticleSystem ShootSystem;
    private ObjectPool<TrailRenderer> TrailPool;
    private bool LastFrameWantedToShoot;

    /// <summary>
    /// Spawns the Gun Model into the scene
    /// </summary>
    /// <param name="Parent">Parent for the gun model</param>
    /// <param name="ActiveMonoBehaviour">An Active MonoBehaviour that can have Coroutines attached to them.
    /// The input handling script is a good candidate for this.
    /// </param>
    public void Spawn(Transform Parent, MonoBehaviour ActiveMonoBehaviour)
    {
        this.ActiveMonoBehaviour = ActiveMonoBehaviour;
        
        // in editor these will not be properly reset, in build it's fine
        LastShootTime = 0; 
        StopShootingTime = 0;
        InitialClickTime = 0;

        TrailPool = new ObjectPool<TrailRenderer>(CreateTrail);

        Model = Instantiate(ModelPrefab);
        Model.transform.SetParent(Parent, false);
        Model.transform.localPosition = SpawnPoint;
        Model.transform.localRotation = Quaternion.Euler(SpawnRotation);

        ShootSystem = Model.GetComponentInChildren<ParticleSystem>();
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
            if (AmmoConfig.CurrentClipAmmo > 0)
            {
                Shoot();
            }
        }

        if (!WantsToShoot && LastFrameWantedToShoot)
        {
            StopShootingTime = Time.time;
            LastFrameWantedToShoot = false;
        }
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
    /// Performs the shooting raycast if possible based on gun rate of fire. Also applies bullet spread.
    /// </summary>
    private void Shoot()
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
            ShootSystem.Play();

            Vector3 spreadAmount = ShootConfig.GetSpread(Time.time - InitialClickTime);
            Model.transform.forward += Model.transform.TransformDirection(spreadAmount);
            
            Vector3 shootDirection = ShootSystem.transform.forward;

            AmmoConfig.CurrentClipAmmo--;

            if (Physics.Raycast(
                    ShootSystem.transform.position,
                    shootDirection,
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
                        ShootSystem.transform.position + (shootDirection * TrailConfig.MissDistance),
                        new RaycastHit()
                    )
                );
            }
        }
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
            SurfaceManager.Instance.HandleImpact(
                Hit.transform.gameObject,
                EndPoint,
                Hit.normal,
                ImpactType,
                0
            );

            if (Hit.collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(DamageConfig.GetDamage(distance));
            }
        }

        yield return new WaitForSeconds(TrailConfig.Duration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        TrailPool.Release(instance);
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
}
