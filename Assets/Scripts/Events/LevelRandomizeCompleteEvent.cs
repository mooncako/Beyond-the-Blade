using MoreMountains.Tools;
using UnityEngine;

public struct LevelRandomizeCompleteEvent
{
    public EventStateType State;
    public Transform SpawnPoint;
    public LevelSystem Level;

    public LevelRandomizeCompleteEvent(EventStateType state, Transform spawnPoint, LevelSystem level)
    {
        State = state;
        SpawnPoint = spawnPoint;
        Level = level;
    }

    private static LevelRandomizeCompleteEvent e;

    public static void Trigger(EventStateType state, Transform spawnPoint, LevelSystem level)
    {
        e.State = state;
        e.SpawnPoint = spawnPoint;
        e.Level = level;
        MMEventManager.TriggerEvent(e);
    }
}
