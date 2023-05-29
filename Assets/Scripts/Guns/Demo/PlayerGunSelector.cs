using LlamAcademy.Guns.Modifiers;
using System.Collections.Generic;
using System.Linq;
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
        private GunScriptableObject ActiveBaseGun;

        private void Awake()
        {
            GunScriptableObject gun = Guns.Find(gun => gun.Type == Gun);

            if (gun == null)
            {
                Debug.LogError($"No GunScriptableObject found for GunType: {gun}");
                return;
            }

            ActiveBaseGun = gun;
            SetupGun(ActiveBaseGun);
        }

        private void SetupGun(GunScriptableObject Gun)
        {
            ActiveGun = Gun.Clone() as GunScriptableObject;
            ActiveGun.Spawn(GunParent, this, Camera);

            // some magic for IK
            DoIKMagic();
        }

        private void DoIKMagic()
        {
            Transform[] allChildren = GunParent.GetComponentsInChildren<Transform>();
            InverseKinematics.LeftElbowIKTarget = allChildren.FirstOrDefault(child => child.name == "LeftElbow");
            InverseKinematics.RightElbowIKTarget = allChildren.FirstOrDefault(child => child.name == "RightElbow");
            InverseKinematics.LeftHandIKTarget = allChildren.FirstOrDefault(child => child.name == "LeftHand");
            InverseKinematics.RightHandIKTarget = allChildren.FirstOrDefault(child => child.name == "RightHand");
        }

        public void DespawnActiveGun()
        {
            ActiveGun.Despawn();
            Destroy(ActiveGun);
        }

        public void PickupGun(GunScriptableObject Gun)
        {
            DespawnActiveGun();
            ActiveBaseGun = Gun;
            SetupGun(ActiveBaseGun);
        }

        public void ApplyModifiers(IModifier[] Modifiers)
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
