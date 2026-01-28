using MoreMountains.Tools;
using UnityEngine;

public struct EnemyStartSpawnEvent
{
    public EnemySpawnPos[] EnemySpawnPositions;
    public EnemySpawnerType EnemySpawnerType;
    public EnemySpawnModeType SpawnPositionType;
    public EnemyPool EnemyPool;
    public bool IsPrecisePos;
    public int MinEnemyCountPerWave;
    public int MaxEnemyCountPerWave;

    public EnemyStartSpawnEvent(EnemySpawnPos[] enemySpawnPositions, EnemySpawnerType enemySpawnerType, EnemySpawnModeType spawnPositionType, EnemyPool enemyPool, bool isPrecisePos, int minEnemyCountPerWave, int maxEnemyCountPerWave)
    {
        EnemySpawnPositions = enemySpawnPositions;
        EnemySpawnerType = enemySpawnerType;
        SpawnPositionType = spawnPositionType;
        EnemyPool = enemyPool;
        IsPrecisePos = isPrecisePos;
        MinEnemyCountPerWave = minEnemyCountPerWave;
        MaxEnemyCountPerWave = maxEnemyCountPerWave;
    }

    public static EnemyStartSpawnEvent e;
    public static void Trigger(EnemySpawnPos[] enemySpawnPositions, EnemySpawnerType enemySpawnerType, EnemySpawnModeType spawnPositionType, EnemyPool enemyPool, bool isPrecisePos, int minEnemyCountPerWave, int maxEnemyCountPerWave)
    {
        e.EnemySpawnPositions = enemySpawnPositions;
        e.EnemySpawnerType = enemySpawnerType;
        e.SpawnPositionType = spawnPositionType;
        e.EnemyPool = enemyPool;
        e.IsPrecisePos = isPrecisePos;
        e.MinEnemyCountPerWave = minEnemyCountPerWave;
        e.MaxEnemyCountPerWave = maxEnemyCountPerWave;
        MMEventManager.TriggerEvent(e);
    }
}
