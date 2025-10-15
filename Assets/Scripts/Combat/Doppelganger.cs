using System.Collections.Generic;
using Animancer;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(AnimationStateMachine))]
[RequireComponent(typeof(AnimancerComponent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AOEApplier))]
public class Doppelganger : MonoBehaviour, IPoolable
{
    [SerializeField, BoxGroup("References")] private AnimationStateMachine _animationStateMachine;
    [SerializeField, BoxGroup("References")] private AOEApplier AOEApplier;
    [SerializeField, BoxGroup("References")] private Transform AttackPoint;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private string _skillId;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private float _hitStopDuration;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private Weapon _weapon;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private Controller _owner;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private LayerMask _attackableMask;
    [field: SerializeField, BoxGroup("Debug"), ReadOnly] protected List<GameObject> _hitTargets = new List<GameObject>();

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

    }

    [Button]
    public void AssignData(string skillId, Weapon weapon, Controller controller)
    {
        _skillId = skillId;
        _weapon = weapon;
        _owner = controller;
        _hitStopDuration = _owner.HitStopDuration;
        _animationStateMachine.SetOwner(controller);
    }

    [Button]
    public void PlaySkill()
    {
        SpawnVFXEvent.Trigger(transform, _weapon.GetSkill(_skillId).AnimationID, _weapon.GetSkill(_skillId).VFXInfo);
        _animationStateMachine.SetAction(_weapon.GetAnimationClip(_weapon.GetSkill(_skillId).AnimationID), AnimationStateType.Attack, _weapon.GetSkill(_skillId));
        _animationStateMachine.SwitchState(AnimationStateType.Attack);
        _animationStateMachine.OnStateExit.AddListener(OnSkillEnd);
    }

    private void OnSkillEnd(AnimationStateType previous, AnimationStateType current)
    {
        _animationStateMachine.OnStateExit.RemoveListener(OnSkillEnd);
        if (previous == AnimationStateType.Attack && current == AnimationStateType.Idle)
        {
            gameObject.SetActive(false);
        }
        
    }

    public virtual void DamageAnimEvent()
    {
        _hitTargets.Clear();
        if (_weapon.GetSkill(_skillId).IsTargetedGroundAOE)
        {
            _hitTargets = AOEApplier.GetDamagedEntities(_weapon.GetSkill(_skillId).SkillRange.AreaType, _owner.TargetPos, _attackableMask);
        }
        else
        {
            if (AttackPoint != null)
            {
                _hitTargets = AOEApplier.GetDamagedEntities(_weapon.GetSkill(_skillId).SkillRange.AreaType, AttackPoint.position, _attackableMask);
            }
            else
            {
                _hitTargets = AOEApplier.GetDamagedEntities(_weapon.GetSkill(_skillId).SkillRange.AreaType, transform.position, _attackableMask);

            }
        }

        foreach (GameObject target in _hitTargets)
        {
            Health health = target.GetComponent<Health>();
            DamageInfo info = new DamageInfo(_weapon.GetSkill(_skillId).Damage * _owner.Stats.DamageMultiplier, target, health, gameObject, DamageType.Regular);
            health.Damage(info);
        }

        if (_hitTargets.Count > 0)
        {
            HitStop.Begin(_animationStateMachine.CurrentState.AnimancerState, _hitStopDuration, _hitStopTween);
            CameraShakeEvent.Trigger(new LightShake());
        }
    }
}
