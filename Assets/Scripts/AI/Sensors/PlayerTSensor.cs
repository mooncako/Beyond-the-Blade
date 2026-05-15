using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerTSensor : Sensor
{
    [SerializeField, BoxGroup("References")] private AgentBlackboard _blackboard;

    protected override void OnValidate()
    {
        base.OnValidate();

        if (_blackboard == null)
        {
            _blackboard = GetComponentInParent<AgentBlackboard>();
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (!IsDetectionMatch(other))
        {
            return;
        }

        base.OnTriggerEnter(other);
        _blackboard?.SetTarget(other.transform);
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (!IsDetectionMatch(other))
        {
            return;
        }

        base.OnTriggerExit(other);
        _blackboard?.ClearTarget(other.transform);
    }
}
