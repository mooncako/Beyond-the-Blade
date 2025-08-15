using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class CollisionTrigger : MonoBehaviour
{
    [SerializeField] private LayerMask _activeMask;
    [SerializeField] private bool _doOnce = false;
    
    [SerializeField] public UnityEvent<Collider> TriggerEnter;
    [SerializeField] public UnityEvent<Collider> TriggerExit;

    private bool _canTriggerEnter = true;
    private bool _canTriggerExit = true;


    void OnTriggerEnter(Collider other)
    {
        if((_activeMask.value & (1 << other.gameObject.layer)) != 0)
        {
            if(_canTriggerEnter)
            {
                TriggerEnter?.Invoke(other);
            }

            if(_doOnce)
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

            if(_doOnce)
            {
                _canTriggerExit = false;
            }
        }
    }
}
