using UnityEngine;

[CreateAssetMenu(fileName = "Doppelganger", menuName = "Modifier/Buffs/DoppelgangerBuff")]
public class DoppelgangerBuff : ModifierSO
{
    public override void Perform(in ModifierContext context)
    {
        context.Caster.GetComponentInChildren<DoppelgangerSpawner>().Spawn(context.Caster);
    }
}
