//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static Esper.SkillWeb.Skill;

namespace Esper.SkillWeb
{
    /// <summary>
    /// Skill Web utility functions.
    /// </summary>
    public static class SkillWebUtility
    {
        /// <summary>
        /// Generates a new GUID.
        /// </summary>
        /// <returns>The generated GUID.</returns>
        public static string GenerateNewGuid()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets the position of the RectTransform completely clamped inside the screen.
        /// </summary>
        /// <param name="rectTransform">The RectTransform.</param>
        /// <param name="canvas">The canvas.</param>
        /// <returns>The clamped position.</returns>
        public static Vector2 GetCompleteClampedPosition(RectTransform rectTransform, Canvas canvas)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                // Get screen dimensions in canvas space
                Vector2 canvasSize = new Vector2(Screen.width, Screen.height);

                // Get the size of the RectTransform
                Vector2 size = rectTransform.sizeDelta * rectTransform.lossyScale;

                // Get the position of the RectTransform relative to the screen
                Vector3[] worldCorners = new Vector3[4];
                rectTransform.GetWorldCorners(worldCorners);

                // Convert world corners to screen space
                Vector3 bottomLeft = RectTransformUtility.WorldToScreenPoint(null, worldCorners[0]);
                Vector3 topRight = RectTransformUtility.WorldToScreenPoint(null, worldCorners[2]);

                // Calculate clamped position
                Vector2 clampedPosition = rectTransform.anchoredPosition;
                Vector2 pivotOffset = rectTransform.pivot - new Vector2(0.5f, 0.5f);

                // Clamp horizontally
                if (bottomLeft.x < 0)
                {
                    clampedPosition.x += -bottomLeft.x;
                }
                if (topRight.x > canvasSize.x)
                {
                    clampedPosition.x -= (topRight.x - canvasSize.x);
                }

                // Clamp vertically
                if (bottomLeft.y < 0)
                {
                    clampedPosition.y += -bottomLeft.y;
                }
                if (topRight.y > canvasSize.y)
                {
                    clampedPosition.y -= (topRight.y - canvasSize.y);
                }

                // Apply the clamped position
                return clampedPosition;
            }
            else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                // Get the camera from the canvas
                Camera cam = canvas.worldCamera;
                if (cam == null) return Vector2.zero;

                // Convert the RectTransform's position to screen space
                Vector3[] worldCorners = new Vector3[4];
                rectTransform.GetWorldCorners(worldCorners);

                // Convert each world corner to screen space
                Vector3 bottomLeft = RectTransformUtility.WorldToScreenPoint(cam, worldCorners[0]);
                Vector3 topRight = RectTransformUtility.WorldToScreenPoint(cam, worldCorners[2]);

                // Get the screen bounds
                float screenWidth = Screen.width;
                float screenHeight = Screen.height;

                // Calculate the new anchored position
                Vector2 clampedPosition = rectTransform.anchoredPosition;

                // Clamp horizontally
                if (bottomLeft.x < 0)
                {
                    clampedPosition.x += -bottomLeft.x / canvas.scaleFactor;
                }
                if (topRight.x > screenWidth)
                {
                    clampedPosition.x -= (topRight.x - screenWidth) / canvas.scaleFactor;
                }

                // Clamp vertically
                if (bottomLeft.y < 0)
                {
                    clampedPosition.y += -bottomLeft.y / canvas.scaleFactor;
                }
                if (topRight.y > screenHeight)
                {
                    clampedPosition.y -= (topRight.y - screenHeight) / canvas.scaleFactor;
                }

                // Apply the clamped position
                return clampedPosition;
            }

            return Vector2.zero;
        }

        /// <summary>
        /// Gets the position of the RectTransform clamped inside the screen or a specified content area.
        /// </summary>
        /// <param name="rectTransform">The RectTransform.</param>
        /// <param name="canvas">The canvas.</param>
        /// <param name="content">Optional content RectTransform to clamp within. If null, clamps to screen.</param>
        /// <returns>The clamped position.</returns>
        public static Vector3 GetClampedPosition(RectTransform rectTransform, Canvas canvas, RectTransform content = null)
        {
            Vector3[] worldCorners = new Vector3[4];
            rectTransform.GetWorldCorners(worldCorners);
            Camera cam = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;
            Vector3 bottomLeft = RectTransformUtility.WorldToScreenPoint(cam, worldCorners[0]) + rectTransform.sizeDelta * canvas.scaleFactor;
            Vector3 topRight = RectTransformUtility.WorldToScreenPoint(cam, worldCorners[2]) - rectTransform.sizeDelta * canvas.scaleFactor;
            Vector3 position = rectTransform.position;
            Vector2 offset = Vector2.zero;

            // Get clamp boundaries
            float minX, maxX, minY, maxY;

            if (content != null)
            {
                // Clamp to content RectTransform
                Vector3[] contentCorners = new Vector3[4];
                content.GetWorldCorners(contentCorners);
                Vector3 contentBottomLeft = RectTransformUtility.WorldToScreenPoint(cam, contentCorners[0]);
                Vector3 contentTopRight = RectTransformUtility.WorldToScreenPoint(cam, contentCorners[2]);

                minX = contentBottomLeft.x;
                maxX = contentTopRight.x;
                minY = contentBottomLeft.y;
                maxY = contentTopRight.y;
            }
            else
            {
                // Clamp to screen
                minX = 0;
                maxX = Screen.width;
                minY = 0;
                maxY = Screen.height;
            }

            // Horizontal Clamping
            if (bottomLeft.x < minX)
            {
                offset.x = minX - bottomLeft.x;
            }
            else if (topRight.x > maxX)
            {
                offset.x = maxX - topRight.x;
            }

            // Vertical Clamping
            if (bottomLeft.y < minY)
            {
                offset.y = minY - bottomLeft.y;
            }
            else if (topRight.y > maxY)
            {
                offset.y = maxY - topRight.y;
            }

            // Apply offset in screen space and convert back to world space
            if (offset != Vector2.zero)
            {
                Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(cam, position);
                screenPos += (Vector3)offset;
                RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, screenPos, cam, out Vector3 worldPos);
                return worldPos;
            }

            return position;
        }

        /// <summary>
        /// Gets the mouse position.
        /// </summary>
        /// <returns>The mouse position.</returns>
        public static Vector2 GetMousePosition()
        {
            var mousePosition = Mouse.current.position.ReadValue();
            mousePosition.y = Screen.height - mousePosition.y;
            return mousePosition;
        }

        /// <summary>
        /// Gets the mouse position.
        /// </summary>
        /// <returns>The mouse position.</returns>
        public static Vector2 GetMousePositionUGUI()
        {
            return Mouse.current.position.ReadValue();
        }

        /// <summary>
        /// Forces a visual element inside the game viewport if it's somewhat outside of it.
        /// </summary>
        /// <param name="element">The visual element.</param>
        public static void ForceInsideView(VisualElement element)
        {
            if (element.worldBound.xMin < 0)
            {
                element.style.left = element.resolvedStyle.left - element.worldBound.xMin;
            }
            else if (element.worldBound.xMax > Screen.width)
            {
                element.style.left = element.resolvedStyle.left - (element.worldBound.xMax - Screen.width);
            }

            if (element.worldBound.yMin < 0)
            {
                element.style.top = element.resolvedStyle.top - element.worldBound.yMin;
            }
            else if (element.worldBound.yMax > Screen.height)
            {
                element.style.top = element.resolvedStyle.top - (element.worldBound.yMax - Screen.height);
            }
        }

        /// <summary>
        /// Forces a RectTransform inside the screen.
        /// </summary>
        /// <param name="rectTransform">The RectTransform.</param>
        /// <param name="canvas">The canvas.</param>
        public static void ForceInsideView(RectTransform rectTransform, Canvas canvas)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                // Get screen dimensions in canvas space
                Vector2 canvasSize = new Vector2(Screen.width, Screen.height);

                // Get the size of the RectTransform
                Vector2 size = rectTransform.sizeDelta * rectTransform.lossyScale;

                // Get the position of the RectTransform relative to the screen
                Vector3[] worldCorners = new Vector3[4];
                rectTransform.GetWorldCorners(worldCorners);

                // Convert world corners to screen space
                Vector3 bottomLeft = RectTransformUtility.WorldToScreenPoint(null, worldCorners[0]);
                Vector3 topRight = RectTransformUtility.WorldToScreenPoint(null, worldCorners[2]);

                // Calculate clamped position
                Vector2 clampedPosition = rectTransform.anchoredPosition;
                Vector2 pivotOffset = rectTransform.pivot - new Vector2(0.5f, 0.5f);

                // Clamp horizontally
                if (bottomLeft.x < 0)
                    clampedPosition.x += -bottomLeft.x;
                if (topRight.x > canvasSize.x)
                    clampedPosition.x -= (topRight.x - canvasSize.x);

                // Clamp vertically
                if (bottomLeft.y < 0)
                {
                    clampedPosition.y += -bottomLeft.y;
                }
                if (topRight.y > canvasSize.y)
                {
                    clampedPosition.y -= (topRight.y - canvasSize.y);
                }

                // Apply the clamped position
                rectTransform.anchoredPosition = clampedPosition;
            }
            else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                // Get the camera from the canvas
                Camera cam = canvas.worldCamera;
                if (cam == null) return;

                // Convert the RectTransform's position to screen space
                Vector3[] worldCorners = new Vector3[4];
                rectTransform.GetWorldCorners(worldCorners);

                // Convert each world corner to screen space
                Vector3 bottomLeft = RectTransformUtility.WorldToScreenPoint(cam, worldCorners[0]);
                Vector3 topRight = RectTransformUtility.WorldToScreenPoint(cam, worldCorners[2]);

                // Get the screen bounds
                float screenWidth = Screen.width;
                float screenHeight = Screen.height;

                // Calculate the new anchored position
                Vector2 clampedPosition = rectTransform.anchoredPosition;

                // Clamp horizontally
                if (bottomLeft.x < 0)
                {
                    clampedPosition.x += -bottomLeft.x / canvas.scaleFactor;
                }
                if (topRight.x > screenWidth)
                {
                    clampedPosition.x -= (topRight.x - screenWidth) / canvas.scaleFactor;
                }

                // Clamp vertically
                if (bottomLeft.y < 0)
                {
                    clampedPosition.y += -bottomLeft.y / canvas.scaleFactor;
                }
                if (topRight.y > screenHeight)
                {
                    clampedPosition.y -= (topRight.y - screenHeight) / canvas.scaleFactor;
                }

                // Apply the clamped position
                rectTransform.anchoredPosition = clampedPosition;
            }
        }

        /// <summary>
        /// Gets the next size.
        /// </summary>
        /// <param name="current">The current size.</param>
        /// <returns>The next size.</returns>
        public static Size Next(this Size current)
        {
            Size[] values = (Size[])Enum.GetValues(typeof(Size));
            int index = Array.IndexOf(values, current);
            int nextIndex = (index + 1) % values.Length;
            return values[nextIndex];
        }

        /// <summary>
        /// Gets the previous size.
        /// </summary>
        /// <param name="current">The current size.</param>
        /// <returns>The previous size.</returns>
        public static Size Previous(this Size current)
        {
            Size[] values = (Size[])Enum.GetValues(typeof(Size));
            int index = Array.IndexOf(values, current);
            int prevIndex = (index - 1 + values.Length) % values.Length;
            return values[prevIndex];
        }
    }
}