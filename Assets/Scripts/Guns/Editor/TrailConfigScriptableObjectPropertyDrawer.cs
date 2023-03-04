using LlamAcademy.Guns.Editors;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LlamAcademy.Guns
{
    [CustomPropertyDrawer(typeof(TrailConfigScriptableObject), true)]
    public class TrailConfigScriptableObjectPropertyDrawer : PropertyDrawer
    {
        private VisualElement PropertyContainer;

        public override VisualElement CreatePropertyGUI(SerializedProperty Property)
        {
            ScriptableObjectCreator<TrailConfigScriptableObject> creator = new(Property, "Trail Configuration", GunScriptableObjectEditor.GUN_ASSET_PATH + "Trail/");

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

            SerializedObject serializedObject = new SerializedObject(Property.objectReferenceValue);

            ObjectField materialField = new("Material");
            materialField.objectType = typeof(Material);
            materialField.BindProperty(serializedObject.FindProperty("Material"));
            PropertyContainer.Add(materialField);

            CurveField WidthCurve = new CurveField("Width Curve");
            WidthCurve.BindProperty(serializedObject.FindProperty("WidthCurve"));
            PropertyContainer.Add(WidthCurve);

            SliderWithInput durationSlider = new(serializedObject.FindProperty("Duration"), "Duration", 0.01f, 5f);
            PropertyContainer.Add(durationSlider);
            SliderWithInput vertexDistanceSlider = new(serializedObject.FindProperty("MinVertexDistance"), "Min Vertex Distance", 0.01f, 0.1f);
            PropertyContainer.Add(vertexDistanceSlider);

            GradientField colorGradient = new("Color Gradient");
            colorGradient.BindProperty(serializedObject.FindProperty("Color"));
            PropertyContainer.Add(colorGradient);

            SliderWithInput missDistance = new(serializedObject.FindProperty("MissDistance"), "Simulated Miss Distance", 10, 1000);
            PropertyContainer.Add(missDistance);
            SliderWithInput simulationSpeed = new(serializedObject.FindProperty("SimulationSpeed"), "Simulated Bullet Speed", 10, 1000);
            PropertyContainer.Add(simulationSpeed);

            return PropertyContainer;
        }

        private void HandleSOChange(VisualElement ContentRoot, SerializedProperty Property, ChangeEvent<Object> ChangeEvent)
        {
            if (ChangeEvent.newValue is TrailConfigScriptableObject)
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
