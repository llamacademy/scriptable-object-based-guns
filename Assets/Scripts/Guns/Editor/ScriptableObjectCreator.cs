using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;

namespace LlamAcademy.Guns.Editors
{
    public class ScriptableObjectCreator<T> : VisualElement where T : ScriptableObject
    {
        #region Boilerplate for Showing up in UI Builder
        public new class UxmlFactory : UxmlFactory<ScriptableObjectCreator<T>> { }
        public ScriptableObjectCreator() { }
        #endregion

        #region Control Bindings
        public override VisualElement contentContainer => ContentPanel;
        private VisualElement CreateNewPanel => this.Q("new-panel");
        private VisualElement ContentPanel => this.Q("content-panel");
        private VisualElement TitleContainer => this.Q("title-container");
        private Label CaretLabel => this.Q<Label>("caret");
        private Label TitleField => this.Q<Label>("title");
        private ObjectField ScriptableObjectField => this.Q<ObjectField>("scriptable-object");
        private ObjectField ExistingScriptableObjectField => this.Q<ObjectField>("scriptable-object-existing");
        private Label MissingSOLabel => this.Q<Label>("missing-so-label");
        private Button CreateButton => this.Q<Button>("create");
        private Button DeleteButton => this.Q<Button>("delete");
        private TextField NameInput => this.Q<TextField>("name-input");
        #endregion

        private string CreationPath;
        private bool IsExpanded = true;

        public ScriptableObjectCreator(SerializedProperty Property, string Title, string CreationPath)
        {
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Guns/Editor/ScriptableObjectCreator.uxml");
            asset.CloneTree(this);

            this.CreationPath = CreationPath;
            TitleField.text = Title;
            TitleContainer.RegisterCallback<ClickEvent>(HandleTitleClick);
            MissingSOLabel.text = $"No {Property.name} Exists!";
            CreateButton.SetEnabled(!string.IsNullOrEmpty(NameInput.text.Trim()));
            CreateButton.RegisterCallback<ClickEvent>((clickEvent) => HandleCreate(Property, NameInput.text.Trim()));
            DeleteButton.RegisterCallback<ClickEvent>((clickEvent) => HandleDelete(Property));
            NameInput.RegisterValueChangedCallback((changeEvent) => CreateButton.SetEnabled(!string.IsNullOrEmpty(changeEvent.newValue.Trim())));

            ScriptableObjectField.objectType = typeof(T);
            ExistingScriptableObjectField.objectType = typeof(T);
            ScriptableObjectField.BindProperty(Property);
            ExistingScriptableObjectField.BindProperty(Property);

            if (Property.objectReferenceValue == null)
            {
                SerializedProperty gunType = Property.serializedObject.FindProperty("Type");
                string gunTypeName = gunType.enumDisplayNames[gunType.enumValueIndex];
                NameInput.value = $"{gunTypeName} {Title.Replace("uration", "")}";
                CreateButton.SetEnabled(true);

                ContentPanel.AddToClassList("hidden");
            }
            else
            {
                CreateNewPanel.AddToClassList("hidden");
            }

            ScriptableObjectField.RegisterValueChangedCallback(HandleObjectValueChanged);
            ExistingScriptableObjectField.RegisterValueChangedCallback(HandleObjectValueChanged);
        }

        private void HandleObjectValueChanged(ChangeEvent<Object> ChangeEvent)
        {
            if (ChangeEvent.newValue == null)
            {
                ContentPanel.AddToClassList("hidden");
                CreateNewPanel.RemoveFromClassList("hidden");
            }
            else
            {
                CreateNewPanel.AddToClassList("hidden");
                ContentPanel.RemoveFromClassList("hidden");
            }

            // For some reason even if we register a callback this doesn't give us the event ;_;
            //SendEvent(ChangeEvent<T>.GetPooled(ChangeEvent.previousValue as T, ChangeEvent.newValue as T));
        }

        private void HandleCreate(SerializedProperty Property, string Name)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                T newSO = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(newSO, CreationPath + Name + ".asset");
                Property.objectReferenceValue = newSO;
                Property.serializedObject.ApplyModifiedProperties();
                Property.serializedObject.Update();
            }
        }

        private async void HandleDelete(SerializedProperty Property)
        {
            string path = AssetDatabase.GetAssetPath(Property.objectReferenceInstanceIDValue);
            Property.objectReferenceValue = null;
            Property.serializedObject.ApplyModifiedProperties();
            await Task.Delay(100);
            AssetDatabase.DeleteAsset(path);
        }

        private void HandleTitleClick(ClickEvent ClickEvent)
        {
            if (IsExpanded)
            {
                CaretLabel.RemoveFromClassList("rotate-90");
                CaretLabel.AddToClassList("rotate-0");
                
                ContentPanel.RemoveFromClassList("expanded");
                CreateNewPanel.RemoveFromClassList("expanded");
                
                ContentPanel.AddToClassList("collapsed");
                CreateNewPanel.AddToClassList("collapsed");
            }
            else
            {
                CaretLabel.RemoveFromClassList("rotate-0");
                CaretLabel.AddToClassList("rotate-90");

                ContentPanel.AddToClassList("expanded");
                CreateNewPanel.AddToClassList("expanded");
                
                ContentPanel.RemoveFromClassList("collapsed");
                CreateNewPanel.RemoveFromClassList("collapsed");
            }

            IsExpanded = !IsExpanded;
        }
    }
}
