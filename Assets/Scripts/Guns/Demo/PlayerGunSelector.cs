using LlamAcademy.Guns.Modifiers;
using System.Collections.Generic;
using UnityEngine;

namespace LlamAcademy.Guns.Demo
{
    [DisallowMultipleComponent]
    public class PlayerGunSelector : MonoBehaviour
    {
        public Camera Camera;
        [SerializeField]
        private GunType Gun;
        [SerializeField]
        private Transform GunParent;
        [SerializeField]
        private List<GunScriptableObject> Guns;
        [SerializeField]
        private PlayerIK InverseKinematics;

        [Space]
        [Header("Runtime Filled")]
        public GunScriptableObject ActiveGun;
        [SerializeField]
        private GunScriptableObject ActiveBaseGun;

        private void Start()
        {
            GunScriptableObject gun = Guns.Find(gun => gun.Type == Gun);

            if (gun == null)
            {
                Debug.LogError($"No GunScriptableObject found for GunType: {gun}");
                return;
            }

            SetupGun(gun);
        }

        private void SetupGun(GunScriptableObject Gun)
        {
            ActiveBaseGun = Gun;
            ActiveGun = Gun.Clone() as GunScriptableObject;
            ActiveGun.Spawn(GunParent, this, Camera);

            InverseKinematics.SetGunStyle(ActiveGun.Type == GunType.Glock);
            InverseKinematics.Setup(GunParent);
        }

        public void DespawnActiveGun()
        {
            ActiveGun.Despawn();
            Destroy(ActiveGun);
        }

        public void PickupGun(GunScriptableObject Gun)
        {
            DespawnActiveGun();
            SetupGun(Gun);
        }

        public void DespawnActiveGun()
        {
            DespawnActiveGun();
            SetupGun(ActiveBaseGun);

            foreach (IModifier modifier in Modifiers)
            {
                modifier.Apply(ActiveGun);    
            }
        }
    }
}
