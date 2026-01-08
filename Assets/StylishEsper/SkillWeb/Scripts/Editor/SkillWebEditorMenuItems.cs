//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

#if UNITY_EDITOR
using Esper.SkillWeb.UI.UGUI;
using UnityEditor;
using UnityEngine;

namespace Esper.SkillWeb.Editor
{
    public static class InventoolEditorMenuItems
    {
        [MenuItem("GameObject/Skill Web/Initializer", priority = 0)]
        private static void AddInitializer()
        {
            var initializer = AssetSearch.FindFirstInScene<SkillWebInitializer>();

            if (!initializer)
            {
                initializer = ObjectFactory.CreateGameObject("SkillWebInitializer", typeof(SkillWebInitializer)).GetComponent<SkillWebInitializer>();

                if (Selection.activeGameObject)
                {
                    initializer.transform.SetParent(Selection.activeGameObject.transform, false);
                }

                Undo.RecordObject(initializer, "Undo Add Skill Web Initializer");
            }

            Selection.activeGameObject = initializer.gameObject;
        }

        [MenuItem("GameObject/Skill Web/Web View", priority = 1)]
        private static void AddWebViewUGUI()
        {
            var webView = Object.Instantiate(AssetSearch.Find<WebViewUGUI>("SkillWeb", "Resources/Prefabs/uGUI/WebViewUGUI.prefab"));
            webView.name = "WebView";

            if (Selection.activeGameObject)
            {
                webView.transform.SetParent(Selection.activeGameObject.transform, false);
            }

            var canvas = webView.GetComponent<Canvas>();
            canvas.worldCamera = AssetSearch.FindFirstInScene<Camera>();

            Undo.RecordObject(webView, "Undo Add Web View");

            Selection.activeGameObject = webView.gameObject;

            var selector = AssetSearch.FindFirstInScene<WebViewSelectorUGUI>();

            if (!selector)
            {
                selector = Object.Instantiate(AssetSearch.Find<WebViewSelectorUGUI>("SkillWeb", "Resources/Prefabs/uGUI/SelectorUGUI.prefab"));
                selector.name = "Selector";
                Undo.RecordObject(selector, "Undo Add Selector");
            }
        }

        [MenuItem("GameObject/Skill Web/Selector", priority = 2)]
        private static void AddSelectorUGUI()
        {
            WebViewSelectorUGUI selector = AssetSearch.FindFirstInScene<WebViewSelectorUGUI>();

            if (!selector)
            {
                selector = Object.Instantiate(AssetSearch.Find<WebViewSelectorUGUI>("SkillWeb", "Resources/Prefabs/uGUI/SelectorUGUI.prefab"));
                selector.name = "Selector";

                if (Selection.activeGameObject)
                {
                    selector.transform.SetParent(Selection.activeGameObject.transform, false);
                }

                var canvas = selector.GetComponent<Canvas>();
                canvas.worldCamera = AssetSearch.FindFirstInScene<Camera>();

                Undo.RecordObject(selector, "Undo Add Hovercard");
            }

            Selection.activeGameObject = selector.gameObject;
        }

        [MenuItem("GameObject/Skill Web/Hovercard", priority = 3)]
        private static void AddHovercardUGUI()
        {
            HovercardUGUI hovercard = AssetSearch.FindFirstInScene<HovercardUGUI>();

            if (!hovercard)
            {
                hovercard = Object.Instantiate(AssetSearch.Find<HovercardUGUI>("SkillWeb", "Resources/Prefabs/uGUI/HovercardUGUI.prefab"));
                hovercard.name = "Hovercard";

                if (Selection.activeGameObject)
                {
                    hovercard.transform.SetParent(Selection.activeGameObject.transform, false);
                }

                var canvas = hovercard.GetComponent<Canvas>();
                canvas.worldCamera = AssetSearch.FindFirstInScene<Camera>();

                Undo.RecordObject(hovercard, "Undo Add Hovercard");
            }

            Selection.activeGameObject = hovercard.gameObject;
        }
    }
}
#endif