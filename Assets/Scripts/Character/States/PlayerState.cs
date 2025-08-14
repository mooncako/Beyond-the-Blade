using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerState
{
    protected PlayerController Player;
    protected PlayerStateMachine StateMachine; 

    protected PlayerState(PlayerController player, PlayerStateMachine stateMachine)
    {
        Player = player;
        StateMachine = stateMachine;
    }

    public virtual void Enter()
    {

    }
    public virtual void Update()
    {
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        
        // Project the camera's forward and right vectors onto the horizontal plane
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();
        
        // Calculate the movement direction relative to the camera
        Vector3 moveDirection = 
            cameraRight * Player.InputProcessor.InputVectorNormalized.x + 
            cameraForward * Player.InputProcessor.InputVectorNormalized.y;
        
        Player.Movement.SetMoveInput(moveDirection);
        
        if (Player.Combat.IsStaggered)
        {
            StateMachine.ChangeState(Player.States.StaggerState);
        }
    }
    public virtual void FixedUpdate()
    {

    }    
    public virtual void Exit()
    {

    }
}
