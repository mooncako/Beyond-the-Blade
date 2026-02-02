using MoreMountains.Tools;
using UnityEngine;

public struct EncounterStartEvent
{
    public int EnemyCount;
    public string EncounterID;
    public EncounterType EncounterType;

    public EncounterStartEvent(int enemyCount, string encounterID, EncounterType encounterType)
    {
        EnemyCount = enemyCount;
        EncounterID = encounterID;
        EncounterType = encounterType;
    }

    private static EncounterStartEvent e;

    public static void Trigger(int enemyCount, string encounterID, EncounterType encounterType)
    {
        e.EnemyCount = enemyCount;
        e.EncounterID = encounterID;
        e.EncounterType = encounterType;
        MMEventManager.TriggerEvent(e);
    }
}
