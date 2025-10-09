using MoreMountains.Tools;
using UnityEngine;

public struct SpawnVFXEvent
{
    public string Id;
    public VFXInfo Info;

    public SpawnVFXEvent(string id, VFXInfo info)
    {
        Id = id;
        Info = info;
    }

    public static SpawnVFXEvent e;
    public static void Trigger(string id, VFXInfo info)
    {
        e.Id = id;
        e.Info = info;
        MMEventManager.TriggerEvent(e);
    }
}
