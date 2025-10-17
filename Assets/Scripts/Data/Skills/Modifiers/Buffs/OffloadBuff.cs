using UnityEngine;

[CreateAssetMenu(fileName = "Offload AOE", menuName = "Modifier/Buffs/OffloadBuff")]
public class OffloadBuff : ModifierSO
{
    public override void Perform(in ModifierContext context)
    {
        SpawnOffloadAOEEvent.Trigger(context.Caster.GetCurrentSkill(), context.Caster.transform);
    }
}
