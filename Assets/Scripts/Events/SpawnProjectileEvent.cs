using MoreMountains.Tools;
using UnityEngine;

public struct SpawnProjectileEvent
{
    public string Id;
    public Vector3 Position;
    public Vector3 Direction;
    public GameObject Owner;

    public SpawnProjectileEvent(string id, Vector3 position, Vector3 direction, GameObject owner)
    {
        Id = id;
        Position = position;
        Direction = direction;
        Owner = owner;
    }

    public static SpawnProjectileEvent e;
    public static void Trigger(string id, Vector3 position, Vector3 direction, GameObject owner)
    {
        e.Id = id;
        e.Position = position;
        e.Direction = direction;
        e.Owner = owner;
        MMEventManager.TriggerEvent(e);
    }
}
