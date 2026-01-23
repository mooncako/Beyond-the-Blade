using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class PortalClipPerObject : MonoBehaviour
{
    [Header("Portals (BoxCollider defines volume)")]
    [SerializeField] private BoxCollider portal0;
    [SerializeField] private BoxCollider portal1;

    [Header("Targets")]
    [SerializeField] private bool autoCollectChildren = true;
    [SerializeField] private bool includeInactive = false;
    [SerializeField] private List<Renderer> renderers = new();

    [Header("Enable")]
    [SerializeField] private bool enabled0 = true;
    [SerializeField] private bool enabled1 = true;

    // Portal 0 IDs
    static readonly int C0  = Shader.PropertyToID("_PortalCenterWS0");
    static readonly int X0  = Shader.PropertyToID("_PortalAxisXWS0");
    static readonly int Y0  = Shader.PropertyToID("_PortalAxisYWS0");
    static readonly int Z0  = Shader.PropertyToID("_PortalAxisZWS0");
    static readonly int E0  = Shader.PropertyToID("_PortalHalfExtentsWS0");
    static readonly int EN0 = Shader.PropertyToID("_PortalEnabled0");

    // Portal 1 IDs
    static readonly int C1  = Shader.PropertyToID("_PortalCenterWS1");
    static readonly int X1  = Shader.PropertyToID("_PortalAxisXWS1");
    static readonly int Y1  = Shader.PropertyToID("_PortalAxisYWS1");
    static readonly int Z1  = Shader.PropertyToID("_PortalAxisZWS1");
    static readonly int E1  = Shader.PropertyToID("_PortalHalfExtentsWS1");
    static readonly int EN1 = Shader.PropertyToID("_PortalEnabled1");

    private readonly Dictionary<Renderer, MaterialPropertyBlock> _blocks = new();

    void Awake()
    {
        if (autoCollectChildren) RebuildRendererList();
        EnsureBlocks();
    }

    [ContextMenu("Rebuild Renderer List")]
    public void RebuildRendererList()
    {
        renderers.Clear();
        renderers.AddRange(GetComponentsInChildren<Renderer>(includeInactive));
        EnsureBlocks();
    }

    void LateUpdate()
    {
        if (renderers.Count == 0) return;

        GetPortalAxesData(portal0, enabled0, out float en0, out Vector3 center0, out Vector3 ax0, out Vector3 ay0, out Vector3 az0, out Vector3 ext0);
        GetPortalAxesData(portal1, enabled1, out float en1, out Vector3 center1, out Vector3 ax1, out Vector3 ay1, out Vector3 az1, out Vector3 ext1);

        for (int i = 0; i < renderers.Count; i++)
        {
            var r = renderers[i];
            if (!r) continue;

            var mpb = GetBlock(r);
            r.GetPropertyBlock(mpb);

            mpb.SetFloat(EN0, en0);
            mpb.SetVector(C0, center0);
            mpb.SetVector(X0, ax0);
            mpb.SetVector(Y0, ay0);
            mpb.SetVector(Z0, az0);
            mpb.SetVector(E0, ext0);

            mpb.SetFloat(EN1, en1);
            mpb.SetVector(C1, center1);
            mpb.SetVector(X1, ax1);
            mpb.SetVector(Y1, ay1);
            mpb.SetVector(Z1, az1);
            mpb.SetVector(E1, ext1);

            r.SetPropertyBlock(mpb);
        }
    }

    static void GetPortalAxesData(
        BoxCollider col, bool enabledFlag,
        out float enabledValue,
        out Vector3 centerWS,
        out Vector3 axisXWS,
        out Vector3 axisYWS,
        out Vector3 axisZWS,
        out Vector3 halfExtentsWS)
    {
        if (!enabledFlag || col == null)
        {
            enabledValue = 0f;
            centerWS = Vector3.zero;
            axisXWS = Vector3.right;
            axisYWS = Vector3.up;
            axisZWS = Vector3.forward;
            halfExtentsWS = Vector3.zero;
            return;
        }

        enabledValue = 1f;

        Transform t = col.transform;

        // World-space center of the BoxCollider
        centerWS = t.TransformPoint(col.center);

        // Oriented axes (normalized directions)
        axisXWS = t.right.normalized;
        axisYWS = t.up.normalized;
        axisZWS = t.forward.normalized; // portal local +Z

        // World half extents along each local axis
        // BoxCollider.size is in local units; scale it into world units.
        Vector3 halfLocal = col.size * 0.5f;
        Vector3 s = t.lossyScale;
        halfExtentsWS = new Vector3(
            halfLocal.x * Mathf.Abs(s.x),
            halfLocal.y * Mathf.Abs(s.y),
            halfLocal.z * Mathf.Abs(s.z)
        );
    }

    void EnsureBlocks()
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            var r = renderers[i];
            if (!r) continue;
            if (!_blocks.ContainsKey(r))
                _blocks[r] = new MaterialPropertyBlock();
        }
    }

    MaterialPropertyBlock GetBlock(Renderer r)
    {
        if (!_blocks.TryGetValue(r, out var b) || b == null)
            _blocks[r] = b = new MaterialPropertyBlock();
        return b;
    }
}
