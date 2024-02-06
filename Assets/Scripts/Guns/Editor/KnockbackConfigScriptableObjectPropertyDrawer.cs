using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LlamAcademy.Guns.Editors
{
    [CustomPropertyDrawer(typeof(KnockbackConfigScriptableObject), true)]
    public class KnockbackConfigScriptableObjectPropertyDrawer : PropertyDrawer
    {
        private VisualElement PropertyContainer;

        public override VisualElement CreatePropertyGUI(SerializedProperty Property)
        {
            ScriptableObjectCreator<KnockbackConfigScriptableObject> creator = new(Property, "Knockback Configuration",
                GunScriptableObjectEditor.GUN_ASSET_PATH + "Knockback/");

            VisualElement root = new();
            root.AddToClassList("panel");

            VisualElement contentPanel = creator.contentContainer;

            if (Property.objectReferenceValue != null)
            {
                contentPanel.Add(BuildPropertyUI(Property));
            }

            creator.RegisterCallback<ChangeEvent<Object>>((changeEvent) =>
                HandleSOChange(contentPanel, Property, changeEvent));
            root.Add(creator);

            return root;
        }

        private void HandleSOChange(VisualElement ContentRoot, SerializedProperty Property,
            ChangeEvent<Object> ChangeEvent)
        {
            if (ChangeEvent.newValue == null && PropertyContainer != null)
            {
                ContentRoot.Remove(PropertyContainer);
                PropertyContainer = null;
            }
            else if (ChangeEvent.newValue != null && PropertyContainer == null)
            {
                ContentRoot.Add(BuildPropertyUI(Property));
            }
        }

        private VisualElement BuildPropertyUI(SerializedProperty Property)
        {
            PropertyContainer = new();

            SerializedObject serializedObject = new(Property.objectReferenceValue);
            FloatField strengthField = new("Knockback Strength");
            strengthField.BindProperty(serializedObject.FindProperty("KnockbackStrength"));
            PropertyContainer.Add(strengthField);

            PropertyField falloffCurveField = new();
            falloffCurveField.label = "Falloff Curve";
            falloffCurveField.BindProperty(serializedObject.FindProperty("DistanceFalloff"));
            falloffCurveField.name = "falloff-curve";
            PropertyContainer.Add(falloffCurveField);

            FloatField maxDurationField = new("Max Knockback Duration");
            maxDurationField.BindProperty(serializedObject.FindProperty("MaximumKnockbackTime"));
            maxDurationField.name = "max-duration";
            PropertyContainer.Add(maxDurationField);

            return PropertyContainer;
        }
    }
}