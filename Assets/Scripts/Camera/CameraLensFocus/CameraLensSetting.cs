using System;
using UnityEngine;


[Serializable]
public class CameraLensSetting
{
    public float Fov;

    public CameraLensSetting()
    {
        Fov = 15f;
    }

    public CameraLensSetting(float fov)
    {
        Fov = fov;
    }
}
