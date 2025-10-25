using PrimeTween;
using UnityEngine;

[CreateAssetMenu(fileName = "Invincible", menuName = "Modifier/Buffs/InvincibleBuff")]
public class InvincibleBuff : ModifierSO
{
    //immune to damage in the duration of the length of the skill
    public float Duration = 1f;

    public override void Perform(in ModifierContext context)
    {
        if (context.Caster is PlayerController p)
        {
            ApplyInvincibility(p);
        }
        else if (context.Caster is EnemyController e)
        {
            ApplyInvincibility(e);
        }
    }

    private void ApplyInvincibility(Controller controller)
    {
        controller.Health.IsDamageable = false;
        Tween.Delay(Duration).OnComplete(() =>
        {
            if (controller != null && controller.Health != null)
            {
                controller.Health.IsDamageable = true;
            }
        });
    }
}
