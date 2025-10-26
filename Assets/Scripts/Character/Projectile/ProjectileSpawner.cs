using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns and manages projectiles for a character - follows DoppelgangerSpawner pattern
/// Attach this component to characters that need to shoot projectiles
/// </summary>
[RequireComponent(typeof(ObjectPool))]
public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private ObjectPool _pool;
    [SerializeField, BoxGroup("References")] private List<ProjectilePrefabEntry> _projectilePrefabs;
    [SerializeField, BoxGroup("Settings")] private PoolConfig _poolConfig;

    [System.Serializable]
    public class ProjectilePrefabEntry
    {
        public string ID;
        public GameObject Prefab;
    }

    private Dictionary<string, GameObject> _projectileDict;

    void OnValidate()
    {
        if (_pool == null) _pool = GetComponent<ObjectPool>();
    }

    void Start()
    {
        // Build dictionary for quick lookup
        _projectileDict = new Dictionary<string, GameObject>();
        List<GameObject> poolList = new List<GameObject>();

        foreach (var entry in _projectilePrefabs)
        {
            if (!string.IsNullOrEmpty(entry.ID) && entry.Prefab != null)
            {
                _projectileDict[entry.ID] = entry.Prefab;
                poolList.Add(entry.Prefab);
            }
        }

        // Initialize pool (same as DoppelgangerSpawner line 14-18)
        _pool.InitializeRuntimePool(poolList, _poolConfig.InitialSize, _poolConfig.MaxSize, _poolConfig.CanExpand);
    }

    /// <summary>
    /// Spawn a projectile - direct call, no events (like DoppelgangerSpawner.Spawn)
    /// </summary>
    public void SpawnProjectile(string projectileId, Vector3 position, Vector3 direction, GameObject owner, ProjectileData data)
    {
        // Get prefab from dictionary
        if (!_projectileDict.TryGetValue(projectileId, out GameObject prefab))
        {
            Debug.LogError($"Projectile ID '{projectileId}' not found in projectile prefabs.");
            return;
        }

        // Get from pool (same as DoppelgangerSpawner line 29)
        BaseProjectile projectile = _pool.Get(prefab).GetComponent<BaseProjectile>();
        if (projectile == null)
        {
            Debug.LogError($"Prefab '{prefab.name}' does not contain a BaseProjectile component.");
            return;
        }

        // Setup position and rotation (same as DoppelgangerSpawner line 30-31)
        projectile.transform.position = position;
        projectile.transform.rotation = Quaternion.LookRotation(direction);

        // Setup projectile (same as DoppelgangerSpawner line 32-33)
        projectile.SetOwner(owner);
        projectile.Initialize(data);
        projectile.ObjectPool = _pool;

        // Listen for destruction to return to pool (same as DoppelgangerSpawner line 35)
        projectile.OnProjectileDestroyed.AddListener(() => OnProjectileDestroyed(projectile.gameObject));

        // Launch (same as DoppelgangerSpawner line 34)
        projectile.LaunchInDirection(direction);
    }

    private void OnProjectileDestroyed(GameObject projectile)
    {
        // Return to pool (same as DoppelgangerSpawner line 38-40)
        _pool.Return(projectile);
    }
}

