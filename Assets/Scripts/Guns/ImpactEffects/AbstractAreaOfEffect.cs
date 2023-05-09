using LlamAcademy.Guns.ImpactEffects;
using UnityEngine;

namespace LlamAcademy.Guns
{
    public abstract class AbstractAreaOfEffect : ICollisionHandler
    {
        public float Radius = 1;
        public AnimationCurve DamageFalloff;
        public int BaseDamage = 10;
        public int MaxEnemiesAffected = 10;

        protected Collider[] HitObjects;
        protected int Hits;

        public AbstractAreaOfEffect(float Radius, AnimationCurve DamageFalloff, int BaseDamage, int MaxEnemiesAffected)
        {
            this.Radius = Radius;
            this.DamageFalloff = DamageFalloff;
            this.BaseDamage = BaseDamage;
            this.MaxEnemiesAffected = MaxEnemiesAffected;
            HitObjects = new Collider[MaxEnemiesAffected];
        }

        public virtual void HandleImpact(Collider ImpactedObject, Vector3 HitPosition, Vector3 HitNormal, GunScriptableObject Gun)
        {
            Hits = Physics.OverlapSphereNonAlloc(
                HitPosition,
                Radius,
                HitObjects,
                Gun.ShootConfig.HitMask
            );
            for (int i = 0; i < Hits; i++)
            {
                if (HitObjects[i].TryGetComponent(out IDamageable damageable))
                {
                    float distance = Vector3.Distance(HitPosition, HitObjects[i].ClosestPoint(HitPosition));

                    damageable.TakeDamage(
                        Mathf.CeilToInt(BaseDamage * DamageFalloff.Evaluate(distance / Radius))
                    );
                }
            }
        }
    }
}
