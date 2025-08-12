using System;
using System.Collections;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineBasicMultiChannelPerlin))]
public class CameraShake : MonoBehaviour, MMEventListener<CameraShakeEvent>
{
    [SerializeField, FoldoutGroup("References")] private CinemachineBasicMultiChannelPerlin _cinemachineNoise;

    void OnValidate()
    {
        if(_cinemachineNoise == null) _cinemachineNoise = GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void OnEnable()
    {
        this.MMEventStartListening<CameraShakeEvent>();
    }

    private void OnDisable()
    {
        this.MMEventStopListening<CameraShakeEvent>();
    }

    public void OnMMEvent(CameraShakeEvent e)
    {
        OnCameraShake(e.Setting);
    }


    private void OnCameraShake(CameraShakeSettings shakeSettings)
    {
        _cinemachineNoise.AmplitudeGain = shakeSettings.Amplitude;
        _cinemachineNoise.FrequencyGain = shakeSettings.Frequency;
        StartCoroutine(CameraShakeCO(shakeSettings.Duration));
    }

    private IEnumerator CameraShakeCO(float duration)
    {
        yield return new WaitForSeconds(duration);

        _cinemachineNoise.AmplitudeGain = 0;
        _cinemachineNoise.FrequencyGain = 0;
    }

    
}
