Shader "Hidden/Custom/SS_Outline_Distort_HDRP"
{
    Properties
    {
        _MaskTex("Mask", 2DArray) = "" {}
        _DistortTex("Distort Texture", 2D) = "gray" {}
        _Distort("Distort", Float) = 0
        _DistortSpeed("Distort Speed", Float) = 0
        _OutlineWidth("Outline Width", Float) = 1
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
    }

    SubShader
    {
        Tags { "RenderPipeline"="HDRenderPipeline" "RenderType"="Opaque" }

        Pass
        {
            Name "FullscreenOutline"
            ZWrite Off
            ZTest Always
            Cull Off
            Blend One OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassCommon.hlsl"

            #define SAMPLE_COUNT 8

            TEXTURE2D_X(_MaskTex);
            SAMPLER(sampler_MaskTex);

            TEXTURE2D(_DistortTex);
            SAMPLER(sampler_DistortTex);
            float4 _DistortTex_ST;

            float4 _OutlineColor;
            float  _OutlineWidth;
            float  _Distort;
            float  _DistortSpeed;

            // Set from C# (1/w, 1/h, w, h)
            float4 _MaskTex_TexelSize;

            struct OAttributes
            {
                uint vertexID : SV_VertexID;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct OVaryings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            OVaryings Vert(OAttributes a)
            {
                OVaryings o;
                UNITY_SETUP_INSTANCE_ID(a);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.positionCS = GetFullScreenTriangleVertexPosition(a.vertexID);
                o.uv         = GetFullScreenTriangleTexCoord(a.vertexID);
                return o;
            }

            float4 Frag(OVaryings i) : SV_Target
{
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

    float2 baseUV = i.uv;
    float2 texel  = _MaskTex_TexelSize.xy;

    // Center test stays undistorted: only draw outside
    float center = SAMPLE_TEXTURE2D_X(_MaskTex, sampler_MaskTex, baseUV).r;
    if (center > 0.001)
        clip(-1);

    float edge = 0.0;

    [unroll]
    for (int k = 0; k < SAMPLE_COUNT; k++)
    {
        float s, c;
        sincos(radians(360.0 / (float)SAMPLE_COUNT * k), s, c);
        float2 dir = float2(s, c);

        // Base neighbor location (no distortion yet)
        float2 sampleBaseUV = baseUV + dir * texel * _OutlineWidth;

        // Sample noise PER-DIRECTION so it doesn't translate the whole kernel
        float2 noiseUV = sampleBaseUV * _DistortTex_ST.xy + _DistortTex_ST.zw;
        noiseUV += _Time.y * _DistortSpeed * float2(0, -.13);

        // Scalar noise in [-1, 1]
        float n = SAMPLE_TEXTURE2D(_DistortTex, sampler_DistortTex, noiseUV).r * 2.0 - 1.0;

        // Distort only along the radial direction (prevents sideways drift)
        float2 jitter = dir * (n * _Distort) * texel;

        float2 suv = sampleBaseUV + jitter;
        suv = saturate(suv);

        float m = SAMPLE_TEXTURE2D_X(_MaskTex, sampler_MaskTex, suv).r;
        edge = max(edge, step(0.001, m));
    }

    if (edge > 0.0)
        return float4(_OutlineColor.rgb, 1);

    return 0;
}


            ENDHLSL
        }
    }
}
