using Animancer;
using UnityEngine;

public class StaggerAnimationState : AnimationState
{
    public StaggerAnimationState(AnimationStateMachine stateMachine, AnimancerComponent animancer)
    {
        _stateMachine = stateMachine;
        _animancer = animancer;
        Key = "Stagger";
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnEnterState()
    {
        if (Owner != null)
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
        if (Owner != null)
            Owner.CanMove = true;
    }
}
