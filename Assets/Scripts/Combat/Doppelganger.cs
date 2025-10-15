using System;
using System.Collections.Generic;
using Animancer;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AnimationStateMachine))]
[RequireComponent(typeof(AnimancerComponent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AOEApplier))]
public class Doppelganger : MonoBehaviour, IPoolable
{
    [SerializeField, BoxGroup("References")] private AnimationStateMachine _animationStateMachine;
    [SerializeField, BoxGroup("References")] private AOEApplier AOEApplier;
    [SerializeField, BoxGroup("References")] private Transform AttackPoint;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private Skill _skill;
    [SerializeField, BoxGroup("Debug"), ReadOnly] public ObjectPool Pool;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private float _hitStopDuration;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private Weapon _weapon;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private Controller _owner;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private LayerMask _attackableMask;
    [field: SerializeField, BoxGroup("Debug"), ReadOnly] protected List<GameObject> _hitTargets = new List<GameObject>();

    [HideInInspector] public UnityEvent<GameObject> OnDoppelgangerEnd;

    private Tween _hitStopTween;

    private void OnValidate()
    {
        if (_animationStateMachine == null) _animationStateMachine = GetComponent<AnimationStateMachine>();
        if ((_attackableMask & (1 << 8)) == 0)
        {
            _attackableMask |= 1 << 8;
        }
    }

    public void OnPoolGet()
    {

    }

    public void OnPoolReturn()
    {
        OnDoppelgangerEnd.RemoveAllListeners();
    }

    [Button]
    public void AssignData(Controller controller)
    {

        _owner = controller;
        _skill = controller.GetCurrentSkill();
        _weapon = controller.CurrentWeapon;
        _hitStopDuration = _owner.HitStopDuration;
        _animationStateMachine.SetOwner(controller);
    }

    [Button]
    public void PlaySkill()
    {
        SpawnVFXEvent.Trigger(transform, _skill.AnimationID, _skill.VFXInfo);
        Skill skill = new Skill(_skill);
        _skill = skill;
        AOEApplier.X = _skill.SkillRange.X;
        AOEApplier.Y = _skill.SkillRange.Y;
        AOEApplier.Z = _skill.SkillRange.Z;
        skill.Buffs.RemoveAll(item => item.Item1 == "DOPPELGANGER");
        _animationStateMachine.SetAction(_weapon.GetAnimationClip(skill.AnimationID), AnimationStateType.Attack, skill);
        _animationStateMachine.SwitchState(AnimationStateType.Attack);
        _animationStateMachine.OnStateExit.AddListener(OnSkillEnd);
    }

    private void OnSkillEnd(AnimationStateType previous, AnimationStateType current)
    {
        _animationStateMachine.OnStateExit.RemoveListener(OnSkillEnd);
        if (previous == AnimationStateType.Attack && current == AnimationStateType.Idle)
        {
            OnDoppelgangerEnd.Invoke(gameObject);
        }
        
    }

    public virtual void DamageAnimEvent()
    {
        _hitTargets.Clear();
        if (_skill.IsTargetedGroundAOE)
        {
            _hitTargets = AOEApplier.GetDamagedEntities(_skill.SkillRange.AreaType, _owner.TargetPos, _attackableMask);
        }
        else
        {
            if (AttackPoint != null)
            {
                _hitTargets = AOEApplier.GetDamagedEntities(_skill.SkillRange.AreaType, AttackPoint.position, _attackableMask);
            }
            else
            {
                _hitTargets = AOEApplier.GetDamagedEntities(_skill.SkillRange.AreaType, transform.position, _attackableMask);

            }
        }
        foreach (GameObject target in _hitTargets)
        {
            Health health = target.GetComponent<Health>();
            DamageInfo info = new DamageInfo(_skill.Damage * _owner.Stats.DamageMultiplier, target, health, gameObject, DamageType.Regular);
            health.Damage(info);
        }

        if (_hitTargets.Count > 0)
        {
            HitStop.Begin(_animationStateMachine.CurrentState.AnimancerState, _hitStopDuration, _hitStopTween);
            CameraShakeEvent.Trigger(new LightShake());
        }
    }
}
