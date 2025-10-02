using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private VisualEffect _spawnVFX;
    [SerializeField, BoxGroup("References")] private EnemySpawnVFXEventHandler _spawnVFXEventHandler;
    [SerializeField, BoxGroup("References")] private GameObject _spawnedEntity;

    void OnValidate()
    {
        if (_spawnVFX == null) _spawnVFX = GetComponentInChildren<VisualEffect>();
        if (_spawnVFXEventHandler == null) _spawnVFXEventHandler = GetComponentInChildren<EnemySpawnVFXEventHandler>();
    }

    void OnEnable()
    {
        _spawnVFXEventHandler.OnSpawnFinished.AddListener(SpawnEntity);
        
    }

    void OnDisable()
    {
        _spawnVFXEventHandler.OnSpawnFinished.RemoveListener(SpawnEntity);
        _spawnedEntity?.SetActive(false);
    }

    [Button]
    public void StartSpawn()
    {
        _spawnVFX.Play();
    }

    private void SpawnEntity()
    {
        _spawnedEntity.SetActive(true);
        EnemySpawnedEvent.Trigger(_spawnedEntity.GetComponent<Health>());
    }
}
