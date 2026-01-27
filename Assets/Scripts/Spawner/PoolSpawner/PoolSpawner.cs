using Sirenix.OdinInspector;
using UnityEngine;

public class PoolSpawner : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] protected EnemyPool _enemyPool;
    [field: SerializeField, BoxGroup("References"), InlineButton("FindPositions", "Find")] protected EnemySpawnPos[] _enemySpawnPositions;
    [SerializeField, BoxGroup("References")] protected CollisionTrigger _spawnTrigger;
    [SerializeField, BoxGroup("Settings")] protected SpawnType _spawnPositionType;
    [SerializeField, BoxGroup("Settings")] protected bool _isPrecisePos = false;
    [SerializeField, BoxGroup("Settings")] protected int _minEnemyCountPerWave = 3;
    [SerializeField, BoxGroup("Settings")] protected int _maxEnemyCountPerWave = 5;

    protected virtual void OnValidate()
    {
        if (_spawnTrigger == null) _spawnTrigger = GetComponentInChildren<CollisionTrigger>();
        if (_enemyPool == null) _enemyPool = GetComponentInChildren<EnemyPool>();
    }

    private void FindPositions()
    {
        _enemySpawnPositions = GetComponentsInChildren<EnemySpawnPos>();
    }

    protected virtual void SpawnEnemies()
    {
        EnemyStartSpawnEvent.Trigger(_enemySpawnPositions, EnemySpawnerType.Train, _spawnPositionType, _enemyPool, _isPrecisePos, _minEnemyCountPerWave, _maxEnemyCountPerWave);
    }
}
