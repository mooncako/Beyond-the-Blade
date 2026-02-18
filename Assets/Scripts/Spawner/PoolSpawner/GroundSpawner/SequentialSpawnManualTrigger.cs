using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(CollisionTrigger))]
public class SequentialSpawnManualTrigger : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private CollisionTrigger _spawnTrigger;
    [SerializeField, BoxGroup("Settings")] private string _encounterID;

    void OnValidate()
    {
        if (_spawnTrigger == null)
        {
            _spawnTrigger = GetComponent<CollisionTrigger>();
            _spawnTrigger.DoOnce = true;
        }
    }

    void OnEnable()
    {
        _spawnTrigger.TriggerEnter.AddListener(TriggerEnter);
    }

    void OnDisable()
    {
        _spawnTrigger.TriggerEnter.RemoveListener(TriggerEnter);
    }

    private void TriggerEnter(Collider other)
    {
        ManualSequentialEnemySpawnerTriggerEvent.Trigger(_encounterID);
    }
}
