using UnityEngine;
using UnityEngine.Animations.Rigging;

public class StaggerState : PlayerState
{
    public StaggerState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) {}

    public override void Enter()
    {
        base.Enter();
        Player.OnStateChanged.Invoke(STATE.Staggered);
        if (Player.Combat.IsCharging == true)
        {
            Player.Combat.CancelCharge();
            EventHub.Instance.OnChargeEnded.Invoke();
        }
    }

    public override void Update()
    {
        if (!Player.Combat.IsStaggered)
        {
            StateMachine.ChangeState(Player.States.IdleState);
        }
    }
    public override void Exit()
    {
        base.Exit();
    }
}