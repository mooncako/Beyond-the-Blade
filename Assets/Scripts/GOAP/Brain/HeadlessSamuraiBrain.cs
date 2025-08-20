using System;
using System.Collections.Generic;
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
        _provider.ClearGoal();

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
        if (!gameObject.activeSelf) return;

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
        // _provider.ClearGoal();
        // _provider.RequestGoal<WanderGoal>(false);
        _isPlayerInRange = false;
    }

    

    
}
