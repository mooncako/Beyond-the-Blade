using MoreMountains.Tools;
using UnityEngine;

public struct PlayerAnimationStateChangeEvent
{
    public PlayerStateType State;
    public PlayerAnimationStateChangeEvent(PlayerStateType state)
    {
        State = state;
    }

    private static PlayerAnimationStateChangeEvent e;

    public static void Trigger(PlayerStateType state)
    {
        e.State = state;
        MMEventManager.TriggerEvent(e);
    }
}
