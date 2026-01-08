using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class LevelAssigner : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private BoxCollider _collider;
    [SerializeField, BoxGroup("References")] private LevelSystem _levelSystem;
    [SerializeField, BoxGroup("Settings")] private LayerMask _playerMask;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private bool _hasTriggered = false;

    void OnValidate()
    {
        if(_collider == null)
        {
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;
        }

        if(_levelSystem == null)
        {
            _levelSystem = GetComponentInParent<LevelSystem>();
        } 

        if ((_playerMask & (1 << 7)) == 0)
        {
            _playerMask |= 1 << 7;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        
        if ((_playerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            if(_hasTriggered) return;
            _hasTriggered = true;
            LevelManager.Instance.ChangeLevel(_levelSystem);
        }
    }
}
