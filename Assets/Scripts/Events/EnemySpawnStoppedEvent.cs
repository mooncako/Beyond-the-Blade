using MoreMountains.Tools;
using UnityEngine;

public struct EnemySpawnStoppedEvent
{
    public static EnemySpawnStoppedEvent e;

    public static void Trigger()
    {
        MMEventManager.TriggerEvent(e);
    }
}
