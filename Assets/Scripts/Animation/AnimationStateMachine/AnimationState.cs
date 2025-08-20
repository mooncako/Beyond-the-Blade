using System;
using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class AnimationState
{
    public ClipState State;
    [SerializeField, FoldoutGroup("References")] protected AnimationStateMachine _stateMachine;
    [SerializeField, FoldoutGroup("References")] protected AnimancerComponent _animancer;

    [SerializeField] private AnimationClip _defaultClip;
    public string Key;
    public float FadeDuration = 0;
    public FadeMode FadeMode;
    public AnimationState NextState;



    public AnimationState()
    {

    }

    public AnimationState(AnimationStateMachine stateMachine, AnimancerComponent animancer)
    {
        _stateMachine = stateMachine;
        _animancer = animancer;

    }


    public virtual void OnEnterState()
    {
        _animancer.TryPlay(State, FadeDuration, FadeMode);
    }

    public virtual void OnInterrupt()
    {

    }

    public virtual void OnExitState()
    {

    }

    public void RefreshState()
    {
        if (_defaultClip != null)
        {
            State.Clip = _defaultClip;
            State.Key = Key;
        }
        
    }
}
