using System;
using Animancer;
using UnityEngine;
using Animancer.TransitionLibraries;
using CrashKonijn.Agent.Runtime;
using PrimeTween;

[Serializable]
public class IdleAnimationState : AnimationState
{
    public IdleAnimationState()
    {

    }

    public IdleAnimationState(AnimationStateMachine stateMachine, AnimancerComponent animancer, ClipTransition clip)
    {
        _stateMachine = stateMachine;
        _animancer = animancer;
        Key = "Idle";
        Clip = clip;
    }

    public IdleAnimationState(AnimationStateMachine stateMachine, AnimancerComponent animancer)
    {
        _stateMachine = stateMachine;
        _animancer = animancer;
        Key = "Idle";
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
        if (Owner != null)
        {
            if(_stateMachine.PreviousState is StaggerAnimationState) return;
            if (Owner.CurrentWeapon.InCombo)
            {
                Owner.ActivateSkill();
                if (Owner.CheckSkill())
                {
                    Tween.Delay(1).OnComplete(() =>
                    {
                        if (!Owner.IsSkillNull())
                        {
                            Owner.PlaySkillEffect();
                            Owner.AnimationStateMachine.SetAction(Owner.CurrentWeapon.GetAnimationClip(Owner.GetCurrentSkillAnimationID()), AnimationStateType.Attack, Owner.GetCurrentSkill());
                            Owner.AnimationStateMachine.InterruptState(AnimationStateType.Attack);
                        }
                        else
                        {
                            Owner.CanAttack = true;
                            Owner.ToggleIsSkillPlaying(false);
                        }

                    });
                }
                else
                {
                    if (!Owner.IsSkillNull())
                    {
                        Owner.PlaySkillEffect();
                        Owner.AnimationStateMachine.SetAction(Owner.CurrentWeapon.GetAnimationClip(Owner.GetCurrentSkillAnimationID()), AnimationStateType.Attack, Owner.GetCurrentSkill());
                        Owner.AnimationStateMachine.InterruptState(AnimationStateType.Attack);
                    }

                }
            }
        }

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
