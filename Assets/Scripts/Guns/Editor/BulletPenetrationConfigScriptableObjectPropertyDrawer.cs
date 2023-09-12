using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LlamAcademy.Guns.Editors
{
    [CustomPropertyDrawer(typeof(BulletPenetrationConfigScriptableObject), true)]
    public class BulletPenetrationConfigScriptableObjectPropertyDrawer : PropertyDrawer
    {
        private VisualElement PropertyContainer;

        public override VisualElement CreatePropertyGUI(SerializedProperty Property)
        {
            ScriptableObjectCreator<BulletPenetrationConfigScriptableObject> creator = new(Property,
                "Bullet Penetration Configuration", GunScriptableObjectEditor.GUN_ASSET_PATH + "Bullet Pen/");

            VisualElement root = new VisualElement();
            root.AddToClassList("panel");

            VisualElement contentPanel = creator.contentContainer;

            if (Property.objectReferenceValue != null)
            {
                creator.contentContainer.Add(BuildPropertyUI(Property));
            }

            creator.RegisterCallback<ChangeEvent<Object>>((changeEvent) =>
                HandleSOChange(creator.contentContainer, Property, changeEvent));
            root.Add(creator);

            return root;
        }

        private VisualElement BuildPropertyUI(SerializedProperty Property)
        {
            PropertyContainer = new VisualElement();

            SerializedObject serializedObject = new(Property.objectReferenceValue);

            IntegerField maxObjectPenField = new("Max Objects to Penetrate");
            maxObjectPenField.BindProperty(serializedObject.FindProperty("MaxObjectsToPenetrate"));
            PropertyContainer.Add(maxObjectPenField);

            SerializedProperty penetrationDepthProperty = serializedObject.FindProperty("MaxPenetrationDepth");
            SliderWithInput penetrationDepthField = new(penetrationDepthProperty, "Penetration Depth", 0.01f, 3f);
            PropertyContainer.Add(penetrationDepthField);

            Vector3Field accuracyLossField = new("Accuracy Loss");
            accuracyLossField.BindProperty(serializedObject.FindProperty("AccuracyLoss"));
            PropertyContainer.Add(accuracyLossField);

            SerializedProperty penetrationDamageRetentionProperty =
                serializedObject.FindProperty("DamageRetentionPercentage");
            SliderWithInput penetrationDamageRetentionField = new(penetrationDamageRetentionProperty,
                "Percent Damage Retained Per Penetration", 0.01f, 1f);
            PropertyContainer.Add(penetrationDamageRetentionField);

            HelpBox helpBox =
                    new(
                        "For each object penetrated, the damage calculation will consider this number.\r\n" +
                        "A value of 1 means there is no damage loss regardless of the object penetrated. A value of 0 means all damage is lost.",
                        HelpBoxMessageType.Info)
                ;
            PropertyContainer.Add(helpBox);

            return PropertyContainer;
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
    }
}