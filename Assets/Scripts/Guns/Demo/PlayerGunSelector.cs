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

        private void Awake()
        {
            GunScriptableObject gun = Guns.Find(gun => gun.Type == Gun);

            if (gun == null)
            {
                Debug.LogError($"No GunScriptableObject found for GunType: {gun}");
                return;
            }

            ActiveGun = gun.Clone() as GunScriptableObject;
            ActiveGun.Spawn(GunParent, this, Camera);

            // some magic for IK
            Transform[] allChildren = GunParent.GetComponentsInChildren<Transform>();
            InverseKinematics.LeftElbowIKTarget = allChildren.FirstOrDefault(child => child.name == "LeftElbow");
            InverseKinematics.RightElbowIKTarget = allChildren.FirstOrDefault(child => child.name == "RightElbow");
            InverseKinematics.LeftHandIKTarget = allChildren.FirstOrDefault(child => child.name == "LeftHand");
            InverseKinematics.RightHandIKTarget = allChildren.FirstOrDefault(child => child.name == "RightHand");
        }
    }
}
