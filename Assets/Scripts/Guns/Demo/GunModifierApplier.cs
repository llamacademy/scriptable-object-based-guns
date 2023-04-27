using LlamAcademy.Guns.ImpactEffects;
using LlamAcademy.Guns.Modifiers;
using LlamAcademy.ImpactSystem;
using UnityEngine;

namespace LlamAcademy.Guns.Demo
{
    public class GunModifierApplier : MonoBehaviour
    {
        [SerializeField]
        private ImpactType ImpactTypeOverride;
        [SerializeField]
        private PlayerGunSelector GunSelector;

        private void Start()
        {
            new ImpactTypeModifier()
            {
                Amount = ImpactTypeOverride
            }.Apply(GunSelector.ActiveGun);

            GunSelector.ActiveGun.BulletImpactEffects = new ICollisionHandler[]
            {
                new Frost(
                    1.5f,
                    new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 0.25f)}),
                    10,
                    10,
                    new AnimationCurve(new Keyframe[] { 
                        new Keyframe(0, 0.25f), 
                        new Keyframe(1.75f, 0.25f), 
                        new Keyframe(2, 1)
                    })
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
            //        GunSelector.ActiveGun.DamageConfig.DamageCurve.Evaluate(0),
            //        10
            //    )
            //};
        }
    }
}