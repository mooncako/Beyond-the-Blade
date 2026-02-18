Shader "Custom/PortalVolume_Occluder_HDRP"
{
    Properties
    {
        _PortalStencilRef ("Portal Stencil Ref", Float) = 1
        _PortalStencilWriteMask ("Portal Stencil WriteMask", Float) = 1
    }

    HLSLINCLUDE
    #pragma target 4.5
    #pragma only_renderers d3d11 ps4 xboxone vulkan metal switch

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

    struct Attributes
    {
        float3 positionOS : POSITION;
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
    };

    Varyings Vert(Attributes v)
    {
        Varyings o;
        o.positionCS = TransformWorldToHClip(TransformObjectToWorld(v.positionOS));
        return o;
    }

    float4 Frag(Varyings i) : SV_Target
    {
        // Color won't be written because of ColorMask 0.
        return 0;
    }
    ENDHLSL

    SubShader
    {
        Tags
        {
            "RenderPipeline"="HDRenderPipeline"
            "RenderType"="Opaque"
            // If you are NOT using a Custom Pass, keep this early.
            // If you ARE using a Custom Pass at "Before Transparent", the queue is less important.
            "Queue"="Geometry+0"
        }

        Pass
        {
            Name "ForwardOnly"
            Tags { "LightMode"="ForwardOnly" }

            Cull Back

            // Usually you want the stencil to respect depth so it only marks visible parts of the volume.
            ZWrite Off
            ZTest Always

            // Do not write any color
            ColorMask 0

            // WRITE portal stencil bit
            Stencil
            {
                Ref [_PortalStencilRef]
                WriteMask [_PortalStencilWriteMask]
                ReadMask 255

                CompFront Always
                PassFront Replace
                FailFront Keep
                ZFailFront Keep

                CompBack Always
                PassBack Replace
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
