using CrashKonijn.Agent.Core;
using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

public class Brain : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] protected AgentBehaviour _agent;
    [SerializeField, BoxGroup("References")] protected GoapActionProvider _provider;
    [SerializeField, BoxGroup("References")] protected GoapBehaviour _goap;
    [SerializeField, BoxGroup("References")] protected PlayerSensor _playerSensor;
    [SerializeField, BoxGroup("References")] protected AttackSensorConfigSO _attackSensorConfigSO;

    [SerializeField, BoxGroup("Debug"), ReadOnly] protected bool _isPlayerInRange = false;
    [SerializeField, BoxGroup("Debug"), ReadOnly] protected bool _isPlayerDetected = false;


    protected virtual void OnValidate()
    {
        if (_agent == null) _agent = GetComponent<AgentBehaviour>();
        if (_provider == null) _provider = GetComponent<GoapActionProvider>();
        if (_goap == null) _goap = GetComponent<GoapBehaviour>();
        if (_playerSensor == null) _playerSensor = GetComponentInChildren<PlayerSensor>();
    }

    protected virtual void OnEnable()
    {
        _agent.IsPaused = false;
        _playerSensor.OnPlayerEnter += OnPlayerEnter;
        _playerSensor.OnPlayerExit += OnPlayerExit;
        _agent.Events.OnActionEnd += OnActionEnd;
    }

    protected virtual void OnDisable()
    {
        _agent.IsPaused = true;
        _playerSensor.OnPlayerEnter -= OnPlayerEnter;
        _playerSensor.OnPlayerExit -= OnPlayerExit;
        _agent.Events.OnActionEnd -= OnActionEnd;
    }

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        _playerSensor.Collider.radius = _attackSensorConfigSO.SensorRadius;
    }

    protected virtual void OnActionEnd(IAction action)
    {

    }

    protected virtual void OnPlayerEnter(Transform player)
    {

    }

    protected virtual void OnPlayerExit(Vector3 lastKnownPosition)
    {
        
    }
}
