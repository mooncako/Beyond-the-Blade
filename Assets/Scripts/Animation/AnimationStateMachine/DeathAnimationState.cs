using System;
using Animancer;
using UnityEngine;
using Animancer.TransitionLibraries;

[Serializable]
public class DeathAnimationState : AnimationState
{
    public DeathAnimationState()
    {

    }

    public DeathAnimationState(AnimationStateMachine stateMachine, AnimancerComponent animancer)
    {
        _stateMachine = stateMachine;
        _animancer = animancer;
        Key = "Death";
        PossibleInterruptStates = AnimationStateType.None;
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

