//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using Esper.SkillWeb.Graph;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Esper.SkillWeb.UI
{
    /// <summary>
    /// Works with a Web to provide runtime web viewing capabilities.
    /// </summary>
    public abstract class RuntimeWebViewer : MonoBehaviour
    {
        /// <summary>
        /// The reference of the Web that was last loaded into the UI.
        /// </summary>
        [NonSerialized]
        public Web web;

        /// <summary>
        /// The name of the web graph. This is used to automatically grab the web at start.
        /// </summary>
        public string webName;

        /// <summary>
        /// If the web should be loaded on start.
        /// </summary>
        public bool loadOnStart;

        /// <summary>
        /// The center of all nodes.
        /// </summary>
        [NonSerialized]
        public Vector2 centroid;

        /// <summary>
        /// A callback for when the web UI begins to load.
        /// </summary>
        [HideInInspector]
        public static UnityEvent<RuntimeWebViewer> onLoadStarted = new();

        /// <summary>
        /// A callback for when the web UI is finished loading.
        /// </summary>
        [HideInInspector]
        public static UnityEvent<RuntimeWebViewer> onLoadCompleted = new();

        /// <summary>
        /// If a web is set.
        /// </summary>
        public bool IsWebSet { get => web != null; }

        /// <summary>
        /// Sets the web.
        /// </summary>
        /// <param name="webGraph">The web graph to view.</param>
        public void SetWeb(WebGraph webGraph)
        {
            if (!webGraph)
            {
                return;
            }

            web = new Web(webGraph);
            webName = web.graph.webName;
        }

        /// <summary>
        /// Sets the web.
        /// </summary>
        /// <param name="web">The web to view.</param>
        public void SetWeb(Web web)
        {
            if (web == null)
            {
                return;
            }

            this.web = web;
            webName = web.graph.webName;
        }

        /// <summary>
        /// Sets the web to null.
        /// </summary>
        public void UnsetWeb()
        {
            web = null;
        }

        /// <summary>
        /// Adds to the revertable state. If this is the first skill node added, the web will become revertable. The web is
        /// only revertable if changes have been made.
        /// </summary>
        public void AddToRevertableState(SkillNode skillNode)
        {
            web.AddToRevertableState(skillNode);
        }

        /// <summary>
        /// Confirms the changes made. The graph will not be revertable until changes are made once again.
        /// </summary>
        public void ConfirmChanges()
        {
            web.ConfirmChanges();
        }

        /// <summary>
        /// Reverts the changes made. The graph will not be revertable until changes are made once again.
        /// </summary>
        public void RevertChanges()
        {
            web.RevertChanges();
        }

        /// <summary>
        /// Gets the center position of the web. This is more accurate than CalculateCenterPosition for custom UI objects 
        /// added to the graph content. However, this should be used after the web is loaded.
        /// </summary>
        /// <returns>The center of the web.</returns>
        public virtual Vector2 GetCenterPosition()
        {
            var center = centroid;
            center.x *= -1;
            return center;
        }

        /// <summary>
        /// Calculates the center position of all nodes.
        /// </summary>
        /// <returns>The center position.</returns>
        public virtual Vector2 CalculateCenterPosition()
        {
            if (web == null || web.skillNodes.Count == 0)
            {
                return Vector2.zero;
            }

            float minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;

            foreach (var node in web.skillNodes.Values)
            {
                var pos = new Vector2(node.position.x, node.position.y * -1);
                minX = Mathf.Min(minX, pos.x);
                maxX = Mathf.Max(maxX, pos.x);
                minY = Mathf.Min(minY, pos.y);
                maxY = Mathf.Max(maxY, pos.y);
            }

            Vector2 visualCenter = new Vector2((minX + maxX) / 2, (minY + maxY) / 2);
            return visualCenter;
        }
    }
}