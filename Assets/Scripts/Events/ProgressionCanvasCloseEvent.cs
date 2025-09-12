using MoreMountains.Tools;
using UnityEngine;

public struct ProgressionCanvasCloseEvent
{

    public static ProgressionCanvasCloseEvent e;

    public static void Trigger()
    {
        MMEventManager.TriggerEvent(e);
    }
}
