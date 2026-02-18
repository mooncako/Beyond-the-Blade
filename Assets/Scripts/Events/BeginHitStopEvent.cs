using MoreMountains.Tools;
using UnityEngine;

public struct BeginHitStopEvent
{
    public static BeginHitStopEvent e;
    public static void Trigger()
    {
        MMEventManager.TriggerEvent(e);
    }
}
