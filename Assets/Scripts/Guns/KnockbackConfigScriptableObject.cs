using UnityEngine;

namespace LlamAcademy.Guns
{
    [CreateAssetMenu(fileName = "KnockbackConfig Config", menuName = "Guns/Knockback Config", order = 8)]
    public class KnockbackConfigScriptableObject : ScriptableObject, System.ICloneable
    {
        public float KnockbackStrength = 25000;
        public ParticleSystem.MinMaxCurve DistanceFalloff;
        public float MaximumKnockbackTime = 1;

        public Vector3 GetKnockbackStrength(Vector3 Direction, float Distance)
        {
            return KnockbackStrength * DistanceFalloff.Evaluate(Distance) * Direction;
        }

        public object Clone()
        {
            KnockbackConfigScriptableObject clone = CreateInstance<KnockbackConfigScriptableObject>();

            Utilities.CopyValues(this, clone);

            return clone;
        }
    }
}