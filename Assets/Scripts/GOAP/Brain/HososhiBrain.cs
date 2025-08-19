using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.GenTest;
using UnityEngine;

public class HososhiBrain : Brain
{
    protected override void OnValidate()
    {
        base.OnValidate();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _provider.RequestGoal<WanderGoal>(false);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void Awake()
    {
        if(_provider != null && _goap != null)
        {
            if(_provider.AgentTypeBehaviour == null)
            {
                _provider.AgentType = _goap.GetAgentType("Hososhi");
            }
        }
    }

    protected override void Start()
    {
        _provider.RequestGoal<WanderGoal>(false);
        _playerSensor.Collider.radius = _attackSensorConfigSO.SensorRadius;
    }

    protected override void OnActionEnd(IAction action)
    {
        if (_isPlayerDetected)
        {
            _provider.RequestGoal<KillPlayerGoal, StrafeGoal>();
        }
        else
        {
            _provider.RequestGoal<WanderGoal>();
        }
    }

    protected override void OnPlayerEnter(Transform player)
    {
        _provider.ClearGoal();
        _provider.RequestGoal<KillPlayerGoal, StrafeGoal>();
        _isPlayerInRange = true;
        _isPlayerDetected = true;
    }

    protected override void OnPlayerExit(Vector3 lastKnownPosition)
    {
        _isPlayerInRange = false;
    }
}
