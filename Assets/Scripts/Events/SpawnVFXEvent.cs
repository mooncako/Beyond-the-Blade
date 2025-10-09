using MoreMountains.Tools;
using UnityEngine;

public struct SpawnVFXEvent
{
    public Transform Owner;
    public string Id;
    public VFXInfo Info;

    public SpawnVFXEvent(Transform owner, string id, VFXInfo info)
    {
        Owner = owner;
        Id = id;
        Info = info;
    }

    public static SpawnVFXEvent e;
    public static void Trigger(Transform owner, string id, VFXInfo info)
    {
        e.Owner = owner;
        e.Id = id;
        e.Info = info;
        MMEventManager.TriggerEvent(e);
    }
}
