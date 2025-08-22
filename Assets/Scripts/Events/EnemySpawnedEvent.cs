using MoreMountains.Tools;
using UnityEngine;

public struct EnemySpawnedEvent
{
    public Health Health;

    public EnemySpawnedEvent(Health health)
    {
        Health = health;
    }

    private static EnemySpawnedEvent e;

    public static void Trigger(Health health)
    {
        e.Health = health;
        MMEventManager.TriggerEvent(e);
    }
}
