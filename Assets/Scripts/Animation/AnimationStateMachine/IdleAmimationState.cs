using System;
using Animancer;
using UnityEngine;
using Animancer.TransitionLibraries;

[Serializable]
public class IdleAnimationState : AnimationState
{
    public IdleAnimationState()
    {

    }

    public IdleAnimationState(AnimationStateMachine stateMachine, AnimancerComponent animancer, ClipTransition clip)
    {
        _stateMachine = stateMachine;
        _animancer = animancer;
        Key = "Idle";
        Clip = clip;
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
