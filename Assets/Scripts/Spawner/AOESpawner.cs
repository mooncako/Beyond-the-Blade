using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(ObjectPool))]
public class AOESpawner : MonoBehaviour,
    MMEventListener<SpawnOffloadAOEEvent>,
    MMEventListener<SpawnContinuousAOEEvent>
{
    [SerializeField, BoxGroup("References")] private ObjectPool _objectPool;
    [SerializeField, BoxGroup("References")] private GameObject _offLoadAOEPrefab;
    [SerializeField, BoxGroup("References")] private GameObject _continuousAOEPrefab;
    private HashSet<(GameObject, Skill)> _continuousAOEInstigatorPairs = new HashSet<(GameObject, Skill)>();

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
        if(_continuousAOEInstigatorPairs.Contains((e.Instigator, e.Skill)))
        {
            return;
        }
        _continuousAOEInstigatorPairs.Add((e.Instigator, e.Skill));
        ContinuousAOE aoe = _objectPool.Get(_continuousAOEPrefab).GetComponent<ContinuousAOE>();
        aoe.ObjectPool = _objectPool;
        aoe.transform.parent = e.Transform;
        aoe.transform.localPosition = Vector3.zero;
        aoe.transform.localRotation = Quaternion.identity;
        StartCoroutine(AOEReturnCO(e.SkillDuration, aoe, (e.Instigator, e.Skill)));
        aoe.UpdateAttack(e.Skill, e.Skill.DamageTickTime, e.Skill.DamageType, e.Instigator, e.EnemyMask);

    }

    private IEnumerator AOEReturnCO(float duration, AreaOfEffect aoe, (GameObject, Skill) instigatorPair)
    {
        yield return new WaitForSeconds(duration);
        _continuousAOEInstigatorPairs.Remove(instigatorPair);
        aoe.transform.parent = null;
        _objectPool.Return(aoe.gameObject);
    }
}
