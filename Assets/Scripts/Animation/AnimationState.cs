using System;
using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class AnimationState
{
    public AnimancerState State;
    [SerializeField, FoldoutGroup("References")] private AnimationStateMachine _stateMachine;
    [SerializeField, FoldoutGroup("References")] private AnimancerComponent _animancer;

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

    public void OnEnterState()
    {
        _animancer.TryPlay(State, FadeDuration, FadeMode);
    }

    public void OnInterrupt()
    {

    }

    public void OnExitState()
    {

    }
}
