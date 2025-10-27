//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

#if UNITY_EDITOR
using Esper.SkillWeb.Database;
using Esper.SkillWeb.Settings;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Esper.SkillWeb.Editor
{
    public class SkillWebSettingsEditorWindow : EditorWindow
    {
        private EnumField debugLogField;
        private Toggle enablePlayerLevelRequirementToggle;
        private Toggle enableDowngradingToggle;
        private ObjectField skillDatasetField;
        private ObjectField webDatasetField;
        private ListView tagsList;
        private FloatField zoomStrengthField;
        private FloatField zoomSmoothingField;
        private FloatField minScaleField;
        private FloatField maxScaleField;
        private EnumField startingScaleField;
        private FloatField panSmoothingField;
        private FloatField afterSnapDelayField;
        private Toggle automaticBoundsToggle;
        private FloatField resetFocusSpeedField;
        private EnumField mousePanButton;
        private FloatField tinySizeField;
        private FloatField smallSizeField;
        private FloatField mediumSizeField;
        private FloatField largeSizeField;
        private FloatField giantSizeField;
        private Button resetSkillDatasetsButton;
        private Button resetWebDatasetsButton;
        private Button deleteAllSkillsButton;
        private Button deleteAllWebsButton;
        private Button validateButton;
        private TextField databaseNameField;
        private EnumField deltaTimeField;

        private SkillWebSettings settings;

        [MenuItem("Window/Skill Web/Settings")]
        public static void Open()
        {
            SkillWebSettingsEditorWindow wnd = GetWindow<SkillWebSettingsEditorWindow>();
            wnd.titleContent = new GUIContent("Skill Web Settings");
        }

        private void CreateGUI()
        {
            minSize = new Vector2(350, 450);

            VisualElement root = rootVisualElement;
            var visualTree = AssetSearch.Find<VisualTreeAsset>("SkillWeb", "Scripts/Editor/SkillWebSettingsEditorWindow.uxml");
            visualTree.CloneTree(root);

            debugLogField = root.Q<EnumField>("DebugLogField");
            enablePlayerLevelRequirementToggle = root.Q<Toggle>("EnablePlayerLevelRequirementToggle");
            enableDowngradingToggle = root.Q<Toggle>("EnableDowngradingToggle");
            skillDatasetField = root.Q<ObjectField>("SkillDatasetField");
            webDatasetField = root.Q<ObjectField>("WebDatasetField");
            tagsList = root.Q<ListView>("TagsList");
            zoomStrengthField = root.Q<FloatField>("ZoomStrengthField");
            zoomSmoothingField = root.Q<FloatField>("ZoomSmoothingField");
            minScaleField = root.Q<FloatField>("MinScaleField");
            maxScaleField = root.Q<FloatField>("MaxScaleField");
            startingScaleField = root.Q<EnumField>("StartingScaleField");
            panSmoothingField = root.Q<FloatField>("PanSmoothingField");
            afterSnapDelayField = root.Q<FloatField>("AfterSnapDelayField");
            automaticBoundsToggle = root.Q<Toggle>("AutomaticBoundsToggle");
            resetFocusSpeedField = root.Q<FloatField>("ResetFocusSpeedField");
            mousePanButton = root.Q<EnumField>("MousePanButtonField");
            tinySizeField = root.Q<FloatField>("TinySizeField");
            smallSizeField = root.Q<FloatField>("SmallSizeField");
            mediumSizeField = root.Q<FloatField>("MediumSizeField");
            largeSizeField = root.Q<FloatField>("LargeSizeField");
            giantSizeField = root.Q<FloatField>("GiantSizeField");
            resetSkillDatasetsButton = root.Q<Button>("ResetSkillDatasetsButton");
            resetWebDatasetsButton = root.Q<Button>("ResetWebDatasetsButton");
            deleteAllSkillsButton = root.Q<Button>("DeleteAllSkillsButton");
            deleteAllWebsButton = root.Q<Button>("DeleteAllWebsButton");
            validateButton = root.Q<Button>("ValidateButton");
            databaseNameField = root.Q<TextField>("DatabaseNameField");
            deltaTimeField = root.Q<EnumField>("DeltaTimeField");

            resetSkillDatasetsButton.clicked += ResetAllSkillDatasets;
            resetWebDatasetsButton.clicked += ResetAllWebDatasets;
            deleteAllSkillsButton.clicked += DeleteAllSkills;
            deleteAllWebsButton.clicked += DeleteAllWebs;
            validateButton.clicked += ValidateDatabase;

            settings = GetOrCreateSettings();
            var serializedObject = new SerializedObject(settings);
            debugLogField.BindProperty(serializedObject.FindProperty("debugLogMode"));
            enablePlayerLevelRequirementToggle.BindProperty(serializedObject.FindProperty("enablePlayerLevelRequirement"));
            enableDowngradingToggle.BindProperty(serializedObject.FindProperty("enableDowngrading"));
            skillDatasetField.BindProperty(serializedObject.FindProperty("skillDatasetReference"));
            webDatasetField.BindProperty(serializedObject.FindProperty("webDatasetReference"));
            tagsList.BindProperty(serializedObject.FindProperty("tags"));
            zoomStrengthField.BindProperty(serializedObject.FindProperty("zoomStrength"));
            zoomSmoothingField.BindProperty(serializedObject.FindProperty("zoomSmoothing"));
            minScaleField.BindProperty(serializedObject.FindProperty("minScale"));
            maxScaleField.BindProperty(serializedObject.FindProperty("maxScale"));
            startingScaleField.BindProperty(serializedObject.FindProperty("startingScale"));
            panSmoothingField.BindProperty(serializedObject.FindProperty("panSmoothing"));
            afterSnapDelayField.BindProperty(serializedObject.FindProperty("afterSnapDelay"));
            automaticBoundsToggle.BindProperty(serializedObject.FindProperty("automaticBoundsEnabled"));
            resetFocusSpeedField.BindProperty(serializedObject.FindProperty("resetFocusSpeed"));
            mousePanButton.BindProperty(serializedObject.FindProperty("mousePanButton"));
            tinySizeField.BindProperty(serializedObject.FindProperty("skillNodeSizes").FindPropertyRelative("tiny"));
            smallSizeField.BindProperty(serializedObject.FindProperty("skillNodeSizes").FindPropertyRelative("small"));
            mediumSizeField.BindProperty(serializedObject.FindProperty("skillNodeSizes").FindPropertyRelative("medium"));
            largeSizeField.BindProperty(serializedObject.FindProperty("skillNodeSizes").FindPropertyRelative("large"));
            giantSizeField.BindProperty(serializedObject.FindProperty("skillNodeSizes").FindPropertyRelative("giant"));
            databaseNameField.BindProperty(serializedObject.FindProperty("databaseName"));
            deltaTimeField.BindProperty(serializedObject.FindProperty("deltaTime"));

            tagsList.itemsAdded += x =>
            {
                OnTagListChanged();
            };

            tagsList.itemsRemoved += x =>
            {
                OnTagListChanged();
            };

            tagsList.RegisterCallback<FocusOutEvent>(x =>
            {
                OnTagListChanged();
            });

            skillDatasetField.RegisterValueChangedCallback(x =>
            {
                if (!EditorUtility.DisplayDialog("Change Dataset?", "Changing this value will delete all existing datasets " +
                    "for all skills, replacing them with new ones of this type or nothing if this field is left empty. This " +
                    "cannot be undone. Nothing will happen unless the set dataset type is different from the previous one. " +
                    "Continue?", "Confirm", "Cancel"))
                {
                    skillDatasetField.SetValueWithoutNotify(x.previousValue);
                }
                else
                {
                    bool differentTypes = !x.newValue || !x.previousValue || x.previousValue.GetType() != x.newValue.GetType();

                    if (differentTypes)
                    {
                        var allSkills = SkillWeb.GetAllSkills();

                        foreach (var skill in allSkills)
                        {
                            if (skill.dataset)
                            {
                                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(skill.dataset));
                                skill.dataset = null;
                            }

                            if (x.newValue)
                            {
                                skill.GenerateDataset();
                            }
                        }
                    }
                }
            });

            webDatasetField.RegisterValueChangedCallback(x =>
            {
                if (!EditorUtility.DisplayDialog("Change Dataset?", "Changing this value will delete all existing datasets " +
                    "for all web graphs, replacing them with new ones of this type or nothing if this field is left empty. This " +
                    "cannot be undone. Nothing will happen unless the set dataset type is different from the previous one. " +
                    "Continue?", "Confirm", "Cancel"))
                {
                    webDatasetField.SetValueWithoutNotify(x.previousValue);
                }
                else
                {
                    bool differentTypes = !x.newValue || !x.previousValue || x.previousValue.GetType() != x.newValue.GetType();

                    if (differentTypes)
                    {
                        var allWebs = SkillWeb.GetAllWebGraphs();

                        foreach (var web in allWebs)
                        {
                            if (web.dataset)
                            {
                                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(web.dataset));
                                web.dataset = null;
                            }

                            if (x.newValue)
                            {
                                web.GenerateDataset();
                            }
                        }
                    }
                }
            });

            zoomStrengthField.RegisterValueChangedCallback(x =>
            {
                if (x.newValue < 0)
                {
                    settings.zoomStrength = 0;
                    zoomStrengthField.SetValueWithoutNotify(0);
                }
            });

            zoomSmoothingField.RegisterValueChangedCallback(x =>
            {
                if (x.newValue < 0)
                {
                    settings.zoomSmoothing = 0;
                    zoomSmoothingField.SetValueWithoutNotify(0);
                }
            });

            minScaleField.RegisterValueChangedCallback(x =>
            {
                if (x.newValue < 0)
                {
                    settings.minScale = 0;
                    minScaleField.SetValueWithoutNotify(0);
                }
                else if (x.newValue > 1)
                {
                    settings.minScale = 1;
                    minScaleField.SetValueWithoutNotify(1);
                }
            });

            maxScaleField.RegisterValueChangedCallback(x =>
            {
                if (x.newValue < 1)
                {
                    settings.maxScale = 1;
                    maxScaleField.SetValueWithoutNotify(1);
                }
            });

            panSmoothingField.RegisterValueChangedCallback(x =>
            {
                if (x.newValue < 0)
                {
                    settings.panSmoothing = 0;
                    panSmoothingField.SetValueWithoutNotify(0);
                }
            });

            afterSnapDelayField.RegisterValueChangedCallback(x =>
            {
                if (x.newValue < 0)
                {
                    settings.afterSnapDelay = 0;
                    afterSnapDelayField.SetValueWithoutNotify(0);
                }
            });

            resetFocusSpeedField.RegisterValueChangedCallback(x =>
            {
                if (x.newValue < 0)
                {
                    settings.resetFocusSpeed = 0;
                    resetFocusSpeedField.SetValueWithoutNotify(0);
                }
            });

            tinySizeField.RegisterValueChangedCallback(x =>
            {
                if (x.newValue < 0)
                {
                    settings.skillNodeSizes.tiny = 0;
                    tinySizeField.SetValueWithoutNotify(0);
                }
            });

            smallSizeField.RegisterValueChangedCallback(x =>
            {
                if (x.newValue < 0)
                {
                    settings.skillNodeSizes.small = 0;
                    smallSizeField.SetValueWithoutNotify(0);
                }
            });

            mediumSizeField.RegisterValueChangedCallback(x =>
            {
                if (x.newValue < 0)
                {
                    settings.skillNodeSizes.medium = 0;
                    mediumSizeField.SetValueWithoutNotify(0);
                }
            });

            largeSizeField.RegisterValueChangedCallback(x =>
            {
                if (x.newValue < 0)
                {
                    settings.skillNodeSizes.large = 0;
                    largeSizeField.SetValueWithoutNotify(0);
                }
            });

            giantSizeField.RegisterValueChangedCallback(x =>
            {
                if (x.newValue < 0)
                {
                    settings.skillNodeSizes.giant = 0;
                    giantSizeField.SetValueWithoutNotify(0);
                }
            });
        }

        private void OnTagListChanged()
        {
            tagsList.schedule.Execute(() =>
            {
                if (HasOpenInstances<SkillBankEditorWindow>())
                {
                    GetWindow<SkillBankEditorWindow>().ResetFilters();
                }
            });
        }

        private void ValidateDatabase()
        {
            var path = Path.Combine(Application.streamingAssetsPath, settings.databaseName + ".db");

            if (!File.Exists(path))
            {
                SkillWebDatabase.Initialize();
            }
            else
            {
                SkillWebDatabase.Disconnect();
                SkillWebDatabase.DeleteRuntimeDatabase();
                SkillWebDatabase.DeleteEditorDatabase();
                SkillWebDatabase.Initialize();
            }

            var skills = SkillWeb.GetAllSkills();

            foreach (var item in skills)
            {
                item.UpdateDatabaseRecord();
            }

            var webs = SkillWeb.GetAllWebGraphs();

            foreach (var item in webs)
            {
                item.UpdateDatabaseRecord();
            }

            skills = null;
            webs = null;

            if (!HasOpenInstances<SkillBankEditorWindow>() && !HasOpenInstances<WebCreatorEditorWindow>())
            {
                SkillWebDatabase.Disconnect();
            }

            Resources.UnloadUnusedAssets();

            EditorUtility.DisplayDialog("Done.", "Database validated.", "Ok");
        }

        private void ResetAllSkillDatasets()
        {
            if (!EditorUtility.DisplayDialog($"Reset All Skill Datasets?", "Are you sure you want to reset all skill datasets? This cannot be undone.", "Reset", "Cancel"))
            {
                return;
            }

            var allSkills = SkillWeb.GetAllSkills();

            foreach (var skill in allSkills)
            {
                if (skill.dataset)
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(skill.dataset));
                }

                skill.GenerateDataset();
            }

            if (HasOpenInstances<SkillBankEditorWindow>())
            {
                GetWindow<SkillBankEditorWindow>().Close();
            }
        }

        private void ResetAllWebDatasets()
        {
            if (!EditorUtility.DisplayDialog($"Reset All Web Datasets?", "Are you sure you want to reset all web datasets? This cannot be undone.", "Reset", "Cancel"))
            {
                return;
            }

            var allWebs = SkillWeb.GetAllWebGraphs();

            foreach (var web in allWebs)
            {
                if (web.dataset)
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(web.dataset));
                }

                web.GenerateDataset();
            }

            if (HasOpenInstances<WebCreatorEditorWindow>())
            {
                GetWindow<WebCreatorEditorWindow>().Close();
            }
        }

        private void DeleteAllSkills()
        {
            if (!EditorUtility.DisplayDialog($"Delete All Skills?", "Are you sure you want to delete all skills? This cannot be undone.", "Delete", "Cancel"))
            {
                return;
            }

            bool disconnectOnComplete = false;

            if (!SkillWebDatabase.IsConnected)
            {
                SkillWebDatabase.Initialize();
                disconnectOnComplete = true;
            }

            var allSkills = SkillWeb.GetAllSkills();

            foreach (var skill in allSkills)
            {
                skill.DeleteDatabaseRecord();

                if (skill.dataset)
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(skill.dataset));
                }

                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(skill));
            }

            if (HasOpenInstances<SkillBankEditorWindow>())
            {
                GetWindow<SkillBankEditorWindow>().Close();
            }

            if (disconnectOnComplete)
            {
                SkillWebDatabase.Disconnect();
            }
        }

        private void DeleteAllWebs()
        {
            if (!EditorUtility.DisplayDialog($"Delete All Web Graphs?", "Are you sure you want to delete all web graphs? This cannot be undone.", "Delete", "Cancel"))
            {
                return;
            }

            bool disconnectOnComplete = false;

            if (!SkillWebDatabase.IsConnected)
            {
                SkillWebDatabase.Initialize();
                disconnectOnComplete = true;
            }

            var allWebs = SkillWeb.GetAllWebGraphs();

            foreach (var web in allWebs)
            {
                web.DeleteDatabaseRecord();

                if (web.dataset)
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(web.dataset));
                }

                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(web));   
            }

            if (HasOpenInstances<WebCreatorEditorWindow>())
            {
                GetWindow<WebCreatorEditorWindow>().Close();
            }

            if (disconnectOnComplete)
            {
                SkillWebDatabase.Disconnect();
            }
        }

        public static SkillWebSettings GetOrCreateSettings()
        {
            string startPath = AssetSearch.FindFolder("SkillWeb");
            string assetDirectoryPath = Path.Combine(startPath, "Resources", "SkillWeb", "Settings");
            string fullAssetPath = Path.Combine(assetDirectoryPath, "SkillWebSettings.asset");
            string fullDirectoryPath = Path.GetFullPath(assetDirectoryPath);

            if (!Directory.Exists(fullDirectoryPath))
            {
                Directory.CreateDirectory(fullDirectoryPath);
            }

            var settings = AssetSearch.Find<SkillWebSettings>(fullAssetPath);

            if (!settings)
            {
                settings = CreateInstance<SkillWebSettings>();
                settings.debugLogMode = SkillWebSettings.DebugLogMode.Normal;
                settings.skillNodeSizes.tiny = 100;
                settings.skillNodeSizes.small = 125;
                settings.skillNodeSizes.medium = 150;
                settings.skillNodeSizes.large = 175;
                settings.skillNodeSizes.giant = 200;
                settings.tags = new System.Collections.Generic.List<string>() { "Default" };
                AssetDatabase.CreateAsset(settings, fullAssetPath);
                AssetDatabase.SaveAssets();
            }

            return settings;
        }
    }
}
#endif