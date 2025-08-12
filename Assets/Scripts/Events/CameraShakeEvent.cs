using MoreMountains.Tools;

public struct CameraShakeEvent
{
    public CameraShakeSettings Setting;

    public CameraShakeEvent(CameraShakeSettings setting)
    {
        Setting = setting;
    }

    private static CameraShakeEvent e;

    public static void Trigger(CameraShakeSettings setting)
    {
        e.Setting = setting;
        MMEventManager.TriggerEvent(e);
    }
}
