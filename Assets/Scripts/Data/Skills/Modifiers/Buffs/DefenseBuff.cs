using PrimeTween;
using UnityEngine;

[CreateAssetMenu(fileName = "Defense", menuName = "Modifier/Buffs/DefenseBuff")]
public class DefenseBuff : ModifierSO
{
    //double damage reduction for 3 seconds
    public float DamageReductionIncrease = 1f;
    public float Duration = 3f;

    public override void Perform(in ModifierContext context)
    {
        if (context.Caster is PlayerController p)
        {
            ApplyDefenseBuff(p);
        }
        else if (context.Caster is EnemyController e)
        {
            ApplyDefenseBuff(e);
        }
    }

    private void ApplyDefenseBuff(Controller controller)
    {
        controller.Stats.TempDamageReduction += DamageReductionIncrease;
        controller.OnStatsUpdated();
        Tween.Delay(Duration).OnComplete(() =>
        {
            if (controller != null && controller.Stats != null)
            {
                controller.Stats.TempDamageReduction -= DamageReductionIncrease;
                controller.OnStatsUpdated();
            }
        });
    }
}
