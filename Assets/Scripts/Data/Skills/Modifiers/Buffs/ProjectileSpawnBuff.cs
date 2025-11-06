using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileSpawn", menuName = "Modifier/Buffs/ProjectileSpawnBuff")]
public class ProjectileSpawnBuff : ModifierSO
{
    public override void Perform(in ModifierContext context)
    {
        Vector3 spawnPos = context.Caster.AttackPoint ? context.Caster.AttackPoint.position : context.Caster.transform.position;

        SpawnProjectileEvent.Trigger(context.Caster.GetCurrentSkill().AnimationID, spawnPos, context.Caster.transform.forward, context.Caster.gameObject);
    }
}
