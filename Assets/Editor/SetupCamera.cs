using UnityEngine;
using UnityEditor;
using Unity.Cinemachine;

public class SetupCamera : MonoBehaviour
{
    [MenuItem("GameObject/PlayerTrackingCam", false, 0)]
    private static void CreateNewCinemachineCam(MenuCommand command)
    {
        GameObject obj = new GameObject("CinemachineCamera");

        CinemachineCamera cam = obj.AddComponent<CinemachineCamera>();
        cam.Lens.FieldOfView = 20f;

        CinemachinePositionComposer composer = obj.AddComponent<CinemachinePositionComposer>();
        composer.CameraDistance = 24f;

        CinemachineBasicMultiChannelPerlin shake = obj.AddComponent<CinemachineBasicMultiChannelPerlin>();
        shake.AmplitudeGain = 0;
        shake.FrequencyGain = 0;

        obj.AddComponent<CameraShake>();
        obj.AddComponent<CameraTargetAssigner>();
    }
}
