using UnityEngine;

public class SheatheState : PlayerState
{
    
    public SheatheState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) {}

    public override void Enter()
    {
        base.Enter();
        Player.Animator.ResetTrigger("Muso");
        Player.OnStateChanged.Invoke(STATE.Sheathe);
    }

    public override void Update()
    {
        base.Update();
        if (Player.Combat.IsAttacking)
        {
            StateMachine.ChangeState(Player.States.AttackingState);
        }

        if (Player.Combat.IsParrying)
        {
            StateMachine.ChangeState(Player.States.ParryingState);
        }
    }
    
    public override void Exit()
    {
        
    }
}
