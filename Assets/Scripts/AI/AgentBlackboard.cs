using Sirenix.OdinInspector;
using UnityEngine;

public class AgentBlackboard : MonoBehaviour
{
    [SerializeField, BoxGroup("Data"), ReadOnly] private Vector3 _targetPosition;
    [SerializeField, BoxGroup("Data"), ReadOnly] private Transform _targetTransform;

    public bool HasTarget => _targetTransform != null;
    public Vector3 TargetPosition => _targetPosition;
    public Transform TargetTransform => _targetTransform;

    public void SetTarget(Transform target)
    {
        _targetTransform = target;

        if (target != null)
        {
            _targetPosition = target.position;
        }
    }

    public void ClearTarget(Transform target = null)
    {
        if (target != null && _targetTransform != target)
        {
            return;
        }

        _targetTransform = null;
    }

    public void SyncTargetPosition()
    {
        if (_targetTransform != null)
        {
            _targetPosition = _targetTransform.position;
        }
    }
}
