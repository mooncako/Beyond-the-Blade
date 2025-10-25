using MoreMountains.Tools;
using UnityEngine;

public struct CameraRotateEvent
{
    public static CameraRotateEvent e;
    public static void Trigger()
    {
        MMEventManager.TriggerEvent(e);
    }
}
