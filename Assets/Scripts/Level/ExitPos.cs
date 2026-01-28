
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class ExitPos : DrawPos
{
    [BoxGroup("References")] public SplineContainer SplineContainer;


    public Vector3 GetTeleportExit()
    {
        float3 splineExitPos = SplineContainer.Spline.ToArray()[SplineContainer.Spline.ToArray().Length - 1].Position;
        
        return SplineContainer.transform.TransformPoint(splineExitPos);
    }

}
