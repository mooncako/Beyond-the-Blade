using UnityEngine;

[CreateAssetMenu(fileName = "Teleport", menuName = "Modifier/Buffs/TeleportBuff")]
public class TeleportBuff : ModifierSO
{
    public override void Perform(in ModifierContext context)
    {
        if (context.Caster is PlayerController p)
        {
            p.Movement.Teleport(p.GetAimPoint());
        }
        else if(context.Caster is EnemyController e)
        {
            e.Movement.Teleport(e.CurrentTargetTransform.forward);
        }
        
    }
}
