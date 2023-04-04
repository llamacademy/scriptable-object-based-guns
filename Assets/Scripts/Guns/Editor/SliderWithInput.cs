using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace LlamAcademy.Guns.Editors
{
    public class SliderWithInput : VisualElement
    {
        #region Boilerplate for Showing up in UI Builder
        public new class UxmlFactory : UxmlFactory<SliderWithInput> { }
        public SliderWithInput() { }
        #endregion

        private Slider Slider => this.Q<Slider>("slider");
        private FloatField Input => this.Q<FloatField>("input");

        public SliderWithInput(
            SerializedProperty Property,
            string Label = "",
            float MinValue = 0,
            float MaxValue = 10)
        {
            Init(Property, Label, MinValue, MaxValue);
        }

        public void Init(
            SerializedProperty Property,
            string Label = "",
            float MinValue = 0,
            float MaxValue = 10)
        {
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/Scripts/Guns/Editor/SliderWithInput.uxml"
            );

            asset.CloneTree(this);

            Slider.lowValue = MinValue;
            Slider.highValue = MaxValue;
            Slider.label = Label;
            Slider.BindProperty(Property);
            Input.BindProperty(Property);
        }
    }
}
