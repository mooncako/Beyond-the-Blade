using MoreMountains.Tools;

public struct NewProgressionDropEvent
{
    public ProgressionType ProgressionType;

    public NewProgressionDropEvent(ProgressionType progressionType)
    {
        ProgressionType = progressionType;
    }

    public static NewProgressionDropEvent e;

    public static void Trigger(ProgressionType progressionType)
    {
        e.ProgressionType = progressionType;
        MMEventManager.TriggerEvent(e);
    }
}
