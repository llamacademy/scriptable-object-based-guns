using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace LlamAcademy.Guns.Demo
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class PlayerIK : MonoBehaviour
    {
        public Transform LeftHandIKTarget;
        public Transform RightHandIKTarget;
        public Transform LeftElbowIKTarget;
        public Transform RightElbowIKTarget;

        [Range(0, 1f)]
        public float HandIKAmount = 1f;
        [Range(0, 1f)]
        public float ElbowIKAmount = 1f;

        [SerializeField]
        private Animator Animator;

        private void Awake()
        {
            Animator = GetComponent<Animator>();
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (LeftHandIKTarget != null)
            {
                Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, HandIKAmount);
                Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, HandIKAmount);
                Animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandIKTarget.position);
                Animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandIKTarget.rotation);
            }
            if (RightHandIKTarget != null)
            {
                Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, HandIKAmount);
                Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, HandIKAmount);
                Animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandIKTarget.rotation);
                Animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandIKTarget.position);
            }
            if (LeftElbowIKTarget != null)
            {
                Animator.SetIKHintPosition(AvatarIKHint.LeftElbow, LeftElbowIKTarget.position);
                Animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, ElbowIKAmount);
            }

            if (RightElbowIKTarget != null)
            {
                Animator.SetIKHintPosition(AvatarIKHint.RightElbow, RightElbowIKTarget.position);
                Animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, ElbowIKAmount);
            }
        }

        public void SetGunStyle(bool OneHanded)
        {
            Animator.SetBool("Is2HandedGun", !OneHanded);
            Animator.SetBool("Is1HandedGun", OneHanded);
        }

        public void Setup(Transform GunParent)
        {
            Transform[] allChildren = GunParent.GetComponentsInChildren<Transform>();
            LeftElbowIKTarget = allChildren.FirstOrDefault(child => child.name == "LeftElbow");
            RightElbowIKTarget = allChildren.FirstOrDefault(child => child.name == "RightElbow");
            LeftHandIKTarget = allChildren.FirstOrDefault(child => child.name == "LeftHand");
            RightHandIKTarget = allChildren.FirstOrDefault(child => child.name == "RightHand");
        }
    }
}