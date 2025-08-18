using System.Collections.Generic;
using System.Diagnostics;
using Sirenix.OdinInspector;
using UnityEngine;

public class AOEApplier : MonoBehaviour
{
    [SerializeField, BoxGroup("Settings")] private LayerMask _hitMask;

    [BoxGroup("Settings")] public float X;
    [BoxGroup("Settings")] public float Y;
    [BoxGroup("Settings")] public float Z;

    [SerializeField, BoxGroup("Settings")] private float _coneHeight = Mathf.Infinity;
    [SerializeField, BoxGroup("Settings")] private float _arcHeight = 2;

    [SerializeField, BoxGroup("Debug")] private SkillAreaType _type;

    private readonly Collider[] _buffer = new Collider[64];
    private readonly List<Collider> _hits = new List<Collider>(64);
    private List<GameObject> _damagedEntities = new List<GameObject>();
    private int count = 0;


    public List<GameObject> GetDamagedEntities(SkillAreaType type, Vector3 center)
    {
        _damagedEntities.Clear();

        switch (type)
        {
            case SkillAreaType.Box:
                count = SkillAreaCalculation.OverlapBox(center + transform.forward * 1.5f, new Vector3(X, Y, Z), transform.rotation, _hitMask, _buffer);
                for (int i = 0; i < count; i++)
                {
                    _damagedEntities.Add(_buffer[i].gameObject);
                }
                break;

            case SkillAreaType.Circle:
                count = SkillAreaCalculation.OverlapCircle(center, X, _hitMask, _buffer);
                for (int i = 0; i < count; i++)
                {
                    _damagedEntities.Add(_buffer[i].gameObject);
                }
                break;

            case SkillAreaType.Cone:
                count = SkillAreaCalculation.OverlapCone(center, transform.forward, Y, X, _coneHeight, _hitMask, _buffer, _hits);
                for (int i = 0; i < count; i++)
                {
                    _damagedEntities.Add(_hits[i].gameObject);
                }
                break;

            case SkillAreaType.Arc:
                count = SkillAreaCalculation.OverlapArc(center, transform.forward, X, Z, Y, _arcHeight, _hitMask, _buffer, _hits);
                for (int i = 0; i < count; i++)
                {
                    _damagedEntities.Add(_hits[i].gameObject);
                }
                break;
        }

        return _damagedEntities;
    }

    #if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        switch (_type)
        {
            case SkillAreaType.Box:
                // Box gizmo (approx)
                Gizmos.matrix = Matrix4x4.TRS(transform.position + transform.forward * 1.5f, transform.rotation, Vector3.one);
                Gizmos.DrawWireCube(Vector3.zero, new Vector3(X, Y, Z));
                break;
            case SkillAreaType.Circle:
                Gizmos.DrawWireSphere(transform.position, X);
                break;
            case SkillAreaType.Cone:
                // Cone (wire)
                DrawCone(transform.position, transform.forward, X, Y);
                break;
            case SkillAreaType.Arc:
                // Arc (sector)
                DrawArcSector(transform.position, transform.forward, X, Y, Z);
                break;
        }
        
    }

    static void DrawCone(Vector3 o, Vector3 fwd, float range, float angle)
    {
        var up = Vector3.up;
        var right = Vector3.Cross(up, fwd).normalized;
        up = Vector3.Cross(fwd, right).normalized;
        float half = angle * 0.5f;
        Quaternion qL = Quaternion.AngleAxis(-half, up);
        Quaternion qR = Quaternion.AngleAxis(half, up);
        Vector3 L = qL * fwd * range;
        Vector3 R = qR * fwd * range;
        Gizmos.DrawLine(o, o + L);
        Gizmos.DrawLine(o, o + R);
        UnityEditor.Handles.DrawWireArc(o, up, L.normalized, angle, range);
    }

    static void DrawArcSector(Vector3 o, Vector3 fwd, float outer, float inner, float angle)
    {
        Vector3 nFwd = new Vector3(fwd.x, 0, fwd.z).normalized;
        if (nFwd.sqrMagnitude < 1e-5f) nFwd = Vector3.forward;
        var up = Vector3.up;
        var left = Quaternion.AngleAxis(-angle * 0.5f, up) * nFwd;
        UnityEditor.Handles.DrawWireArc(o, up, left, angle, outer);
        if (inner > 0f)
            UnityEditor.Handles.DrawWireArc(o, up, left, angle, inner);
        UnityEditor.Handles.DrawLine(o + left * inner, o + left * outer);
        var right = Quaternion.AngleAxis(angle * 0.5f, up) * nFwd;
        UnityEditor.Handles.DrawLine(o + right * inner, o + right * outer);
    }
#endif
}
