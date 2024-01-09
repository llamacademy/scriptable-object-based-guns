using UnityEngine;

namespace LlamAcademy.Guns
{
    [RequireComponent(typeof(Collider))]
    public class KnockbackOnCollision : MonoBehaviour
    {
        [SerializeField] private float MaxMoveTime = 1f;
        private void OnCollisionEnter(Collision other)
        {
            if (other.transform.TryGetComponent(out IKnockbackable knockbackable))
            {
                knockbackable.GetKnockedBack(-other.impulse, MaxMoveTime);
            }
        }
    }
}