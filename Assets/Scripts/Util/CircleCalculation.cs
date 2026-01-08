using UnityEngine;
using UnityEngine.AI;

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
    public static Vector3 RandomPointOnCircleFromEdgeSafe(
        Vector3 center,
        Vector3 edgePoint,
        float radius,
        NavMeshAgent agent,                 // used to validate reachability (and for areaMask if desired)
        float edgeBuffer = 0.5f,            // min distance from any NavMesh edge
        float sampleMaxDistance = 1.5f,
        int areaMask = NavMesh.AllAreas,
        int maxAttempts = 12,
        float maxHeightDelta = 0.6f         // reject candidates far above/below the center's mesh
    )
    {
        // Build basis vectors on XZ plane
        Vector3 r = new Vector3(edgePoint.x - center.x, 0f, edgePoint.z - center.z);
        Vector3 u = r.sqrMagnitude > 1e-12f ? r.normalized : Vector3.right;
        Vector3 v = Vector3.Cross(Vector3.up, u).normalized;

        // Snap 'center' to its closest point on the mesh so our height delta check is meaningful
        if (!NavMesh.SamplePosition(center, out var centerHit, sampleMaxDistance, areaMask))
        {
            // If we can’t even find the center on the mesh, bail
            return Vector3.zero;
        }

        var path = new NavMeshPath();

        for (int i = 0; i < maxAttempts; i++)
        {
            float theta = Random.Range(0f, Mathf.PI * 2f);
            Vector3 offset = (Mathf.Cos(theta) * u + Mathf.Sin(theta) * v) * radius;
            Vector3 candidate = new Vector3(center.x + offset.x, center.y, center.z + offset.z);

            // 1) Snap candidate to NavMesh
            if (!NavMesh.SamplePosition(candidate, out var hit, sampleMaxDistance, areaMask))
                continue;

            // 2) Reject if too close to an edge
            if (NavMesh.FindClosestEdge(hit.position, out var edge, areaMask))
            {
                if (edge.distance < edgeBuffer)
                    continue;
            }
            else
            {
                // If we can’t evaluate the edge distance, be conservative
                continue;
            }

            // 3) Reject large vertical mismatches (often across different floors/ledges)
            if (Mathf.Abs(hit.position.y - centerHit.position.y) > maxHeightDelta)
                continue;

            // 4) Require a COMPLETE path from the agent (or from centerHit) to the candidate
            // Prefer agent.CalculatePath so agent’s radius/step/area costs apply
            bool reachable;
            if (agent != null && agent.isOnNavMesh)
            {
                reachable = agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete;
            }
            else
            {
                // Fallback: compute from centerHit
                reachable = NavMesh.CalculatePath(centerHit.position, hit.position, areaMask, path)
                            && path.status == NavMeshPathStatus.PathComplete;
            }

            if (!reachable)
                continue;

            // 5) Optional: ensure there isn’t a boundary between center and candidate
            // (helps avoid choosing points across a gap)
            if (NavMesh.Raycast(centerHit.position, hit.position, out var castHit, areaMask))
                continue;

            // Passed all checks — use it
            return hit.position;
        }

        // Fallback: stay put (or use centerHit.position if you prefer not to return zero)
        return centerHit.position;
    }


}
