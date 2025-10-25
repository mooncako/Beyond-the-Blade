using System;
using Animancer;

[Serializable]
public class StaggerAnimationState : AnimationState
{
    public StaggerAnimationState(AnimationStateMachine stateMachine, AnimancerComponent animancer)
    {
        _stateMachine = stateMachine;
        _animancer = animancer;
        Key = "Stagger";
    }
   

    public override void OnEnterState()
    {
        base.OnEnterState();
        Owner?.ToggleIsSkillPlaying(false);
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
