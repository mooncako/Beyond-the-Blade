using MoreMountains.Tools;
using UnityEngine;

public struct EnterNewLevelEvent
{
    public LevelType LevelType;
    public EnterNewLevelEvent(LevelType levelType)
    {
        LevelType = levelType;
    }

    public static EnterNewLevelEvent e;
    public static void Trigger(LevelType levelType)
    {
        e.LevelType = levelType;
        MMEventManager.TriggerEvent(e);
    }
}
