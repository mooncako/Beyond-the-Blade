using MoreMountains.Tools;
using UnityEngine;

public struct LevelSetupCompleteEvent
{
    public Vector3 SpawnPosition;

    public LevelSetupCompleteEvent(Vector3 spawnPosition)
    {
        SpawnPosition = spawnPosition;
    }

    public static LevelSetupCompleteEvent e;
    public static void Trigger(Vector3 spawnPosition)
    {
        e.SpawnPosition = spawnPosition;
        MMEventManager.TriggerEvent(e);
    }
}
