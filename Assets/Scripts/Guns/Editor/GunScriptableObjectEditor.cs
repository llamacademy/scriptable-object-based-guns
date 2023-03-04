using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LlamAcademy.Guns.Editors
{
    [CustomEditor(typeof(GunScriptableObject), true)]
    public class GunScriptableObjectEditor : Editor
    {
        [SerializeField]
        private VisualTreeAsset VisualTree;

        public const string GUN_ASSET_PATH = "Assets/Guns/";

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement inspector = new();

            // Import UXML
            inspector.Add(VisualTree.Instantiate());

            return inspector;
        }
    }
}
