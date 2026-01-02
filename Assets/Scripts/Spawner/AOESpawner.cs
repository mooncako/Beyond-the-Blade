using System.Collections;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(ObjectPool))]
public class AOESpawner : MonoBehaviour,
    MMEventListener<SpawnOffloadAOEEvent>,
    MMEventListener<SpawnContinuousAOEEvent>
{
    [SerializeField, BoxGroup("References")] private ObjectPool _objectPool;
    [SerializeField, BoxGroup("References")] private GameObject _offLoadAOEPrefab;
    [SerializeField, BoxGroup("References")] private GameObject _continuousAOEPrefab;

    void OnValidate()
    {
        if (_objectPool == null) _objectPool = GetComponent<ObjectPool>();
    }

    void OnEnable()
    {
        this.MMEventStartListening<SpawnOffloadAOEEvent>();
        this.MMEventStartListening<SpawnContinuousAOEEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<SpawnOffloadAOEEvent>();
        this.MMEventStopListening<SpawnContinuousAOEEvent>();
    }

    public void OnMMEvent(SpawnOffloadAOEEvent e)
    {
        OffloadAOE aoe = _objectPool.Get(_offLoadAOEPrefab).GetComponent<OffloadAOE>();
        aoe.OnAOETriggered.AddListener(() => _objectPool.Return(aoe.gameObject));
        aoe.AssignData(e.Skill);
        aoe.transform.position = new Vector3(e.Transform.position.x, e.Transform.position.y + .05f, e.Transform.position.z);
        aoe.transform.rotation = e.Transform.rotation;
        aoe.ObjectPool = _objectPool;
        aoe.Generate();
    }

    public void OnMMEvent(SpawnContinuousAOEEvent e)
    {
        ContinuousAOE aoe = _objectPool.Get(_continuousAOEPrefab).GetComponent<ContinuousAOE>();
        aoe.ObjectPool = _objectPool;
        StartCoroutine(AOEReturnCO(e.SkillDuration, aoe));
        aoe.UpdateAttack(e.Skill, e.DamageTickTime, e.DamageType, e.Instigator);
    }

    private IEnumerator AOEReturnCO(float duration, AreaOfEffect aoe)
    {
        yield return new WaitForSeconds(duration);
        _objectPool.Return(aoe.gameObject);
    }
}
