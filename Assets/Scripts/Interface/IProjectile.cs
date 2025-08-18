using UnityEngine;

public interface IProjectile
{
    /// <summary>
    /// Initialize the projectile with configuration data
    /// </summary>
    void Initialize(ProjectileData data);
    
    /// <summary>
    /// Activate the projectile and start movement
    /// </summary>
    void Activate();
    
    /// <summary>
    /// Deactivate the projectile
    /// </summary>
    void Deactivate();
    
    /// <summary>
    /// Set the direction the projectile should travel
    /// </summary>
    void SetDirection(Vector3 direction);
    
    /// <summary>
    /// Set the initial position of the projectile
    /// </summary>
    void SetPosition(Vector3 position);
    
    /// <summary>
    /// Get current projectile position
    /// </summary>
    Vector3 GetPosition();
    
    /// <summary>
    /// Get current projectile velocity
    /// </summary>
    Vector3 GetVelocity();
    
    /// <summary>
    /// Check if projectile is currently active
    /// </summary>
    bool IsActive { get; }
    
    /// <summary>
    /// Get the owner/source of this projectile
    /// </summary>
    GameObject Owner { get; }
}
