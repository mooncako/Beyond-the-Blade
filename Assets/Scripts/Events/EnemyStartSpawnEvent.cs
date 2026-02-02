using MoreMountains.Tools;
using UnityEngine;

public struct EnemyStartSpawnEvent
{
    public EnemySpawnPos[] EnemySpawnPositions;
    public EnemySpawnerType EnemySpawnerType;
    public EnemySpawnModeType SpawnPositionType;
    public EnemyPool EnemyPool;
    public bool IsPrecisePos;
    public int EnemyCount;
    public string EncounterID;

    public EnemyStartSpawnEvent(EnemySpawnPos[] enemySpawnPositions, EnemySpawnerType enemySpawnerType, EnemySpawnModeType spawnPositionType, EnemyPool enemyPool, bool isPrecisePos, int enemyCount, string encounterID)
    {
        EnemySpawnPositions = enemySpawnPositions;
        EnemySpawnerType = enemySpawnerType;
        SpawnPositionType = spawnPositionType;
        EnemyPool = enemyPool;
        IsPrecisePos = isPrecisePos;
        EnemyCount = enemyCount;
        EncounterID = encounterID;
    }

    public static EnemyStartSpawnEvent e;
    public static void Trigger(EnemySpawnPos[] enemySpawnPositions, EnemySpawnerType enemySpawnerType, EnemySpawnModeType spawnPositionType, EnemyPool enemyPool, bool isPrecisePos, int enemyCount, string encounterID)
    {
        e.EnemySpawnPositions = enemySpawnPositions;
        e.EnemySpawnerType = enemySpawnerType;
        e.SpawnPositionType = spawnPositionType;
        e.EnemyPool = enemyPool;
        e.IsPrecisePos = isPrecisePos;
        e.EnemyCount = enemyCount;
        e.EncounterID = encounterID;
        MMEventManager.TriggerEvent(e);
    }
}
