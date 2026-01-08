using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "EnergySurge", menuName = "Modifier/Buffs/EnergySurgeBuff")]
public class EnergySurgeBuff : ModifierSO
{
    [SerializeField, BoxGroup("Settings")] private float _surgeAmount = 1;

    public override void Perform(in ModifierContext context)
    {
        var energy = context.Caster.Energy;
        energy.GainEnergy(_surgeAmount);
    }
}
