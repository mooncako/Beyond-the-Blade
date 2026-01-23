using MoreMountains.Tools;
using UnityEngine;

public struct EnemyStartSpawnEvent
{
    public EnemySpawnPos[] EnemySpawnPositions;
    public EnemySpawnerType EnemySpawnerType;
    public EnemyPool EnemyPool;
    public int MinEnemyCountPerWave;
    public int MaxEnemyCountPerWave;

    public EnemyStartSpawnEvent(EnemySpawnPos[] enemySpawnPositions, EnemySpawnerType enemySpawnerType, EnemyPool enemyPool, int minEnemyCountPerWave, int maxEnemyCountPerWave)
    {
        EnemySpawnPositions = enemySpawnPositions;
        EnemySpawnerType = enemySpawnerType;
        EnemyPool = enemyPool;
        MinEnemyCountPerWave = minEnemyCountPerWave;
        MaxEnemyCountPerWave = maxEnemyCountPerWave;
    }

    public static EnemyStartSpawnEvent e;
    public static void Trigger(EnemySpawnPos[] enemySpawnPositions, EnemySpawnerType enemySpawnerType, EnemyPool enemyPool, int minEnemyCountPerWave, int maxEnemyCountPerWave)
    {
        e.EnemySpawnPositions = enemySpawnPositions;
        e.EnemySpawnerType = enemySpawnerType;
        e.EnemyPool = enemyPool;
        e.MinEnemyCountPerWave = minEnemyCountPerWave;
        e.MaxEnemyCountPerWave = maxEnemyCountPerWave;
        MMEventManager.TriggerEvent(e);
    }
}
