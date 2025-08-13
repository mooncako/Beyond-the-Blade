using MoreMountains.Tools;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineCamera))]
public class CameraTargetAssigner : MonoBehaviour, MMEventListener<AssignCamTargetEvent>
{
    [SerializeField, BoxGroup("References")] private CinemachineCamera _cam;

    void OnValidate()
    {
        if(_cam == null) _cam = GetComponent<CinemachineCamera>();
    }

    void OnEnable()
    {
        this.MMEventStartListening<AssignCamTargetEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<AssignCamTargetEvent>();
    }

    public void OnMMEvent(AssignCamTargetEvent e)
    {
        _cam.Target.TrackingTarget = e.Target;
    }
}
