using UnityEngine;

namespace LlamAcademy.Guns.ImpactEffects
{
    public class Explode : AbstractAreaOfEffect
    {
        public Explode(float Radius, AnimationCurve DamageFalloff, float BaseDamage, int MaxEnemiesAffected) : base(Radius, DamageFalloff, BaseDamage, MaxEnemiesAffected)
        {
        }
    }
}
