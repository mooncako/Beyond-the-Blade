using System.Collections.Generic;
using Animancer;
using Animancer.FSM;
using CrashKonijn.Goap.GenTest;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;
using UnityUtils;


[RequireComponent(typeof(CustomCharacterMovement))]
[RequireComponent(typeof(Targetable))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Vision))]
[RequireComponent(typeof(AnimancerComponent))]
[RequireComponent(typeof(AOEApplier))]
[RequireComponent(typeof(PersistentVFXHelper))]
public class Controller : MonoBehaviour
{
    [field: SerializeField, FoldoutGroup("Base Reference")] public CustomCharacterMovement Movement { get; private set; }  // get / private set is effectively read only
    [SerializeField, FoldoutGroup("Base Reference")] protected AnimationStateMachine _animationStateMachine;
    public AnimationStateMachine AnimationStateMachine => _animationStateMachine;
    [field: SerializeField, FoldoutGroup("Base Reference")] public Targetable Targetable { get; private set; }
    [field: SerializeField, FoldoutGroup("Base Reference")] public Health Health { get; private set; }
    [field: SerializeField, FoldoutGroup("Base Reference")] public Vision Vision { get; private set; }
    [field: SerializeField, FoldoutGroup("Base Reference")] public AOEApplier AOEApplier { get; private set; }
    [field: SerializeField, FoldoutGroup("Base Reference")] public Transform AttackPoint { get; private set; }
    [field: SerializeField, FoldoutGroup("Base Reference")] protected Weapon[] _weapons;
    [field: SerializeField, FoldoutGroup("Base Reference")] protected PersistentVFXHelper _persistentVFXHelper;
    [SerializeField, FoldoutGroup("Base Reference")] protected ParryCollider _parryCollider;
    [field: SerializeField, BoxGroup("Stats")] public Stats Stats { get; private set; }
    [SerializeField, BoxGroup("Settings")] protected LayerMask _attackableMask;
    [HideInInspector] public LayerMask AttackableMask => _attackableMask;
    [SerializeField, BoxGroup("Settings")] protected LayerMask _parryMask;
    [SerializeField, BoxGroup("Settings")] protected float _hitStopDuration;
    [HideInInspector] public float HitStopDuration => _hitStopDuration;
    [SerializeField, BoxGroup("Settings")] protected float _attackKnockbackForce;
    [BoxGroup("Weapon")] public Weapon CurrentWeapon;
    [BoxGroup("Debug"), ReadOnly] public bool CanMove = true;
    [BoxGroup("Debug"), ReadOnly] public bool CanAttack = true;
    [field: SerializeField, BoxGroup("Debug"), ReadOnly] protected Skill _currentSkill;
    [field: SerializeField, BoxGroup("Debug"), ReadOnly] public Vector3 TargetPos;
    [field: SerializeField, BoxGroup("Debug"), ReadOnly] protected bool _isSkillPlaying = false;
    [field: SerializeField, BoxGroup("Debug"), ReadOnly] protected List<GameObject> _hitTargets = new List<GameObject>();
    [BoxGroup("Debug"), ReadOnly] public bool IsStunImmune = false;

    protected Tween _hitStopTween;
    protected ModifierContext _context;

    protected virtual void Awake()
    {
        _context = new ModifierContext(this);
        ApplyStats();
    }

    protected virtual void OnValidate()
    {
        if (Movement == null) Movement = GetComponent<CustomCharacterMovement>();
        if (Targetable == null) Targetable = GetComponent<Targetable>();
        if (Health == null) Health = GetComponent<Health>();
        if (Vision == null) Vision = GetComponent<Vision>();
        if (AOEApplier == null) AOEApplier = GetComponent<AOEApplier>();
        if (_parryCollider == null) _parryCollider = GetComponentInChildren<ParryCollider>();
        if (_animationStateMachine == null) _animationStateMachine = GetComponent<AnimationStateMachine>();
        if (_persistentVFXHelper == null) _persistentVFXHelper = GetComponent<PersistentVFXHelper>();
        _weapons = GetComponentsInChildren<Weapon>();

        if ((_parryMask & (1 << 11)) == 0)
        {
            _parryMask |= 1 << 11;
        }
    }

    protected virtual void OnEnable()
    {
        if (_parryCollider != null)
        {
            _parryCollider.OnParried.AddListener(OnParried);
        }
    }

    protected virtual void OnDisable()
    {
        if (_parryCollider != null)
        {
            _parryCollider.OnParried.RemoveListener(OnParried);
        }
        _hitStopTween.Stop();
    }

    protected virtual void Update()
    {

        if (!AnimationStateMachine.IsInActionState() && !AnimationStateMachine.IsInStaggerState())
        {
            if (Movement.MoveInput != Vector3.zero)
            {
                AnimationStateMachine.SwitchState(AnimationStateType.Move);
            }
            else
            {
                AnimationStateMachine.SwitchState(AnimationStateType.Idle);
            }
        }
    }

    public virtual void ActivateSkill()
    {
        if (CurrentWeapon == null) return;
        if (!IsSkillPlaying())
        {
            _currentSkill = CurrentWeapon.LoopBasicAttack();
            ApplySkillEffect();
            // Animator.OverrideClipForState("Attack", CurrentWeapon.GetAnimationClip(_currentSkill.AnimationID));
            // Animator.Play("Attack");
            // AnimancerState state = Animancer.Play(CurrentWeapon.GetAnimationClip(_currentSkill.AnimationID));
            // Animancer.Play( CurrentWeapon.GetAnimationClip(_currentSkill.AnimationID),0.25f);
            // state.Time = 0;
            // state.Events(this).OnEnd ??= OnEnable;



            _isSkillPlaying = true;
        }


    }



    protected virtual void ApplySkillEffect(Skill skill = null)
    {

        //TODO buffs & debuffs
        if (skill == null)
        {
            AOEApplier.X = _currentSkill.SkillRange.X;
            AOEApplier.Y = _currentSkill.SkillRange.Y;
            AOEApplier.Z = _currentSkill.SkillRange.Z;
        }
        else
        {
            AOEApplier.X = skill.SkillRange.X;
            AOEApplier.Y = skill.SkillRange.Y;
            AOEApplier.Z = skill.SkillRange.Z;
        }


    }

    public virtual void DamageAnimEvent()
    {
        _hitTargets.Clear();
        if (_currentSkill.IsTargetedGroundAOE)
        {
            _hitTargets = AOEApplier.GetDamagedEntities(_currentSkill.SkillRange.AreaType, TargetPos, _attackableMask);
        }
        else
        {
            if (AttackPoint != null)
            {
                _hitTargets = AOEApplier.GetDamagedEntities(_currentSkill.SkillRange.AreaType, AttackPoint.position, _attackableMask);
            }
            else
            {
                _hitTargets = AOEApplier.GetDamagedEntities(_currentSkill.SkillRange.AreaType, transform.position, _attackableMask);

            }
        }

        foreach (GameObject target in _hitTargets)
        {
            Health health = target.GetComponent<Health>();
            DamageInfo info = new DamageInfo(_currentSkill.Damage * Stats.DamageMultiplier, target, health, gameObject, DamageType.Regular);
            health.Damage(info);
        }

        if (_hitTargets.Count > 0)
        {
            HitStop.Begin(AnimationStateMachine.CurrentState.AnimancerState, _hitStopDuration, _hitStopTween);
            CameraShakeEvent.Trigger(new LightShake());
        }

        _isSkillPlaying = false;
    }

    public virtual void SetTargetPos(Vector3 position)
    {
        TargetPos = position;
    }

    public bool IsSkillPlaying()
    {
        return _isSkillPlaying;
    }

    [Button]
    public virtual void ApplyStats()
    {
        if (Stats == null) return;
        Vision.ApplyStats(Stats);
        Health.ApplyStats(Stats);
    }
    public string GetCurrentSkillAnimationID()
    {
        return _currentSkill.AnimationID;
    }

    public virtual void ParryColliderOpenAnimEvent()
    {
        _parryCollider.OpenCollider();
    }

    public virtual void ParryColliderCloseAnimEvent()
    {
        _parryCollider.CloseCollider();
    }

    protected virtual void OnParried(float duration)
    {
        AnimationStateMachine.InterruptState(AnimationStateType.Stagger);
    }

    public virtual void StartAttackCooldown()
    {

    }

    public virtual void ModifierRelayAnimEvent(ModifierSO modifier)
    {
        modifier.Perform(_context);
    }

    public Skill GetCurrentSkill()
    {
        return _currentSkill;
    }

    public virtual void OnStatsUpdated()
    {
        Movement.SetSpeedMultiplier(Stats.MovementSpeedMultiplier);
        Health.UpdateMaxHealth(Stats.MaxHealth);
    }

    public bool IsAttacking()
    {
        return AnimationStateMachine.IsInAttackActionState() || AnimationStateMachine.IsInAbilityActionState();
    }

    public void AddPersistentVFX(VisualEffect effect)
    {
        _persistentVFXHelper.PersistentEffects.Add(effect);
    }

}
