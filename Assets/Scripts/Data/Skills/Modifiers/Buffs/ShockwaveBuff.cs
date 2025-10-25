using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shockwave", menuName = "Modifier/Buffs/ShockwaveBuff")]
public class ShockwaveBuff : ModifierSO
{
    //AOE Knock back
    public float KnockbackForce = 2000f;

    public override void Perform(in ModifierContext context)
    {
        if (context.Caster is PlayerController p)
        {
            List<GameObject> affectedEntities = p.AOEApplier.GetDamagedEntities(p.GetCurrentSkill().SkillRange.AreaType, p.AttackPoint.position, p.AttackableMask);
            for (int i = 0; i < affectedEntities.Count; i++)
            {
                EnemyController enemy = affectedEntities[i].GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.Movement.KnockBack(p.transform, KnockbackForce);
                }
            }
        }
        else if (context.Caster is EnemyController e)
        {
            List<GameObject> affectedEntities = e.AOEApplier.GetDamagedEntities(e.GetCurrentSkill().SkillRange.AreaType, e.AttackPoint.position, e.AttackableMask);
            for (int i = 0; i < affectedEntities.Count; i++)
            {
                PlayerController player = affectedEntities[i].GetComponent<PlayerController>();
                if (player != null)
                {
                    player.Movement.KnockBack(e.transform, KnockbackForce);
                }
            }
        }
    }
}
