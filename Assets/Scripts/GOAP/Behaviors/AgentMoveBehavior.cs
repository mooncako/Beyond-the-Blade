using System;
using System.Collections;
using CrashKonijn.Agent.Core;
using CrashKonijn.Agent.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyController), typeof(AgentBehaviour))]
public class AgentMoveBehavior : MonoBehaviour
{
    [SerializeField, FoldoutGroup("References")] private Animator _animator;
    [SerializeField, FoldoutGroup("References")] private AgentBehaviour _agentBehavior;
    [SerializeField, FoldoutGroup("References")] private EnemyController _controller;
    [SerializeField, FoldoutGroup("References")] private AnimationStateMachine _animationStateMachine;


    [SerializeField, BoxGroup("Debug"), ReadOnly] private ITarget _currentTarget;

    private void OnValidate()
    {
        if (_animator == null) _animator = GetComponent<Animator>();
        if (_controller == null) _controller = GetComponent<EnemyController>();
        if (_agentBehavior == null) _agentBehavior = GetComponent<AgentBehaviour>(); //similar to the navmeshagent
        if (_animationStateMachine == null) _animationStateMachine = GetComponent<AnimationStateMachine>();

    }

    private void OnEnable()
    {
        _agentBehavior.Events.OnTargetChanged += OnTargetChanged;
        _agentBehavior.Events.OnTargetNotInRange += OnTargetNotInRange;
        // _controller.Movement.Stop();
        // StartCoroutine(MovementCO());
    }

    private void OnDisable()
    {
        _agentBehavior.Events.OnTargetChanged -= OnTargetChanged;
        _agentBehavior.Events.OnTargetNotInRange -= OnTargetNotInRange;
        // StopCoroutine(MovementCO());
    }

    void Update()
    {
        if (_agentBehavior.IsPaused) return;
        if (_currentTarget == null) return;
        if (!_controller.CanMove) return;

        // if(!AnimationStateMachine.IsInMoveState()) return;
        
        _controller.MoveTo(_currentTarget.Position);
    }

    private void OnTargetChanged(ITarget target, bool inRange)
    {
        _currentTarget = target;
        _controller.MoveTo(_currentTarget.Position);
    }


    private void OnTargetNotInRange(ITarget target)
    {
        
    }

    private IEnumerator MovementCO()
    {
        while(true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(.1f, .5f));
            if (_agentBehavior.IsPaused) continue;
            if (_currentTarget == null) continue;
            if (!_controller.CanMove) continue;

            // if(!AnimationStateMachine.IsInMoveState()) return;
            _controller.MoveTo(_currentTarget.Position);
        }
        
    }
    
    
}