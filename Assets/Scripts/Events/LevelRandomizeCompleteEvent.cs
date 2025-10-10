using MoreMountains.Tools;
using UnityEngine;

public struct LevelRandomizeCompleteEvent
{
    public EventStateType State;
    public Transform SpawnPoint;
    public EnemySpawnPos[] EnemySpawnPositions;

    public LevelRandomizeCompleteEvent(EventStateType state, Transform spawnPoint, EnemySpawnPos[] enemySpawnPositions)
    {
        State = state;
        SpawnPoint = spawnPoint;
        EnemySpawnPositions = enemySpawnPositions;
    }

    private static LevelRandomizeCompleteEvent e;

    public static void Trigger(EventStateType state, Transform spawnPoint, EnemySpawnPos[] enemySpawnPositions)
    {
        e.State = state;
        e.SpawnPoint = spawnPoint;
        e.EnemySpawnPositions = enemySpawnPositions;
        MMEventManager.TriggerEvent(e);
    }
}
