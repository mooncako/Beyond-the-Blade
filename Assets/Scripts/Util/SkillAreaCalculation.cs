using System.Collections.Generic;
using UnityEngine;

public static class SkillAreaCalculation
{
    public static int OverlapBox(Vector3 center, Vector3 size, Quaternion rotation, LayerMask mask, Collider[] results, QueryTriggerInteraction qti = QueryTriggerInteraction.Collide)
    {
        Vector3 half = size * .5f;
        return Physics.OverlapBoxNonAlloc(center, half, results, rotation, mask, qti);
    }

    public static int OverlapCircle(Vector3 center, float radius, LayerMask mask, Collider[] results, QueryTriggerInteraction qti = QueryTriggerInteraction.Collide)
    {
        return Physics.OverlapSphereNonAlloc(center, radius, results, mask, qti);
    }

    public static int OverlapCone(Vector3 center, Vector3 forward, float angleDeg, float range, float heightLimit, LayerMask mask, Collider[] buffer, List<Collider> hitsOut, QueryTriggerInteraction qti = QueryTriggerInteraction.Collide)
    {
        hitsOut.Clear();

        int count = Physics.OverlapSphereNonAlloc(center, range, buffer, mask, qti);
        float cosHalf = Mathf.Cos(Mathf.Deg2Rad * (angleDeg * .5f));
        forward = forward.sqrMagnitude > 0.0001f ? forward.normalized : Vector3.forward;

        for (int i = 0; i < count; i++)
        {
            Collider c = buffer[i];

            Vector3 p = Physics.ClosestPoint(center, c, c.transform.position, c.transform.rotation);
            Vector3 toP = p - center;
            float dist = toP.magnitude;

            if (dist < Mathf.Epsilon || dist > range) continue;

            if (!float.IsInfinity(heightLimit))
            {
                float dy = Mathf.Abs(toP.y);
                if (dy > heightLimit * .5f) continue;
            }

            float cos = Vector3.Dot(forward, toP / dist);
            if (cos >= cosHalf)
            {
                hitsOut.Add(c);
            }
        }

        return hitsOut.Count;
    }
    
    public static int OverlapArc(Vector3 center, Vector3 forward, float outerRadius, float arcAngleDeg, float innerRadius,
                                 float heightLimit, LayerMask mask, Collider[] buffer, List<Collider> hitsOut,
                                 QueryTriggerInteraction qti = QueryTriggerInteraction.Collide)
    {
        hitsOut.Clear();
        int count = Physics.OverlapSphereNonAlloc(center, outerRadius, buffer, mask, qti);

        Vector3 f = forward;
        f.y = 0f;
        if (f.sqrMagnitude < 1e-5f) f = Vector3.forward;
        f.Normalize();
        float half = arcAngleDeg * 0.5f;

        for (int i = 0; i < count; i++)
        {
            Collider c = buffer[i];
            Vector3 p = Physics.ClosestPoint(center, c, c.transform.position, c.transform.rotation);
            Vector3 toP = p - center;

            // Height clamp
            if (!float.IsInfinity(heightLimit) && Mathf.Abs(toP.y) > heightLimit * 0.5f) continue;

            // XZ checks
            Vector3 toPXZ = new Vector3(toP.x, 0f, toP.z);
            float d = toPXZ.magnitude;
            if (d < Mathf.Max(0f, innerRadius) || d > outerRadius) continue;

            float angle = Vector3.Angle(f, toPXZ / Mathf.Max(d, 1e-5f));
            if (angle <= half)
                hitsOut.Add(c);
        }

        return hitsOut.Count;
    }
}
