using System;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class MovingPlatformPlayerCapture : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private BoxCollider _collider;
    [SerializeField, BoxGroup("Settings")] private LayerMask _playerMask;

    private void OnValidate()
    {
        if (_collider == null)
        {
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;
        }
        
        if ((_playerMask & (1 << 7)) == 0)
        {
            _playerMask |= 1 << 7;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((_playerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            other.transform.parent = transform.parent;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((_playerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            other.transform.parent = null;
        }
    }
}
