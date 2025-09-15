using MoreMountains.Tools;
using UnityEngine;

public struct NewItemSelectionEvent
{
    public EventStateType Type;

    public NewItemSelectionEvent(EventStateType type)
    {
        Type = type;
    }

    public static NewItemSelectionEvent e;
    public static void Trigger(EventStateType type)
    {
        e.Type = type;

        MMEventManager.TriggerEvent(e);
    }
}
