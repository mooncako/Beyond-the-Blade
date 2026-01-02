

using MoreMountains.Tools;
using UnityEngine;

public struct SpawnContinuousAOEEvent
{
    public Skill Skill;
    public Transform Transform;
    public float DamageTickTime;
    public float SkillDuration;
    public DamageType DamageType;
    public GameObject Instigator;

    public SpawnContinuousAOEEvent(Skill skill, Transform transform, float damageTickTime, float skillDuration, DamageType damageType, GameObject instigator)
    {
        Skill = skill;
        Transform = transform;
        DamageTickTime = damageTickTime;
        SkillDuration = skillDuration;
        DamageType = damageType;
        Instigator = instigator;
    }

    public static SpawnContinuousAOEEvent e;
    public static void Trigger(Skill skill, Transform transform, float damageTickTime, float skillDuration, DamageType damageType, GameObject instigator)
    {
        e.Skill = skill;
        e.Transform = transform;
        e.DamageTickTime = damageTickTime;
        e.SkillDuration = skillDuration;
        e.DamageType = damageType;
        e.Instigator = instigator;
        MMEventManager.TriggerEvent(e);
    }
}
