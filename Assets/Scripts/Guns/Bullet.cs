using System.Collections;
using UnityEngine;

namespace LlamAcademy.Guns
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoBehaviour
    {
        private Rigidbody Rigidbody;
        [field: SerializeField]
        public Vector3 SpawnLocation
        {
            get; private set;
        }

        public delegate void CollisionEvent(Bullet Bullet, Collision Collision);
        public event CollisionEvent OnCollsion;

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        public void Spawn(Vector3 SpawnForce)
        {
            SpawnLocation = transform.position;
            transform.forward = SpawnForce.normalized;
            Rigidbody.AddForce(SpawnForce);
            StartCoroutine(DelayedDisable(2));
        }

        private IEnumerator DelayedDisable(float Time)
        {
            yield return new WaitForSeconds(Time);
            OnCollisionEnter(null);
        }

        private void OnCollisionEnter(Collision collision)
        {
            OnCollsion?.Invoke(this, collision);
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            Rigidbody.velocity = Vector3.zero;
            Rigidbody.angularVelocity = Vector3.zero;
            OnCollsion = null;
        }
    }
}