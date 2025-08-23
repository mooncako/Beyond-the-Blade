using MoreMountains.Tools;
using UnityEngine;

public struct LevelRandomizeCompleteEvent
{
    public EventStateType State;
    public Transform SpawnPoint;

    public LevelRandomizeCompleteEvent(EventStateType state, Transform spawnPoint)
    {
        State = state;
        SpawnPoint = spawnPoint;
    }

    private static LevelRandomizeCompleteEvent e;

    public static void Trigger(EventStateType state, Transform spawnPoint)
    {
        e.State = state;
        e.SpawnPoint = spawnPoint;
        MMEventManager.TriggerEvent(e);
    }
}
