using System;
using UnityEngine;

[Serializable]
public struct VFXInfo
{
    public Vector3 Pos;
    public Quaternion Rot;
    public Vector3 Scale;
    public bool IsPersistent;


    public VFXInfo(Vector3 pos, Quaternion rot, Vector3 scale, bool stayInParent)
    {
        Pos = pos;
        Rot = rot;
        Scale = scale;
        IsPersistent = stayInParent;
    }
}
