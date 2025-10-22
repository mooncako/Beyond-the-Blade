using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;
using Animancer.TransitionLibraries;
using FMOD.Studio;
using System.Collections.Generic;
using UnityEngine.Events;
public class AnimationStateMachine : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private ModifierDatabaseSO _modifierDatabase;
    [SerializeField, BoxGroup("References")] private AnimancerComponent _animancer;
    [SerializeField, BoxGroup("References")] private Controller _owner;
    [SerializeField] private IdleAnimationState _idleState;
    [SerializeField] private MoveAnimationState _moveState;
    [SerializeField] private ActionAnimationState _attackActionState;
    [SerializeField] private ActionAnimationState _parryActionState;
    [SerializeField] private ActionAnimationState _dashActionState;
    [SerializeField] private ActionAnimationState _abilityActionState;
    [SerializeField] private ActionAnimationState _executionActionState;
    [SerializeField] private StaggerAnimationState _staggerState;
    [SerializeField] private DeathAnimationState _deathState;
    [SerializeField] private LocomotionAnimationSO _locomotionAnimation;
    [SerializeField] private DirectionalMovementAnimationsSO _directionalMovementAnimations;
    [SerializeField] public LinearMixerTransition LocomotionBlendtree { get; set; }
    [SerializeField] public AvatarMask UpperBodyMask;
    public AnimancerLayer BaseLayer;
    public AnimancerLayer UpperBodyLayer;

    [HideInInspector] public UnityEvent<AnimationStateType, AnimationStateType> OnStateExit;

    [ReadOnly, BoxGroup("Debug"), ShowInInspector] public AnimationState CurrentState;
    [ReadOnly, BoxGroup("Debug"), ShowInInspector] public AnimationState PreviousState;
    [DisplayAsString, BoxGroup("Debug"), ShowInInspector] private AnimationStateType _currentState;

    private List<(float, ModifierSO)> _currentModifiers = new List<(float, ModifierSO)>();

    void OnValidate()
    {
        if (_owner == null) _owner = GetComponent<Controller>();
        if (_animancer == null)
        {
            _animancer = GetComponent<AnimancerComponent>();
            _idleState = new IdleAnimationState(this, _animancer, _locomotionAnimation.Idle);
            _moveState = new MoveAnimationState(this, _animancer, _locomotionAnimation.Run);
            _attackActionState = new ActionAnimationState(this, _animancer, "Attack");
            _parryActionState = new ActionAnimationState(this, _animancer, "Parry");
            _abilityActionState = new ActionAnimationState(this, _animancer, "Ability");
            _dashActionState = new ActionAnimationState(this, _animancer, "Dash");
            _executionActionState = new ActionAnimationState(this, _animancer, "Execution");
            _staggerState = new StaggerAnimationState(this, _animancer);
            _deathState = new DeathAnimationState(this, _animancer);
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
        _executionActionState = new ActionAnimationState(this, _animancer, "Execution");
        _staggerState = new StaggerAnimationState(this, _animancer);
        _deathState = new DeathAnimationState(this, _animancer);
        SetOwner();

    }
 
#endif

    void Awake()
    {
        CurrentState = _idleState;
        SetOwner();
        BaseLayer = _animancer.Layers[0];
        BaseLayer.SetDebugName("Base Layer");
        BaseLayer.Weight = 1;
        UpperBodyLayer = _animancer.Layers[1];
        UpperBodyLayer.Mask = UpperBodyMask;
        UpperBodyLayer.SetDebugName("Upper Body Layer");
        UpperBodyLayer.Weight = 0;


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
        // Update directional movement parameters if we're in move state with directional movement enabled
        if (CurrentState == _moveState && _moveState != null && _moveState.IsDirectionalMovement)
        {
            _moveState.UpdateMovementParameters();
        }
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
            case AnimationStateType.Execution:

                break;
            case AnimationStateType.Stagger:

                break;
        }
        CurrentState.SwapClip(clip);
    }

    /// <summary>
    /// Forcifully switching to another state, does not care whether the current state can transfer to the target state, unless dead
    /// </summary>
    /// <param name="type"></param>
    public void SwitchState(AnimationStateType type)
    {
        if (IsInDeathState()) return;

        CurrentState.OnExitState();
        OnStateExit.Invoke(_currentState, type);
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
            case AnimationStateType.Execution:
                CurrentState = _executionActionState;
                _currentState = AnimationStateType.Execution;
                break;
            case AnimationStateType.Dash:
                CurrentState = _dashActionState;
                _currentState = AnimationStateType.Dash;
                break;
            case AnimationStateType.Stagger:
                CurrentState = _staggerState;
                _currentState = AnimationStateType.Stagger;
                break;
            case AnimationStateType.Death:
                CurrentState = _deathState;
                _currentState = AnimationStateType.Death;
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
            case AnimationStateType.Execution:
                CurrentState = _executionActionState;
                _currentState = AnimationStateType.Execution;
                break;
            case AnimationStateType.Dash:
                CurrentState = _dashActionState;
                _currentState = AnimationStateType.Dash;
                break;
            case AnimationStateType.Stagger:
                CurrentState = _staggerState;
                _currentState = AnimationStateType.Stagger;
                break;
            case AnimationStateType.Death:
                CurrentState = _deathState;
                _currentState = AnimationStateType.Death;
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

        for (int i = 0; i < skill.Debuffs.Count; i++)
        {
            _currentModifiers.Add((skill.Debuffs[i].Item2, _modifierDatabase.SkillModifierDict[skill.Debuffs[i].Item1]));
        }

        if (type == AnimationStateType.Attack)
        {
            for(int i = 0; i < _owner.CurrentWeapon.UniversalAttackModifiers.Count; i++)
            {
                switch(_owner.CurrentWeapon.UniversalAttackModifiers[i].Item2)
                {
                    case UpgradeSlotType.Start:
                        _currentModifiers.Add((0, _modifierDatabase.SkillModifierDict[_owner.CurrentWeapon.UniversalAttackModifiers[i].Item1]));
                        break;
                    case UpgradeSlotType.Mid:
                        _currentModifiers.Add((0.5f, _modifierDatabase.SkillModifierDict[_owner.CurrentWeapon.UniversalAttackModifiers[i].Item1]));
                        break;
                    case UpgradeSlotType.End:
                        _currentModifiers.Add((1, _modifierDatabase.SkillModifierDict[_owner.CurrentWeapon.UniversalAttackModifiers[i].Item1]));
                        break;
                }
            }
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
            case AnimationStateType.Execution:
                _executionActionState.Clip = clip;
                _executionActionState.UpdateModifiers(_currentModifiers);
                break;
            case AnimationStateType.Dash:
                _dashActionState.Clip = clip;
                _dashActionState.UpdateModifiers(_currentModifiers);
                break;
        }


        
        
    }

    public void SetOwner(Controller owner = null)
    {
        if (owner == null)
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
            if (_executionActionState != null)
                _executionActionState.Owner = GetComponent<Controller>();
            if (_dashActionState != null)
                _dashActionState.Owner = GetComponent<Controller>();
            if (_staggerState != null)
                _staggerState.Owner = GetComponent<Controller>();
            if (_deathState != null)
                _deathState.Owner = GetComponent<Controller>();
        }
        else
        {
            if (_idleState != null)
                _idleState.Owner = owner;
            if (_moveState != null)
                _moveState.Owner = owner;
            if (_attackActionState != null)
                _attackActionState.Owner = owner;
            if (_parryActionState != null)
                _parryActionState.Owner = owner;
            if (_abilityActionState != null)
                _abilityActionState.Owner = owner;
            if (_executionActionState != null)
                _executionActionState.Owner = owner;
            if (_dashActionState != null)
                _dashActionState.Owner = owner;
            if (_staggerState != null)
                _staggerState.Owner = owner;
            if (_deathState != null)
                _deathState.Owner = owner;
        }
        

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
                            || _currentState == AnimationStateType.Dash
                            || _currentState == AnimationStateType.Execution;
    }

    public bool IsInStaggerState()
    {
        return CurrentState == _staggerState;
    }

    public bool IsInDeathState()
    {
        return CurrentState == _deathState;
    }

    public bool IsMovable()
    {
        return CurrentState == _idleState || CurrentState == _moveState;
    }

    /// <summary>
    /// Gets the directional movement animations asset
    /// </summary>
    public DirectionalMovementAnimationsSO GetDirectionalMovementAnimations()
    {
        return _directionalMovementAnimations;
    }

    /// <summary>
    /// Sets the directional movement animations asset
    /// </summary>
    public void SetDirectionalMovementAnimations(DirectionalMovementAnimationsSO animations)
    {
        _directionalMovementAnimations = animations;
    }

    /// <summary>
    /// Enables or disables directional movement on the move state
    /// </summary>
    public void SetDirectionalMovement(bool isDirectional)
    {
        if (_moveState != null)
        {
            _moveState.SetDirectionalMovement(isDirectional);
        }
    }

    /// <summary>
    /// Checks if the move state is using directional movement
    /// </summary>
    public bool IsUsingDirectionalMovement()
    {
        return _moveState != null && _moveState.IsDirectionalMovement;
    }
}
