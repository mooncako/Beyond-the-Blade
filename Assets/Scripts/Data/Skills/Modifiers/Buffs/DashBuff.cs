using UnityEngine;

[CreateAssetMenu(fileName = "Dash", menuName = "Modifier/Buffs/DashBuff")]
public class DashBuff : ModifierSO
{
    //dash forward when attack
    public float DashForce = 2000f;

    public override void Perform(in ModifierContext context)
    {
        if (context.Caster is PlayerController p)
        {
            ApplyDash(p);
        }
        else if (context.Caster is EnemyController e)
        {
            ApplyDash(e);
        }
    }

    private void ApplyDash(Controller controller)
    {
        Vector3 forwardDirection = controller.transform.forward;
        controller.Movement.Dash(forwardDirection, DashForce);
    }
}
