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
        _playerSensor.Collider.radius = _attackSensorConfigSO.SensorRadius;
    }
}
