using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

namespace LlamAcademy.Guns.Demo.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject PlayerObject;
        [SerializeField] private Canvas UICanvas;
        [SerializeField] private AttachmentDocumentController AttachmentController;
        
        private UIDocument Document;
        private VisualElement Root;
        private Button PlayButton;
        private Button LoadoutButton;

        private void Awake()
        {
            Document = GetComponent<UIDocument>();
            Document.sortingOrder = 2;

            Root = Document.rootVisualElement.Q("root");
            Root.AddToClassList("visible");
            PlayButton = Document.rootVisualElement.Q<Button>("play");
            LoadoutButton = Document.rootVisualElement.Q<Button>("loadout");

            AttachmentController.OnHide += () => Document.sortingOrder = 2; 
            AttachmentController.OnShow += () => Document.sortingOrder = 0; 
            
            PlayButton.RegisterCallback<ClickEvent>(_ => PlayGame());
            LoadoutButton.RegisterCallback<ClickEvent>(_ => ShowLoadout());
        }

        private void PlayGame()
        {
            Root.AddToClassList("hidden");
            Root.RemoveFromClassList("visible");
            Root.pickingMode = PickingMode.Ignore;
            // order is important here - PlayerObject must be enabled to properly set
            // Animator bools for IK
            PlayerObject.SetActive(true);
            AttachmentController.Hide();
            UICanvas.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void ShowLoadout()
        {
            Document.sortingOrder = 0;
            AttachmentController.Show();
        }
    }
}
