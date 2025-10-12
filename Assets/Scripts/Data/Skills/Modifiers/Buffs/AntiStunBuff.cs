using PrimeTween;
using UnityEngine;

[CreateAssetMenu(fileName = "AntiStun", menuName = "Modifier/Buffs/AntiStunBuff")]
public class AntiStunBuff : ModifierSO
{
    //immune to stun in the duration of the length of the skill
    public float Duration = 1f;

    //TODO: currently only works for player, add to enemy later
    public override void Perform(in ModifierContext context)
    {
        if (context.Caster is PlayerController p)
        {
            ApplyStunImmunity(p);
        }
    }

    private void ApplyStunImmunity(Controller controller)
    {
        controller.IsStunImmune = true;
        Tween.Delay(Duration).OnComplete(() =>
        {
            if (controller != null)
            {
                controller.IsStunImmune = false;
            }
        });
    }
}
