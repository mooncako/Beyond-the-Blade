using MoreMountains.Tools;
using UnityEngine;

public struct EnemyStartSpawnEvent
{
    public EnemySpawnPos[] EnemySpawnPositions;
    public EnemySpawnerType EnemySpawnerType;

    public EnemyStartSpawnEvent(EnemySpawnPos[] enemySpawnPositions, EnemySpawnerType enemySpawnerType)
    {
        EnemySpawnPositions = enemySpawnPositions;
        EnemySpawnerType = enemySpawnerType;
    }

    public static EnemyStartSpawnEvent e;
    public static void Trigger(EnemySpawnPos[] enemySpawnPositions, EnemySpawnerType enemySpawnerType)
    {
        e.EnemySpawnPositions = enemySpawnPositions;
        e.EnemySpawnerType = enemySpawnerType;
        MMEventManager.TriggerEvent(e);
    }
}
