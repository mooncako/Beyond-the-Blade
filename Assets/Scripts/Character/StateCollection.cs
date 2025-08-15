using UnityEngine;

public class StateCollection{
    public IdleState IdleState {get; private set;}
    public MovingState MovingState {get; private set;}
    public AttackingState AttackingState {get; private set;}
    public ChargingState ChargingState {get; private set;}  
    public ParryingState ParryingState {get; private set;}
    public MusoState MusoState {get; private set;}
    public SheatheState SheatheState {get; private set;}
    public StaggerState StaggerState {get; private set;}
    public DeadState DeadState {get; private set;}

    public StateCollection(PlayerController player, PlayerStateMachine stateMachine)
    {
        IdleState = new IdleState(player, stateMachine);
        MovingState = new MovingState(player, stateMachine);
        AttackingState = new AttackingState(player, stateMachine);
        ChargingState = new ChargingState(player, stateMachine);
        ParryingState = new ParryingState(player, stateMachine);
        MusoState = new MusoState(player, stateMachine);
        SheatheState = new SheatheState(player, stateMachine);        
        StaggerState = new StaggerState(player, stateMachine);
        DeadState = new DeadState(player, stateMachine); 
    }
}
