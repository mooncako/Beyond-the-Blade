//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using Esper.SkillWeb.Graph;
using System.Collections;
using UnityEngine;

namespace Esper.SkillWeb.UI.UGUI
{
    /// <summary>
    /// A line that represents a skill node connection.
    /// </summary>
    public class ConnectionLine : ConnectionUGUI
    {
        /// <summary>
        /// The line renderer component.
        /// </summary>
        public LineRenderer lineRenderer;

        /// <summary>
        /// The UI line renderer component. This is a maskable version of the line renderer.
        /// </summary>
        public UILineRenderer uiLineRenderer;

        /// <summary>
        /// The highlight animation speed.
        /// </summary>
        [Min(0f)]
        public float highlightAnimationSpeed = 1.5f;

        /// <summary>
        /// The start position. Relevant for visual connections.
        /// </summary>
        protected Vector2 startPosition;

        /// <summary>
        /// The current highlight coroutine.
        /// </summary>
        protected Coroutine highlightCoroutine;

        public override void SetConnection(Connection connection, WebViewUGUI webView)
        {
            base.SetConnection(connection, webView);
            Refresh();
        }

        /// <summary>
        /// Sets up the connection UI.
        /// </summary>
        /// <param name="startPosition">The start position.</param>
        /// <param name="skillNodeUGUI">The skill UI to connect to.</param>
        public void SetConnection(Vector2 startPosition, SkillNodeUGUI skillNodeUGUI)
        {
            connection = null;
            this.startPosition = startPosition;
            output = skillNodeUGUI;
            Refresh();
        }

        public override void Refresh()
        {
            UpdateLine();
            ApplyActiveState();
        }

        public override void UpdateLine()
        {
            if (lineRenderer)
            {
                if (connection != null)
                {
                    lineRenderer.SetPosition(0, Input.rectTransform.anchoredPosition);
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, Output.rectTransform.anchoredPosition);
                }
                else
                {
                    lineRenderer.SetPosition(0, startPosition);
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, Output.rectTransform.anchoredPosition);
                }
            }
            else if (uiLineRenderer)
            {
                if (connection != null)
                {
                    uiLineRenderer.SetPosition(0, Input.rectTransform.anchoredPosition);
                    uiLineRenderer.SetPosition(uiLineRenderer.positionCount - 1, Output.rectTransform.anchoredPosition);
                }
                else
                {
                    uiLineRenderer.SetPosition(0, startPosition);
                    uiLineRenderer.SetPosition(uiLineRenderer.positionCount - 1, Output.rectTransform.anchoredPosition);
                }
            }
        }

        public override void Highlight()
        {
            // Remove previous highlight if it exists
            if (isHighlighted || highlightCoroutine != null)
            {
                Unhighlight();
            }

            base.Highlight();

            // Start material animation
            highlightCoroutine = StartCoroutine(HighlightAnimationCoroutine());
        }

        public override void Unhighlight()
        {
            base.Unhighlight();

            if (highlightCoroutine != null)
            {
                StopCoroutine(highlightCoroutine);
                highlightCoroutine = null;
            }

            // Set the highlight strength to 0
            if (lineRenderer)
            {
                lineRenderer.material.SetFloat("_HighlightStrength", 0f);
            }
            else if (uiLineRenderer)
            {
                uiLineRenderer.material.SetFloat("_HighlightStrength", 0f);
            }
        }

        public override void ApplyActiveState()
        {
            // Update texture based on active state
            bool active = IsActive;

            if (lineRenderer)
            {
                lineRenderer.material.SetFloat("_Active", active ? 1f : 0f);
            }
            else if (uiLineRenderer)
            {
                if (!uiLineRenderer.cachedMaterial)
                {
                    uiLineRenderer.materialForRendering.SetFloat("_Active", active ? 1f : 0f);
                }
                else
                {
                    uiLineRenderer.material.SetFloat("_Active", active ? 1f : 0f);
                }        
            }
        }

        /// <summary>
        /// Handles the highlight animation.
        /// </summary>
        /// <returns>Yields multiple times to allow the animation to run.</returns>
        protected IEnumerator HighlightAnimationCoroutine()
        {
            bool increaseStrength = true;
            float timeElapsed = 0;

            while (isHighlighted)
            {
                yield return null;
                var deltaTime = SkillWeb.Settings.GetDeltaTime();
                timeElapsed += (increaseStrength ? deltaTime : -deltaTime) * highlightAnimationSpeed;

                if (lineRenderer)
                {
                    lineRenderer.material.SetFloat("_HighlightStrength", timeElapsed);
                }
                else if (uiLineRenderer)
                {
                    uiLineRenderer.material.SetFloat("_HighlightStrength", timeElapsed);
                }

                if (increaseStrength && timeElapsed >= 1f || !increaseStrength && timeElapsed <= 0f)
                {
                    increaseStrength = !increaseStrength;
                }
            }
        }
    }
}