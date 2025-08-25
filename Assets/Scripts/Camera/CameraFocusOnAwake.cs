using UnityEngine;

public class CameraFocusOnAwake : MonoBehaviour
{
    void Start()
    {
        AssignCamTargetEvent.Trigger(transform);
    }
}
