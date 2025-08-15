using UnityEngine;

public class MovingState : PlayerState
{
    public MovingState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) {}

    public override void Enter()
    {
        base.Enter();
        PlayerAnimationStateChangeEvent.Trigger(PlayerStateType.Moving);
    }

    public override void Update()
    {
        base.Update();
        
        if (Player.InputProcessor.InputVector == Vector2.zero)
        {
            StateMachine.ChangeState(Player.States.IdleState);
        }
        
        if (Player.CurrentState == PlayerStateType.Attacking)
        {
            StateMachine.ChangeState(Player.States.AttackingState);
        }

        if (Player.CurrentState == PlayerStateType.Parrying)
        {
            StateMachine.ChangeState(Player.States.ParryingState);
        }
    }
}
