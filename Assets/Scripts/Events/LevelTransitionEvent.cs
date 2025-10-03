using MoreMountains.Tools;
using UnityEngine;

public struct LevelTransitionEvent
{
    public EventStateType Type;

    public LevelTransitionEvent(EventStateType type)
    {
        Type = type;
    }

    public static LevelTransitionEvent e;
    public static void Trigger(EventStateType type)
    {
        e.Type = type;
        MMEventManager.TriggerEvent(e);
    }
}
