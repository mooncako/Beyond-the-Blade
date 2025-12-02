using System;
using System.Collections;
using System.Collections.Generic;
using Animancer;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityUtils;

public class EnemyController : Controller, IPoolable
{
    [field: SerializeField, FoldoutGroup("Base Reference")] private PlayerSensor _playerSensor;
    
    [SerializeField, FoldoutGroup("Base Reference")] private Brain _brain;
    [SerializeField, FoldoutGroup("Base Reference")] public NavMeshAgent Agent;
    [SerializeField, FoldoutGroup("Base Reference")] public Posture Posture;
    

    
    [field: SerializeField, BoxGroup("Debug")] private float _attackCooldown = .3f;
    [field: SerializeField, BoxGroup("Debug")] private bool _canRotate = true;

    [field: SerializeField, BoxGroup("Debug"), ReadOnly] public Transform CurrentTargetTransform;

    private Tween _attackDelayTween;
    private Tween _staggerTween;


    protected override void OnValidate()
    {
        base.OnValidate();
        if (Energy == null) Energy = GetComponent<Energy>();
        if (_playerSensor == null) _playerSensor = GetComponentInChildren<PlayerSensor>();
        if (Agent == null) Agent = GetComponent<NavMeshAgent>();
        if (Posture == null) Posture = GetComponent<Posture>();

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
        Health.OnDeath.AddListener(OnDeath);
        Posture.OnStunned.AddListener(OnStunned);

        // StartCoroutine(UpdateStateCO());
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
        Health.OnDeath.RemoveListener(OnDeath);
        Posture.OnStunned.AddListener(OnStunned);

        _attackDelayTween.Stop();
        _staggerTween.Stop();
        // StopCoroutine(UpdateStateCO());
    }

    private void FixedUpdate()
    {
        if (CurrentTargetTransform != null && _canRotate)
        {
            Movement.SetLookPosition(CurrentTargetTransform.position);
        }
    }

    protected override void Update()
    {
        if (!AnimationStateMachine.IsInActionState() && !AnimationStateMachine.IsInStaggerState() && !AnimationStateMachine.IsInDeathState())
        {
            if (Movement.IsAgentMoving())
            {
                if(!AnimationStateMachine.IsInMoveState())
                    AnimationStateMachine.SwitchState(AnimationStateType.Move);
            }
        }
    }

    private IEnumerator UpdateStateCO()
    {
        while(true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(.5f, .8f));
            if (!AnimationStateMachine.IsInActionState() && !AnimationStateMachine.IsInStaggerState() && !AnimationStateMachine.IsInDeathState())
            {
                if(AnimationStateMachine.IsInMoveState())
                    AnimationStateMachine.SwitchState(AnimationStateType.Idle);
            }
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
        if (!AnimationStateMachine.IsInActionState()) return;

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

         if (AnimationStateMachine.IsInStaggerState()) return;

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
            if (_currentSkill != null)
            {
                _isSkillPlaying = true;

            }

        }

    }

    public void ActivateHeavySkill()
    {
        if (CurrentWeapon == null) return;
        if (!IsSkillPlaying())
        {
            _currentSkill = CurrentWeapon.LoopHeavyAttack();
            if (_currentSkill != null)
            {
                _isSkillPlaying = true;

            }

        }
    }
    
    public void ActivateProjectile()
    {
        if (CurrentWeapon == null) return;
        if (!IsSkillPlaying())
        {
            _currentSkill = CurrentWeapon.GetProjectile();
            if (_currentSkill != null)
            {
                _isSkillPlaying = true;

            }

            // swapout animation in AnimationStateMachine
            //_animationStateMachine.SwapAnimation(AnimationStateType.Action, CurrentWeapon.GetAnimationClip(_currentSkill.AnimationID));

        }
    }

    public bool CheckSkill()
    {
        if (_currentSkill == null) return false;
        if (_currentSkill.VFXInfo.UsingIndicator)
        {
            SpawnVFXEvent.Trigger(transform, "ATTACK_WARNING", new VFXInfo(new Vector3(0, .9f, 0), transform.rotation, Vector3.one, true, false));
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsSkillNull()
    {
        return _currentSkill == null;
    }
    
    public void PlaySkillEffect()
    {
        SpawnVFXEvent.Trigger(transform, _currentSkill.AnimationID, _currentSkill.VFXInfo);
        ApplySkillEffect();
    }

    protected override void OnParried(float duration)
    {
        base.OnParried(duration);
        Posture.IncreaseStun(UnityEngine.Random.Range(_currentSkill.Damage/10, _currentSkill.Damage/10 + _currentSkill.Damage/20), duration);
        Stun(.2f, null, false);
        ParrySuccessEvent.Trigger();
        CameraShakeEvent.Trigger(new LightShake());
    }

    private void OnStunned(float duration)
    {
        Stun(duration, null, true);
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
        Movement.KnockBack(info.Instigator.transform, 500);
    }

    public override void Stun(float duration, Action onComplete = null, bool forceStun = false)
    {
        if (IsStunImmune && !forceStun) return;
        _persistentVFXHelper.StopPersistentEffects();
        _brain.Stun(duration, onComplete);
        AnimationStateMachine.SwitchState(AnimationStateType.Stagger);
        _staggerTween = Tween.Delay(duration).OnComplete(() =>
        {
            AnimationStateMachine.SwitchState(AnimationStateType.Idle);
        });
    }

    public void OnPoolGet()
    {

    }

    public void OnPoolReturn()
    {
        Movement.CanMove = true;
        _canRotate = true;
        gameObject.layer = LayerMask.NameToLayer("Character");
        _matController.ResetDissolve();
    }

    public void ToggleRotationAnimEvent(int toggle)
    {
        if (toggle == 0)
        {
            _canRotate = true;
        }
        else
        {
            _canRotate = false;
        }

    }

    public void OnDeath(DamageInfo info)
    {
        _persistentVFXHelper.StopPersistentEffects();
        _brain.Dead();
        Movement.Stop();
        Movement.CanMove = false;
        _canRotate = false;
        gameObject.layer = LayerMask.NameToLayer("Corpse");
    }

    public override void ApplyStats()
    {
        base.ApplyStats();
        Energy.ApplyStats(Stats);
        if(Stats is EnemyStatsSO enemyStats)
        {
            Posture?.ApplyStats(enemyStats.StunThreshold);
        }
        
    }
}
