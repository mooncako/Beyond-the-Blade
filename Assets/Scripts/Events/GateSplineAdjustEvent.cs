using MoreMountains.Tools;
using UnityEngine;

public struct GateSplineAdjustEvent
{
    public LevelSystem LevelSystem;

    public GateSplineAdjustEvent(LevelSystem levelSystem)
    {
        LevelSystem = levelSystem;
    }

    public static GateSplineAdjustEvent e;
    public static void Trigger(LevelSystem levelSystem)
    {
        e.LevelSystem = levelSystem;
        MMEventManager.TriggerEvent(e);
    }
}
