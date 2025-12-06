using System;
using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;
using Animancer.TransitionLibraries;
using CrashKonijn.Agent.Runtime;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public abstract class AnimationState
{
    public ClipState State;
    public AnimancerState AnimancerState;
    [SerializeField, FoldoutGroup("References")] protected AnimationStateMachine _stateMachine;
    [SerializeField, FoldoutGroup("References")] protected AnimancerComponent _animancer;
    [SerializeField, FoldoutGroup("References")] public Controller Owner;
    [SerializeField, BoxGroup("Modifiers")] public List<(float eventIndex, ModifierSO modifier)> Modifiers = new List<(float, ModifierSO)>();

    // [SerializeField] private ClipTransition _defaultClip;
    public ClipTransition Clip;
    private ClipTransition _defaultClip;
    public string Key;
    public float FadeDuration = 0;
    public FadeMode FadeMode;
    public AnimationState NextState;
    public AnimationStateType PossibleInterruptStates;
    [HideInInspector, SerializeField] private AnimationStateType _emptyStates;
    [HideInInspector, SerializeField] private AnimationStateType _tempStates;
    [HideInInspector, SerializeField] protected bool _isInterruptionBlocked = false;


    public AnimationState()
    {

    }


    public AnimationState(AnimationStateMachine stateMachine, AnimancerComponent animancer, ClipTransition clip)
    {
        _stateMachine = stateMachine;
        _animancer = animancer;
        Clip = clip;
        _defaultClip = Clip;
    }


    public virtual void OnEnterState()
    {
        if (Clip != null && Clip.Clip != null)
        {
            AnimancerState = _animancer.Play(Clip);
        }


    }

    public virtual void OnInterrupt()
    {

    }

    public virtual void OnExitState()
    {
        if (_isInterruptionBlocked)
        {
            ToggleInterruption(true);
        }

        Owner.ToggleRootMotionOffAnimEvent();
    }

    public void RefreshState()
    {
        // if (_defaultClip != null)
        // {
        //     Clip = _defaultClip;
        // }


    }

    public void SwapClip(ClipTransition clip)
    {
        Clip = clip;
        AnimancerState = _animancer.Play(Clip);
    }

    public virtual void ToggleInterruption(bool toggle)
    {
        if (toggle)
        {
            _isInterruptionBlocked = false;
            PossibleInterruptStates = _tempStates;
        }
        else
        {
            
            _isInterruptionBlocked = true;
            _tempStates = PossibleInterruptStates;
            PossibleInterruptStates = _emptyStates;
        }
    }

 
    

}
