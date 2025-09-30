using MoreMountains.Tools;
using UnityEngine;

public struct GateOpenEvent
{
    public static GateOpenEvent e;
    public static void Trigger()
    {
        MMEventManager.TriggerEvent(e);
    }
}
