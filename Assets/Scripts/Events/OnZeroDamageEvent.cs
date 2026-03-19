using MoreMountains.Tools;
using UnityEngine;

public struct OnZeroDamageEvent
{
    public GameObject Source;

    public OnZeroDamageEvent(GameObject source)
    {
        Source = source;
    }

    public static OnZeroDamageEvent e;
    public static void Trigger(GameObject source)
    {
        e.Source = source;
        MMEventManager.TriggerEvent(e);
    }
}
