using MoreMountains.Tools;
using UnityEngine;

public struct GateOpenEvent
{
    public LevelSystem Level;

    public GateOpenEvent(LevelSystem level)
    {
        Level = level;
    }


    public static GateOpenEvent e;
    public static void Trigger(LevelSystem level)
    {
        e.Level = level;
        MMEventManager.TriggerEvent(e);
    }
}
