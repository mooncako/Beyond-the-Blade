using UnityEngine;

[CreateAssetMenu(fileName = "Teleport", menuName = "Modifier/Buffs/TeleportBuff")]
public class TeleportBuff : ModifierSO
{
    public override void Perform(Controller controller)
    {
        base.Perform(controller);
        if (controller is PlayerController)
        {
            PlayerController pController = controller as PlayerController;

            pController.Movement.Teleport(pController.GetAimPoint());
        }
        else
        {
            EnemyController eController = controller as EnemyController;
            eController.Movement.Teleport(eController.CurrentTargetTransform.forward);
        }
        
    }
}
