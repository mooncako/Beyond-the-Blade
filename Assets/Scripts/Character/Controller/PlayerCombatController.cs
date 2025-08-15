using UnityEngine;
using System.Collections;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;
using UnityEngine.VFX;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Cinemachine;
using UnityUtils;

[RequireComponent(typeof(CustomCharacterMovement))]
public class PlayerCombatController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator _animator;
    [SerializeField] private CustomCharacterMovement _movement;
    [SerializeField] private PlayerController _player;
    [SerializeField] private GameObject _weapon;
    [SerializeField] private BezierLine _bezierLine;
    [SerializeField] private LineRenderer _lineRenderer;

    [Header("General Settings")]
    [SerializeField] private bool _isTutorial = false;

    [Header("Attack Settings")]
    [SerializeField] private float _attackRate = 3f;
    [SerializeField] private float _attackRange = 3f;
    [SerializeField] private float _chargeDuration = 1.5f;
    [SerializeField] private float _musoReadyDuration = 3f;
    [SerializeField] private int _musoThreshold = 2;
    [SerializeField, ReadOnly] private int _currentMusoStack = 0;
    [SerializeField] private float _attackKnockbackForce = 10f;
    [SerializeField] private LayerMask _attackLayer;
    [Range(0, 1), SerializeField] private float _hitStopDuration = .05f;

    [Header("Weapon Collider")]
    [SerializeField] private Collider _weaponCollider;

    [Header("Parry Settings")]
    [SerializeField] private float _parryRadius = 1.5f;
    [SerializeField] private float _parryAngle = 100f;
    [SerializeField] private LayerMask _parryLayer;

    [Header("VFX")]
    [FoldoutGroup("Slash")] [SerializeField] private GameObject[] _slashVFXArray;
    [FoldoutGroup("Slash")] [SerializeField] private Transform _slashref;
    [SerializeField] private float _slashVFXDuration = 0.12f;
    [SerializeField] private VisualEffect _musoVFX;
    [SerializeField] private GameObject _parryVFXPrefab;
    [SerializeField] private float _parryVFXDuration = 1.5f;
    // [SerializeField] private ParryHit _parryHitVFX;


    private EventInstance _attackInstance;
    private EventInstance _parryInstance;

    [Header("Debug View")]
    [SerializeField] private bool _drawGizmos = true;
    [SerializeField] private Color _arcColor = new Color(1, 0.5f, 0, 0.3f);
    private float _lastAttackTime = Mathf.NegativeInfinity;
    public float ChargeDuration => _chargeDuration;
    public bool IsAttacking { get; private set; }
    public bool IsCharging { get; private set; }
    public bool IsParrying { get; private set; }
    public bool ChargeCompleted { get; private set; }
    public bool IsStaggered { get; private set; }
    public bool MusoReady { get; private set; }
    public bool IsComboWindowOpen { get; private set; }
    private float _chargeStartTime;
    private float _chargeThreshold = 0.5f;
    private bool _isPerfectParryWindowActive = false;
    private bool _isWeakParryWindowActive = false;
    private IEnumerator _musoTimerCO;
    private HashSet<int> _hitEnemiesThisAttack = new HashSet<int>();
    private Vector3 _parryDirection;
    private HashSet<int> _processedParryColliders = new HashSet<int>();
    // [SerializeField] private EnemyController _musoTarget;

    private void Awake()
    {
        _movement = GetComponent<CustomCharacterMovement>();
    }

    void OnValidate()
    {
        if (_animator == null) _animator = GetComponent<Animator>();
        if (_movement == null) _movement = GetComponent<CustomCharacterMovement>();
        if (_player == null) _player = GetComponent<PlayerController>();
    }

    private void Start()
    {

    }

    void OnEnable()
    {

    }

    private void OnDisable()
    {

    }
    private void Update()
    {
     
        // start charge timer
        if (IsCharging)
        {
            _chargeStartTime += Time.deltaTime;
        }
        // // check if attack input is held down long enough for charge
        // if (_chargeStartTime >= _chargeThreshold)
        // {
        //     StartHeavyCharge();
        // }
        
        // if (IsCharging && _chargeStartTime >= ChargeDuration)
        // {
        //     ChargeCompleted = true;
        // }
        // If any parry window is active, check for collisions
        // if (_isPerfectParryWindowActive)
        // {
        //     DetectParryInArc();
        // }

        // if (MusoReady)
        // {
        //     if (_musoTarget != null)
        //         _musoTarget.MaterialController.UnHightlight();

        //     _musoTarget = FindClosestEnemyToPosition(PlayerController.Instance.GetAimPosition(), _attackRange * 2f);

        //     if (_musoTarget == null)
        //     {
        //         _lineRenderer.enabled = false;
        //     }

        //     if (_musoTarget != null)
        //     {
        //         _lineRenderer.enabled = true;
        //         _musoTarget.MaterialController.Highlight();
        //         _bezierLine.endPoint = _musoTarget.transform;
        //     }
        // }
    }

    public void CheckChargeTime(Vector3 aimPosition)
    {
        // if (MusoReady)
        // {
        //     MusoAttack(aimPosition);
        // }
        // else
        {
            TryLightAttack(aimPosition);
        }
    }

    public void TryLightAttack(Vector3 aimPosition)
    {
        // _player.TryRotate();
        if (Time.timeScale > 0)
        {
            // _player.DisableMovementRotationAnimEvent();
        }
        IsCharging = false;
        _chargeStartTime = 0f;
        if (Time.time > _lastAttackTime + 0.166f / _attackRate)
        {
            _player.SetActionAvailable(PlayerActionType.Move, false);
            IsAttacking = true;
            ExecuteLightAttack(aimPosition);
        }
    }

    public void ExecuteLightAttack(Vector3 aimPosition)
    {
        _animator.SetTrigger("Attack");
        _animator.SetBool("IsAttacking", true);
        // _movement.Dash(_movement.LookDirection, 10f);
        _lastAttackTime = Time.time;
        _hitEnemiesThisAttack.Clear();
        // _animator.SetLayerWeight(1, 0); //set lower body layer mask to 0
    }

    // public void OnWeaponHit(Collider other)
    // {
    //     if (!_weaponCollider.enabled == true) return;

    //     // Get ID for enemy hit
    //     int enemyId = other.gameObject.GetInstanceID();

    //     // Check if enemy have been hit in this attack
    //     if (!_hitEnemiesThisAttack.Contains(enemyId))
    //     {
    //         // Add to the hit list
    //         _hitEnemiesThisAttack.Add(enemyId);
    //         if (other.TryGetComponent<EnemyController>(out var enemy))
    //         {
    //             if (enemy.Health.IsDeflecting == true)
    //             {
    //                 PlayerStagger();
    //                 return;
    //             }
    //             BeginHitStop(1);
    //             if(enemy.IsTank)
    //                 enemy.ApplyKnockback(transform.position, _attackKnockbackForce);
    //             enemy.Health.Damage(new DamageInfo(1, enemy.gameObject, gameObject, DamageType.Regular));
    //         }
    //     }
    // }

    // Combo window tracking
    public void OpenComboWindow()
    {
        IsComboWindowOpen = true;
    }

    public void CloseComboWindow()
    {
        IsComboWindowOpen = false;
    }

    public void ActivateWeaponCollider()
    {
        _weaponCollider.enabled = true;
        _hitEnemiesThisAttack.Clear();
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

    public void EndAttack()
    {
        IsAttacking = false;
        _weaponCollider.enabled = false;
        _hitEnemiesThisAttack.Clear();
    }

    public void Parry(Vector3 aimPosition)
    {
        //Debug.Log($"CanDamage: {_player.Health.CanDamage}");
        _animator.SetTrigger("Parry");
        IsParrying = true;
        _parryDirection = (aimPosition - transform.position).normalized;
        _parryDirection.y = 0;
        // Clear recorded colliders
        _processedParryColliders.Clear();
        ResetParryCO();
    }

    // private void DetectParryInArc()
    // {
    //     // get all colliders in arc
    //     Collider[] hits = Physics.OverlapSphere(new Vector3(_player.transform.position.x, _player.transform.position.y + _player.Movement.Height/2, _player.transform.position.z) + _player.transform.forward *
    //     (_parryRadius * .5f), _parryRadius, _parryLayer);

    //     foreach (Collider hit in hits)
    //     {
    //         // if (hit.transform.parent == null)
    //         // {
    //         //     if (hit.gameObject.TryGetComponent(out Fireball fireball))
    //         //     {
    //         //         fireball.OnDeflect();
    //         //     }
    //         //     else if (hit.gameObject.TryGetComponent(out SpearProjectile spear))
    //         //     {
    //         //         spear.OnDeflect();
    //         //     }
    //         // }
    //         // else
    //         {
    //             Vector3 directionToTarget = hit.transform.parent.gameObject.transform.position - _player.transform.position;
    //             directionToTarget.y = 0;

    //             float angleToTarget = Vector3.Angle(_player.transform.forward, directionToTarget);

    //             if (angleToTarget < _parryAngle / 2)
    //             {
    //                 // add collider to the array
    //                 int colliderID = hit.GetInstanceID();
    //                 if (!_processedParryColliders.Contains(colliderID))
    //                 {
    //                     _processedParryColliders.Add(colliderID);
    //                     var enemy = hit.GetComponentInParent<EnemyController>();
    //                     if (enemy != null)
    //                     {
    //                         if (_isPerfectParryWindowActive)
    //                         {
    //                             _animator.SetBool("PerfectParry", true);
    //                             HandlePerfectParry(enemy, hit.transform);
    //                         }
    //                         // else if (_isWeakParryWindowActive)
    //                         // {
    //                         //     _animator.SetBool("WeakParry", true);
    //                         //     HandleWeakParry(enemy, hit.transform);
    //                         // }
    //                     }
    //                 }
    //             }
    //         }
    //     }
    // }

    // private void HandlePerfectParry(EnemyController enemy, Transform hitTransform)
    // {
    //     //Debug.Log("Perfect Parry");
    //     BeginHitStop(1);
    //     enemy.Stagger();
    //     _player.StartIframe();
    //     _player.Animator.SetBool("Deflect", true);
    //     if (_parryVFXPrefab != null)
    //     {
    //         GameObject vfxInstance = Instantiate
    //         (_parryVFXPrefab, _weapon.transform.position, Quaternion.LookRotation(transform.position - _weapon.transform.position));
    //         Destroy(vfxInstance, _parryVFXDuration);
    //     }
    //     _parryInstance = FMODUnity.RuntimeManager.CreateInstance(_perfectParrySFX);
    //     // int randomIndex = Random.Range(0, 2);
    //     // _parryInstance.setParameterByName("Perfect", randomIndex);
    //     RuntimeManager.AttachInstanceToGameObject(_parryInstance, gameObject, GetComponent<Rigidbody>());
    //     _parryInstance.start();
    //     _parryInstance.release();
    //     StartCoroutine(ResetParryCO());
    // }

    public void PlayerStagger()
    {
        StartCoroutine(PlayerStaggerCO());
    }

    private IEnumerator PlayerStaggerCO()
    {
        IsStaggered = true;
        _animator.SetBool("IsStaggered", true);
        //Debug.Log("Staggered");
        yield return new WaitForSeconds(0.25f);
        //Debug.Log("Recovered");
        IsStaggered = false;
        _animator.SetBool("IsStaggered", false);
    }

    public void AnimationEventPerfectParry()
    {
        _isPerfectParryWindowActive = true;
        _isWeakParryWindowActive = false;
    }

    private IEnumerator ResetParryCO()
    {
        yield return new WaitForSeconds(0.2f);
        _animator.SetTrigger("ParryReset");
        _animator.SetBool("IsParrying", false);
        _animator.ResetTrigger("Parry");
        _animator.SetBool("PerfectParry", false);
        IsParrying = false;
        _isPerfectParryWindowActive = false;
        _isWeakParryWindowActive = false;
    }

    // private EnemyController FindClosestEnemyToPosition(Vector3 position, float maxDistance)
    // {
    //     // Find all enemies in scene within the attack layer
    //     Collider[] colliders = Physics.OverlapSphere(position, maxDistance, _attackLayer);

    //     EnemyController closestEnemy = null;
    //     float closestDistance = maxDistance;

    //     foreach (Collider collider in colliders)
    //     {
    //         if (collider.TryGetComponent<EnemyController>(out var enemy))
    //         {
    //             float distance = Vector3.Distance(position, enemy.transform.position);
    //             if (distance < closestDistance)
    //             {
    //                 closestDistance = distance;
    //                 closestEnemy = enemy;
    //             }
    //         }
    //     }

    //     return closestEnemy;
    // }

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

    private void OnDrawGizmosSelected()
    {
        if (!_drawGizmos || !_player.transform) return;

        Gizmos.color = _arcColor;
        Vector3 playerForward = _player.transform.forward;
        Vector3 playerPosition = _player.transform.position;
        for (int i = 0; i <= 20; i++)
        {
            float angle = Mathf.Lerp(-_parryAngle / 2, _parryAngle / 2, i / 20f);
            Vector3 dir = Quaternion.Euler(0, angle, 0) * playerForward * _parryRadius;
            Gizmos.DrawLine(playerPosition, playerPosition + dir);

            if (i > 0)
            {
                Vector3 prevDir = Quaternion.Euler(0, Mathf.Lerp(-_parryAngle / 2, _parryAngle / 2, (i - 1) / 20f), 0) * playerForward * _parryRadius;
                Gizmos.DrawLine(playerPosition + prevDir, playerPosition + dir);
            }
        }

        Gizmos.DrawWireSphere(new Vector3(_player.transform.position.x, _player.transform.position.y + _player.Movement.Height/2, _player.transform.position.z) + _player.transform.forward *
        (_parryRadius * .5f), _parryRadius);
    }

    public void EndParry()
    {
        IsParrying = false;
        _isPerfectParryWindowActive = false;
        _isWeakParryWindowActive = false;
    }

    public void EndStagger()
    {
        IsStaggered = false;
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

    public void BeginHitStop(int damageType)
    {
        HitStop.Begin(_animator, "HitStop");
        if (damageType == 1)
        {
            CameraShakeEvent.Trigger(new LightShake());
        }
        else if (damageType == 2)
        {
            CameraShakeEvent.Trigger(new HeavyShake());
        }
        StartCoroutine(HitStopCO());
    }

    private IEnumerator HitStopCO()
    {
        yield return new WaitForSeconds(_hitStopDuration);
        _animator.SetFloat("HitStop", 1);
    }

   

    public float GetKnockbackForce()
    {
        return _attackKnockbackForce;
    }

    public int GetMusoStack()
    {
        return _musoThreshold - _currentMusoStack;
    }
}
