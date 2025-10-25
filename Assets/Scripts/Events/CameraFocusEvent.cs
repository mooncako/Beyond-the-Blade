using MoreMountains.Tools;
using UnityEngine;

public struct CameraFocusEvent
{
    public CameraLensSetting Setting;

    public CameraFocusEvent(CameraLensSetting setting)
    {
        Setting = setting;
    }

    public static CameraFocusEvent e;

    public static void Trigger(CameraLensSetting setting)
    {
        e.Setting = setting;
        MMEventManager.TriggerEvent(e);
    }
}
