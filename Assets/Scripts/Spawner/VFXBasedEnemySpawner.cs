using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

public class VFXBasedEnemySpawner : EnemySpawner
{
    [SerializeField, BoxGroup("References")] private VisualEffect _spawnVFX;
    [SerializeField, BoxGroup("References")] private VFXFinishedEventHandler _spawnVFXEventHandler;

    protected override void OnValidate()
    {
        base.OnValidate();
        if (_spawnVFX == null) _spawnVFX = GetComponentInChildren<VisualEffect>();
        if (_spawnVFXEventHandler == null) _spawnVFXEventHandler = GetComponentInChildren<VFXFinishedEventHandler>(); 
    }

    void OnEnable()
    {
        _spawnVFXEventHandler.OnVfxFinished.AddListener(SpawnEntity);
        
    }

    void OnDisable()
    {
        _spawnVFXEventHandler.OnVfxFinished.RemoveListener(SpawnEntity);
    }

    [Button]
    public override void StartSpawn()
    {
        _spawnVFX.Play();
    }

    protected override void SpawnEntity()
    {
        base.SpawnEntity();
    }

    public override void OnPoolGet()
    {
        base.OnPoolGet();
    }

    public override void OnPoolReturn()
    {
        base.OnPoolReturn();
    }
}
