using System;
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
    [SerializeField, BoxGroup("References")] protected PlayerSensor _combatRangeSensor;
    [SerializeField, BoxGroup("References"), ShowIf("Personality", PersonalityType.Evasive)] protected PlayerSensor _evadeSensor;
    [SerializeField, BoxGroup("References")] protected AttackSensorConfigSO _attackSensorConfigSO;
    [SerializeField, BoxGroup("References")] protected CustomCharacterMovement _movement;
    [SerializeField, BoxGroup("References")] protected EnemyController _controller;
    [SerializeField, BoxGroup("Settings")] protected float _spawnDelay = .2f;
    [SerializeField, BoxGroup("Settings")] public PersonalityType Personality;

    [SerializeField, BoxGroup("Debug"), ReadOnly] protected bool _isPlayerInCombatRange = false;
    [SerializeField, BoxGroup("Debug"), ReadOnly, ShowIf("Personality", PersonalityType.Evasive)] protected bool _isPlayerInCloseRange = false;
    public bool IsPlayerInCombatRange => _isPlayerInCombatRange;
    public bool IsPlayerInCloseRange => _isPlayerInCloseRange;

    protected Tween _spawnDelayTween;
    protected Tween _staggerDelayTween;

    protected virtual void OnValidate()
    {
        if (_agent == null) _agent = GetComponent<AgentBehaviour>();
        if (_provider == null) _provider = GetComponent<GoapActionProvider>();
        if (_goap == null) _goap = GetComponent<GoapBehaviour>();
        if (_combatRangeSensor == null) _combatRangeSensor = GetComponentsInChildren<PlayerSensor>()[0];
        if(Personality == PersonalityType.Evasive && _evadeSensor == null)
        {
            var sensors = GetComponentsInChildren<PlayerSensor>();
            if(sensors.Length > 1)
            {
                _evadeSensor = sensors[1];
            }
        }
        if (_movement == null) _movement = GetComponent<CustomCharacterMovement>();
        if (_controller == null) _controller = GetComponent<EnemyController>();
    }

    protected virtual void OnEnable()
    {
        _combatRangeSensor.OnPlayerEnter += OnPlayerEnterCombatRange;
        _combatRangeSensor.OnPlayerExit += OnPlayerExitCombatRange;
        if(_evadeSensor != null)
        {
            _evadeSensor.OnPlayerEnter += OnPlayerEnterCloseRange;
            _evadeSensor.OnPlayerExit += OnPlayerExitCloseRange;
        }
        _agent.Events.OnActionEnd += OnActionEnd;
        _agent.IsPaused = false;
        _provider.ClearGoal();
    }

    protected virtual void OnDisable()
    {
        _combatRangeSensor.OnPlayerEnter -= OnPlayerEnterCombatRange;
        _combatRangeSensor.OnPlayerExit -= OnPlayerExitCombatRange;
        if(_evadeSensor != null)
        {
            _evadeSensor.OnPlayerEnter -= OnPlayerEnterCloseRange;
            _evadeSensor.OnPlayerExit -= OnPlayerExitCloseRange;
        }
        _agent.Events.OnActionEnd -= OnActionEnd;
        _spawnDelayTween.Stop();
        _staggerDelayTween.Stop();
    }

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        _combatRangeSensor.Collider.radius = _attackSensorConfigSO.CombatSensorRadius;
        if(_evadeSensor != null)
        {
            _evadeSensor.Collider.radius = _attackSensorConfigSO.CloseRangeSensorRadius;
        }
    }

    protected virtual void OnActionEnd(IAction action)
    {

    }

    protected virtual void OnPlayerEnterCombatRange(Transform player)
    {

    }

    protected virtual void OnPlayerExitCombatRange(Vector3 lastKnownPosition)
    {

    }

    protected virtual void OnPlayerEnterCloseRange(Transform player)
    {
        
    }

    protected virtual void OnPlayerExitCloseRange(Vector3 lastKnownPosition)
    {
        
    }

    [Sirenix.OdinInspector.Button]
    public virtual void Stun(float duration, Action onComplete = null)
    {
        IAction action = _agent.ActionState.Action;
        _provider.ClearGoal();
        _agent.IsPaused = true;
        _movement.Stop();
        _staggerDelayTween = Tween.Delay(duration).OnComplete(() =>
        {
            _agent.IsPaused = false;
            OnActionEnd(action);
            if (onComplete != null)
            {
                onComplete.Invoke();
            }
        });
    }

    public virtual void Dead()
    {
        _agent.IsPaused = true;
        _staggerDelayTween.Stop();
        _spawnDelayTween.Stop();
    }

    public void AssignAttackSensorConfig(AttackSensorConfigSO config)
    {
        _attackSensorConfigSO = config;
    }
}
