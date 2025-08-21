using MoreMountains.Tools;
using UnityEngine;

public struct EncounterStartEvent
{
    public float Timer;

    public EncounterStartEvent(float timer)
    {
        Timer = timer;
    }

    private static EncounterStartEvent e;

    public static void Trigger(float timer)
    {
        e.Timer = timer;
        MMEventManager.TriggerEvent(e);
    }
}
