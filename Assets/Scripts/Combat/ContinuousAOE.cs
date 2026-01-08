using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinuousAOE : AreaOfEffect
{
    [SerializeField, BoxGroup("Settings")] private float _damageTickTime = .5f;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private DamageType _damageType = DamageType.Regular;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private GameObject _instigator;
    [SerializeField] private HashSet<GameObject> _currentAffectedEnemies = new HashSet<GameObject>();
    [HideInInspector] public ObjectPool ObjectPool;
    private Dictionary<GameObject, Coroutine> _damageCoroutines = new Dictionary<GameObject, Coroutine>();

#if UNITY_EDITOR
    [ShowInInspector, BoxGroup("Debug"), ReadOnly] private List<GameObject> _currentAffectedEnemyList => _currentAffectedEnemies.ToList();
#endif

    protected void OnTriggerEnter(Collider other)
    {
        if(_enabled)
        {
            if(_aoeApplier.GetDamagedEntities(_range.AreaType, transform.position, _enemyMask).Count > 0)
            {
                List<GameObject> gameObjects = _aoeApplier.GetDamagedEntities(_range.AreaType, transform.position, _enemyMask);
                for (int i = 0; i < gameObjects.Count; i++)
                {
                    if(!_currentAffectedEnemies.Contains(gameObjects[i]))
                    {
                        _currentAffectedEnemies.Add(gameObjects[i]);
                        _damageCoroutines.Add(gameObjects[i],StartCoroutine(DamageCO(gameObjects[i].GetComponent<Health>())));
                    }
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(_currentAffectedEnemies.Contains(other.gameObject))
        {
            _currentAffectedEnemies.Remove(other.gameObject);
            StopCoroutine(_damageCoroutines[other.gameObject]);
            _damageCoroutines.Remove(other.gameObject);
        }
    }

    protected override void OnDisable()
    {
        StopAllCoroutines();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        StopAllCoroutines();
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if(ObjectPool != null)
        {
            ObjectPool.Return(gameObject);
        }
    }

    public void UpdateAttack(Skill skill, float damageTickTime, DamageType damageType, GameObject instigator, LayerMask enemyMask)
    {
        AssignData(skill);
        _damageTickTime = damageTickTime;
        _damageType = damageType;
        _instigator = instigator;
        _enemyMask = enemyMask;
        Generate();
    }

    private IEnumerator DamageCO(Health health)
    {
        while(true)
        {
            yield return new WaitForSeconds(_damageTickTime);
            health.Damage(new DamageInfo(_damage, health.gameObject, health, _instigator, _damageType));
        }
    }
}
