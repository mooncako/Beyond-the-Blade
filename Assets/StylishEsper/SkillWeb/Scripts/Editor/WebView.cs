//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditor.UIElements;
using Esper.SkillWeb.Graph;
using UnityEngine.Events;

namespace Esper.SkillWeb.Editor
{
#if !UNITY_2022
    [UxmlElement]
    public partial class WebView : GraphView
    {
#elif UNITY_2022
    public class WebView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<WebView, UxmlTraits> { }
#endif
        public WebGraph webGraph;
        public UnityEvent onGraphChanged = new();
        private SkillFinder skillFinder;
        private Dictionary<int, SkillNodeEditor> loadedNodeElements = new();
        private SkillNodeEditor nodeToReplace;
        private WebGraph temp;
        private List<Graph.GraphElement> copiedElements = new();

        private Vector2 mouseDownPosition;
        private Vector2 mouseUpPosition;
        private Vector2 mousePosition;
        private Vector2 unscaledMouseDownPosition;
        private Vector2 unscaledMouseUpPosition;
        private Vector2 unscaledMousePosition;

        private bool graphChangeLock;

        public bool HasUnsavedChanges
        {
            get
            {
                if (!webGraph)
                {
                    return false;
                }

                return EditorUtility.IsDirty(webGraph);
            }
        }

        public WebView()
        {
            Insert(0, new GridBackground());

            var contentZoomer = new ContentZoomer();
            contentZoomer.minScale = 0.05f;
            contentZoomer.maxScale = 2f;

            this.AddManipulator(contentZoomer);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            RegisterCallback<MouseDownEvent>(evt =>
            {
                unscaledMouseDownPosition = evt.localMousePosition;
                var view = viewTransform as VisualElement;
                mouseDownPosition = (evt.localMousePosition - (Vector2)view.resolvedStyle.translate) / scale;
            });

            RegisterCallback<MouseUpEvent>(evt =>
            {
                unscaledMouseUpPosition = evt.localMousePosition;
                var view = viewTransform as VisualElement;
                mouseUpPosition = (evt.localMousePosition - (Vector2)view.resolvedStyle.translate) / scale;

                if (evt.button == 0)
                {
                    CloseSkillFinder();
                }
            });

            RegisterCallback<MouseMoveEvent>(evt =>
            {
                unscaledMousePosition = evt.localMousePosition;
                var view = viewTransform as VisualElement;
                mousePosition = (evt.localMousePosition - (Vector2)view.resolvedStyle.translate) / scale;
            });

            var styleSheet = AssetSearch.Find<StyleSheet>("SkillWeb", "Scripts/Editor/WebCreatorEditorWindow.uss");
            styleSheets.Add(styleSheet);

            graphViewChanged += OnGraphChanged;
        }

        public void Initialize(Toolbar skillFinderRoot)
        {
            skillFinder = new SkillFinder(OnSkillSelected, skillFinderRoot);
            CloseSkillFinder();
        }

        public void OnSkillSelected(Skill skill)
        {
            if (nodeToReplace == null)
            {
                CreateNewSkillNode(skill);
            }
            else
            {
                ReplaceSkillOfSelection(skill);
            }
        }

        private GraphViewChange OnGraphChanged(GraphViewChange graphViewChange)
        {
            if (!webGraph || graphChangeLock)
            {
                return graphViewChange;
            }

            Undo.RegisterCompleteObjectUndo(webGraph, "Graph Change");

            if (graphViewChange.elementsToRemove != null)
            {
                foreach (var item in graphViewChange.elementsToRemove)
                {
                    if (item is Edge)
                    {
                        var edge = item as Edge;
                        var outputNode = (SkillNodeEditor)edge.output.node;
                        var inputNode = (SkillNodeEditor)edge.input.node;
                        var outputPortIndex = 0;
                        var inputPortIndex = 0;

                        for (int i = 0; i < edge.output.node.outputContainer.childCount; i++)
                        {
                            if (edge.output == (Port)edge.output.node.outputContainer.ElementAt(i))
                            {
                                outputPortIndex = i;
                                break;
                            }
                        }

                        for (int i = 0; i < edge.input.node.inputContainer.childCount; i++)
                        {
                            if (edge.input == (Port)edge.input.node.inputContainer.ElementAt(i))
                            {
                                inputPortIndex = i;
                                break;
                            }
                        }

                        var connectionToRemove = new Connection(outputNode.Id, inputNode.Id, outputPortIndex, inputPortIndex, 0);

                        foreach (var connection in webGraph.connections)
                        {
                            if (connection.Matches(connectionToRemove))
                            {
                                webGraph.connections.Remove(connection);
                                break;
                            }
                        }
                    }
                    else if (item is SkillNodeEditor)
                    {
                        var node = item as SkillNodeEditor;
                        webGraph.skillNodes.Remove(node.Value);
                        loadedNodeElements.Remove(node.Id);
                    }
                }
            }

            if (graphViewChange.edgesToCreate != null)
            {
                foreach (var edge in graphViewChange.edgesToCreate)
                {
                    var connection = GetConnectionData(edge);
                    bool matchFound = false;

                    foreach (var existingConnection in webGraph.connections)
                    {
                        if (existingConnection.Matches(connection))
                        {
                            schedule.Execute(x =>
                            {
                                edge.input.Disconnect(edge);
                                edge.output.Disconnect(edge);
                                RemoveElement(edge);
                            });

                            matchFound = true;
                            break;
                        }
                    }

                    if (!matchFound)
                    {
                        webGraph.connections.Add(connection);
                        edge.edgeControl.RegisterCallback<GeometryChangedEvent>(x => UpdateEdgeIndexField(edge, connection));
                    }
                }
            }

            if (graphViewChange.movedElements != null)
            {
                foreach (var item in graphViewChange.movedElements)
                {
                    if (item is SkillNodeEditor)
                    {
                        var node = item as SkillNodeEditor;
                        node.Value.position = new Vector2(item.resolvedStyle.left, item.resolvedStyle.top);
                        node.positionField.SetValueWithoutNotify(node.Value.position);
                    }
                }
            }

            EditorUtility.SetDirty(webGraph);
            onGraphChanged.Invoke();
            return graphViewChange;
        }

        private void UpdateEdgeIndexField(Edge edge, Connection connection)
        {
            var indexField = edge.Q<IntegerField>("PrefabIndexField");

            if (indexField == null)
            {
                indexField = new IntegerField();
                indexField.name = "PrefabIndexField";

                indexField.SetValueWithoutNotify(connection.styleIndex);
                indexField.RegisterValueChangedCallback(x =>
                {
                    if (x.newValue < 0)
                    {
                        indexField.SetValueWithoutNotify(0);
                        return;
                    }

                    Undo.RegisterCompleteObjectUndo(webGraph, "Graph Change");
                    connection.styleIndex = x.newValue;
                    EditorUtility.SetDirty(webGraph);
                    onGraphChanged.Invoke();
                });

                edge.Add(indexField);
            }

            var view = viewTransform as VisualElement;
            var center = (edge.edgeControl.worldBound.center - (Vector2)view.resolvedStyle.translate - new Vector2(0, 21)) / scale;
            indexField.style.left = center.x;
            indexField.style.top = center.y;
        }

        public Connection GetConnectionData(Edge edge)
        {
            var outputNode = (SkillNodeEditor)edge.output.node;
            var inputNode = (SkillNodeEditor)edge.input.node;
            var outputPortIndex = 0;
            var inputPortIndex = 0;

            for (int i = 0; i < edge.output.node.outputContainer.childCount; i++)
            {
                if (edge.output == (Port)edge.output.node.outputContainer.ElementAt(i))
                {
                    outputPortIndex = i;
                    break;
                }
            }

            for (int i = 0; i < edge.input.node.inputContainer.childCount; i++)
            {
                if (edge.input == (Port)edge.input.node.inputContainer.ElementAt(i))
                {
                    inputPortIndex = i;
                    break;
                }
            }

            var connection = new Connection(outputNode.Value.id, inputNode.Value.id, outputPortIndex, inputPortIndex, 0);
            connection.webGraphID = webGraph.id;
            return connection;
        }

        public List<Graph.GraphElement> GetSelection()
        {
            var selection = new List<Graph.GraphElement>();

            foreach (var item in this.selection)
            {
                if (item is SkillNodeEditor)
                {
                    var skillNodeElement = item as SkillNodeEditor;
                    selection.Add(skillNodeElement.Value);
                }
                else if (item is Edge)
                {
                    var edge = item as Edge;
                    selection.Add(GetConnectionData(edge));
                }
            }

            return selection;
        }

        public void Refresh()
        {
            DisplayWeb(webGraph);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = ports.Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
            return compatiblePorts;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (!webGraph)
            {
                return;
            }

            if (selection.Count > 0)
            {
                evt.menu.AppendAction("Cut", x => CutSelection());
                evt.menu.AppendAction("Copy", x => CopySelection());
                evt.menu.AppendAction("Delete", x => DeleteSelection());
            }

            if (copiedElements.Count > 0)
            {
                evt.menu.AppendAction("Paste", x => PasteSelection());
            }

            evt.menu.AppendSeparator();

            if (selection.Count == 1 && selection[0] is SkillNodeEditor)
            {
                nodeToReplace = selection[0] as SkillNodeEditor;
                evt.menu.AppendAction("Replace Skill", x => OpenSkillFinder());
            }
            else
            {
                nodeToReplace = null;
                evt.menu.AppendAction("Add Skill", x => OpenSkillFinder());
            }
        }

        public void OpenSkillFinder()
        {
            var position = unscaledMouseUpPosition - new Vector2(0, 20);
            skillFinder.Open(position);
        }

        public void CloseSkillFinder()
        {
            skillFinder.Close();
        }

        public void DisplayWeb(WebGraph web)
        {
            graphChangeLock = true;

            webGraph = web;

            DeleteElements(graphElements);

            if (web)
            {
                temp = web.CreateCopy();
                loadedNodeElements.Clear();

                foreach (var node in web.skillNodes)
                {
                    if (node is SkillNode)
                    {
                        CreateSkillNodeElement(node);
                    }
                }

                foreach (var connection in web.connections)
                {
                    var outputNode = loadedNodeElements[connection.outputNodeID];
                    var inputNode = loadedNodeElements[connection.inputNodeID];
                    var edge = outputNode.outputPorts[connection.outputPortIndex].ConnectTo(inputNode.inputPorts[connection.inputPortIndex]);
                    edge.edgeControl.RegisterCallback<GeometryChangedEvent>(x => UpdateEdgeIndexField(edge, connection));
                    AddElement(edge);
                }
            }

            graphChangeLock = false;
        }

        public void CreateNewSkillNode(Skill skill)
        {
            Undo.RegisterCompleteObjectUndo(webGraph, "Graph Change");

            var skillNode = new SkillNode(webGraph.GetAvailableNodeID(), skill, mouseUpPosition);
            skillNode.webGraphID = webGraph.id;
            webGraph.skillNodes.Add(skillNode);
            CreateSkillNodeElement(skillNode);
            CloseSkillFinder();

            EditorUtility.SetDirty(webGraph);
            onGraphChanged.Invoke();
        }

        public void CreateSkillNodeElement(SkillNode skillNode)
        {
            var skillNodeElement = new SkillNodeEditor(skillNode);

            skillNodeElement.RegisterCallback<MouseUpEvent>(evt =>
            {
                if (evt.button == 1)
                {
                    var view = viewTransform as VisualElement;
                    mouseUpPosition = (evt.localMousePosition - (Vector2)view.resolvedStyle.translate) / scale;
                    unscaledMouseUpPosition = evt.mousePosition;
                }
            });

            skillNodeElement.positionField.SetValueWithoutNotify(skillNodeElement.Value.position);
            skillNodeElement.positionField.RegisterValueChangedCallback(x =>
            {
                if (skillNodeElement.Value != null)
                {
                    skillNodeElement.Value.position = x.newValue;
                    skillNodeElement.positionField.SetValueWithoutNotify(skillNodeElement.Value.position);
                    skillNodeElement.SetPosition(skillNodeElement.Value.position);
                    EditorUtility.SetDirty(webGraph);
                    onGraphChanged.Invoke();
                }
            });

            skillNodeElement.hasConnectionDependencyToggle.SetValueWithoutNotify(skillNode.hasConnectionDependency);
            skillNodeElement.hasConnectionDependencyToggle.RegisterValueChangedCallback(x =>
            {
                if (skillNodeElement.Value != null)
                {
                    Undo.RegisterCompleteObjectUndo(webGraph, "Graph Change");
                    skillNodeElement.Value.hasConnectionDependency = x.newValue;
                    EditorUtility.SetDirty(webGraph);
                    onGraphChanged.Invoke();
                }
            });

            if (skillNode.dependencyCount == 0)
            {
                skillNode.dependencyCount = 1;
            }

            skillNodeElement.dependencyCountField.SetValueWithoutNotify(skillNode.dependencyCount);
            skillNodeElement.dependencyCountField.RegisterValueChangedCallback(x => 
            {
                if (skillNodeElement.Value == null)
                    return;

                var totalConnections = skillNodeElement.GetTotalConnectionCount();

                if (totalConnections == 0)
                {
                    totalConnections = 1;
                }

                if (x.newValue < 1)
                {
                    skillNodeElement.dependencyCountField.SetValueWithoutNotify(1);
                    return;
                }
                else if (x.newValue > totalConnections)
                {
                    skillNodeElement.dependencyCountField.SetValueWithoutNotify(totalConnections);
                    return;
                }

                Undo.RegisterCompleteObjectUndo(webGraph, "Graph Change");
                var skill = skillNodeElement.Value;
                skill.dependencyCount = x.newValue;

                if (skill.maxedRequirementCount > skill.dependencyCount)
                {
                    skill.maxedRequirementCount = skill.dependencyCount;
                    skillNodeElement.maxedRequirementCountField.SetValueWithoutNotify(skillNode.maxedRequirementCount);
                }

                EditorUtility.SetDirty(webGraph);
                onGraphChanged.Invoke();
            });

            skillNodeElement.dependencyCountField.style.display = skillNodeElement.hasConnectionDependencyToggle.value ? DisplayStyle.Flex : DisplayStyle.None;

            skillNodeElement.maxedRequirementCountField.SetValueWithoutNotify(skillNode.maxedRequirementCount);
            skillNodeElement.maxedRequirementCountField.RegisterValueChangedCallback(x =>
            {
                if (skillNodeElement.Value == null)
                    return;

                if (x.newValue < 0)
                {
                    skillNodeElement.maxedRequirementCountField.SetValueWithoutNotify(0);
                    return;
                }
                else if (x.newValue > skillNode.dependencyCount)
                {
                    skillNodeElement.maxedRequirementCountField.SetValueWithoutNotify(skillNode.dependencyCount);
                    return;
                }

                Undo.RegisterCompleteObjectUndo(webGraph, "Graph Change");
                skillNodeElement.Value.maxedRequirementCount = x.newValue;
                EditorUtility.SetDirty(webGraph);
                onGraphChanged.Invoke();
            });

            skillNodeElement.maxedRequirementCountField.style.display = skillNodeElement.hasConnectionDependencyToggle.value ? DisplayStyle.Flex : DisplayStyle.None;

            skillNodeElement.hideFieldsButton.clicked += () =>
            {
                if (skillNodeElement.Value == null)
                    return;

                var skill = skillNodeElement.Value;
                skill.hideFieldsInWebGraphEditor = !skill.hideFieldsInWebGraphEditor;
                skillNodeElement.UpdateFieldsHiddenState();
                Undo.RegisterCompleteObjectUndo(webGraph, "Graph Change");
                EditorUtility.SetDirty(webGraph);
                onGraphChanged.Invoke();
            };

            loadedNodeElements.Add(skillNodeElement.Id, skillNodeElement);
            AddElement(skillNodeElement);
        }

        public void ReplaceSkillOfSelection(Skill skill)
        {
            if (selection.Count == 1 && selection[0] is SkillNodeEditor)
            {
                nodeToReplace.Value.skill = skill;
                nodeToReplace.Refresh();
                nodeToReplace = null;
                CloseSkillFinder();
                EditorUtility.SetDirty(webGraph);
                onGraphChanged.Invoke();
            }
        }

        public void Save()
        {
            if (!webGraph)
            {
                return;
            }

            webGraph.Save();
            temp = webGraph.CreateCopy();
            onGraphChanged.Invoke();
        }

        public void Revert()
        {
            if (!webGraph)
            {
                return;
            }

            webGraph.CopyData(temp);
            webGraph.Save();
            onGraphChanged.Invoke();
        }

        public void CutSelection()
        {
            CopySelection();
            DeleteSelection();
        }

        public void CopySelection()
        {
            copiedElements.Clear();
            var selection = GetSelection();
            selection = selection.OrderByDescending(x => x is SkillNode).ToList();

            foreach (var element in selection)
            {
                if (element is SkillNode)
                {
                    var skillNode = element as SkillNode;
                    copiedElements.Add(skillNode.CreateCopy());
                }
                else if (element is Connection)
                {
                    var connection = element as Connection;
                    copiedElements.Add(connection.CreateCopy());
                }
            }
        }

        public void PasteSelection()
        {
            Undo.RegisterCompleteObjectUndo(webGraph, "Graph Change");

            Dictionary<int, int> newIds = new();

            var distancesFromFirst = new List<Vector2>() { Vector2.zero };
            SkillNode first = null;

            for (int i = 0; i < copiedElements.Count; i++)
            {
                if (copiedElements[i] is SkillNode)
                {
                    var node = copiedElements[i] as SkillNode;

                    if (first == null)
                    {
                        first = node;
                    }
                    else
                    {
                        distancesFromFirst.Add(node.position - first.position);
                    }
                }
            }

            if (first == null)
            {
                copiedElements.Clear();
                return;
            }

            int count = 0;
            bool firstNode = true;
            foreach (var element in copiedElements)
            {
                if (element is SkillNode)
                {
                    var skillNode = (element as SkillNode).CreateCopy();
                    int prevID = skillNode.id;
                    int newID = webGraph.GetAvailableNodeID();
                    newIds.Add(prevID, newID);
                    skillNode.id = newID;

                    if (firstNode)
                    {
                        skillNode.position = mousePosition;
                        firstNode = false;
                    }
                    else
                    {
                        skillNode.position = mousePosition + distancesFromFirst[count];
                    }

                    webGraph.skillNodes.Add(skillNode);
                    CreateSkillNodeElement(skillNode);

                    count++;
                }
                else if (element is Connection)
                {
                    var connection = (element as Connection).CreateCopy();

                    if (newIds.ContainsKey(connection.outputNodeID))
                    {
                        connection.outputNodeID = newIds[connection.outputNodeID];
                    }

                    if (newIds.ContainsKey(connection.inputNodeID))
                    {
                        connection.inputNodeID = newIds[connection.inputNodeID];
                    }

                    webGraph.connections.Add(connection);
                    var outputNode = loadedNodeElements[connection.outputNodeID];
                    var inputNode = loadedNodeElements[connection.inputNodeID];
                    var edge = outputNode.outputPorts[connection.outputPortIndex].ConnectTo(inputNode.inputPorts[connection.inputPortIndex]);
                    AddElement(edge);
                }
            }

            EditorUtility.SetDirty(webGraph);
            onGraphChanged.Invoke();
        }

        public override EventPropagation DeleteSelection()
        {
            return base.DeleteSelection();
        }
    }
}
#endif