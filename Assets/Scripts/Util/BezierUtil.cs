using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;


public static class BezierUtil
{

    
    internal static Vector3 CalculateBezierPoint(float3 p0, float3 p2, float t, float height)
    {
        Profiler.BeginSample("bezier");
        var result = new NativeArray<float3>(1, Allocator.Persistent);

        var bezierJob = new BezierJob()
        {
            p0 = new float3(p0.x, p0.y, p0.z),
            p2 = new float3(p2.x, p2.y, p2.z),
            t = t,
            height = height,
            point = result
        };

        var bezierJobHandle = bezierJob.Schedule(32, 32);

        bezierJobHandle.Complete();
        Vector3 output = result[0];
        result.Dispose();
        Profiler.EndSample();
        return output;
    }
    


    /*
    internal static Vector3 CalculateBezierPoint(Vector3 p0, Vector3 p2, float t, float height)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p1 = (p0 + p2) / 2;
        p1.y = ((p0.y + p2.y) / 2) + height;
        Vector3 point = uu * p0 + 2 * u * t * p1 + tt * p2;
        return point;
    }
    */
   
}



[BurstCompile]
public struct BezierJob: IJobParallelFor
{
    [ReadOnly] public float3 p0;
    [ReadOnly] public float3 p2;
    [ReadOnly] public float t;
    [ReadOnly] public float height;

    public NativeArray<float3> point;

    public void Execute(int i)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float3 p1 = (p0 + p2) / 2;
        p1.y = ((p0.y + p2.y) / 2) + height;

        point[0] = uu * p0 + 2 * u * t * p1 + tt * p2;
    }
}


