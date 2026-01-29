using MoreMountains.Tools;
using UnityEngine;

public struct EncounterStartEvent
{
    public float Timer;
    public string EncounterID;
    public EncounterType EncounterType;

    public EncounterStartEvent(float timer, string encounterID, EncounterType encounterType)
    {
        Timer = timer;
        EncounterID = encounterID;
        EncounterType = encounterType;
    }

    private static EncounterStartEvent e;

    public static void Trigger(float timer, string encounterID, EncounterType encounterType)
    {
        e.Timer = timer;
        e.EncounterID = encounterID;
        e.EncounterType = encounterType;
        MMEventManager.TriggerEvent(e);
    }
}
