using MoreMountains.Tools;
using UnityEngine;

public struct PlayerOnHealthChangeEvent
{
    public Health Health;
    public PlayerOnHealthChangeEvent(Health health)
    {
        Health = health;
    }

    public static PlayerOnHealthChangeEvent e;
    public static void Trigger(Health health)
    {
        e.Health = health;
        MMEventManager.TriggerEvent(e);
    }
}
