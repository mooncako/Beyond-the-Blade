using UnityEngine;

public class SheatheState : PlayerState
{
    
    public SheatheState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) {}

    public override void Enter()
    {
        base.Enter();
        Player.Animator.ResetTrigger("Muso");
        PlayerAnimationStateChangeEvent.Trigger(PlayerStateType.Sheathe);
    }

    public override void Update()
    {
        base.Update();
        if (Player.CurrentState == PlayerStateType.Attacking)
        {
            StateMachine.ChangeState(Player.States.AttackingState);
        }

        if (Player.CurrentState == PlayerStateType.Parrying)
        {
            StateMachine.ChangeState(Player.States.ParryingState);
        }
    }
    
    public override void Exit()
    {
        
    }
}
