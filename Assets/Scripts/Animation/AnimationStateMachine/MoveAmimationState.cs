using System;
using Animancer;
using UnityEngine;

[Serializable]
public class MoveAnimationState : AnimationState
{
    public MoveAnimationState()
    {

    }

    public MoveAnimationState(AnimationStateMachine stateMachine, AnimancerComponent animancer)
    {
        _stateMachine = stateMachine;
        _animancer = animancer;
        Key = "Move";
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
