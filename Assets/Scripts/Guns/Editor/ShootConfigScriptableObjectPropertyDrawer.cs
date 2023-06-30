using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.UI.Design;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LlamAcademy.Guns.Editors
{
    /// <summary>
    /// This class demonstrates how to create a custom property drawer using UI Toolkit
    /// completely with code. It allows easily creating new ShootConfigScriptableObjects from the
    /// inspector, instead of having to create them from menus and hook up references separately.
    /// </summary>
    [CustomPropertyDrawer(typeof(ShootConfigScriptableObject), true)]
    public class ShootConfigScriptableObjectPropertyDrawer : PropertyDrawer
    {
        private VisualElement ContentPanel;
        private Label CaretLabel;
        private bool IsExpanded = true;

        public override VisualElement CreatePropertyGUI(SerializedProperty Property)
        {
            VisualElement inspector = new();
            inspector.AddToClassList("panel");

            return BuildUI(inspector, Property);
        }

        private VisualElement BuildUI(VisualElement RootElement, SerializedProperty Property)
        {
            VisualElement titleContainer = new();
            titleContainer.AddToClassList("align-horizontal");
            CaretLabel = new("▸");
            CaretLabel.style.fontSize = 18;
            CaretLabel.AddToClassList(IsExpanded ? "rotate-90" : "rotate-0");
            Label title = new("Shooting Configuration");
            title.AddToClassList("header");
            titleContainer.Add(CaretLabel);
            titleContainer.Add(title);
            titleContainer.RegisterCallback<ClickEvent>(HandleTitleClick);

            RootElement.Add(titleContainer);

            if (Property.objectReferenceValue == null)
            {
                ContentPanel = BuildNoShootPanel(RootElement, Property);
                RootElement.Add(ContentPanel);
            }
            else
            {
                ContentPanel = BuildShootConfigBox(RootElement, Property);
                RootElement.Add(ContentPanel);
            }

            ContentPanel.AddToClassList(IsExpanded ? "expanded" : "collapsed");

            return RootElement;
        }

        private VisualElement BuildShootConfigBox(VisualElement RootElement, SerializedProperty Property)
        {
            VisualElement shootConfigBox = new();
            shootConfigBox.name = "shoot-config-box";

            shootConfigBox.Add(BuildObjectField(RootElement, Property));

            Button deleteButton = new(() => DeleteSO(Property));
            deleteButton.text = "Delete";
            deleteButton.AddToClassList("danger");
            deleteButton.AddToClassList("align-right");
            deleteButton.AddToClassList("mb-8");
            shootConfigBox.Add(deleteButton);

            SerializedObject shootConfigSO = new(Property.objectReferenceValue);

            Label bulletBehavior = new("Gun/Bullet Interaction");
            bulletBehavior.AddToClassList("bold");
            shootConfigBox.Add(bulletBehavior);

            EnumField shootTypeField = new("Shoot Type", ShootType.FromGun);
            shootTypeField.BindProperty(shootConfigSO.FindProperty("ShootType"));
            shootConfigBox.Add(shootTypeField);

            ObjectField bulletPrefab = new("Bullet Prefab");
            bulletPrefab.objectType = typeof(Bullet);
            bulletPrefab.BindProperty(shootConfigSO.FindProperty("BulletPrefab"));
            FloatField bulletSpawnForceField = new("Bullet Force");
            bulletSpawnForceField.BindProperty(shootConfigSO.FindProperty("BulletSpawnForce"));
            FloatField bulletWeightField = new("Bullet Mass");
            bulletWeightField.BindProperty(shootConfigSO.FindProperty("BulletWeight"));

            SerializedProperty isHitscan = shootConfigSO.FindProperty("IsHitscan");
            Toggle isHitscanToggle = new("Is Hitscan Gun");
            isHitscanToggle.RegisterValueChangedCallback((changeEvent) =>
            {
                if (changeEvent.newValue)
                {
                    bulletPrefab.AddToClassList("hidden");
                    bulletSpawnForceField.AddToClassList("hidden");
                    bulletWeightField.AddToClassList("hidden");
                }
                else
                {
                    bulletPrefab.RemoveFromClassList("hidden");
                    bulletSpawnForceField.RemoveFromClassList("hidden");
                    bulletWeightField.RemoveFromClassList("hidden");
                }
            });
            isHitscanToggle.BindProperty(isHitscan);
            shootConfigBox.Add(isHitscanToggle);
            
            if (isHitscan.boolValue)
            {
                bulletSpawnForceField.AddToClassList("hidden");
                bulletPrefab.AddToClassList("hidden");
                bulletWeightField.AddToClassList("hidden");
            }
            shootConfigBox.Add(bulletPrefab);
            shootConfigBox.Add(bulletSpawnForceField);
            shootConfigBox.Add(bulletWeightField);

            LayerMaskField hitMaskField = new("Hit Mask");
            hitMaskField.BindProperty(shootConfigSO.FindProperty("HitMask"));
            shootConfigBox.Add(hitMaskField);

            VisualElement rangeContainer = new();
            rangeContainer.AddToClassList("align-horizontal");

            Slider shootDelaySlider = new("Delay Between Shots", 0.001f, 3f);
            shootDelaySlider.BindProperty(shootConfigSO.FindProperty("FireRate"));
            shootDelaySlider.AddToClassList("flex-grow");
            FloatField shootDelayField = new();
            shootDelayField.style.minWidth = 35;
            shootDelayField.BindProperty(shootConfigSO.FindProperty("FireRate"));

            rangeContainer.Add(shootDelaySlider);
            rangeContainer.Add(shootDelayField);
            shootConfigBox.Add(rangeContainer);

            Label spreadLabel = new("Bullet Spread");
            spreadLabel.AddToClassList("mt-8");
            spreadLabel.AddToClassList("bold");
            shootConfigBox.Add(spreadLabel);

            VisualElement recoilRecoveryContainer = new();
            recoilRecoveryContainer.AddToClassList("align-horizontal");

            Slider recoilRecoverySlider = new("Recoil Recovery Speed", 0.01f, 5f);
            recoilRecoverySlider.BindProperty(shootConfigSO.FindProperty("RecoilRecoverySpeed"));
            recoilRecoverySlider.AddToClassList("flex-grow");
            FloatField recoilRecoveryField = new();
            recoilRecoveryField.style.minWidth = 35;
            recoilRecoveryField.BindProperty(shootConfigSO.FindProperty("RecoilRecoverySpeed"));

            recoilRecoveryContainer.Add(recoilRecoverySlider);
            recoilRecoveryContainer.Add(recoilRecoveryField);
            shootConfigBox.Add(recoilRecoveryContainer);

            VisualElement maxSpreadTimeContainer = new();
            maxSpreadTimeContainer.AddToClassList("align-horizontal");

            Slider maxSpreadTimeSlider = new("Max Spread Time", 0.01f, 5f);
            maxSpreadTimeSlider.BindProperty(shootConfigSO.FindProperty("MaxSpreadTime"));
            maxSpreadTimeSlider.AddToClassList("flex-grow");
            FloatField maxSpreadTimeField = new();
            maxSpreadTimeField.style.minWidth = 35;
            maxSpreadTimeField.BindProperty(shootConfigSO.FindProperty("MaxSpreadTime"));

            maxSpreadTimeContainer.Add(maxSpreadTimeSlider);
            maxSpreadTimeContainer.Add(maxSpreadTimeField);
            shootConfigBox.Add(maxSpreadTimeContainer);

            SerializedProperty spreadType = shootConfigSO.FindProperty("SpreadType");

            EnumField spreadTypeField = new("Spread Type", BulletSpreadType.Simple);

            Vector3Field simpleSpreadField = new("Spread Range");
            HelpBox spreadHelpBox = new("This defines the offset in x/y/z direction when \"Max Spread Time\" has been reached.\r\n" +
                "Lower values mean less total recoil.\r\n" +
                "Range goes from negative to positive of the provided value. For example, 0.1 on x means a range of -0.1 to 0.1 will be chosen for the x offset"
                , HelpBoxMessageType.Info
            );

            FloatField spreadMultiplierField = new("Multiplier");
            spreadMultiplierField.BindProperty(shootConfigSO.FindProperty("SpreadMultiplier"));
            ObjectField spreadTextureField = new("Texture");
            spreadTextureField.BindProperty(shootConfigSO.FindProperty("SpreadTexture"));
            spreadTextureField.objectType = typeof(Texture2D);

            spreadTypeField.BindProperty(spreadType);
            spreadTypeField.RegisterValueChangedCallback((changeEvent) =>
            {
                BulletSpreadType newValue = (BulletSpreadType)changeEvent.newValue;

                switch (newValue)
                {
                    case BulletSpreadType.TextureBased:
                        {
                            simpleSpreadField.AddToClassList("hidden");
                            spreadHelpBox.AddToClassList("hidden");
                            spreadMultiplierField.RemoveFromClassList("hidden");
                            spreadTextureField.RemoveFromClassList("hidden");
                            break;
                        }
                    case BulletSpreadType.Simple:
                        {
                            simpleSpreadField.RemoveFromClassList("hidden");
                            spreadHelpBox.RemoveFromClassList("hidden");
                            spreadMultiplierField.AddToClassList("hidden");
                            spreadTextureField.AddToClassList("hidden");
                            break;
                        }
                    case BulletSpreadType.None:
                        {
                            simpleSpreadField.AddToClassList("hidden");
                            spreadHelpBox.AddToClassList("hidden");
                            spreadMultiplierField.AddToClassList("hidden");
                            spreadTextureField.AddToClassList("hidden");
                            break;
                        }
                };
            });
            shootConfigBox.Add(spreadTypeField);


            simpleSpreadField.BindProperty(shootConfigSO.FindProperty("Spread"));
            shootConfigBox.Add(spreadHelpBox);
            shootConfigBox.Add(simpleSpreadField);

            shootConfigBox.Add(spreadTextureField);
            shootConfigBox.Add(spreadMultiplierField);

            if (spreadType.enumValueIndex != 1) // simple spread
            {
                simpleSpreadField.AddToClassList("hidden");
                spreadHelpBox.AddToClassList("hidden");
            }
            else if (spreadType.enumValueIndex != 2) // texture based spread
            {
                spreadMultiplierField.AddToClassList("hidden");
                spreadTextureField.AddToClassList("hidden");
            }

            return shootConfigBox;
        }

        private VisualElement BuildNoShootPanel(VisualElement RootElement, SerializedProperty Property)
        {
            VisualElement noShootConfigBox = new();
            noShootConfigBox.name = "no-shoot-config-box";

            Label noShootConfigLabel = new("No Shoot Config Exists!");
            noShootConfigLabel.AddToClassList("mb-8");
            noShootConfigBox.Add(noShootConfigLabel);

            noShootConfigBox.Add(new Label("Create a new one with name"));
            VisualElement horizontalBox = new();
            horizontalBox.AddToClassList("align-horizontal");

            TextField soNameField = new();
            soNameField.AddToClassList("flex-grow");
            SerializedProperty gunType = Property.serializedObject.FindProperty("Type");
            string gunTypeName = gunType.enumDisplayNames[gunType.enumValueIndex];
            soNameField.value = $"{gunTypeName} Shoot Config";

            Button createButton = new(() => CreateShootConfig(soNameField.text, Property));
            createButton.text = "Create";
            createButton.SetEnabled(true);

            horizontalBox.Add(soNameField);
            horizontalBox.Add(createButton);

            noShootConfigBox.Add(horizontalBox);

            Label selectExistingLabel = new("Select Existing");
            selectExistingLabel.AddToClassList("pt-4");
            selectExistingLabel.AddToClassList("mt-4");
            selectExistingLabel.AddToClassList("thin-border-top");
            noShootConfigBox.Add(selectExistingLabel);

            noShootConfigBox.Add(BuildObjectField(RootElement, Property));

            return noShootConfigBox;
        }

        private ObjectField BuildObjectField(VisualElement RootElement, SerializedProperty Property)
        {
            ObjectField shootConfigObjectField = new("Shoot Config");
            shootConfigObjectField.objectType = typeof(ShootConfigScriptableObject);
            shootConfigObjectField.BindProperty(Property.serializedObject.FindProperty("ShootConfig"));

            // ChangeEvents are dispatched AFTER the change, so you can't just check against the current SerializedProperty
            // value. You have to check against the previous one. If we didn't do the if() check below
            // there's infinite looping through the events changing.
            ShootConfigScriptableObject currentValue = Property.objectReferenceValue as ShootConfigScriptableObject;
            shootConfigObjectField.RegisterValueChangedCallback((changeEvent) =>
            {
                if (changeEvent.newValue != currentValue)
                {
                    HandleChangeShootConfig(changeEvent, RootElement, Property);
                }
            });

            return shootConfigObjectField;
        }

        private void HandleChangeShootConfig(ChangeEvent<Object> ChangeEvent, VisualElement RootElement, SerializedProperty Property)
        {
            RootElement.Clear();
            BuildUI(RootElement, Property.serializedObject.FindProperty("ShootConfig"));
        }

        private void CreateShootConfig(string Name, SerializedProperty Property)
        {
            ShootConfigScriptableObject shootConfig = ScriptableObject.CreateInstance<ShootConfigScriptableObject>();
            AssetDatabase.CreateAsset(shootConfig, 
                GunScriptableObjectEditor.GUN_ASSET_PATH + "Shoot/" + Name + ".asset"
            );
            Property.objectReferenceValue = shootConfig;
            Property.serializedObject.ApplyModifiedProperties();
            Property.serializedObject.Update();
        }

        private async void DeleteSO(SerializedProperty Property)
        {
            string path = AssetDatabase.GetAssetPath(Property.objectReferenceInstanceIDValue);
            Property.objectReferenceValue = null;
            Property.serializedObject.ApplyModifiedProperties();
            // if we don't defer this, then we do a null vs null check in OnEditorGUI and need to rebuild the whole tree
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
                ContentPanel.AddToClassList("collapsed");
            }
            else
            {
                CaretLabel.RemoveFromClassList("rotate-0");
                CaretLabel.AddToClassList("rotate-90");

                ContentPanel.AddToClassList("expanded");
                ContentPanel.RemoveFromClassList("collapsed");
            }

            IsExpanded = !IsExpanded;
        }
    }
}
