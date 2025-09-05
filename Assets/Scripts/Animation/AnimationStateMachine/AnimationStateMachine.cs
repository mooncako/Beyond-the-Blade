using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;
using Animancer.TransitionLibraries;
using FMOD.Studio;
using System.Collections.Generic;
public class AnimationStateMachine : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private ModifierDatabaseSO _modifierDatabase;
    [SerializeField, BoxGroup("References")] private AnimancerComponent _animancer;
    [SerializeField] private IdleAnimationState _idleState;
    [SerializeField] private MoveAnimationState _moveState;
    [SerializeField] private ActionAnimationState _attackActionState;
    [SerializeField] private ActionAnimationState _parryActionState;
    [SerializeField] private ActionAnimationState _dashActionState;
    [SerializeField] private ActionAnimationState _abilityActionState;
    [SerializeField] private StaggerAnimationState _staggerState;
    [SerializeField] private LocomotionAnimationSO _locomotionAnimation;
    [SerializeField] public LinearMixerTransition LocomotionBlendtree { get; set; }

    [ReadOnly, BoxGroup("Debug"), ShowInInspector] public AnimationState CurrentState;
    [ReadOnly, BoxGroup("Debug"), ShowInInspector] public AnimationState PreviousState;
    [DisplayAsString, BoxGroup("Debug"), ShowInInspector] private AnimationStateType _currentState;

    private List<(float, ModifierSO)> _currentModifiers = new List<(float, ModifierSO)>();

    void OnValidate()
    {
        if (_animancer == null)
        {
            _animancer = GetComponent<AnimancerComponent>();
            _idleState = new IdleAnimationState(this, _animancer, _locomotionAnimation.Idle);
            _moveState = new MoveAnimationState(this, _animancer, _locomotionAnimation.Run);
            _attackActionState = new ActionAnimationState(this, _animancer, "Attack");
            _parryActionState = new ActionAnimationState(this, _animancer, "Parry");
            _abilityActionState = new ActionAnimationState(this, _animancer, "Ability");
            _dashActionState = new ActionAnimationState(this, _animancer, "Dash");
            _staggerState = new StaggerAnimationState(this, _animancer);
            SetOwner();
        }
    }
#if UNITY_EDITOR
    [Button]
    private void ApplyChanges()
    {
        _animancer = GetComponent<AnimancerComponent>();
        // _idleState = new IdleAnimationState(this, _animancer, _locomotionAnimation.Idle);
        // _moveState = new MoveAnimationState(this, _animancer, _locomotionAnimation.Run);
        _attackActionState = new ActionAnimationState(this, _animancer, "Attack");
        _parryActionState = new ActionAnimationState(this, _animancer, "Parry");
        _abilityActionState = new ActionAnimationState(this, _animancer, "Ability");
        _dashActionState = new ActionAnimationState(this, _animancer, "Dash");
        _staggerState = new StaggerAnimationState(this, _animancer);
        SetOwner();

    }
#endif

    void Awake()
    {
        CurrentState = _idleState;
        SetOwner();
    }

    void OnEnable()
    {
        _idleState.RefreshState();
        _moveState.RefreshState();
        _attackActionState.RefreshState();
        CurrentState = _idleState;
        CurrentState.OnEnterState();
    }
    private void Update()
    {


    }
    public void SwapAnimation(AnimationStateType type, ClipTransition clip)
    {
        switch (type)
        {
            case AnimationStateType.Idle:

                break;
            case AnimationStateType.Move:

                break;
            case AnimationStateType.Attack:

                break;
            case AnimationStateType.Parry:

                break;
            case AnimationStateType.Ability:

                break;
            case AnimationStateType.Dash:

                break;
            case AnimationStateType.Stagger:

                break;
        }
        CurrentState.SwapClip(clip);
    }

    /// <summary>
    /// Forcifully switching to another state, does not care whether the current state can transfer to the target state
    /// </summary>
    /// <param name="type"></param>
    public void SwitchState(AnimationStateType type)
    {
        CurrentState.OnExitState();

        switch (type)
        {
            case AnimationStateType.Idle:
                CurrentState = _idleState;
                _currentState = AnimationStateType.Idle;
                break;
            case AnimationStateType.Move:
                CurrentState = _moveState;
                _currentState = AnimationStateType.Move;
                break;
            case AnimationStateType.Attack:
                CurrentState = _attackActionState;
                _currentState = AnimationStateType.Attack;
                break;
            case AnimationStateType.Parry:
                CurrentState = _parryActionState;
                _currentState = AnimationStateType.Parry;
                break;
            case AnimationStateType.Ability:
                CurrentState = _abilityActionState;
                _currentState = AnimationStateType.Ability;
                break;
            case AnimationStateType.Dash:
                CurrentState = _dashActionState;
                _currentState = AnimationStateType.Dash;
                break;
            case AnimationStateType.Stagger:
                CurrentState = _staggerState;
                _currentState = AnimationStateType.Stagger;
                break;
        }

        CurrentState.OnEnterState();
    }

    /// <summary>
    /// Interrupting the current state and switch to another one if allows
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool InterruptState(AnimationStateType type)
    {
        if (!CanEnter(type)) return false;

        CurrentState.OnExitState();
        CurrentState.OnInterrupt();

        switch (type)
        {
            case AnimationStateType.Idle:

                CurrentState = _idleState;

                _currentState = AnimationStateType.Idle;
                break;
            case AnimationStateType.Move:
                CurrentState = _moveState;
                _currentState = AnimationStateType.Move;
                break;
            case AnimationStateType.Attack:
                CurrentState = _attackActionState;
                _currentState = AnimationStateType.Attack;
                break;
            case AnimationStateType.Parry:
                CurrentState = _parryActionState;
                _currentState = AnimationStateType.Parry;
                break;
            case AnimationStateType.Ability:
                CurrentState = _abilityActionState;
                _currentState = AnimationStateType.Ability;
                break;
            case AnimationStateType.Dash:
                CurrentState = _dashActionState;
                _currentState = AnimationStateType.Dash;
                break;
            case AnimationStateType.Stagger:
                CurrentState = _staggerState;
                _currentState = AnimationStateType.Stagger;
                break;
        }
        CurrentState.OnEnterState();
        return true;
    }

    public void SetAction(ClipTransition clip, AnimationStateType type, Skill skill)
    {
        // append the current modifiers on the skill to the list and push it to the animancer state
        _currentModifiers.Clear();
        for (int i = 0; i < skill.Buffs.Count; i++)
        {
            _currentModifiers.Add((skill.Buffs[i].Item2, _modifierDatabase.SkillModifierDict[skill.Buffs[i].Item1]));
        }

        switch (type)
        {
            case AnimationStateType.Attack:
                _attackActionState.Clip = clip;
                _attackActionState.UpdateModifiers(_currentModifiers);
                break;
            case AnimationStateType.Parry:
                _parryActionState.Clip = clip;
                _parryActionState.UpdateModifiers(_currentModifiers);
                break;
            case AnimationStateType.Ability:
                _abilityActionState.Clip = clip;
                _abilityActionState.UpdateModifiers(_currentModifiers);
                break;
        }


        
        
    }

    private void SetOwner()
    {
        if (_idleState != null)
            _idleState.Owner = GetComponent<Controller>();
        if (_moveState != null)
            _moveState.Owner = GetComponent<Controller>();
        if (_attackActionState != null)
            _attackActionState.Owner = GetComponent<Controller>();
        if (_parryActionState != null)
            _parryActionState.Owner = GetComponent<Controller>();
        if (_abilityActionState != null)
            _abilityActionState.Owner = GetComponent<Controller>();
        if (_dashActionState != null)
            _dashActionState.Owner = GetComponent<Controller>();
        if (_staggerState != null)
            _staggerState.Owner = GetComponent<Controller>();

    }

    public bool CanEnter(AnimationStateType stateType)
    {
        return (CurrentState.PossibleInterruptStates & stateType) == stateType;
    }

    public bool IsInMoveState()
    {
        return CurrentState == _moveState;
    }

    public bool IsInIdleState()
    {
        return CurrentState == _idleState;
    }

    public bool IsInAttackActionState()
    {
        return CurrentState == _attackActionState;
    }

    public bool IsInParryActionState()
    {
        return CurrentState == _parryActionState;
    }

    public bool IsInAbilityActionState()
    {
        return CurrentState == _abilityActionState;
    }

    public bool IsInActionState()
    {
        return _currentState == AnimationStateType.Attack
                            || _currentState == AnimationStateType.Parry
                            || _currentState == AnimationStateType.Ability
                            || _currentState == AnimationStateType.Dash;
    }

    public bool IsInStaggerState()
    {
        return CurrentState == _staggerState;
    }

    public bool IsMovable()
    {
        return CurrentState == _idleState || CurrentState == _moveState;
    }
}
