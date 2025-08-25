using System.Collections;
using System.Collections.Generic;
using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityUtils;

public class EnemyController : Controller, IPoolable
{
    [field: SerializeField, FoldoutGroup("Base Reference")] private PlayerSensor _playerSensor;
    [field: SerializeField, FoldoutGroup("Base Reference")] private AnimationStateMachine _animationStateMachine;
    [SerializeField, FoldoutGroup("Base Reference")] private Brain _brain;

    [BoxGroup("Debug"), ReadOnly] public bool CanMove = true;
    [BoxGroup("Debug"), ReadOnly] public bool CanAttack = true;

    [field: SerializeField, BoxGroup("Debug"), ReadOnly] public Transform CurrentTargetTransform;


    protected override void OnValidate()
    {
        base.OnValidate();
        if (_playerSensor == null) _playerSensor = GetComponentInChildren<PlayerSensor>();
        if (_animationStateMachine == null) _animationStateMachine = GetComponent<AnimationStateMachine>();
        if (_brain == null) _brain = GetComponent<Brain>();
        if ((_attackableMask & (1 << 7)) == 0)
        {
            _attackableMask |= 1 << 7;
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        _playerSensor.OnPlayerEnter += playerTransform =>
        {
            CurrentTargetTransform = playerTransform;
            Movement.LookInMoveDirection = false;
        };
    }

    protected override void OnDisable()
    {
        _playerSensor.OnPlayerEnter -= playerTransform =>
        {
            CurrentTargetTransform = playerTransform;
            Movement.LookInMoveDirection = false;
        };
    }

    private void Update()
    {
        if (CurrentTargetTransform != null)
        {
            Movement.SetLookPosition(CurrentTargetTransform.position);
        }
    }


    public void MoveTo(Vector3 destination)
    {
        if (CanMove)
        {
            Movement.MoveTo(destination);
        }
        else
        {
            Movement.Stop();
        }
    }

    public void Stop()
    {
        Movement.Stop();
    }

    public override void DamageAnimEvent()
    {
        if (_currentSkill.IsTargetedGroundAOE)
        {
            _hitTargets = AOEApplier.GetDamagedEntities(_currentSkill.SkillRange.AreaType, _targetPos, _attackableMask);
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
            DamageInfo info = new DamageInfo(_currentSkill.Damage, target, Health, gameObject, DamageType.Regular);
            target.GetComponent<Health>().Damage(info);
        }

        _isSkillPlaying = false;
    }

    public override void ActivateSkill()
    {
        if (CurrentWeapon == null) return;
        if (!IsSkillPlaying())
        {
             _currentSkill = CurrentWeapon.LoopBasicAttack();
            ApplySkillEffect();
            // swapout animation in AnimationStateMachine
            //_animationStateMachine.SwapAnimation(AnimationStateType.Action, CurrentWeapon.GetAnimationClip(_currentSkill.AnimationID));
            

            _isSkillPlaying = true;
        }
    }

    public void OnPoolGet()
    {

    }

    public void OnPoolReturn()
    {
        throw new System.NotImplementedException();
    }
}
