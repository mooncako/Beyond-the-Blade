using MoreMountains.Tools;
using UnityEngine;

public struct RoomClearedEvent
{
    public static RoomClearedEvent e;

    public static void Trigger()
    {
        MMEventManager.TriggerEvent(e);
    }
}
