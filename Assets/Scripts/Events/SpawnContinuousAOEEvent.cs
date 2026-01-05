

using MoreMountains.Tools;
using UnityEngine;

public struct SpawnContinuousAOEEvent
{
    public Skill Skill;
    public Transform Transform;
    public float SkillDuration;
    public GameObject Instigator;
    public LayerMask EnemyMask;

    public SpawnContinuousAOEEvent(Skill skill, Transform transform, float skillDuration, GameObject instigator, LayerMask enemyMask)
    {
        Skill = skill;
        Transform = transform;
        SkillDuration = skillDuration;
        Instigator = instigator;
        EnemyMask = enemyMask;
    }

    public static SpawnContinuousAOEEvent e;
    public static void Trigger(Skill skill, Transform transform, float skillDuration, GameObject instigator, LayerMask enemyMask)
    {
        e.Skill = skill;
        e.Transform = transform;
        e.SkillDuration = skillDuration;
        e.Instigator = instigator;
        e.EnemyMask = enemyMask;
        MMEventManager.TriggerEvent(e);
    }
}
