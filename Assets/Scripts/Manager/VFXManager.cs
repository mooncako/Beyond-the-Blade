using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(ObjectPool))]
public class VFXManager : MonoBehaviour,
    MMEventListener<SpawnVFXEvent>
{
    [SerializeField, BoxGroup("References")] private VFXPrefabDatabaseSO _prefabDatabase;
    [SerializeField, BoxGroup("References")] private ObjectPool _pool;
    [SerializeField, BoxGroup("Settings")] private PoolConfig _poolConfig;

    void OnValidate()
    {
        if (_pool == null) _pool = GetComponent<ObjectPool>();
    }

    void Start()
    {
        _pool.InitializeRuntimePool(_prefabDatabase.VFXPrefabs, _poolConfig.InitialSize, _poolConfig.MaxSize, _poolConfig.CanExpand);
    }

    void OnEnable()
    {
        this.MMEventStartListening<SpawnVFXEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<SpawnVFXEvent>();
    }

    public void OnMMEvent(SpawnVFXEvent e)
    {
        
    }
}
