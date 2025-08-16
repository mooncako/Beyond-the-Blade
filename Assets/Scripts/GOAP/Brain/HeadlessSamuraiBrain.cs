using System;
using CrashKonijn.Agent.Core;
using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.GenTest;
using CrashKonijn.Goap.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

public class HeadlessSamuraiBrain : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private AgentBehaviour _agent;
    [SerializeField, BoxGroup("References")] private GoapActionProvider _provider;
    [SerializeField, BoxGroup("References")] private GoapBehaviour _goap;
    [SerializeField, BoxGroup("References")] private PlayerSensor _playerSensor;
    [SerializeField, BoxGroup("References")] private AttackSensorConfigSO _attackSensorConfigSO;

    [SerializeField, BoxGroup("Debug"), ReadOnly] private bool _isPlayerInRange = false;

    void OnValidate()
    {
        if (_agent == null) _agent = GetComponent<AgentBehaviour>();
        if (_provider == null) _provider = GetComponent<GoapActionProvider>();
        if (_goap == null) _goap = GetComponent<GoapBehaviour>();
    }

    void OnEnable()
    {
        _playerSensor.OnPlayerEnter += OnPlayerEnter;
        _playerSensor.OnPlayerExit += OnPlayerExit;
        _agent.Events.OnActionEnd += OnActionEnd;
    }

    void OnDisable()
    {
        _playerSensor.OnPlayerEnter -= OnPlayerEnter;
        _playerSensor.OnPlayerExit -= OnPlayerExit;
        _agent.Events.OnActionEnd -= OnActionEnd;
    }

    void Awake()
    {
        if(_provider != null && _goap != null)
        {
            if(_provider.AgentTypeBehaviour == null)
            {
                _provider.AgentType = _goap.GetAgentType("HeadlessSamurai");
            }
        }
    }

    void Start()
    {
        _provider.RequestGoal<WanderGoal>(false);
        _playerSensor.Collider.radius = _attackSensorConfigSO.SensorRadius;
    }

    private void OnActionEnd(IAction action)
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

    private void OnPlayerEnter(Transform player)
    {
        _provider.RequestGoal<KillPlayerGoal>();
        _isPlayerInRange = true;
    }

    private void OnPlayerExit(Vector3 lastKnownPosition)
    {
        _provider.RequestGoal<WanderGoal>();
        _isPlayerInRange = false;
    }

    

    
}
