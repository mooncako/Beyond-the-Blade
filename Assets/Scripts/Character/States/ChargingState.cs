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
        if (!Player.Combat.IsCharging)
        {
            StateMachine.ChangeState(Player.States.IdleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }   
}
