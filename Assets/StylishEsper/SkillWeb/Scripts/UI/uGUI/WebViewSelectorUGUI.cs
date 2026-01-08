//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Esper.SkillWeb.UI.UGUI
{
    /// <summary>
    /// Used by the WebView for input support.
    /// </summary>
    public class WebViewSelectorUGUI : MonoBehaviour
    {
        /// <summary>
        /// The Animator component.
        /// </summary>
        protected Animator animator;

        /// <summary>
        /// The RectTransform component.
        /// </summary>
        protected RectTransform rectTransform;

        /// <summary>
        /// The target canvas's graphic raycaster.
        /// </summary>
        protected GraphicRaycaster raycaster;

        /// <summary>
        /// The event system in the scene.
        /// </summary>
        protected EventSystem eventSystem;

        [HideInInspector]
        public Canvas canvas;

        /// <summary>
        /// The node currently focused.
        /// </summary>
        [NonSerialized]
        public SkillNodeUGUI focusedNode;

        /// <summary>
        /// The normal size of the selector.
        /// </summary>
        [SerializeField]
        protected Vector2 normalSize = new Vector2(100, 100);

        /// <summary>
        /// The unfocused animation name.
        /// </summary>
        [SerializeField]
        protected string unfocusedAnimName = "Unfocused";

        /// <summary>
        /// The focused animation name.
        /// </summary>
        [SerializeField]
        protected string focusedAnimName = "Focused";

        /// <summary>
        /// If the selector is currently visible.
        /// </summary>
        public bool IsVisible { get => gameObject.activeInHierarchy; }

        /// <summary>
        /// The active instance.
        /// </summary>
        public static WebViewSelectorUGUI Instance { get; protected set; }

        private void Awake()
        {
            // Singleton
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            canvas = GetComponent<Canvas>();
            animator = GetComponentInChildren<Animator>();
            eventSystem = SceneSearch.FindFirstInScene<EventSystem>();

            if (transform.childCount > 0)
            {
                rectTransform = transform.GetChild(0) as RectTransform;
            }
            else
            {
                rectTransform = transform as RectTransform;
            }
        }

        private void Start()
        {
            SetVisible(false);
        }

        private void Update()
        {
            var detectedNode = RaycastAtScreenCenter();
            var previousNode = focusedNode;
            focusedNode = detectedNode;

            if (previousNode != detectedNode)
            {
                bool requiresSnapping = !previousNode && focusedNode || detectedNode != focusedNode;

                if (focusedNode && requiresSnapping)
                {
                    WebViewUGUI.Active.Snap(focusedNode);
                    focusedNode.HighlightPaths();
                    SetFocused(true);
                }
                else
                {
                    if (previousNode)
                    {
                        previousNode.UnhighlightPaths();
                    }

                    SetFocused(false);
                }
            }

            UpdateSize();
        }

        private SkillNodeUGUI RaycastAtScreenCenter()
        {
            if (!raycaster)
            {
                TargetActive();

                if (!raycaster)
                {
                    return null;
                }
            }

            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

            PointerEventData pointerData = new PointerEventData(eventSystem)
            {
                position = screenCenter
            };

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerData, results);

            if (results.Count > 0)
            {
                foreach (RaycastResult result in results)
                {
                    if (result.gameObject.TryGetComponent(out SkillNodeUGUI skillNodeUI))
                    {
                        return skillNodeUI;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Matches the size of the focused node. 
        /// </summary>
        public virtual void UpdateSize()
        {
            if (!WebViewUGUI.Active)
            {
                SkillWebLogger.LogWarning("Web View Selector: an instance of a web view is required.");
                return;
            }

            var webView = WebViewUGUI.Active;

            float normalizedScale = (webView.targetScale.x - SkillWeb.Settings.minScale) / (SkillWeb.Settings.maxScale - SkillWeb.Settings.minScale);
            float scale = Mathf.Lerp(0.4f, SkillWeb.Settings.maxScale, normalizedScale);

            if (focusedNode)
            {
                rectTransform.sizeDelta = focusedNode.rectTransform.sizeDelta * scale;
            }
            else
            {
                rectTransform.sizeDelta = normalSize;
            }

            scale = Mathf.Min(scale, 1);
            rectTransform.localScale = Vector3.one * scale;
        }

        /// <summary>
        /// Targets the active web view.
        /// </summary>
        public void TargetActive()
        {
            if (WebViewUGUI.Active)
            {
                raycaster = WebViewUGUI.Active.graphContent.GetComponent<GraphicRaycaster>();
            }
            else
            {
                SkillWebLogger.LogWarning("Web View Selector: no active web view has been set!");
            }
        }

        /// <summary>
        /// Sets the visibility.
        /// </summary>
        /// <param name="visible">If the selector should be visible.</param>
        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);

            if (visible)
            {
                SetFocused(false);
            }
            else
            {
                if (HovercardUGUI.Instance && HovercardUGUI.Instance.IsOpen)
                {
                    HovercardUGUI.Instance.Close();
                }

                focusedNode = null;
            }
        }

        /// <summary>
        /// Sets the focused state.
        /// </summary>
        /// <param name="focused">The new focused state.</param>
        public void SetFocused(bool focused)
        {
            if (focused)
            {
                animator.Play("Focused");

                if (focusedNode)
                {
                    HovercardUGUI.Instance?.Open(focusedNode);
                }
            }
            else
            {
                animator.Play("Unfocused");
                HovercardUGUI.Instance?.Close();
            }
        }
    }
}