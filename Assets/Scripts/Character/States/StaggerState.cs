using UnityEngine;
using UnityEngine.Animations.Rigging;

public class StaggerState : PlayerState
{
    public StaggerState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) {}

    public override void Enter()
    {
        base.Enter();
        PlayerAnimationStateChangeEvent.Trigger(PlayerStateType.Staggered);

    }

    public override void Update()
    {
        if (Player.CurrentState != PlayerStateType.Staggered)
        {
            StateMachine.ChangeState(Player.States.IdleState);
        }
    }
    public override void Exit()
    {
        base.Exit();
    }
}