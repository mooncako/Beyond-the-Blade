using System;
using Animancer;
using UnityEngine;
using Animancer.TransitionLibraries;

[Serializable]
public class MoveAnimationState : AnimationState
{
    public MoveAnimationState()
    {

    }

    public MoveAnimationState(AnimationStateMachine stateMachine, AnimancerComponent animancer, ClipTransition clip)
    {
        _stateMachine = stateMachine;
        _animancer = animancer;
        Key = "Move";
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
