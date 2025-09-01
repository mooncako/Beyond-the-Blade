using System.Collections;
using Animancer;
using Animancer.FSM;
using PrimeTween;
using UnityEngine;

public static class HitStop
{
    public static void Begin(AnimancerState state, float duration, Tween delayTween)
    {
        float defaultSpeed= state.Speed;
        state.Speed = 0;
        delayTween.Stop();
        delayTween = Tween.Delay(duration).OnComplete(() =>
        {
            state.Speed = defaultSpeed;
        });
    }

    
}
