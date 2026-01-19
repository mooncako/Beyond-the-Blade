using MoreMountains.Tools;
using UnityEngine;

public struct PlayerRespawnTeleportEvent
{
    public PlayerController PlayerController;
    public PlayerRespawnTeleportEvent(PlayerController playerController)
    {
        PlayerController = playerController;
    }

    public static PlayerRespawnTeleportEvent e;
    public static void Trigger(PlayerController playerController)
    {
        e.PlayerController = playerController;
        MMEventManager.TriggerEvent(e);
    }
}
