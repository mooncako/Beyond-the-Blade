using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class CollisionTrigger : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private BoxCollider _collider;
    [SerializeField] private LayerMask _activeMask;
    public LayerMask LayerMask
    {
        get { return _activeMask; }
        set { _activeMask = value; }
    }
    [SerializeField] public bool DoOnce = false;
    
    [SerializeField] public UnityEvent<Collider> TriggerEnter;
    [SerializeField] public UnityEvent<Collider> TriggerExit;

    private bool _canTriggerEnter = true;
    private bool _canTriggerExit = true;

    void OnValidate()
    {
        if(_collider == null) 
        {
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if((_activeMask.value & (1 << other.gameObject.layer)) != 0)
        {
            if(_canTriggerEnter)
            {
                TriggerEnter?.Invoke(other);
            }

            if(DoOnce)
            {
                _canTriggerEnter = false;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if((_activeMask.value & (1 << other.gameObject.layer)) != 0)
        {
            if(_canTriggerExit)
            {
                TriggerExit?.Invoke(other);
            }

            if(DoOnce)
            {
                _canTriggerExit = false;
            }
        }
    }
}
