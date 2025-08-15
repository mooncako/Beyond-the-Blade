
using UnityEngine;
public class AttackingState : PlayerState
{
    public AttackingState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) {}

    public override void Enter()
    {
        base.Enter();
        PlayerAnimationStateChangeEvent.Trigger(PlayerStateType.Attacking);
    }

    public override void Update()
    {
        base.Update();
        
        if (Player.CurrentState != PlayerStateType.Attacking)
        {
            StateMachine.ChangeState(Player.States.IdleState);
        }
    }
}
