using UnityEngine;

public class MusoState : PlayerState
{
    private float _musoReadyDuration = 3f;
    
    public MusoState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) {}

    public override void Enter()
    {
        base.Enter();
        Player.OnStateChanged.Invoke(STATE.Muso);
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
