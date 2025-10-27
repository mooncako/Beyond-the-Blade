//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using UnityEngine;

namespace Esper.SkillWeb.UI.UGUI
{
    /// <summary>
    /// A connection line with support to curve like an arc.
    /// </summary>
    public class CurvedConnectionLine : ConnectionLine
    {
        /// <summary>
        /// The curve control point.
        /// </summary>
        public Vector3 controlPoint = Vector3.zero;

        /// <summary>
        /// The number of points on the line. The more there are, the smoother it will look.
        /// </summary>
        [Min(2)]
        public int points = 2;

        public override void UpdateLine()
        {
            // Do nothing if there are no line points
            if (points <= 0)
            {
                return;
            }

            // Set the number of points
            lineRenderer.positionCount = points;

            // Get the first and final positions
            Vector3 p0;
            Vector3 p2;

            if (connection != null)
            {
                p0 = Input.rectTransform.anchoredPosition;
                p2 = Output.rectTransform.anchoredPosition;
            }
            else
            {
                p0 = startPosition;
                p2 = Output.rectTransform.anchoredPosition;
            }

            // Midpoint between start and end
            Vector3 mid = (p0 + p2) / 2;

            Vector3 p1 = mid + controlPoint;

            // Loop for number of points
            for (int i = 0; i < points; i++)
            {
                float t = i / (float)(points - 1);

                // Set each point with a curve formula (Quadratic Bézier formula)
                Vector3 point = Mathf.Pow(1 - t, 2) * p0 + 2 * (1 - t) * t * p1 + Mathf.Pow(t, 2) * p2;
                lineRenderer.SetPosition(i, point);
            }
        }
    }
}