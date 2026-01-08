using MoreMountains.Tools;
using UnityEngine;

public struct CameraRotateEvent
{
    public Vector2 Input;

    public CameraRotateEvent(Vector2 input)
    {
        Input = input;
    }


    public static CameraRotateEvent e;
    public static void Trigger(Vector2 input)
    {
        e.Input = input;
        MMEventManager.TriggerEvent(e);
    }
}
