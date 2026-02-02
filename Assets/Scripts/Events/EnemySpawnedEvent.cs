using MoreMountains.Tools;
using UnityEngine;

public struct EnemySpawnedEvent
{
    public Health Health;
    public string EncounterID;

    public EnemySpawnedEvent(Health health, string encounterID)
    {
        Health = health;
        EncounterID = encounterID;
    }

    private static EnemySpawnedEvent e;

    public static void Trigger(Health health, string encounterID)
    {
        e.Health = health;
        e.EncounterID = encounterID;
        MMEventManager.TriggerEvent(e);
    }
}
