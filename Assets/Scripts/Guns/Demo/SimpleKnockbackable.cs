using UnityEngine;

namespace LlamAcademy.Guns.Demo
{
    [RequireComponent(typeof(Rigidbody))]
    public class SimpleKnockbackable : MonoBehaviour, IKnockbackable
    {
        [field: SerializeField] public float StillThreshold { get; set; }

        private Rigidbody Rigidbody;
        
        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        public void GetKnockedBack(Vector3 force, float maxMoveTime)
        {
            Rigidbody.AddForce(force);
        }
    }
}