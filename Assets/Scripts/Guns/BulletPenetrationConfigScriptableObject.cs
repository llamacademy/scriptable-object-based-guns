using UnityEngine;

namespace LlamAcademy.Guns
{
    [CreateAssetMenu(fileName = "Bullet Penetration Config", menuName = "Guns/Bullet Penetration Config", order = 6)]
    public class BulletPenetrationConfigScriptableObject : ScriptableObject, System.ICloneable
    {
        public int MaxObjectsToPenetrate = 0;
        public float MaxPenetrationDepth = 0.275f;
        public Vector3 AccuracyLoss = new(0.1f, 0.1f, 0.1f);
        public float DamageRetentionPercentage;
        
        public object Clone()
        {
            BulletPenetrationConfigScriptableObject config = CreateInstance<BulletPenetrationConfigScriptableObject>();

            Utilities.CopyValues(this, config);

            return config;
        }
    }
}