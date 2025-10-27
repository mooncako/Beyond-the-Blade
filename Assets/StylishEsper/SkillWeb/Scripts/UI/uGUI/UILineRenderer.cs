//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Esper.SkillWeb.UI.UGUI
{
    /// <summary>
    /// A UI component for rendering customizable lines with width curves, color gradients, and rounded caps/corners.
    /// </summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public class UILineRenderer : MaskableGraphic
    {
        /// <summary>
        /// The list of points that define the line path.
        /// </summary>
        [SerializeField]
        private List<Vector2> points = new List<Vector2>();

        /// <summary>
        /// The width of the line in units.
        /// </summary>
        [SerializeField]
        private float lineWidth = 1f;

        /// <summary>
        /// Animation curve that controls how the line width varies along its length.
        /// </summary>
        [SerializeField]
        private AnimationCurve widthCurve = AnimationCurve.Constant(0f, 1f, 1f);

        /// <summary>
        /// Gradient that controls how the line color varies along its length.
        /// </summary>
        [SerializeField]
        private Gradient colorGradient = new Gradient();

        /// <summary>
        /// Number of vertices used to create smooth corners between line segments.
        /// </summary>
        [SerializeField]
        private int cornerVertices = 0;

        /// <summary>
        /// Number of vertices used to create rounded end caps at the line ends.
        /// </summary>
        [SerializeField]
        private int endCapVertices = 0;

        /// <summary>
        /// Whether the line should form a closed loop by connecting the last point to the first.
        /// </summary>
        [SerializeField]
        private bool loop = false;

        /// <summary>
        /// The instanced material cached when materialForRendering is called. This is essentially the material applied
        /// to the line.
        /// </summary>
        [HideInInspector]
        public Material cachedMaterial;

        public override Material material 
        { 
            get
            {
                if (cachedMaterial)
                    return cachedMaterial;

                if (m_Material)
                    return m_Material;

                return defaultMaterial;
            }
            set
            {
                base.material = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the line.
        /// </summary>
        public float LineWidth
        {
            get { return lineWidth; }
            set { lineWidth = value; SetVerticesDirty(); }
        }

        /// <summary>
        /// Gets or sets the curve that controls line width along its length.
        /// </summary>
        public AnimationCurve WidthCurve
        {
            get { return widthCurve; }
            set { widthCurve = value; SetVerticesDirty(); }
        }

        /// <summary>
        /// Gets or sets the gradient that controls line color along its length.
        /// </summary>
        public Gradient ColorGradient
        {
            get { return colorGradient; }
            set { colorGradient = value; SetVerticesDirty(); }
        }

        /// <summary>
        /// Gets or sets the number of vertices used for rounded corners.
        /// </summary>
        public int CornerVertices
        {
            get { return cornerVertices; }
            set { cornerVertices = Mathf.Max(0, value); SetVerticesDirty(); }
        }

        /// <summary>
        /// Gets or sets the number of vertices used for rounded end caps.
        /// </summary>
        public int EndCapVertices
        {
            get { return endCapVertices; }
            set { endCapVertices = Mathf.Max(0, value); SetVerticesDirty(); }
        }

        /// <summary>
        /// Gets or sets whether the line should connect the last point to the first point.
        /// </summary>
        public bool Loop
        {
            get { return loop; }
            set { loop = value; SetVerticesDirty(); }
        }

        /// <summary>
        /// Gets the main texture from the current material.
        /// </summary>
        public override Texture mainTexture
        {
            get
            {
                Material currentMat = GetCurrentMaterial();
                if (currentMat != null)
                    return currentMat.mainTexture;
                return base.mainTexture;
            }
        }

        /// <summary>
        /// Gets or sets the number of positions in the line.
        /// </summary>
        public int positionCount
        {
            get { return points.Count; }
            set
            {
                if (value < 0) value = 0;
                while (points.Count < value)
                    points.Add(Vector2.zero);
                while (points.Count > value)
                    points.RemoveAt(points.Count - 1);
                SetVerticesDirty();
            }
        }

        public override Material materialForRendering
        {
            get
            {
                Material baseMaterial = GetCurrentMaterial();
                var modified = GetModifiedMaterial(baseMaterial);
                cachedMaterial = new Material(modified);
                return cachedMaterial;
            }
        }

        /// <summary>
        /// Gets the current material that should be used for rendering.
        /// </summary>
        /// <returns>The material to use for rendering.</returns>
        public Material GetCurrentMaterial()
        {
            if (material)
            { 
                return material;
            }

            return defaultMaterial;
        }

        /// <summary>
        /// Populates the mesh with line vertices and triangles.
        /// </summary>
        /// <param name="vh">The vertex helper to populate.</param>
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (points.Count < 2)
                return;

            GenerateLineMesh(vh);
        }

        /// <summary>
        /// Generates the complete line mesh including segments, loops, and end caps.
        /// </summary>
        /// <param name="vh">The vertex helper to add vertices to.</param>
        private void GenerateLineMesh(VertexHelper vh)
        {
            List<Vector2> processedPoints = new List<Vector2>(points);

            if (processedPoints.Count < 2)
                return;

            // Generate vertices for line segments
            for (int i = 0; i < processedPoints.Count - 1; i++)
            {
                GenerateLineSegment(vh, processedPoints[i], processedPoints[i + 1], i, processedPoints.Count - 1);
            }

            // Handle loop connection
            if (loop && processedPoints.Count > 2)
            {
                GenerateLineSegment(vh, processedPoints[processedPoints.Count - 1], processedPoints[0],
                                  processedPoints.Count - 1, processedPoints.Count - 1);
            }

            // Generate end caps if specified
            if (endCapVertices > 0 && !loop)
            {
                GenerateEndCap(vh, processedPoints[0], processedPoints[1], true);
                GenerateEndCap(vh, processedPoints[processedPoints.Count - 1],
                              processedPoints[processedPoints.Count - 2], false);
            }
        }

        /// <summary>
        /// Generates vertices for a single line segment between two points.
        /// </summary>
        /// <param name="vh">The vertex helper to add vertices to.</param>
        /// <param name="start">The starting point of the segment.</param>
        /// <param name="end">The ending point of the segment.</param>
        /// <param name="segmentIndex">The index of this segment in the line.</param>
        /// <param name="totalSegments">The total number of segments in the line.</param>
        private void GenerateLineSegment(VertexHelper vh, Vector2 start, Vector2 end, int segmentIndex, int totalSegments)
        {
            Vector2 direction = (end - start).normalized;
            Vector2 perpendicular = new Vector2(-direction.y, direction.x);

            float startT = (float)segmentIndex / totalSegments;
            float endT = (float)(segmentIndex + 1) / totalSegments;

            float startWidth = lineWidth * widthCurve.Evaluate(startT) * 0.5f;
            float endWidth = lineWidth * widthCurve.Evaluate(endT) * 0.5f;

            Color startColor = colorGradient.Evaluate(startT) * color;
            Color endColor = colorGradient.Evaluate(endT) * color;

            // Create quad vertices
            Vector2 startOffset = perpendicular * startWidth;
            Vector2 endOffset = perpendicular * endWidth;

            UIVertex[] vertices = new UIVertex[4];

            // Calculate UV coordinates for material tiling
            float uvStart = startT;
            float uvEnd = endT;

            // Bottom left
            vertices[0] = CreateVertex(start - startOffset, startColor, new Vector2(uvStart, 0));
            // Top left  
            vertices[1] = CreateVertex(start + startOffset, startColor, new Vector2(uvStart, 1));
            // Top right
            vertices[2] = CreateVertex(end + endOffset, endColor, new Vector2(uvEnd, 1));
            // Bottom right
            vertices[3] = CreateVertex(end - endOffset, endColor, new Vector2(uvEnd, 0));

            vh.AddUIVertexQuad(vertices);
        }

        /// <summary>
        /// Generates a rounded end cap at the start or end of the line.
        /// </summary>
        /// <param name="vh">The vertex helper to add vertices to.</param>
        /// <param name="center">The center point of the end cap.</param>
        /// <param name="adjacent">The adjacent point used to determine direction.</param>
        /// <param name="isStart">True if this is the start cap, false if it's the end cap.</param>
        private void GenerateEndCap(VertexHelper vh, Vector2 center, Vector2 adjacent, bool isStart)
        {
            if (endCapVertices <= 0) return;

            Vector2 direction = (adjacent - center).normalized;
            if (!isStart) direction = -direction;

            float t = isStart ? 0f : 1f;
            float width = lineWidth * widthCurve.Evaluate(t) * 0.5f;
            Color capColor = colorGradient.Evaluate(t) * color;

            Vector2 perpendicular = new Vector2(-direction.y, direction.x);

            // Create fan of triangles for rounded end cap
            int vertexStart = vh.currentVertCount;

            // Center vertex
            vh.AddVert(CreateVertex(center, capColor, new Vector2(0.5f, 0.5f)));

            // Create vertices around the cap
            for (int i = 0; i <= endCapVertices; i++)
            {
                float angle = Mathf.PI * i / endCapVertices;
                Vector2 offset = (Mathf.Cos(angle) * perpendicular + Mathf.Sin(angle) * direction) * width;
                Vector2 uv = new Vector2(0.5f + Mathf.Cos(angle) * 0.5f, 0.5f + Mathf.Sin(angle) * 0.5f);
                vh.AddVert(CreateVertex(center + offset, capColor, uv));
            }

            // Create triangles
            for (int i = 0; i < endCapVertices; i++)
            {
                vh.AddTriangle(vertexStart, vertexStart + i + 1, vertexStart + i + 2);
            }
        }

        /// <summary>
        /// Creates a UI vertex with the specified position, color, and UV coordinates.
        /// </summary>
        /// <param name="position">The world position of the vertex.</param>
        /// <param name="color">The color of the vertex.</param>
        /// <param name="uv">The UV coordinates of the vertex.</param>
        /// <returns>A configured UIVertex.</returns>
        private UIVertex CreateVertex(Vector2 position, Color color, Vector2 uv)
        {
            UIVertex vertex = UIVertex.simpleVert;
            vertex.position = position;
            vertex.color = color;
            vertex.uv0 = uv;
            return vertex;
        }

        /// <summary>
        /// Sets the position of a specific point in the line.
        /// </summary>
        /// <param name="index">The index of the point to set.</param>
        /// <param name="position">The new position for the point.</param>
        public void SetPosition(int index, Vector2 position)
        {
            if (index >= 0 && index < points.Count)
            {
                points[index] = position;
                SetVerticesDirty();
            }
        }

        /// <summary>
        /// Gets the position of a specific point in the line.
        /// </summary>
        /// <param name="index">The index of the point to get.</param>
        /// <returns>The position of the point, or Vector2.zero if index is invalid.</returns>
        public Vector2 GetPosition(int index)
        {
            if (index >= 0 && index < points.Count)
                return points[index];
            return Vector2.zero;
        }

        /// <summary>
        /// Sets all positions in the line from an array.
        /// </summary>
        /// <param name="positions">Array of positions to set.</param>
        public void SetPositions(Vector2[] positions)
        {
            points.Clear();
            points.AddRange(positions);
            SetVerticesDirty();
        }

        /// <summary>
        /// Sets all positions in the line from a list.
        /// </summary>
        /// <param name="positions">List of positions to set.</param>
        public void SetPositions(List<Vector2> positions)
        {
            points.Clear();
            points.AddRange(positions);
            SetVerticesDirty();
        }

        /// <summary>
        /// Gets all positions in the line and copies them to the provided array.
        /// </summary>
        /// <param name="positions">Array to copy positions into.</param>
        /// <returns>The number of positions copied.</returns>
        public int GetPositions(Vector2[] positions)
        {
            int count = Mathf.Min(positions.Length, points.Count);
            for (int i = 0; i < count; i++)
            {
                positions[i] = points[i];
            }
            return count;
        }

        /// <summary>
        /// Adds a new position to the end of the line.
        /// </summary>
        /// <param name="position">The position to add.</param>
        public void AddPosition(Vector2 position)
        {
            points.Add(position);
            SetVerticesDirty();
        }

        /// <summary>
        /// Removes a position at the specified index.
        /// </summary>
        /// <param name="index">The index of the position to remove.</param>
        public void RemovePosition(int index)
        {
            if (index >= 0 && index < points.Count)
            {
                points.RemoveAt(index);
                SetVerticesDirty();
            }
        }

        /// <summary>
        /// Removes all positions from the line.
        /// </summary>
        public void ClearPositions()
        {
            points.Clear();
            SetVerticesDirty();
        }

        /// <summary>
        /// Sets the position of a specific point using a 3D vector (Z value is ignored).
        /// </summary>
        /// <param name="index">The index of the point to set.</param>
        /// <param name="position">The new 3D position for the point.</param>
        public void SetPosition3D(int index, Vector3 position)
        {
            SetPosition(index, new Vector2(position.x, position.y));
        }

        /// <summary>
        /// Sets all positions in the line from a 3D array (Z values are ignored).
        /// </summary>
        /// <param name="positions">Array of 3D positions to set.</param>
        public void SetPositions3D(Vector3[] positions)
        {
            Vector2[] positions2D = new Vector2[positions.Length];
            for (int i = 0; i < positions.Length; i++)
            {
                positions2D[i] = new Vector2(positions[i].x, positions[i].y);
            }
            SetPositions(positions2D);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Validates component properties in the editor
        /// </summary>
        protected override void OnValidate()
        {
            base.OnValidate();
            lineWidth = Mathf.Max(0.01f, lineWidth);
            cornerVertices = Mathf.Max(0, cornerVertices);
            endCapVertices = Mathf.Max(0, endCapVertices);

            if (widthCurve == null)
                widthCurve = AnimationCurve.Constant(0f, 1f, 1f);

            if (colorGradient == null)
            {
                colorGradient = new Gradient();
                colorGradient.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
                );
            }

            // Force material update in editor
            SetMaterialDirty();
        }
#endif
    }
}