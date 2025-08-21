using MoreMountains.Tools;
using UnityEngine;

public struct EnemyClearedEvent
{
    private static EnemyClearedEvent e;

    public static void Trigger()
    {
        MMEventManager.TriggerEvent(e);
    }
}
