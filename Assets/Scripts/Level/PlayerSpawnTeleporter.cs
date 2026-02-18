using MoreMountains.Tools;
using UnityEngine;

public class PlayerSpawnTeleporter : MonoBehaviour,
    MMEventListener<PlayerRespawnTeleportEvent>
{

    void OnEnable()
    {
        this.MMEventStartListening<PlayerRespawnTeleportEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<PlayerRespawnTeleportEvent>();
    }

    public void OnMMEvent(PlayerRespawnTeleportEvent e)
    {
        e.PlayerController.Movement.Teleport(transform);
    }
}
