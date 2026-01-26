using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class AIUtil
{
    private static NavMeshTriangulation _tri;
    private static float[] _triCumAreas;
    private static int _triCount;

    public static void BuildSampler()
    {
        _tri = NavMesh.CalculateTriangulation();
        _triCount = _tri.indices.Length / 3;

        _triCumAreas = new float[_triCount];
        float cum = 0f;

        for (int i = 0; i < _triCount; i++)
        {
            var a = _tri.vertices[_tri.indices[i * 3 + 0]];
            var b = _tri.vertices[_tri.indices[i * 3 + 1]];
            var c = _tri.vertices[_tri.indices[i * 3 + 2]];

            float area = Vector3.Cross(b - a, c - a).magnitude * .5f;
            cum += Mathf.Max(area, 1e-6f);
            _triCumAreas[i] = cum;
        }
    }

    public static Vector3 GetRandomPointOnNavMesh(int areaMask = NavMesh.AllAreas)
    {
        if (_triCumAreas == null || _triCumAreas.Length == 0) BuildSampler();
        float r = Random.value * _triCumAreas[_triCumAreas.Length - 1];

        int t = System.Array.FindIndex(_triCumAreas, cum => cum >= r);
        if (t < 0) t = _triCount - 1;

        var a = _tri.vertices[_tri.indices[t * 3 + 0]];
        var b = _tri.vertices[_tri.indices[t * 3 + 1]];
        var c = _tri.vertices[_tri.indices[t * 3 + 2]];

        float u = Random.value;
        float v = Random.value;

        if (u + v > 1f)
        {
            u = 1f - u;
            v = 1f - v;
        }

        Vector3 p = a + u * (b - a) + v * (c - a);

        return NavMesh.SamplePosition(p, out var hit, 0.5f, areaMask) ? hit.position : p;
    }

    public static Vector3 GetRandomSpawnPosFromArry(EnemySpawnPos[] spawnPositions, bool isPrecisePos = false)
    {
        int index = Random.Range(0, spawnPositions.Length);
        if (isPrecisePos)
        {
            return spawnPositions[index].transform.position;
        }
        Vector2 offset = Random.insideUnitCircle * spawnPositions[index].Radius;
        Vector3 spawnPos = new Vector3(spawnPositions[index].transform.position.x + offset.x, spawnPositions[index].transform.position.y, spawnPositions[index].transform.position.z + offset.y);
        return spawnPos;
    }

    public static Vector3 GetRandomSpawnPos(EnemySpawnPos spawnPosition, bool isPrecisePos = false)
    {
        if(isPrecisePos)
        {
            return spawnPosition.transform.position;
        }
        Vector2 offset = Random.insideUnitCircle * spawnPosition.Radius;
        Vector3 spawnPos = new Vector3(spawnPosition.transform.position.x + offset.x, spawnPosition.transform.position.y, spawnPosition.transform.position.z + offset.y);
        return spawnPos;
    }

    public static EnemyProfile PickEnemyBasedOnDifficultyIndex(List<EnemyProfile> eligibleEnemies, float difficultyIndex)
    {
        float totalWeight = 0f;
        foreach (var enemy in eligibleEnemies)
        {
            float weight = Mathf.Max(0f, difficultyIndex - enemy.DifficultyIndex + 1f);
            totalWeight += weight;
        }

        if (totalWeight <= 0f) return null;

        float r = Random.value * totalWeight;
        float cumulativeWeight = 0f;

        foreach (var enemy in eligibleEnemies)
        {
            float weight = Mathf.Max(0f, difficultyIndex - enemy.DifficultyIndex + 1f);
            cumulativeWeight += weight;

            if (r <= cumulativeWeight)
            {
                return enemy;
            }
        }

        return null;
    }
}
