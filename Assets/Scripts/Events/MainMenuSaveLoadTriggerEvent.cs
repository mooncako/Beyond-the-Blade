using MoreMountains.Tools;
using UnityEngine;

public struct MainMenuSaveLoadTriggerEvent
{
    public SaveLoadEventType EventType;

    public MainMenuSaveLoadTriggerEvent(SaveLoadEventType eventType)
    {
        EventType = eventType;
    }

    public static MainMenuSaveLoadTriggerEvent e;

    public static void Trigger(SaveLoadEventType eventType)
    {
        e.EventType = eventType;
        MMEventManager.TriggerEvent(e);
    }
}
