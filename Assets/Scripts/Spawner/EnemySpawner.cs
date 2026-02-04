using Sirenix.OdinInspector;
using UnityEngine;

public class EnemySpawner : MonoBehaviour, IPoolable
{
    [SerializeField, BoxGroup("References")] protected GameObject _spawnedEntity;
    [SerializeField, BoxGroup("References")] protected CustomCharacterMovement _spawnedEntityMovement;
    [SerializeField, BoxGroup("Settings"), ReadOnly] public string EncounterID;

    protected virtual void OnValidate()
    {
        if (_spawnedEntityMovement == null && _spawnedEntity != null && _spawnedEntity.TryGetComponent(out CustomCharacterMovement movement))
        {
            _spawnedEntityMovement = movement;
            _spawnedEntity.SetActive(false);
        }
    }


    [Button]
    public virtual void StartSpawn()
    {

    }

    protected virtual void SpawnEntity()
    {
        _spawnedEntity.SetActive(true);
        EnemySpawnedEvent.Trigger(_spawnedEntity.GetComponent<Health>(), EncounterID);
    }

    public virtual void OnPoolGet()
    {
        _spawnedEntity?.SetActive(false);
    }

    public virtual void OnPoolReturn()
    {
        _spawnedEntity?.SetActive(false);
    }
}
