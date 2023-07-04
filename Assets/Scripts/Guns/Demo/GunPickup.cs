using UnityEngine;

namespace LlamAcademy.Guns.Demo
{
    [RequireComponent(typeof(Collider))]
    public class GunPickup : MonoBehaviour
    {
        public GunScriptableObject Gun;
        public Vector3 SpinDirection = Vector3.up;

        private void Update()
        {
            transform.Rotate(SpinDirection);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerGunSelector gunSelector))
            {
                gunSelector.PickupGun(Gun);
                Destroy(gameObject);
            }
        }
    }
}
