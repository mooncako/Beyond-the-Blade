using System;
using CrashKonijn.Agent.Core;
using CrashKonijn.Agent.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyController), typeof(AgentBehaviour))]
public class AgentMoveBehavior : MonoBehaviour
{
    [SerializeField, FoldoutGroup("References")] private CustomCharacterMovement _movement;
    [SerializeField, FoldoutGroup("References")] private Animator _animator;
    [SerializeField, FoldoutGroup("References")] private AgentBehaviour _agentBehavior;
    [SerializeField, FoldoutGroup("References")] private EnemyController _controller;


    [SerializeField, BoxGroup("Debug"), ReadOnly] private ITarget _currentTarget;

    private void OnValidate()
    {
        if (_movement == null) _movement = GetComponent<CustomCharacterMovement>();
        if (_animator == null) _animator = GetComponent<Animator>();
        if (_controller == null) _controller = GetComponent<EnemyController>();
        if (_agentBehavior == null) _agentBehavior = GetComponent<AgentBehaviour>(); //similar to the navmeshagent
    }

    private void OnEnable()
    {
        _agentBehavior.Events.OnTargetChanged += OnTargetChanged;
        _agentBehavior.Events.OnTargetNotInRange += OnTargetNotInRange;
    }

    private void OnDisable()
    {
        _agentBehavior.Events.OnTargetChanged -= OnTargetChanged;
        _agentBehavior.Events.OnTargetNotInRange -= OnTargetNotInRange;
    }

    void Update()
    {
        if (_currentTarget == null) return;


        if (_controller.CanMove)
                _movement.MoveTo(_currentTarget.Position);
            else
            {
                _movement.Stop();
            }
    }

    private void OnTargetChanged(ITarget target, bool inRange)
    {
        _currentTarget = target;
        _movement.MoveTo(_currentTarget.Position);
    }


    private void OnTargetNotInRange(ITarget target)
    {
        
    }

    
    
}