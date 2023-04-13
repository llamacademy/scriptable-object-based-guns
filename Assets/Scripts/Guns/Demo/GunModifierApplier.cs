using LlamAcademy.Guns.Modifiers;
using UnityEngine;

namespace LlamAcademy.Guns.Demo
{
    public class GunModifierApplier : MonoBehaviour
    {
        [SerializeField]
        private PlayerGunSelector GunSelector;

        private void Start()
        {
            DamageModifier damageModifier = new()
            {
                Amount = 1.5f,
                AttributeName = "DamageConfig/DamageCurve"
            };
            damageModifier.Apply(GunSelector.ActiveGun);

            Vector3Modifier spreadModifier = new()
            {
                Amount = new Vector3(1.2f, 1.2f, 1.2f),
                AttributeName = "ShootConfig/Spread"
            };
            spreadModifier.Apply(GunSelector.ActiveGun);
        }
    }
}