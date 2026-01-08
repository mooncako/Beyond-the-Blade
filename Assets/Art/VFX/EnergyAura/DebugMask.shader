Shader "Hidden/Custom/DebugMaskHDRP"
{
    Properties
    {
        _MaskTex("Mask", 2DArray) = "" {}
        _Exposure("Exposure", Range(0,5)) = 1
    }

    SubShader
    {
        Tags { "RenderPipeline"="HDRenderPipeline" }

        Pass
        {
            Name "DebugMask"
            ZWrite Off
            ZTest Always
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassCommon.hlsl"

            TEXTURE2D_X(_MaskTex);
            SAMPLER(sampler_MaskTex);

            float _Exposure;

            struct DAttributes { uint vertexID : SV_VertexID; };
            struct DVaryings   { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; };

            DVaryings Vert(DAttributes a)
            {
                DVaryings o;
                o.positionCS = GetFullScreenTriangleVertexPosition(a.vertexID);
                o.uv         = GetFullScreenTriangleTexCoord(a.vertexID);
                return o;
            }

            float4 Frag(DVaryings i) : SV_Target
            {
                float m = SAMPLE_TEXTURE2D_X(_MaskTex, sampler_MaskTex, i.uv).r;
                m = saturate(m * _Exposure);
                // show as grayscale, alpha=1 so it is obvious
                return float4(m, m, m, 1);
            }
            ENDHLSL
        }
    }
}
