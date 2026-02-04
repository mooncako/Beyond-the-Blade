using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(EncounterSetting))]
public class PoolSpawner : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] protected EnemyPool _enemyPool;
    [SerializeField, BoxGroup("References")] protected EncounterSetting _encounterSetting;
    [field: SerializeField, BoxGroup("References"), InlineButton("FindPositions", "Find")] protected EnemySpawnPos[] _enemySpawnPositions;
    [SerializeField, BoxGroup("References")] protected CollisionTrigger _spawnTrigger;
    [SerializeField, BoxGroup("Settings")] protected EnemySpawnModeType _spawnPositionType;
    [SerializeField, BoxGroup("Settings")] protected EnemySpawnTriggerType _spawnTriggerType = EnemySpawnTriggerType.Collision;
    [SerializeField, BoxGroup("Settings")] protected bool _isPrecisePos = false;
    [SerializeField, BoxGroup("Settings")] protected int _minEnemyCountPerWave = 3;
    [SerializeField, BoxGroup("Settings")] protected int _maxEnemyCountPerWave = 5;
    [SerializeField, BoxGroup("Settings")] protected EnemySpawnerType _enemySpawnerType;

    [HideInInspector] public UnityEvent<Collider> OnSpawnCompleted;

    protected virtual void OnValidate()
    {
        if (_spawnTrigger == null) _spawnTrigger = GetComponentInChildren<CollisionTrigger>();
        if (_enemyPool == null) _enemyPool = GetComponentInChildren<EnemyPool>();
        if (_encounterSetting == null) _encounterSetting = GetComponent<EncounterSetting>();
    }

    private void FindPositions()
    {
        _enemySpawnPositions = GetComponentsInChildren<EnemySpawnPos>();
    }

    protected virtual void SpawnEnemies()
    {
        int enemyCount = Random.Range(_minEnemyCountPerWave, _maxEnemyCountPerWave + 1);
        _encounterSetting.StartEncounter(EncounterType.Combat, enemyCount);
        EnemyStartSpawnEvent.Trigger(_enemySpawnPositions, _enemySpawnerType, _spawnPositionType, _enemyPool, _isPrecisePos, enemyCount, _encounterSetting.EncounterID);
    }
}
