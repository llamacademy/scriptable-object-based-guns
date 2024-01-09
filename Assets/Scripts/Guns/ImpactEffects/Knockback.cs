using LlamAcademy.Guns.ImpactEffects;
using UnityEngine;

namespace LlamAcademy.Guns.Guns.ImpactEffects
{
    public class Knockback : ICollisionHandler
    {
        public void HandleImpact(
            Collider ImpactedObject,
            Vector3 HitPosition,
            Vector3 HitNormal,
            float DistanceTravelled,
            GunScriptableObject Gun)
        {
            if (ImpactedObject.TryGetComponent(out IKnockbackable knockbackable))
            {
                knockbackable.GetKnockedBack(
                    Gun.KnockbackConfig.GetKnockbackStrength(-HitNormal, DistanceTravelled),
                    Gun.KnockbackConfig.MaximumKnockbackTime
                );
            }
        }
    }
}