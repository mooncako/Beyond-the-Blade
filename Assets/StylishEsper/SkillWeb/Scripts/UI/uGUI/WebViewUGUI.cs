//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using Esper.SkillWeb.Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Esper.SkillWeb.UI.UGUI
{
    /// <summary>
    /// A UI element with panning and zooming capabilities for viewing a skill web.
    /// </summary>
    public class WebViewUGUI : RuntimeWebViewer, IScrollHandler, IBeginDragHandler, IDragHandler
    {
        /// <summary>
        /// If testing should be enabled.
        /// </summary>
        public bool enableTesting;

        /// <summary>
        /// The starting skill points for test purposes.
        /// </summary>
        public int startingSkillPoints;

        /// <summary>
        /// A prefab that represents a tiny skill node.
        /// </summary>
        [SerializeField]
        protected SkillNodeUGUI skillNodeTinyPrefab;

        /// <summary>
        /// A prefab that represents a small skill node.
        /// </summary>
        [SerializeField]
        protected SkillNodeUGUI skillNodeSmallPrefab;

        /// <summary>
        /// A prefab that represents a medium skill node.
        /// </summary>
        [SerializeField]
        protected SkillNodeUGUI skillNodeMediumPrefab;

        /// <summary>
        /// A prefab that represents a large skill node.
        /// </summary>
        [SerializeField]
        protected SkillNodeUGUI skillNodeLargePrefab;

        /// <summary>
        /// A prefab that represents a giant skill node.
        /// </summary>
        [SerializeField]
        protected SkillNodeUGUI skillNodeGiantPrefab;

        /// <summary>
        /// A prefab that represents a connection.
        /// </summary>
        [SerializeField]
        protected ConnectionUGUI[] connectionPrefabs;

        /// <summary>
        /// The main content container.
        /// </summary>
        public RectTransform content;

        /// <summary>
        /// The UI object that will be panned and zoomed.
        /// </summary>
        public RectTransform graphContent;

        /// <summary>
        /// A RectTransform that acts as the bounds of the graph.
        /// </summary>
        public RectTransform bounds;

        /// <summary>
        /// If true, automatic bounds will use the screen size as the bounds. Otherwise, the size of the content will be used.
        /// </summary>
        public bool clampInsideScreen = true;

        /// <summary>
        /// The main canvas.
        /// </summary>
        protected Canvas canvas;

        /// <summary>
        /// A list of all loaded skill node UI objects.
        /// </summary>
        public Dictionary<int, SkillNodeUGUI> loadedSkillNodes = new();

        /// <summary>
        /// A list of all loaded skill connection objects.
        /// </summary>
        public Dictionary<Connection, ConnectionUGUI> loadedConnections = new();

        /// <summary>
        /// The last mouse position before dragging began.
        /// </summary>
        protected Vector2 lastMousePosition;

        /// <summary>
        /// The target zoom scale.
        /// </summary>
        [NonSerialized]
        public Vector3 targetScale;

        /// <summary>
        /// The target position used for zooming.
        /// </summary>
        [NonSerialized]
        public Vector2 targetPosition;

        /// <summary>
        /// The starting scale.
        /// </summary>
        protected Vector3 startingScale;

        /// <summary>
        /// The starting position used for resetting.
        /// </summary>
        protected Vector2 startingPosition;

        /// <summary>
        /// The size of the panning bounds.
        /// </summary>
        protected Vector2 boundsSize;

        /// <summary>
        /// A constant amount added to the target position.
        /// </summary>
        [NonSerialized]
        public Vector2 move = Vector2.zero;

        /// <summary>
        /// The constant zoom delta.
        /// </summary>
        [NonSerialized]
        public float zoomDelta;

        /// <summary>
        /// If the graph control is currently disabled.
        /// </summary>
        protected bool controlDisabled;

        /// <summary>
        /// If the web view is currently open.
        /// </summary>
        public bool IsOpen { get => content.gameObject.activeSelf; }

        /// <summary>
        /// The active web view.
        /// </summary>
        public static WebViewUGUI Active { get; protected set; }

        /// <summary>
        /// The active instance.
        /// </summary>
        [Obsolete("WebViewUGUI.Instance has been deprecated. Use WebViewUGUI.Active instead.")]
        public static WebViewUGUI Instance { get; protected set; }

        private void Awake()
        {
            canvas = GetComponent<Canvas>();
            targetScale = Vector3.one * SkillWeb.Settings.minScale;
            SetActive();
        }

        private void Start()
        {
            if (!string.IsNullOrEmpty(webName) && loadOnStart)
            {
                web = new Web(SkillWeb.GetWebGraph(webName));
                Load();
            }

            if (enableTesting)
            {
                SkillWeb.skillPoints = startingSkillPoints;
            }
        }

        private void Update()
        {
            if (controlDisabled)
            {
                return;
            }

            float deltaTime = SkillWeb.Settings.GetDeltaTime();

            if (zoomDelta != 0)
            {
                var position = new Vector2(Screen.width, Screen.height) / 2;
                float zoomStrength = Mathf.Log(targetScale.x + 1f);
                Zoom(position, zoomDelta * zoomStrength * deltaTime, false);
            }

            SetTargetPosition(targetPosition + (move * 500 * deltaTime * targetScale));

            // Smoothly interpolate to the target scale and position
            if (graphContent.localScale != targetScale)
            {
                graphContent.localScale = Vector3.Lerp(graphContent.localScale, targetScale, deltaTime * SkillWeb.Settings.zoomSmoothing);
            }

            if (graphContent.anchoredPosition != targetPosition)
            {
                graphContent.anchoredPosition = Vector2.Lerp(graphContent.anchoredPosition, targetPosition, deltaTime * SkillWeb.Settings.panSmoothing);

                if (HovercardUGUI.Instance && HovercardUGUI.Instance.IsOpen)
                {
                    HovercardUGUI.Instance.UpdatePosition();
                }
            }
        }

        /// <summary>
        /// Finds a specific WebViewUGUI component in the active scene.
        /// </summary>
        /// <param name="webName">The name of the web.</param>
        /// <param name="log">If an error message should be logged in case of failure.</param>
        /// <returns>The web view with a web reference with the specific name. Null will be returned if it was not found.</returns>
        public static WebViewUGUI Find(string webName, bool log = true)
        {
            var webViews = FindObjectsByType<WebViewUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (var webView in webViews)
            {
                if (webView.webName == webName)
                {
                    return webView;
                }
            }

            if (log)
            {
                SkillWebLogger.LogWarning($"Web View: unable to find the web view component that has a loaded web named '{webName}'.");
            }

            return null;
        }

        /// <summary>
        /// Sets this web view as the active one.
        /// </summary>
        public void SetActive()
        {
            Active = this;
            WebViewSelectorUGUI.Instance?.TargetActive();
        }

        /// <summary>
        /// Enables the web view.
        /// </summary>
        public virtual void Open()
        {
            content.gameObject.SetActive(true);
        }

        /// <summary>
        /// Disables the web view.
        /// </summary>
        public virtual void Close()
        {
            content.gameObject.SetActive(false);

            if (WebViewSelectorUGUI.Instance)
            {
                WebViewSelectorUGUI.Instance.SetVisible(false);
            }
        }

        /// <summary>
        /// Toggles the open and close state.
        /// </summary>
        public virtual void Toggle()
        {
            if (IsOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }

        /// <summary>
        /// Unloads the loaded web by clearing the UI.
        /// </summary>
        public void Unload()
        {
            UnsetWeb();

            // Destroy existing UI objects
            foreach (var node in loadedSkillNodes.Values)
            {
                Destroy(node.gameObject);
            }

            loadedSkillNodes.Clear();

            foreach (var connection in loadedConnections.Values)
            {
                Destroy(connection.gameObject);
            }

            loadedConnections.Clear();

            Vector3 scale = Vector3.one;

            switch (SkillWeb.Settings.startingScale)
            {
                case Settings.SkillWebSettings.StartingScale.Min:
                    scale = Vector3.one * SkillWeb.Settings.minScale;
                    break;

                case Settings.SkillWebSettings.StartingScale.Normal:
                    scale = Vector3.one;
                    break;

                case Settings.SkillWebSettings.StartingScale.Max:
                    scale = Vector3.one * SkillWeb.Settings.maxScale;
                    break;
            }

            startingScale = scale;
            targetScale = startingScale;
            startingPosition = Vector3.zero;
            targetPosition = startingPosition;
        }

        /// <summary>
        /// Loads the web graph. This will unload the previous one.
        /// </summary>
        /// <param name="webGraph">The web graph to load.</param>
        public void Load(WebGraph webGraph)
        {
            onLoadStarted?.Invoke(this);

            Unload();
            SetWeb(webGraph);
            CreateUI();
            web.UpdateAllStates();

            onLoadCompleted?.Invoke(this);
        }

        /// <summary>
        /// Loads the web. This will unload the previous one.
        /// </summary>
        /// <param name="web">The web to load.</param>
        public void Load(Web web)
        {
            onLoadStarted?.Invoke(this);

            Unload();
            SetWeb(web);
            CreateUI();
            web.UpdateAllStates();

            onLoadCompleted?.Invoke(this);
        }

        /// <summary>
        /// Loads the web. This will unload the previous one.
        /// </summary>
        public void Load()
        {
            if (web != null)
            {
                Load(web);
            }
            else
            {
                SkillWebLogger.LogError("Web View: unable to load. Web is null");
            }         
        }

        /// <summary>
        /// Creates the graph UI.
        /// </summary>
        protected void CreateUI()
        {
            if (web == null)
            {
                SkillWebLogger.LogWarning("Web View: cannot create UI for a null web.");
                return;
            }

            // Set starting position at center of all nodes
            centroid = CalculateCenterPosition();
            startingPosition = Vector2.zero;
            targetPosition = startingPosition;
            graphContent.anchoredPosition = targetPosition;
            graphContent.localScale = startingScale;

            // Create UI for each skill node
            var allNodes = web.skillNodes;

            // Start creating bounds
            var boundsMax = Vector2.zero;
            var boundsMin = Vector2.zero;

            if (allNodes.Count > 0)
            {
                boundsMin = Vector2.one * float.MaxValue;
                boundsMax = Vector2.one * float.MinValue;

                foreach (var node in allNodes.Values)
                {
                    SkillNodeUGUI skillUI;

                    switch (node.skill.size)
                    {
                        case Skill.Size.Tiny:
                            skillUI = Instantiate(skillNodeTinyPrefab, graphContent);
                            break;

                        case Skill.Size.Small:
                            skillUI = Instantiate(skillNodeSmallPrefab, graphContent);
                            break;

                        case Skill.Size.Medium:
                            skillUI = Instantiate(skillNodeMediumPrefab, graphContent);
                            break;

                        case Skill.Size.Large:
                            skillUI = Instantiate(skillNodeLargePrefab, graphContent);
                            break;

                        case Skill.Size.Giant:
                            skillUI = Instantiate(skillNodeGiantPrefab, graphContent);
                            break;

                        default:
                            skillUI = Instantiate(skillNodeTinyPrefab, graphContent);
                            break;
                    }

                    skillUI.SetSkill(node, this);

                    // Reverse Y position, this is required because editor UI is handled differently
                    var position = new Vector2(node.position.x, node.position.y * -1);

                    // Set position relative to the center
                    var relativePosition = position - centroid;
                    skillUI.rectTransform.anchoredPosition3D = new Vector3(relativePosition.x, relativePosition.y, -0.02f);

                    loadedSkillNodes.Add(node.id, skillUI);

                    // Update bounds if necessary
                    if (boundsMin.x > relativePosition.x)
                    {
                        boundsMin.x = relativePosition.x;
                    }
                    else if (boundsMax.x < relativePosition.x)
                    {
                        boundsMax.x = relativePosition.x;
                    }

                    if (boundsMin.y > relativePosition.y)
                    {
                        boundsMin.y = relativePosition.y;
                    }
                    else if (boundsMax.y < relativePosition.y)
                    {
                        boundsMax.y = relativePosition.y;
                    }
                }
            }
            else
            {
                boundsMax = Vector2.zero;
                boundsMin = Vector2.zero;
            }

            // Set bound size and update bounds
            boundsSize = boundsMax - boundsMin;

            if (boundsSize.x < 0)
            {
                boundsSize.x = 0;
            }

            if (boundsSize.y < 0)
            {
                boundsSize.y = 0;
            }

            UpdateBounds(targetPosition);

            // Create connection UI
            foreach (var connection in web.Connections)
            {
                var connectionUI = Instantiate(connectionPrefabs[connection.styleIndex], graphContent);
                var rectTransform = connectionUI.transform as RectTransform;
                rectTransform.anchoredPosition3D = new Vector3(rectTransform.anchoredPosition3D.x, rectTransform.anchoredPosition3D.y, -0.01f);
                connectionUI.SetConnection(connection, this);
                connectionUI.transform.SetAsFirstSibling();
                loadedConnections.Add(connection, connectionUI);
            }
        }

        /// <summary>
        /// Resets all skills. onDowngrade will be invoked for all skills before the skill level is reset.
        /// </summary>
        public void ResetAllSkills()
        {
            web.ResetAllSkills();
        }

        /// <summary>
        /// Creates a connection line from a point to a specific skill node for visual purposes only. The connection is
        /// not a real connection—it won't function as normal ones.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="nodeID">The ID of the node. This is not the skill ID.</param>
        /// <param name="prefabIndex">The connection prefab index.</param>
        public void CreateVisualConnectionLine(Vector2 position, int nodeID, int prefabIndex = 0)
        {
            if (!loadedSkillNodes.ContainsKey(nodeID))
            {
                SkillWebLogger.LogWarning($"Skill Web: failed to create a connection line. The skill node with the ID {nodeID} doesn't exist in the web or the web has not been loaded.");
                return;
            }

            CreateVisualConnectionLine(position, loadedSkillNodes[nodeID], prefabIndex);
        }

        /// <summary>
        /// Creates a connection line from a point to a specific skill node for visual purposes only. The connection is
        /// not a real connection—it won't function as normal ones.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="node">The skill node to connect to.</param>
        /// <param name="prefabIndex">The connection prefab index.</param>
        public void CreateVisualConnectionLine(Vector2 position, SkillNodeUGUI node, int prefabIndex = 0)
        {
            if (!loadedSkillNodes.ContainsKey(node.skillNode.id))
            {
                SkillWebLogger.LogWarning($"Skill Web: failed to create a connection line. The skill node with the ID {node.skillNode.id} doesn't exist in the web or the web has not been loaded.");
                return;
            }

            var connectionUI = Instantiate(connectionPrefabs[prefabIndex] as ConnectionLine, graphContent);
            var rectTransform = connectionUI.transform as RectTransform;
            rectTransform.anchoredPosition3D = new Vector3(rectTransform.anchoredPosition3D.x, rectTransform.anchoredPosition3D.y, -0.01f);
            connectionUI.SetConnection(position, node);

            // Create and add new connection
            var connection = new Connection(node.skillNode.id, -1, 0, 0, prefabIndex);
            node.skillNode.connections.Add(connection);

            loadedConnections.Add(connection, connectionUI);
        }

        /// <summary>
        /// Focuses on a skill node.
        /// </summary>
        /// <param name="node">The skill node UI object.</param>
        public void Focus(SkillNodeUGUI node)
        {
            if (controlDisabled || !node)
            {
                return;
            }

            StartCoroutine(FocusCoroutine(node));

            if (WebViewSelectorUGUI.Instance && !WebViewSelectorUGUI.Instance.IsVisible)
            {
                WebViewSelectorUGUI.Instance.SetVisible(true);
            }
        }

        /// <summary>
        /// Unsets the focused node.
        /// </summary>
        public void RemoveFocus()
        {
            WebViewSelectorUGUI.Instance.focusedNode = null;

            if (WebViewSelectorUGUI.Instance && WebViewSelectorUGUI.Instance.IsVisible)
            {
                WebViewSelectorUGUI.Instance.SetVisible(false);
            }
        }

        /// <summary>
        /// Resets the view.
        /// </summary>
        public void ResetView()
        {
            if (controlDisabled)
            {
                return;
            }

            StartCoroutine(ResetViewCoroutine());
        }

        /// <summary>
        /// Pan the graph towards a direction.
        /// </summary>
        /// <param name="dir">The direction.</param>
        public void Move(Vector2 dir)
        {
            move = dir * -1;

            if (WebViewSelectorUGUI.Instance && !WebViewSelectorUGUI.Instance.IsVisible)
            {
                WebViewSelectorUGUI.Instance.SetVisible(true);
            }
        }

        /// <summary>
        /// Snaps to a skill node with the current scale.
        /// </summary>
        /// <param name="skillNodeUGUI">The skill node to snap to.</param>
        public void Snap(SkillNodeUGUI skillNodeUGUI)
        {
            if (controlDisabled)
            {
                return;
            }

            if (zoomDelta != 0)
            {
                return;
            }

            if (!loadedSkillNodes.ContainsKey(skillNodeUGUI.skillNode.id))
            { 
                SkillWebLogger.LogError("Web View: cannot snap to a skill node that doesn't exist. Has the web view been initialized?");
                return;
            }

            targetPosition = skillNodeUGUI.rectTransform.anchoredPosition * -1 * targetScale;
            graphContent.anchoredPosition = targetPosition;

            UpdateBounds(targetPosition);
            StartCoroutine(ControlDelayCoroutine(SkillWeb.Settings.afterSnapDelay));
        }

        /// <summary>
        /// Upgrades the focused skill. If nothing is focused, nothing will happen.
        /// </summary>
        public void UpgradeFocused()
        {
            if (!WebViewSelectorUGUI.Instance)
            {
                SkillWebLogger.LogError("Web View: web view selector is required for focusing. Refer to the documentation.");
                return;
            }

            WebViewSelectorUGUI.Instance.focusedNode?.TryUpgrade();
        }

        /// <summary>
        /// Downgrades the focused skill. If nothing is focused, nothing will happen.
        /// </summary>
        public void DowngradeFocused()
        {
            if (!WebViewSelectorUGUI.Instance)
            {
                SkillWebLogger.LogError("Web View: web view selector is required for focusing. Refer to the documentation.");
                return;
            }

            WebViewSelectorUGUI.Instance.focusedNode?.TryDowngrade();
        }

        /// <summary>
        /// Delays control.
        /// </summary>
        /// <returns>Yields for a delay.</returns>
        protected IEnumerator ControlDelayCoroutine(float delay)
        {
            controlDisabled = true;
            yield return new WaitForSecondsRealtime(delay);
            controlDisabled = false;
        }

        /// <summary>
        /// Focuses the closest skill towards a direction.
        /// </summary>
        /// <param name="dir">The direction.</param>
        public void Focus(Vector2 dir)
        {
            if (!WebViewSelectorUGUI.Instance)
            {
                SkillWebLogger.LogError("Web View: web view selector is required for focusing. Refer to the documentation.");
                return;
            }

            if (controlDisabled)
            {
                return;
            }

            var focusedNode = WebViewSelectorUGUI.Instance.focusedNode;
            SkillNodeUGUI closest = null;
            var target = focusedNode ? focusedNode.rectTransform.anchoredPosition : Vector2.zero;
            float prevDistance = float.MaxValue;

            foreach (var item in loadedSkillNodes.Values)
            {
                if (item != focusedNode)
                {
                    float dot = Vector2.Dot((item.rectTransform.anchoredPosition - target).normalized, dir);

                    if (dot > 0f)
                    {
                        float distance = Vector2.Distance(item.rectTransform.anchoredPosition, target);
                        distance *= 10 - (9 * dot);

                        if (distance < prevDistance)
                        {
                            closest = item;
                            prevDistance = distance;
                        }
                    }
                }
            }

            Focus(closest);
        }

        /// <summary>
        /// Sets a constant in or out zoom relative to the current center of the screen. Set this to 0 for no
        /// constant zoom.
        /// </summary>
        /// <param name="zoomStrength">The zoom strength. Use negatives to zoom out.</param>
        public void SetConstantZoom(float zoomStrength)
        {
            zoomDelta = zoomStrength;
        }

        /// <summary>
        /// Zooms in and out towards a screen position.
        /// </summary>
        /// <param name="position">The screen position.</param>
        /// <param name="scrollDelta">The scroll amount.</param>
        /// <param name="scrollDelta">If the scrollDelta should be normalized.</param>
        public void Zoom(Vector2 position, float scrollDelta, bool normalize = true)
        {
            if (controlDisabled)
            {
                return;
            }

            if (normalize)
            {
                if (scrollDelta > 0)
                {
                    scrollDelta = 1;
                }
                else if (scrollDelta < 0)
                {
                    scrollDelta = -1;
                }
            }

            float scrollAmount = scrollDelta * SkillWeb.Settings.zoomStrength * targetScale.x;

            // If already at zoom limit, skip zoom
            if ((scrollAmount > 0 && targetScale.x >= SkillWeb.Settings.maxScale) ||
                (scrollAmount < 0 && targetScale.x <= SkillWeb.Settings.minScale))
            {
                return;
            }

            Vector3 oldTargetScale = targetScale;

            // Clamp scale between min and max
            Vector3 newTargetScale = ClampScale(targetScale + Vector3.one * scrollAmount);

            // Set the new scale
            targetScale = newTargetScale;

            // Calculate the effective scroll amount
            float effectiveScrollAmount = newTargetScale.x - oldTargetScale.x;

            var mousePos = position;

            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                // Get the mouse position relative to the canvas
                var mousePosLocal = mousePos - (Vector2)graphContent.position;

                // Adjust position to zoom in/out based on the mouse position
                var pivotDelta = mousePosLocal * (effectiveScrollAmount / graphContent.localScale.x) / canvas.scaleFactor;
                SetTargetPosition(targetPosition - pivotDelta);
            }
            else
            {
                // Convert content position to screen point
                var contentScreenPos = canvas.worldCamera.WorldToScreenPoint(graphContent.position);

                // Get the direction and distance away from mouse
                var dir = mousePos - (Vector2)contentScreenPos;

                // Adjust position to zoom in/out based on the mouse position
                var pivotDelta = dir * (effectiveScrollAmount/ graphContent.localScale.x) / canvas.scaleFactor;
                SetTargetPosition(targetPosition - pivotDelta);
            }
        }

        public void OnScroll(PointerEventData eventData)
        {
            Zoom(eventData.position, eventData.scrollDelta.y);

            if (WebViewSelectorUGUI.Instance && WebViewSelectorUGUI.Instance.IsVisible)
            {
                WebViewSelectorUGUI.Instance.SetVisible(false);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (controlDisabled || eventData.button != SkillWeb.Settings.mousePanButton)
            {
                return;
            }

            lastMousePosition = eventData.position;

            if (WebViewSelectorUGUI.Instance && WebViewSelectorUGUI.Instance.IsVisible)
            {
                WebViewSelectorUGUI.Instance.SetVisible(false);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (controlDisabled || eventData.button != SkillWeb.Settings.mousePanButton)
            {
                return;
            }

            Vector2 delta = (eventData.position - lastMousePosition) / canvas.scaleFactor;
            SetTargetPosition(targetPosition + delta);
            lastMousePosition = eventData.position;
        }

        /// <summary>
        /// Updates the panning bounds.
        /// </summary>
        private void UpdateBounds(Vector2 targetPosition)
        {
            bounds.sizeDelta = boundsSize * targetScale.x;
            bounds.anchoredPosition = targetPosition;
        }

        /// <summary>
        /// Attempts to set the target position to pan towards. Clamping will apply.
        /// </summary>
        /// <param name="newPosition">The new target position.</param>
        private void SetTargetPosition(Vector2 newPosition)
        {
            UpdateBounds(newPosition);

            if (SkillWeb.Settings.automaticBoundsEnabled)
            {
                var unclamped = bounds.position;
                var clamped = SkillWebUtility.GetClampedPosition(bounds, canvas, clampInsideScreen ? null : content);

                if (unclamped != clamped)
                {
                    UpdateBounds(targetPosition);
                    return;
                }
            }

            targetPosition = newPosition;
        }

        /// <summary>
        /// Clamps the scale within the min and max ranges.
        /// </summary>
        /// <param name="scale">The attempted scale.</param>
        /// <returns>The scale clamped within the min and max ranges.</returns>
        private Vector3 ClampScale(Vector3 scale)
        {
            float clampedX = Mathf.Clamp(scale.x, SkillWeb.Settings.minScale, SkillWeb.Settings.maxScale);
            float clampedY = Mathf.Clamp(scale.y, SkillWeb.Settings.minScale, SkillWeb.Settings.maxScale);
            return new Vector3(clampedX, clampedY, 1);
        }

        /// <summary>
        /// Focuses on a skill node.
        /// </summary>
        /// <param name="skillNodeUGUI">The skill node UI object.</param>
        /// <returns>Yields until zoomed and moved towards the skill node.</returns>
        private IEnumerator FocusCoroutine(SkillNodeUGUI skillNodeUGUI)
        {
            controlDisabled = true;

            var fromScale = graphContent.localScale;
            var fromPosition = graphContent.anchoredPosition;
            targetScale = Vector3.one * SkillWeb.Settings.maxScale;
            targetPosition = skillNodeUGUI.rectTransform.anchoredPosition * -1 * targetScale;

            float timeElapsed = 0f;
            float duration = 1f / SkillWeb.Settings.resetFocusSpeed;

            while (timeElapsed < duration)
            {
                float t = timeElapsed / duration;

                graphContent.localScale = Vector3.Lerp(fromScale, targetScale, t);
                graphContent.anchoredPosition = Vector2.Lerp(fromPosition, targetPosition, t);

                timeElapsed += SkillWeb.Settings.GetDeltaTime();
                yield return null;
            }

            // Snap to final values just in case
            graphContent.localScale = targetScale;
            graphContent.anchoredPosition = targetPosition;

            UpdateBounds(targetPosition);

            controlDisabled = false;
        }

        /// <summary>
        /// Resets the view.
        /// </summary>
        /// <returns>Yields until the view is reset.</returns>
        private IEnumerator ResetViewCoroutine()
        {
            controlDisabled = true;

            var fromScale = graphContent.localScale;
            var fromPosition = graphContent.anchoredPosition;
            targetScale = startingScale;
            targetPosition = startingPosition;

            float timeElapsed = 0f;
            float duration = 1f / SkillWeb.Settings.resetFocusSpeed;

            while (timeElapsed < duration)
            {
                float t = timeElapsed / duration;

                graphContent.localScale = Vector3.Lerp(fromScale, targetScale, t);
                graphContent.anchoredPosition = Vector2.Lerp(fromPosition, targetPosition, t);

                timeElapsed += SkillWeb.Settings.GetDeltaTime();
                yield return null;
            }

            // Snap to final values just in case
            graphContent.localScale = targetScale;
            graphContent.anchoredPosition = targetPosition;

            UpdateBounds(targetPosition);

            controlDisabled = false;
        }
    }
}