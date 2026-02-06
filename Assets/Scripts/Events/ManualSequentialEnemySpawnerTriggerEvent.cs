using MoreMountains.Tools;
using UnityEngine;

public struct ManualSequentialEnemySpawnerTriggerEvent
{
    public string EncounterID;

    public ManualSequentialEnemySpawnerTriggerEvent(string encounterID)
    {
        EncounterID = encounterID;
    }

    public static ManualSequentialEnemySpawnerTriggerEvent e;
    public static void Trigger(string encounterID)
    {
        e.EncounterID = encounterID;
        MMEventManager.TriggerEvent(e);
    }
}
