using UnityEngine;

[CreateAssetMenu(fileName = "DepleteEnergy", menuName = "Modifier/NonDroppable/DepleteEnergy")]
public class DepleteEnergy : ModifierSO
{
    public override void Perform(in ModifierContext context)
    {
        var energy = context.Caster.Energy;
        energy.DepletesEnergy(999);
    }
}
