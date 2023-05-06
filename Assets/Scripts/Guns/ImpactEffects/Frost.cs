using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlamAcademy.Guns.ImpactEffects
{
    public class Frost : AbstractAreaOfEffect
    {
        public AnimationCurve SlowDecay;

        public Frost(float Radius, AnimationCurve DamageFalloff, int BaseDamage, int MaxEnemiesAffected)
            : base(Radius, DamageFalloff, BaseDamage, MaxEnemiesAffected) { }
        public Frost(float Radius, AnimationCurve DamageFalloff, int BaseDamage, int MaxEnemiesAffected, AnimationCurve SlowDecay)
            : base(Radius, DamageFalloff, BaseDamage, MaxEnemiesAffected)
        {
            this.SlowDecay = SlowDecay;
        }

        public override void HandleImpact(Collider ImpactedObject, Vector3 HitPosition, Vector3 HitNormal, GunScriptableObject Gun)
        {
            base.HandleImpact(ImpactedObject, HitPosition, HitNormal, Gun);

            for (int i = 0; i < Hits; i++)
            {
                if (HitObjects[i].TryGetComponent(out ISlowable slowable))
                {
                    slowable.Slow(SlowDecay);
                }
            }
        }
    }
}
