using LlamAcademy.ImpactSystem;

namespace LlamAcademy.Guns.Modifiers
{
    public class ImpactTypeModifier : AbstractValueModifier<ImpactType>
    {
        public override void Apply(GunScriptableObject Gun)
        {
            Gun.ImpactType = Amount;
        }
    }
}
