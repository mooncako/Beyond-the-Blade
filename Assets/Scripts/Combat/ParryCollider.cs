using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SphereCollider))]
public class ParryCollider : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private SphereCollider _collider;
    [SerializeField, BoxGroup("References")] private PlayerController _playerController;
    
    [SerializeField, BoxGroup("Settings")] private float _radius = .5f;
    
    [BoxGroup("Events")] public UnityEvent<float> OnParried;
    [BoxGroup("Events")] public UnityEvent<BaseProjectile> OnProjectileDeflected;

    void OnValidate()
    {
        if (_collider == null)
        {
            _collider = GetComponent<SphereCollider>();
            _collider.isTrigger = true; // change to trigger for projectile detection
            _collider.radius = _radius;
        }
        
        if (_playerController == null)
        {
            _playerController = GetComponentInParent<PlayerController>();
        }
    }

    void Awake()
    {
        _collider.radius = _radius;
        _collider.isTrigger = true; // changed from false
        CloseCollider();
    }

    void OnDisable()
    {
        if (_collider != null)
            CloseCollider();
    }

    void LateUpdate()
    {
        if (_playerController == null || !_collider.enabled) return;

        if (!_playerController.IsPerfectParryWindowActive)
            CloseCollider();
    }

    public void OpenCollider()
    {
        _collider.enabled = true;
    }

    public void CloseCollider()
    {
        _collider.enabled = false;
    }

    public void OnParry(float duration)
    {
        OnParried.Invoke(duration);
        CloseCollider();
    }
    
    private void OnTriggerEnter(Collider other)
    {
       
        if (_playerController == null || !_playerController.IsPerfectParryWindowActive) return;
        

        BaseProjectile projectile = other.attachedRigidbody != null
            ? other.attachedRigidbody.GetComponent<BaseProjectile>()
            : other.GetComponent<BaseProjectile>();

        if (projectile == null)
        {
            projectile = other.GetComponentInParent<BaseProjectile>();
        }

        if (projectile == null || !projectile.IsActive) return;
        
        if (!projectile.CanBeDeflected) return;
        
        if (projectile.Owner == _playerController.gameObject) return;
        
        DeflectProjectile(projectile);
    }
    
    private void DeflectProjectile(BaseProjectile projectile)
    {
        Vector3 deflectDirection = CalculateDeflectDirection(projectile);
        
        projectile.Deflect(_playerController.gameObject, deflectDirection);
        
        OnProjectileDeflected?.Invoke(projectile);
        
        CloseCollider();
        
        // Notify player controller
        _playerController.OnProjectileDeflected(projectile);
    }
    
    private Vector3 CalculateDeflectDirection(BaseProjectile projectile)
    {

        //  1: Aim at nearest enemy
        EnemyController nearestEnemy = _playerController.FindClosestEnemyToPosition(
            transform.position, 50f
        );
        
        if (nearestEnemy != null)
        {
            Vector3 toEnemy = (nearestEnemy.transform.position - projectile.transform.position).normalized;
            return toEnemy;
        }
        
        // 2: Reflect back toward shooter
        Vector3 reflectDirection = -projectile.GetVelocity().normalized;
        return reflectDirection;
    }
}
