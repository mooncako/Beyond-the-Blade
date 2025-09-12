using MoreMountains.Tools;
using UnityEngine;

public struct NewAbilityEvent
{
    public EventStateType Type;

    public NewAbilityEvent(EventStateType type)
    {
        Type = type;
    }

    public static NewAbilityEvent e;

    public static void Trigger(EventStateType type)
    {
        e.Type = type;
        MMEventManager.TriggerEvent(e);
    }
}
