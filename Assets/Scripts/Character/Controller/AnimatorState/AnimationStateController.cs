using UnityEngine;
using UnityEngine.Events;

public class AnimationStateController : StateMachineBehaviour
{
    [SerializeField] private bool _applyRootMotion = false;
    [SerializeField] private bool _canMove = true;
    [SerializeField] private float _speedMultiplier = 1f;

    [SerializeField] private bool _canAttack = true;
    [SerializeField] private bool _canParry = true;
    [SerializeField] private bool _canMuso = true;
    
    [SerializeField] private PlayerStateType _stateType = PlayerStateType.Idle;
    [SerializeField] private UnityEvent _stateEnterEvent;
    [SerializeField] private UnityEvent _stateExitEvent;
    
    [Header("Animator Parameters")]
    [SerializeField] private string[] _resetTriggersOnEnter;
    [SerializeField] private string[] _resetTriggersOnExit;
    [SerializeField] private string[] _setBoolsOnEnter;
    [SerializeField] private bool[] _boolValuesOnEnter;
    [SerializeField] private string[] _setBoolsOnExit;
    [SerializeField] private bool[] _boolValuesOnExit;
    
    [Header("Combat Controller")]
    [SerializeField] private bool _resetIsAttackingOnExit = false;
    [SerializeField] private bool _resetIsParryingOnExit = false;
    [SerializeField] private bool _resetIsChargingOnExit = false;
    [SerializeField] private bool _resetIsStaggeredOnExit = false;
    [SerializeField] private bool _resetMusoReadyOnExit = false;

    [Header("Input Processing")]
    [SerializeField] private bool _reactivateMovementInputOnExit = false;

    private CustomCharacterMovement _movement;
    private PlayerController _playerController;
    private PlayerCombatController _combatController;
    private Animator _animator;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        // Cache components
        _animator = animator;
        _movement = animator.GetComponent<CustomCharacterMovement>();
        _playerController = animator.GetComponent<PlayerController>();
        _combatController = animator.GetComponent<PlayerCombatController>();

        // Set movement states
        if (_movement != null)
        {
            _movement.CanMove = _canMove;
            _movement.SetSpeedMultiplier(_speedMultiplier);
            animator.applyRootMotion = _applyRootMotion;
        }

        // Set action availability
        if (_playerController != null)
        {
            _playerController.SetActionAvailable(PlayerActionType.Move, _canMove);
            _playerController.SetActionAvailable(PlayerActionType.Attack, _canAttack);
            _playerController.SetActionAvailable(PlayerActionType.Parry, _canParry);
            // _playerController.SetActionAvailable(PlayerActionType.Muso, _canMuso);

            // Invoke state change event
            PlayerAnimationStateChangeEvent.Trigger(_stateType);
        }
        
        // Reset triggers on enter
        if (_resetTriggersOnEnter != null && _resetTriggersOnEnter.Length > 0)
        {
            foreach (string trigger in _resetTriggersOnEnter)
            {
                if (!string.IsNullOrEmpty(trigger))
                {
                    animator.ResetTrigger(trigger);
                }
            }
        }
        
        // Set bools on enter
        if (_setBoolsOnEnter != null && _boolValuesOnEnter != null && 
            _setBoolsOnEnter.Length == _boolValuesOnEnter.Length)
        {
            for (int i = 0; i < _setBoolsOnEnter.Length; i++)
            {
                if (!string.IsNullOrEmpty(_setBoolsOnEnter[i]))
                {
                    animator.SetBool(_setBoolsOnEnter[i], _boolValuesOnEnter[i]);
                }
            }
        }
        
        // Invoke the Unity event
        _stateEnterEvent?.Invoke();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        
        // Reset triggers on exit
        if (_resetTriggersOnExit != null && _resetTriggersOnExit.Length > 0)
        {
            foreach (string trigger in _resetTriggersOnExit)
            {
                if (!string.IsNullOrEmpty(trigger))
                {
                    animator.ResetTrigger(trigger);
                }
            }
        }
        
        // Set bools on exit
        if (_setBoolsOnExit != null && _boolValuesOnExit != null && 
            _setBoolsOnExit.Length == _boolValuesOnExit.Length)
        {
            for (int i = 0; i < _setBoolsOnExit.Length; i++)
            {
                if (!string.IsNullOrEmpty(_setBoolsOnExit[i]))
                {
                    animator.SetBool(_setBoolsOnExit[i], _boolValuesOnExit[i]);
                }
            }
        }
        
        // Reset combat controller flags
        if (_combatController != null)
        {
            if (_resetIsAttackingOnExit)
            {
                _combatController.EndAttack();
            }
            
            if (_resetIsParryingOnExit)
            {
                _combatController.EndParry();
            }
            
            // if (_resetIsChargingOnExit)
            // {
            //     _combatController.CancelCharge();
            // }
            
            if (_resetIsStaggeredOnExit)
            {
                _combatController.EndStagger();
            }
        }
        
        // Reactivate movement input if needed
        if (_reactivateMovementInputOnExit && _playerController != null)
        {
            _playerController.SetActionAvailable(PlayerActionType.Move, true);
            
            // Check if there's stored input to immediately apply
            if (_playerController.InputProcessor.RawInputVector != Vector2.zero)
            {
                _playerController.InputProcessor.SetInputActive(true);
            }
        }
        
        // Invoke the Unity event
        _stateExitEvent?.Invoke();
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (!_applyRootMotion) return;
        animator.ApplyBuiltinRootMotion();

        // Stop rotation if > 10% into animation duration
        if (stateInfo.normalizedTime > 0.1f) return;

        if (_movement != null)
        {
            Quaternion aimRotation = Quaternion.LookRotation(_movement.LookDirection);
            animator.transform.rotation = Quaternion.Lerp(animator.transform.rotation, aimRotation, Time.deltaTime * 15f);
        }
    }
}
