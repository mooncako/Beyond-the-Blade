using System.Collections.Generic;
using Animancer;
using Animancer.FSM;
using CrashKonijn.Goap.GenTest;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityUtils;


[RequireComponent(typeof(CustomCharacterMovement))]
[RequireComponent(typeof(Targetable))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Vision))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AnimancerComponent))]
[RequireComponent(typeof(AOEApplier))]
public class Controller : MonoBehaviour
{
    [field: SerializeField, FoldoutGroup("Base Reference")] public CustomCharacterMovement Movement { get; private set; }  // get / private set is effectively read only
    [field: SerializeField, FoldoutGroup("Base Reference")] public Targetable Targetable { get; private set; }
    [field: SerializeField, FoldoutGroup("Base Reference")] public Health Health { get; private set; }
    [field: SerializeField, FoldoutGroup("Base Reference")] public Vision Vision { get; private set; }
    [field: SerializeField, FoldoutGroup("Base Reference")] public Animator Animator { get; private set; }
    [field: SerializeField, FoldoutGroup("Base Reference")] public AOEApplier AOEApplier { get; private set; }
    [field: SerializeField, FoldoutGroup("Base Reference")] public Transform AttackPoint { get; private set; }
    [field: SerializeField, FoldoutGroup("Base Reference")] public AnimancerComponent Animancer { get; private set; }
    [field: SerializeField, FoldoutGroup("Base Reference")] protected Weapon[] _weapons;
    [field: SerializeField, BoxGroup("Stats")] public Stats Stats { get; private set; }
    [BoxGroup("Weapon")] public Weapon CurrentWeapon;
    [field: SerializeField, BoxGroup("Debug"), ReadOnly] protected Skill _currentSkill;
    [field: SerializeField, BoxGroup("Debug"), ReadOnly] protected Vector3 _targetPos;
    [field: SerializeField, BoxGroup("Debug"), ReadOnly] protected bool _isSkillPlaying = false;
    [field: SerializeField, BoxGroup("Debug"), ReadOnly] protected List<GameObject> _hitTargets = new List<GameObject>();

    protected virtual void Awake()
    {
        ApplyStats();
    }

    protected virtual void OnValidate()
    {
        if (Movement == null) Movement = GetComponent<CustomCharacterMovement>();
        if (Targetable == null) Targetable = GetComponent<Targetable>();
        if (Health == null) Health = GetComponent<Health>();
        if (Vision == null) Vision = GetComponent<Vision>();
        if (Animator == null) Animator = GetComponent<Animator>();
        if (AOEApplier == null) AOEApplier = GetComponent<AOEApplier>();
        if (Animancer == null) Animancer = GetComponent<AnimancerComponent>();
        _weapons = GetComponentsInChildren<Weapon>();
    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {
        
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
            AnimancerState state = Animancer.Play(CurrentWeapon.GetAnimationClip(_currentSkill.AnimationID));
            // Animancer.Play( CurrentWeapon.GetAnimationClip(_currentSkill.AnimationID),0.25f);
            state.Time = 0;
            // state.Events(this).OnEnd ??= OnEnable;



            _isSkillPlaying = true;
        }
        
    }

   

    protected virtual void ApplySkillEffect()
    {

        //TODO buffs & debuffs
        AOEApplier.X = _currentSkill.SkillRange.X;
        AOEApplier.Y = _currentSkill.SkillRange.Y;
        AOEApplier.Z = _currentSkill.SkillRange.Z;
        _hitTargets.Clear();

    }

    public virtual void DamageAnimEvent()
    {

        if (_currentSkill.IsTargetedGroundAOE)
        {
            _hitTargets = AOEApplier.GetDamagedEntities(_currentSkill.SkillRange.AreaType, _targetPos);
        }
        else
        {
            if (AttackPoint != null)
            {
                _hitTargets = AOEApplier.GetDamagedEntities(_currentSkill.SkillRange.AreaType, AttackPoint.position);
            }
            else
            {
                _hitTargets = AOEApplier.GetDamagedEntities(_currentSkill.SkillRange.AreaType, transform.position);
            }
        }


        foreach (GameObject target in _hitTargets)
        {
            Health health = target.GetComponent<Health>();

            DamageInfo info = new DamageInfo(_currentSkill.Damage, target, health, gameObject, DamageType.Regular);
            health.Damage(info);
        }
        
        _isSkillPlaying = false;
    }

    public virtual void SetTargetPos(Vector3 position)
    {
        _targetPos = position;
    }

    public bool IsSkillPlaying()
    {
        return _isSkillPlaying;
    }

    [Button]
    public void ApplyStats()
    {
        if (Stats == null) return;
        Vision.ApplyStats(Stats);
        Health.ApplyStats(Stats);
    }
}
