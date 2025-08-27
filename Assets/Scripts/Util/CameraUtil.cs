using UnityEngine;

public static class CameraUtil
{
    // Returns a camera-relative, snapped world direction on the XZ plane.
    // snaps = 4 (N/E/S/W) or 8 (adds diagonals).
    public static Vector3 GetSnappedDir(Vector2 input, Camera cam, int snaps = 8, float deadzone = 0.01f)
    {
        if (!cam || input.sqrMagnitude <= deadzone * deadzone)
            return Vector3.zero;

        // 1) Camera-relative planar basis (ignore pitch/roll)
        Vector3 camF = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
        Vector3 camR = Vector3.ProjectOnPlane(cam.transform.right, Vector3.up).normalized;

        // 2) Camera-relative world direction from input
        Vector3 worldDir = (camR * input.x + camF * input.y);
        if (worldDir.sqrMagnitude < 1e-6f) return Vector3.zero;
        worldDir.Normalize();

        // 3) Build snap candidates
        // 4-way: F, R, -F, -R
        // 8-way: add (F+R), (F-R), (-F+R), (-F-R)
        if (snaps != 8) snaps = 4;

        // precompute candidates
        Vector3[] cands4 = new[]
        {
            camF,
            camR,
            -camF,
            -camR
        };

        if (snaps == 4)
            return BestDot(worldDir, cands4);

        Vector3[] cands8 = new[]
        {
            camF,
            (camF + camR).normalized,
            camR,
            (camR - camF).normalized,
            -camF,
            (-camF - camR).normalized,
            -camR,
            (-camR + camF).normalized
        };

        return BestDot(worldDir, cands8);

        static Vector3 BestDot(Vector3 dir, Vector3[] cands)
        {
            int best = 0; float bestDot = -2f;
            for (int i = 0; i < cands.Length; i++)
            {
                float d = Vector3.Dot(dir, cands[i]);
                if (d > bestDot) { bestDot = d; best = i; }
            }
            return cands[best];
        }
    }



}
