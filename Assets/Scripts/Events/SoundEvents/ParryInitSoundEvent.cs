using MoreMountains.Tools;
using UnityEngine;

public struct ParryInitSoundEvent
{
    public static ParryInitSoundEvent e;
    public static void Trigger()
    {
        MMEventManager.TriggerEvent(e);
    }
}
