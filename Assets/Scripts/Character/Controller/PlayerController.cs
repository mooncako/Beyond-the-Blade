using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.VFX;
using TMPro;
using Unity.Cinemachine;
using UnityUtils;
using Animancer;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CustomCharacterMovement))]
public class PlayerController : Controller, MMEventListener<PlayerAnimationStateChangeEvent>, MMEventListener<LevelRandomizeCompleteEvent>
{
    [field: SerializeField, FoldoutGroup("Base Reference")] private PlayerInput _input;
    [field: SerializeField, FoldoutGroup("Base Reference")] private BezierLine _bezierLine;
    [field: SerializeField, FoldoutGroup("Base Reference")] private LineRenderer _lineRenderer;
    [field: SerializeField, FoldoutGroup("Base Reference")] private Collider _weaponCollider;
    [Header("General Settings")]
    [SerializeField] private bool _isTutorial = false;
    [BoxGroup("Input")] public InputProcessor InputProcessor;
    [BoxGroup("Input"), ReadOnly] public Vector2 RotateInput { get; set; }
    [BoxGroup("Input")] public PlayerStateMachine StateMachine { get; private set; }
    [BoxGroup("Input")] public StateCollection States { get; private set; }
    [BoxGroup("Input"), ReadOnly] public PlayerStateType CurrentState { get; private set; }
    [BoxGroup("Input"), ReadOnly] public bool CanRotate = true;
    [BoxGroup("Input"), ReadOnly] private Vector3 _aimPoint;
    private Dictionary<PlayerActionType, bool> _availableActions = new Dictionary<PlayerActionType, bool>();

    [Header("VFX")]
    [FoldoutGroup("Slash")] [SerializeField] private GameObject[] _slashVFXArray;
    [FoldoutGroup("Slash")] [SerializeField] private Transform _slashref;
    [SerializeField] private float _slashVFXDuration = 0.12f;
    [SerializeField] private VisualEffect _musoVFX;
    [SerializeField] private GameObject _parryVFXPrefab;
    [SerializeField] private float _parryVFXDuration = 1.5f;
    // [SerializeField] private ParryHit _parryHitVFX;

    [Header("Animancer")]
    [SerializeField] private AnimancerComponent _animancerComponent;


#if UNITY_EDITOR
    [Header("Current State")]
    [DisplayAsString, HideLabel, ShowInInspector] public string PlayerCurrentState => StateMachine?.CurrentState.ToString() ?? "None";

    public Vector3 Position => throw new System.NotImplementedException();
#endif
    private EventInstance _attackInstance;
    private EventInstance _parryInstance;
    private Vector3 _forward;
    private HashSet<int> _hitEnemiesThisAttack = new HashSet<int>();
    private Vector3 _parryDirection;
    private HashSet<int> _processedParryColliders = new HashSet<int>();
    private bool _isPerfectParryWindowActive = false;
    public bool MusoReady { get; private set; }
    [SerializeField] private EnemyController _musoTarget;

    protected override void OnValidate()
    {
        base.OnValidate();
        if (_input == null) _input = GetComponent<PlayerInput>();
        if ((_attackableMask & (1 << 8)) == 0)
        {
            _attackableMask |= 1 << 8;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        StateMachine = new PlayerStateMachine();
        States = new StateCollection(this, StateMachine);
        InputProcessor = new InputProcessor();

        foreach (PlayerActionType actionType in System.Enum.GetValues(typeof(PlayerActionType)))
        {
            _availableActions[actionType] = true;
        }
        
    }

    void Start()
    {
        StateMachine.Initialize(States.IdleState);
        _animationStateMachine = GetComponent<AnimationStateMachine>();
        _animancerComponent = GetComponent<AnimancerComponent>();
    }

    protected override void Update()
    {
        base.Update();
        HandleRotation();
        StateMachine.CurrentState.Update();
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
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.MMEventStopListening<PlayerAnimationStateChangeEvent>();
        this.MMEventStopListening<LevelRandomizeCompleteEvent>();
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

    private Vector3 GetAimPoint()
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
        InputProcessor.SetInputActive(_animationStateMachine.IsMovable());
        // if (!_animationStatemachine.IsInActionState())
        //     _animationStatemachine.SwitchState(AnimationStateType.Move); 

    }

    public void InputRotate(InputAction.CallbackContext context)
    {

        Vector2 inputValue = context.ReadValue<Vector2>();

        RotateInput = inputValue;
    }

    public void InputAttack(InputAction.CallbackContext context)
    {
        if (context.started && IsActionAvailable(PlayerActionType.Attack))
        {
            ExecuteLightAttack(GetAimPoint());
            _currentSkill = CurrentWeapon.LoopBasicAttack();
            _animationStateMachine.SetActionStateClip(CurrentWeapon.GetAnimationClip(_currentSkill.AnimationID), AnimationStateType.Attack);
            _animationStateMachine.InterruptState(AnimationStateType.Attack);
        }
        else
        {
            
        }
    }
    public void InputParry(InputAction.CallbackContext context)
    {
        if (context.started && IsActionAvailable(PlayerActionType.Parry))
        {
            Parry(GetAimPoint());
            _currentSkill = CurrentWeapon.GetRandomParrySkill();
            _animationStateMachine.SetActionStateClip(CurrentWeapon.GetAnimationClip(_currentSkill.AnimationID), AnimationStateType.Parry);
            _animationStateMachine.InterruptState(AnimationStateType.Parry);
        }
        else
        {

        }
    }

    public void InputDash(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if(Movement.IsGrounded)
                Movement.Dash(Stats.DashForce);
        }
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

    private bool IsActionAvailable(PlayerActionType actionType)
    {
        return _availableActions.TryGetValue(actionType, out bool available) && available;
    }

    public void TryLightAttack(Vector3 aimPosition)
    {
        // TryRotate();
        if (Time.timeScale > 0)
        {
            // DisableMovementRotationAnimEvent();
        }
        {
            SetActionAvailable(PlayerActionType.Move, false);
            ExecuteLightAttack(aimPosition);
        }
    }

        public void ExecuteLightAttack(Vector3 aimPosition)
    {
        // _movement.Dash(_movement.LookDirection, 10f);
        // _lastAttackTime = Time.time;
        // Movement.SetLookPosition(aimPosition);
        // _animator.SetLayerWeight(1, 0); //set lower body layer mask to 0
    }
    
    // private void CheckMuso()
    // {
    //     _currentMusoStack++;
    //     EventHub.Instance.OnMusoChargeIncreased.Invoke(_currentMusoStack);
    //     FMODUnity.RuntimeManager.PlayOneShot(_musoChargeGainedSFX, transform.position);

    //     if (_currentMusoStack == _musoThreshold)
    //     {
    //         _currentMusoStack = 0;
    //         StartMuso();
    //     }
    // }

    // public void MusoAttack(Vector3 aimPosition)
    // {
    //     // _player.TryRotate();
    //     _animator.SetTrigger("Muso");
    //     _lastAttackTime = Time.time;

    //     // Find closest enemy to aim position
    //     // EnemyController targetEnemy = FindClosestEnemyToPosition(aimPosition, _attackRange * 2f);

    //     // if (targetEnemy != null)
    //     {
    //         // Look at the target
    //         Vector3 lookDirection = targetEnemy.transform.position - transform.position;
    //         lookDirection.y = 0;
    //         transform.rotation = Quaternion.LookRotation(lookDirection);
    //         FMODUnity.RuntimeManager.PlayOneShot(_musoAttackSFX, transform.position);
    //         //CreateMusoEffect(aimPosition);
    //         BeginHitStop(2);
    //         targetEnemy.Health.Damage(new DamageInfo(1, targetEnemy.gameObject, gameObject, DamageType.Core));
    //         _player.Movement.Teleport(targetEnemy.MusoPosition.position);
    //     }
    //     EventHub.Instance.OnMusoHit.Invoke();
    //     EndMuso();
    //     if(!_isTutorial)
    //         StopCoroutine(_musoTimerCO);
    // }

    // public void MisoAttack(Vector3 aimPosition)
    // {
    //     MusoAttack(aimPosition);
    // }

    // public void EndMiso()
    // {
    //     EndMuso();
    // }

    public void CleanUpLightAttack()
    {
        _weaponCollider.enabled = false;
        _hitEnemiesThisAttack.Clear();
    }

    public void Parry(Vector3 aimPosition)
    {
        //Debug.Log($"CanDamage: {_player.Health.CanDamage}");

        // Movement.SetLookPosition(aimPosition);
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

        for (int i = 0; i < _hitTargets.Count; i++)
        {
            _hitTargets[i].GetComponent<ParryCollider>().OnParry(.5f); // TODO: Add Stats regarding parry and stagger
        }
    }

    private void HandlePerfectParry(EnemyController enemy, Transform hitTransform)
    {
        //Debug.Log("Perfect Parry");
        // BeginHitStop(1);
        // enemy.Stagger();
        // StartIframe();
        // if (_parryVFXPrefab != null)
        // {
        //     GameObject vfxInstance = Instantiate
        //     (_parryVFXPrefab, _weapon.transform.position, Quaternion.LookRotation(transform.position - _weapon.transform.position));
        //     Destroy(vfxInstance, _parryVFXDuration);
        // }
        // _parryInstance = FMODUnity.RuntimeManager.CreateInstance(_perfectParrySFX);
        // int randomIndex = Random.Range(0, 2);
        // _parryInstance.setParameterByName("Perfect", randomIndex);
        RuntimeManager.AttachInstanceToGameObject(_parryInstance, gameObject, GetComponent<Rigidbody>());
        _parryInstance.start();
        _parryInstance.release();
    }

    // public void PlayerStagger()
    // {
    //     StartCoroutine(PlayerStaggerCO());
    // }

    // private IEnumerator PlayerStaggerCO()
    // {
    //     IsStaggered = true;
    //     _animator.SetBool("IsStaggered", true);
    //     //Debug.Log("Staggered");
    //     yield return new WaitForSeconds(0.25f);
    //     //Debug.Log("Recovered");
    //     IsStaggered = false;
    //     _animator.SetBool("IsStaggered", false);
    // }

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

    // private IEnumerator MusoTimerCO()
    // {
    //     yield return new WaitForSeconds(_musoReadyDuration);
    //     if (_musoTarget != null)
    //         _musoTarget.MaterialController.UnHightlight();
    //     EndMuso();
    //     FMODUnity.RuntimeManager.PlayOneShot(_musoExitSFX, transform.position);
    // }

    public void EndParry()
    {
        _isPerfectParryWindowActive = false;
    }

    // public void StartMuso()
    // {
    //     EventHub.Instance.OnMusoStart.Invoke();
    //     _animator.SetBool("MusoReady", true);
    //     MusoReady = true;
    //     _lineRenderer.enabled = true;
    //     FMODUnity.RuntimeManager.PlayOneShot(_musoActivatedSFX, transform.position);
    //     if (!_isTutorial)
    //     {
    //         _musoTimerCO = MusoTimerCO();
    //         StartCoroutine(_musoTimerCO);
    //     }
            
    // }

    // public void EndMuso()
    // {
    //     EventHub.Instance.OnMusoEnd.Invoke();
    //     MusoReady = false;
    //     _animator.SetBool("MusoReady", false);
    //     _lineRenderer.enabled = false;
    //     FMODUnity.RuntimeManager.PlayOneShot(_musoExitSFX, transform.position);
    // }   

    public float GetKnockbackForce()
    {
        return _attackKnockbackForce;
    }

    public bool IsValid()
    {
        return Health.IsAlive;
    }
}
