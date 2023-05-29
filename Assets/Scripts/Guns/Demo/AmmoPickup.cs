using UnityEngine;

namespace LlamAcademy.Guns.Demo
{
    [RequireComponent(typeof(Collider))]
    public class AmmoPickup : MonoBehaviour
    {
        public int AmmoAmount = 30;
        public GunType Type;
        public Vector3 SpinDirection = Vector3.up;

        private void Update()
        {
            transform.Rotate(SpinDirection);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerGunSelector gunSelector) && gunSelector.ActiveGun.Type == Type)
            {
                gunSelector.ActiveGun.AmmoConfig.AddAmmo(AmmoAmount);
                Destroy(gameObject);
            }
        }
    }
}
