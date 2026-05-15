using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SphereCollider))]
public class Sensor : MonoBehaviour
{
    [SerializeField, BoxGroup("Settings")] protected LayerMask _detectionMask;
    [SerializeField, BoxGroup("References")] protected SphereCollider _collider;

    public UnityEvent OnEnter;
    public UnityEvent OnExit;

    protected virtual void OnValidate()
    {
        if (_collider == null) _collider = GetComponent<SphereCollider>();
        _collider.isTrigger = true;
    }

    protected bool IsDetectionMatch(Collider other)
    {
        return ((1 << other.gameObject.layer) & _detectionMask) != 0;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!IsDetectionMatch(other))
        {
            return;
        }

        OnEnter?.Invoke();
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (!IsDetectionMatch(other))
        {
            return;
        }

        OnExit?.Invoke();
    }
}
