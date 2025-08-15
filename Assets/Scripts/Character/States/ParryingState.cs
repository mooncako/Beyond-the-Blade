using UnityEngine;

public class ParryingState : PlayerState
{
    
    public ParryingState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) {}

    public override void Enter()
    {
        base.Enter();
        PlayerAnimationStateChangeEvent.Trigger(PlayerStateType.Parrying);

        Player.Movement.Stop();
        Player.Movement.SetMoveInput(Vector3.zero);
        Player.Movement.ResetSpeed();  
    }

    public override void Update()
    {
        if (Player.Combat.MusoReady)
        {
            StateMachine.ChangeState(Player.States.MusoState);
        }
        
        if (Player.CurrentState != PlayerStateType.Parrying)
        {
            StateMachine.ChangeState(Player.States.IdleState);
        }
    }
    public override void Exit()
    {
        base.Exit();

    }
}
