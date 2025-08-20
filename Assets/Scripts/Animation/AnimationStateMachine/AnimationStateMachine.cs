using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

public class AnimationStateMachine : MonoBehaviour
{
    [SerializeField] private AnimancerComponent _animancer;
    [SerializeField] private IdleAnimationState _idleState;
    [SerializeField] private MoveAnimationState _moveState;
    [SerializeField] private ActionAnimationState _actionState;
    [ReadOnly, BoxGroup("Debug")] public AnimationState CurrentState;
    [ReadOnly, BoxGroup("Debug")] public AnimationState PreviousState;

    void OnValidate()
    {
        if (_animancer == null)
        {
            _animancer = GetComponent<AnimancerComponent>();
            _idleState = new IdleAnimationState(this, _animancer);
            _moveState = new MoveAnimationState(this, _animancer);
            _actionState = new ActionAnimationState(this, _animancer);
        }
    }

    void Awake()
    {
        CurrentState = _idleState;
    }

    void OnEnable()
    {
        _idleState.RefreshState();
        _moveState.RefreshState();
        _actionState.RefreshState();
        CurrentState = _idleState;
        CurrentState.OnEnterState();
    }

    
}
