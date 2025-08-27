using System;
using Animancer;
using UnityEngine;

[Serializable]
public class StaggerAnimationState : AnimationState
{
    public StaggerAnimationState(AnimationStateMachine stateMachine, AnimancerComponent animancer)
    {
        _stateMachine = stateMachine;
        _animancer = animancer;
        Key = "Stagger";
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnEnterState()
    {
        _animancer.Play(Clip);
    }
    public override void OnInterrupt()
    {
        base.OnInterrupt();
    }

    public override void OnExitState()
    {
        base.OnExitState();
    }
}
