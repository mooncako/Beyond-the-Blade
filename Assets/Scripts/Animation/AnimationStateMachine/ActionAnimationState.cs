using System;
using Animancer;
using UnityEngine;
using PrimeTween;
using System.Collections.Generic;
using Sirenix.OdinInspector;

[Serializable]
public class ActionAnimationState : AnimationState
{
    private AnimancerEvent.Sequence _events;

    [SerializeField, BoxGroup("Settings")] private bool _canNaturalInterrupt = true;
    [SerializeField, BoxGroup("Settings")] private bool _autoTransition = true;
    private bool _animationStarted;
    private float _animationSpeed;

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
        _onEnterEvent.Invoke();
        if (Owner != null)
        {
            Owner.CanMove = false;
            Owner.ToggleRootMotionOnAnimEvent();
        }
            
        if (Clip.Clip != null)
        {
            ToggleInterruption(false);
            AnimancerState = _animancer.Play(Clip);
            _animationSpeed = AnimancerState.Speed;
            AnimancerState.Speed = _animationSpeed * Owner.Stats.AttackSpeed * Owner.TimeScale.CurrentTimeScale;
            _animationStarted = true;
            AddEvents(AnimancerState);
        }
        else
        {
            if(_autoTransition)
            {
                Tween.Delay(.5f).OnComplete(() =>
                {
                    _stateMachine.SwitchState(AnimationStateType.Idle);
                });
            }
            
        }

    }

    public override void OnUpdateState()
    {
        base.OnUpdateState();
        if (_animationStarted)
        {
            AnimancerState.Speed = _animationSpeed * Owner.Stats.AttackSpeed * Owner.TimeScale.CurrentTimeScale;
        }
    }


    public override void OnInterrupt()
    {
        base.OnInterrupt();
    }

    public override void OnExitState()
    {
        base.OnExitState();
        if (Owner != null)
            Owner.CanMove = true;
        Owner?.StartAttackCooldown();
        Owner?.ToggleIsSkillPlaying(false);
        if(Owner is PlayerController pC)
        {
            pC.ToggleAimMode(false);
        }

    }
    
    public void UpdateModifiers(List<(float, ModifierSO)> modifiers)
    {
        for (int i = 0; i < modifiers.Count; i++)
        {
            Modifiers.Add((modifiers[i].Item1, modifiers[i].Item2));
        }
    }

    public void AddEvents(AnimancerState state)
    {
        state.Events(this).Clear();
        foreach (var modifierTuple in Modifiers)
        {
            state.Events(this).Add(modifierTuple.eventIndex, () => Owner.ModifierRelayAnimEvent(modifierTuple.modifier));
        }

        if(_canNaturalInterrupt)
        {
            state.Events(this).Add(.3f, () => ToggleInterruption(true));
        }

        if (_autoTransition)
        {
            state.Events(this).OnEnd ??= () =>
            {
                _stateMachine.SwitchState(AnimationStateType.Idle);
            };
        }

        Modifiers.Clear();
    }
}
