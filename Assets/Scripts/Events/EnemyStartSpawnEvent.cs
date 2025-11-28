using MoreMountains.Tools;
using UnityEngine;

public struct EnemyStartSpawnEvent
{
    public EnemySpawnPos[] EnemySpawnPositions;

    public EnemyStartSpawnEvent(EnemySpawnPos[] enemySpawnPositions)
    {
        EnemySpawnPositions = enemySpawnPositions;
    }

    public static EnemyStartSpawnEvent e;
    public static void Trigger(EnemySpawnPos[] enemySpawnPositions)
    {
        e.EnemySpawnPositions = enemySpawnPositions;
        MMEventManager.TriggerEvent(e);
    }
}
