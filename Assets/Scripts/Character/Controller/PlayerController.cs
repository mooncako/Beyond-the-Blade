using System.Collections.Generic;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.VFX;
using Animancer;
using PrimeTween;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Splines;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CustomCharacterMovement))]
public class PlayerController : Controller,
    MMEventListener<PlayerAnimationStateChangeEvent>,
    MMEventListener<LevelSetupCompleteEvent>,
    MMEventListener<SkillSwapEvent>,
    MMEventListener<AbilitySwapEvent>,
    MMEventListener<AddNewAbilityEvent>,
    MMEventListener<ParrySuccessEvent>,
    MMEventListener<CurrencyEarnedEvent>,
    MMEventListener<LevelTransitionEvent>
{
    [field: SerializeField, FoldoutGroup("Base Reference")] private PlayerInput _input;
    // [field: SerializeField, FoldoutGroup("Base Reference")] private BezierLine _bezierLine;
    // [field: SerializeField, FoldoutGroup("Base Reference")] private LineRenderer _lineRenderer;
    [field: SerializeField, FoldoutGroup("Base Reference")] public Stamina Stamina;
    [SerializeField, FoldoutGroup("Base Reference")] private SplineAnimate _splineAnimate;
    [SerializeField, FoldoutGroup("Base Reference")] private GameObject _playerMesh;
    [SerializeField, FoldoutGroup("Base Reference")] private VisualEffect _teleportEffect;
    [SerializeField, FoldoutGroup("Base Reference")] private PlayerSoundController _playerSoundController;

    [Header("General Settings")]
    [BoxGroup("Input")] public InputProcessor InputProcessor;
    [SerializeField, BoxGroup("Input"), Tooltip("The maximum distance for aim assist to find a target.")] private float _aimAssistDistance = 2.5f;
    [BoxGroup("Input"), ReadOnly] public Vector2 RotateInput { get; set; }
    [BoxGroup("Input"), ReadOnly] public PlayerStateType CurrentState { get; private set; }
    [BoxGroup("Input"), ReadOnly] public bool CanRotate = true;
    [BoxGroup("Input"), ReadOnly] private Vector3 _aimPoint;
    private Dictionary<PlayerActionType, bool> _availableActions = new Dictionary<PlayerActionType, bool>();
    [BoxGroup("Ability"), ReadOnly] public Skill CurrentAbility { get; private set; }
    [BoxGroup("Ability"), ReadOnly] public UnityEvent<float> OnAbilityStartCooldown;

    [SerializeField, BoxGroup("Debug"), ReadOnly] private bool _isInAimMode = false;

    // [SerializeField] private ParryHit _parryHitVFX;

    [Header("Animancer")]
    [SerializeField] private AnimancerComponent _animancerComponent;

    public PlayerStatsSO PlayerStats => (PlayerStatsSO)Stats;

    private Vector3 _forward;

    private bool _isPerfectParryWindowActive = false;
    public bool IsPerfectParryWindowActive => _isPerfectParryWindowActive;

    [HideInInspector] public UnityEngine.Events.UnityEvent OnExecutionStarted;
    [HideInInspector] public UnityEngine.Events.UnityEvent OnAbilityCycled;
    [ReadOnly] public bool IsNewSession = true;

    private Tween _iframeTween;
    private static readonly Collider[] _enemyOverlapBuffer = new Collider[64];

    protected override void OnValidate()
    {
        base.OnValidate();
        if (_input == null) _input = GetComponent<PlayerInput>();
        //if (AnimationStateMachine == null) AnimationStateMachine = GetComponent<AnimationStateMachine>();  //duplicate in base
        if (_animancerComponent == null) _animancerComponent = GetComponent<AnimancerComponent>();
        if (Energy == null) Energy = GetComponent<Energy>();
        if (Stamina == null) Stamina = GetComponent<Stamina>();
        if (_splineAnimate == null) _splineAnimate = GetComponent<SplineAnimate>();
        if (_playerSoundController == null) _playerSoundController = GetComponent<PlayerSoundController>();
        if ((_attackableMask & (1 << 8)) == 0)
        {
            _attackableMask |= 1 << 8;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        if (IsNewSession)
        {
            Stats.Clear();
            IsNewSession = false;
        }
        InputProcessor = new InputProcessor();

        foreach (PlayerActionType actionType in System.Enum.GetValues(typeof(PlayerActionType)))
        {
            _availableActions[actionType] = true;
        }

    }

    void Start()
    {

    }

    protected override void Update()
    {
        if (!AnimationStateMachine.IsInActionState() && !AnimationStateMachine.IsInStaggerState() && !AnimationStateMachine.IsInDeathState())
        {
            if (Movement.MoveInput != Vector3.zero)
            {
                if(!AnimationStateMachine.IsInMoveState())
                    AnimationStateMachine.SwitchState(AnimationStateType.Move);
            }
            else
            {
                if(!AnimationStateMachine.IsInIdleState())
                    AnimationStateMachine.SwitchState(AnimationStateType.Idle);
            }
        }

        HandleRotation();
        InputProcessor.SetInputActive(AnimationStateMachine.IsMovable());

        // link the input to movement
        var customMovement = Movement as CustomCharacterMovement;
        if (customMovement != null)
        {
            customMovement.SetMoveInput(GetMoveDir());
        }
        if (_isPerfectParryWindowActive)
        {
            DetectParry();
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.MMEventStartListening<PlayerAnimationStateChangeEvent>();
        this.MMEventStartListening<LevelSetupCompleteEvent>();
        this.MMEventStartListening<SkillSwapEvent>();
        this.MMEventStartListening<AbilitySwapEvent>();
        this.MMEventStartListening<AddNewAbilityEvent>();
        this.MMEventStartListening<ParrySuccessEvent>();
        this.MMEventStartListening<CurrencyEarnedEvent>();
        this.MMEventStartListening<LevelTransitionEvent>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        //Listen for projectile deflection
        if (_parryCollider != null)
        {
            _parryCollider.OnProjectileDeflected.AddListener(OnProjectileDeflected);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.MMEventStopListening<PlayerAnimationStateChangeEvent>();
        this.MMEventStopListening<LevelSetupCompleteEvent>();
        this.MMEventStopListening<SkillSwapEvent>();
        this.MMEventStopListening<AbilitySwapEvent>();
        this.MMEventStopListening<AddNewAbilityEvent>();
        this.MMEventStopListening<ParrySuccessEvent>();
        this.MMEventStopListening<CurrencyEarnedEvent>();
        this.MMEventStopListening<LevelTransitionEvent>();
        SceneManager.sceneLoaded -= OnSceneLoaded;
        _iframeTween.Stop();
        
        if (_parryCollider != null)
        {
            _parryCollider.OnProjectileDeflected.RemoveListener(OnProjectileDeflected);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (CurrentAbility == null)
        {
            CurrentAbility = CurrentWeapon.GetAbility();
        }

        PlayerInitializedEvent.Trigger(this);
        if(scene.name == SCENENAME.Hub)
        {
            Reset();
            PlayerRespawnTeleportEvent.Trigger(this);
        }
    }

    public void OnMMEvent(PlayerAnimationStateChangeEvent e)
    {
        CurrentState = e.State;
    }

    public void OnMMEvent(LevelSetupCompleteEvent e)
    {
        Movement.Teleport(e.SpawnPosition);
    }

    public void OnMMEvent(SkillSwapEvent e)
    {
        CurrentWeapon.WeaponSkillDict[AvailableSkillType.Attack][CurrentWeapon.GetAttackSkillIndexWithCooldown(e.Skill.Cooldown)] = e.SkillId;
        CurrentWeapon.RefreshAvailableSkills();
    }
    public void OnMMEvent(AbilitySwapEvent e)
    {
        CurrentWeapon.WeaponSkillDict[AvailableSkillType.Ability][e.Index] = e.SkillId;
        CurrentWeapon.RefreshAvailableSkills();
        NewAbilityCallbackEvent.Trigger(e.SkillId, e.Index, true, CurrentWeapon.SkillDict[e.SkillId]);
    }

    public void OnMMEvent(AddNewAbilityEvent e)
    {
        if (CurrentWeapon.WeaponSkillDict[AvailableSkillType.Ability].Count < LIMIT.MaxAbilityCount)
        {
            CurrentWeapon.WeaponSkillDict[AvailableSkillType.Ability].Add(e.SkillId);
            CurrentWeapon.RefreshAvailableSkills();
            NewAbilityCallbackEvent.Trigger(e.SkillId, CurrentWeapon.WeaponSkillDict[AvailableSkillType.Ability].Count - 1, true, CurrentWeapon.SkillDict[e.SkillId]);
        }
        else
        {
            NewAbilityCallbackEvent.Trigger(e.SkillId, e.Index, false, CurrentWeapon.SkillDict[e.SkillId]);
        }
    }
    public void OnMMEvent(ParrySuccessEvent e)
    {
        SpawnVFXEvent.Trigger(AttackPoint, "PARRY_SUCCESS", new VFXInfo(new Vector3(0, 0.2f, 0.5f), Quaternion.identity, Vector3.one, false, false));
        _animationStateMachine.CurrentState.ToggleInterruption(true);
        Stamina.GainStamina(Stats.ParryStaminaCost/2);
    }

    public void OnMMEvent(CurrencyEarnedEvent e)
    {
        if (e.CurrencyType == CurrencyType.Gold)
        {
            PlayerStats.GoldCount += e.Amount;
        }
        else if (e.CurrencyType == CurrencyType.SoulShard)
        {
            PlayerStats.SoulShardCount += e.Amount;
        }
    }

    public void OnMMEvent(LevelTransitionEvent e)
    {
        if(e.Type == EventStateType.OnEventCompleted)
        {
            if(_animationStateMachine.IsInDeathState())
            {
                _animationStateMachine.InterruptState(AnimationStateType.Revive);
            }
        }
    }

    private void HandleRotation()
    {
        if (Mathf.Approximately(Time.deltaTime, 0)) return;
        if (!CanRotate) return;
        

        if(!_isInAimMode)
        {
            if (GetMoveDir() == Vector3.zero) return;
            Movement.SetLookDirection(GetMoveDir());
        } 
        else
        {
            Movement.SetLookPosition(GetAimPoint());
        }  
    }

    private Vector3 GetMoveDir()
    {
        if (_forward == Vector3.zero)
        {
            _forward = transform.forward;
            _forward.y = 0;
            _forward.Normalize();
        }


        Vector3 right = new Vector3(_forward.z, 0, -_forward.x);
        if (_input.currentControlScheme == "Keyboard&Mouse")
        {

            Vector3 aimDir = CameraUtil.GetSnappedDir(InputProcessor.InputVector, Camera.main, 8);
            return aimDir;
        }
        else if (_input.currentControlScheme == "Gamepad")
        {
            if (InputProcessor.InputVector.sqrMagnitude > .01f)
            {
                Vector3 aimDir = CameraUtil.GetSnappedDir(InputProcessor.InputVector, Camera.main, 8);
                return aimDir;
            }
        }
        return Vector3.zero;
    }

    public Vector3 GetAimPoint()
    {
        if (_forward == Vector3.zero)
        {
            _forward = transform.forward;
            _forward.y = 0;
            _forward.Normalize();
        }


        Vector3 right = new Vector3(_forward.z, 0, -_forward.x);
        if (_input.currentControlScheme == "Keyboard&Mouse")
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane plane = new Plane(Vector3.up, transform.position);
            if (plane.Raycast(mouseRay, out float planeDistance))
            {
                
                _aimPoint = mouseRay.GetPoint(planeDistance);
                _aimPoint.y = transform.position.y;
                Vector3 direction = (_aimPoint - transform.position).normalized;
                Vector3 playerAimForward = transform.position + direction;
                if(FindClosestEnemyToPosition(playerAimForward, _aimAssistDistance, out Vector3 aimPoint))
                {
                    return aimPoint;
                }
                return _aimPoint;
            }

        }
        else if (_input.currentControlScheme == "Gamepad")
        {
            if (InputProcessor.InputVector.sqrMagnitude > .01f)
            {
                Vector3 aimDir = CameraUtil.GetSnappedDir(InputProcessor.InputVector, Camera.main, 8);
                return aimDir;
            }
        }
        return Vector3.zero;
    }

    private bool FindClosestEnemyToPosition(Vector3 position, float maxDistance, out Vector3 pos)
    {
        int hitCount = Physics.OverlapSphereNonAlloc(
            position,
            maxDistance,
            _enemyOverlapBuffer,
            _attackableMask,
            QueryTriggerInteraction.Ignore
        );

        if (hitCount == 0)
        {
            pos = position;
            return false;
        }

        EnemyController closestEnemy = null;
        float maxDistanceSqr = maxDistance * maxDistance;
        float closestSqr = maxDistanceSqr;

        for (int i = 0; i < hitCount; i++)
        {
            var col = _enemyOverlapBuffer[i];
            if (!col) continue;

            if (!col.TryGetComponent<EnemyController>(out var enemy))
                continue;

            Vector3 toEnemy = enemy.transform.position - position;
            float sqr = toEnemy.sqrMagnitude;

            if (sqr < closestSqr)
            {
                closestSqr = sqr;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy == null)
        {
            pos = position;   // no valid enemy found, despite colliders in range
            return false;
        }

        pos = new Vector3(closestEnemy.transform.position.x, position.y, closestEnemy.transform.position.z);
        return true;
    }


    public void InputMovement(InputAction.CallbackContext context)
    {
        // Always process the input vector, regardless of action availability
        // This ensures we're always capturing the latest input
        Vector2 inputValue = context.ReadValue<Vector2>();

        // Store the input in the InputProcessor
        InputProcessor.ProcessInputVector(inputValue);


    }

    public void InputRotate(InputAction.CallbackContext context)
    {

        Vector2 inputValue = context.ReadValue<Vector2>();

        RotateInput = inputValue;
    }

    public void InputAttack(InputAction.CallbackContext context)
    {
        if (context.started && IsActionAvailable(AnimationStateType.Attack))
        { 
            _isInAimMode = true;
            ExecuteLightAttack(GetAimPoint());
            UpdateCurrentSkill(CurrentWeapon.LoopBasicAttack());
            if (_currentSkill != null)
            {
                SpawnVFXEvent.Trigger(transform, _currentSkill.AnimationID, _currentSkill.VFXInfo);
                AnimationStateMachine.SetAction(CurrentWeapon.GetAnimationClip(_currentSkill.AnimationID), AnimationStateType.Attack, _currentSkill);
                AnimationStateMachine.InterruptState(AnimationStateType.Attack);
            }

            Movement.Stop();
        }
        else if(context.canceled)
        {
            _isInAimMode = false;
        }
    }

    public void InputParry(InputAction.CallbackContext context)
    {
        if (context.started && IsActionAvailable(AnimationStateType.Parry))
        {
            if (Stamina.ConsumeStamina(Stats.ParryStaminaCost))
            {
                _isInAimMode = true;
                Parry(GetAimPoint());
                UpdateCurrentSkill(CurrentWeapon.GetParrySkill());
                AnimationStateMachine.SetAction(CurrentWeapon.GetAnimationClip(_currentSkill.AnimationID), AnimationStateType.Parry, _currentSkill);
                AnimationStateMachine.InterruptState(AnimationStateType.Parry);
                Movement.Stop();
            }
        }else if(context.canceled)
        {
            _isInAimMode = false;
        }
    }

    public void InputDash(InputAction.CallbackContext context)
    {
        if (context.started && IsActionAvailable(AnimationStateType.Dash))
        {

            if (Movement.IsGrounded && Stamina.ConsumeStamina(Stats.DashStaminaCost))
            {
                _playerSoundController.PlayDashSFX();
                Movement.Dash(InputProcessor.RawInputVector != Vector2.zero ? CameraUtil.GetSnappedDir(InputProcessor.RawInputVector, Camera.main, 8) : GetMoveDir(), Stats.DashDistance);
                StartIframe();
                UpdateCurrentSkill(CurrentWeapon.GetDashSkill());
                AnimationStateMachine.SetAction(CurrentWeapon.GetAnimationClip(_currentSkill.AnimationID), AnimationStateType.Dash, _currentSkill);
                AnimationStateMachine.InterruptState(AnimationStateType.Dash);

            }

        }
    }

    public void InputUseAbility(InputAction.CallbackContext context)
    {

        if (context.performed && !CurrentWeapon.IsCurrentAbilityInCooldown())
        {
            AnimationStateMachine.SwitchState(AnimationStateType.Ready);
            _isInAimMode = true;
            
        }
        else if (context.canceled && AnimationStateMachine.IsInReadyActionState())
        {
            _isInAimMode = false;
            if (CurrentAbility == null) return;

            CurrentWeapon.StartAbilityCooldown();
            UpdateCurrentSkill(CurrentAbility);
            AnimationStateMachine.SetAction(CurrentWeapon.GetAnimationClip(CurrentAbility.AnimationID), AnimationStateType.Ability, _currentSkill);
            AnimationStateMachine.SwitchState(AnimationStateType.Ability);
            OnAbilityStartCooldown.Invoke(CurrentAbility.Cooldown);

        }
    }

    public void InputAbilityCycleUpward(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            CurrentWeapon.UpdateAbilityIndex(true);
            CurrentAbility = CurrentWeapon.GetAbility();
            OnAbilityCycled.Invoke();
        }
    }


    public void InputAbilityCycleDownward(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            CurrentWeapon.UpdateAbilityIndex(false);
            CurrentAbility = CurrentWeapon.GetAbility();
            OnAbilityCycled.Invoke();
        }
    }

    public void InputExecution(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (Energy.IsFull)
            {
                TryExecution();
            }
        }
    }

    private void TryExecution()
    {
        // TODO: Muso Algorithm
        _currentSkill = CurrentWeapon.GetExecutionSkill();
        AnimationStateMachine.SetAction(CurrentWeapon.GetAnimationClip(_currentSkill.AnimationID), AnimationStateType.Execution, _currentSkill);
        AnimationStateMachine.InterruptState(AnimationStateType.Execution);
        Movement.Stop();
        Energy.Execute(); // depletes energy
    }

    public void InputPauseUnPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (Time.timeScale > .5f)
            {
                //Pause
                Time.timeScale = 0;
            }
            else
            {
                //Resume
                Time.timeScale = 1;
            }
        }
    }


    public void SetActionAvailable(PlayerActionType actionType, bool available)
    {
        _availableActions[actionType] = available;
    }

    private bool IsActionAvailable(AnimationStateType stateType)
    {
        return AnimationStateMachine.CanEnter(stateType);
    }

    public void ExecuteLightAttack(Vector3 aimPosition)
    {
        Movement.SetLookPosition(aimPosition);

    }

    public void Parry(Vector3 aimPosition)
    {
        //Debug.Log($"CanDamage: {_player.Health.CanDamage}");

        Movement.SetLookPosition(aimPosition);
        _isPerfectParryWindowActive = true;
        ParryInitSoundEvent.Trigger();
    }
    
    private void DetectParry()
    {
        // get all colliders in arc
        if (AttackPoint != null)
        {
            _hitTargets = AOEApplier.GetDamagedEntities(_currentSkill.SkillRange.AreaType, AttackPoint.position, _parryMask);
        }
        else
        {
            _hitTargets = AOEApplier.GetDamagedEntities(_currentSkill.SkillRange.AreaType, transform.position, _parryMask);
        }

        if (_hitTargets.Count > 0)
        {
            Energy.GainEnergy(Stats.ParryEnergyGain * Stats.ResourceGainMultiplier);
        }

        for (int i = 0; i < _hitTargets.Count; i++)
        {
            _hitTargets[i].GetComponent<ParryCollider>().OnParry(Stats.RegulerStunDuration); // TODO: Add Stats regarding parry and stagger
        }


    }

    private void UpdateCurrentSkill(Skill skill)
    {
        _currentSkill = skill;
        ApplySkillEffect();
    }

    public EnemyController FindClosestEnemyToPosition(Vector3 position, float maxDistance)
    {
        // Find all enemies in scene within the attack layer
        Collider[] colliders = Physics.OverlapSphere(position, maxDistance, _attackableMask);

        EnemyController closestEnemy = null;
        float closestDistance = maxDistance;

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<EnemyController>(out var enemy))
            {
                float distance = Vector3.Distance(position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }

        return closestEnemy;
    }
    public void StopParryAnimEvent()
    {
        _isPerfectParryWindowActive = false;
    }
    
    public void OnProjectileDeflected(BaseProjectile projectile)
    {
        Energy.GainEnergy(Stats.ParryEnergyGain * Stats.ResourceGainMultiplier);
        
        ParrySuccessEvent.Trigger();
        
        CameraShakeEvent.Trigger(new LightShake());
        
    }


    private void StartIframe()
    {
        _iframeTween.Stop();
        Health.IsDamageable = false;
        _iframeTween = Tween.Delay(Stats.IframeDuration).OnComplete(() => Health.IsDamageable = true);
        Health.OnIframe.Invoke(Stats.IframeDuration);
    }

    public void EndParry()
    {
        _isPerfectParryWindowActive = false;
    }

    public float GetKnockbackForce()
    {
        return _attackKnockbackForce;
    }

    public bool IsValid()
    {
        return Health.IsAlive;
    }

    public void SetCurrentAbility(string skillId)
    {
        CurrentAbility = CurrentWeapon.GetSkill(skillId);
    }

    [Button]
    public override void ApplyStats()
    {
        if (Stats == null) return;
        Vision.ApplyStats(Stats);
        Health.ApplyStats(Stats);
        Energy.ApplyStats(Stats);
        Stamina.ApplyStats(Stats);
    }

    public override void OnStatsUpdated()
    {
        base.OnStatsUpdated();
        Energy.ApplyStats(Stats);
        Stamina.ApplyStats(Stats);
    }

    public void StartNewSession()
    {
        IsNewSession = true;
    }

    public void Reset()
    {
        Stats.Clear();
        ApplyStats();
        CurrentWeapon.ResetSkills();
        _matController.ResetDissolve();
    }

    public override void Stun(float duration, Action onComplete = null, bool forceStun = false)
    {
        if (IsStunImmune && !forceStun) return;
        
        _animationStateMachine.InterruptState(AnimationStateType.Stagger);
        Tween.Delay(duration).OnComplete(() =>
        {
            _animationStateMachine.SwitchState(AnimationStateType.Idle);
            _input.SwitchCurrentActionMap("Player");
            if(onComplete != null)
            {
                onComplete.Invoke();
            }
        });
    }

    public override void StartStunAnimEvent()
    {
        _input.SwitchCurrentActionMap("UI");
    }

    public void PortalTrigger(SplineContainer container, Vector3 exitPos, float duration = 1f)
    {
        ToggleKillzEvent.Trigger(false);
        _splineAnimate.Container = container;
        _splineAnimate.NormalizedTime = 0;
        _splineAnimate.Duration = duration;
        _splineAnimate.Play();
        _playerMesh.SetActive(false);
        _teleportEffect.Play();
        Tween.Delay(_splineAnimate.Duration).OnComplete(() =>
        {
            _playerMesh.SetActive(true);
            _teleportEffect.Stop();
            Movement.Teleport(exitPos);
            ToggleKillzEvent.Trigger(true);
        });
    }

    public void ToggleAimMode(bool toggle)
    {
        _isInAimMode = toggle;
    }

    public override void DamageAnimEvent(string animationID)
    {
        if (_animationStateMachine.IsInStaggerState()) return;

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
            if(!Health.IsDamageable)
            {
                health.Damage(info, true);
            }
            else
            {
                health.Damage(info);
            }
            
        }

        if (_hitTargets.Count > 0)
        {
            HitStop.Begin(AnimationStateMachine.CurrentState.AnimancerState, _hitStopDuration, _hitStopTween);
            CameraShakeEvent.Trigger(new LightShake());
        }
    }
}
