using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(CollisionTrigger))]
public class TrainSpawnerManualTrigger : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private CollisionTrigger _trigger;
    [SerializeField, BoxGroup("References")] private TrainPortal[] _trainPortals;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private int _currentIndex;

    void OnValidate()
    {
        if(_trigger == null) 
        {
            _trigger = GetComponent<CollisionTrigger>();
        }
        _trainPortals = GetComponentsInChildren<TrainPortal>();
    }

    void OnEnable()
    {
        _trigger.TriggerEnter.AddListener(TriggerNext);
        for(int i = 0; i < _trainPortals.Length; i++)
        {
            _trainPortals[i].OnSpawnCompleted.AddListener(TriggerNext);
        }
    }

    void OnDisable()
    {
        _trigger.TriggerEnter.RemoveListener(TriggerNext);
        for(int i = 0; i < _trainPortals.Length; i++)
        {
            _trainPortals[i].OnSpawnCompleted.RemoveListener(TriggerNext);
        }
    }

    private void TriggerNext(Collider other)
    {
        if(_currentIndex >= _trainPortals.Length) return;
        _trainPortals[_currentIndex].OnTrigger(other);
        _currentIndex++;
    }
}
