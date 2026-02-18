using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(CollisionTrigger))]
public class ClearLevelCollisionTrigger : MonoBehaviour
{
    
    [SerializeField, BoxGroup("References")] private CollisionTrigger _collisionTrigger;

    void OnValidate()
    {
        if(_collisionTrigger == null)
        {
            _collisionTrigger = GetComponent<CollisionTrigger>();
            _collisionTrigger.LayerMask = LayerMask.GetMask("Player");
        }
    }

    void OnEnable()
    {
        _collisionTrigger.TriggerEnter.AddListener(OnPlayerEnter);
    }

    void OnDisable()
    {
        _collisionTrigger.TriggerEnter.RemoveListener(OnPlayerEnter);
    }


    private void OnPlayerEnter(Collider other)
    {
        LevelClearedEvent.Trigger(LevelManager.Instance.CurrentLevel.PossibleRewardType, false);
    }


}
