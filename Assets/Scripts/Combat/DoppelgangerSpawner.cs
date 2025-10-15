using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(ObjectPool))]
public class DoppelgangerSpawner : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private Doppelganger _doppelgangerPrefab;
    [SerializeField, BoxGroup("References")] private ObjectPool _pool;
    [SerializeField, BoxGroup("Settings")] private PoolConfig _poolConfig;

    void Start()
    {
        List<GameObject> pooledObjects = new List<GameObject>
        {
            _doppelgangerPrefab.gameObject
        };
        _pool.InitializeRuntimePool(pooledObjects, _poolConfig.InitialSize, _poolConfig.MaxSize, _poolConfig.CanExpand);
    }

    void OnValidate()
    {
        if (_pool == null) _pool = GetComponent<ObjectPool>();
    }

    public void Spawn(Controller controller)
    {
        if (!controller.IsAttacking()) return;
        Doppelganger doppelganger = _pool.Get(_doppelgangerPrefab.gameObject).GetComponent<Doppelganger>();
        doppelganger.transform.position = controller.transform.position;
        doppelganger.transform.rotation = controller.transform.rotation;
        doppelganger.AssignData(controller);
        doppelganger.PlaySkill();
        doppelganger.OnDoppelgangerEnd.AddListener(OnDoppelgangerEnd);
    }

    private void OnDoppelgangerEnd(GameObject doppelganger)
    {
        _pool.Return(doppelganger);
    }
}
