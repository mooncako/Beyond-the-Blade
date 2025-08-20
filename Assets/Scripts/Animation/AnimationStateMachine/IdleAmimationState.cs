using System;
using Animancer;
using UnityEngine;

[Serializable]
public class IdleAnimationState : AnimationState
{
    public IdleAnimationState()
    {

    }

    public IdleAnimationState(AnimationStateMachine stateMachine, AnimancerComponent animancer)
    {
        _stateMachine = stateMachine;
        _animancer = animancer;
        Key = "Idle";
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
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
