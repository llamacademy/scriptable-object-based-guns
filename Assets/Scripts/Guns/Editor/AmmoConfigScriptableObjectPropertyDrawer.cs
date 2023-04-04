using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LlamAcademy.Guns.Editors
{
    [CustomPropertyDrawer(typeof(AmmoConfigScriptableObject), true)]
    public class AmmoConfigScriptableObjectPropertyDrawer : PropertyDrawer
    {
        private IntegerField CurrentAmmoField; 
        private IntegerField CurrentClipAmmoField;
        private VisualElement PropertyContainer;
        private bool UseMaxAmmoAsCurrentAmmo = true;

        public override VisualElement CreatePropertyGUI(SerializedProperty Property)
        {
            ScriptableObjectCreator<AmmoConfigScriptableObject> creator = new(Property, "Ammo Configuration", GunScriptableObjectEditor.GUN_ASSET_PATH + "Ammo/");

            VisualElement root = new();
            root.AddToClassList("panel");

            if (Property.objectReferenceValue != null)
            {
                AmmoConfigScriptableObject ammoConfig = Property.objectReferenceValue as AmmoConfigScriptableObject;
                UseMaxAmmoAsCurrentAmmo = (
                    ammoConfig.CurrentAmmo == ammoConfig.MaxAmmo
                    && ammoConfig.CurrentClipAmmo == ammoConfig.ClipSize
                );
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

            IntegerField maxAmmoField = new("Max Ammo");
            maxAmmoField.BindProperty(serializedObject.FindProperty("MaxAmmo"));
            PropertyContainer.Add(maxAmmoField);

            IntegerField clipSizeField = new("Clip Size");
            clipSizeField.BindProperty(serializedObject.FindProperty("ClipSize"));
            PropertyContainer.Add(clipSizeField);

            CurrentAmmoField = new("Current Ammo");
            CurrentAmmoField.BindProperty(serializedObject.FindProperty("CurrentAmmo"));

            CurrentClipAmmoField = new("Current Clip Ammo");
            CurrentClipAmmoField.BindProperty(serializedObject.FindProperty("CurrentClipAmmo"));
            if (!EditorApplication.isPlaying && UseMaxAmmoAsCurrentAmmo)
            {
                CurrentClipAmmoField.AddToClassList("hidden");
                CurrentAmmoField.AddToClassList("hidden");
            }

            Toggle defaultToMaxToggle = new("Default to Max Ammo");
            defaultToMaxToggle.value = UseMaxAmmoAsCurrentAmmo;
            defaultToMaxToggle.RegisterValueChangedCallback((changeEvent) =>
            {
                if (!EditorApplication.isPlaying)
                {
                    if (changeEvent.newValue)
                    {
                        CurrentClipAmmoField.AddToClassList("hidden");
                        CurrentAmmoField.AddToClassList("hidden");
                    }
                    else
                    {
                        CurrentClipAmmoField.RemoveFromClassList("hidden");
                        CurrentAmmoField.RemoveFromClassList("hidden");
                    }
                }
            });
            PropertyContainer.Add(defaultToMaxToggle);

            Label overrideLabel = new("Current Clip Info");
            PropertyContainer.Add(CurrentAmmoField);
            PropertyContainer.Add(CurrentClipAmmoField);

            EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;

            return PropertyContainer;
        }

        private void HandlePlayModeStateChanged(PlayModeStateChange State)
        {
            if (State == PlayModeStateChange.EnteredPlayMode)
            {
                CurrentClipAmmoField.RemoveFromClassList("hidden");
                CurrentAmmoField.RemoveFromClassList("hidden");
            }
            else if (State == PlayModeStateChange.ExitingPlayMode && UseMaxAmmoAsCurrentAmmo)
            {
                CurrentClipAmmoField.AddToClassList("hidden");
                CurrentAmmoField.AddToClassList("hidden");
            }
        }

        private void HandleSOChange(VisualElement ContentRoot, SerializedProperty Property, ChangeEvent<Object> ChangeEvent)
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
