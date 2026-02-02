using MoreMountains.Tools;
using UnityEngine;

public struct EncounterClearEvent
{
    public string EncounterID;
    public EncounterClearEvent(string encounterID)
    {
        EncounterID = encounterID;
    }

    public static EncounterClearEvent e;
    public static void Trigger(string encounterID)
    {
        e.EncounterID = encounterID;
        MMEventManager.TriggerEvent(e);
    }
}
