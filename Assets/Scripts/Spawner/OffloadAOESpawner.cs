using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(ObjectPool))]
public class OffloadAOESpawner : MonoBehaviour,
    MMEventListener<SpawnOffloadAOEEvent>
{
    [SerializeField, BoxGroup("References")] private ObjectPool _objectPool;
    [SerializeField, BoxGroup("References")] private GameObject _offLoadAOEPrefab;

    void OnValidate()
    {
        if (_objectPool == null) _objectPool = GetComponent<ObjectPool>();
    }

    void OnEnable()
    {
        this.MMEventStartListening<SpawnOffloadAOEEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<SpawnOffloadAOEEvent>();
    }

    public void OnMMEvent(SpawnOffloadAOEEvent e)
    {
        OffloadAOE aoe = _objectPool.Get(_offLoadAOEPrefab).GetComponent<OffloadAOE>();
        aoe.AssignData(e.Skill);
        aoe.transform.position = new Vector3(e.Transform.position.x, e.Transform.position.y+.05f, e.Transform.position.z);
        aoe.transform.rotation = e.Transform.rotation;
        aoe.Generate();
    }
}
