using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class BaseProjectile : MonoBehaviour, IProjectile
{
    [Header("Events")]
    [Tooltip("Called when projectile hits a target")]
    public UnityEvent<GameObject> OnTargetHit;
    
    [Tooltip("Called when projectile is destroyed")]
    public UnityEvent OnProjectileDestroyed;

    // Interface Properties
    public bool IsActive { get; private set; }
    public GameObject Owner { get; private set; }

    // Private fields
    [SerializeField] private ProjectileData _data;
    private Vector3 _direction;
    private Vector3 _velocity;
    private float _distanceTraveled;
    private float _timeAlive;
    private int _hitCount;
    private int _bounceCount;
    
    private Rigidbody _rigidbody;
    private Collider _collider;
    private Vector3 _startPosition;
    private ObjectPool _pool;

    #region Unity Lifecycle

    private void Awake()
    {
        _pool = GetComponentInParent<ObjectPool>();
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

    public void Initialize(ProjectileData data)
    {
        _data = data;
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

    #endregion

    #region Static Factory Methods (Direct Approach)

    /// <summary>
    /// Create and fire a projectile directly - Basic version
    /// </summary>
    // public static BaseProjectile CreateAndFire(GameObject prefab, Vector3 position, Vector3 direction, GameObject owner)
    // {
    //     // Create basic projectile data
    //     ProjectileData data = ProjectileData.CreateBasic(10f, 25f, 0, -1);
    //     return CreateAndFire(prefab, position, direction, owner, data);
    // }

    /// <summary>
    /// Create and fire a projectile directly - Full version with ObjectPool integration
    /// </summary>
    // public static BaseProjectile CreateAndFire(GameObject prefab, Vector3 position, Vector3 direction, GameObject owner, ProjectileData data)
    // {
    //     if (prefab == null)
    //     {
    //         Debug.LogError("Cannot create projectile: prefab is null");
    //         return null;
    //     }

    //     GameObject projectileObj = null;
    //     BaseProjectile projectile = null;

    //     // Try to use ObjectPool first
    //     if (ObjectPool.Instance != null)
    //     {
    //         projectileObj = ObjectPool.Instance.Get(prefab);
    //         if (projectileObj != null)
    //         {
    //             projectile = projectileObj.GetComponent<BaseProjectile>();
    //             if (projectile != null)
    //             {
    //                 // Reset position and rotation for pooled object
    //                 projectileObj.transform.position = position;
    //                 projectileObj.transform.rotation = Quaternion.LookRotation(direction);
                    
    //                 // Setup and launch pooled projectile
    //                 projectile.SetOwner(owner);
    //                 projectile.Initialize(data);
    //                 projectile.SetPosition(position);
    //                 projectile.LaunchInDirection(direction);
    //                 return projectile;
    //             }
    //         }
    //     }

        // Fallback to direct instantiation if ObjectPool failed or not available
    //     projectileObj = Instantiate(prefab, position, Quaternion.LookRotation(direction));
    //     projectile = projectileObj.GetComponent<BaseProjectile>();

    //     if (projectile == null)
    //     {
    //         Debug.LogError($"Prefab '{prefab.name}' does not have BaseProjectile component!");
    //         Destroy(projectileObj);
    //         return null;
    //     }

    //     // Setup and launch instantiated projectile
    //     projectile.SetOwner(owner);
    //     projectile.Initialize(data);
    //     projectile.SetPosition(position);
    //     projectile.LaunchInDirection(direction);

    //     return projectile;
    // }

    /// <summary>
    /// Create and fire projectile towards a target
    /// </summary>
    // public static BaseProjectile CreateAndFireAt(GameObject prefab, Vector3 position, Vector3 targetPosition, GameObject owner, ProjectileData data)
    // {
    //     Vector3 direction = (targetPosition - position).normalized;
    //     return CreateAndFire(prefab, position, direction, owner, data);
    // }

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

    private void HandleCollision(Collider other)
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

    private void HandleTargetHit(GameObject target)
    {
        // Create damage info
        DamageInfo damageInfo = new DamageInfo(
            _data.damage,
            target,
            gameObject,
            Owner,
            _data.damageType
        );

        // Try to apply damage if target has a Health component
        Health targetHealth = target.GetComponent<Health>();
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

        // Draw velocity vector
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, _velocity.normalized * 2f);
        
        // Draw remaining range
        Gizmos.color = Color.yellow;
        float remainingRange = _data.maxRange - _distanceTraveled;
        Vector3 endPosition = transform.position + _direction * remainingRange;
        Gizmos.DrawWireSphere(endPosition, 0.5f);
    }

    #endregion
}
