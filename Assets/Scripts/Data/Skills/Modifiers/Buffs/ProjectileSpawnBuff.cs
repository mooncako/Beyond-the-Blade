using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileSpawn", menuName = "Modifier/Buffs/ProjectileSpawnBuff")]
public class ProjectileSpawnBuff : ModifierSO
{
    public override void Perform(in ModifierContext context)
    {
        SpawnProjectileEvent.Trigger(context.Caster.GetCurrentSkill().AnimationID, context.Caster.transform.forward, context.Caster.transform.forward, context.Caster.gameObject);
    }
}
