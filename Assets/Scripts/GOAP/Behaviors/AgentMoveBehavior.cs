using System;
using System.Collections;
using CrashKonijn.Agent.Core;
using CrashKonijn.Agent.Runtime;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyController))]
public class AgentMoveBehavior : MonoBehaviour
{
    // [SerializeField, FoldoutGroup("References")] private Animator _animator;
    [SerializeField, FoldoutGroup("References")] private Agent _agent;
    [SerializeField, FoldoutGroup("References")] private EnemyController _controller;
    [SerializeField, FoldoutGroup("References")] private AnimationStateMachine _animationStateMachine;

    private void OnValidate()
    {
        // if (_animator == null) _animator = GetComponent<Animator>();
        if (_controller == null) _controller = GetComponent<EnemyController>();
        if (_agent == null) _agent = GetComponent<Agent>();
        if (_animationStateMachine == null) _animationStateMachine = GetComponent<AnimationStateMachine>();

    }

    private void OnEnable()
    {
        _agent.OnStateChanged += OnStateChanged;
    }

    private void OnDisable()
    {
        _agent.OnStateChanged -= OnStateChanged;
    }

    void FixedUpdate()
    {
        
        if (_agent.Blackboard == null || _agent.Blackboard.TargetTransform == null) 
        {
            _controller.Movement.Stop();
            return;
        }
        if (!_controller.CanMove) 
        {
            _controller.Movement.Stop();
            return;
        }
        if (_controller.AnimationStateMachine.IsInStaggerState())
        {
            _controller.Movement.Stop();
            return;
        }
        
        if(_agent.GetNextMovementPosition(out Vector3 nextPosition))
        {
            _controller.MoveTo(nextPosition);
            // transform.position = Vector3.MoveTowards(transform.position, nextPosition, Time.fixedDeltaTime * 3f);
        }
    }

    private void OnStateChanged(AgentStateSO newState)
    {
        
    }

    
    
}