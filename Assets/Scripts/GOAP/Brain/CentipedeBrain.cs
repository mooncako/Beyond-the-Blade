using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.GenTest;
using UnityEngine;

public class CentipedeBrain : Brain
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
                _provider.AgentType = _goap.GetAgentType("Centipede");
            }
        }
    }

    protected override void Start()
    {
        _provider.RequestGoal<KillPlayerGoal>(false);
        _combatRangeSensor.Collider.radius = _attackSensorConfigSO.SensorRadius;
    }

    protected override void OnActionEnd(IAction action)
    {
        if (!gameObject.activeSelf) return;
        _provider.RequestGoal<KillPlayerGoal>();
    }
}
