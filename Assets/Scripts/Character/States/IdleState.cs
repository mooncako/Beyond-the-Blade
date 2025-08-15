using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) {}

    public override void Enter()
    {
        base.Enter();
        Player.Movement.Stop();
        Player.Movement.SetMoveInput(Vector3.zero);
        Player.Movement.ResetSpeed();
        PlayerAnimationStateChangeEvent.Trigger(PlayerStateType.Idle);  
    }

    public override void Update()
    {
        base.Update();
        
        if (Player.InputProcessor.InputVector != Vector2.zero)
        {
            StateMachine.ChangeState(Player.States.MovingState);
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
