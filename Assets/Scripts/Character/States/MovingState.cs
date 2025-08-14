using UnityEngine;

public class MovingState : PlayerState
{
    public MovingState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) {}

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        
        if (Player.InputProcessor.InputVector == Vector2.zero)
        {
            StateMachine.ChangeState(Player.States.IdleState);
        }
        
        if (Player.Combat.IsAttacking)
        {
            StateMachine.ChangeState(Player.States.AttackingState);
        }

        if (Player.Combat.IsParrying)
        {
            StateMachine.ChangeState(Player.States.ParryingState);
        }
    }
}
