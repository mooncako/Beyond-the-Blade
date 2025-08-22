using MoreMountains.Tools;
using UnityEngine;

public struct LevelRandomizeCompleteEvent
{
    public EventStateType State;
    public Vector3 SpawnPosition;

    public LevelRandomizeCompleteEvent(EventStateType state, Vector3 spawnPosition)
    {
        State = state;
        SpawnPosition = spawnPosition;
    }

    private static LevelRandomizeCompleteEvent e;

    public static void Trigger(EventStateType state, Vector3 spawnPosition)
    {
        e.State = state;
        e.SpawnPosition = spawnPosition;
        MMEventManager.TriggerEvent(e);
    }
}
