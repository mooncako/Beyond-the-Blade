//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

#if UNITY_EDITOR
using Esper.SkillWeb.Database;
using Esper.SkillWeb.Graph;
using Esper.SkillWeb.Settings;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Esper.SkillWeb.Editor
{
    public class WebCreatorEditorWindow : EditorWindow
    {
        private DraggableEditorElement webList;
        private Button webListResizeButton;
        private ToolbarSearchField searchField;
        private VisualElement webListContent;
        private VisualElement webListScrollContent;
        private DraggableEditorElement datasetContainer;
        private VisualElement datasetContent;
        private Button datasetResizeButton;
        private Button createButton;
        private Button deleteButton;
        private Button previousPageButton;
        private Button nextPageButton;
        private Label pagesLabel;
        private VisualElement webInspector;
        private TextField webNameField;
        private WebView webView;
        private Button saveButton;

        private int amountPerPage = 10;
        private int currentPage = 1;
        private int maxPages = 1;
        private int currentPageFiltered = 1;
        private int maxPagesFiltered = 1;
        private Vector2 originalWebListSize;
        private Vector2 originalDatasetSize;
        private bool isWebListMinimized;
        private bool isDatasetMinimized;
        private List<WebGraph> allWebs = new();
        private List<WebGraph> allWebsFiltered = new();
        private WebGraph selectedWeb;
        private Dictionary<WebGraph, WebEditorElement> loadedWebElements = new();
        private bool filterOn;


        [MenuItem("Window/Skill Web/Web Creator")]
        public static void Open()
        {
            WebCreatorEditorWindow wnd = GetWindow<WebCreatorEditorWindow>();
            wnd.titleContent = new GUIContent("Web Creator");
        }

        private void CreateGUI()
        {
            minSize = new Vector2(700, 500);

            VisualElement root = rootVisualElement;
            var visualTree = AssetSearch.Find<VisualTreeAsset>("SkillWeb", "Scripts/Editor/WebCreatorEditorWindow.uxml");
            visualTree.CloneTree(root);

            if (!SkillWeb.Settings)
            {
                SkillWebSettingsEditorWindow.GetOrCreateSettings();
            }

            webList = root.Q<DraggableEditorElement>("WebList");
            webListResizeButton = webList.Q<Button>("WebListResizeButton");
            datasetResizeButton = root.Q<Button>("DatasetResizeButton");
            searchField = webList.Q<ToolbarSearchField>("SearchField");
            webListContent = webList.Q<VisualElement>("WebListContent");
            webListScrollContent = webList.Q<VisualElement>("WebListScrollContent");
            createButton = webList.Q<Button>("CreateButton");
            deleteButton = webList.Q<Button>("DeleteButton");
            previousPageButton = webList.Q<Button>("PreviousPageButton");
            nextPageButton = webList.Q<Button>("NextPageButton");
            pagesLabel = webList.Q<Label>("PagesLabel");
            saveButton = root.Q<Button>("SaveButton");
            webInspector = root.Q<VisualElement>("WebInspector");
            webNameField = root.Q<TextField>("WebNameField");
            webView = root.Q<WebView>("WebView");
            datasetContainer = root.Q<DraggableEditorElement>("DatasetContainer");
            datasetContent = root.Q<VisualElement>("DatasetContent");

            var skillFinderRoot = root.Q<Toolbar>("SkillFinder");
            webView.Initialize(skillFinderRoot);
            webView.onGraphChanged.AddListener(OnGraphChanged);

            webNameField.RegisterCallback<FocusOutEvent>(x =>
            {
                if (!selectedWeb)
                {
                    return;
                }

                if (loadedWebElements.ContainsKey(selectedWeb))
                {
                    loadedWebElements[selectedWeb].Refresh();
                }

                selectedWeb.UpdateAssetName();
            });

            webListResizeButton.clicked += ToggleWebListResize;
            webListResizeButton.RegisterCallback<PointerUpEvent>(x => webList.ForceStopDrag());
            webList.RegisterCallback<GeometryChangedEvent>(StoreOriginalWebListSize);
            webList.RegisterCallback<GeometryChangedEvent>((x) => webList.ClampOnScreen());

            createButton.RegisterCallback<PointerUpEvent>(x => webList.ForceStopDrag());
            deleteButton.RegisterCallback<PointerUpEvent>(x => webList.ForceStopDrag());
            previousPageButton.RegisterCallback<PointerUpEvent>(x => webList.ForceStopDrag());
            nextPageButton.RegisterCallback<PointerUpEvent>(x => webList.ForceStopDrag());
            searchField.RegisterCallback<FocusEvent>(x => webList.ForceStopDrag());
            searchField.RegisterCallback<PointerLeaveEvent>(x => webList.ForceStopDrag());
            searchField.RegisterCallback<PointerUpEvent>(x => webList.ForceStopDrag());

            datasetResizeButton.clicked += ToggleDatasetResize;
            datasetResizeButton.RegisterCallback<PointerUpEvent>(x => datasetContainer.ForceStopDrag());
            datasetContainer.RegisterCallback<GeometryChangedEvent>(StoreOriginalDatasetSize);
            datasetContainer.RegisterCallback<GeometryChangedEvent>((x) => datasetContainer.ClampOnScreen());

            root.RegisterCallback<GeometryChangedEvent>((x) =>
            {
                webList.ClampOnScreen();
                datasetContainer.ClampOnScreen();
            });

            createButton.clicked += CreateWeb;
            deleteButton.clicked += DeleteWeb;
            previousPageButton.clicked += PreviousPage;
            nextPageButton.clicked += NextPage;
            searchField.RegisterValueChangedCallback(x => { ApplyFilter(); });
            saveButton.clicked += webView.Save;
            saveButton.style.display = DisplayStyle.None;

            SkillWebDatabase.Initialize();

            DeselectWeb();
            GetAllWebs();
        }

        private void OnDestroy()
        {
            if (!Application.isPlaying)
            {
                SkillWebDatabase.Disconnect();
            }

            AskToSave();
        }

        [Shortcut("Web Editor/Save", typeof(WebCreatorEditorWindow), KeyCode.S, ShortcutModifiers.Action)]
        private static void SaveShortcut()
        {
            if (HasOpenInstances<WebCreatorEditorWindow>())
            {
                var window = GetWindow<WebCreatorEditorWindow>();
                window.webView.Save();
            }
        }

        [Shortcut("Web Editor/Delete", typeof(WebCreatorEditorWindow), KeyCode.Delete)]
        private static void DeleteShortcut()
        {
            if (HasOpenInstances<WebCreatorEditorWindow>())
            {
                var window = GetWindow<WebCreatorEditorWindow>();
                window.webView.DeleteSelection();
            }
        }

        [Shortcut("Web Editor/Copy", typeof(WebCreatorEditorWindow), KeyCode.C, ShortcutModifiers.Action)]
        private static void CopyShortcut()
        {
            if (HasOpenInstances<WebCreatorEditorWindow>())
            {
                var window = GetWindow<WebCreatorEditorWindow>();
                window.webView.CopySelection();
            }
        }

        [Shortcut("Web Editor/Paste", typeof(WebCreatorEditorWindow), KeyCode.V, ShortcutModifiers.Action)]
        private static void PasteShortcut()
        {
            if (HasOpenInstances<WebCreatorEditorWindow>())
            {
                var window = GetWindow<WebCreatorEditorWindow>();
                window.webView.PasteSelection();
            }
        }

        [Shortcut("Web Editor/Cut", typeof(WebCreatorEditorWindow), KeyCode.X, ShortcutModifiers.Action)]
        private static void CutShortcut()
        {
            if (HasOpenInstances<WebCreatorEditorWindow>())
            {
                var window = GetWindow<WebCreatorEditorWindow>();
                window.webView.CutSelection();
            }
        }

        private void OnGraphChanged()
        {
            if (webView == null || !webView.webGraph)
            {
                return;
            }

            webView.webGraph.changeGuid = SkillWebUtility.GenerateNewGuid();

            EditorApplication.delayCall += () =>
            {
                if (webView.HasUnsavedChanges)
                {
                    saveButton.style.display = DisplayStyle.Flex;
                }
                else
                {
                    saveButton.style.display = DisplayStyle.None;
                }
            };
        }

        private void AskToSave()
        {
            if (!webView.HasUnsavedChanges || !webView.webGraph)
            {
                return;
            }

            if (EditorUtility.DisplayDialog("Unsaved Changes", "You have unsaved changes. If the graph is not saved, all changes will be reverted. Would you like to save the graph?", "Save", "Revert"))
            {
                webView.Save();
            }
            else
            {
                webView.Revert();
            }
        }

        private void StoreOriginalWebListSize(GeometryChangedEvent evt)
        {
            originalWebListSize = new Vector2(webList.resolvedStyle.width, webList.resolvedStyle.height);
            webList.UnregisterCallback<GeometryChangedEvent>(StoreOriginalWebListSize);
        }

        private void StoreOriginalDatasetSize(GeometryChangedEvent evt)
        {
            originalDatasetSize = new Vector2(datasetContainer.resolvedStyle.width, datasetContainer.resolvedStyle.height);
            datasetContainer.UnregisterCallback<GeometryChangedEvent>(StoreOriginalDatasetSize);
        }

        private void ToggleWebListResize()
        {
            if (isWebListMinimized)
            {
                isWebListMinimized = false;
                webList.style.width = originalWebListSize.x;
                webList.style.height = originalWebListSize.y;
                webListContent.style.display = DisplayStyle.Flex;
                webListResizeButton.text = "-";
            }
            else
            {
                isWebListMinimized = true;
                webList.style.width = 50;
                webList.style.height = 50;
                webListContent.style.display = DisplayStyle.None;
                webListResizeButton.text = "+";
            }

            webList.ForceStopDrag();
        }

        private void ToggleDatasetResize()
        {
            if (isDatasetMinimized)
            {
                isDatasetMinimized = false;
                datasetContainer.style.width = originalDatasetSize.x;
                datasetContainer.style.height = originalDatasetSize.y;
                datasetContent.style.display = DisplayStyle.Flex;
                datasetResizeButton.text = "-";
            }
            else
            {
                isDatasetMinimized = true;
                datasetContainer.style.width = 80;
                datasetContainer.style.height = 80;
                datasetContent.style.display = DisplayStyle.None;
                datasetResizeButton.text = "+";
            }

            datasetContainer.ForceStopDrag();
        }

        private void CreateWeb()
        {
            var web = WebGraph.Create();
            allWebs.Add(web);
            RecalculatePages();
            currentPage = maxPages;
            ReloadCurrentPage();
            SelectWeb(web);
        }

        private void DeleteWeb()
        {
            if (!selectedWeb)
            {
                return;
            }

            if (!EditorUtility.DisplayDialog($"Delete {selectedWeb.webName}?", "Are you sure you want to delete this web? This cannot be undone.", "Delete", "Cancel"))
            {
                return;
            }

            if (webView.webGraph == selectedWeb)
            {
                webView.DisplayWeb(null);
            }

            allWebs.Remove(selectedWeb);

            if (selectedWeb.dataset)
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(selectedWeb.dataset));
            }

            selectedWeb.DeleteDatabaseRecord();
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(selectedWeb));
            RecalculatePages();
            DeselectWeb();
            ReloadCurrentPage();
        }

        private void SelectWeb(WebGraph web)
        {
            if (selectedWeb)
            {
                Undo.ClearUndo(selectedWeb);
                AskToSave();
            }

            DeselectWeb();
            selectedWeb = web;

            if (string.IsNullOrEmpty(selectedWeb.changeGuid))
            {
                selectedWeb.changeGuid = SkillWebUtility.GenerateNewGuid();
            }

            foreach (var skillNode in selectedWeb.skillNodes)
            {
                if (string.IsNullOrEmpty(skillNode.guid))
                {
                    skillNode.guid = SkillWebUtility.GenerateNewGuid();
                }
            }

            if (web && loadedWebElements.ContainsKey(web))
            {
                loadedWebElements[web].selected = true;
                loadedWebElements[web].Refresh();
            }

            webView.DisplayWeb(selectedWeb);
            datasetContent.Clear();

            if (selectedWeb.dataset)
            {
                var serializedDataset = new SerializedObject(selectedWeb.dataset);
                InspectorElement inspector = new InspectorElement(serializedDataset);
                inspector.style.paddingLeft = 0;
                datasetContent.Add(inspector);

                datasetContent.parent.RegisterCallback<FocusEvent>(x => datasetContainer.ForceStopDrag(), TrickleDown.TrickleDown);
                datasetContent.parent.RegisterCallback<PointerLeaveEvent>(x => datasetContainer.ForceStopDrag(), TrickleDown.TrickleDown);
                datasetContent.parent.RegisterCallback<PointerUpEvent>(x => datasetContainer.ForceStopDrag(), TrickleDown.TrickleDown);
            }

            datasetContainer.style.visibility = Visibility.Visible;
            UpdateInspector();
        }

        private void DeselectWeb()
        {
            if (selectedWeb && loadedWebElements.ContainsKey(selectedWeb))
            {
                loadedWebElements[selectedWeb].selected = false;
                loadedWebElements[selectedWeb].Refresh();
            }

            selectedWeb = null;
            datasetContainer.style.visibility = Visibility.Hidden;
            UpdateInspector();
        }

        private void UpdateInspector()
        {
            if (selectedWeb)
            {
                var serializedObject = new SerializedObject(selectedWeb);
                webNameField.BindProperty(serializedObject.FindProperty("webName"));
                webInspector.style.display = DisplayStyle.Flex;
            }
            else
            {
                webInspector.style.display = DisplayStyle.None;
            }
        }

        private void GetAllWebs()
        {
            allWebs = SkillWeb.GetAllWebGraphs().OrderBy(x => x.id).ToList();
            RecalculatePages();
            ReloadCurrentPage();
        }

        private void ApplyFilter()
        {
            filterOn = !string.IsNullOrEmpty(searchField.value);
            allWebsFiltered = allWebs.Where(x => x.webName.ToLower().Contains(searchField.value.ToLower())).ToList();
            RecalculatePages();
            ReloadCurrentPage();
        }

        private void ReloadCurrentPage()
        {
            loadedWebElements.Clear();
            webListScrollContent.Clear();

            if (!filterOn)
            {
                for (int i = (currentPage - 1) * amountPerPage; i < allWebs.Count && i < currentPage * amountPerPage; i++)
                {
                    var web = allWebs[i];
                    var webElement = new WebEditorElement();
                    webElement.SetWeb(web, selectedWeb == web);

                    webElement.clicked += () =>
                    {
                        SelectWeb(web);
                    };

                    webElement.RegisterCallback<PointerUpEvent>(x => webList.ForceStopDrag());

                    webListScrollContent.Add(webElement);
                    loadedWebElements.Add(web, webElement);
                }
            }
            else
            {
                for (int i = (currentPageFiltered - 1) * amountPerPage; i < allWebsFiltered.Count && i < currentPageFiltered * amountPerPage; i++)
                {
                    var web = allWebsFiltered[i];
                    var webElement = new WebEditorElement();
                    webElement.SetWeb(web, selectedWeb == web);

                    webElement.clicked += () =>
                    {
                        SelectWeb(web);
                    };

                    webElement.RegisterCallback<PointerUpEvent>(x => webList.ForceStopDrag());

                    webListScrollContent.Add(webElement);
                    loadedWebElements.Add(web, webElement);
                }
            }
        }

        private void RecalculatePages()
        {
            if (allWebs.Count == 0)
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
                    maxPages = Mathf.CeilToInt(allWebs.Count / (float)amountPerPage);

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
                    maxPagesFiltered = Mathf.CeilToInt(allWebsFiltered.Count / (float)amountPerPage);

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
        }

        private void OnEnable()
        {
            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedo;
        }

        private void OnUndoRedo()
        {
            webView.Refresh();
            webView.onGraphChanged?.Invoke();
        }
    }
}
#endif