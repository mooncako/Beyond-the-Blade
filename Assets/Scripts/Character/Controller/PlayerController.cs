using System.Collections.Generic;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.VFX;
using Animancer;
using PrimeTween;
using UnityEditor.Rendering.LookDev;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CustomCharacterMovement))]
public class PlayerController : Controller, MMEventListener<PlayerAnimationStateChangeEvent>, MMEventListener<LevelRandomizeCompleteEvent>, MMEventListener<SkillSwapEvent>
{
    [field: SerializeField, FoldoutGroup("Base Reference")] private PlayerInput _input;
    [field: SerializeField, FoldoutGroup("Base Reference")] private BezierLine _bezierLine;
    [field: SerializeField, FoldoutGroup("Base Reference")] private LineRenderer _lineRenderer;
    [field: SerializeField, FoldoutGroup("Base Reference")] private Collider _weaponCollider;
    [field: SerializeField, FoldoutGroup("Base Reference")] public Energy Energy;
    [Header("General Settings")]
    [SerializeField] private bool _isTutorial = false;
    [BoxGroup("Input")] public InputProcessor InputProcessor;
    [BoxGroup("Input"), ReadOnly] public Vector2 RotateInput { get; set; }
    [BoxGroup("Input"), ReadOnly] public PlayerStateType CurrentState { get; private set; }
    [BoxGroup("Input"), ReadOnly] public bool CanRotate = true;
    [BoxGroup("Input"), ReadOnly] private Vector3 _aimPoint;
    private Dictionary<PlayerActionType, bool> _availableActions = new Dictionary<PlayerActionType, bool>();
    [BoxGroup("Ability"), ReadOnly] public Skill CurrentAbility { get; private set; }
    [BoxGroup("Ability"), ReadOnly] private bool _abilityInCooldown;
    [BoxGroup("Ability"), ReadOnly] public UnityEvent OnAbilityStartCooldown;

    [Header("VFX")]
    [FoldoutGroup("Slash")][SerializeField] private GameObject[] _slashVFXArray;
    [FoldoutGroup("Slash")][SerializeField] private Transform _slashref;
    [SerializeField] private float _slashVFXDuration = 0.12f;
    [SerializeField] private VisualEffect _musoVFX;
    [SerializeField] private GameObject _parryVFXPrefab;
    [SerializeField] private float _parryVFXDuration = 1.5f;
    // [SerializeField] private ParryHit _parryHitVFX;

    [Header("Animancer")]
    [SerializeField] private AnimancerComponent _animancerComponent;


    private Vector3 _forward;
    private HashSet<int> _hitEnemiesThisAttack = new HashSet<int>();
    private bool _isPerfectParryWindowActive = false;
    public bool MusoReady { get; private set; }
    [SerializeField] private EnemyController _musoTarget;

    [HideInInspector] public UnityEvent OnExecutionStarted;

    private Tween _iframeTween;

    protected override void OnValidate()
    {
        base.OnValidate();
        if (_input == null) _input = GetComponent<PlayerInput>();
        if (_animationStateMachine == null) _animationStateMachine = GetComponent<AnimationStateMachine>();
        if (_animancerComponent == null) _animancerComponent = GetComponent<AnimancerComponent>();
        if (Energy == null) Energy = GetComponent<Energy>();
        if ((_attackableMask & (1 << 8)) == 0)
        {
            _attackableMask |= 1 << 8;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        
        InputProcessor = new InputProcessor();

        foreach (PlayerActionType actionType in System.Enum.GetValues(typeof(PlayerActionType)))
        {
            _availableActions[actionType] = true;
        }

    }

    void Start()
    {

        //foreach (string skillId in CurrentWeapon.WeaponSkillSO.SkillDict[2])
        //{
        //    AbilityList.Add(skillId, new PlayableSkill(skillId));
        //}
        if (CurrentAbility == null)
        {
            //foreach (string abilityId in AbilityList.Keys)
            //{
            //    if (AbilityList.ContainsKey(abilityId))
            //        _currentAbility = AbilityList[abilityId];
            //}
            CurrentAbility = CurrentWeapon.GetAbility();
        }


    }

    protected override void Update()
    {
        base.Update();
        HandleRotation();
        InputProcessor.SetInputActive(_animationStateMachine.IsMovable());

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
        if (MusoReady)
        {
            if (_musoTarget != null)
                // _musoTarget.MaterialController.UnHightlight();

                // _musoTarget = FindClosestEnemyToPosition(GetLookDirection(), 100);

                if (_musoTarget == null)
                {
                    _lineRenderer.enabled = false;
                }

            if (_musoTarget != null)
            {
                _lineRenderer.enabled = true;
                // _musoTarget.MaterialController.Highlight();
                _bezierLine.endPoint = _musoTarget.transform;
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.MMEventStartListening<PlayerAnimationStateChangeEvent>();
        this.MMEventStartListening<LevelRandomizeCompleteEvent>();
        this.MMEventStartListening<SkillSwapEvent>();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.MMEventStopListening<PlayerAnimationStateChangeEvent>();
        this.MMEventStopListening<LevelRandomizeCompleteEvent>();
        this.MMEventStopListening<SkillSwapEvent>();
        _iframeTween.Stop();
    }

    public void OnMMEvent(PlayerAnimationStateChangeEvent e)
    {
        CurrentState = e.State;
    }

    public void OnMMEvent(LevelRandomizeCompleteEvent e)
    {
        if (e.State == EventStateType.OnEventEnd)
        {
            Movement.Teleport(e.SpawnPoint.position);
        }
    }
    public void OnMMEvent(SkillSwapEvent e)
    {
        CurrentWeapon.WeaponSkillDict[0][CurrentWeapon.GetAttackSkillIndexWithCooldown(e.Skill.Cooldown)] = e.SkillId;
        CurrentWeapon.RefreshAvailableSkills();
    }

    private void HandleRotation()
    {
        if (Mathf.Approximately(Time.deltaTime, 0)) return;
        if (!CanRotate) return;
        if (GetMoveDir() == Vector3.zero) return;
        Movement.SetLookDirection(GetMoveDir());
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

    public void InputMovement(InputAction.CallbackContext context)
    {
        // Always process the input vector, regardless of action availability
        // This ensures we're always capturing the latest input
        Vector2 inputValue = context.ReadValue<Vector2>();

        // Store the input in the InputProcessor
        InputProcessor.ProcessInputVector(inputValue);

        // Only apply movement if the action is available
        //InputProcessor.SetInputActive(_animationStateMachine.IsMovable());
        //if (!_animationStateMachine.IsInActionState())
        //    _animationStateMachine.SwitchState(AnimationStateType.Move);


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
            ExecuteLightAttack(GetAimPoint());
            _currentSkill = CurrentWeapon.LoopBasicAttack();
            if (_currentSkill != null)
            {
                _animationStateMachine.SetAction(CurrentWeapon.GetAnimationClip(_currentSkill.AnimationID), AnimationStateType.Attack, _currentSkill);
                _animationStateMachine.InterruptState(AnimationStateType.Attack);
            }
            
            Movement.Stop();
        }
        else
        {

        }
    }
    public void InputParry(InputAction.CallbackContext context)
    {
        if (context.started && IsActionAvailable(AnimationStateType.Parry))
        {
            Parry(GetAimPoint());
            _currentSkill = CurrentWeapon.GetRandomParrySkill();
            _animationStateMachine.SetAction(CurrentWeapon.GetAnimationClip(_currentSkill.AnimationID), AnimationStateType.Parry, _currentSkill);
            _animationStateMachine.InterruptState(AnimationStateType.Parry);
            Movement.Stop();
        }
        else
        {

        }
    }

    public void InputDash(InputAction.CallbackContext context)
    {
        if (context.started && IsActionAvailable(AnimationStateType.Dash))
        {
            if (Movement.IsGrounded)
            {

                Movement.Dash(InputProcessor.RawInputVector != Vector2.zero ? CameraUtil.GetSnappedDir(InputProcessor.RawInputVector, Camera.main, 8) : GetMoveDir(), Stats.DashForce);
                StartIframe();

                _animationStateMachine.InterruptState(AnimationStateType.Dash);

            }

        }
    }

    public void InputUseAbility(InputAction.CallbackContext context)
    {
        if (context.started && IsActionAvailable(AnimationStateType.Ability))
        {
            if (CurrentAbility == null) return;
            if (_abilityInCooldown) return;

            _animationStateMachine.SetAction(CurrentWeapon.GetAnimationClip(CurrentAbility.AnimationID), AnimationStateType.Ability, _currentSkill);
            if (_animationStateMachine.InterruptState(AnimationStateType.Ability))
            {
                OnAbilityStartCooldown.Invoke();
                StartCoroutine(AbilityCooldownCo(CurrentAbility.Cooldown));
            }

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
        _animationStateMachine.SetAction(CurrentWeapon.GetAnimationClip(_currentSkill.AnimationID), AnimationStateType.Execution, _currentSkill);
        _animationStateMachine.InterruptState(AnimationStateType.Execution);
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
        return _animationStateMachine.CanEnter(stateType);
    }

    public void ExecuteLightAttack(Vector3 aimPosition)
    {
        // _movement.Dash(_movement.LookDirection, 10f);
        // _lastAttackTime = Time.time;
        Movement.SetLookPosition(aimPosition);
        // _animator.SetLayerWeight(1, 0); //set lower body layer mask to 0
    }


    public void CleanUpLightAttack()
    {
        _weaponCollider.enabled = false;
        _hitEnemiesThisAttack.Clear();
    }

    public void Parry(Vector3 aimPosition)
    {
        //Debug.Log($"CanDamage: {_player.Health.CanDamage}");

        Movement.SetLookPosition(aimPosition);
        _isPerfectParryWindowActive = true;
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
            _hitTargets[i].GetComponent<ParryCollider>().OnParry(Stats.HitStunDuration); // TODO: Add Stats regarding parry and stagger
        }

        
    }

    private EnemyController FindClosestEnemyToPosition(Vector3 position, float maxDistance)
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

    private void CreateMusoEffect(Vector3 aimPosition)
    {
        if (_musoVFX != null)
        {

            float angle = Mathf.Atan2(aimPosition.x, aimPosition.z) * Mathf.Rad2Deg;
            _musoVFX.SetFloat("Rotation", angle);

            _musoVFX.Play();
        }
    }

    private void StartIframe()
    {
        _iframeTween.Stop();
        Health.IsDamageable = false;
        _iframeTween = Tween.Delay(Stats.IframeDuration).OnComplete(() => Health.IsDamageable = true);
        Health.OnIframe.Invoke(Stats.IframeDuration);
    }

    public void SlashEffect(int Index)
    {
        _slashVFXArray[Index].GetComponent<ParticleSystem>().Play();
        StartCoroutine(DisableSlashVFXCO(Index));
    }

    private IEnumerator DisableSlashVFXCO(int index)
    {
        yield return new WaitForSeconds(_slashVFXDuration);
        _slashVFXArray[index].GetComponent<ParticleSystem>().Stop();
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
    private IEnumerator AbilityCooldownCo(float cooldownTime)
    {
        _abilityInCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        _abilityInCooldown = false;
    }

    [Button]
    public override void ApplyStats()
    {
        if (Stats == null) return;
        Vision.ApplyStats(Stats);
        Health.ApplyStats(Stats);
        Energy.ApplyStats(Stats);
    }

    public override void OnStatsUpdated()
    {
        base.OnStatsUpdated();
        Energy.ApplyStats(Stats);
    }

}
