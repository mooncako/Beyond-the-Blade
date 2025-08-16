using System;
using CrashKonijn.Agent.Core;
using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.GenTest;
using CrashKonijn.Goap.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

public class HeadlessSamuraiBrain : Brain
{

    protected override void OnValidate()
    {
        base.OnValidate();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
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
                _provider.AgentType = _goap.GetAgentType("HeadlessSamurai");
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
        if (_isPlayerInRange)
        {
            _provider.RequestGoal<KillPlayerGoal>();
        }
        else
        {
            _provider.RequestGoal<WanderGoal>();
        }
    }

    protected override void OnPlayerEnter(Transform player)
    {
        _provider.RequestGoal<KillPlayerGoal>();
        _isPlayerInRange = true;
    }

    protected override void OnPlayerExit(Vector3 lastKnownPosition)
    {
        _provider.RequestGoal<WanderGoal>(false);
        _isPlayerInRange = false;
    }

    

    
}
