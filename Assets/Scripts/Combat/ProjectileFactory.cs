using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(ProjectileSpawner))]
[RequireComponent(typeof(DontDestroy))]
public class ProjectileFactory : MonoBehaviour,
    MMEventListener<SpawnProjectileEvent>
{
    [SerializeField, BoxGroup("References")] private ProjectileSpawner _projectileSpawner;
    [SerializeField, BoxGroup("References")] private ObjectPool _pool;

    void OnValidate()
    {
        if (_projectileSpawner == null) _projectileSpawner = GetComponent<ProjectileSpawner>();
        if (_pool == null) _pool = GetComponent<ObjectPool>();
    }

    void OnEnable()
    {
        this.MMEventStartListening<SpawnProjectileEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<SpawnProjectileEvent>();
    }

    public void OnMMEvent(SpawnProjectileEvent e)
    {
        _projectileSpawner.SpawnProjectile(e.Id, e.Position, e.Direction, e.Owner);
    }
}
