//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

#if UNITY_EDITOR
using Esper.SkillWeb.Database;
using Esper.SkillWeb.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Esper.SkillWeb.Editor
{
    public class SkillBankEditorWindow : EditorWindow
    {
        private ToolbarSearchField searchField;
        private DropdownField tagFilter;
        private DropdownField sizeFilter;
        private VisualElement skillScrollContent;
        private ScrollView inspectorScroll;
        private Button createButton;
        private Button deleteButton;
        private Button previousPageButton;
        private Button nextPageButton;
        private Label pagesLabel;
        private Foldout generalFoldout;
        private Foldout datasetFoldout;
        private TextField nameField;
        private DropdownField tagField;
        private IntegerField maxLevelField;
        private IntegerField levelRequirementField;
        private ObjectField lockedIconField;
        private ColorField lockedColorField;
        private ObjectField unlockedIconField;
        private ColorField unlockedColorField;
        private ObjectField obtainedIconField;
        private ColorField obtainedColorField;
        private ObjectField maxedIconField;
        private ColorField maxedColorField;
        private EnumField sizeField;
        private ObjectField demoClipField;
        private Label skillAmountLabel;

        private Skill selectedSkill;
        private List<Skill> allSkills = new();
        private List<Skill> allSkillsFiltered = new();
        private Dictionary<Skill, SkillEditorElement> loadedSkillElements = new();
        private int amountPerPage = 20;
        private int currentPage = 1;
        private int maxPages = 1;
        private int currentPageFiltered = 1;
        private int maxPagesFiltered = 1;
        private bool filterOn;

        [MenuItem("Window/Skill Web/Skill Bank")]
        public static void Open()
        {
            SkillBankEditorWindow wnd = GetWindow<SkillBankEditorWindow>();
            wnd.titleContent = new GUIContent("Skill Bank");
        }

        private void CreateGUI()
        {
            minSize = new Vector2(700, 500);

            VisualElement root = rootVisualElement;
            var visualTree = AssetSearch.Find<VisualTreeAsset>("SkillWeb", "Scripts/Editor/SkillBankEditorWindow.uxml");
            visualTree.CloneTree(root);

            if (!SkillWeb.Settings)
            {
                SkillWebSettingsEditorWindow.GetOrCreateSettings();
            }

            searchField = root.Q<ToolbarSearchField>("SearchField");
            tagFilter = root.Q<DropdownField>("TagFilter");
            sizeFilter = root.Q<DropdownField>("SizeFilter");
            skillScrollContent = root.Q<VisualElement>("SkillScrollContent");
            inspectorScroll = root.Q<ScrollView>("InspectorScroll");
            createButton = root.Q<Button>("CreateButton");
            deleteButton = root.Q<Button>("DeleteButton");
            previousPageButton = root.Q<Button>("PreviousPageButton");
            nextPageButton = root.Q<Button>("NextPageButton");
            pagesLabel = root.Q<Label>("PagesLabel");
            generalFoldout = root.Q<Foldout>("GeneralFoldout");
            datasetFoldout = root.Q<Foldout>("DatasetFoldout");
            nameField = root.Q<TextField>("NameField");
            tagField = root.Q<DropdownField>("TagField");
            maxLevelField = root.Q<IntegerField>("MaxLevelField");
            levelRequirementField = root.Q<IntegerField>("LevelRequirementField");
            lockedIconField = root.Q<ObjectField>("LockedIconField");
            lockedColorField = root.Q<ColorField>("LockedColorField");
            unlockedIconField = root.Q<ObjectField>("UnlockedIconField");
            unlockedColorField = root.Q<ColorField>("UnlockedColorField");
            obtainedIconField = root.Q<ObjectField>("ObtainedIconField");
            obtainedColorField = root.Q<ColorField>("ObtainedColorField");
            maxedIconField = root.Q<ObjectField>("MaxedIconField");
            maxedColorField = root.Q<ColorField>("MaxedColorField");
            sizeField = root.Q<EnumField>("SizeField");
            demoClipField = root.Q<ObjectField>("DemoClipField");
            skillAmountLabel = root.Q<Label>("SkillAmountLabel");

            createButton.clicked += CreateSkill;
            deleteButton.clicked += DeleteSkill;
            previousPageButton.clicked += PreviousPage;
            nextPageButton.clicked += NextPage;

            ResetFilters();

            searchField.RegisterValueChangedCallback(x => { ApplyFilter(); });
            tagFilter.RegisterValueChangedCallback(x => { ApplyFilter(); });
            sizeFilter.RegisterValueChangedCallback(x => { ApplyFilter(); });

            nameField.RegisterCallback<FocusOutEvent>(x =>
            {
                if (!selectedSkill)
                {
                    return;
                }

                if (loadedSkillElements.ContainsKey(selectedSkill))
                {
                    loadedSkillElements[selectedSkill].Refresh();
                }

                selectedSkill.UpdateAssetName();
            });

            tagField.RegisterValueChangedCallback(x => 
            {
                if (!selectedSkill)
                {
                    return;
                }
                
                selectedSkill.tagIndex = tagField.index;
                selectedSkill.Save();
            });

            lockedIconField.RegisterValueChangedCallback(x =>
            {
                if (!selectedSkill)
                {
                    return;
                }

                if (loadedSkillElements.ContainsKey(selectedSkill))
                {
                    loadedSkillElements[selectedSkill].Refresh();
                }
            });

            lockedColorField.RegisterValueChangedCallback(x =>
            {
                if (!selectedSkill)
                {
                    return;
                }

                if (loadedSkillElements.ContainsKey(selectedSkill))
                {
                    loadedSkillElements[selectedSkill].Refresh();
                }
            });

            unlockedIconField.RegisterValueChangedCallback(x =>
            {
                if (!selectedSkill)
                {
                    return;
                }

                if (loadedSkillElements.ContainsKey(selectedSkill))
                {
                    loadedSkillElements[selectedSkill].Refresh();
                }
            });

            unlockedColorField.RegisterValueChangedCallback(x =>
            {
                if (!selectedSkill)
                {
                    return;
                }

                if (loadedSkillElements.ContainsKey(selectedSkill))
                {
                    loadedSkillElements[selectedSkill].Refresh();
                }
            });

            obtainedIconField.RegisterValueChangedCallback(x =>
            {
                if (!selectedSkill)
                {
                    return;
                }

                if (loadedSkillElements.ContainsKey(selectedSkill))
                {
                    loadedSkillElements[selectedSkill].Refresh();
                }
            });

            obtainedColorField.RegisterValueChangedCallback(x =>
            {
                if (!selectedSkill)
                {
                    return;
                }

                if (loadedSkillElements.ContainsKey(selectedSkill))
                {
                    loadedSkillElements[selectedSkill].Refresh();
                }
            });

            maxedIconField.RegisterValueChangedCallback(x =>
            {
                if (!selectedSkill)
                {
                    return;
                }

                if (loadedSkillElements.ContainsKey(selectedSkill))
                {
                    loadedSkillElements[selectedSkill].Refresh();
                }
            });

            maxedColorField.RegisterValueChangedCallback(x =>
            {
                if (!selectedSkill)
                {
                    return;
                }

                if (loadedSkillElements.ContainsKey(selectedSkill))
                {
                    loadedSkillElements[selectedSkill].Refresh();
                }
            });

            SkillWebDatabase.Initialize();

            DeselectSkill();
            GetAllSkills();
            UpdateSkillAmountLabel();
        }

        private void OnDestroy()
        {
            if (!Application.isPlaying)
            {
                SkillWebDatabase.Disconnect();
            }
        }

        public void ResetFilters()
        {
            List<string> tagChoices = new List<string>() { "Any" };

            if (SkillWeb.Settings)
            {
                foreach (string tag in SkillWeb.Settings.tags)
                {
                    tagChoices.Add(tag);
                }
            }

            tagFilter.SetValueWithoutNotify(tagChoices[0]);
            tagFilter.choices = tagChoices;

            List<string> sizeChoices = new List<string>() { "Any", "Tiny", "Small", "Medium", "Large", "Giant" };
            sizeFilter.SetValueWithoutNotify(sizeChoices[0]);
            sizeFilter.choices = sizeChoices;

            searchField.SetValueWithoutNotify(string.Empty);

            ApplyFilter();
        }

        private void CreateSkill()
        {
            var skill = Skill.Create();
            allSkills.Add(skill);
            RecalculatePages();
            currentPage = maxPages;
            RefreshPagesLabel();
            ReloadCurrentPage();
            SelectSkill(skill);
            UpdateSkillAmountLabel();
        }

        private void DeleteSkill()
        {
            if (!selectedSkill)
            {
                return;
            }

            allSkills.Remove(selectedSkill);

            if (selectedSkill.dataset)
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(selectedSkill.dataset));
            }

            selectedSkill.DeleteDatabaseRecord();
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(selectedSkill));
            RecalculatePages();
            DeselectSkill();
            ReloadCurrentPage();
            UpdateSkillAmountLabel();
        }

        private void SelectSkill(Skill skill)
        {
            DeselectSkill();
            selectedSkill = skill;

            if (skill && loadedSkillElements.ContainsKey(skill))
            {
                loadedSkillElements[skill].selected = true;
                loadedSkillElements[skill].Refresh();
            }

            UpdateInspector();
        }

        private void DeselectSkill()
        {
            if (selectedSkill && loadedSkillElements.ContainsKey(selectedSkill))
            {
                loadedSkillElements[selectedSkill].selected = false;
                loadedSkillElements[selectedSkill].Refresh();
            }

            selectedSkill = null;
            UpdateInspector();
        }

        private void UpdateInspector()
        {
            if (selectedSkill)
            {
                var serializedObject = new SerializedObject(selectedSkill);
                nameField.BindProperty(serializedObject.FindProperty("skillName"));
                maxLevelField.BindProperty(serializedObject.FindProperty("maxLevel"));
                levelRequirementField.BindProperty(serializedObject.FindProperty("levelRequirement"));
                lockedIconField.BindProperty(serializedObject.FindProperty("lockedIcon").FindPropertyRelative("icon"));
                lockedColorField.BindProperty(serializedObject.FindProperty("lockedIcon").FindPropertyRelative("color"));
                unlockedIconField.BindProperty(serializedObject.FindProperty("unlockedIcon").FindPropertyRelative("icon"));
                unlockedColorField.BindProperty(serializedObject.FindProperty("unlockedIcon").FindPropertyRelative("color"));
                obtainedIconField.BindProperty(serializedObject.FindProperty("obtainedIcon").FindPropertyRelative("icon"));
                obtainedColorField.BindProperty(serializedObject.FindProperty("obtainedIcon").FindPropertyRelative("color"));
                maxedIconField.BindProperty(serializedObject.FindProperty("maxedIcon").FindPropertyRelative("icon"));
                maxedColorField.BindProperty(serializedObject.FindProperty("maxedIcon").FindPropertyRelative("color"));
                sizeField.BindProperty(serializedObject.FindProperty("size"));
                demoClipField.BindProperty(serializedObject.FindProperty("demoClip"));

                List<string> choices = new List<string>() { "None" };

                foreach (string tag in SkillWeb.Settings.tags)
                {
                    choices.Add(tag);
                }

                if (selectedSkill.tagIndex < 0 || selectedSkill.tagIndex >= choices.Count)
                {
                    Debug.LogWarning($"Skill Bank: missing tag detected. The tag for the skill {selectedSkill.skillName} has been set to none.");
                    selectedSkill.tagIndex = 0;
                }

                tagField.choices = choices;

                try
                {
                    tagField.SetValueWithoutNotify(choices[selectedSkill.tagIndex]);
                }
                catch 
                {
                    tagField.SetValueWithoutNotify("None");
                }
  
                datasetFoldout.Clear();

                if (selectedSkill.dataset)
                {
                    var serializedDataset = new SerializedObject(selectedSkill.dataset);
                    InspectorElement inspector = new InspectorElement(serializedDataset);
                    inspector.style.paddingLeft = 0;
                    datasetFoldout.Add(inspector);
                }

                inspectorScroll.style.display = DisplayStyle.Flex;
            }
            else
            {
                inspectorScroll.style.display = DisplayStyle.None;
            }
        }

        private void GetAllSkills()
        {
            allSkills = SkillWeb.GetAllSkills().OrderBy(x => x.id).ToList();
            RecalculatePages();
            ReloadCurrentPage();
        }

        private void ApplyFilter()
        {
            filterOn = tagFilter.index != 0 || sizeFilter.index != 0 || !string.IsNullOrEmpty(searchField.value);

            allSkillsFiltered = allSkills.Where(x =>
            {
                bool tagMatches = tagFilter.index == 0 || tagFilter.index == x.tagIndex;
                bool sizeMatches = sizeFilter.index == 0 || (Skill.Size)(sizeFilter.index - 1) == x.size;
                bool nameMatches = x.skillName.ToLower().Contains(searchField.value.ToLower());
                return tagMatches && sizeMatches && nameMatches;
            }).ToList();

            RecalculatePages();
            ReloadCurrentPage();
        }

        private void ReloadCurrentPage()
        {
            loadedSkillElements.Clear();
            skillScrollContent.Clear();

            if (!filterOn)
            {
                for (int i = (currentPage - 1) * amountPerPage; i < allSkills.Count && i < currentPage * amountPerPage; i++)
                {
                    var skill = allSkills[i];
                    var skillElement = new SkillEditorElement();
                    skillElement.SetSkill(skill, selectedSkill == skill);

                    skillElement.clicked += () =>
                    {
                        SelectSkill(skill);
                    };

                    skillScrollContent.Add(skillElement);
                    loadedSkillElements.Add(skill, skillElement);
                }
            }
            else
            {
                for (int i = (currentPageFiltered - 1) * amountPerPage; i < allSkillsFiltered.Count && i < currentPageFiltered * amountPerPage; i++)
                {
                    var skill = allSkillsFiltered[i];
                    var skillElement = new SkillEditorElement();
                    skillElement.SetSkill(skill, selectedSkill == skill);

                    skillElement.clicked += () =>
                    {
                        SelectSkill(skill);
                    };

                    skillScrollContent.Add(skillElement);
                    loadedSkillElements.Add(skill, skillElement);
                }
            }
        }

        private void RecalculatePages()
        {
            if (allSkills.Count == 0)
            {
                currentPage = 1;
                maxPages = 1;
                currentPageFiltered = 1;
                maxPagesFiltered = 1;
            }
            else
            {
                if (!filterOn)
                {
                    maxPages = Mathf.CeilToInt(allSkills.Count / (float)amountPerPage);

                    if (maxPages == 0)
                    {
                        maxPages = 1;
                    }

                    if (currentPage == 0)
                    {
                        currentPage = 1;
                    }

                    if (currentPage > maxPages)
                    {
                        currentPage = maxPages;
                    }
                }
                else
                {
                    maxPagesFiltered = Mathf.CeilToInt(allSkillsFiltered.Count / (float)amountPerPage);

                    if (maxPagesFiltered == 0)
                    {
                        maxPagesFiltered = 1;
                    }

                    if (currentPageFiltered == 0)
                    {
                        currentPageFiltered = 1;
                    }

                    if (currentPageFiltered > maxPagesFiltered)
                    {
                        currentPageFiltered = maxPagesFiltered;
                    }
                }
            }

            RefreshPagesLabel();
        }

        private void RefreshPagesLabel()
        {
            if (!filterOn)
            {
                pagesLabel.text = $"{currentPage}/{maxPages}";
            }
            else
            {
                pagesLabel.text = $"{currentPageFiltered}/{maxPagesFiltered}";
            }
        }

        private void PreviousPage()
        {
            if (!filterOn)
            {
                if (currentPage == 1)
                {
                    return;
                }

                currentPage--;
            }
            else
            {
                if (currentPageFiltered == 1)
                {
                    return;
                }

                currentPageFiltered--;
            }

            RefreshPagesLabel();
            ReloadCurrentPage();
            UpdateSkillAmountLabel();
        }

        private void NextPage()
        {
            if (!filterOn)
            {
                if (currentPage == maxPages)
                {
                    return;
                }

                currentPage++;
            }
            else
            {
                if (currentPageFiltered == maxPages)
                {
                    return;
                }

                currentPageFiltered++;
            }

            RefreshPagesLabel();
            ReloadCurrentPage();
            UpdateSkillAmountLabel();
        }

        private void UpdateSkillAmountLabel()
        {
            if (allSkills.Count == 0)
            {
                skillAmountLabel.text = "0-0 of 0";
                return;
            }

            int totalCount;
            int displayMin;
            int displayMax;

            if (!filterOn)
            {
                totalCount = allSkills.Count;
                displayMin = amountPerPage * (currentPage - 1) + 1;
                displayMax = Math.Min(totalCount, amountPerPage * currentPage);
            }
            else
            {
                totalCount = allSkillsFiltered.Count;
                displayMin = amountPerPage * (currentPageFiltered - 1) + 1;
                displayMax = Math.Min(totalCount, amountPerPage * currentPageFiltered);
            }

            skillAmountLabel.text = $"{displayMin}-{displayMax} of {totalCount}";
        }
    }
}
#endif