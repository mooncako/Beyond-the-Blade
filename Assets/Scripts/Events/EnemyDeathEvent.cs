using MoreMountains.Tools;
using UnityEngine;

public struct EnemyDeathEvent
{
    public DamageInfo Info;
    public EnemyDeathEvent(DamageInfo info)
    {
        Info = info;
    }

    public static EnemyDeathEvent e;

    public static void Trigger(DamageInfo info)
    {
        e.Info = info;
        MMEventManager.TriggerEvent(e);
    }
}