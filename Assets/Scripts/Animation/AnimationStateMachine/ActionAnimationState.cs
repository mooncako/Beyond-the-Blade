using System;
using Animancer;
using UnityEngine;
using Animancer.TransitionLibraries;
using System.Collections;
using PrimeTween;
using System.Collections.Generic;

[Serializable]
public class ActionAnimationState : AnimationState
{
    private AnimancerEvent.Sequence _events;

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
            AnimancerState.Speed *= Owner.Stats.AttackSpeed;
            AddEvents(AnimancerState);
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
        Modifiers.Clear();
        if (Owner != null)
            Owner.CanMove = true;
    }
    
    public void UpdateModifiers(List<(float, ModifierSO)> modifiers)
    {
        Modifiers.Clear();
        for (int i = 0; i < modifiers.Count; i++)
        {
            Modifiers.Add((modifiers[i].Item1, modifiers[i].Item2));
        }
    }

    public void AddEvents(AnimancerState state)
    {
        foreach (var modifierTuple in Modifiers)
        {
            state.Events(this).Add(modifierTuple.eventIndex, () => Owner.ModifierRelayAnimEvent(modifierTuple.modifier));
        }
        state.Events(this).OnEnd ??= () => _stateMachine.SwitchState(AnimationStateType.Idle);

    }
}
