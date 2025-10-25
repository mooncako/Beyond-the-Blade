using PrimeTween;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Modifier/Buffs/AttackBuff")]
public class AttackBuff : ModifierSO
{
    //double the attack for 3 seconds
    public float DamageMultiplierIncrease = 1f;
    public float Duration = 3f;

    public override void Perform(in ModifierContext context)
    {
        ApplyAttackBuff(context.Caster);
    }

    private void ApplyAttackBuff(Controller controller)
    {
        controller.Stats.TempDamageMultiplier += DamageMultiplierIncrease;
        controller.OnStatsUpdated();
        Tween.Delay(Duration).OnComplete(() =>
        {
            if (controller != null && controller.Stats != null)
            {
                controller.Stats.TempDamageMultiplier -= DamageMultiplierIncrease;
                controller.OnStatsUpdated();
            }
        });
    }
}
