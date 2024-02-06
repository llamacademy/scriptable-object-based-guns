using UnityEngine;

namespace LlamAcademy.Guns
{
    public interface IKnockbackable
    {
        public float StillThreshold { get; set; }
        void GetKnockedBack(Vector3 force, float maxMoveTime);
    }
}