using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class ContinuousAOE : AreaOfEffect
{
    [SerializeField, BoxGroup("Settings")] private float _damageTickTime = .5f;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private DamageType _damageType = DamageType.Regular;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private GameObject _instigator;
    [SerializeField] private HashSet<GameObject> _currentAffectedEnemies = new HashSet<GameObject>();

#if UNITY_EDITOR
    [ShowInInspector, BoxGroup("Debug"), ReadOnly] private List<GameObject> _currentAffectedEnemyList => _currentAffectedEnemies.ToList();
#endif

    protected override void OnTriggerStay(Collider other)
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
                        StartCoroutine(DamageCO(gameObjects[i].GetComponent<Health>()));
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
            StopCoroutine(DamageCO(other.GetComponent<Health>()));
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    public void UpdateAttack(Skill skill, float damageTickTime, DamageType damageType, GameObject instigator)
    {
        AssignData(skill);
        _damageTickTime = damageTickTime;
        _damageType = damageType;
        _instigator = instigator;
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
