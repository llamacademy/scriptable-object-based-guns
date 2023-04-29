using LlamAcademy.ImpactSystem;

namespace LlamAcademy.Guns.Modifiers
{
    public class ImpactTypeModifier : AbstractValueModifier<ImpactType>
    {
        public ImpactTypeModifier(): base() { }
        public ImpactTypeModifier(ImpactType Type, string AttributeName): base(Type, AttributeName) { }

        public override void Apply(GunScriptableObject Gun)
        {
            Gun.ImpactType = Amount;
        }
    }
}
