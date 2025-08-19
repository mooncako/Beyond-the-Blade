using UnityEngine;

/// <summary>
/// Simple weapon implementation using direct projectile approach
/// No launcher component needed - handles everything internally
/// </summary>
public class DirectWeapon : MonoBehaviour
{
    [Header("Weapon Configuration")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform[] firePoints; // Multiple fire points for different weapons
    [SerializeField] private float fireRate = 0.5f; // Time between shots
    [SerializeField] private bool isAutomatic = false;
    
    [Header("Projectile Stats")]
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private float projectileDamage = 30f;
    [SerializeField] private float projectileRange = 25f;
    [SerializeField] private bool isPiercing = false;
    
    [Header("Team & Targeting")]
    [SerializeField] private int weaponTeam = 0;
    [SerializeField] private LayerMask targetLayers = -1;
    
    // Internal state
    private float nextFireTime = 0f;
    private int currentFirePointIndex = 0;

    #region Public Interface

    /// <summary>
    /// Try to fire the weapon in a direction
    /// </summary>
    public bool TryFire(Vector3 direction)
    {
        if (!CanFire()) return false;
        
        FireProjectile(direction);
        nextFireTime = Time.time + fireRate;
        return true;
    }

    /// <summary>
    /// Try to fire at a specific target position
    /// </summary>
    public bool TryFireAt(Vector3 targetPosition)
    {
        if (!CanFire()) return false;
        
        Vector3 firePosition = GetCurrentFirePoint();
        Vector3 direction = (targetPosition - firePosition).normalized;
        
        FireProjectile(direction);
        nextFireTime = Time.time + fireRate;
        return true;
    }

    /// <summary>
    /// Try to fire at a target GameObject
    /// </summary>
    public bool TryFireAt(GameObject target)
    {
        if (target == null) return false;
        return TryFireAt(target.transform.position);
    }

    /// <summary>
    /// Check if weapon can fire right now
    /// </summary>
    public bool CanFire()
    {
        return Time.time >= nextFireTime && projectilePrefab != null;
    }

    /// <summary>
    /// Get time until weapon can fire again
    /// </summary>
    public float GetCooldownRemaining()
    {
        return Mathf.Max(0f, nextFireTime - Time.time);
    }

    #endregion

    #region Weapon Variants

    /// <summary>
    /// Fire a burst of projectiles
    /// </summary>
    public void FireBurst(Vector3 direction, int burstCount = 3, float burstDelay = 0.1f)
    {
        if (!CanFire()) return;
        
        StartCoroutine(FireBurstCoroutine(direction, burstCount, burstDelay));
        nextFireTime = Time.time + fireRate;
    }

    /// <summary>
    /// Fire projectiles in a spread pattern
    /// </summary>
    public void FireSpread(Vector3 baseDirection, int projectileCount = 3, float spreadAngle = 30f)
    {
        if (!CanFire()) return;
        
        Vector3 firePosition = GetCurrentFirePoint();
        
        for (int i = 0; i < projectileCount; i++)
        {
            float angle = 0f;
            
            if (projectileCount > 1)
            {
                float step = spreadAngle / (projectileCount - 1);
                angle = -spreadAngle * 0.5f + (step * i);
            }
            
            Vector3 direction = Quaternion.Euler(0, angle, 0) * baseDirection;
            
            // Reduced damage for spread shots
            ProjectileData spreadData = CreateProjectileData();
            spreadData.damage = projectileDamage * 0.8f;
            
            BaseProjectile.CreateAndFire(projectilePrefab, firePosition, direction, gameObject, spreadData);
        }
        
        nextFireTime = Time.time + fireRate;
    }

    /// <summary>
    /// Fire a charged projectile with increased stats
    /// </summary>
    public void FireCharged(Vector3 direction, float chargeMultiplier = 2f)
    {
        if (!CanFire()) return;
        
        ProjectileData chargedData = CreateProjectileData();
        chargedData.damage *= chargeMultiplier;
        chargedData.speed *= 1.5f;
        chargedData.maxRange *= 1.2f;
        
        Vector3 firePosition = GetCurrentFirePoint();
        BaseProjectile.CreateAndFire(projectilePrefab, firePosition, direction, gameObject, chargedData);
        
        nextFireTime = Time.time + (fireRate * 1.5f); // Longer cooldown for charged shots
    }

    #endregion

    #region Private Methods

    private void FireProjectile(Vector3 direction)
    {
        Vector3 firePosition = GetCurrentFirePoint();
        ProjectileData data = CreateProjectileData();
        
        // ObjectPool integration is now built into CreateAndFire
        BaseProjectile projectile = BaseProjectile.CreateAndFire(
            projectilePrefab, 
            firePosition, 
            direction, 
            gameObject, 
            data
        );
        
        // Cycle to next fire point for multi-barrel weapons
        CycleFirePoint();
    }

    private Vector3 GetCurrentFirePoint()
    {
        if (firePoints == null || firePoints.Length == 0)
        {
            return transform.position;
        }
        
        Transform currentPoint = firePoints[currentFirePointIndex];
        return currentPoint != null ? currentPoint.position : transform.position;
    }

    private void CycleFirePoint()
    {
        if (firePoints != null && firePoints.Length > 1)
        {
            currentFirePointIndex = (currentFirePointIndex + 1) % firePoints.Length;
        }
    }

    private ProjectileData CreateProjectileData()
    {
        return new ProjectileData
        {
            speed = projectileSpeed,
            damage = projectileDamage,
            maxRange = projectileRange,
            lifetime = projectileRange / projectileSpeed + 1f, // Auto-calculate lifetime
            damageType = DamageType.Regular,
            isPiercing = isPiercing,
            maxHits = isPiercing ? 3 : 1,
            canBounce = false,
            maxBounces = 0,
            ownerTeam = weaponTeam,
            targetLayers = targetLayers,
            collisionLayers = -1, // Hit everything
            rotateInFlight = false,
            rotationSpeed = 0f
        };
    }

    private System.Collections.IEnumerator FireBurstCoroutine(Vector3 direction, int burstCount, float burstDelay)
    {
        for (int i = 0; i < burstCount; i++)
        {
            if (i > 0) // Don't delay the first shot
            {
                yield return new WaitForSeconds(burstDelay);
            }
            
            FireProjectile(direction);
        }
    }

    #endregion

    #region Debug and Visualization

    private void OnDrawGizmosSelected()
    {
        // Draw fire points
        if (firePoints != null)
        {
            for (int i = 0; i < firePoints.Length; i++)
            {
                if (firePoints[i] != null)
                {
                    Gizmos.color = i == currentFirePointIndex ? Color.green : Color.yellow;
                    Gizmos.DrawWireSphere(firePoints[i].position, 0.2f);
                    
                    // Draw fire direction
                    Gizmos.color = Color.blue;
                    Gizmos.DrawRay(firePoints[i].position, transform.forward * 3f);
                }
            }
        }
        else
        {
            // Draw default fire point
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.2f);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, transform.forward * 3f);
        }
        
        // Draw range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, projectileRange);
    }

    [ContextMenu("Test Fire Forward")]
    private void TestFireForward()
    {
        TryFire(transform.forward);
    }

    [ContextMenu("Test Fire Spread")]
    private void TestFireSpread()
    {
        FireSpread(transform.forward, 5, 45f);
    }

    [ContextMenu("Test Fire Burst")]
    private void TestFireBurst()
    {
        FireBurst(transform.forward, 3, 0.1f);
    }

    #endregion
}

/// <summary>
/// Example player controller using DirectWeapon
/// </summary>
public class PlayerWeaponController : MonoBehaviour
{
    [SerializeField] private DirectWeapon weapon;
    [SerializeField] private Camera playerCamera;
    
    private void Update()
    {
        if (weapon == null) return;
        
        // Auto fire
        if (Input.GetMouseButton(0))
        {
            Vector3 fireDirection = GetAimDirection();
            weapon.TryFire(fireDirection);
        }
        
        // Spread shot
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Vector3 fireDirection = GetAimDirection();
            weapon.FireSpread(fireDirection, 5, 45f);
        }
        
        // Charged shot
        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector3 fireDirection = GetAimDirection();
            weapon.FireCharged(fireDirection, 2.5f);
        }
    }
    
    private Vector3 GetAimDirection()
    {
        if (playerCamera != null)
        {
            return playerCamera.transform.forward;
        }
        
        return transform.forward;
    }
}
