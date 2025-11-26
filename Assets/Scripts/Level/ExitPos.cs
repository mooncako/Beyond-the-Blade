
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class ExitPos : DrawPos
{
    [BoxGroup("References")] public SplineContainer Spline;

    void OnValidate()
    {
        if(Spline == null) Spline = GetComponent<SplineContainer>();
    }

    public Vector3 GetTeleportExit()
    {
        float3 splineExitPos = Spline.Spline.ToArray()[Spline.Spline.ToArray().Length - 1].Position;
        return new Vector3(transform.position.x + splineExitPos.x, transform.position.y + splineExitPos.y, transform.position.z + splineExitPos.z);
    }
}
