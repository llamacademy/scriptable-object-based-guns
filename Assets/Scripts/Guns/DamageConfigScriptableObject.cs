using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace LlamAcademy.Guns
{
    [CreateAssetMenu(fileName = "Damage Config", menuName = "Guns/Damage Config", order = 1)]
    public class DamageConfigScriptableObject : ScriptableObject, System.ICloneable
    {
        public MinMaxCurve DamageCurve;

        private void Reset()
        {
            DamageCurve.mode = ParticleSystemCurveMode.Curve;
        }

        public int GetDamage(float Distance = 0)
        {
            return Mathf.CeilToInt(DamageCurve.Evaluate(Distance, Random.value));
        }

        public object Clone()
        {
            DamageConfigScriptableObject config = CreateInstance<DamageConfigScriptableObject>();

            config.DamageCurve = DamageCurve;
            return config;
        }
    }
}