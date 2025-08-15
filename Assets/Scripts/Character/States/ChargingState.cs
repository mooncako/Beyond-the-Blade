using UnityEngine;
using UnityEngine.InputSystem;

public class ChargingState : PlayerState
{
    public ChargingState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        PlayerAnimationStateChangeEvent.Trigger(PlayerStateType.Charging);
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }   
}
