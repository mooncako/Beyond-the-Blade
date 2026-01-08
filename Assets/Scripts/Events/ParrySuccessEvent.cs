using MoreMountains.Tools;
using UnityEngine;

public struct ParrySuccessEvent
{
    public static ParrySuccessEvent e;
    public static void Trigger()
    {
        MMEventManager.TriggerEvent(e);
    }
}
