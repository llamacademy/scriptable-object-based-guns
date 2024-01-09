using System;
using LlamAcademy.Guns.ImpactEffects;
using LlamAcademy.Guns.Modifiers;
using LlamAcademy.ImpactSystem;
using System.Collections;
using System.Collections.Generic;
using LlamAcademy.Guns.Persistence;
using LlamAcademy.Guns.Persistence.Model;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using SlotType = LlamAcademy.Guns.Slot;

namespace LlamAcademy.Guns.Demo.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class AttachmentDocumentController : MonoBehaviour
    {
        private bool _IsVisible;

        [field: SerializeField]
        public bool IsVisible
        {
            get { return _IsVisible; }
            private set
            {
                if (value)
                {
                    OnShow?.Invoke();
                }
                else
                {
                    OnHide?.Invoke();
                }

                _IsVisible = value;
            }
        }

        [SerializeField] private PlayerGunSelector GunSelector;

        [SerializeField] private ImpactType ExplosiveType;
        [SerializeField] private ImpactType FrostType;

        private AttachmentSlot HandleSlot;
        private AttachmentSlot BarrelSlot;
        private AttachmentSlot AmmoSlot;
        private VisualElement Dropdown;

        private IDataService DataService = new JsonDataService();
        private const string SAVE_FILE_PATH = "/player-loadout.json";

        public delegate void VisibilityEvent();

        public VisibilityEvent OnHide;
        public VisibilityEvent OnShow;

        private readonly Dictionary<Slot, int> SelectedAttachments = new()
        {
            { Slot.Handle, 0 },
            { Slot.Barrel, 0 },
            { Slot.Ammo, 0 }
        };

        /// <summary>
        /// Mock attachments. Probably better for you to persist these, including the user's selection.
        /// </summary>
        private static readonly Dictionary<Slot, List<Attachment>> MockAttachments = new()
        {
            {
                Slot.Handle, new List<Attachment>()
                {
                    new Attachment("Standard", "Standard issue", new(), 0, 0, true),
                    new Attachment("Potato Grip",
                        "Reduces the time it takes to reach maximum spread. Reduces vertical spread by 10%", new()
                        {
                            new Vector3Modifier(new Vector3(1, 0.9f, 1), "ShootConfig/Spread"),
                            new FloatModifier(0.9f, "ShootConfig/SpreadMultiplier"),
                            new FloatModifier(1.25f, "ShootConfig/MaxSpreadTime")
                        }, 0, 0),
                    new Attachment("Enhanced Grips", "Reduces Spread by 25%", new()
                    {
                        new FloatModifier(0.75f, "ShootConfig/SpreadMultiplier"),
                        new Vector3Modifier(new Vector3(0.75f, 0.75f, 0.75f), "ShootConfig/Spread")
                    }, 0, 0)
                }
            },
            {
                Slot.Barrel, new List<Attachment>()
                {
                    new Attachment("Standard", "Standard issue", new(), 0, 0, true),
                    new Attachment("Long Barrel", "Reduces the time it takes to reach maximum spread.",
                        new() { new FloatModifier(1.25f, "ShootConfig/MaxSpreadTime") }, 0, 0)
                }
            },
            {
                Slot.Ammo, new List<Attachment>()
                {
                    new Attachment("Standard", "Standard issue", new(), 0, 0, true),
                    new Attachment("Explosive Rounds",
                        "Explodes on impact dealing damage to nearby enemies. Increases bullet weight significantly.",
                        new()
                        {
                            new ImpactTypeModifier(null, ""),
                            new FloatModifier(2, "ShootConfig/BulletWeight"),
                            new ImpactEffectReplacementModifier(new ICollisionHandler[]
                            {
                                new Explode(1.5f,
                                    new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 0.5f) }),
                                    10, 10)
                            })
                        }, 0, 0),
                    new Attachment("Frost Rounds",
                        "Creates a small frost explosion on impact dealing light damage to nearby enemies and slowing them. Increases bullet weight significantly.",
                        new()
                        {
                            new ImpactTypeModifier(null, ""),
                            new FloatModifier(2, "ShootConfig/BulletWeight"),
                            new ImpactEffectReplacementModifier(new ICollisionHandler[]
                            {
                                new Frost(0.75f,
                                    new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 0.5f) }), 7,
                                    10,
                                    new AnimationCurve(new Keyframe[]
                                        { new Keyframe(0, 0.5f), new Keyframe(1.75f, 0.5f), new Keyframe(2, 1) }))
                            })
                        }, 0, 0)
                }
            }
        };

        private UIDocument Document;
        private ScrollView SlotScrollView;
        private VisualElement BorderLeft;
        private VisualElement OptionContainer;
        private ScrollView OptionScrollView;
        private VisualElement GunIcon;
        private Label GunName;

        private void Awake()
        {
            Document = GetComponent<UIDocument>();
            IsVisible = false;
            Document.rootVisualElement.AddToClassList("hidden");

            // some hack to add in the serialized fields into the dictionary.
            // If you have serialized data (highly recommend) instead of static dictionary you shouldn't need to do this
            // video is already quite long so ... :) quick and dirty
            (MockAttachments[Slot.Ammo][1].Modifiers[0] as ImpactTypeModifier).Amount = ExplosiveType;
            (MockAttachments[Slot.Ammo][2].Modifiers[0] as ImpactTypeModifier).Amount = FrostType;
        }

        private void Start()
        {
            LoadPlayerLoadout();

            SetupUxmlReferences();

            SetupDynamicFields();

            Button closeButton = Document.rootVisualElement.Q<Button>();
            closeButton.RegisterCallback<ClickEvent>((_) => Hide());
        }

        public void Hide()
        {
            string selectedGunName = GunName.text;
            GunScriptableObject selectedGun = GunSelector.Guns.Find((gun) => gun.Name == selectedGunName);
            GunSelector.PickupGun(selectedGun);
            GunSelector.ApplyModifiers(CollectModifiers());

            Loadout loadout = new()
            {
                Gun = GunSelector.ActiveBaseGun,
                Attachments = new int[]
                {
                    SelectedAttachments[Slot.Barrel],
                    SelectedAttachments[Slot.Handle],
                    SelectedAttachments[Slot.Ammo],
                }
            };
            if (DataService.SaveData(SAVE_FILE_PATH, loadout))
            {
                Debug.Log($"Successfully saved gun data to {SAVE_FILE_PATH}");
            }
            else
            {
                Debug.LogError("Unable to persist player loadout!");
            }

            Document.rootVisualElement.AddToClassList("hidden");
            Document.rootVisualElement.RemoveFromClassList("visible");
            IsVisible = false;
        }

        public void Show()
        {
            Document.rootVisualElement.AddToClassList("visible");
            Document.rootVisualElement.RemoveFromClassList("hidden");
            IsVisible = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void SetupUxmlReferences()
        {
            BorderLeft = Document.rootVisualElement.Q<VisualElement>("border-left");
            GunIcon = Document.rootVisualElement.Q<VisualElement>("icon");
            GunName = Document.rootVisualElement.Q<Label>("gunName");
            SlotScrollView = Document.rootVisualElement.Q<ScrollView>("slot-scrollview");
            OptionContainer = Document.rootVisualElement.Q<VisualElement>("option-container");
            OptionScrollView = OptionContainer.Q<ScrollView>();
        }

        private void LoadPlayerLoadout()
        {
            try
            {
                Loadout loadout = DataService.LoadData<Loadout>(SAVE_FILE_PATH);
                // Set selected attachments (controls what is applied in CollectModifiers)
                SelectedAttachments[Slot.Barrel] = loadout.Attachments[0];
                SelectedAttachments[Slot.Handle] = loadout.Attachments[1];
                SelectedAttachments[Slot.Ammo] = loadout.Attachments[2];
                // Set mock attachments dictionary to show selected (controls highlighting on UI)
                foreach (KeyValuePair<Slot, List<Attachment>> keyValuePair in MockAttachments)
                {
                    for (int i = 0; i < keyValuePair.Value.Count; i++)
                    {
                        keyValuePair.Value[i].IsSelected = SelectedAttachments[keyValuePair.Key] == i;
                    }
                }

                GunSelector.PickupGun(loadout.Gun);
            }
            catch (Exception e)
            {
                Debug.Log("Unable to load player loadout. Using default loadout.");
            }
        }

        private void SetupDynamicFields()
        {
            Attachment barrelAttachment = MockAttachments[Slot.Barrel][SelectedAttachments[Slot.Barrel]];
            BarrelSlot = new(barrelAttachment.Name, barrelAttachment.Description,
                barrelAttachment.Name.ToLower().Replace(" ", "-"));

            BarrelSlot.Button.RegisterCallback<ClickEvent>((_) => HandleSlotClick(Slot.Barrel, BarrelSlot));
            SlotScrollView.Add(BarrelSlot);

            Attachment handleAttachment = MockAttachments[Slot.Handle][SelectedAttachments[Slot.Handle]];
            HandleSlot = new(handleAttachment.Name, handleAttachment.Description,
                handleAttachment.Name.ToLower().Replace(" ", "-"));
            HandleSlot.Button.RegisterCallback<ClickEvent>((_) => HandleSlotClick(Slot.Handle, HandleSlot));
            SlotScrollView.Add(HandleSlot);

            Attachment ammoAttachment = MockAttachments[Slot.Ammo][SelectedAttachments[Slot.Ammo]];
            AmmoSlot = new(ammoAttachment.Name, ammoAttachment.Description,
                ammoAttachment.Name.ToLower().Replace(" ", "-"));
            AmmoSlot.Button.RegisterCallback<ClickEvent>((_) => HandleSlotClick(Slot.Ammo, AmmoSlot));
            SlotScrollView.Add(AmmoSlot);

            DropdownField gunSelector = Document.rootVisualElement.Q<DropdownField>("gun-selector");
            gunSelector.choices = GunSelector.Guns.ConvertAll(gun => gun.Name);
            SetGunNameAndIcon((GunSelector.ActiveGun == null ? GunSelector.Guns[0].Name : GunSelector.ActiveGun.Name),
                string.Empty);
            gunSelector.RegisterCallback<ChangeEvent<string>>(HandleChangeGun);
            gunSelector.SetValueWithoutNotify(GunSelector.Gun.ToString());
        }

        private void HandleChangeGun(ChangeEvent<string> changeEvent)
        {
            string oldGun = changeEvent.previousValue;
            string newGun = changeEvent.newValue;
            SetGunNameAndIcon(newGun, oldGun);

            HideOptionContainer();
            ResetAllAttachments();
        }

        private void ResetAllAttachments()
        {
            ClearSlotSelectionClasses();
            HandleSlot.Setup("Handle", "Standard issue", "standard");
            AmmoSlot.Setup("Ammo Type", "Standard issue", "standard");
            BarrelSlot.Setup("Barrel", "Standard issue", "standard");

            SelectedAttachments[Slot.Ammo] = 0;
            SelectedAttachments[Slot.Barrel] = 0;
            SelectedAttachments[Slot.Handle] = 0;

            foreach (KeyValuePair<Slot, List<Attachment>> keyValuePair in MockAttachments)
            {
                keyValuePair.Value[0].IsSelected = true;
                for (int i = 1; i < keyValuePair.Value.Count; i++)
                {
                    keyValuePair.Value[i].IsSelected = false;
                }
            }
        }

        private void SetGunNameAndIcon(string newGun, string oldGun)
        {
            GunName.text = newGun;
            if (!string.IsNullOrEmpty(oldGun))
            {
                GunIcon.RemoveFromClassList(oldGun.ToLower().Replace(" ", "-"));
            }

            GunIcon.AddToClassList(newGun.ToLower().Replace(" ", "-"));
        }

        private IEnumerator BuildOptionsForSlot(Slot FocusSlot)
        {
            OptionContainer.AddToClassList("fade-in-right");
            OptionContainer.RemoveFromClassList("fade-out-left");
            OptionContainer.focusable = true;
            OptionContainer.pickingMode = PickingMode.Position;

            VisualElement backButton = OptionContainer.Q<VisualElement>("icon");
            OptionScrollView.Clear();

            backButton.RegisterCallback<ClickEvent>((_) => HideOptionContainer());

            for (int i = 0; i < MockAttachments[FocusSlot].Count; i++)
            {
                int index = i;
                Attachment attachment = MockAttachments[FocusSlot][i];
                AttachmentSlot slot = new(attachment.Name, attachment.Description,
                    attachment.Name.ToLower().Replace(" ", "-"));

                slot.RegisterCallback<ClickEvent>((_) => HandleChangeOption(slot, FocusSlot, index));

                if (MockAttachments[FocusSlot][i].IsSelected)
                {
                    slot.AddToClassList("selected");
                }

                VisualElement chevron = slot.Q<VisualElement>(null, "chevron-right");
                chevron.RemoveFromClassList("chevron-right");
                chevron.AddToClassList("display-none");

                OptionScrollView.Add(slot);
            }

            int childIndex = 3;
            foreach (VisualElement child in OptionScrollView.Children())
            {
                child.AddToClassList("fade-out-up");
                child.AddToClassList($"delay-{childIndex * 50}ms");
                childIndex++;
            }

            // without a delay, the "in" animation doesn't play since we've added both in and out states in the same frame
            // the UI system doesn't know which transition to do so it just accepts the final state
            yield return null;

            foreach (VisualElement child in OptionScrollView.Children())
            {
                child.AddToClassList("fade-in-down");
            }

            BorderLeft.AddToClassList("border-left-animate-height-100");
            BorderLeft.AddToClassList("delay-150ms");
        }

        private void HideOptionContainer()
        {
            OptionContainer.AddToClassList("fade-out-left");
            OptionContainer.RemoveFromClassList("fade-in-right");
            BorderLeft.RemoveFromClassList("border-left-animate-height-100");
            BorderLeft.RemoveFromClassList("delay-150ms");
            BorderLeft.AddToClassList("delay-100ms");
            OptionContainer.focusable = false;
            OptionContainer.pickingMode = PickingMode.Ignore;

            int index = 0;
            foreach (VisualElement child in OptionScrollView.Children())
            {
                int i = index;
                child.RemoveFromClassList("fade-in-down");
                child.RemoveFromClassList($"delay-{(i + 3) * 50}ms");
                child.AddToClassList($"delay-{(OptionScrollView.childCount - i) * 50}ms");
                index++;
            }

            ClearSlotSelectionClasses();
        }

        private void HandleSlotClick(Slot Type, AttachmentSlot Slot)
        {
            ClearSlotSelectionClasses();
            Slot.AddToClassList("selected");
            StartCoroutine(BuildOptionsForSlot(Type));
        }

        private void HandleChangeOption(AttachmentSlot Slot, Slot FocusSlot, int Index)
        {
            foreach (VisualElement child in OptionScrollView.Children())
            {
                child.RemoveFromClassList("selected");
            }

            Slot.AddToClassList("selected");

            // Deselect the previously selected one
            MockAttachments[FocusSlot][SelectedAttachments[FocusSlot]].IsSelected = false;
            // Select the new one
            SelectedAttachments[FocusSlot] = Index;
            MockAttachments[FocusSlot][Index].IsSelected = true;

            AttachmentSlot slot = null;
            switch (FocusSlot)
            {
                case SlotType.Ammo:
                    slot = AmmoSlot;
                    break;
                case SlotType.Barrel:
                    slot = BarrelSlot;
                    break;
                case SlotType.Handle:
                    slot = HandleSlot;
                    break;
            }

            slot.Icon.ClearClassList();
            slot.Setup(Slot.Name.text, Slot.Description.text, Slot.Name.text.ToLower().Replace(" ", "-"));
        }

        private void ClearSlotSelectionClasses()
        {
            foreach (VisualElement element in SlotScrollView.Children())
            {
                element.RemoveFromClassList("selected");
            }
        }

        private IModifier[] CollectModifiers()
        {
            List<IModifier> modifiersToApply = new();

            foreach (KeyValuePair<Slot, int> keyValuePair in SelectedAttachments)
            {
                modifiersToApply.AddRange(MockAttachments[keyValuePair.Key][keyValuePair.Value].Modifiers);
            }

            return modifiersToApply.ToArray();
        }
    }
}