using PrimeTween;
using UnityEngine;

[CreateAssetMenu(fileName = "BackStep", menuName = "Modifier/Buffs/BackStepBuff")]
public class BackStepBuff : ModifierSO
{
    //evade backward for a certain distance
    public float BackStepDistance = 2f;
    public float BackStepForce = 2000f;

    public override void Perform(in ModifierContext context)
    {
        if (context.Caster is PlayerController p)
        {
            ApplyBackStep(p);
        }
        else if (context.Caster is EnemyController e)
        {
            ApplyBackStep(e);
        }
    }

    private void ApplyBackStep(Controller controller)
    {
       
        Vector3 backwardDirection = -controller.transform.forward;
        controller.Movement.Dash(backwardDirection, BackStepForce);
    }
}
