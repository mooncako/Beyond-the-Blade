using MoreMountains.Tools;
using UnityEngine;

public struct PlayerOnDamageEvent
{
    public Health Health;
    public PlayerOnDamageEvent(Health health)
    {
        Health = health;
    }

    public static PlayerOnDamageEvent e;
    public static void Trigger(Health health)
    {
        e.Health = health;
        MMEventManager.TriggerEvent(e);
    }
}
