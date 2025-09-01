using System;
using Animancer;
using UnityEngine;
using Animancer.TransitionLibraries;
using System.Collections;
using PrimeTween;

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

    public ActionAnimationState(AnimationStateMachine stateMachine, AnimancerComponent animancer, string key)
    {
        _stateMachine = stateMachine;
        _animancer = animancer;
        Key = key;
    }

    public override void OnEnterState()
    {
        if (Owner != null)
            Owner.CanMove = false;
        if (Clip.Clip != null)
        {
            AnimancerState = _animancer.Play(Clip);
            AnimancerState state = _animancer.States.Current;
            state.Events(this).OnEnd = () => _stateMachine.SwitchState(AnimationStateType.Idle);
        }
        else
        {
            Tween.Delay(.5f).OnComplete(() =>
            {
                _stateMachine.SwitchState(AnimationStateType.Idle);
            });
        }
        
    }


    public override void OnInterrupt()
    {
        base.OnInterrupt();
    }

    public override void OnExitState()
    {
        base.OnExitState();
        if(Owner != null)
            Owner.CanMove = true;
    }
}
