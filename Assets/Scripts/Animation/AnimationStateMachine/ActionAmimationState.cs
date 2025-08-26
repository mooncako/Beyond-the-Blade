using System;
using Animancer;
using UnityEngine;
using Animancer.TransitionLibraries;

[Serializable]
public class ActionAnimationState : AnimationState
{
    public ActionAnimationState()
    {

    }

    public ActionAnimationState(AnimationStateMachine stateMachine, AnimancerComponent animancer)
    {
        _stateMachine = stateMachine;
        _animancer = animancer;
        Key = "Action";
    }

    public override void OnEnterState()
    {
        Owner.CanMove = false;
        _animancer.Play(Clip);
        AnimancerState state = _animancer.States.Current;
        state.Events(this).OnEnd = () => _stateMachine.SwitchState(AnimationStateType.Idle);
    }

  

    public override void OnInterrupt()
    {
        base.OnInterrupt();
    }

    public override void OnExitState()
    {
        base.OnExitState();
        Owner.CanMove = true;
    }
}
