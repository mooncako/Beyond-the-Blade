using MoreMountains.Tools;
using UnityEngine;

public struct SpawnOffloadAOEEvent
{
    public Skill Skill;
    public Transform Transform;

    public SpawnOffloadAOEEvent(Skill skill, Transform transform)
    {
        Skill = skill;
        Transform = transform;
    }

    public static SpawnOffloadAOEEvent e;
    public static void Trigger(Skill skill, Transform transform)
    {
        e.Skill = skill;
        e.Transform = transform;
        MMEventManager.TriggerEvent(e);
    }
}
