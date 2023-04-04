using LlamAcademy.Guns.Editors;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LlamAcademy.Guns
{
    [CustomPropertyDrawer(typeof(AudioConfigScriptableObject), true)]
    public class AudioConfigScriptableObjectPropertyDrawer : PropertyDrawer
    {
        private VisualElement PropertyContainer;

        public override VisualElement CreatePropertyGUI(SerializedProperty Property)
        {
            ScriptableObjectCreator<AudioConfigScriptableObject> creator = new(Property, "Audio Configuration", GunScriptableObjectEditor.GUN_ASSET_PATH + "Audio/");

            VisualElement root = new();
            root.AddToClassList("panel");

            if (Property.objectReferenceValue != null)
            {
                creator.contentContainer.Add(BuildPropertyUI(Property));
            }

            creator.RegisterCallback<ChangeEvent<Object>>((changeEvent) => HandleSOChange(creator.contentContainer, Property, changeEvent));
            root.Add(creator);

            return root;
        }

        private VisualElement BuildPropertyUI(SerializedProperty Property)
        {
            PropertyContainer = new VisualElement();
            SerializedObject serializedObject = new(Property.objectReferenceValue);

            SliderWithInput volumeSlider = new(serializedObject.FindProperty("Volume"), "Volume", 0, 1);
            PropertyContainer.Add(volumeSlider);

            PropertyField fireClips = new();
            fireClips.AddToClassList("pl-16");
            fireClips.label = "Shooting Clips";
            fireClips.BindProperty(serializedObject.FindProperty("FireClips"));
            PropertyContainer.Add(fireClips);

            PropertyContainer.Add(BuildObjectField(serializedObject.FindProperty("EmptyClip"), "Out of Ammo Clip"));
            PropertyContainer.Add(BuildObjectField(serializedObject.FindProperty("ReloadClip"), "Reload Clip"));
            PropertyContainer.Add(BuildObjectField(serializedObject.FindProperty("LastBulletClip"), "Last Bullet Clip"));

            return PropertyContainer;
        }

        private ObjectField BuildObjectField(SerializedProperty Property, string Label)
        {
            ObjectField objectField = new(Label);
            objectField.objectType = typeof(AudioClip);
            objectField.BindProperty(Property);

            return objectField;
        }

        private void HandleSOChange(VisualElement ContentRoot, SerializedProperty Property, ChangeEvent<Object> ChangeEvent)
        {
            if (ChangeEvent.newValue is AudioConfigScriptableObject)
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
}
