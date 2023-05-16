using System;
using System.Reflection;

namespace LlamAcademy.Guns.Modifiers
{
    /// <summary>
    /// Applies arbitrary percentage offsets to values of a provided type.
    /// </summary>
    /// <typeparam name="T">Type of the field to be modified</typeparam>
    public abstract class AbstractValueModifier<T> : IModifier
    {
        public AbstractValueModifier() { }
        public AbstractValueModifier(T Amount, string AttributeName)
        {
            this.AttributeName= AttributeName;
            this.Amount = Amount;
        }

        /// <summary>
        /// AttributeName can be a discrete field on the Gun, or provided as a path to a property.
        /// For example, to modifiy the damage curve, you can set the value to "DamageConfig/DamageCurve"
        /// Since the GunScriptableObject itself primarily is a container for other Configurations, in most cases
        /// it is expected that a path to a property will be provided.
        /// </summary>
        public string AttributeName;
        /// <summary>
        /// The amount (usually as a percentage offset) to be applied.
        /// </summary>
        public T Amount;

        /// <summary>
        /// Concrete implementations of this should apply some modification to the Gun or sub-configurations of the gun.
        /// </summary>
        /// <param name="Gun">Gun to be modified</param>
        public abstract void Apply(GunScriptableObject Gun);

        /// <summary>
        /// Attempts to return a reference or value specified at <see cref="AttributeName"/> using reflection.
        /// If the path does not exist, and <see cref="InvalidPathSpecifiedException"/> will be thrown and no out parameters will be populated.
        /// </summary>
        /// <typeparam name="FieldType">Type expected at the <see cref="AttributeName"/> path.</typeparam>
        /// <param name="Gun">Active instance of a gun to be searched</param>
        /// <param name="TargetObject">out parameter. The live object that houses the <see cref="AttributeName"/> that should have the value updated</param>
        /// <param name="Field">out parameter. The FieldInfo reflecting the value of type <typeparamref name="FieldType"/> at path <see cref="AttributeName"/>.</param>
        /// <returns></returns>
        /// <exception cref="InvalidPathSpecifiedException"></exception>
        protected FieldType GetAttribute<FieldType>(
            GunScriptableObject Gun,
            out object TargetObject,
            out FieldInfo Field)
        {
            string[] paths = AttributeName.Split("/");
            string attribute = paths[paths.Length - 1];

            Type type = Gun.GetType();
            object target = Gun;

            for (int i = 0; i < paths.Length - 1; i++)
            {
                FieldInfo field = type.GetField(paths[i]);
                if (field == null)
                {
                    UnityEngine.Debug.LogError($"Unable to apply modifier" +
                        $" to attribute {AttributeName} because it does not exist on gun {Gun}");
                    throw new InvalidPathSpecifiedException(AttributeName);
                }
                else
                {
                    target = field.GetValue(target);
                    type = target.GetType();
                }
            }

            FieldInfo attributeField = type.GetField(attribute);
            if (attributeField == null)
            {
                UnityEngine.Debug.LogError($"Unable to apply modifier to attribute " +
                    $"{AttributeName} because it does not exist on gun {Gun}");
                throw new InvalidPathSpecifiedException(AttributeName);
            }

            Field = attributeField;
            TargetObject = target;
            return (FieldType)attributeField.GetValue(target);
        }
    }
}
