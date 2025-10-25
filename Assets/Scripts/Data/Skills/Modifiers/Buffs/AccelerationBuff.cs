using PrimeTween;
using UnityEngine;

[CreateAssetMenu(fileName = "Acceleration", menuName = "Modifier/Buffs/AccelerationBuff")]
public class AccelerationBuff : ModifierSO
{
    //double the movement speed for 3 seconds
    public float SpeedMultiplier = 1f; 
    public float Duration = 3f;

    public override void Perform(in ModifierContext context)
    {
        ApplyAccelerationBuff(context.Caster);
    }

    private void ApplyAccelerationBuff(Controller controller)
    {
        controller.Stats.TempMovementSpeedMultiplier += SpeedMultiplier;
        controller.OnStatsUpdated();
        Tween.Delay(Duration).OnComplete(() =>
        {
            if (controller != null && controller.Stats != null)
            {
                controller.Stats.TempMovementSpeedMultiplier -= SpeedMultiplier;
                controller.OnStatsUpdated();
            }
        });
    }
}
