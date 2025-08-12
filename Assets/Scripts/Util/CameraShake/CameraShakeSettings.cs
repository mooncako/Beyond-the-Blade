using System;
using UnityEngine;

[Serializable]
public class CameraShakeSettings
{
    public float Duration;
    public float Amplitude;
    public float Frequency;

    public CameraShakeSettings(float duration = .5f, float amplitude = 5, float frequency = 5)
    {
        Duration = duration;
        Amplitude = amplitude;
        Frequency = frequency;
    }
}
