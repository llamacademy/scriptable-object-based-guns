using System.Collections;
using UnityEngine;

namespace LlamAcademy.Guns
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoBehaviour
    {
        private int ObjectsPenetrated;

        public Rigidbody Rigidbody { get; private set; }
        [field: SerializeField] public Vector3 SpawnLocation { get; private set; }

        [field: SerializeField] public Vector3 SpawnVelocity { get; private set; }

        public delegate void CollisionEvent(Bullet Bullet, Collision Collision, int ObjectsPenetrated);

        public event CollisionEvent OnCollision;

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        public void Spawn(Vector3 SpawnForce)
        {
            ObjectsPenetrated = 0;
            SpawnLocation = transform.position;
            transform.forward = SpawnForce.normalized;
            Rigidbody.AddForce(SpawnForce);
            SpawnVelocity = SpawnForce * Time.fixedDeltaTime / Rigidbody.mass;
            StartCoroutine(DelayedDisable(2));
        }

        private IEnumerator DelayedDisable(float Time)
        {
            yield return null;
            yield return new WaitForSeconds(Time);
            OnCollisionEnter(null);
        }

        private void OnCollisionEnter(Collision collision)
        {
            OnCollision?.Invoke(this, collision, ObjectsPenetrated);
            ObjectsPenetrated++;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            Rigidbody.velocity = Vector3.zero;
            Rigidbody.angularVelocity = Vector3.zero;
            OnCollision = null;
        }
    }
}