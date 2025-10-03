using System.Collections;
using System.Collections.Generic;
using Steamworks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Interface for objects that can be pooled
/// </summary>
public interface IPoolable
{
    /// <summary>
    /// Called when object is retrieved from pool
    /// </summary>
    void OnPoolGet();
    
    /// <summary>
    /// Called when object is returned to pool
    /// </summary>
    void OnPoolReturn();
}

/// <summary>
/// Enhanced object pooling system with type-safe operations and performance optimizations
/// </summary>
public class ObjectPool : MonoBehaviour
{

    [Header("Pool Configuration")]
    [SerializeField] private List<PoolConfig> _poolConfigs = new List<PoolConfig>();
    
    [Header("Settings")]
    [SerializeField] private bool _logPoolStats = false;
    [SerializeField] private bool _turnOnDebugLog = false;
    [SerializeField] private bool _initializeOnStart = true;
    [SerializeField] private bool _initializeAsChild = false;   

    // Pool storage: Prefab -> Queue of available objects
    private Dictionary<GameObject, Queue<GameObject>> _pools = new Dictionary<GameObject, Queue<GameObject>>();
    private Dictionary<GameObject, PoolConfig> _configs = new Dictionary<GameObject, PoolConfig>();
    private Dictionary<GameObject, int> _activeCount = new Dictionary<GameObject, int>();


    #region Unity Lifecycle

    private void Awake()
    {
        if(_initializeOnStart)
            InitializePools();
    }

    #endregion

    #region Initialization

    private void InitializePools()
    {
        // Initialize each pool
        foreach (var config in _poolConfigs)
        {
            if (config.prefab != null)
            {
                CreatePool(config);
            }
        }

        if (_logPoolStats)
        {
            LogPoolStatistics();
        }
    }

    private void CreatePool(PoolConfig config)
    {
        var prefab = config.prefab;
        var queue = new Queue<GameObject>();
        
        // Store config and initialize counters
        _configs[prefab] = config;
        _activeCount[prefab] = 0;

        // Pre-instantiate objects
        for (int i = 0; i < config.initialSize; i++)
        {
            GameObject obj = CreatePooledObject(prefab);
            queue.Enqueue(obj);
        }

        _pools[prefab] = queue;
    }

    private GameObject CreatePooledObject(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, transform.position, Quaternion.identity);
        obj.name = $"{prefab.name}_Pooled";
        if (_initializeAsChild)
            obj.transform.parent = transform;

        obj.SetActive(false);
        return obj;
    }

    #endregion

    #region Public API

    /// <summary>
    /// Get an object from the pool
    /// </summary>
    public GameObject Get(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("Cannot get object: prefab is null");
            return null;
        }

        if (!_pools.ContainsKey(prefab))
        {
            if (_turnOnDebugLog) Debug.LogWarning($"Pool for prefab '{prefab.name}' not found. Creating runtime pool.");
            CreateRuntimePool(prefab);
        }

        var pool = _pools[prefab];
        var config = _configs[prefab];
        GameObject obj = null;

        // Try to get from pool
        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        // Expand pool if allowed and under max size
        else if (config.canExpand && (config.maxSize == 0 || GetTotalCount(prefab) < config.maxSize))
        {
            obj = CreatePooledObject(prefab);
        }

        if (obj != null)
        {
            obj.SetActive(true);
            _activeCount[prefab]++;

            // Call poolable interface if available
            var poolable = obj.GetComponent<IPoolable>();
            poolable?.OnPoolGet();

            return obj;
        }

        if (_turnOnDebugLog) Debug.LogWarning($"Pool for '{prefab.name}' is exhausted and cannot expand");
        return null;
    }

    /// <summary>
    /// Get an object from the pool with type safety
    /// </summary>
    public T Get<T>(GameObject prefab) where T : Component
    {
        GameObject obj = Get(prefab);
        return obj?.GetComponent<T>();
    }

    /// <summary>
    /// Return an object to the pool
    /// </summary>
    public void Return(GameObject obj)
    {
        if (obj == null)
        {
            if (_turnOnDebugLog) Debug.LogError("Cannot return null object to pool");
            return;
        }

        // Find which prefab this object belongs to
        GameObject prefab = FindPrefabForObject(obj);
        if (prefab == null)
        {
            if (_turnOnDebugLog) Debug.LogWarning($"Object '{obj.name}' doesn't belong to any pool. Destroying instead.");
            Destroy(obj);
            return;
        }

        // Call poolable interface if available
        var poolable = obj.GetComponent<IPoolable>();
        poolable?.OnPoolReturn();

        // Reset object state
        obj.SetActive(false);
        
        // Return to pool
        _pools[prefab].Enqueue(obj);
        _activeCount[prefab]--;
    }

    /// <summary>
    /// Return an object to the pool after a delay
    /// </summary>
    public void ReturnAfterDelay(GameObject obj, float delay)
    {
        if (obj != null)
        {
            StartCoroutine(ReturnAfterDelayCoroutine(obj, delay));
        }
    }

    /// <summary>
    /// Prewarm a specific pool
    /// </summary>
    public void PrewarmPool(GameObject prefab, int count)
    {
        if (!_pools.ContainsKey(prefab))
        {
            if (_turnOnDebugLog) Debug.LogError($"Pool for prefab '{prefab.name}' not found");
            return;
        }

        var config = _configs[prefab];
        var pool = _pools[prefab];

        for (int i = 0; i < count; i++)
        {
            if (config.maxSize > 0 && GetTotalCount(prefab) >= config.maxSize)
                break;

            GameObject obj = CreatePooledObject(prefab);
            pool.Enqueue(obj);
        }
    }

    /// <summary>
    /// Clear all objects from a specific pool
    /// </summary>
    public void ClearPool(GameObject prefab)
    {
        if (!_pools.ContainsKey(prefab))
            return;

        var pool = _pools[prefab];
        while (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        
        _activeCount[prefab] = 0;
    }

    /// <summary>
    /// Clear all pools
    /// </summary>
    public void ClearAllPools()
    {
        if (_pools.Count == 0) return;
        foreach (var prefab in _pools.Keys)
        {
            ClearPool(prefab);
        }
    }

    /// <summary>
    /// Clear the current pool, and build a new pool at runtime based on the prefabs
    /// </summary>
    /// <param name="prefabs"></param>
    public void InitializeRuntimePool(List<GameObject> prefabs)
    {
        ClearAllPools();
        _poolConfigs.Clear();
        foreach (GameObject go in prefabs)
        {
            var config = new PoolConfig
            {
                prefab = go,
                initialSize = 20,
                maxSize = 40,
                canExpand = true
            };
            _poolConfigs.Add(config);
        }

        InitializePools();
    }

    #endregion

    #region Pool Statistics

    public int GetActiveCount(GameObject prefab)
    {
        return _activeCount.ContainsKey(prefab) ? _activeCount[prefab] : 0;
    }

    public int GetAvailableCount(GameObject prefab)
    {
        return _pools.ContainsKey(prefab) ? _pools[prefab].Count : 0;
    }

    public int GetTotalCount(GameObject prefab)
    {
        return GetActiveCount(prefab) + GetAvailableCount(prefab);
    }

    public void LogPoolStatistics()
    {
        if (_turnOnDebugLog)     Debug.Log("=== Object Pool Statistics ===");
        foreach (var prefab in _pools.Keys)
        {
            if (_turnOnDebugLog) Debug.Log($"{prefab.name}: Active={GetActiveCount(prefab)}, Available={GetAvailableCount(prefab)}, Total={GetTotalCount(prefab)}");
        }
    }

    #endregion

    #region Private Methods

    private void CreateRuntimePool(GameObject prefab)
    {
        var config = new PoolConfig
        {
            prefab = prefab,
            initialSize = 5,
            maxSize = 20,
            canExpand = true
        };
        CreatePool(config);
    }

    private GameObject FindPrefabForObject(GameObject obj)
    {
        string objName = obj.name.Replace("_Pooled", "").Replace("(Clone)", "");
        
        foreach (var prefab in _pools.Keys)
        {
            if (prefab.name == objName)
            {
                return prefab;
            }
        }
        
        return null;
    }

    private System.Collections.IEnumerator ReturnAfterDelayCoroutine(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Return(obj);
    }

    #endregion

    #region Debug

    [ContextMenu("Log Pool Stats")]
    private void LogStats()
    {
        LogPoolStatistics();
    }


    #endregion
}
