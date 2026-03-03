using MoreMountains.Tools;
using UnityEngine;

public struct WitchTimeEvent
{
    public float Duration;
    public float TimeScale;
    public TeamType TargetTeam;

    public WitchTimeEvent(float duration, float timeScale, TeamType targetTeam)
    {
        Duration = duration;
        TimeScale = timeScale;
        TargetTeam = targetTeam;
    }

    public static WitchTimeEvent e;
    public static void Trigger(float duration, float timeScale, TeamType targetTeam)
    {
        e.Duration = duration;
        e.TimeScale = timeScale;
        e.TargetTeam = targetTeam;
        MMEventManager.TriggerEvent(e);
    }
}
