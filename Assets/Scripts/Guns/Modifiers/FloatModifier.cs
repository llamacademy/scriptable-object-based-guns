using System.Reflection;

namespace LlamAcademy.Guns.Modifiers
{
    public class FloatModifier : AbstractValueModifier<float>
    {
        public FloatModifier(): base() { }
        public FloatModifier(float Amount, string AttributeName): base(Amount, AttributeName) { }
        public override void Apply(GunScriptableObject Gun)
        {
            float value = GetAttribute<float>(Gun, out object targetObject, out FieldInfo field);
            value *= Amount;
            field.SetValue(targetObject, value);
        }
    }
}
