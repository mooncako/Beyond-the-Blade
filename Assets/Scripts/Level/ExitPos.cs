
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
public class ExitPos : DrawPos
{
    [BoxGroup("References")] public SplineContainer SplineContainer;

    void OnValidate()
    {
        if(SplineContainer == null) SplineContainer = GetComponent<SplineContainer>();
    }

    public Vector3 GetTeleportExit()
    {
        float3 splineExitPos = SplineContainer.Spline.ToArray()[SplineContainer.Spline.ToArray().Length - 1].Position;
        
        return SplineContainer.transform.TransformPoint(splineExitPos);
    }

    public BezierKnot GetLastKnot()
    {
        return SplineContainer.Spline.ToArray()[SplineContainer.Spline.ToArray().Length - 1];
    }
}
