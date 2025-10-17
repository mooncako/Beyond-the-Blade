using System.Collections.Generic;
using UnityEngine;

public static class AOEGenerator
{
    public static Mesh BuildRectangle(float width, float length)
    {
        var mesh = new Mesh { name = $"Offload_{width}x{length}" };
        float hw = width * .5f;
        float hl = length * .5f;

        var verts = new Vector3[]
        {
            new Vector3(-hw, 0f, -hl),
            new Vector3(hw, 0f, -hl),
            new Vector3(hw, 0f, hl),
            new Vector3(-hw, 0f, hl)
        };

        var uvs = new Vector2[]
        {
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(1f, 1f),
            new Vector2(0f, 1f)
        };

        var tris = new int[] { 0, 2, 1, 0, 3, 2 };
        mesh.SetVertices(verts);
        mesh.SetUVs(0, new List<Vector2>(uvs));
        mesh.SetTriangles(tris, 0);
        ApplyUpwardNormals(mesh);
        return mesh;
    }

    public static Mesh BuildCircle(float radius, int radialSegments = 64)
    {
        radialSegments = Mathf.Max(3, radialSegments);
        var mesh = new Mesh { name = $"Offload_r{radius}_seg{radialSegments}" };

        var verts = new List<Vector3>(radialSegments + 1);
        var uvs   = new List<Vector2>(radialSegments + 1);
        var tris  = new List<int>(radialSegments * 3);

        // center
        verts.Add(Vector3.zero);
        uvs.Add(new Vector2(0.5f, 0.5f));

        float inv = 1f / (2f * radius); // for UV mapping [-r..r] -> [0..1]
        for (int i = 0; i <= radialSegments; i++)
        {
            float t = (float)i / radialSegments;
            float ang = t * Mathf.PI * 2f;
            float x = Mathf.Cos(ang) * radius;
            float z = Mathf.Sin(ang) * radius;
            verts.Add(new Vector3(x, 0f, z));
            uvs.Add(new Vector2(x * inv + 0.5f, z * inv + 0.5f));
        }

        for (int i = 1; i <= radialSegments; i++)
        {
            // Tri: center, i, i+1 (winding CCW from above)
            tris.Add(0);
            tris.Add(i);
            tris.Add(i + 1);
        }

        mesh.SetVertices(verts);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        ApplyUpwardNormals(mesh);
        return mesh;
    }
    public static Mesh BuildSector(float radius, float angleDeg, int radialSegments = 64)
    {
        radialSegments = Mathf.Max(1, radialSegments);
        angleDeg = Mathf.Clamp(angleDeg, 0.01f, 360f);
        var mesh = new Mesh { name = $"Offload_r{radius}_a{angleDeg}_seg{radialSegments}" };

        var verts = new List<Vector3>(radialSegments + 2);
        var uvs   = new List<Vector2>(radialSegments + 2);
        var tris  = new List<int>(radialSegments * 3);

        verts.Add(Vector3.zero);
        uvs.Add(new Vector2(0.5f, 0.5f));

        float angRad = angleDeg * Mathf.Deg2Rad;
        float inv = 1f / (2f * radius);

        for (int i = 0; i <= radialSegments; i++)
        {
            float t = (float)i / radialSegments;
            float a = -angRad * 0.5f + t * angRad; // center on +Z axis
            float x = Mathf.Sin(a) * radius;
            float z = Mathf.Cos(a) * radius;
            verts.Add(new Vector3(x, 0f, z));
            uvs.Add(new Vector2(x * inv + 0.5f, z * inv + 0.5f));
        }

        for (int i = 1; i <= radialSegments; i++)
        {
            tris.Add(0);
            tris.Add(i);
            tris.Add(i + 1);
        }

        mesh.SetVertices(verts);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        ApplyUpwardNormals(mesh);
        return mesh;
    }

    public static Mesh BuildRing(float innerRadius, float outerRadius, int radialSegments = 64)
    {
        radialSegments = Mathf.Max(3, radialSegments);
        innerRadius = Mathf.Max(0f, Mathf.Min(innerRadius, outerRadius * 0.999f));

        var mesh = new Mesh { name = $"Offload_ir{innerRadius}_or{outerRadius}_seg{radialSegments}" };

        var verts = new List<Vector3>((radialSegments + 1) * 2);
        var uvs   = new List<Vector2>((radialSegments + 1) * 2);
        var tris  = new List<int>(radialSegments * 6);

        float invOuter = 1f / (2f * outerRadius);
        for (int i = 0; i <= radialSegments; i++)
        {
            float t = (float)i / radialSegments;
            float ang = t * Mathf.PI * 2f;
            float cos = Mathf.Cos(ang);
            float sin = Mathf.Sin(ang);

            var outer = new Vector3(cos * outerRadius, 0f, sin * outerRadius);
            var inner = new Vector3(cos * innerRadius, 0f, sin * innerRadius);

            verts.Add(outer);
            verts.Add(inner);

            uvs.Add(new Vector2(outer.x * invOuter + 0.5f, outer.z * invOuter + 0.5f));
            uvs.Add(new Vector2(inner.x * invOuter + 0.5f, inner.z * invOuter + 0.5f));
        }

        for (int i = 0; i < radialSegments; i++)
        {
            int i0 = i * 2;
            int i1 = i0 + 1;
            int i2 = i0 + 2;
            int i3 = i0 + 3;

            // Quad as two triangles (outer strip: even indices)
            tris.Add(i0); tris.Add(i1); tris.Add(i2);
            tris.Add(i2); tris.Add(i1); tris.Add(i3);
        }

        mesh.SetVertices(verts);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        ApplyUpwardNormals(mesh);
        return mesh;
    }

    public static Mesh BuildRingSector(float innerRadius, float outerRadius, float angleDeg, int radialSegments = 64)
    {
        radialSegments = Mathf.Max(1, radialSegments);
        innerRadius = Mathf.Max(0f, Mathf.Min(innerRadius, outerRadius * 0.999f));
        angleDeg = Mathf.Clamp(angleDeg, 0.01f, 360f);

        var mesh = new Mesh { name = $"Offload_ir{innerRadius}_or{outerRadius}_a{angleDeg}_seg{radialSegments}" };

        var verts = new List<Vector3>((radialSegments + 1) * 2);
        var uvs   = new List<Vector2>((radialSegments + 1) * 2);
        var tris  = new List<int>(radialSegments * 6);

        float angRad = angleDeg * Mathf.Deg2Rad;
        float invOuter = 1f / (2f * outerRadius);

        for (int i = 0; i <= radialSegments; i++)
        {
            float t = (float)i / radialSegments;
            float a = -angRad * 0.5f + t * angRad; // centered on +Z
            float cos = Mathf.Cos(a);
            float sin = Mathf.Sin(a);

            var outer = new Vector3(sin * outerRadius, 0f, cos * outerRadius);
            var inner = new Vector3(sin * innerRadius, 0f, cos * innerRadius);

            verts.Add(outer);
            verts.Add(inner);

            uvs.Add(new Vector2(outer.x * invOuter + 0.5f, outer.z * invOuter + 0.5f));
            uvs.Add(new Vector2(inner.x * invOuter + 0.5f, inner.z * invOuter + 0.5f));
        }

        for (int i = 0; i < radialSegments; i++)
        {
            int i0 = i * 2;
            int i1 = i0 + 1;
            int i2 = i0 + 2;
            int i3 = i0 + 3;

            // Strip between outer & inner radii
            tris.Add(i0); tris.Add(i1); tris.Add(i2);
            tris.Add(i2); tris.Add(i1); tris.Add(i3);
        }

        mesh.SetVertices(verts);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        ApplyUpwardNormals(mesh);
        return mesh;
    }

    private static void ApplyUpwardNormals(Mesh mesh)
    {
        // Efficient constant-up normals; recalc bounds and tangents for lighting/shaders
        var verts = mesh.vertices;
        var norms = new Vector3[verts.Length];
        for (int i = 0; i < norms.Length; i++) norms[i] = Vector3.up;
        mesh.normals = norms;

        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        // If your material is single-sided and the mesh looks invisible from above,
        // flip triangle winding: Array.Reverse(trisPerSubmesh) then SetTriangles again.
    }
}

