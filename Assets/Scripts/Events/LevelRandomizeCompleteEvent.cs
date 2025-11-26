using MoreMountains.Tools;
using UnityEngine;

public struct LevelRandomizeCompleteEvent
{
    public EventStateType State;
    public Transform SpawnPoint;
    public LevelSystem Level;
    public EnemySpawnPos[] EnemySpawnPositions;

    public LevelRandomizeCompleteEvent(EventStateType state, Transform spawnPoint, EnemySpawnPos[] enemySpawnPositions, LevelSystem level)
    {
        State = state;
        SpawnPoint = spawnPoint;
        EnemySpawnPositions = enemySpawnPositions;
        Level = level;
    }

    private static LevelRandomizeCompleteEvent e;

    public static void Trigger(EventStateType state, Transform spawnPoint, EnemySpawnPos[] enemySpawnPositions, LevelSystem level)
    {
        e.State = state;
        e.SpawnPoint = spawnPoint;
        e.EnemySpawnPositions = enemySpawnPositions;
        e.Level = level;
        MMEventManager.TriggerEvent(e);
    }
}
