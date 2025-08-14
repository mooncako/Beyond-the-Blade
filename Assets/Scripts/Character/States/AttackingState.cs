
using UnityEngine;
public class AttackingState : PlayerState
{
    public AttackingState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) {}

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        
        if (!Player.Combat.IsAttacking)
        {
            StateMachine.ChangeState(Player.States.IdleState);
        }
    }
}
