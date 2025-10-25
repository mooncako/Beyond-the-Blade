using UnityEngine;

[System.Serializable]
public struct ProjectileData
{
    [Header("Movement")]
    [Tooltip("Speed of the projectile in units per second")]
    public float speed;
    
    [Tooltip("Maximum distance the projectile can travel")]
    public float maxRange;
    
    [Tooltip("Maximum lifetime in seconds before auto-destruction")]
    public float lifetime;
    
    [Header("Damage")]
    [Tooltip("Damage amount dealt on impact")]
    public float damage;
    
    [Tooltip("Type of damage dealt")]
    public DamageType damageType;
    
    [Header("Behavior")]
    [Tooltip("Can the projectile pass through multiple enemies")]
    public bool isPiercing;
    
    [Tooltip("Number of enemies projectile can hit (0 = infinite if piercing)")]
    public int maxHits;
    
    [Tooltip("Should the projectile bounce off surfaces")]
    public bool canBounce;
    
    [Tooltip("Number of bounces allowed")]
    public int maxBounces;
    
    [Header("Targeting")]
    [Tooltip("Team that fired this projectile")]
    public int ownerTeam;
    
    [Tooltip("Layer mask for valid targets")]
    public LayerMask targetLayers;
    
    [Tooltip("Layer mask for surfaces to collide with")]
    public LayerMask collisionLayers;
    
    [Header("Visual")]
    [Tooltip("Should the projectile rotate during flight")]
    public bool rotateInFlight;
    
    [Tooltip("Rotation speed in degrees per second")]
    public float rotationSpeed;

    /// <summary>
    /// Create a basic projectile data configuration
    /// </summary>
    public static ProjectileData CreateBasic(float speed, float damage, int ownerTeam, LayerMask targetLayers)
    {
        return new ProjectileData
        {
            speed = speed,
            maxRange = 50f,
            lifetime = 10f,
            damage = damage,
            damageType = DamageType.Regular,
            isPiercing = false,
            maxHits = 1,
            canBounce = false,
            maxBounces = 0,
            ownerTeam = ownerTeam,
            targetLayers = targetLayers,
            collisionLayers = -1, // All layers
            rotateInFlight = false,
            rotationSpeed = 0f
        };
    }
}
