using CrashKonijn.Agent.Core;
using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.GenTest;
using CrashKonijn.Goap.Runtime;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

public class Brain : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] protected AgentBehaviour _agent;
    [SerializeField, BoxGroup("References")] protected GoapActionProvider _provider;
    [SerializeField, BoxGroup("References")] protected GoapBehaviour _goap;
    [SerializeField, BoxGroup("References")] protected PlayerSensor _playerSensor;
    [SerializeField, BoxGroup("References")] protected AttackSensorConfigSO _attackSensorConfigSO;
    [SerializeField, BoxGroup("Settings")] protected float _spanwDelay = .2f;

    [SerializeField, BoxGroup("Debug"), ReadOnly] protected bool _isPlayerInRange = false;
    [SerializeField, BoxGroup("Debug"), ReadOnly] protected bool _isPlayerDetected = false;

    protected Tween _spawnDelayTween;
    protected Tween _staggerDelayTween;

    protected virtual void OnValidate()
    {
        if (_agent == null) _agent = GetComponent<AgentBehaviour>();
        if (_provider == null) _provider = GetComponent<GoapActionProvider>();
        if (_goap == null) _goap = GetComponent<GoapBehaviour>();
        if (_playerSensor == null) _playerSensor = GetComponentInChildren<PlayerSensor>();
    }

    protected virtual void OnEnable()
    {
        _playerSensor.OnPlayerEnter += OnPlayerEnter;
        _playerSensor.OnPlayerExit += OnPlayerExit;
        _agent.Events.OnActionEnd += OnActionEnd;
        _isPlayerDetected = false;
        _provider.ClearGoal();
        _provider.RequestGoal<WanderGoal>(false);
    }

    protected virtual void OnDisable()
    {
        _playerSensor.OnPlayerEnter -= OnPlayerEnter;
        _playerSensor.OnPlayerExit -= OnPlayerExit;
        _agent.Events.OnActionEnd -= OnActionEnd;
        _spawnDelayTween.Stop();
        _staggerDelayTween.Stop();
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

    public virtual void Stagger(float duration)
    {
        IAction action = _agent.ActionState.Action;
        _provider.ClearGoal();
        _agent.IsPaused = true;
        _staggerDelayTween = Tween.Delay(duration).OnComplete(() =>
        {
            _agent.IsPaused = false;
            OnActionEnd(action);
        });
    }
}
