using MoreMountains.Tools;
using UnityEngine;

public struct PickupUsedEvent
{

    public static PickupUsedEvent e;

    public static void Trigger()
    {
        MMEventManager.TriggerEvent(e);
    }
}
