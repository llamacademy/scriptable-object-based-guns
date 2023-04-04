using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LlamAcademy.Guns.Editors
{
    [CustomPropertyDrawer(typeof(DamageConfigScriptableObject), true)]
    public class DamageConfigScriptableObjectPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty Property)
        {
            ScriptableObjectCreator<DamageConfigScriptableObject> creator = new(Property, "Damage Configuration", GunScriptableObjectEditor.GUN_ASSET_PATH + "Damage/");

            VisualElement root = new VisualElement();
            root.AddToClassList("panel");

            VisualElement contentPanel = creator.contentContainer;

            if (Property.objectReferenceValue != null)
            {
                contentPanel.Add(BuildPropertyField(Property));
            }

            creator.RegisterCallback<ChangeEvent<Object>>((changeEvent) => HandleSOChange(contentPanel, Property, changeEvent));
            root.Add(creator);

            return root;
        }

        private void HandleSOChange(VisualElement ContentRoot, SerializedProperty Property, ChangeEvent<Object> ChangeEvent)
        {
            if (ChangeEvent.newValue == null)
            {
                VisualElement damageCurve = ContentRoot.Q("damage-curve");
                if (damageCurve != null)
                {
                    ContentRoot.Remove(damageCurve);
                }
            }
            else
            {
                VisualElement damageCurve = ContentRoot.Q("damage-curve");
                if (damageCurve == null)
                {
                    ContentRoot.Add(BuildPropertyField(Property));
                }
            }
        }

        private VisualElement BuildPropertyField(SerializedProperty Property)
        {
            SerializedObject serializedObject = new(Property.objectReferenceValue);
            PropertyField damageCurveField = new();
            damageCurveField.label = "Damage Curve";
            SerializedProperty property = serializedObject.FindProperty("DamageCurve");
            damageCurveField.BindProperty(property);
            damageCurveField.name = "damage-curve";

            return damageCurveField;
        }

    }
}
