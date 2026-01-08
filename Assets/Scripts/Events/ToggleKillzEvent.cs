using MoreMountains.Tools;
using UnityEngine;

public struct ToggleKillzEvent
{
    public bool Toggle;
    public ToggleKillzEvent(bool toggle)
    {
        Toggle = toggle;
    }

    public static ToggleKillzEvent e;
    public static void Trigger(bool toggle)
    {
        e.Toggle = toggle;
        MMEventManager.TriggerEvent(e);
    }
}
