//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

#if UNITY_EDITOR
using Esper.SkillWeb.UI.UGUI;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using ObjectField = UnityEditor.UIElements.ObjectField;

namespace Esper.SkillWeb.Editor
{
    [CustomEditor(typeof(WebViewUGUI))]
    public class WebViewUGUIEditor : UnityEditor.Editor
    {
        private Toggle enableTestingField;
        private IntegerField startingSkillPointsField;
        private ObjectField skillNodeTinyPrefabField;
        private ObjectField skillNodeSmallPrefabField;
        private ObjectField skillNodeMediumPrefabField;
        private ObjectField skillNodeLargePrefabField;
        private ObjectField skillNodeGiantPrefabField;
        private ListView connectionPrefabsField;
        private ObjectField contentField;
        private ObjectField graphContentField;
        private ObjectField boundsField;
        private Toggle loadOnStartField;
        private TextField webNameField;
        private Toggle clampInsideScreen;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement inspector = new VisualElement();
            var visualTree = AssetSearch.Find<VisualTreeAsset>("SkillWeb", "Scripts/Editor/WebViewUGUIEditor.uxml");
            visualTree.CloneTree(inspector);

            enableTestingField = inspector.Q<Toggle>("EnableTestingField");
            startingSkillPointsField = inspector.Q<IntegerField>("StartingSkillPointsField");
            skillNodeTinyPrefabField = inspector.Q<ObjectField>("SkillNodeTinyPrefabField");
            skillNodeSmallPrefabField = inspector.Q<ObjectField>("SkillNodeSmallPrefabField");
            skillNodeMediumPrefabField = inspector.Q<ObjectField>("SkillNodeMediumPrefabField");
            skillNodeLargePrefabField = inspector.Q<ObjectField>("SkillNodeLargePrefabField");
            skillNodeGiantPrefabField = inspector.Q<ObjectField>("SkillNodeGiantPrefabField");
            connectionPrefabsField = inspector.Q<ListView>("ConnectionPrefabsField");
            contentField = inspector.Q<ObjectField>("ContentField");
            graphContentField = inspector.Q<ObjectField>("GraphContentField");
            boundsField = inspector.Q<ObjectField>("BoundsField");
            loadOnStartField = inspector.Q<Toggle>("LoadOnStartField");
            webNameField = inspector.Q<TextField>("WebNameField");
            clampInsideScreen = inspector.Q<Toggle>("ClampInsideScreenField");

            var serializedObject = new SerializedObject(target);
            enableTestingField.BindProperty(serializedObject.FindProperty("enableTesting"));
            startingSkillPointsField.BindProperty(serializedObject.FindProperty("startingSkillPoints"));
            skillNodeTinyPrefabField.BindProperty(serializedObject.FindProperty("skillNodeTinyPrefab"));
            skillNodeSmallPrefabField.BindProperty(serializedObject.FindProperty("skillNodeSmallPrefab"));
            skillNodeMediumPrefabField.BindProperty(serializedObject.FindProperty("skillNodeMediumPrefab"));
            skillNodeLargePrefabField.BindProperty(serializedObject.FindProperty("skillNodeLargePrefab"));
            skillNodeGiantPrefabField.BindProperty(serializedObject.FindProperty("skillNodeGiantPrefab"));
            connectionPrefabsField.BindProperty(serializedObject.FindProperty("connectionPrefabs"));
            contentField.BindProperty(serializedObject.FindProperty("content"));
            graphContentField.BindProperty(serializedObject.FindProperty("graphContent"));
            boundsField.BindProperty(serializedObject.FindProperty("bounds"));
            loadOnStartField.BindProperty(serializedObject.FindProperty("loadOnStart"));
            webNameField.BindProperty(serializedObject.FindProperty("webName"));
            clampInsideScreen.BindProperty(serializedObject.FindProperty("clampInsideScreen"));

            enableTestingField.RegisterValueChangedCallback(x =>
            {
                UpdateElements();
            });

            UpdateElements();

            return inspector;
        }

        private void UpdateElements()
        {
            var webView = (WebViewUGUI)target;
            startingSkillPointsField.style.display = webView.enableTesting ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
#endif