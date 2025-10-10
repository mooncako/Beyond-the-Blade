using System.Collections;
using System.Collections.Generic;
using Animancer;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityUtils;

public class EnemyController : Controller, IPoolable
{
    [field: SerializeField, FoldoutGroup("Base Reference")] private PlayerSensor _playerSensor;
    
    [SerializeField, FoldoutGroup("Base Reference")] private Brain _brain;

    
    [field: SerializeField, BoxGroup("Debug")] private float _attackCooldown = .3f;

    [field: SerializeField, BoxGroup("Debug"), ReadOnly] public Transform CurrentTargetTransform;

    private Tween _attackDelayTween;
    private Tween _staggerTween;


    protected override void OnValidate()
    {
        base.OnValidate();
        if (_playerSensor == null) _playerSensor = GetComponentInChildren<PlayerSensor>();

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
        if (_parryCollider != null)
        {
            _parryCollider.OnParried.AddListener(OnParried);
        }

        _playerSensor.OnPlayerEnter += playerTransform =>
        {
            CurrentTargetTransform = playerTransform;
            Movement.LookInMoveDirection = false;
        };

        Health.OnDamage.AddListener(DamageFeedback);

    }

    protected override void OnDisable()
    {
        if (_parryCollider != null)
        {
            _parryCollider.OnParried.RemoveListener(OnParried);
        }
        _playerSensor.OnPlayerEnter -= playerTransform =>
        {
            CurrentTargetTransform = playerTransform;
            Movement.LookInMoveDirection = false;
        };

        Health.OnDamage.RemoveListener(DamageFeedback);

        _attackDelayTween.Stop();
    }

    private void FixedUpdate()
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
        if (_animationStateMachine.IsInStaggerState()) return;

        _hitTargets.Clear();

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

         if (_animationStateMachine.IsInStaggerState()) return;

        foreach (GameObject target in _hitTargets)
        {
            DamageInfo info = new DamageInfo(_currentSkill.Damage * Stats.DamageMultiplier, target, Health, gameObject, DamageType.Regular);
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

    protected override void OnParried(float duration)
    {
        base.OnParried(duration);
        _brain.Stagger(duration);
        _staggerTween = Tween.Delay(duration).OnComplete(() =>
        {
            _animationStateMachine.SwitchState(AnimationStateType.Idle);
        });
    }

    public override void StartAttackCooldown()
    {
        _attackDelayTween = Tween.Delay(_attackCooldown).OnComplete(() =>
        {
            CanAttack = true;
        });
    }

    private void DamageFeedback(DamageInfo info)
    {
        if (!_brain.IsTank)
        {
            _brain.Stagger(.1f, () => Movement.KnockBack(info.Instigator.transform, 500));
        }
    }

    public void OnPoolGet()
    {

    }

    public void OnPoolReturn()
    {
        
    }
}
