using MoreMountains.Tools;
using UnityEngine;

public struct BuildNavMeshEvent
{
    public bool ForceBuild;

    public BuildNavMeshEvent(bool forceBuild)
    {
        ForceBuild = forceBuild;
    }

    public static BuildNavMeshEvent e;
    public static void Trigger(bool forceBuild)
    {
        e.ForceBuild = forceBuild;
        MMEventManager.TriggerEvent(e);
    }
}
