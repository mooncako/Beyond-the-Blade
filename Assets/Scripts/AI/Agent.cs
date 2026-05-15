using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(AgentBlackboard))]
public class Agent : SerializedMonoBehaviour
{
    [SerializeField, BoxGroup("References")] protected AgentBlackboard _blackboard;
    [SerializeField, BoxGroup("References")] protected PlayerTSensor _playerSensor;
    [SerializeField, BoxGroup("Settings"), InlineEditor] protected AgentConfigSO _config;
    [field: SerializeField, BoxGroup("Settings")] protected List<AgentStateSO> _states = new List<AgentStateSO>();
    [SerializeField, BoxGroup("Settings")] protected AgentStateSO _initialState;
    [SerializeField, BoxGroup("Debug"), ReadOnly] protected AgentStateSO _currentState;

    // Strafe state (main thread only)
    private float _strafeAngle;
    private float _strafeDirection = 1f;
    private float _strafeTimer;

    // Movement threading
    private Vector3 _cachedNextPosition;
    private readonly object _positionLock = new object();
    private Task<Vector3> _positionTask;
    private CancellationTokenSource _movementCts;

    public AgentBlackboard Blackboard => _blackboard;
    public AgentStateSO CurrentState => _currentState;
    public event Action<AgentStateSO> OnStateChanged;

    protected virtual void OnValidate()
    {
        if (_blackboard == null) _blackboard = GetComponent<AgentBlackboard>();
        if (_playerSensor == null) _playerSensor = GetComponentInChildren<PlayerTSensor>();
    }

    protected virtual void Awake()
    {
        if (_blackboard == null)
        {
            _blackboard = GetComponent<AgentBlackboard>();
        }
        _movementCts = new CancellationTokenSource();
        _cachedNextPosition = transform.position;
        _strafeTimer = UnityEngine.Random.Range(
            _config != null ? _config.MinStrafeDuration : 0.2f,
            _config != null ? _config.MaxStrafeDuration : 0.5f);
    }

    protected virtual void Start()
    {
        if (_initialState != null)
        {
            ChangeState(_initialState);
        }
    }

    protected virtual void OnDestroy()
    {
        _movementCts?.Cancel();
        _movementCts?.Dispose();
    }

    protected virtual void Update()
    {
        _blackboard?.SyncTargetPosition();
        UpdateStrafeState(Time.deltaTime);

        if (_currentState != null)
        {
            _currentState.OnStateUpdate(this, Time.deltaTime);
        }
    }

    private void UpdateStrafeState(float deltaTime)
    {
        if (_config == null) return;

        _strafeTimer -= deltaTime;
        if (_strafeTimer <= 0f)
        {
            _strafeDirection = -_strafeDirection;
            _strafeTimer = UnityEngine.Random.Range(_config.MinStrafeDuration, _config.MaxStrafeDuration);
        }
        _strafeAngle += _strafeDirection * _config.StrafeAngularSpeed * deltaTime;
    }

    public bool ChangeState(AgentStateSO nextState)
    {
        if (nextState == null || nextState == _currentState)
        {
            return false;
        }

        if (_currentState != null && _currentState.IsStateProhibited(nextState))
        {
            return false;
        }

        if (!nextState.CanEnter(this))
        {
            return false;
        }

        _currentState?.OnStateExit(this);
        _currentState = nextState;
        _currentState.OnStateEnter(this);
        OnStateChanged?.Invoke(_currentState);
        return true;
    }

    public bool GetNextMovementPosition(out Vector3 position)
    {
        if (_blackboard == null || _blackboard.TargetTransform == null)
        {
            position = transform.position;
            return false;
        }

        lock (_positionLock)
        {
            position = _cachedNextPosition;
        }

        ScheduleMovementCalculation();
        return true;
    }

    private void ScheduleMovementCalculation()
    {
        if (_config == null) return;
        if (_positionTask != null && !_positionTask.IsCompleted) return;

        var input = new MovementInput(
            transform.position,
            _blackboard.TargetPosition,
            _strafeAngle,
            _config.AttackRange,
            _config.StrafeRange,
            _config.ChaseRange);

        var token = _movementCts.Token;
        _positionTask = Task.Run(() => CalculateMovementPosition(input), token);
        _positionTask.ContinueWith(t =>
        {
            if (t.IsCanceled || t.IsFaulted) return;
            lock (_positionLock)
            {
                _cachedNextPosition = t.Result;
            }
        }, TaskContinuationOptions.ExecuteSynchronously);
    }

    private static Vector3 CalculateMovementPosition(MovementInput input)
    {
        float dx = input.TargetPosition.x - input.AgentPosition.x;
        float dz = input.TargetPosition.z - input.AgentPosition.z;
        float distanceSq = dx * dx + dz * dz;

        // Within attack range — stop
        if (distanceSq <= input.AttackRange * input.AttackRange)
        {
            return input.AgentPosition;
        }

        // Within strafe range — orbit around target at StrafeRange radius
        if (distanceSq <= input.StrafeRange * input.StrafeRange)
        {
            return new Vector3(
                input.TargetPosition.x + Mathf.Cos(input.StrafeAngle) * input.StrafeRange,
                input.AgentPosition.y,
                input.TargetPosition.z + Mathf.Sin(input.StrafeAngle) * input.StrafeRange);
        }

        // Beyond strafe range (including beyond chase range) — chase the target
        return input.TargetPosition;
    }

    private readonly struct MovementInput
    {
        public readonly Vector3 AgentPosition;
        public readonly Vector3 TargetPosition;
        public readonly float StrafeAngle;
        public readonly float AttackRange;
        public readonly float StrafeRange;
        public readonly float ChaseRange;

        public MovementInput(Vector3 agentPos, Vector3 targetPos, float strafeAngle,
            float attackRange, float strafeRange, float chaseRange)
        {
            AgentPosition = agentPos;
            TargetPosition = targetPos;
            StrafeAngle = strafeAngle;
            AttackRange = attackRange;
            StrafeRange = strafeRange;
            ChaseRange = chaseRange;
        }
    }
}
