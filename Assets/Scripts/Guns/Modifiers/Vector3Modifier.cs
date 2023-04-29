using System.Reflection;
using UnityEngine;

namespace LlamAcademy.Guns.Modifiers
{
    public class Vector3Modifier : AbstractValueModifier<Vector3>
    {
        public Vector3Modifier(): base() { }
        public Vector3Modifier(Vector3 Amount, string AttributeName): base(Amount, AttributeName) { }

        public override void Apply(GunScriptableObject Gun)
        {
            try
            {
                Vector3 value = GetAttribute<Vector3>(
                    Gun, 
                    out object targetObject, 
                    out FieldInfo field
                );
                value = new(
                    value.x * Amount.x, 
                    value.y * Amount.y, 
                    value.z * Amount.z
                );
                field.SetValue(targetObject, value);
            }
            catch (InvalidPathSpecifiedException) { } // just log errors, don't kill modifier process
        }
    }
}
