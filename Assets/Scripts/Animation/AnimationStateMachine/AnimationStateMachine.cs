using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;
using Animancer.TransitionLibraries;
using FMOD.Studio;
public class AnimationStateMachine : MonoBehaviour
{
    [SerializeField] private AnimancerComponent _animancer;
    [SerializeField] private IdleAnimationState _idleState;
    [SerializeField] private MoveAnimationState _moveState;
    [SerializeField] private ActionAnimationState _actionState;
    [SerializeField] private LocomotionAnimationSO _locomotionAnimation;
    [ReadOnly, BoxGroup("Debug"), ShowInInspector] public AnimationState CurrentState;
    [ReadOnly, BoxGroup("Debug"), ShowInInspector] public AnimationState PreviousState;
    [DisplayAsString, BoxGroup(""), ShowInInspector] private AnimationStateType _currentState;


    void OnValidate()
    {
        if (_animancer == null)
        {
            _animancer = GetComponent<AnimancerComponent>();
            _idleState = new IdleAnimationState(this, _animancer, _locomotionAnimation.Idle);
            _moveState = new MoveAnimationState(this, _animancer, _locomotionAnimation.Run);
            _actionState = new ActionAnimationState(this, _animancer);
            SetOwner();
        }
    }

    void Awake()
    {
        CurrentState = _idleState;
        SetOwner();
    }

    void OnEnable()
    {
        _idleState.RefreshState();
        _moveState.RefreshState();
        _actionState.RefreshState();
        CurrentState = _idleState;
        CurrentState.OnEnterState();
    }

    public void SwapAnimation(AnimationStateType type, ClipTransition clip)
    {
        switch (type)
        {
            case AnimationStateType.Idle:

                break;
            case AnimationStateType.Move:

                break;
            case AnimationStateType.Action:

                break;
        }
        CurrentState.SwapClip(clip);
    }

    public void SwitchState(AnimationStateType type)
    {
        switch (type)
        {
            case AnimationStateType.Idle:
                CurrentState.OnExitState();
                CurrentState = _idleState;
                CurrentState.OnEnterState();
                _currentState = AnimationStateType.Idle;
                break;
            case AnimationStateType.Move:
                CurrentState.OnExitState();
                CurrentState = _moveState;
                CurrentState.OnEnterState();
                _currentState = AnimationStateType.Move;
                break;
            case AnimationStateType.Action:
                CurrentState.OnExitState();
                CurrentState = _actionState;
                CurrentState.OnEnterState();
                _currentState = AnimationStateType.Action;
                break;
        }


    }

    public void SetActionStateClip(ClipTransition clip)
    {
        _actionState.Clip = clip;
    }

    private void SetOwner()
    {
        if (_idleState != null)
            _idleState.Owner = GetComponent<EnemyController>();
        if (_moveState != null)
            _moveState.Owner = GetComponent<EnemyController>();
        if (_actionState != null)
            _actionState.Owner = GetComponent<EnemyController>();
    }
}
