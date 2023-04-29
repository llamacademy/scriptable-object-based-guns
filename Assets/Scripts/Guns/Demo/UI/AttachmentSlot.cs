using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LlamAcademy.Guns.Demo.UI
{
    public class AttachmentSlot : VisualElement
    {
        public Label Name => this.Q<Label>("name");
        public Label Description => this.Q<Label>("description");
        public VisualElement Icon => this.Q<VisualElement>("icon");
        public VisualElement Button => this.Q<VisualElement>(null, "chevron-right");

        private static VisualTreeAsset Tree;

        public new class UxmlFactory : UxmlFactory<AttachmentSlot> { }
        public AttachmentSlot()
        {
            GetTreeAsset();
        }

        public AttachmentSlot(string Name, string Description, string IconName)
        {
            GetTreeAsset();
            Setup(Name, Description, IconName);
        }

        private void GetTreeAsset()
        {
            if (Tree == null)
            {
                if (Application.isPlaying)
                {
                    Tree = Resources.Load<VisualTreeAsset>("UI/Templates/AttachmentSlot");
                }
                else
                {
                    Tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Resources/UI/Templates/AttachmentSlot.uxml");
                }
            }

            if (Tree == null)
            {
                Debug.LogError("Invalid path specified to visual tree asset");
            }
            else
            {
                Tree.CloneTree(this);
            }

            this.AddToClassList("attachment-slot");
            this.AddToClassList("deselected");
        }

        public void Setup(string Name, string Description, string IconName)
        {
            this.Name.text = Name;
            this.Description.text = Description;
            Icon.AddToClassList(IconName);
        }
    }
}
