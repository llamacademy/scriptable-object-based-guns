using LlamAcademy.Guns.ImpactEffects;
using LlamAcademy.Guns.Modifiers;
using LlamAcademy.ImpactSystem;
using UnityEngine;

namespace LlamAcademy.Guns.Demo
{
    public class GunModifierApplier : MonoBehaviour
    {
        [SerializeField]
        private ImpactType ImpactType;
        [SerializeField]
        private PlayerGunSelector GunSelector;

        private void Start()
        {
            new ImpactTypeModifier()
            {
                Amount = ImpactType
            }.Apply(GunSelector.ActiveGun);

            GunSelector.ActiveGun.BulletImpactEffects = new ICollisionHandler[]
            {
                new Frost(
                    1,
                    new AnimationCurve() {
                        keys = new Keyframe[] {
                            new Keyframe(0, 1),
                            new Keyframe(1, 0.25f)
                        }
                    },
                    5,
                    10,
                     new AnimationCurve() {
                        keys = new Keyframe[] {
                            new Keyframe(0, 0.5f),
                            new Keyframe(1.75f, 0.5f),
                            new Keyframe(2, 1),
                        }
                    }
                )
            };
            //GunSelector.ActiveGun.BulletImpactEffects = new ICollisionHandler[]
            //{
            //    new Explode(
            //        1,
            //        new AnimationCurve() {
            //            keys = new Keyframe[] {
            //                new Keyframe(0, 1),
            //                new Keyframe(1, 0.25f)
            //            }
            //        },
            //        10,
            //        10
            //    )
            //};
        }
    }
}