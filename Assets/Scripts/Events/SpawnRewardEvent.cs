using MoreMountains.Tools;
using UnityEngine;

public struct SpawnRewardEvent
{
    public static SpawnRewardEvent e;
    public static void Trigger()
    {
        MMEventManager.TriggerEvent(e);
    }
}
