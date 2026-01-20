Shader "Custom/PortalOccludee_HDRP"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _BaseMap ("Base Map", 2D) = "white" {}

        // Portal stencil controls (keep these stable and under your control)
        _PortalStencilRef ("Portal Stencil Ref", Float) = 1
        _PortalStencilReadMask ("Portal Stencil ReadMask", Float) = 1
    }

    HLSLINCLUDE
    #pragma target 4.5
    #pragma only_renderers d3d11 ps4 xboxone vulkan metal switch

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

    TEXTURE2D(_BaseMap);
    SAMPLER(sampler_BaseMap);
    float4 _BaseMap_ST;
    float4 _BaseColor;

    struct Attributes
    {
        float3 positionOS : POSITION;
        float2 uv         : TEXCOORD0;
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 uv         : TEXCOORD0;
    };

    Varyings Vert(Attributes v)
    {
        Varyings o;
        o.positionCS = TransformWorldToHClip(TransformObjectToWorld(v.positionOS));
        o.uv = v.uv * _BaseMap_ST.xy + _BaseMap_ST.zw;
        return o;
    }

    float4 Frag(Varyings i) : SV_Target
    {
        float4 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv) * _BaseColor;
        return albedo;
    }
    ENDHLSL

    SubShader
    {
        Tags
        {
            "RenderPipeline"="HDRenderPipeline"
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        Pass
        {
            Name "ForwardOnly"
            Tags { "LightMode"="ForwardOnly" }

            Cull Back
            ZWrite Off
            ZTest LEqual

            // Standard alpha blending (premultiply not assumed)
            Blend SrcAlpha OneMinusSrcAlpha

            // Portal occlusion: do NOT draw where portal stencil bit is set
            Stencil
            {
                Ref [_PortalStencilRef]
                ReadMask [_PortalStencilReadMask]
                WriteMask 0

                CompFront NotEqual
                PassFront Keep
                FailFront Keep
                ZFailFront Keep

                CompBack NotEqual
                PassBack Keep
                FailBack Keep
                ZFailBack Keep
            }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            ENDHLSL
        }
    }
}
