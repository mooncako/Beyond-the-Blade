using UnityEngine;
using UnityEngine.AI;

public static class AIUtil
{
    public static Vector3 GetRandomSpawnPos(NavMeshTriangulation triangulation)
    {
        NavMeshHit hit;

        int vertexIndex = Random.Range(0, triangulation.vertices.Length);
        for (int i = 0; i < 30; i++)
        {
            if (NavMesh.SamplePosition(triangulation.vertices[vertexIndex], out hit, 50, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        Debug.LogError("PositionNotFound");
        return Vector3.zero;
        
    }
}
