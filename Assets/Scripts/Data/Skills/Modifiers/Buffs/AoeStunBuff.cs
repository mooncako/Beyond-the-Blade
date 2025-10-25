using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AOEStun", menuName = "Modifier/Buffs/AOEStunBuff")]
public class AoeStunBuff : ModifierSO
{
    //TODO: Trigger VFX

    public float StunDuration = 1;

    public override void Perform(in ModifierContext context)
    {
        if (context.Caster is PlayerController p)
        {
            List<GameObject> affectedEntities = p.AOEApplier.GetDamagedEntities(p.GetCurrentSkill().SkillRange.AreaType, p.AttackPoint.position, p.AttackableMask);
            for(int i = 0; i < affectedEntities.Count; i++)
            {
                affectedEntities[i].GetComponent<EnemyController>().Stun(StunDuration);
            }
        }
        else if (context.Caster is EnemyController e)
        {
            List<GameObject> affectedEntities = e.AOEApplier.GetDamagedEntities(e.GetCurrentSkill().SkillRange.AreaType, e.AttackPoint.position, e.AttackableMask);
            for(int i = 0; i < affectedEntities.Count; i++)
            {
                affectedEntities[i].GetComponent<PlayerController>().Stun(StunDuration);
            }
        }
        
    }
}
