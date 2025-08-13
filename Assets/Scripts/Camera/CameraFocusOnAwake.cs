using UnityEngine;

public class CameraFocusOnAwake : MonoBehaviour
{
    void Awake()
    {
        AssignCamTargetEvent.Trigger(transform);
    }
}
