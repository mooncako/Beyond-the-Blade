using MoreMountains.Tools;
using UnityEngine;

public struct ReturnEnemyEvent
{
    public GameObject Go;
    public ReturnEnemyEvent(GameObject go)
    {
        Go = go;
    }

    public static ReturnEnemyEvent e;
    public static void Trigger(GameObject go)
    {
        e.Go = go;
        MMEventManager.TriggerEvent(e);
    }
}
