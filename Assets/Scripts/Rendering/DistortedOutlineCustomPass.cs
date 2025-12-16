using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RendererUtils;

[System.Serializable]
public sealed class DistortedOutlineCustomPass : CustomPass
{
    [Header("Selection")]
    public LayerMask outlineLayer = 1 << 7;

    [Header("Materials/Shaders")]
    public Shader maskShader;               // Hidden/Custom/OutlineMaskHDRP
    public Material fullscreenOutlineMat;   // Hidden/Custom/SS_Outline_Distort_HDRP

    [Header("Debug")]
    public bool debugShowMask = false;
    public Material debugMaskMat; // assign a material using Hidden/Custom/DebugMaskHDRP


    RTHandle _maskRT;
    Material _maskMat;
    [System.NonSerialized] private ShaderTagId[] _shaderTags;

    protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
    {
        _shaderTags ??= new[]{
            new ShaderTagId("Forward"),
            new ShaderTagId("ForwardOnly"),
            new ShaderTagId("SRPDefaultUnlit")
        };

        if (maskShader == null)
            maskShader = Shader.Find("Hidden/Custom/OutlineMaskHDRP");

        _maskMat = CoreUtils.CreateEngineMaterial(maskShader);

        _maskRT = RTHandles.Alloc(
            scaleFactor: Vector2.one,
            slices: TextureXR.slices,
            dimension: TextureXR.dimension,
            colorFormat: GraphicsFormat.R8_UNorm,
            useDynamicScale: true,
            name: "OutlineMaskRT"
        );
    }

    protected override void Execute(CustomPassContext ctx)
    {
        if (_shaderTags == null)
        {
            _shaderTags = new[]
            {
            new ShaderTagId("Forward"),
            new ShaderTagId("ForwardOnly"),
            new ShaderTagId("SRPDefaultUnlit"),
        };
        }

        if (_maskMat == null)
            return;

        var settings = ctx.hdCamera.volumeStack.GetComponent<DistortedOutlineSettings>();
        bool settingsActive = (settings != null && settings.IsActive());

        // 1) Render mask
        CoreUtils.SetRenderTarget(ctx.cmd, _maskRT, ClearFlag.Color, Color.clear);

        var rld = new RendererListDesc(_shaderTags, ctx.cullingResults, ctx.hdCamera.camera)
        {
            renderQueueRange = RenderQueueRange.all,
            layerMask = outlineLayer,                    // Unity layers: ALL
            renderingLayerMask = 0xFFFFFFFFu,  // HDRP rendering layers: ALL
            sortingCriteria = SortingCriteria.None,
            overrideMaterial = _maskMat,
            overrideMaterialPassIndex = 0,
        };

        var rl = ctx.renderContext.CreateRendererList(rld);
        ctx.cmd.DrawRendererList(rl);

        // 2) Debug: show mask
        if (debugShowMask && debugMaskMat != null)
        {
            debugMaskMat.SetTexture("_MaskTex", _maskRT.rt);
            debugMaskMat.SetFloat("_Exposure", 1f);
            HDUtils.DrawFullScreen(ctx.cmd, debugMaskMat, ctx.cameraColorBuffer);
            return;
        }

        // 3) Outline composite
        if (!settingsActive || fullscreenOutlineMat == null)
            return;

        fullscreenOutlineMat.SetTexture("_MaskTex", _maskRT.rt);
        fullscreenOutlineMat.SetFloat("_OutlineWidth", settings.outlineWidth.value);
        fullscreenOutlineMat.SetColor("_OutlineColor", settings.outlineColor.value);

        if (settings.distortTex.value != null)
            fullscreenOutlineMat.SetTexture("_DistortTex", settings.distortTex.value);

        fullscreenOutlineMat.SetFloat("_Distort", settings.distort.value);
        fullscreenOutlineMat.SetFloat("_DistortSpeed", settings.distortSpeed.value);

        // IMPORTANT: texel size from the actual mask RT
        float mw = _maskRT.rt.width;
        float mh = _maskRT.rt.height;
        fullscreenOutlineMat.SetVector("_MaskTex_TexelSize", new Vector4(1f / mw, 1f / mh, mw, mh));



        HDUtils.DrawFullScreen(ctx.cmd, fullscreenOutlineMat, ctx.cameraColorBuffer);
    }





    protected override void Cleanup()
    {
        CoreUtils.Destroy(_maskMat);
        _maskRT?.Release();
    }
}
