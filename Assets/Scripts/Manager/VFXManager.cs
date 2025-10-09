using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEditor.Embree;
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
        if (_prefabDatabase.VFXDatabase.ContainsKey(e.Id))
        {
            GameObject go = _pool.Get(_prefabDatabase.VFXDatabase[e.Id]);
            go.transform.SetParent(e.Owner, false);
            go.transform.position = e.Owner.position;
            go.transform.localPosition = new Vector3(go.transform.localPosition.x + e.Info.Pos.x, go.transform.localPosition.y + e.Info.Pos.y, go.transform.localPosition.z + e.Info.Pos.z);
            go.transform.rotation = e.Info.Rot;
            go.transform.localScale = e.Info.Scale;
            go.GetComponent<VisualEffect>().Play();
            Tween.Delay(.1f).OnComplete(() => go.transform.SetParent(null));
            go.GetComponent<VFXFinishedEventHandler>().OnSpawnFinished.AddListener(() => ReturnVFX(go));
        }
        
    }

    private void ReturnVFX(GameObject go)
    {
        _pool.Return(go);
        go.GetComponent<VFXFinishedEventHandler>().OnSpawnFinished.RemoveAllListeners();
    }
}
