using MoreMountains.Tools;
using UnityEngine;

public struct AssignCamTargetEvent
{
    public Transform Target;

    public AssignCamTargetEvent(Transform target)
    {
        Target = target;
    }

    private static AssignCamTargetEvent e;

    public static void Trigger(Transform target)
    {
        e.Target = target;
        MMEventManager.TriggerEvent(e);
    }
}
