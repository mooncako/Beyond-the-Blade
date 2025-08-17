using UnityEngine;

public static class CircleCalculation
{
    /// <summary>
    /// Gets a random point on a circle given the center of the circle, an edge point of the circle and the radius
    /// of the circle.
    /// </summary>
    /// <param name="center"></param>
    /// <param name="edgePoint"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public static Vector3 RandomPointOnCircleFromEdge(Vector3 center, Vector3 edgePoint, float radius)
    {
        Vector3 r = new Vector3(edgePoint.x - center.x, 0f, edgePoint.z - center.z);
        Vector3 u = r.sqrMagnitude > 1e-12f ? r.normalized : Vector3.right;

        Vector3 v = Vector3.Cross(Vector3.up, u).normalized;

        float theta = Random.Range(0f, Mathf.PI * 2f);
        Vector3 offset = (Mathf.Cos(theta) * u + Mathf.Sin(theta) * v) * radius;
        return new Vector3(center.x + offset.x, center.y, center.z + offset.z);
    }
}
