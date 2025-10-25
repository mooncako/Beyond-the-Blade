using UnityEngine;
using Animancer;
using System;

[Serializable]
public class ReviveAnimationState : AnimationState
{
    public ReviveAnimationState(AnimationStateMachine stateMachine, AnimancerComponent animancer)
    {
        _stateMachine = stateMachine;
        _animancer = animancer;
        Key = "Revive";
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
