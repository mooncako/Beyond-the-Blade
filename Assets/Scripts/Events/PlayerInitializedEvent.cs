using MoreMountains.Tools;
using UnityEngine;

public struct PlayerInitializedEvent
{
    public PlayerController Player;

    public PlayerInitializedEvent(PlayerController player)
    {
        Player = player;
    }

    public static PlayerInitializedEvent e;

    public static void Trigger(PlayerController player)
    {
        e.Player = player;
        MMEventManager.TriggerEvent(e);
    }
}
