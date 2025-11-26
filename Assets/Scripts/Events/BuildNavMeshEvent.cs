using MoreMountains.Tools;
using UnityEngine;

public struct BuildNavMeshEvent
{
    public static BuildNavMeshEvent e;
    public static void Trigger()
    {
        MMEventManager.TriggerEvent(e);
    }
}
