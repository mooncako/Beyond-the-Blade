using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class BaseProjectile : MonoBehaviour, IProjectile, IPoolable
{
    [Header("Events")]
    [Tooltip("Called when projectile hits a target")]
    public UnityEvent<GameObject> OnTargetHit;
    
    [Tooltip("Called when projectile is destroyed")]
    public UnityEvent OnProjectileDestroyed;
    
    [Tooltip("Called when projectile is deflected")]
    public UnityEvent<GameObject> OnProjectileDeflected;

    // Interface Properties
    public bool IsActive { get; private set; }
    public GameObject Owner { get; private set; }

    public ObjectPool ObjectPool;

    // Private fields
    [SerializeField] protected ProjectileData _data;
    private Vector3 _direction;
    private Vector3 _velocity;
    private float _distanceTraveled;
    private float _timeAlive;
    protected int _hitCount;
    private int _bounceCount;
    
    private Rigidbody _rigidbody;
    protected Collider _collider;
    private Vector3 _startPosition;
    
    [Header("Deflection")]
    [SerializeField] private bool _canBeDeflected = true;
    public bool CanBeDeflected => _canBeDeflected;
    
    private bool _hasBeenDeflected = false;

    #region Unity Lifecycle

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        // Configure rigidbody for projectile movement
        _rigidbody.useGravity = false; // No gravity as requested
        _rigidbody.linearDamping = 0f;
        _rigidbody.angularDamping = 0f;

        // Set collider as trigger for hit detection
        _collider.isTrigger = true;

        IsActive = false;
    }

    private void FixedUpdate()
    {
        if (!IsActive) return;

        UpdateMovement();
        UpdateLifetime();
        CheckRange();
        
        if (_data.rotateInFlight)
        {
            UpdateRotation();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsActive) return;

        HandleCollision(other);
    }

    #endregion

    #region IProjectile Implementation

    public void Initialize()
    {
        _hitCount = 0;
        _bounceCount = 0;
        _distanceTraveled = 0f;
        _timeAlive = 0f;
    }

    public void Activate()
    {
        IsActive = true;
        _startPosition = transform.position;
        _rigidbody.linearVelocity = _velocity;
        
        // Enable collider and rigidbody
        _collider.enabled = true;
        _rigidbody.isKinematic = false;
    }

    public void Deactivate()
    {
        IsActive = false;
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.isKinematic = true;
        _collider.enabled = false;
        
        OnProjectileDestroyed?.Invoke();
    }

    public void SetDirection(Vector3 direction)
    {
        _direction = direction.normalized;
        _velocity = _direction * _data.speed;
        
        // Orient the projectile to face the direction of travel
        if (_direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(_direction);
        }
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
        _startPosition = position;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public Vector3 GetVelocity()
    {
        return _velocity;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Set the owner of this projectile
    /// </summary>
    public void SetOwner(GameObject owner)
    {
        Owner = owner;
    }

    /// <summary>
    /// Launch the projectile towards a target position
    /// </summary>
    public void LaunchTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        SetDirection(direction);
        Activate();
    }

    /// <summary>
    /// Launch the projectile in a specific direction
    /// </summary>
    public void LaunchInDirection(Vector3 direction)
    {
        SetDirection(direction);
        Activate();
    }

    public void Deflect(GameObject deflector, Vector3 deflectDirection)
    {
        if (!_canBeDeflected || _hasBeenDeflected) return;
        
        _hasBeenDeflected = true;
        
        Owner = deflector;
        
        Targetable deflectorTargetable = deflector.GetComponent<Targetable>();
        if (deflectorTargetable != null)
        {
            _data.ownerTeam = deflectorTargetable.Team;
        }
        
        // Redirect projectile in new direction
        SetDirection(deflectDirection);
        
        // Reset hit count so deflected projectile can hit again
        _hitCount = 0;
        
        
        // Trigger deflect event for VFX/audio feedback
        OnProjectileDeflected?.Invoke(deflector);
        
        Debug.Log($"Projectile deflected by {deflector.name}! New direction: {deflectDirection}");
    }

    #endregion

    #region IPoolable Implementation
    public void OnPoolGet()
    {
        _hitCount = 0;
        _bounceCount = 0;
        _distanceTraveled = 0f;
        _timeAlive = 0f;
        _hasBeenDeflected = false; // Reset deflection flag
        IsActive = false;
    }
    public void OnPoolReturn()
    {
        OnProjectileDestroyed?.RemoveAllListeners();
        OnTargetHit?.RemoveAllListeners();
        OnProjectileDeflected?.RemoveAllListeners();

        IsActive = false;
        _rigidbody.linearVelocity = Vector3.zero;
        _hasBeenDeflected = false;
    }
    #endregion

    #region Private Methods

    private void UpdateMovement()
    {
        Vector3 previousPosition = transform.position;
        
        // Update position based on velocity
        transform.position += _velocity * Time.fixedDeltaTime;
        
        // Track distance traveled
        _distanceTraveled += Vector3.Distance(previousPosition, transform.position);
    }

    private void UpdateLifetime()
    {
        _timeAlive += Time.fixedDeltaTime;
        
        if (_timeAlive >= _data.lifetime)
        {
            DestroyProjectile();
        }
    }

    private void CheckRange()
    {
        if (_distanceTraveled >= _data.maxRange)
        {
            DestroyProjectile();
        }
    }

    private void UpdateRotation()
    {
        transform.Rotate(Vector3.forward, _data.rotationSpeed * Time.fixedDeltaTime);
    }

    protected virtual void HandleCollision(Collider other)
    {
        // Check if this is a valid target
        if (IsValidTarget(other))
        {
            HandleTargetHit(other.gameObject);
        }
        // Check if this is a surface to bounce off or collide with
        else if (ShouldCollideWith(other))
        {
            if (_data.canBounce && _bounceCount < _data.maxBounces)
            {
                HandleBounce(other);
            }
            else
            {
                DestroyProjectile();
            }
        }
    }

    private bool IsValidTarget(Collider other)
    {
        // Check if the object is on a valid target layer
        if ((_data.targetLayers.value & (1 << other.gameObject.layer)) == 0)
            return false;

        // Check for Targetable component and team
        Targetable targetable = other.GetComponent<Targetable>();
        if (targetable != null)
        {
            // Don't hit objects on the same team
            if (targetable.Team == _data.ownerTeam)
                return false;
                
            // Don't hit non-targetable objects
            if (!targetable.IsTargetable)
                return false;
        }

        // Don't hit the owner
        if (other.gameObject == Owner)
            return false;

        return true;
    }

    private bool ShouldCollideWith(Collider other)
    {
        return (_data.collisionLayers.value & (1 << other.gameObject.layer)) != 0;
    }

    protected virtual void HandleTargetHit(GameObject target)
    {
        // Create damage info
        

        // Try to apply damage if target has a Health component
        Health targetHealth = target.GetComponent<Health>();

        DamageInfo damageInfo = new DamageInfo(
            _data.damage,
            target,
            targetHealth,
            Owner,
            _data.damageType
        );
        
        if (targetHealth != null)
        {
            // Note: You may need to implement a TakeDamage method on Health component
            // targetHealth.TakeDamage(damageInfo);
        }

        // Trigger hit event
        OnTargetHit?.Invoke(target);

        // Increment hit count
        _hitCount++;

        // Check if projectile should be destroyed after hit
        if (!_data.isPiercing || (_data.maxHits > 0 && _hitCount >= _data.maxHits))
        {
            DestroyProjectile();
        }
    }

    private void HandleBounce(Collider surface)
    {
        // Calculate bounce direction (simplified)
        Vector3 normal = Vector3.up; // Default normal, should be calculated from surface
        
        // You might want to use raycasting to get the exact surface normal
        Vector3 bounceDirection = Vector3.Reflect(_direction, normal);
        
        SetDirection(bounceDirection);
        _bounceCount++;
    }

    protected virtual void DestroyProjectile()
    {
        Deactivate();
        
        // Destroy the GameObject (you might want to use object pooling instead)
        // Destroy(gameObject);
    }

    #endregion


    #region Debug

    private void OnDrawGizmosSelected()
    {
        if (!IsActive) return;

        // Draw velocity vector - red if not deflected, cyan if deflected
        Gizmos.color = _hasBeenDeflected ? Color.cyan : Color.red;
        Gizmos.DrawRay(transform.position, _velocity.normalized * 2f);
        
        // Draw remaining range
        Gizmos.color = Color.yellow;
        float remainingRange = _data.maxRange - _distanceTraveled;
        Vector3 endPosition = transform.position + _direction * remainingRange;
        Gizmos.DrawWireSphere(endPosition, 0.5f);
    }
    
    [Sirenix.OdinInspector.Button("Test Deflect (Forward)"), Sirenix.OdinInspector.BoxGroup("Debug")]
    private void DebugDeflectForward()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Debug deflect only works in Play mode!");
            return;
        }
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Deflect(player, transform.forward);
        }
        else
        {
            Debug.LogError("No player found! Make sure player has 'Player' tag.");
        }
    }
    
    [Sirenix.OdinInspector.Button("Test Deflect (Reverse)"), Sirenix.OdinInspector.BoxGroup("Debug")]
    private void DebugDeflectReverse()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Debug deflect only works in Play mode!");
            return;
        }
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Deflect(player, -_direction);
        }
        
    }

    #endregion
}
