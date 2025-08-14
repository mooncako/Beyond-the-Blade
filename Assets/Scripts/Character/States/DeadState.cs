using UnityEngine;
using UnityEngine.InputSystem;
public class DeadState : PlayerState
{
    public DeadState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) {}

    public override void Enter()
    {
        base.Enter();
        if (Player.Combat.IsCharging == true)
        {
            Player.Combat.CancelCharge();
            EventHub.Instance.OnChargeEnded.Invoke();
        }
        
        PlayerAnimationStateChangeEvent.Trigger(PlayerStateType.Dead);

        new WaitForSeconds(1f);
        Player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        Player.Animator.SetTrigger("IsDead");
        Cursor.lockState = CursorLockMode.None;
    }
}