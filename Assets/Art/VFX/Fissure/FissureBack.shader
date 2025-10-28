Shader "Shader Graphs/FissureBack"
{
    Properties
    {
        _InnerEdge("InnerEdge", Range(0, 1)) = 0.21
        [HDR]_VoidColor("VoidColor", Color) = (0.1315238, 0, 2.297397, 0)
        _Speed("Speed", Range(0, 1)) = 0.093
        _FresnelPower("FresnelPower", Float) = 5
        [HDR]_BorderColor("BorderColor", Color) = (0, 0.5321255, 1, 0)
        _Seed("Seed", Vector) = (0, 0, 0, 0)
        [NoScaleOffset]_TwirlTexture("TwirlTexture", 2D) = "white" {}
        [HideInInspector]_EmissionColor("Color", Color) = (1, 1, 1, 1)
        [HideInInspector]_RenderQueueType("Float", Float) = 4
        [HideInInspector][ToggleUI]_AddPrecomputedVelocity("Boolean", Float) = 0
        [HideInInspector][ToggleUI]_DepthOffsetEnable("Boolean", Float) = 0
        [HideInInspector][ToggleUI]_ConservativeDepthOffsetEnable("Boolean", Float) = 0
        [HideInInspector][ToggleUI]_TransparentWritingMotionVec("Boolean", Float) = 0
        [HideInInspector][ToggleUI]_AlphaCutoffEnable("Boolean", Float) = 0
        [HideInInspector]_TransparentSortPriority("_TransparentSortPriority", Float) = 0
        [HideInInspector][ToggleUI]_UseShadowThreshold("Boolean", Float) = 0
        [HideInInspector][ToggleUI]_DoubleSidedEnable("Boolean", Float) = 0
        [HideInInspector][Enum(Flip, 0, Mirror, 1, None, 2)]_DoubleSidedNormalMode("Float", Float) = 2
        [HideInInspector]_DoubleSidedConstants("Vector4", Vector) = (1, 1, -1, 0)
        [HideInInspector][Enum(Auto, 0, On, 1, Off, 2)]_DoubleSidedGIMode("Float", Float) = 0
        [HideInInspector][ToggleUI]_TransparentDepthPrepassEnable("Boolean", Float) = 0
        [HideInInspector][ToggleUI]_TransparentDepthPostpassEnable("Boolean", Float) = 0
        [HideInInspector][ToggleUI]_PerPixelSorting("Boolean", Float) = 0
        [HideInInspector]_SurfaceType("Float", Float) = 1
        [HideInInspector]_BlendMode("Float", Float) = 0
        [HideInInspector]_SrcBlend("Float", Float) = 1
        [HideInInspector]_DstBlend("Float", Float) = 0
        [HideInInspector]_DstBlend2("Float", Float) = 0
        [HideInInspector]_AlphaSrcBlend("Float", Float) = 1
        [HideInInspector]_AlphaDstBlend("Float", Float) = 0
        [HideInInspector][ToggleUI]_ZWrite("Boolean", Float) = 0
        [HideInInspector][ToggleUI]_TransparentZWrite("Boolean", Float) = 0
        [HideInInspector]_CullMode("Float", Float) = 2
        [HideInInspector][ToggleUI]_EnableFogOnTransparent("Boolean", Float) = 1
        [HideInInspector]_CullModeForward("Float", Float) = 2
        [HideInInspector][Enum(Front, 1, Back, 2)]_TransparentCullMode("Float", Float) = 2
        [HideInInspector][Enum(UnityEngine.Rendering.HighDefinition.OpaqueCullMode)]_OpaqueCullMode("Float", Float) = 2
        [HideInInspector]_ZTestDepthEqualForOpaque("Float", Int) = 4
        [HideInInspector][Enum(UnityEngine.Rendering.CompareFunction)]_ZTestTransparent("Float", Float) = 4
        [HideInInspector][ToggleUI]_TransparentBackfaceEnable("Boolean", Float) = 0
        [HideInInspector][ToggleUI]_EnableBlendModePreserveSpecularLighting("Boolean", Float) = 0
        [HideInInspector][ToggleUI]_ExcludeFromTUAndAA("Boolean", Float) = 0
        [HideInInspector]_StencilRef("Float", Int) = 0
        [HideInInspector]_StencilWriteMask("Float", Int) = 6
        [HideInInspector]_StencilRefDepth("Float", Int) = 1
        [HideInInspector]_StencilWriteMaskDepth("Float", Int) = 9
        [HideInInspector]_StencilRefMV("Float", Int) = 33
        [HideInInspector]_StencilWriteMaskMV("Float", Int) = 43
        [HideInInspector]_StencilRefDistortionVec("Float", Int) = 4
        [HideInInspector]_StencilWriteMaskDistortionVec("Float", Int) = 4
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="HDRenderPipeline"
            "RenderType"="HDUnlitShader"
            "Queue"="Transparent+0"
            "DisableBatching"="False"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="HDUnlitSubTarget"
        }
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }
        
            // Render State
            Cull [_CullMode]
        ZWrite On
        ColorMask 0
        ZClip [_ZClip]
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma instancing_options renderinglayer
        #pragma target 4.5
        #pragma vertex Vert
        #pragma fragment Frag
        #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch
        #pragma multi_compile_instancing
        
            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>
        
            // Defines
            #define SHADERPASS SHADERPASS_SHADOWS
        #define SUPPORT_GLOBAL_MIP_BIAS 1
        
            // For custom interpolators to inject a substruct definition before FragInputs definition,
            // allowing for FragInputs to capture CI's intended for ShaderGraph's SDI.
            struct CustomInterpolators
        {
        };
        #define USE_CUSTOMINTERP_SUBSTRUCT
        
        
        
            // TODO: Merge FragInputsVFX substruct with CustomInterpolators.
        	#ifdef HAVE_VFX_MODIFICATION
        	struct FragInputsVFX
            {
                /* WARNING: $splice Could not find named fragment 'FragInputsVFX' */
            };
            #endif
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl" // Required by Tessellation.hlsl
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl" // Required to be include before we include properties as it define DECLARE_STACK_CB
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl" // Required before including properties as it defines UNITY_TEXTURE_STREAMING_DEBUG_VARS
            // Always include Shader Graph version
            // Always include last to avoid double macros
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl" // Need to be here for Gradient struct definition
        
            // --------------------------------------------------
            // Defines
        
            // Attribute
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TANGENT_TO_WORLD
            #define VARYINGS_NEED_TEXCOORD0
        
            #define HAVE_MESH_MODIFICATION
        
            //Strip down the FragInputs.hlsl (on graphics), so we can only optimize the interpolators we use.
            //if by accident something requests contents of FragInputs.hlsl, it will be caught as a compiler error
            //Frag inputs stripping is only enabled when FRAG_INPUTS_ENABLE_STRIPPING is set
            #if !defined(SHADER_STAGE_RAY_TRACING) && SHADERPASS != SHADERPASS_RAYTRACING_GBUFFER && SHADERPASS != SHADERPASS_FULL_SCREEN_DEBUG
            #define FRAG_INPUTS_ENABLE_STRIPPING
            #endif
            #define FRAG_INPUTS_USE_TEXCOORD0
        
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
        
        
        
        
            // Following two define are a workaround introduce in 10.1.x for RaytracingQualityNode
            // The ShaderGraph don't support correctly migration of this node as it serialize all the node data
            // in the json file making it impossible to uprgrade. Until we get a fix, we do a workaround here
            // to still allow us to rename the field and keyword of this node without breaking existing code.
            #ifdef RAYTRACING_SHADER_GRAPH_DEFAULT
            #define RAYTRACING_SHADER_GRAPH_HIGH
            #endif
        
            #ifdef RAYTRACING_SHADER_GRAPH_RAYTRACED
            #define RAYTRACING_SHADER_GRAPH_LOW
            #endif
            // end
        
            #ifndef SHADER_UNLIT
            // We need isFrontFace when using double sided - it is not required for unlit as in case of unlit double sided only drive the cullmode
            // VARYINGS_NEED_CULLFACE can be define by VaryingsMeshToPS.FaceSign input if a IsFrontFace Node is included in the shader graph.
            #if defined(_DOUBLESIDED_ON) && !defined(VARYINGS_NEED_CULLFACE)
                #define VARYINGS_NEED_CULLFACE
            #endif
            #endif
        
            // Specific Material Define
        // Setup a define to say we are an unlit shader
        #define SHADER_UNLIT
        
        // Following Macro are only used by Unlit material
        #if defined(_ENABLE_SHADOW_MATTE)
            #if SHADERPASS == SHADERPASS_FORWARD_UNLIT
                #pragma multi_compile_fragment USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST
            #elif SHADERPASS == SHADERPASS_PATH_TRACING
                #define LIGHTLOOP_DISABLE_TILE_AND_CLUSTER
            #endif
        
        // We don't want to have the lightloop defined for the ray tracing passes, but we do for the rasterisation and path tracing shader passes.
        #if !defined(SHADER_STAGE_RAY_TRACING) || SHADERPASS == SHADERPASS_PATH_TRACING
            #define HAS_LIGHTLOOP
        #endif
        #endif
            // Caution: we can use the define SHADER_UNLIT onlit after the above Material include as it is the Unlit template who define it
        
            // To handle SSR on transparent correctly with a possibility to enable/disable it per framesettings
            // we should have a code like this:
            // if !defined(_DISABLE_SSR_TRANSPARENT)
            // pragma multi_compile _ WRITE_NORMAL_BUFFER
            // endif
            // i.e we enable the multicompile only if we can receive SSR or not, and then C# code drive
            // it based on if SSR transparent in frame settings and not (and stripper can strip it).
            // this is currently not possible with our current preprocessor as _DISABLE_SSR_TRANSPARENT is a keyword not a define
            // so instead we used this and chose to pay the extra cost of normal write even if SSR transaprent is disabled.
            // Ideally the shader graph generator should handle it but condition below can't be handle correctly for now.
            #if SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_PREPASS
            #if !defined(_DISABLE_SSR_TRANSPARENT) && !defined(SHADER_UNLIT)
                #define WRITE_NORMAL_BUFFER
            #endif
            #endif
        
            // See Lit.shader
            #if SHADERPASS == SHADERPASS_MOTION_VECTORS && defined(WRITE_DECAL_BUFFER_AND_RENDERING_LAYER)
                #define WRITE_DECAL_BUFFER
            #endif
        
            #ifndef DEBUG_DISPLAY
                // In case of opaque we don't want to perform the alpha test, it is done in depth prepass and we use depth equal for ztest (setup from UI)
                // Don't do it with debug display mode as it is possible there is no depth prepass in this case
                #if !defined(_SURFACE_TYPE_TRANSPARENT)
                    #if SHADERPASS == SHADERPASS_FORWARD
                    #define SHADERPASS_FORWARD_BYPASS_ALPHA_TEST
                    #elif SHADERPASS == SHADERPASS_GBUFFER
                    #define SHADERPASS_GBUFFER_BYPASS_ALPHA_TEST
                    #endif
                #endif
            #endif
        
            // Define _DEFERRED_CAPABLE_MATERIAL for shader capable to run in deferred pass
            #if defined(SHADER_LIT) && !defined(_SURFACE_TYPE_TRANSPARENT)
                #define _DEFERRED_CAPABLE_MATERIAL
            #endif
        
            // Translate transparent motion vector define
            #if (defined(_TRANSPARENT_WRITES_MOTION_VEC) || defined(_TRANSPARENT_REFRACTIVE_SORT)) && defined(_SURFACE_TYPE_TRANSPARENT)
                #define _WRITE_TRANSPARENT_MOTION_VECTOR
            #endif
        
            // -- Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _VoidColor;
        float _Speed;
        float _FresnelPower;
        float4 _BorderColor;
        float2 _Seed;
        float4 _TwirlTexture_TexelSize;
        float _InnerEdge;
        float4 _EmissionColor;
        float _UseShadowThreshold;
        float4 _DoubleSidedConstants;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        float _BlendMode;
        float _EnableBlendModePreserveSpecularLighting;
        CBUFFER_END
        
        
        // Object and Global properties
        TEXTURE2D(_TwirlTexture);
        SAMPLER(sampler_TwirlTexture);
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Includes
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Debug/DebugDisplay.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/Unlit.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/ClassicNoise3D.hlsl"
        #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi3D.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            struct AttributesMesh
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct VaryingsMeshToPS
        {
            SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
             float3 positionRWS;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct SurfaceDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 WorldSpaceNormal;
             float3 ObjectSpaceViewDirection;
             float3 WorldSpaceViewDirection;
             float3 AbsoluteWorldSpacePosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct PackedVaryingsMeshToPS
        {
            SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
             float4 tangentWS : INTERP0;
             float4 texCoord0 : INTERP1;
             float3 positionRWS : INTERP2;
             float3 normalWS : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
        };
        
            PackedVaryingsMeshToPS PackVaryingsMeshToPS (VaryingsMeshToPS input)
        {
            PackedVaryingsMeshToPS output;
            ZERO_INITIALIZE(PackedVaryingsMeshToPS, output);
            output.positionCS = input.positionCS;
            output.tangentWS.xyzw = input.tangentWS;
            output.texCoord0.xyzw = input.texCoord0;
            output.positionRWS.xyz = input.positionRWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            return output;
        }
        
        VaryingsMeshToPS UnpackVaryingsMeshToPS (PackedVaryingsMeshToPS input)
        {
            VaryingsMeshToPS output;
            output.positionCS = input.positionCS;
            output.tangentWS = input.tangentWS.xyzw;
            output.texCoord0 = input.texCoord0.xyzw;
            output.positionRWS = input.positionRWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
        
            // Graph Functions
            
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_DotProduct_float3(float3 A, float3 B, out float Out)
        {
            Out = dot(A, B);
        }
        
        void Unity_SampleGradientV1_float(Gradient Gradient, float Time, out float4 Out)
        {
            // convert to OkLab if we need perceptual color space.
            float3 color = lerp(Gradient.colors[0].rgb, LinearToOklab(Gradient.colors[0].rgb), Gradient.type == 2);
        
            [unroll]
            for (int c = 1; c < Gradient.colorsLength; c++)
            {
                float colorPos = saturate((Time - Gradient.colors[c - 1].w) / (Gradient.colors[c].w - Gradient.colors[c - 1].w)) * step(c, Gradient.colorsLength - 1);
                float3 color2 = lerp(Gradient.colors[c].rgb, LinearToOklab(Gradient.colors[c].rgb), Gradient.type == 2);
                color = lerp(color, color2, lerp(colorPos, step(0.01, colorPos), Gradient.type % 2)); // grad.type == 1 is fixed, 0 and 2 are blends.
            }
            color = lerp(color, OklabToLinear(color), Gradient.type == 2);
        
        #ifdef UNITY_COLORSPACE_GAMMA
            color = LinearToSRGB(color);
        #endif
        
            float alpha = Gradient.alphas[0].x;
            [unroll]
            for (int a = 1; a < Gradient.alphasLength; a++)
            {
                float alphaPos = saturate((Time - Gradient.alphas[a - 1].y) / (Gradient.alphas[a].y - Gradient.alphas[a - 1].y)) * step(a, Gradient.alphasLength - 1);
                alpha = lerp(alpha, Gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), Gradient.type % 2));
            }
        
            Out = float4(color, alpha);
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
        Out = A * B;
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        struct Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float
        {
        };
        
        void SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(float3 Vector3_7940555B, float Vector1_1B8B9078, Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float IN, out float Value_0)
        {
        float3 _Property_44999cc87708de82a26b39ae1da975ec_Out_0_Vector3 = Vector3_7940555B;
        float _Property_dad5add45a7fa785be976f925bc5a5da_Out_0_Float = Vector1_1B8B9078;
        float3 _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3;
        Unity_Multiply_float3_float3(_Property_44999cc87708de82a26b39ae1da975ec_Out_0_Vector3, (_Property_dad5add45a7fa785be976f925bc5a5da_Out_0_Float.xxx), _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3);
        float _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float;
        PerlinNoise3D_float(_Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3, _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float);
        float _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float;
        Unity_Remap_float(_PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float, float2 (-1.15, 1.15), float2 (0, 1), _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float);
        Value_0 = _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_RandomRange_float(float2 Seed, float Min, float Max, out float Out)
        {
             float randomno =  frac(sin(dot(Seed, float2(12.9898, 78.233)))*43758.5453);
             Out = lerp(Min, Max, randomno);
        }
        
        struct Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float
        {
        };
        
        void SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(float3 Vector3_375394F7, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float IN, out float Value_1, out float Cells_2)
        {
        float3 _Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3 = Vector3_375394F7;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        VoronoiPrecise3D_float(_Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_16eef79ff58443149895b35d0a88bdb2_Out_0_Float = _InnerEdge;
            float4 _UV_ce55b36e72754a339073e4c05a64b551_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_054563945a0a4f688fce11f0c067b0c8_Out_1_Vector2 = _UV_ce55b36e72754a339073e4c05a64b551_Out_0_Vector4.xy;
            float _Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float;
            Unity_Distance_float2(_Swizzle_054563945a0a4f688fce11f0c067b0c8_Out_1_Vector2, float2(0.5, 0.5), _Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float);
            float _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float;
            Unity_OneMinus_float(_Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float, _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float);
            float _Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float;
            Unity_Smoothstep_float(_Property_16eef79ff58443149895b35d0a88bdb2_Out_0_Float, float(1), _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float, _Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float);
            Gradient _Gradient_c34cf660310448768ec81f4600b23ad8_Out_0_Gradient = NewGradient(0, 4, 2, float4(0, 0, 0, 0.271931),float4(0.01568037, 0.01568037, 0.01568037, 0.4678416),float4(0.1071531, 0.1071531, 0.1071531, 0.6052644),float4(1, 1, 1, 1),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0), float2(1, 0),float2(1, 1),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0));
            float _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float;
            Unity_DotProduct_float3(IN.ObjectSpaceViewDirection, IN.ObjectSpaceNormal, _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float);
            float4 _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4;
            Unity_SampleGradientV1_float(_Gradient_c34cf660310448768ec81f4600b23ad8_Out_0_Gradient, _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float, _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4);
            float4 _Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float.xxxx), _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4, _Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4);
            Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a;
            float _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float;
            SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(IN.ObjectSpaceViewDirection, float(10), _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a, _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float);
            float3 _Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3;
            Unity_Multiply_float3_float3(IN.ObjectSpaceViewDirection, (_Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float.xxx), _Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3);
            float _Property_ba2d1376c19c40fbadc820d0fb721b2e_Out_0_Float = _Speed;
            float _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_ba2d1376c19c40fbadc820d0fb721b2e_Out_0_Float, _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float);
            float2 _Property_2cfd9c61936548ee85383d83df6beba0_Out_0_Vector2 = _Seed;
            float _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float;
            Unity_RandomRange_float(_Property_2cfd9c61936548ee85383d83df6beba0_Out_0_Vector2, float(26.96), float(45.6), _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float);
            Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a;
            float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Value_1_Float;
            float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float;
            SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(_Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3, _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float, _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Value_1_Float, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float);
            float4 _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4, (_Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float.xxxx), _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4);
            float3 _Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3;
            Unity_Absolute_float3(IN.AbsoluteWorldSpacePosition, _Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3);
            Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1;
            float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Value_1_Float;
            float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float;
            SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(_Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3, IN.TimeParameters.x, float(6.2), _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1, _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Value_1_Float, _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float);
            float _Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float;
            Unity_Smoothstep_float(float(-0.41), float(1), _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float, _Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float);
            float _Property_82bfb359e6494ff08a54e8c08976a6f1_Out_0_Float = _FresnelPower;
            float _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_82bfb359e6494ff08a54e8c08976a6f1_Out_0_Float, _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float);
            float _Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float, _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float, _Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float);
            float4 _Add_0b088d988049415c84334d987e692959_Out_2_Vector4;
            Unity_Add_float4(_Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4, (_Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float.xxxx), _Add_0b088d988049415c84334d987e692959_Out_2_Vector4);
            surface.Alpha = (_Add_0b088d988049415c84334d987e692959_Out_2_Vector4).x;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES AttributesMesh
            #define VaryingsMeshType VaryingsMeshToPS
            #define VFX_SRP_VARYINGS VaryingsMeshType
            #define VFX_SRP_SURFACE_INPUTS FragInputs
            #endif
            
        VertexDescriptionInputs AttributesMeshToVertexDescriptionInputs(AttributesMesh input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else
        #endif
        
            return output;
        }
        
        VertexDescription GetVertexDescription(AttributesMesh input, float3 timeParameters
        #ifdef HAVE_VFX_MODIFICATION
            , AttributesElement element
        #endif
        )
        {
            // build graph inputs
            VertexDescriptionInputs vertexDescriptionInputs = AttributesMeshToVertexDescriptionInputs(input);
            // Override time parameters with used one (This is required to correctly handle motion vectors for vertex animation based on time)
        
            // evaluate vertex graph
        #ifdef HAVE_VFX_MODIFICATION
            GraphProperties properties;
            ZERO_INITIALIZE(GraphProperties, properties);
        
            // Fetch the vertex graph properties for the particle instance.
            GetElementVertexProperties(element, properties);
        
            VertexDescription vertexDescription = VertexDescriptionFunction(vertexDescriptionInputs, properties);
        #else
            VertexDescription vertexDescription = VertexDescriptionFunction(vertexDescriptionInputs);
        #endif
            return vertexDescription;
        
        }
        
        AttributesMesh ApplyMeshModification(AttributesMesh input, float3 timeParameters
        #ifdef USE_CUSTOMINTERP_SUBSTRUCT
            #ifdef TESSELLATION_ON
            , inout VaryingsMeshToDS varyings
            #else
            , inout VaryingsMeshToPS varyings
            #endif
        #endif
        #ifdef HAVE_VFX_MODIFICATION
                , AttributesElement element
        #endif
            )
        {
            VertexDescription vertexDescription = GetVertexDescription(input, timeParameters
        #ifdef HAVE_VFX_MODIFICATION
                , element
        #endif
            );
        
            // copy graph output to the results
            input.positionOS = vertexDescription.Position;
            input.normalOS = vertexDescription.Normal;
            input.tangentOS.xyz = vertexDescription.Tangent;
        
            
        
            return input;
        }
        
        #if defined(_ADD_CUSTOM_VELOCITY) // For shader graph custom velocity
        // Return precomputed Velocity in object space
        float3 GetCustomVelocity(AttributesMesh input
        #ifdef HAVE_VFX_MODIFICATION
            , AttributesElement element
        #endif
        )
        {
            VertexDescription vertexDescription = GetVertexDescription(input, _TimeParameters.xyz
        #ifdef HAVE_VFX_MODIFICATION
                , element
        #endif
            );
            return vertexDescription.CustomVelocity;
        }
        #endif
        
        FragInputs BuildFragInputs(VaryingsMeshToPS input)
        {
            FragInputs output;
            ZERO_INITIALIZE(FragInputs, output);
        
            // Init to some default value to make the computer quiet (else it output 'divide by zero' warning even if value is not used).
            // TODO: this is a really poor workaround, but the variable is used in a bunch of places
            // to compute normals which are then passed on elsewhere to compute other values...
            output.tangentToWorld = k_identity3x3;
            output.positionSS = input.positionCS;       // input.positionCS is SV_Position
        
            output.positionRWS =                input.positionRWS;
            output.tangentToWorld =             BuildTangentToWorld(input.tangentWS, input.normalWS);
            output.texCoord0 =                  input.texCoord0;
        
        #if UNITY_ANY_INSTANCING_ENABLED
        #else
        #endif
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
        
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            // splice point to copy custom interpolator fields from varyings to frag inputs
            
        
            return output;
        }
        
        // existing HDRP code uses the combined function to go directly from packed to frag inputs
        FragInputs UnpackVaryingsMeshToFragInputs(PackedVaryingsMeshToPS input)
        {
            UNITY_SETUP_INSTANCE_ID(input);
        #if defined(HAVE_VFX_MODIFICATION) && defined(UNITY_INSTANCING_ENABLED)
            unity_InstanceID = input.instanceID;
        #endif
            VaryingsMeshToPS unpacked = UnpackVaryingsMeshToPS(input);
            return BuildFragInputs(unpacked);
        }
            SurfaceDescriptionInputs FragInputsToSurfaceDescriptionInputs(FragInputs input, float3 viewWS)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            output.WorldSpaceNormal =                           normalize(input.tangentToWorld[2].xyz);
            #if defined(SHADER_STAGE_RAY_TRACING)
            output.ObjectSpaceNormal =                          mul(output.WorldSpaceNormal, (float3x3) ObjectToWorld3x4());
            #else
            output.ObjectSpaceNormal =                          normalize(mul(output.WorldSpaceNormal, (float3x3) UNITY_MATRIX_M));           // transposed multiplication by inverse matrix to handle normal scale
            #endif
            output.WorldSpaceViewDirection =                    normalize(viewWS);
            output.ObjectSpaceViewDirection =                   TransformWorldToObjectDir(output.WorldSpaceViewDirection);
        #if SHADERPASS != SHADERPASS_FOG_VOLUME_VOXELIZATION
        #else
        #endif
            output.AbsoluteWorldSpacePosition =                 GetAbsolutePositionWS(input.positionRWS);
        #if SHADERPASS != SHADERPASS_FOG_VOLUME_VOXELIZATION
        #else
        #endif
        
        #if UNITY_UV_STARTS_AT_TOP
        #else
        #endif
        
        
            output.uv0 =                                        input.texCoord0;
            output.TimeParameters =                             _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        
            // splice point to copy frag inputs custom interpolator pack into the SDI
            
        
            return output;
        }
        
            // --------------------------------------------------
            // Build Surface Data (Specific Material)
        
        void ApplyDecalToSurfaceDataNoNormal(DecalSurfaceData decalSurfaceData, inout SurfaceData surfaceData);
        
        void ApplyDecalAndGetNormal(FragInputs fragInputs, PositionInputs posInput, SurfaceDescription surfaceDescription,
            inout SurfaceData surfaceData)
        {
            float3 doubleSidedConstants = GetDoubleSidedConstants();
        
        #ifdef DECAL_NORMAL_BLENDING
            // SG nodes don't ouptut surface gradients, so if decals require surf grad blending, we have to convert
            // the normal to gradient before applying the decal. We then have to resolve the gradient back to world space
            float3 normalTS;
        
        
        
            #if HAVE_DECALS
            if (_EnableDecals)
            {
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs, alpha);
                ApplyDecalToSurfaceNormal(decalSurfaceData, fragInputs.tangentToWorld[2], normalTS);
                ApplyDecalToSurfaceDataNoNormal(decalSurfaceData, surfaceData);
            }
            #endif
        
            GetNormalWS_SG(fragInputs, normalTS, surfaceData.normalWS, doubleSidedConstants);
        #else
            // normal delivered to master node
        
            #if HAVE_DECALS
            if (_EnableDecals)
            {
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                // Both uses and modifies 'surfaceData.normalWS'.
                DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs, alpha);
                ApplyDecalToSurfaceNormal(decalSurfaceData, surfaceData.normalWS.xyz);
                ApplyDecalToSurfaceDataNoNormal(decalSurfaceData, surfaceData);
            }
            #endif
        #endif
        }
        void BuildSurfaceData(FragInputs fragInputs, inout SurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData)
        {
            // setup defaults -- these are used if the graph doesn't output a value
            ZERO_INITIALIZE(SurfaceData, surfaceData);
        
            // copy across graph values, if defined
        
            #ifdef WRITE_NORMAL_BUFFER
            // When we need to export the normal (in the depth prepass, we write the geometry one)
            surfaceData.normalWS = fragInputs.tangentToWorld[2];
            #endif
        
            #if defined(DEBUG_DISPLAY)
            #if !defined(SHADER_STAGE_RAY_TRACING)
            // Mipmap mode debugging isn't supported with ray tracing as it relies on derivatives
                if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                {
                    #ifdef FRAG_INPUTS_USE_TEXCOORD0
                        surfaceData.color = GET_TEXTURE_STREAMING_DEBUG(posInput.positionSS, fragInputs.texCoord0);
                    #else
                        surfaceData.color = GET_TEXTURE_STREAMING_DEBUG_NO_UV(posInput.positionSS);
                    #endif
                }
            #endif
            #endif
        
            #ifdef _ENABLE_SHADOW_MATTE
        
                #if (SHADERPASS == SHADERPASS_FORWARD_UNLIT) || (SHADERPASS == SHADERPASS_RAYTRACING_GBUFFER) || (SHADERPASS == SHADERPASS_RAYTRACING_INDIRECT) || (SHADERPASS == SHADERPASS_RAYTRACING_FORWARD)
        
                    HDShadowContext shadowContext = InitShadowContext();
        
                    // Evaluate the shadow, the normal is guaranteed if shadow matte is enabled on this shader.
                    float3 shadow3;
                    ShadowLoopMin(shadowContext, posInput, normalize(fragInputs.tangentToWorld[2]), asuint(_ShadowMatteFilter), GetMeshRenderingLayerMask(), shadow3);
        
                    // Compute the average value in the fourth channel
                    float4 shadow = float4(shadow3, dot(shadow3, float3(1.0/3.0, 1.0/3.0, 1.0/3.0)));
        
                    float4 shadowColor = (1.0 - shadow) * surfaceDescription.ShadowTint.rgba;
                    float  localAlpha  = saturate(shadowColor.a + surfaceDescription.Alpha);
        
                    // Keep the nested lerp
                    // With no Color (bsdfData.color.rgb, bsdfData.color.a == 0.0f), just use ShadowColor*Color to avoid a ring of "white" around the shadow
                    // And mix color to consider the Color & ShadowColor alpha (from texture or/and color picker)
                    #ifdef _SURFACE_TYPE_TRANSPARENT
                        surfaceData.color = lerp(shadowColor.rgb * surfaceData.color, lerp(lerp(shadowColor.rgb, surfaceData.color, 1.0 - surfaceDescription.ShadowTint.a), surfaceData.color, shadow.rgb), surfaceDescription.Alpha);
                    #else
                        surfaceData.color = lerp(lerp(shadowColor.rgb, surfaceData.color, 1.0 - surfaceDescription.ShadowTint.a), surfaceData.color, shadow.rgb);
                    #endif
                    localAlpha = ApplyBlendMode(surfaceData.color, localAlpha).a;
        
                    surfaceDescription.Alpha = localAlpha;
        
                #elif SHADERPASS == SHADERPASS_PATH_TRACING
        
                    surfaceData.normalWS = fragInputs.tangentToWorld[2];
                    surfaceData.shadowTint = surfaceDescription.ShadowTint.rgba;
        
                #endif
        
            #endif // _ENABLE_SHADOW_MATTE
        }
        
            // --------------------------------------------------
            // Get Surface And BuiltinData
        
            void GetSurfaceAndBuiltinData(FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData RAY_TRACING_OPTIONAL_PARAMETERS)
            {
                // Don't dither if displaced tessellation (we're fading out the displacement instead to match the next LOD)
                #if !defined(SHADER_STAGE_RAY_TRACING) && !defined(_TESSELLATION_DISPLACEMENT)
                #ifdef LOD_FADE_CROSSFADE // enable dithering LOD transition if user select CrossFade transition in LOD group
                LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                #endif
                #endif
        
                #ifndef SHADER_UNLIT
                #ifdef _DOUBLESIDED_ON
                    float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
                #else
                    float3 doubleSidedConstants = float3(1.0, 1.0, 1.0);
                #endif
        
                ApplyDoubleSidedFlipOrMirror(fragInputs, doubleSidedConstants); // Apply double sided flip on the vertex normal
                #endif // SHADER_UNLIT
        
                SurfaceDescriptionInputs surfaceDescriptionInputs = FragInputsToSurfaceDescriptionInputs(fragInputs, V);
        
                #if defined(HAVE_VFX_MODIFICATION)
                GraphProperties properties;
                ZERO_INITIALIZE(GraphProperties, properties);
        
                GetElementPixelProperties(fragInputs, properties);
        
                SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs, properties);
                #else
                SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);
                #endif
        
                #ifdef DEBUG_DISPLAY
                if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                {
                    surfaceDescription.Alpha = 1.0f;
                }
                #endif
        
                // Perform alpha test very early to save performance (a killed pixel will not sample textures)
                // TODO: split graph evaluation to grab just alpha dependencies first? tricky..
                #ifdef _ALPHATEST_ON
                    float alphaCutoff = surfaceDescription.AlphaClipThreshold;
                    #if SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_PREPASS
                    // The TransparentDepthPrepass is also used with SSR transparent.
                    // If an artists enable transaprent SSR but not the TransparentDepthPrepass itself, then we use AlphaClipThreshold
                    // otherwise if TransparentDepthPrepass is enabled we use AlphaClipThresholdDepthPrepass
                    #elif SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_POSTPASS
                    // DepthPostpass always use its own alpha threshold
                    alphaCutoff = surfaceDescription.AlphaClipThresholdDepthPostpass;
                    #elif (SHADERPASS == SHADERPASS_SHADOWS) || (SHADERPASS == SHADERPASS_RAYTRACING_VISIBILITY)
                    // If use shadow threshold isn't enable we don't allow any test
                    #endif
        
                    GENERIC_ALPHA_TEST(surfaceDescription.Alpha, alphaCutoff);
                #endif
        
                #if !defined(SHADER_STAGE_RAY_TRACING) && _DEPTHOFFSET_ON
                ApplyDepthOffsetPositionInput(V, surfaceDescription.DepthOffset, GetViewForwardDir(), GetWorldToHClipMatrix(), posInput);
                #endif
        
                #ifndef SHADER_UNLIT
                float3 bentNormalWS;
                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData, bentNormalWS);
        
                #ifdef FRAG_INPUTS_USE_TEXCOORD1
                    float4 lightmapTexCoord1 = fragInputs.texCoord1;
                #else
                    float4 lightmapTexCoord1 = float4(0,0,0,0);
                #endif
        
                #ifdef FRAG_INPUTS_USE_TEXCOORD2
                    float4 lightmapTexCoord2 = fragInputs.texCoord2;
                #else
                    float4 lightmapTexCoord2 = float4(0,0,0,0);
                #endif
        
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                // Builtin Data
                // For back lighting we use the oposite vertex normal
                InitBuiltinData(posInput, alpha, bentNormalWS, -fragInputs.tangentToWorld[2], lightmapTexCoord1, lightmapTexCoord2, builtinData);
        
                #else
                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData);
        
                ZERO_BUILTIN_INITIALIZE(builtinData); // No call to InitBuiltinData as we don't have any lighting
                builtinData.opacity = surfaceDescription.Alpha;
        
                #if defined(DEBUG_DISPLAY)
                    // Light Layers are currently not used for the Unlit shader (because it is not lit)
                    // But Unlit objects do cast shadows according to their rendering layer mask, which is what we want to
                    // display in the light layers visualization mode, therefore we need the renderingLayers
                    builtinData.renderingLayers = GetMeshRenderingLayerMask();
                #endif
        
                #endif // SHADER_UNLIT
        
                #ifdef _ALPHATEST_ON
                    // Used for sharpening by alpha to mask - Alpha to covertage is only used with depth only and forward pass (no shadow pass, no transparent pass)
                    builtinData.alphaClipTreshold = alphaCutoff;
                #endif
        
                // override sampleBakedGI - not used by Unlit
        		// When overriding GI, we need to force the isLightmap flag to make sure we don't add APV (sampled in the lightloop) on top of the overridden value (set at GBuffer stage)
        
        
                // Note this will not fully work on transparent surfaces (can check with _SURFACE_TYPE_TRANSPARENT define)
                // We will always overwrite vt feeback with the nearest. So behind transparent surfaces vt will not be resolved
                // This is a limitation of the current MRT approach.
                #ifdef UNITY_VIRTUAL_TEXTURING
                #endif
        
                #if _DEPTHOFFSET_ON
                builtinData.depthOffset = surfaceDescription.DepthOffset;
                #endif
        
                // TODO: We should generate distortion / distortionBlur for non distortion pass
                #if (SHADERPASS == SHADERPASS_DISTORTION)
                builtinData.distortion = surfaceDescription.Distortion;
                builtinData.distortionBlur = surfaceDescription.DistortionBlur;
                #endif
        
                #ifndef SHADER_UNLIT
                // PostInitBuiltinData call ApplyDebugToBuiltinData
                PostInitBuiltinData(V, posInput, surfaceData, builtinData);
                #else
                ApplyDebugToBuiltinData(builtinData);
                #endif
        
                RAY_TRACING_OPTIONAL_ALPHA_TEST_PASS
            }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassDepthOnly.hlsl"
        
            // --------------------------------------------------
            // Visual Effect Vertex Invocations
        
        	#ifdef HAVE_VFX_MODIFICATION
                #if !defined(SHADER_STAGE_RAY_TRACING)
        	    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/VisualEffectVertex.hlsl"
                #else
                #endif
        	#endif
        
            ENDHLSL
        }
        Pass
        {
            Name "META"
            Tags
            {
                "LightMode" = "META"
            }
        
            // Render State
            Cull Off
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma instancing_options renderinglayer
        #pragma target 4.5
        #pragma vertex Vert
        #pragma fragment Frag
        #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch
        #pragma multi_compile_instancing
        
            // Keywords
            #pragma shader_feature _ EDITOR_VISUALIZATION
        #pragma shader_feature _ _SURFACE_TYPE_TRANSPARENT
        #pragma shader_feature_local_fragment _ _ENABLE_FOG_ON_TRANSPARENT
            // GraphKeywords: <None>
        
            // Defines
            #define SHADERPASS SHADERPASS_LIGHT_TRANSPORT
        #define RAYTRACING_SHADER_GRAPH_DEFAULT
        #define SCENEPICKINGPASS 1
        #define SUPPORT_GLOBAL_MIP_BIAS 1
        
            // For custom interpolators to inject a substruct definition before FragInputs definition,
            // allowing for FragInputs to capture CI's intended for ShaderGraph's SDI.
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
        
            // TODO: Merge FragInputsVFX substruct with CustomInterpolators.
        	#ifdef HAVE_VFX_MODIFICATION
        	struct FragInputsVFX
            {
                /* WARNING: $splice Could not find named fragment 'FragInputsVFX' */
            };
            #endif
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl" // Required by Tessellation.hlsl
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl" // Required to be include before we include properties as it define DECLARE_STACK_CB
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl" // Required before including properties as it defines UNITY_TEXTURE_STREAMING_DEBUG_VARS
            // Always include Shader Graph version
            // Always include last to avoid double macros
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl" // Need to be here for Gradient struct definition
        
            // --------------------------------------------------
            // Defines
        
            // Attribute
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define ATTRIBUTES_NEED_TEXCOORD2
            #define ATTRIBUTES_NEED_TEXCOORD3
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_POSITIONPREDISPLACEMENT_WS
            #define VARYINGS_NEED_TANGENT_TO_WORLD
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD1
            #define VARYINGS_NEED_TEXCOORD2
            #define VARYINGS_NEED_TEXCOORD3
        
            #define HAVE_MESH_MODIFICATION
        
            //Strip down the FragInputs.hlsl (on graphics), so we can only optimize the interpolators we use.
            //if by accident something requests contents of FragInputs.hlsl, it will be caught as a compiler error
            //Frag inputs stripping is only enabled when FRAG_INPUTS_ENABLE_STRIPPING is set
            #if !defined(SHADER_STAGE_RAY_TRACING) && SHADERPASS != SHADERPASS_RAYTRACING_GBUFFER && SHADERPASS != SHADERPASS_FULL_SCREEN_DEBUG
            #define FRAG_INPUTS_ENABLE_STRIPPING
            #endif
            #define FRAG_INPUTS_USE_TEXCOORD0
            #define FRAG_INPUTS_USE_TEXCOORD1
            #define FRAG_INPUTS_USE_TEXCOORD2
            #define FRAG_INPUTS_USE_TEXCOORD3
        
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
        
        
        
        
            // Following two define are a workaround introduce in 10.1.x for RaytracingQualityNode
            // The ShaderGraph don't support correctly migration of this node as it serialize all the node data
            // in the json file making it impossible to uprgrade. Until we get a fix, we do a workaround here
            // to still allow us to rename the field and keyword of this node without breaking existing code.
            #ifdef RAYTRACING_SHADER_GRAPH_DEFAULT
            #define RAYTRACING_SHADER_GRAPH_HIGH
            #endif
        
            #ifdef RAYTRACING_SHADER_GRAPH_RAYTRACED
            #define RAYTRACING_SHADER_GRAPH_LOW
            #endif
            // end
        
            #ifndef SHADER_UNLIT
            // We need isFrontFace when using double sided - it is not required for unlit as in case of unlit double sided only drive the cullmode
            // VARYINGS_NEED_CULLFACE can be define by VaryingsMeshToPS.FaceSign input if a IsFrontFace Node is included in the shader graph.
            #if defined(_DOUBLESIDED_ON) && !defined(VARYINGS_NEED_CULLFACE)
                #define VARYINGS_NEED_CULLFACE
            #endif
            #endif
        
            // Specific Material Define
        // Setup a define to say we are an unlit shader
        #define SHADER_UNLIT
        
        // Following Macro are only used by Unlit material
        #if defined(_ENABLE_SHADOW_MATTE)
            #if SHADERPASS == SHADERPASS_FORWARD_UNLIT
                #pragma multi_compile_fragment USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST
            #elif SHADERPASS == SHADERPASS_PATH_TRACING
                #define LIGHTLOOP_DISABLE_TILE_AND_CLUSTER
            #endif
        
        // We don't want to have the lightloop defined for the ray tracing passes, but we do for the rasterisation and path tracing shader passes.
        #if !defined(SHADER_STAGE_RAY_TRACING) || SHADERPASS == SHADERPASS_PATH_TRACING
            #define HAS_LIGHTLOOP
        #endif
        #endif
            // Caution: we can use the define SHADER_UNLIT onlit after the above Material include as it is the Unlit template who define it
        
            // To handle SSR on transparent correctly with a possibility to enable/disable it per framesettings
            // we should have a code like this:
            // if !defined(_DISABLE_SSR_TRANSPARENT)
            // pragma multi_compile _ WRITE_NORMAL_BUFFER
            // endif
            // i.e we enable the multicompile only if we can receive SSR or not, and then C# code drive
            // it based on if SSR transparent in frame settings and not (and stripper can strip it).
            // this is currently not possible with our current preprocessor as _DISABLE_SSR_TRANSPARENT is a keyword not a define
            // so instead we used this and chose to pay the extra cost of normal write even if SSR transaprent is disabled.
            // Ideally the shader graph generator should handle it but condition below can't be handle correctly for now.
            #if SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_PREPASS
            #if !defined(_DISABLE_SSR_TRANSPARENT) && !defined(SHADER_UNLIT)
                #define WRITE_NORMAL_BUFFER
            #endif
            #endif
        
            // See Lit.shader
            #if SHADERPASS == SHADERPASS_MOTION_VECTORS && defined(WRITE_DECAL_BUFFER_AND_RENDERING_LAYER)
                #define WRITE_DECAL_BUFFER
            #endif
        
            #ifndef DEBUG_DISPLAY
                // In case of opaque we don't want to perform the alpha test, it is done in depth prepass and we use depth equal for ztest (setup from UI)
                // Don't do it with debug display mode as it is possible there is no depth prepass in this case
                #if !defined(_SURFACE_TYPE_TRANSPARENT)
                    #if SHADERPASS == SHADERPASS_FORWARD
                    #define SHADERPASS_FORWARD_BYPASS_ALPHA_TEST
                    #elif SHADERPASS == SHADERPASS_GBUFFER
                    #define SHADERPASS_GBUFFER_BYPASS_ALPHA_TEST
                    #endif
                #endif
            #endif
        
            // Define _DEFERRED_CAPABLE_MATERIAL for shader capable to run in deferred pass
            #if defined(SHADER_LIT) && !defined(_SURFACE_TYPE_TRANSPARENT)
                #define _DEFERRED_CAPABLE_MATERIAL
            #endif
        
            // Translate transparent motion vector define
            #if (defined(_TRANSPARENT_WRITES_MOTION_VEC) || defined(_TRANSPARENT_REFRACTIVE_SORT)) && defined(_SURFACE_TYPE_TRANSPARENT)
                #define _WRITE_TRANSPARENT_MOTION_VECTOR
            #endif
        
            // -- Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _VoidColor;
        float _Speed;
        float _FresnelPower;
        float4 _BorderColor;
        float2 _Seed;
        float4 _TwirlTexture_TexelSize;
        float _InnerEdge;
        float4 _EmissionColor;
        float _UseShadowThreshold;
        float4 _DoubleSidedConstants;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        float _BlendMode;
        float _EnableBlendModePreserveSpecularLighting;
        CBUFFER_END
        
        
        // Object and Global properties
        TEXTURE2D(_TwirlTexture);
        SAMPLER(sampler_TwirlTexture);
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Includes
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/PickingSpaceTransforms.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Debug/DebugDisplay.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/Unlit.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/ClassicNoise3D.hlsl"
        #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi3D.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonLighting.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            struct AttributesMesh
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
             float4 uv2 : TEXCOORD2;
             float4 uv3 : TEXCOORD3;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct VaryingsMeshToPS
        {
            SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
             float3 positionRWS;
             float3 positionPredisplacementRWS;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
             float4 texCoord1;
             float4 texCoord2;
             float4 texCoord3;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
        };
        struct VertexDescriptionInputs
        {
        };
        struct SurfaceDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 WorldSpaceNormal;
             float3 ObjectSpaceViewDirection;
             float3 WorldSpaceViewDirection;
             float3 AbsoluteWorldSpacePosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct PackedVaryingsMeshToPS
        {
            SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
             float4 tangentWS : INTERP0;
             float4 texCoord0 : INTERP1;
             float4 texCoord1 : INTERP2;
             float4 texCoord2 : INTERP3;
             float4 texCoord3 : INTERP4;
             float3 positionRWS : INTERP5;
             float3 positionPredisplacementRWS : INTERP6;
             float3 normalWS : INTERP7;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
        };
        
            PackedVaryingsMeshToPS PackVaryingsMeshToPS (VaryingsMeshToPS input)
        {
            PackedVaryingsMeshToPS output;
            ZERO_INITIALIZE(PackedVaryingsMeshToPS, output);
            output.positionCS = input.positionCS;
            output.tangentWS.xyzw = input.tangentWS;
            output.texCoord0.xyzw = input.texCoord0;
            output.texCoord1.xyzw = input.texCoord1;
            output.texCoord2.xyzw = input.texCoord2;
            output.texCoord3.xyzw = input.texCoord3;
            output.positionRWS.xyz = input.positionRWS;
            output.positionPredisplacementRWS.xyz = input.positionPredisplacementRWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            return output;
        }
        
        VaryingsMeshToPS UnpackVaryingsMeshToPS (PackedVaryingsMeshToPS input)
        {
            VaryingsMeshToPS output;
            output.positionCS = input.positionCS;
            output.tangentWS = input.tangentWS.xyzw;
            output.texCoord0 = input.texCoord0.xyzw;
            output.texCoord1 = input.texCoord1.xyzw;
            output.texCoord2 = input.texCoord2.xyzw;
            output.texCoord3 = input.texCoord3.xyzw;
            output.positionRWS = input.positionRWS.xyz;
            output.positionPredisplacementRWS = input.positionPredisplacementRWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
        
            // Graph Functions
            
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_DotProduct_float3(float3 A, float3 B, out float Out)
        {
            Out = dot(A, B);
        }
        
        void Unity_SampleGradientV1_float(Gradient Gradient, float Time, out float4 Out)
        {
            // convert to OkLab if we need perceptual color space.
            float3 color = lerp(Gradient.colors[0].rgb, LinearToOklab(Gradient.colors[0].rgb), Gradient.type == 2);
        
            [unroll]
            for (int c = 1; c < Gradient.colorsLength; c++)
            {
                float colorPos = saturate((Time - Gradient.colors[c - 1].w) / (Gradient.colors[c].w - Gradient.colors[c - 1].w)) * step(c, Gradient.colorsLength - 1);
                float3 color2 = lerp(Gradient.colors[c].rgb, LinearToOklab(Gradient.colors[c].rgb), Gradient.type == 2);
                color = lerp(color, color2, lerp(colorPos, step(0.01, colorPos), Gradient.type % 2)); // grad.type == 1 is fixed, 0 and 2 are blends.
            }
            color = lerp(color, OklabToLinear(color), Gradient.type == 2);
        
        #ifdef UNITY_COLORSPACE_GAMMA
            color = LinearToSRGB(color);
        #endif
        
            float alpha = Gradient.alphas[0].x;
            [unroll]
            for (int a = 1; a < Gradient.alphasLength; a++)
            {
                float alphaPos = saturate((Time - Gradient.alphas[a - 1].y) / (Gradient.alphas[a].y - Gradient.alphas[a - 1].y)) * step(a, Gradient.alphasLength - 1);
                alpha = lerp(alpha, Gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), Gradient.type % 2));
            }
        
            Out = float4(color, alpha);
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
        Out = A * B;
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        struct Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float
        {
        };
        
        void SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(float3 Vector3_7940555B, float Vector1_1B8B9078, Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float IN, out float Value_0)
        {
        float3 _Property_44999cc87708de82a26b39ae1da975ec_Out_0_Vector3 = Vector3_7940555B;
        float _Property_dad5add45a7fa785be976f925bc5a5da_Out_0_Float = Vector1_1B8B9078;
        float3 _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3;
        Unity_Multiply_float3_float3(_Property_44999cc87708de82a26b39ae1da975ec_Out_0_Vector3, (_Property_dad5add45a7fa785be976f925bc5a5da_Out_0_Float.xxx), _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3);
        float _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float;
        PerlinNoise3D_float(_Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3, _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float);
        float _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float;
        Unity_Remap_float(_PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float, float2 (-1.15, 1.15), float2 (0, 1), _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float);
        Value_0 = _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_RandomRange_float(float2 Seed, float Min, float Max, out float Out)
        {
             float randomno =  frac(sin(dot(Seed, float2(12.9898, 78.233)))*43758.5453);
             Out = lerp(Min, Max, randomno);
        }
        
        struct Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float
        {
        };
        
        void SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(float3 Vector3_375394F7, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float IN, out float Value_1, out float Cells_2)
        {
        float3 _Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3 = Vector3_375394F7;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        VoronoiPrecise3D_float(_Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }
        
        float3 Unity_HDRP_GetEmissionHDRColor_float(float3 ldrColor, float luminanceIntensity, float exposureWeight)
        {
            float3 hdrColor = ldrColor * luminanceIntensity;
        
            #ifdef SHADERGRAPH_PREVIEW
            float inverseExposureMultiplier = 1.0;
            #else
            float inverseExposureMultiplier = GetInverseCurrentExposureMultiplier();
            #endif
        
            // Inverse pre-expose using _EmissiveExposureWeight weight
            hdrColor = lerp(hdrColor * inverseExposureMultiplier, hdrColor, exposureWeight);
            return hdrColor;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
            // Graph Vertex
            struct VertexDescription
        {
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            return description;
        }
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float3 Emission;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_d09e7f2c2a774dbd93cc943fa6a50940_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_VoidColor) : _VoidColor;
            float _Property_16eef79ff58443149895b35d0a88bdb2_Out_0_Float = _InnerEdge;
            float4 _UV_ce55b36e72754a339073e4c05a64b551_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_054563945a0a4f688fce11f0c067b0c8_Out_1_Vector2 = _UV_ce55b36e72754a339073e4c05a64b551_Out_0_Vector4.xy;
            float _Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float;
            Unity_Distance_float2(_Swizzle_054563945a0a4f688fce11f0c067b0c8_Out_1_Vector2, float2(0.5, 0.5), _Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float);
            float _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float;
            Unity_OneMinus_float(_Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float, _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float);
            float _Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float;
            Unity_Smoothstep_float(_Property_16eef79ff58443149895b35d0a88bdb2_Out_0_Float, float(1), _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float, _Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float);
            Gradient _Gradient_c34cf660310448768ec81f4600b23ad8_Out_0_Gradient = NewGradient(0, 4, 2, float4(0, 0, 0, 0.271931),float4(0.01568037, 0.01568037, 0.01568037, 0.4678416),float4(0.1071531, 0.1071531, 0.1071531, 0.6052644),float4(1, 1, 1, 1),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0), float2(1, 0),float2(1, 1),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0));
            float _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float;
            Unity_DotProduct_float3(IN.ObjectSpaceViewDirection, IN.ObjectSpaceNormal, _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float);
            float4 _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4;
            Unity_SampleGradientV1_float(_Gradient_c34cf660310448768ec81f4600b23ad8_Out_0_Gradient, _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float, _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4);
            float4 _Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float.xxxx), _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4, _Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4);
            Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a;
            float _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float;
            SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(IN.ObjectSpaceViewDirection, float(10), _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a, _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float);
            float3 _Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3;
            Unity_Multiply_float3_float3(IN.ObjectSpaceViewDirection, (_Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float.xxx), _Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3);
            float _Property_ba2d1376c19c40fbadc820d0fb721b2e_Out_0_Float = _Speed;
            float _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_ba2d1376c19c40fbadc820d0fb721b2e_Out_0_Float, _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float);
            float2 _Property_2cfd9c61936548ee85383d83df6beba0_Out_0_Vector2 = _Seed;
            float _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float;
            Unity_RandomRange_float(_Property_2cfd9c61936548ee85383d83df6beba0_Out_0_Vector2, float(26.96), float(45.6), _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float);
            Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a;
            float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Value_1_Float;
            float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float;
            SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(_Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3, _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float, _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Value_1_Float, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float);
            float4 _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4, (_Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float.xxxx), _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4);
            float4 _Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_d09e7f2c2a774dbd93cc943fa6a50940_Out_0_Vector4, _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4, _Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4);
            float4 _Property_e0f9565d1eaa452fb638faa34cc517eb_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_BorderColor) : _BorderColor;
            float3 _Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3;
            Unity_Absolute_float3(IN.AbsoluteWorldSpacePosition, _Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3);
            Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1;
            float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Value_1_Float;
            float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float;
            SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(_Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3, IN.TimeParameters.x, float(6.2), _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1, _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Value_1_Float, _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float);
            float _Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float;
            Unity_Smoothstep_float(float(-0.41), float(1), _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float, _Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float);
            float _Property_82bfb359e6494ff08a54e8c08976a6f1_Out_0_Float = _FresnelPower;
            float _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_82bfb359e6494ff08a54e8c08976a6f1_Out_0_Float, _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float);
            float _Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float, _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float, _Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float);
            float4 _Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_e0f9565d1eaa452fb638faa34cc517eb_Out_0_Vector4, (_Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float.xxxx), _Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4);
            float3 _EmissionNode_87074dafd5ff4d39bf4005b1089a56e4_Output_0_Vector3 = Unity_HDRP_GetEmissionHDRColor_float((_Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4.xyz).xyz, float(3), float(0));
            float4 _Add_0b088d988049415c84334d987e692959_Out_2_Vector4;
            Unity_Add_float4(_Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4, (_Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float.xxxx), _Add_0b088d988049415c84334d987e692959_Out_2_Vector4);
            surface.BaseColor = (_Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4.xyz);
            surface.Emission = _EmissionNode_87074dafd5ff4d39bf4005b1089a56e4_Output_0_Vector3;
            surface.Alpha = (_Add_0b088d988049415c84334d987e692959_Out_2_Vector4).x;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES AttributesMesh
            #define VaryingsMeshType VaryingsMeshToPS
            #define VFX_SRP_VARYINGS VaryingsMeshType
            #define VFX_SRP_SURFACE_INPUTS FragInputs
            #endif
            
        VertexDescriptionInputs AttributesMeshToVertexDescriptionInputs(AttributesMesh input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
        #if UNITY_ANY_INSTANCING_ENABLED
        #else
        #endif
        
            return output;
        }
        
        VertexDescription GetVertexDescription(AttributesMesh input, float3 timeParameters
        #ifdef HAVE_VFX_MODIFICATION
            , AttributesElement element
        #endif
        )
        {
            // build graph inputs
            VertexDescriptionInputs vertexDescriptionInputs = AttributesMeshToVertexDescriptionInputs(input);
            // Override time parameters with used one (This is required to correctly handle motion vectors for vertex animation based on time)
        
            // evaluate vertex graph
        #ifdef HAVE_VFX_MODIFICATION
            GraphProperties properties;
            ZERO_INITIALIZE(GraphProperties, properties);
        
            // Fetch the vertex graph properties for the particle instance.
            GetElementVertexProperties(element, properties);
        
            VertexDescription vertexDescription = VertexDescriptionFunction(vertexDescriptionInputs, properties);
        #else
            VertexDescription vertexDescription = VertexDescriptionFunction(vertexDescriptionInputs);
        #endif
            return vertexDescription;
        
        }
        
        AttributesMesh ApplyMeshModification(AttributesMesh input, float3 timeParameters
        #ifdef USE_CUSTOMINTERP_SUBSTRUCT
            #ifdef TESSELLATION_ON
            , inout VaryingsMeshToDS varyings
            #else
            , inout VaryingsMeshToPS varyings
            #endif
        #endif
        #ifdef HAVE_VFX_MODIFICATION
                , AttributesElement element
        #endif
            )
        {
            VertexDescription vertexDescription = GetVertexDescription(input, timeParameters
        #ifdef HAVE_VFX_MODIFICATION
                , element
        #endif
            );
        
            // copy graph output to the results
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorVertMeshCustomInterpolation' */
        
            return input;
        }
        
        #if defined(_ADD_CUSTOM_VELOCITY) // For shader graph custom velocity
        // Return precomputed Velocity in object space
        float3 GetCustomVelocity(AttributesMesh input
        #ifdef HAVE_VFX_MODIFICATION
            , AttributesElement element
        #endif
        )
        {
            VertexDescription vertexDescription = GetVertexDescription(input, _TimeParameters.xyz
        #ifdef HAVE_VFX_MODIFICATION
                , element
        #endif
            );
            return vertexDescription.CustomVelocity;
        }
        #endif
        
        FragInputs BuildFragInputs(VaryingsMeshToPS input)
        {
            FragInputs output;
            ZERO_INITIALIZE(FragInputs, output);
        
            // Init to some default value to make the computer quiet (else it output 'divide by zero' warning even if value is not used).
            // TODO: this is a really poor workaround, but the variable is used in a bunch of places
            // to compute normals which are then passed on elsewhere to compute other values...
            output.tangentToWorld = k_identity3x3;
            output.positionSS = input.positionCS;       // input.positionCS is SV_Position
        
            output.positionRWS =                input.positionRWS;
            output.positionPredisplacementRWS = input.positionPredisplacementRWS;
            output.tangentToWorld =             BuildTangentToWorld(input.tangentWS, input.normalWS);
            output.texCoord0 =                  input.texCoord0;
            output.texCoord1 =                  input.texCoord1;
            output.texCoord2 =                  input.texCoord2;
            output.texCoord3 =                  input.texCoord3;
        
        #if UNITY_ANY_INSTANCING_ENABLED
        #else
        #endif
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
        
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            // splice point to copy custom interpolator fields from varyings to frag inputs
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorVaryingsToFragInputs' */
        
            return output;
        }
        
        // existing HDRP code uses the combined function to go directly from packed to frag inputs
        FragInputs UnpackVaryingsMeshToFragInputs(PackedVaryingsMeshToPS input)
        {
            UNITY_SETUP_INSTANCE_ID(input);
        #if defined(HAVE_VFX_MODIFICATION) && defined(UNITY_INSTANCING_ENABLED)
            unity_InstanceID = input.instanceID;
        #endif
            VaryingsMeshToPS unpacked = UnpackVaryingsMeshToPS(input);
            return BuildFragInputs(unpacked);
        }
            SurfaceDescriptionInputs FragInputsToSurfaceDescriptionInputs(FragInputs input, float3 viewWS)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            output.WorldSpaceNormal =                           normalize(input.tangentToWorld[2].xyz);
            #if defined(SHADER_STAGE_RAY_TRACING)
            output.ObjectSpaceNormal =                          mul(output.WorldSpaceNormal, (float3x3) ObjectToWorld3x4());
            #else
            output.ObjectSpaceNormal =                          normalize(mul(output.WorldSpaceNormal, (float3x3) UNITY_MATRIX_M));           // transposed multiplication by inverse matrix to handle normal scale
            #endif
            output.WorldSpaceViewDirection =                    normalize(viewWS);
            output.ObjectSpaceViewDirection =                   TransformWorldToObjectDir(output.WorldSpaceViewDirection);
        #if SHADERPASS != SHADERPASS_FOG_VOLUME_VOXELIZATION
        #else
        #endif
            output.AbsoluteWorldSpacePosition =                 GetAbsolutePositionWS(input.positionRWS);
        #if SHADERPASS != SHADERPASS_FOG_VOLUME_VOXELIZATION
        #else
        #endif
        
        #if UNITY_UV_STARTS_AT_TOP
        #else
        #endif
        
        
            output.uv0 =                                        input.texCoord0;
            output.TimeParameters =                             _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        
            // splice point to copy frag inputs custom interpolator pack into the SDI
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorCopyToSDI' */
        
            return output;
        }
        
            // --------------------------------------------------
            // Build Surface Data (Specific Material)
        
        void ApplyDecalToSurfaceDataNoNormal(DecalSurfaceData decalSurfaceData, inout SurfaceData surfaceData);
        
        void ApplyDecalAndGetNormal(FragInputs fragInputs, PositionInputs posInput, SurfaceDescription surfaceDescription,
            inout SurfaceData surfaceData)
        {
            float3 doubleSidedConstants = GetDoubleSidedConstants();
        
        #ifdef DECAL_NORMAL_BLENDING
            // SG nodes don't ouptut surface gradients, so if decals require surf grad blending, we have to convert
            // the normal to gradient before applying the decal. We then have to resolve the gradient back to world space
            float3 normalTS;
        
        
        
            #if HAVE_DECALS
            if (_EnableDecals)
            {
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs, alpha);
                ApplyDecalToSurfaceNormal(decalSurfaceData, fragInputs.tangentToWorld[2], normalTS);
                ApplyDecalToSurfaceDataNoNormal(decalSurfaceData, surfaceData);
            }
            #endif
        
            GetNormalWS_SG(fragInputs, normalTS, surfaceData.normalWS, doubleSidedConstants);
        #else
            // normal delivered to master node
        
            #if HAVE_DECALS
            if (_EnableDecals)
            {
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                // Both uses and modifies 'surfaceData.normalWS'.
                DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs, alpha);
                ApplyDecalToSurfaceNormal(decalSurfaceData, surfaceData.normalWS.xyz);
                ApplyDecalToSurfaceDataNoNormal(decalSurfaceData, surfaceData);
            }
            #endif
        #endif
        }
        void BuildSurfaceData(FragInputs fragInputs, inout SurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData)
        {
            // setup defaults -- these are used if the graph doesn't output a value
            ZERO_INITIALIZE(SurfaceData, surfaceData);
        
            // copy across graph values, if defined
            surfaceData.color = surfaceDescription.BaseColor;
        
            #ifdef WRITE_NORMAL_BUFFER
            // When we need to export the normal (in the depth prepass, we write the geometry one)
            surfaceData.normalWS = fragInputs.tangentToWorld[2];
            #endif
        
            #if defined(DEBUG_DISPLAY)
            #if !defined(SHADER_STAGE_RAY_TRACING)
            // Mipmap mode debugging isn't supported with ray tracing as it relies on derivatives
                if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                {
                    #ifdef FRAG_INPUTS_USE_TEXCOORD0
                        surfaceData.color = GET_TEXTURE_STREAMING_DEBUG(posInput.positionSS, fragInputs.texCoord0);
                    #else
                        surfaceData.color = GET_TEXTURE_STREAMING_DEBUG_NO_UV(posInput.positionSS);
                    #endif
                }
            #endif
            #endif
        
            #ifdef _ENABLE_SHADOW_MATTE
        
                #if (SHADERPASS == SHADERPASS_FORWARD_UNLIT) || (SHADERPASS == SHADERPASS_RAYTRACING_GBUFFER) || (SHADERPASS == SHADERPASS_RAYTRACING_INDIRECT) || (SHADERPASS == SHADERPASS_RAYTRACING_FORWARD)
        
                    HDShadowContext shadowContext = InitShadowContext();
        
                    // Evaluate the shadow, the normal is guaranteed if shadow matte is enabled on this shader.
                    float3 shadow3;
                    ShadowLoopMin(shadowContext, posInput, normalize(fragInputs.tangentToWorld[2]), asuint(_ShadowMatteFilter), GetMeshRenderingLayerMask(), shadow3);
        
                    // Compute the average value in the fourth channel
                    float4 shadow = float4(shadow3, dot(shadow3, float3(1.0/3.0, 1.0/3.0, 1.0/3.0)));
        
                    float4 shadowColor = (1.0 - shadow) * surfaceDescription.ShadowTint.rgba;
                    float  localAlpha  = saturate(shadowColor.a + surfaceDescription.Alpha);
        
                    // Keep the nested lerp
                    // With no Color (bsdfData.color.rgb, bsdfData.color.a == 0.0f), just use ShadowColor*Color to avoid a ring of "white" around the shadow
                    // And mix color to consider the Color & ShadowColor alpha (from texture or/and color picker)
                    #ifdef _SURFACE_TYPE_TRANSPARENT
                        surfaceData.color = lerp(shadowColor.rgb * surfaceData.color, lerp(lerp(shadowColor.rgb, surfaceData.color, 1.0 - surfaceDescription.ShadowTint.a), surfaceData.color, shadow.rgb), surfaceDescription.Alpha);
                    #else
                        surfaceData.color = lerp(lerp(shadowColor.rgb, surfaceData.color, 1.0 - surfaceDescription.ShadowTint.a), surfaceData.color, shadow.rgb);
                    #endif
                    localAlpha = ApplyBlendMode(surfaceData.color, localAlpha).a;
        
                    surfaceDescription.Alpha = localAlpha;
        
                #elif SHADERPASS == SHADERPASS_PATH_TRACING
        
                    surfaceData.normalWS = fragInputs.tangentToWorld[2];
                    surfaceData.shadowTint = surfaceDescription.ShadowTint.rgba;
        
                #endif
        
            #endif // _ENABLE_SHADOW_MATTE
        }
        
            // --------------------------------------------------
            // Get Surface And BuiltinData
        
            void GetSurfaceAndBuiltinData(FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData RAY_TRACING_OPTIONAL_PARAMETERS)
            {
                // Don't dither if displaced tessellation (we're fading out the displacement instead to match the next LOD)
                #if !defined(SHADER_STAGE_RAY_TRACING) && !defined(_TESSELLATION_DISPLACEMENT)
                #ifdef LOD_FADE_CROSSFADE // enable dithering LOD transition if user select CrossFade transition in LOD group
                LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                #endif
                #endif
        
                #ifndef SHADER_UNLIT
                #ifdef _DOUBLESIDED_ON
                    float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
                #else
                    float3 doubleSidedConstants = float3(1.0, 1.0, 1.0);
                #endif
        
                ApplyDoubleSidedFlipOrMirror(fragInputs, doubleSidedConstants); // Apply double sided flip on the vertex normal
                #endif // SHADER_UNLIT
        
                SurfaceDescriptionInputs surfaceDescriptionInputs = FragInputsToSurfaceDescriptionInputs(fragInputs, V);
        
                #if defined(HAVE_VFX_MODIFICATION)
                GraphProperties properties;
                ZERO_INITIALIZE(GraphProperties, properties);
        
                GetElementPixelProperties(fragInputs, properties);
        
                SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs, properties);
                #else
                SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);
                #endif
        
                #ifdef DEBUG_DISPLAY
                if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                {
                    surfaceDescription.Alpha = 1.0f;
                }
                #endif
        
                // Perform alpha test very early to save performance (a killed pixel will not sample textures)
                // TODO: split graph evaluation to grab just alpha dependencies first? tricky..
                #ifdef _ALPHATEST_ON
                    float alphaCutoff = surfaceDescription.AlphaClipThreshold;
                    #if SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_PREPASS
                    // The TransparentDepthPrepass is also used with SSR transparent.
                    // If an artists enable transaprent SSR but not the TransparentDepthPrepass itself, then we use AlphaClipThreshold
                    // otherwise if TransparentDepthPrepass is enabled we use AlphaClipThresholdDepthPrepass
                    #elif SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_POSTPASS
                    // DepthPostpass always use its own alpha threshold
                    alphaCutoff = surfaceDescription.AlphaClipThresholdDepthPostpass;
                    #elif (SHADERPASS == SHADERPASS_SHADOWS) || (SHADERPASS == SHADERPASS_RAYTRACING_VISIBILITY)
                    // If use shadow threshold isn't enable we don't allow any test
                    #endif
        
                    GENERIC_ALPHA_TEST(surfaceDescription.Alpha, alphaCutoff);
                #endif
        
                #if !defined(SHADER_STAGE_RAY_TRACING) && _DEPTHOFFSET_ON
                ApplyDepthOffsetPositionInput(V, surfaceDescription.DepthOffset, GetViewForwardDir(), GetWorldToHClipMatrix(), posInput);
                #endif
        
                #ifndef SHADER_UNLIT
                float3 bentNormalWS;
                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData, bentNormalWS);
        
                #ifdef FRAG_INPUTS_USE_TEXCOORD1
                    float4 lightmapTexCoord1 = fragInputs.texCoord1;
                #else
                    float4 lightmapTexCoord1 = float4(0,0,0,0);
                #endif
        
                #ifdef FRAG_INPUTS_USE_TEXCOORD2
                    float4 lightmapTexCoord2 = fragInputs.texCoord2;
                #else
                    float4 lightmapTexCoord2 = float4(0,0,0,0);
                #endif
        
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                // Builtin Data
                // For back lighting we use the oposite vertex normal
                InitBuiltinData(posInput, alpha, bentNormalWS, -fragInputs.tangentToWorld[2], lightmapTexCoord1, lightmapTexCoord2, builtinData);
        
                #else
                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData);
        
                ZERO_BUILTIN_INITIALIZE(builtinData); // No call to InitBuiltinData as we don't have any lighting
                builtinData.opacity = surfaceDescription.Alpha;
        
                #if defined(DEBUG_DISPLAY)
                    // Light Layers are currently not used for the Unlit shader (because it is not lit)
                    // But Unlit objects do cast shadows according to their rendering layer mask, which is what we want to
                    // display in the light layers visualization mode, therefore we need the renderingLayers
                    builtinData.renderingLayers = GetMeshRenderingLayerMask();
                #endif
        
                #endif // SHADER_UNLIT
        
                #ifdef _ALPHATEST_ON
                    // Used for sharpening by alpha to mask - Alpha to covertage is only used with depth only and forward pass (no shadow pass, no transparent pass)
                    builtinData.alphaClipTreshold = alphaCutoff;
                #endif
        
                // override sampleBakedGI - not used by Unlit
        		// When overriding GI, we need to force the isLightmap flag to make sure we don't add APV (sampled in the lightloop) on top of the overridden value (set at GBuffer stage)
        
                builtinData.emissiveColor = surfaceDescription.Emission;
        
                // Note this will not fully work on transparent surfaces (can check with _SURFACE_TYPE_TRANSPARENT define)
                // We will always overwrite vt feeback with the nearest. So behind transparent surfaces vt will not be resolved
                // This is a limitation of the current MRT approach.
                #ifdef UNITY_VIRTUAL_TEXTURING
                #endif
        
                #if _DEPTHOFFSET_ON
                builtinData.depthOffset = surfaceDescription.DepthOffset;
                #endif
        
                // TODO: We should generate distortion / distortionBlur for non distortion pass
                #if (SHADERPASS == SHADERPASS_DISTORTION)
                builtinData.distortion = surfaceDescription.Distortion;
                builtinData.distortionBlur = surfaceDescription.DistortionBlur;
                #endif
        
                #ifndef SHADER_UNLIT
                // PostInitBuiltinData call ApplyDebugToBuiltinData
                PostInitBuiltinData(V, posInput, surfaceData, builtinData);
                #else
                ApplyDebugToBuiltinData(builtinData);
                #endif
        
                RAY_TRACING_OPTIONAL_ALPHA_TEST_PASS
            }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassLightTransport.hlsl"
        
            // --------------------------------------------------
            // Visual Effect Vertex Invocations
        
        	#ifdef HAVE_VFX_MODIFICATION
                #if !defined(SHADER_STAGE_RAY_TRACING)
        	    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/VisualEffectVertex.hlsl"
                #else
                #endif
        	#endif
        
            ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }
        
            // Render State
            Cull [_CullMode]
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma editor_sync_compilation
        #pragma instancing_options renderinglayer
        #pragma target 4.5
        #pragma vertex Vert
        #pragma fragment Frag
        #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch
        #pragma multi_compile_instancing
        
            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>
        
            // Defines
            #define SHADERPASS SHADERPASS_DEPTH_ONLY
        #define SCENEPICKINGPASS 1
        #define SUPPORT_GLOBAL_MIP_BIAS 1
        
            // For custom interpolators to inject a substruct definition before FragInputs definition,
            // allowing for FragInputs to capture CI's intended for ShaderGraph's SDI.
            struct CustomInterpolators
        {
        };
        #define USE_CUSTOMINTERP_SUBSTRUCT
        
        
        
            // TODO: Merge FragInputsVFX substruct with CustomInterpolators.
        	#ifdef HAVE_VFX_MODIFICATION
        	struct FragInputsVFX
            {
                /* WARNING: $splice Could not find named fragment 'FragInputsVFX' */
            };
            #endif
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl" // Required by Tessellation.hlsl
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl" // Required to be include before we include properties as it define DECLARE_STACK_CB
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl" // Required before including properties as it defines UNITY_TEXTURE_STREAMING_DEBUG_VARS
            // Always include Shader Graph version
            // Always include last to avoid double macros
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl" // Need to be here for Gradient struct definition
        
            // --------------------------------------------------
            // Defines
        
            // Attribute
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TANGENT_TO_WORLD
            #define VARYINGS_NEED_TEXCOORD0
        
            #define HAVE_MESH_MODIFICATION
        
            //Strip down the FragInputs.hlsl (on graphics), so we can only optimize the interpolators we use.
            //if by accident something requests contents of FragInputs.hlsl, it will be caught as a compiler error
            //Frag inputs stripping is only enabled when FRAG_INPUTS_ENABLE_STRIPPING is set
            #if !defined(SHADER_STAGE_RAY_TRACING) && SHADERPASS != SHADERPASS_RAYTRACING_GBUFFER && SHADERPASS != SHADERPASS_FULL_SCREEN_DEBUG
            #define FRAG_INPUTS_ENABLE_STRIPPING
            #endif
            #define FRAG_INPUTS_USE_TEXCOORD0
        
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
        
        
        
        
            // Following two define are a workaround introduce in 10.1.x for RaytracingQualityNode
            // The ShaderGraph don't support correctly migration of this node as it serialize all the node data
            // in the json file making it impossible to uprgrade. Until we get a fix, we do a workaround here
            // to still allow us to rename the field and keyword of this node without breaking existing code.
            #ifdef RAYTRACING_SHADER_GRAPH_DEFAULT
            #define RAYTRACING_SHADER_GRAPH_HIGH
            #endif
        
            #ifdef RAYTRACING_SHADER_GRAPH_RAYTRACED
            #define RAYTRACING_SHADER_GRAPH_LOW
            #endif
            // end
        
            #ifndef SHADER_UNLIT
            // We need isFrontFace when using double sided - it is not required for unlit as in case of unlit double sided only drive the cullmode
            // VARYINGS_NEED_CULLFACE can be define by VaryingsMeshToPS.FaceSign input if a IsFrontFace Node is included in the shader graph.
            #if defined(_DOUBLESIDED_ON) && !defined(VARYINGS_NEED_CULLFACE)
                #define VARYINGS_NEED_CULLFACE
            #endif
            #endif
        
            // Specific Material Define
        // Setup a define to say we are an unlit shader
        #define SHADER_UNLIT
        
        // Following Macro are only used by Unlit material
        #if defined(_ENABLE_SHADOW_MATTE)
            #if SHADERPASS == SHADERPASS_FORWARD_UNLIT
                #pragma multi_compile_fragment USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST
            #elif SHADERPASS == SHADERPASS_PATH_TRACING
                #define LIGHTLOOP_DISABLE_TILE_AND_CLUSTER
            #endif
        
        // We don't want to have the lightloop defined for the ray tracing passes, but we do for the rasterisation and path tracing shader passes.
        #if !defined(SHADER_STAGE_RAY_TRACING) || SHADERPASS == SHADERPASS_PATH_TRACING
            #define HAS_LIGHTLOOP
        #endif
        #endif
            // Caution: we can use the define SHADER_UNLIT onlit after the above Material include as it is the Unlit template who define it
        
            // To handle SSR on transparent correctly with a possibility to enable/disable it per framesettings
            // we should have a code like this:
            // if !defined(_DISABLE_SSR_TRANSPARENT)
            // pragma multi_compile _ WRITE_NORMAL_BUFFER
            // endif
            // i.e we enable the multicompile only if we can receive SSR or not, and then C# code drive
            // it based on if SSR transparent in frame settings and not (and stripper can strip it).
            // this is currently not possible with our current preprocessor as _DISABLE_SSR_TRANSPARENT is a keyword not a define
            // so instead we used this and chose to pay the extra cost of normal write even if SSR transaprent is disabled.
            // Ideally the shader graph generator should handle it but condition below can't be handle correctly for now.
            #if SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_PREPASS
            #if !defined(_DISABLE_SSR_TRANSPARENT) && !defined(SHADER_UNLIT)
                #define WRITE_NORMAL_BUFFER
            #endif
            #endif
        
            // See Lit.shader
            #if SHADERPASS == SHADERPASS_MOTION_VECTORS && defined(WRITE_DECAL_BUFFER_AND_RENDERING_LAYER)
                #define WRITE_DECAL_BUFFER
            #endif
        
            #ifndef DEBUG_DISPLAY
                // In case of opaque we don't want to perform the alpha test, it is done in depth prepass and we use depth equal for ztest (setup from UI)
                // Don't do it with debug display mode as it is possible there is no depth prepass in this case
                #if !defined(_SURFACE_TYPE_TRANSPARENT)
                    #if SHADERPASS == SHADERPASS_FORWARD
                    #define SHADERPASS_FORWARD_BYPASS_ALPHA_TEST
                    #elif SHADERPASS == SHADERPASS_GBUFFER
                    #define SHADERPASS_GBUFFER_BYPASS_ALPHA_TEST
                    #endif
                #endif
            #endif
        
            // Define _DEFERRED_CAPABLE_MATERIAL for shader capable to run in deferred pass
            #if defined(SHADER_LIT) && !defined(_SURFACE_TYPE_TRANSPARENT)
                #define _DEFERRED_CAPABLE_MATERIAL
            #endif
        
            // Translate transparent motion vector define
            #if (defined(_TRANSPARENT_WRITES_MOTION_VEC) || defined(_TRANSPARENT_REFRACTIVE_SORT)) && defined(_SURFACE_TYPE_TRANSPARENT)
                #define _WRITE_TRANSPARENT_MOTION_VECTOR
            #endif
        
            // -- Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _VoidColor;
        float _Speed;
        float _FresnelPower;
        float4 _BorderColor;
        float2 _Seed;
        float4 _TwirlTexture_TexelSize;
        float _InnerEdge;
        float4 _EmissionColor;
        float _UseShadowThreshold;
        float4 _DoubleSidedConstants;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        float _BlendMode;
        float _EnableBlendModePreserveSpecularLighting;
        CBUFFER_END
        
        
        // Object and Global properties
        TEXTURE2D(_TwirlTexture);
        SAMPLER(sampler_TwirlTexture);
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Includes
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/PickingSpaceTransforms.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Debug/DebugDisplay.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/Unlit.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/ClassicNoise3D.hlsl"
        #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi3D.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonLighting.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            struct AttributesMesh
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct VaryingsMeshToPS
        {
            SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
             float3 positionRWS;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct SurfaceDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 WorldSpaceNormal;
             float3 ObjectSpaceViewDirection;
             float3 WorldSpaceViewDirection;
             float3 AbsoluteWorldSpacePosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct PackedVaryingsMeshToPS
        {
            SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
             float4 tangentWS : INTERP0;
             float4 texCoord0 : INTERP1;
             float3 positionRWS : INTERP2;
             float3 normalWS : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
        };
        
            PackedVaryingsMeshToPS PackVaryingsMeshToPS (VaryingsMeshToPS input)
        {
            PackedVaryingsMeshToPS output;
            ZERO_INITIALIZE(PackedVaryingsMeshToPS, output);
            output.positionCS = input.positionCS;
            output.tangentWS.xyzw = input.tangentWS;
            output.texCoord0.xyzw = input.texCoord0;
            output.positionRWS.xyz = input.positionRWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            return output;
        }
        
        VaryingsMeshToPS UnpackVaryingsMeshToPS (PackedVaryingsMeshToPS input)
        {
            VaryingsMeshToPS output;
            output.positionCS = input.positionCS;
            output.tangentWS = input.tangentWS.xyzw;
            output.texCoord0 = input.texCoord0.xyzw;
            output.positionRWS = input.positionRWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
        
            // Graph Functions
            
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_DotProduct_float3(float3 A, float3 B, out float Out)
        {
            Out = dot(A, B);
        }
        
        void Unity_SampleGradientV1_float(Gradient Gradient, float Time, out float4 Out)
        {
            // convert to OkLab if we need perceptual color space.
            float3 color = lerp(Gradient.colors[0].rgb, LinearToOklab(Gradient.colors[0].rgb), Gradient.type == 2);
        
            [unroll]
            for (int c = 1; c < Gradient.colorsLength; c++)
            {
                float colorPos = saturate((Time - Gradient.colors[c - 1].w) / (Gradient.colors[c].w - Gradient.colors[c - 1].w)) * step(c, Gradient.colorsLength - 1);
                float3 color2 = lerp(Gradient.colors[c].rgb, LinearToOklab(Gradient.colors[c].rgb), Gradient.type == 2);
                color = lerp(color, color2, lerp(colorPos, step(0.01, colorPos), Gradient.type % 2)); // grad.type == 1 is fixed, 0 and 2 are blends.
            }
            color = lerp(color, OklabToLinear(color), Gradient.type == 2);
        
        #ifdef UNITY_COLORSPACE_GAMMA
            color = LinearToSRGB(color);
        #endif
        
            float alpha = Gradient.alphas[0].x;
            [unroll]
            for (int a = 1; a < Gradient.alphasLength; a++)
            {
                float alphaPos = saturate((Time - Gradient.alphas[a - 1].y) / (Gradient.alphas[a].y - Gradient.alphas[a - 1].y)) * step(a, Gradient.alphasLength - 1);
                alpha = lerp(alpha, Gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), Gradient.type % 2));
            }
        
            Out = float4(color, alpha);
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
        Out = A * B;
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        struct Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float
        {
        };
        
        void SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(float3 Vector3_7940555B, float Vector1_1B8B9078, Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float IN, out float Value_0)
        {
        float3 _Property_44999cc87708de82a26b39ae1da975ec_Out_0_Vector3 = Vector3_7940555B;
        float _Property_dad5add45a7fa785be976f925bc5a5da_Out_0_Float = Vector1_1B8B9078;
        float3 _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3;
        Unity_Multiply_float3_float3(_Property_44999cc87708de82a26b39ae1da975ec_Out_0_Vector3, (_Property_dad5add45a7fa785be976f925bc5a5da_Out_0_Float.xxx), _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3);
        float _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float;
        PerlinNoise3D_float(_Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3, _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float);
        float _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float;
        Unity_Remap_float(_PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float, float2 (-1.15, 1.15), float2 (0, 1), _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float);
        Value_0 = _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_RandomRange_float(float2 Seed, float Min, float Max, out float Out)
        {
             float randomno =  frac(sin(dot(Seed, float2(12.9898, 78.233)))*43758.5453);
             Out = lerp(Min, Max, randomno);
        }
        
        struct Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float
        {
        };
        
        void SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(float3 Vector3_375394F7, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float IN, out float Value_1, out float Cells_2)
        {
        float3 _Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3 = Vector3_375394F7;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        VoronoiPrecise3D_float(_Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }
        
        float3 Unity_HDRP_GetEmissionHDRColor_float(float3 ldrColor, float luminanceIntensity, float exposureWeight)
        {
            float3 hdrColor = ldrColor * luminanceIntensity;
        
            #ifdef SHADERGRAPH_PREVIEW
            float inverseExposureMultiplier = 1.0;
            #else
            float inverseExposureMultiplier = GetInverseCurrentExposureMultiplier();
            #endif
        
            // Inverse pre-expose using _EmissiveExposureWeight weight
            hdrColor = lerp(hdrColor * inverseExposureMultiplier, hdrColor, exposureWeight);
            return hdrColor;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float3 Emission;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_d09e7f2c2a774dbd93cc943fa6a50940_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_VoidColor) : _VoidColor;
            float _Property_16eef79ff58443149895b35d0a88bdb2_Out_0_Float = _InnerEdge;
            float4 _UV_ce55b36e72754a339073e4c05a64b551_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_054563945a0a4f688fce11f0c067b0c8_Out_1_Vector2 = _UV_ce55b36e72754a339073e4c05a64b551_Out_0_Vector4.xy;
            float _Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float;
            Unity_Distance_float2(_Swizzle_054563945a0a4f688fce11f0c067b0c8_Out_1_Vector2, float2(0.5, 0.5), _Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float);
            float _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float;
            Unity_OneMinus_float(_Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float, _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float);
            float _Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float;
            Unity_Smoothstep_float(_Property_16eef79ff58443149895b35d0a88bdb2_Out_0_Float, float(1), _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float, _Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float);
            Gradient _Gradient_c34cf660310448768ec81f4600b23ad8_Out_0_Gradient = NewGradient(0, 4, 2, float4(0, 0, 0, 0.271931),float4(0.01568037, 0.01568037, 0.01568037, 0.4678416),float4(0.1071531, 0.1071531, 0.1071531, 0.6052644),float4(1, 1, 1, 1),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0), float2(1, 0),float2(1, 1),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0));
            float _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float;
            Unity_DotProduct_float3(IN.ObjectSpaceViewDirection, IN.ObjectSpaceNormal, _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float);
            float4 _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4;
            Unity_SampleGradientV1_float(_Gradient_c34cf660310448768ec81f4600b23ad8_Out_0_Gradient, _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float, _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4);
            float4 _Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float.xxxx), _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4, _Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4);
            Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a;
            float _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float;
            SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(IN.ObjectSpaceViewDirection, float(10), _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a, _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float);
            float3 _Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3;
            Unity_Multiply_float3_float3(IN.ObjectSpaceViewDirection, (_Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float.xxx), _Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3);
            float _Property_ba2d1376c19c40fbadc820d0fb721b2e_Out_0_Float = _Speed;
            float _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_ba2d1376c19c40fbadc820d0fb721b2e_Out_0_Float, _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float);
            float2 _Property_2cfd9c61936548ee85383d83df6beba0_Out_0_Vector2 = _Seed;
            float _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float;
            Unity_RandomRange_float(_Property_2cfd9c61936548ee85383d83df6beba0_Out_0_Vector2, float(26.96), float(45.6), _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float);
            Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a;
            float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Value_1_Float;
            float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float;
            SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(_Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3, _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float, _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Value_1_Float, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float);
            float4 _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4, (_Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float.xxxx), _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4);
            float4 _Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_d09e7f2c2a774dbd93cc943fa6a50940_Out_0_Vector4, _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4, _Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4);
            float4 _Property_e0f9565d1eaa452fb638faa34cc517eb_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_BorderColor) : _BorderColor;
            float3 _Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3;
            Unity_Absolute_float3(IN.AbsoluteWorldSpacePosition, _Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3);
            Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1;
            float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Value_1_Float;
            float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float;
            SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(_Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3, IN.TimeParameters.x, float(6.2), _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1, _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Value_1_Float, _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float);
            float _Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float;
            Unity_Smoothstep_float(float(-0.41), float(1), _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float, _Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float);
            float _Property_82bfb359e6494ff08a54e8c08976a6f1_Out_0_Float = _FresnelPower;
            float _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_82bfb359e6494ff08a54e8c08976a6f1_Out_0_Float, _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float);
            float _Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float, _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float, _Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float);
            float4 _Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_e0f9565d1eaa452fb638faa34cc517eb_Out_0_Vector4, (_Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float.xxxx), _Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4);
            float3 _EmissionNode_87074dafd5ff4d39bf4005b1089a56e4_Output_0_Vector3 = Unity_HDRP_GetEmissionHDRColor_float((_Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4.xyz).xyz, float(3), float(0));
            float4 _Add_0b088d988049415c84334d987e692959_Out_2_Vector4;
            Unity_Add_float4(_Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4, (_Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float.xxxx), _Add_0b088d988049415c84334d987e692959_Out_2_Vector4);
            surface.BaseColor = (_Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4.xyz);
            surface.Emission = _EmissionNode_87074dafd5ff4d39bf4005b1089a56e4_Output_0_Vector3;
            surface.Alpha = (_Add_0b088d988049415c84334d987e692959_Out_2_Vector4).x;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES AttributesMesh
            #define VaryingsMeshType VaryingsMeshToPS
            #define VFX_SRP_VARYINGS VaryingsMeshType
            #define VFX_SRP_SURFACE_INPUTS FragInputs
            #endif
            
        VertexDescriptionInputs AttributesMeshToVertexDescriptionInputs(AttributesMesh input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else
        #endif
        
            return output;
        }
        
        VertexDescription GetVertexDescription(AttributesMesh input, float3 timeParameters
        #ifdef HAVE_VFX_MODIFICATION
            , AttributesElement element
        #endif
        )
        {
            // build graph inputs
            VertexDescriptionInputs vertexDescriptionInputs = AttributesMeshToVertexDescriptionInputs(input);
            // Override time parameters with used one (This is required to correctly handle motion vectors for vertex animation based on time)
        
            // evaluate vertex graph
        #ifdef HAVE_VFX_MODIFICATION
            GraphProperties properties;
            ZERO_INITIALIZE(GraphProperties, properties);
        
            // Fetch the vertex graph properties for the particle instance.
            GetElementVertexProperties(element, properties);
        
            VertexDescription vertexDescription = VertexDescriptionFunction(vertexDescriptionInputs, properties);
        #else
            VertexDescription vertexDescription = VertexDescriptionFunction(vertexDescriptionInputs);
        #endif
            return vertexDescription;
        
        }
        
        AttributesMesh ApplyMeshModification(AttributesMesh input, float3 timeParameters
        #ifdef USE_CUSTOMINTERP_SUBSTRUCT
            #ifdef TESSELLATION_ON
            , inout VaryingsMeshToDS varyings
            #else
            , inout VaryingsMeshToPS varyings
            #endif
        #endif
        #ifdef HAVE_VFX_MODIFICATION
                , AttributesElement element
        #endif
            )
        {
            VertexDescription vertexDescription = GetVertexDescription(input, timeParameters
        #ifdef HAVE_VFX_MODIFICATION
                , element
        #endif
            );
        
            // copy graph output to the results
            input.positionOS = vertexDescription.Position;
            input.normalOS = vertexDescription.Normal;
            input.tangentOS.xyz = vertexDescription.Tangent;
        
            
        
            return input;
        }
        
        #if defined(_ADD_CUSTOM_VELOCITY) // For shader graph custom velocity
        // Return precomputed Velocity in object space
        float3 GetCustomVelocity(AttributesMesh input
        #ifdef HAVE_VFX_MODIFICATION
            , AttributesElement element
        #endif
        )
        {
            VertexDescription vertexDescription = GetVertexDescription(input, _TimeParameters.xyz
        #ifdef HAVE_VFX_MODIFICATION
                , element
        #endif
            );
            return vertexDescription.CustomVelocity;
        }
        #endif
        
        FragInputs BuildFragInputs(VaryingsMeshToPS input)
        {
            FragInputs output;
            ZERO_INITIALIZE(FragInputs, output);
        
            // Init to some default value to make the computer quiet (else it output 'divide by zero' warning even if value is not used).
            // TODO: this is a really poor workaround, but the variable is used in a bunch of places
            // to compute normals which are then passed on elsewhere to compute other values...
            output.tangentToWorld = k_identity3x3;
            output.positionSS = input.positionCS;       // input.positionCS is SV_Position
        
            output.positionRWS =                input.positionRWS;
            output.tangentToWorld =             BuildTangentToWorld(input.tangentWS, input.normalWS);
            output.texCoord0 =                  input.texCoord0;
        
        #if UNITY_ANY_INSTANCING_ENABLED
        #else
        #endif
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
        
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            // splice point to copy custom interpolator fields from varyings to frag inputs
            
        
            return output;
        }
        
        // existing HDRP code uses the combined function to go directly from packed to frag inputs
        FragInputs UnpackVaryingsMeshToFragInputs(PackedVaryingsMeshToPS input)
        {
            UNITY_SETUP_INSTANCE_ID(input);
        #if defined(HAVE_VFX_MODIFICATION) && defined(UNITY_INSTANCING_ENABLED)
            unity_InstanceID = input.instanceID;
        #endif
            VaryingsMeshToPS unpacked = UnpackVaryingsMeshToPS(input);
            return BuildFragInputs(unpacked);
        }
            SurfaceDescriptionInputs FragInputsToSurfaceDescriptionInputs(FragInputs input, float3 viewWS)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            output.WorldSpaceNormal =                           normalize(input.tangentToWorld[2].xyz);
            #if defined(SHADER_STAGE_RAY_TRACING)
            output.ObjectSpaceNormal =                          mul(output.WorldSpaceNormal, (float3x3) ObjectToWorld3x4());
            #else
            output.ObjectSpaceNormal =                          normalize(mul(output.WorldSpaceNormal, (float3x3) UNITY_MATRIX_M));           // transposed multiplication by inverse matrix to handle normal scale
            #endif
            output.WorldSpaceViewDirection =                    normalize(viewWS);
            output.ObjectSpaceViewDirection =                   TransformWorldToObjectDir(output.WorldSpaceViewDirection);
        #if SHADERPASS != SHADERPASS_FOG_VOLUME_VOXELIZATION
        #else
        #endif
            output.AbsoluteWorldSpacePosition =                 GetAbsolutePositionWS(input.positionRWS);
        #if SHADERPASS != SHADERPASS_FOG_VOLUME_VOXELIZATION
        #else
        #endif
        
        #if UNITY_UV_STARTS_AT_TOP
        #else
        #endif
        
        
            output.uv0 =                                        input.texCoord0;
            output.TimeParameters =                             _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        
            // splice point to copy frag inputs custom interpolator pack into the SDI
            
        
            return output;
        }
        
            // --------------------------------------------------
            // Build Surface Data (Specific Material)
        
        void ApplyDecalToSurfaceDataNoNormal(DecalSurfaceData decalSurfaceData, inout SurfaceData surfaceData);
        
        void ApplyDecalAndGetNormal(FragInputs fragInputs, PositionInputs posInput, SurfaceDescription surfaceDescription,
            inout SurfaceData surfaceData)
        {
            float3 doubleSidedConstants = GetDoubleSidedConstants();
        
        #ifdef DECAL_NORMAL_BLENDING
            // SG nodes don't ouptut surface gradients, so if decals require surf grad blending, we have to convert
            // the normal to gradient before applying the decal. We then have to resolve the gradient back to world space
            float3 normalTS;
        
        
        
            #if HAVE_DECALS
            if (_EnableDecals)
            {
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs, alpha);
                ApplyDecalToSurfaceNormal(decalSurfaceData, fragInputs.tangentToWorld[2], normalTS);
                ApplyDecalToSurfaceDataNoNormal(decalSurfaceData, surfaceData);
            }
            #endif
        
            GetNormalWS_SG(fragInputs, normalTS, surfaceData.normalWS, doubleSidedConstants);
        #else
            // normal delivered to master node
        
            #if HAVE_DECALS
            if (_EnableDecals)
            {
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                // Both uses and modifies 'surfaceData.normalWS'.
                DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs, alpha);
                ApplyDecalToSurfaceNormal(decalSurfaceData, surfaceData.normalWS.xyz);
                ApplyDecalToSurfaceDataNoNormal(decalSurfaceData, surfaceData);
            }
            #endif
        #endif
        }
        void BuildSurfaceData(FragInputs fragInputs, inout SurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData)
        {
            // setup defaults -- these are used if the graph doesn't output a value
            ZERO_INITIALIZE(SurfaceData, surfaceData);
        
            // copy across graph values, if defined
            surfaceData.color = surfaceDescription.BaseColor;
        
            #ifdef WRITE_NORMAL_BUFFER
            // When we need to export the normal (in the depth prepass, we write the geometry one)
            surfaceData.normalWS = fragInputs.tangentToWorld[2];
            #endif
        
            #if defined(DEBUG_DISPLAY)
            #if !defined(SHADER_STAGE_RAY_TRACING)
            // Mipmap mode debugging isn't supported with ray tracing as it relies on derivatives
                if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                {
                    #ifdef FRAG_INPUTS_USE_TEXCOORD0
                        surfaceData.color = GET_TEXTURE_STREAMING_DEBUG(posInput.positionSS, fragInputs.texCoord0);
                    #else
                        surfaceData.color = GET_TEXTURE_STREAMING_DEBUG_NO_UV(posInput.positionSS);
                    #endif
                }
            #endif
            #endif
        
            #ifdef _ENABLE_SHADOW_MATTE
        
                #if (SHADERPASS == SHADERPASS_FORWARD_UNLIT) || (SHADERPASS == SHADERPASS_RAYTRACING_GBUFFER) || (SHADERPASS == SHADERPASS_RAYTRACING_INDIRECT) || (SHADERPASS == SHADERPASS_RAYTRACING_FORWARD)
        
                    HDShadowContext shadowContext = InitShadowContext();
        
                    // Evaluate the shadow, the normal is guaranteed if shadow matte is enabled on this shader.
                    float3 shadow3;
                    ShadowLoopMin(shadowContext, posInput, normalize(fragInputs.tangentToWorld[2]), asuint(_ShadowMatteFilter), GetMeshRenderingLayerMask(), shadow3);
        
                    // Compute the average value in the fourth channel
                    float4 shadow = float4(shadow3, dot(shadow3, float3(1.0/3.0, 1.0/3.0, 1.0/3.0)));
        
                    float4 shadowColor = (1.0 - shadow) * surfaceDescription.ShadowTint.rgba;
                    float  localAlpha  = saturate(shadowColor.a + surfaceDescription.Alpha);
        
                    // Keep the nested lerp
                    // With no Color (bsdfData.color.rgb, bsdfData.color.a == 0.0f), just use ShadowColor*Color to avoid a ring of "white" around the shadow
                    // And mix color to consider the Color & ShadowColor alpha (from texture or/and color picker)
                    #ifdef _SURFACE_TYPE_TRANSPARENT
                        surfaceData.color = lerp(shadowColor.rgb * surfaceData.color, lerp(lerp(shadowColor.rgb, surfaceData.color, 1.0 - surfaceDescription.ShadowTint.a), surfaceData.color, shadow.rgb), surfaceDescription.Alpha);
                    #else
                        surfaceData.color = lerp(lerp(shadowColor.rgb, surfaceData.color, 1.0 - surfaceDescription.ShadowTint.a), surfaceData.color, shadow.rgb);
                    #endif
                    localAlpha = ApplyBlendMode(surfaceData.color, localAlpha).a;
        
                    surfaceDescription.Alpha = localAlpha;
        
                #elif SHADERPASS == SHADERPASS_PATH_TRACING
        
                    surfaceData.normalWS = fragInputs.tangentToWorld[2];
                    surfaceData.shadowTint = surfaceDescription.ShadowTint.rgba;
        
                #endif
        
            #endif // _ENABLE_SHADOW_MATTE
        }
        
            // --------------------------------------------------
            // Get Surface And BuiltinData
        
            void GetSurfaceAndBuiltinData(FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData RAY_TRACING_OPTIONAL_PARAMETERS)
            {
                // Don't dither if displaced tessellation (we're fading out the displacement instead to match the next LOD)
                #if !defined(SHADER_STAGE_RAY_TRACING) && !defined(_TESSELLATION_DISPLACEMENT)
                #ifdef LOD_FADE_CROSSFADE // enable dithering LOD transition if user select CrossFade transition in LOD group
                LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                #endif
                #endif
        
                #ifndef SHADER_UNLIT
                #ifdef _DOUBLESIDED_ON
                    float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
                #else
                    float3 doubleSidedConstants = float3(1.0, 1.0, 1.0);
                #endif
        
                ApplyDoubleSidedFlipOrMirror(fragInputs, doubleSidedConstants); // Apply double sided flip on the vertex normal
                #endif // SHADER_UNLIT
        
                SurfaceDescriptionInputs surfaceDescriptionInputs = FragInputsToSurfaceDescriptionInputs(fragInputs, V);
        
                #if defined(HAVE_VFX_MODIFICATION)
                GraphProperties properties;
                ZERO_INITIALIZE(GraphProperties, properties);
        
                GetElementPixelProperties(fragInputs, properties);
        
                SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs, properties);
                #else
                SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);
                #endif
        
                #ifdef DEBUG_DISPLAY
                if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                {
                    surfaceDescription.Alpha = 1.0f;
                }
                #endif
        
                // Perform alpha test very early to save performance (a killed pixel will not sample textures)
                // TODO: split graph evaluation to grab just alpha dependencies first? tricky..
                #ifdef _ALPHATEST_ON
                    float alphaCutoff = surfaceDescription.AlphaClipThreshold;
                    #if SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_PREPASS
                    // The TransparentDepthPrepass is also used with SSR transparent.
                    // If an artists enable transaprent SSR but not the TransparentDepthPrepass itself, then we use AlphaClipThreshold
                    // otherwise if TransparentDepthPrepass is enabled we use AlphaClipThresholdDepthPrepass
                    #elif SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_POSTPASS
                    // DepthPostpass always use its own alpha threshold
                    alphaCutoff = surfaceDescription.AlphaClipThresholdDepthPostpass;
                    #elif (SHADERPASS == SHADERPASS_SHADOWS) || (SHADERPASS == SHADERPASS_RAYTRACING_VISIBILITY)
                    // If use shadow threshold isn't enable we don't allow any test
                    #endif
        
                    GENERIC_ALPHA_TEST(surfaceDescription.Alpha, alphaCutoff);
                #endif
        
                #if !defined(SHADER_STAGE_RAY_TRACING) && _DEPTHOFFSET_ON
                ApplyDepthOffsetPositionInput(V, surfaceDescription.DepthOffset, GetViewForwardDir(), GetWorldToHClipMatrix(), posInput);
                #endif
        
                #ifndef SHADER_UNLIT
                float3 bentNormalWS;
                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData, bentNormalWS);
        
                #ifdef FRAG_INPUTS_USE_TEXCOORD1
                    float4 lightmapTexCoord1 = fragInputs.texCoord1;
                #else
                    float4 lightmapTexCoord1 = float4(0,0,0,0);
                #endif
        
                #ifdef FRAG_INPUTS_USE_TEXCOORD2
                    float4 lightmapTexCoord2 = fragInputs.texCoord2;
                #else
                    float4 lightmapTexCoord2 = float4(0,0,0,0);
                #endif
        
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                // Builtin Data
                // For back lighting we use the oposite vertex normal
                InitBuiltinData(posInput, alpha, bentNormalWS, -fragInputs.tangentToWorld[2], lightmapTexCoord1, lightmapTexCoord2, builtinData);
        
                #else
                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData);
        
                ZERO_BUILTIN_INITIALIZE(builtinData); // No call to InitBuiltinData as we don't have any lighting
                builtinData.opacity = surfaceDescription.Alpha;
        
                #if defined(DEBUG_DISPLAY)
                    // Light Layers are currently not used for the Unlit shader (because it is not lit)
                    // But Unlit objects do cast shadows according to their rendering layer mask, which is what we want to
                    // display in the light layers visualization mode, therefore we need the renderingLayers
                    builtinData.renderingLayers = GetMeshRenderingLayerMask();
                #endif
        
                #endif // SHADER_UNLIT
        
                #ifdef _ALPHATEST_ON
                    // Used for sharpening by alpha to mask - Alpha to covertage is only used with depth only and forward pass (no shadow pass, no transparent pass)
                    builtinData.alphaClipTreshold = alphaCutoff;
                #endif
        
                // override sampleBakedGI - not used by Unlit
        		// When overriding GI, we need to force the isLightmap flag to make sure we don't add APV (sampled in the lightloop) on top of the overridden value (set at GBuffer stage)
        
                builtinData.emissiveColor = surfaceDescription.Emission;
        
                // Note this will not fully work on transparent surfaces (can check with _SURFACE_TYPE_TRANSPARENT define)
                // We will always overwrite vt feeback with the nearest. So behind transparent surfaces vt will not be resolved
                // This is a limitation of the current MRT approach.
                #ifdef UNITY_VIRTUAL_TEXTURING
                #endif
        
                #if _DEPTHOFFSET_ON
                builtinData.depthOffset = surfaceDescription.DepthOffset;
                #endif
        
                // TODO: We should generate distortion / distortionBlur for non distortion pass
                #if (SHADERPASS == SHADERPASS_DISTORTION)
                builtinData.distortion = surfaceDescription.Distortion;
                builtinData.distortionBlur = surfaceDescription.DistortionBlur;
                #endif
        
                #ifndef SHADER_UNLIT
                // PostInitBuiltinData call ApplyDebugToBuiltinData
                PostInitBuiltinData(V, posInput, surfaceData, builtinData);
                #else
                ApplyDebugToBuiltinData(builtinData);
                #endif
        
                RAY_TRACING_OPTIONAL_ALPHA_TEST_PASS
            }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassDepthOnly.hlsl"
        
            // --------------------------------------------------
            // Visual Effect Vertex Invocations
        
        	#ifdef HAVE_VFX_MODIFICATION
                #if !defined(SHADER_STAGE_RAY_TRACING)
        	    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/VisualEffectVertex.hlsl"
                #else
                #endif
        	#endif
        
            ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }
        
            // Render State
            Cull Off
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma editor_sync_compilation
        #pragma instancing_options renderinglayer
        #pragma target 4.5
        #pragma vertex Vert
        #pragma fragment Frag
        #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch
        #pragma multi_compile_instancing
        
            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>
        
            // Defines
            #define SHADERPASS SHADERPASS_DEPTH_ONLY
        #define RAYTRACING_SHADER_GRAPH_DEFAULT
        #define SCENESELECTIONPASS 1
        #define SUPPORT_GLOBAL_MIP_BIAS 1
        
            // For custom interpolators to inject a substruct definition before FragInputs definition,
            // allowing for FragInputs to capture CI's intended for ShaderGraph's SDI.
            struct CustomInterpolators
        {
        };
        #define USE_CUSTOMINTERP_SUBSTRUCT
        
        
        
            // TODO: Merge FragInputsVFX substruct with CustomInterpolators.
        	#ifdef HAVE_VFX_MODIFICATION
        	struct FragInputsVFX
            {
                /* WARNING: $splice Could not find named fragment 'FragInputsVFX' */
            };
            #endif
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl" // Required by Tessellation.hlsl
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl" // Required to be include before we include properties as it define DECLARE_STACK_CB
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl" // Required before including properties as it defines UNITY_TEXTURE_STREAMING_DEBUG_VARS
            // Always include Shader Graph version
            // Always include last to avoid double macros
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl" // Need to be here for Gradient struct definition
        
            // --------------------------------------------------
            // Defines
        
            // Attribute
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TANGENT_TO_WORLD
            #define VARYINGS_NEED_TEXCOORD0
        
            #define HAVE_MESH_MODIFICATION
        
            //Strip down the FragInputs.hlsl (on graphics), so we can only optimize the interpolators we use.
            //if by accident something requests contents of FragInputs.hlsl, it will be caught as a compiler error
            //Frag inputs stripping is only enabled when FRAG_INPUTS_ENABLE_STRIPPING is set
            #if !defined(SHADER_STAGE_RAY_TRACING) && SHADERPASS != SHADERPASS_RAYTRACING_GBUFFER && SHADERPASS != SHADERPASS_FULL_SCREEN_DEBUG
            #define FRAG_INPUTS_ENABLE_STRIPPING
            #endif
            #define FRAG_INPUTS_USE_TEXCOORD0
        
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
        
        
        
        
            // Following two define are a workaround introduce in 10.1.x for RaytracingQualityNode
            // The ShaderGraph don't support correctly migration of this node as it serialize all the node data
            // in the json file making it impossible to uprgrade. Until we get a fix, we do a workaround here
            // to still allow us to rename the field and keyword of this node without breaking existing code.
            #ifdef RAYTRACING_SHADER_GRAPH_DEFAULT
            #define RAYTRACING_SHADER_GRAPH_HIGH
            #endif
        
            #ifdef RAYTRACING_SHADER_GRAPH_RAYTRACED
            #define RAYTRACING_SHADER_GRAPH_LOW
            #endif
            // end
        
            #ifndef SHADER_UNLIT
            // We need isFrontFace when using double sided - it is not required for unlit as in case of unlit double sided only drive the cullmode
            // VARYINGS_NEED_CULLFACE can be define by VaryingsMeshToPS.FaceSign input if a IsFrontFace Node is included in the shader graph.
            #if defined(_DOUBLESIDED_ON) && !defined(VARYINGS_NEED_CULLFACE)
                #define VARYINGS_NEED_CULLFACE
            #endif
            #endif
        
            // Specific Material Define
        // Setup a define to say we are an unlit shader
        #define SHADER_UNLIT
        
        // Following Macro are only used by Unlit material
        #if defined(_ENABLE_SHADOW_MATTE)
            #if SHADERPASS == SHADERPASS_FORWARD_UNLIT
                #pragma multi_compile_fragment USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST
            #elif SHADERPASS == SHADERPASS_PATH_TRACING
                #define LIGHTLOOP_DISABLE_TILE_AND_CLUSTER
            #endif
        
        // We don't want to have the lightloop defined for the ray tracing passes, but we do for the rasterisation and path tracing shader passes.
        #if !defined(SHADER_STAGE_RAY_TRACING) || SHADERPASS == SHADERPASS_PATH_TRACING
            #define HAS_LIGHTLOOP
        #endif
        #endif
            // Caution: we can use the define SHADER_UNLIT onlit after the above Material include as it is the Unlit template who define it
        
            // To handle SSR on transparent correctly with a possibility to enable/disable it per framesettings
            // we should have a code like this:
            // if !defined(_DISABLE_SSR_TRANSPARENT)
            // pragma multi_compile _ WRITE_NORMAL_BUFFER
            // endif
            // i.e we enable the multicompile only if we can receive SSR or not, and then C# code drive
            // it based on if SSR transparent in frame settings and not (and stripper can strip it).
            // this is currently not possible with our current preprocessor as _DISABLE_SSR_TRANSPARENT is a keyword not a define
            // so instead we used this and chose to pay the extra cost of normal write even if SSR transaprent is disabled.
            // Ideally the shader graph generator should handle it but condition below can't be handle correctly for now.
            #if SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_PREPASS
            #if !defined(_DISABLE_SSR_TRANSPARENT) && !defined(SHADER_UNLIT)
                #define WRITE_NORMAL_BUFFER
            #endif
            #endif
        
            // See Lit.shader
            #if SHADERPASS == SHADERPASS_MOTION_VECTORS && defined(WRITE_DECAL_BUFFER_AND_RENDERING_LAYER)
                #define WRITE_DECAL_BUFFER
            #endif
        
            #ifndef DEBUG_DISPLAY
                // In case of opaque we don't want to perform the alpha test, it is done in depth prepass and we use depth equal for ztest (setup from UI)
                // Don't do it with debug display mode as it is possible there is no depth prepass in this case
                #if !defined(_SURFACE_TYPE_TRANSPARENT)
                    #if SHADERPASS == SHADERPASS_FORWARD
                    #define SHADERPASS_FORWARD_BYPASS_ALPHA_TEST
                    #elif SHADERPASS == SHADERPASS_GBUFFER
                    #define SHADERPASS_GBUFFER_BYPASS_ALPHA_TEST
                    #endif
                #endif
            #endif
        
            // Define _DEFERRED_CAPABLE_MATERIAL for shader capable to run in deferred pass
            #if defined(SHADER_LIT) && !defined(_SURFACE_TYPE_TRANSPARENT)
                #define _DEFERRED_CAPABLE_MATERIAL
            #endif
        
            // Translate transparent motion vector define
            #if (defined(_TRANSPARENT_WRITES_MOTION_VEC) || defined(_TRANSPARENT_REFRACTIVE_SORT)) && defined(_SURFACE_TYPE_TRANSPARENT)
                #define _WRITE_TRANSPARENT_MOTION_VECTOR
            #endif
        
            // -- Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _VoidColor;
        float _Speed;
        float _FresnelPower;
        float4 _BorderColor;
        float2 _Seed;
        float4 _TwirlTexture_TexelSize;
        float _InnerEdge;
        float4 _EmissionColor;
        float _UseShadowThreshold;
        float4 _DoubleSidedConstants;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        float _BlendMode;
        float _EnableBlendModePreserveSpecularLighting;
        CBUFFER_END
        
        
        // Object and Global properties
        TEXTURE2D(_TwirlTexture);
        SAMPLER(sampler_TwirlTexture);
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Includes
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/PickingSpaceTransforms.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Debug/DebugDisplay.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/Unlit.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/ClassicNoise3D.hlsl"
        #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi3D.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonLighting.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            struct AttributesMesh
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct VaryingsMeshToPS
        {
            SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
             float3 positionRWS;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct SurfaceDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 WorldSpaceNormal;
             float3 ObjectSpaceViewDirection;
             float3 WorldSpaceViewDirection;
             float3 AbsoluteWorldSpacePosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct PackedVaryingsMeshToPS
        {
            SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
             float4 tangentWS : INTERP0;
             float4 texCoord0 : INTERP1;
             float3 positionRWS : INTERP2;
             float3 normalWS : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
        };
        
            PackedVaryingsMeshToPS PackVaryingsMeshToPS (VaryingsMeshToPS input)
        {
            PackedVaryingsMeshToPS output;
            ZERO_INITIALIZE(PackedVaryingsMeshToPS, output);
            output.positionCS = input.positionCS;
            output.tangentWS.xyzw = input.tangentWS;
            output.texCoord0.xyzw = input.texCoord0;
            output.positionRWS.xyz = input.positionRWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            return output;
        }
        
        VaryingsMeshToPS UnpackVaryingsMeshToPS (PackedVaryingsMeshToPS input)
        {
            VaryingsMeshToPS output;
            output.positionCS = input.positionCS;
            output.tangentWS = input.tangentWS.xyzw;
            output.texCoord0 = input.texCoord0.xyzw;
            output.positionRWS = input.positionRWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
        
            // Graph Functions
            
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_DotProduct_float3(float3 A, float3 B, out float Out)
        {
            Out = dot(A, B);
        }
        
        void Unity_SampleGradientV1_float(Gradient Gradient, float Time, out float4 Out)
        {
            // convert to OkLab if we need perceptual color space.
            float3 color = lerp(Gradient.colors[0].rgb, LinearToOklab(Gradient.colors[0].rgb), Gradient.type == 2);
        
            [unroll]
            for (int c = 1; c < Gradient.colorsLength; c++)
            {
                float colorPos = saturate((Time - Gradient.colors[c - 1].w) / (Gradient.colors[c].w - Gradient.colors[c - 1].w)) * step(c, Gradient.colorsLength - 1);
                float3 color2 = lerp(Gradient.colors[c].rgb, LinearToOklab(Gradient.colors[c].rgb), Gradient.type == 2);
                color = lerp(color, color2, lerp(colorPos, step(0.01, colorPos), Gradient.type % 2)); // grad.type == 1 is fixed, 0 and 2 are blends.
            }
            color = lerp(color, OklabToLinear(color), Gradient.type == 2);
        
        #ifdef UNITY_COLORSPACE_GAMMA
            color = LinearToSRGB(color);
        #endif
        
            float alpha = Gradient.alphas[0].x;
            [unroll]
            for (int a = 1; a < Gradient.alphasLength; a++)
            {
                float alphaPos = saturate((Time - Gradient.alphas[a - 1].y) / (Gradient.alphas[a].y - Gradient.alphas[a - 1].y)) * step(a, Gradient.alphasLength - 1);
                alpha = lerp(alpha, Gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), Gradient.type % 2));
            }
        
            Out = float4(color, alpha);
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
        Out = A * B;
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        struct Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float
        {
        };
        
        void SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(float3 Vector3_7940555B, float Vector1_1B8B9078, Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float IN, out float Value_0)
        {
        float3 _Property_44999cc87708de82a26b39ae1da975ec_Out_0_Vector3 = Vector3_7940555B;
        float _Property_dad5add45a7fa785be976f925bc5a5da_Out_0_Float = Vector1_1B8B9078;
        float3 _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3;
        Unity_Multiply_float3_float3(_Property_44999cc87708de82a26b39ae1da975ec_Out_0_Vector3, (_Property_dad5add45a7fa785be976f925bc5a5da_Out_0_Float.xxx), _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3);
        float _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float;
        PerlinNoise3D_float(_Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3, _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float);
        float _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float;
        Unity_Remap_float(_PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float, float2 (-1.15, 1.15), float2 (0, 1), _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float);
        Value_0 = _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_RandomRange_float(float2 Seed, float Min, float Max, out float Out)
        {
             float randomno =  frac(sin(dot(Seed, float2(12.9898, 78.233)))*43758.5453);
             Out = lerp(Min, Max, randomno);
        }
        
        struct Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float
        {
        };
        
        void SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(float3 Vector3_375394F7, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float IN, out float Value_1, out float Cells_2)
        {
        float3 _Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3 = Vector3_375394F7;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        VoronoiPrecise3D_float(_Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }
        
        float3 Unity_HDRP_GetEmissionHDRColor_float(float3 ldrColor, float luminanceIntensity, float exposureWeight)
        {
            float3 hdrColor = ldrColor * luminanceIntensity;
        
            #ifdef SHADERGRAPH_PREVIEW
            float inverseExposureMultiplier = 1.0;
            #else
            float inverseExposureMultiplier = GetInverseCurrentExposureMultiplier();
            #endif
        
            // Inverse pre-expose using _EmissiveExposureWeight weight
            hdrColor = lerp(hdrColor * inverseExposureMultiplier, hdrColor, exposureWeight);
            return hdrColor;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float3 Emission;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_d09e7f2c2a774dbd93cc943fa6a50940_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_VoidColor) : _VoidColor;
            float _Property_16eef79ff58443149895b35d0a88bdb2_Out_0_Float = _InnerEdge;
            float4 _UV_ce55b36e72754a339073e4c05a64b551_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_054563945a0a4f688fce11f0c067b0c8_Out_1_Vector2 = _UV_ce55b36e72754a339073e4c05a64b551_Out_0_Vector4.xy;
            float _Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float;
            Unity_Distance_float2(_Swizzle_054563945a0a4f688fce11f0c067b0c8_Out_1_Vector2, float2(0.5, 0.5), _Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float);
            float _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float;
            Unity_OneMinus_float(_Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float, _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float);
            float _Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float;
            Unity_Smoothstep_float(_Property_16eef79ff58443149895b35d0a88bdb2_Out_0_Float, float(1), _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float, _Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float);
            Gradient _Gradient_c34cf660310448768ec81f4600b23ad8_Out_0_Gradient = NewGradient(0, 4, 2, float4(0, 0, 0, 0.271931),float4(0.01568037, 0.01568037, 0.01568037, 0.4678416),float4(0.1071531, 0.1071531, 0.1071531, 0.6052644),float4(1, 1, 1, 1),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0), float2(1, 0),float2(1, 1),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0));
            float _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float;
            Unity_DotProduct_float3(IN.ObjectSpaceViewDirection, IN.ObjectSpaceNormal, _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float);
            float4 _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4;
            Unity_SampleGradientV1_float(_Gradient_c34cf660310448768ec81f4600b23ad8_Out_0_Gradient, _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float, _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4);
            float4 _Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float.xxxx), _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4, _Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4);
            Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a;
            float _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float;
            SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(IN.ObjectSpaceViewDirection, float(10), _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a, _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float);
            float3 _Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3;
            Unity_Multiply_float3_float3(IN.ObjectSpaceViewDirection, (_Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float.xxx), _Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3);
            float _Property_ba2d1376c19c40fbadc820d0fb721b2e_Out_0_Float = _Speed;
            float _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_ba2d1376c19c40fbadc820d0fb721b2e_Out_0_Float, _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float);
            float2 _Property_2cfd9c61936548ee85383d83df6beba0_Out_0_Vector2 = _Seed;
            float _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float;
            Unity_RandomRange_float(_Property_2cfd9c61936548ee85383d83df6beba0_Out_0_Vector2, float(26.96), float(45.6), _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float);
            Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a;
            float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Value_1_Float;
            float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float;
            SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(_Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3, _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float, _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Value_1_Float, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float);
            float4 _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4, (_Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float.xxxx), _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4);
            float4 _Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_d09e7f2c2a774dbd93cc943fa6a50940_Out_0_Vector4, _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4, _Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4);
            float4 _Property_e0f9565d1eaa452fb638faa34cc517eb_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_BorderColor) : _BorderColor;
            float3 _Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3;
            Unity_Absolute_float3(IN.AbsoluteWorldSpacePosition, _Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3);
            Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1;
            float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Value_1_Float;
            float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float;
            SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(_Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3, IN.TimeParameters.x, float(6.2), _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1, _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Value_1_Float, _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float);
            float _Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float;
            Unity_Smoothstep_float(float(-0.41), float(1), _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float, _Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float);
            float _Property_82bfb359e6494ff08a54e8c08976a6f1_Out_0_Float = _FresnelPower;
            float _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_82bfb359e6494ff08a54e8c08976a6f1_Out_0_Float, _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float);
            float _Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float, _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float, _Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float);
            float4 _Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_e0f9565d1eaa452fb638faa34cc517eb_Out_0_Vector4, (_Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float.xxxx), _Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4);
            float3 _EmissionNode_87074dafd5ff4d39bf4005b1089a56e4_Output_0_Vector3 = Unity_HDRP_GetEmissionHDRColor_float((_Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4.xyz).xyz, float(3), float(0));
            float4 _Add_0b088d988049415c84334d987e692959_Out_2_Vector4;
            Unity_Add_float4(_Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4, (_Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float.xxxx), _Add_0b088d988049415c84334d987e692959_Out_2_Vector4);
            surface.BaseColor = (_Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4.xyz);
            surface.Emission = _EmissionNode_87074dafd5ff4d39bf4005b1089a56e4_Output_0_Vector3;
            surface.Alpha = (_Add_0b088d988049415c84334d987e692959_Out_2_Vector4).x;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES AttributesMesh
            #define VaryingsMeshType VaryingsMeshToPS
            #define VFX_SRP_VARYINGS VaryingsMeshType
            #define VFX_SRP_SURFACE_INPUTS FragInputs
            #endif
            
        VertexDescriptionInputs AttributesMeshToVertexDescriptionInputs(AttributesMesh input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else
        #endif
        
            return output;
        }
        
        VertexDescription GetVertexDescription(AttributesMesh input, float3 timeParameters
        #ifdef HAVE_VFX_MODIFICATION
            , AttributesElement element
        #endif
        )
        {
            // build graph inputs
            VertexDescriptionInputs vertexDescriptionInputs = AttributesMeshToVertexDescriptionInputs(input);
            // Override time parameters with used one (This is required to correctly handle motion vectors for vertex animation based on time)
        
            // evaluate vertex graph
        #ifdef HAVE_VFX_MODIFICATION
            GraphProperties properties;
            ZERO_INITIALIZE(GraphProperties, properties);
        
            // Fetch the vertex graph properties for the particle instance.
            GetElementVertexProperties(element, properties);
        
            VertexDescription vertexDescription = VertexDescriptionFunction(vertexDescriptionInputs, properties);
        #else
            VertexDescription vertexDescription = VertexDescriptionFunction(vertexDescriptionInputs);
        #endif
            return vertexDescription;
        
        }
        
        AttributesMesh ApplyMeshModification(AttributesMesh input, float3 timeParameters
        #ifdef USE_CUSTOMINTERP_SUBSTRUCT
            #ifdef TESSELLATION_ON
            , inout VaryingsMeshToDS varyings
            #else
            , inout VaryingsMeshToPS varyings
            #endif
        #endif
        #ifdef HAVE_VFX_MODIFICATION
                , AttributesElement element
        #endif
            )
        {
            VertexDescription vertexDescription = GetVertexDescription(input, timeParameters
        #ifdef HAVE_VFX_MODIFICATION
                , element
        #endif
            );
        
            // copy graph output to the results
            input.positionOS = vertexDescription.Position;
            input.normalOS = vertexDescription.Normal;
            input.tangentOS.xyz = vertexDescription.Tangent;
        
            
        
            return input;
        }
        
        #if defined(_ADD_CUSTOM_VELOCITY) // For shader graph custom velocity
        // Return precomputed Velocity in object space
        float3 GetCustomVelocity(AttributesMesh input
        #ifdef HAVE_VFX_MODIFICATION
            , AttributesElement element
        #endif
        )
        {
            VertexDescription vertexDescription = GetVertexDescription(input, _TimeParameters.xyz
        #ifdef HAVE_VFX_MODIFICATION
                , element
        #endif
            );
            return vertexDescription.CustomVelocity;
        }
        #endif
        
        FragInputs BuildFragInputs(VaryingsMeshToPS input)
        {
            FragInputs output;
            ZERO_INITIALIZE(FragInputs, output);
        
            // Init to some default value to make the computer quiet (else it output 'divide by zero' warning even if value is not used).
            // TODO: this is a really poor workaround, but the variable is used in a bunch of places
            // to compute normals which are then passed on elsewhere to compute other values...
            output.tangentToWorld = k_identity3x3;
            output.positionSS = input.positionCS;       // input.positionCS is SV_Position
        
            output.positionRWS =                input.positionRWS;
            output.tangentToWorld =             BuildTangentToWorld(input.tangentWS, input.normalWS);
            output.texCoord0 =                  input.texCoord0;
        
        #if UNITY_ANY_INSTANCING_ENABLED
        #else
        #endif
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
        
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            // splice point to copy custom interpolator fields from varyings to frag inputs
            
        
            return output;
        }
        
        // existing HDRP code uses the combined function to go directly from packed to frag inputs
        FragInputs UnpackVaryingsMeshToFragInputs(PackedVaryingsMeshToPS input)
        {
            UNITY_SETUP_INSTANCE_ID(input);
        #if defined(HAVE_VFX_MODIFICATION) && defined(UNITY_INSTANCING_ENABLED)
            unity_InstanceID = input.instanceID;
        #endif
            VaryingsMeshToPS unpacked = UnpackVaryingsMeshToPS(input);
            return BuildFragInputs(unpacked);
        }
            SurfaceDescriptionInputs FragInputsToSurfaceDescriptionInputs(FragInputs input, float3 viewWS)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            output.WorldSpaceNormal =                           normalize(input.tangentToWorld[2].xyz);
            #if defined(SHADER_STAGE_RAY_TRACING)
            output.ObjectSpaceNormal =                          mul(output.WorldSpaceNormal, (float3x3) ObjectToWorld3x4());
            #else
            output.ObjectSpaceNormal =                          normalize(mul(output.WorldSpaceNormal, (float3x3) UNITY_MATRIX_M));           // transposed multiplication by inverse matrix to handle normal scale
            #endif
            output.WorldSpaceViewDirection =                    normalize(viewWS);
            output.ObjectSpaceViewDirection =                   TransformWorldToObjectDir(output.WorldSpaceViewDirection);
        #if SHADERPASS != SHADERPASS_FOG_VOLUME_VOXELIZATION
        #else
        #endif
            output.AbsoluteWorldSpacePosition =                 GetAbsolutePositionWS(input.positionRWS);
        #if SHADERPASS != SHADERPASS_FOG_VOLUME_VOXELIZATION
        #else
        #endif
        
        #if UNITY_UV_STARTS_AT_TOP
        #else
        #endif
        
        
            output.uv0 =                                        input.texCoord0;
            output.TimeParameters =                             _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        
            // splice point to copy frag inputs custom interpolator pack into the SDI
            
        
            return output;
        }
        
            // --------------------------------------------------
            // Build Surface Data (Specific Material)
        
        void ApplyDecalToSurfaceDataNoNormal(DecalSurfaceData decalSurfaceData, inout SurfaceData surfaceData);
        
        void ApplyDecalAndGetNormal(FragInputs fragInputs, PositionInputs posInput, SurfaceDescription surfaceDescription,
            inout SurfaceData surfaceData)
        {
            float3 doubleSidedConstants = GetDoubleSidedConstants();
        
        #ifdef DECAL_NORMAL_BLENDING
            // SG nodes don't ouptut surface gradients, so if decals require surf grad blending, we have to convert
            // the normal to gradient before applying the decal. We then have to resolve the gradient back to world space
            float3 normalTS;
        
        
        
            #if HAVE_DECALS
            if (_EnableDecals)
            {
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs, alpha);
                ApplyDecalToSurfaceNormal(decalSurfaceData, fragInputs.tangentToWorld[2], normalTS);
                ApplyDecalToSurfaceDataNoNormal(decalSurfaceData, surfaceData);
            }
            #endif
        
            GetNormalWS_SG(fragInputs, normalTS, surfaceData.normalWS, doubleSidedConstants);
        #else
            // normal delivered to master node
        
            #if HAVE_DECALS
            if (_EnableDecals)
            {
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                // Both uses and modifies 'surfaceData.normalWS'.
                DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs, alpha);
                ApplyDecalToSurfaceNormal(decalSurfaceData, surfaceData.normalWS.xyz);
                ApplyDecalToSurfaceDataNoNormal(decalSurfaceData, surfaceData);
            }
            #endif
        #endif
        }
        void BuildSurfaceData(FragInputs fragInputs, inout SurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData)
        {
            // setup defaults -- these are used if the graph doesn't output a value
            ZERO_INITIALIZE(SurfaceData, surfaceData);
        
            // copy across graph values, if defined
            surfaceData.color = surfaceDescription.BaseColor;
        
            #ifdef WRITE_NORMAL_BUFFER
            // When we need to export the normal (in the depth prepass, we write the geometry one)
            surfaceData.normalWS = fragInputs.tangentToWorld[2];
            #endif
        
            #if defined(DEBUG_DISPLAY)
            #if !defined(SHADER_STAGE_RAY_TRACING)
            // Mipmap mode debugging isn't supported with ray tracing as it relies on derivatives
                if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                {
                    #ifdef FRAG_INPUTS_USE_TEXCOORD0
                        surfaceData.color = GET_TEXTURE_STREAMING_DEBUG(posInput.positionSS, fragInputs.texCoord0);
                    #else
                        surfaceData.color = GET_TEXTURE_STREAMING_DEBUG_NO_UV(posInput.positionSS);
                    #endif
                }
            #endif
            #endif
        
            #ifdef _ENABLE_SHADOW_MATTE
        
                #if (SHADERPASS == SHADERPASS_FORWARD_UNLIT) || (SHADERPASS == SHADERPASS_RAYTRACING_GBUFFER) || (SHADERPASS == SHADERPASS_RAYTRACING_INDIRECT) || (SHADERPASS == SHADERPASS_RAYTRACING_FORWARD)
        
                    HDShadowContext shadowContext = InitShadowContext();
        
                    // Evaluate the shadow, the normal is guaranteed if shadow matte is enabled on this shader.
                    float3 shadow3;
                    ShadowLoopMin(shadowContext, posInput, normalize(fragInputs.tangentToWorld[2]), asuint(_ShadowMatteFilter), GetMeshRenderingLayerMask(), shadow3);
        
                    // Compute the average value in the fourth channel
                    float4 shadow = float4(shadow3, dot(shadow3, float3(1.0/3.0, 1.0/3.0, 1.0/3.0)));
        
                    float4 shadowColor = (1.0 - shadow) * surfaceDescription.ShadowTint.rgba;
                    float  localAlpha  = saturate(shadowColor.a + surfaceDescription.Alpha);
        
                    // Keep the nested lerp
                    // With no Color (bsdfData.color.rgb, bsdfData.color.a == 0.0f), just use ShadowColor*Color to avoid a ring of "white" around the shadow
                    // And mix color to consider the Color & ShadowColor alpha (from texture or/and color picker)
                    #ifdef _SURFACE_TYPE_TRANSPARENT
                        surfaceData.color = lerp(shadowColor.rgb * surfaceData.color, lerp(lerp(shadowColor.rgb, surfaceData.color, 1.0 - surfaceDescription.ShadowTint.a), surfaceData.color, shadow.rgb), surfaceDescription.Alpha);
                    #else
                        surfaceData.color = lerp(lerp(shadowColor.rgb, surfaceData.color, 1.0 - surfaceDescription.ShadowTint.a), surfaceData.color, shadow.rgb);
                    #endif
                    localAlpha = ApplyBlendMode(surfaceData.color, localAlpha).a;
        
                    surfaceDescription.Alpha = localAlpha;
        
                #elif SHADERPASS == SHADERPASS_PATH_TRACING
        
                    surfaceData.normalWS = fragInputs.tangentToWorld[2];
                    surfaceData.shadowTint = surfaceDescription.ShadowTint.rgba;
        
                #endif
        
            #endif // _ENABLE_SHADOW_MATTE
        }
        
            // --------------------------------------------------
            // Get Surface And BuiltinData
        
            void GetSurfaceAndBuiltinData(FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData RAY_TRACING_OPTIONAL_PARAMETERS)
            {
                // Don't dither if displaced tessellation (we're fading out the displacement instead to match the next LOD)
                #if !defined(SHADER_STAGE_RAY_TRACING) && !defined(_TESSELLATION_DISPLACEMENT)
                #ifdef LOD_FADE_CROSSFADE // enable dithering LOD transition if user select CrossFade transition in LOD group
                LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                #endif
                #endif
        
                #ifndef SHADER_UNLIT
                #ifdef _DOUBLESIDED_ON
                    float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
                #else
                    float3 doubleSidedConstants = float3(1.0, 1.0, 1.0);
                #endif
        
                ApplyDoubleSidedFlipOrMirror(fragInputs, doubleSidedConstants); // Apply double sided flip on the vertex normal
                #endif // SHADER_UNLIT
        
                SurfaceDescriptionInputs surfaceDescriptionInputs = FragInputsToSurfaceDescriptionInputs(fragInputs, V);
        
                #if defined(HAVE_VFX_MODIFICATION)
                GraphProperties properties;
                ZERO_INITIALIZE(GraphProperties, properties);
        
                GetElementPixelProperties(fragInputs, properties);
        
                SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs, properties);
                #else
                SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);
                #endif
        
                #ifdef DEBUG_DISPLAY
                if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                {
                    surfaceDescription.Alpha = 1.0f;
                }
                #endif
        
                // Perform alpha test very early to save performance (a killed pixel will not sample textures)
                // TODO: split graph evaluation to grab just alpha dependencies first? tricky..
                #ifdef _ALPHATEST_ON
                    float alphaCutoff = surfaceDescription.AlphaClipThreshold;
                    #if SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_PREPASS
                    // The TransparentDepthPrepass is also used with SSR transparent.
                    // If an artists enable transaprent SSR but not the TransparentDepthPrepass itself, then we use AlphaClipThreshold
                    // otherwise if TransparentDepthPrepass is enabled we use AlphaClipThresholdDepthPrepass
                    #elif SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_POSTPASS
                    // DepthPostpass always use its own alpha threshold
                    alphaCutoff = surfaceDescription.AlphaClipThresholdDepthPostpass;
                    #elif (SHADERPASS == SHADERPASS_SHADOWS) || (SHADERPASS == SHADERPASS_RAYTRACING_VISIBILITY)
                    // If use shadow threshold isn't enable we don't allow any test
                    #endif
        
                    GENERIC_ALPHA_TEST(surfaceDescription.Alpha, alphaCutoff);
                #endif
        
                #if !defined(SHADER_STAGE_RAY_TRACING) && _DEPTHOFFSET_ON
                ApplyDepthOffsetPositionInput(V, surfaceDescription.DepthOffset, GetViewForwardDir(), GetWorldToHClipMatrix(), posInput);
                #endif
        
                #ifndef SHADER_UNLIT
                float3 bentNormalWS;
                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData, bentNormalWS);
        
                #ifdef FRAG_INPUTS_USE_TEXCOORD1
                    float4 lightmapTexCoord1 = fragInputs.texCoord1;
                #else
                    float4 lightmapTexCoord1 = float4(0,0,0,0);
                #endif
        
                #ifdef FRAG_INPUTS_USE_TEXCOORD2
                    float4 lightmapTexCoord2 = fragInputs.texCoord2;
                #else
                    float4 lightmapTexCoord2 = float4(0,0,0,0);
                #endif
        
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                // Builtin Data
                // For back lighting we use the oposite vertex normal
                InitBuiltinData(posInput, alpha, bentNormalWS, -fragInputs.tangentToWorld[2], lightmapTexCoord1, lightmapTexCoord2, builtinData);
        
                #else
                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData);
        
                ZERO_BUILTIN_INITIALIZE(builtinData); // No call to InitBuiltinData as we don't have any lighting
                builtinData.opacity = surfaceDescription.Alpha;
        
                #if defined(DEBUG_DISPLAY)
                    // Light Layers are currently not used for the Unlit shader (because it is not lit)
                    // But Unlit objects do cast shadows according to their rendering layer mask, which is what we want to
                    // display in the light layers visualization mode, therefore we need the renderingLayers
                    builtinData.renderingLayers = GetMeshRenderingLayerMask();
                #endif
        
                #endif // SHADER_UNLIT
        
                #ifdef _ALPHATEST_ON
                    // Used for sharpening by alpha to mask - Alpha to covertage is only used with depth only and forward pass (no shadow pass, no transparent pass)
                    builtinData.alphaClipTreshold = alphaCutoff;
                #endif
        
                // override sampleBakedGI - not used by Unlit
        		// When overriding GI, we need to force the isLightmap flag to make sure we don't add APV (sampled in the lightloop) on top of the overridden value (set at GBuffer stage)
        
                builtinData.emissiveColor = surfaceDescription.Emission;
        
                // Note this will not fully work on transparent surfaces (can check with _SURFACE_TYPE_TRANSPARENT define)
                // We will always overwrite vt feeback with the nearest. So behind transparent surfaces vt will not be resolved
                // This is a limitation of the current MRT approach.
                #ifdef UNITY_VIRTUAL_TEXTURING
                #endif
        
                #if _DEPTHOFFSET_ON
                builtinData.depthOffset = surfaceDescription.DepthOffset;
                #endif
        
                // TODO: We should generate distortion / distortionBlur for non distortion pass
                #if (SHADERPASS == SHADERPASS_DISTORTION)
                builtinData.distortion = surfaceDescription.Distortion;
                builtinData.distortionBlur = surfaceDescription.DistortionBlur;
                #endif
        
                #ifndef SHADER_UNLIT
                // PostInitBuiltinData call ApplyDebugToBuiltinData
                PostInitBuiltinData(V, posInput, surfaceData, builtinData);
                #else
                ApplyDebugToBuiltinData(builtinData);
                #endif
        
                RAY_TRACING_OPTIONAL_ALPHA_TEST_PASS
            }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassDepthOnly.hlsl"
        
            // --------------------------------------------------
            // Visual Effect Vertex Invocations
        
        	#ifdef HAVE_VFX_MODIFICATION
                #if !defined(SHADER_STAGE_RAY_TRACING)
        	    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/VisualEffectVertex.hlsl"
                #else
                #endif
        	#endif
        
            ENDHLSL
        }
        Pass
        {
            Name "MotionVectors"
            Tags
            {
                "LightMode" = "MotionVectors"
            }
        
            // Render State
            Cull [_CullMode]
        ZWrite On
        Stencil
        {
        WriteMask [_StencilWriteMaskMV]
        Ref [_StencilRefMV]
        CompFront Always
        PassFront Replace
        CompBack Always
        PassBack Replace
        }
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma instancing_options renderinglayer
        #pragma target 4.5
        #pragma vertex Vert
        #pragma fragment Frag
        #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch
        #pragma multi_compile_instancing
        
            // Keywords
            #pragma multi_compile_fragment _ WRITE_MSAA_DEPTH
        #pragma shader_feature _ _SURFACE_TYPE_TRANSPARENT
        #pragma shader_feature_local _ _ADD_PRECOMPUTED_VELOCITY
        #pragma shader_feature_local _ _TRANSPARENT_WRITES_MOTION_VEC _TRANSPARENT_REFRACTIVE_SORT
            // GraphKeywords: <None>
        
            // Defines
            #define SHADERPASS SHADERPASS_MOTION_VECTORS
        #define SUPPORT_GLOBAL_MIP_BIAS 1
        
            // For custom interpolators to inject a substruct definition before FragInputs definition,
            // allowing for FragInputs to capture CI's intended for ShaderGraph's SDI.
            struct CustomInterpolators
        {
        };
        #define USE_CUSTOMINTERP_SUBSTRUCT
        
        
        
            // TODO: Merge FragInputsVFX substruct with CustomInterpolators.
        	#ifdef HAVE_VFX_MODIFICATION
        	struct FragInputsVFX
            {
                /* WARNING: $splice Could not find named fragment 'FragInputsVFX' */
            };
            #endif
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl" // Required by Tessellation.hlsl
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl" // Required to be include before we include properties as it define DECLARE_STACK_CB
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl" // Required before including properties as it defines UNITY_TEXTURE_STREAMING_DEBUG_VARS
            // Always include Shader Graph version
            // Always include last to avoid double macros
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl" // Need to be here for Gradient struct definition
        
            // --------------------------------------------------
            // Defines
        
            // Attribute
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TANGENT_TO_WORLD
            #define VARYINGS_NEED_TEXCOORD0
        
            #define HAVE_MESH_MODIFICATION
        
            //Strip down the FragInputs.hlsl (on graphics), so we can only optimize the interpolators we use.
            //if by accident something requests contents of FragInputs.hlsl, it will be caught as a compiler error
            //Frag inputs stripping is only enabled when FRAG_INPUTS_ENABLE_STRIPPING is set
            #if !defined(SHADER_STAGE_RAY_TRACING) && SHADERPASS != SHADERPASS_RAYTRACING_GBUFFER && SHADERPASS != SHADERPASS_FULL_SCREEN_DEBUG
            #define FRAG_INPUTS_ENABLE_STRIPPING
            #endif
            #define FRAG_INPUTS_USE_TEXCOORD0
        
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
        
        
        
        
            // Following two define are a workaround introduce in 10.1.x for RaytracingQualityNode
            // The ShaderGraph don't support correctly migration of this node as it serialize all the node data
            // in the json file making it impossible to uprgrade. Until we get a fix, we do a workaround here
            // to still allow us to rename the field and keyword of this node without breaking existing code.
            #ifdef RAYTRACING_SHADER_GRAPH_DEFAULT
            #define RAYTRACING_SHADER_GRAPH_HIGH
            #endif
        
            #ifdef RAYTRACING_SHADER_GRAPH_RAYTRACED
            #define RAYTRACING_SHADER_GRAPH_LOW
            #endif
            // end
        
            #ifndef SHADER_UNLIT
            // We need isFrontFace when using double sided - it is not required for unlit as in case of unlit double sided only drive the cullmode
            // VARYINGS_NEED_CULLFACE can be define by VaryingsMeshToPS.FaceSign input if a IsFrontFace Node is included in the shader graph.
            #if defined(_DOUBLESIDED_ON) && !defined(VARYINGS_NEED_CULLFACE)
                #define VARYINGS_NEED_CULLFACE
            #endif
            #endif
        
            // Specific Material Define
        // Setup a define to say we are an unlit shader
        #define SHADER_UNLIT
        
        // Following Macro are only used by Unlit material
        #if defined(_ENABLE_SHADOW_MATTE)
            #if SHADERPASS == SHADERPASS_FORWARD_UNLIT
                #pragma multi_compile_fragment USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST
            #elif SHADERPASS == SHADERPASS_PATH_TRACING
                #define LIGHTLOOP_DISABLE_TILE_AND_CLUSTER
            #endif
        
        // We don't want to have the lightloop defined for the ray tracing passes, but we do for the rasterisation and path tracing shader passes.
        #if !defined(SHADER_STAGE_RAY_TRACING) || SHADERPASS == SHADERPASS_PATH_TRACING
            #define HAS_LIGHTLOOP
        #endif
        #endif
            // Caution: we can use the define SHADER_UNLIT onlit after the above Material include as it is the Unlit template who define it
        
            // To handle SSR on transparent correctly with a possibility to enable/disable it per framesettings
            // we should have a code like this:
            // if !defined(_DISABLE_SSR_TRANSPARENT)
            // pragma multi_compile _ WRITE_NORMAL_BUFFER
            // endif
            // i.e we enable the multicompile only if we can receive SSR or not, and then C# code drive
            // it based on if SSR transparent in frame settings and not (and stripper can strip it).
            // this is currently not possible with our current preprocessor as _DISABLE_SSR_TRANSPARENT is a keyword not a define
            // so instead we used this and chose to pay the extra cost of normal write even if SSR transaprent is disabled.
            // Ideally the shader graph generator should handle it but condition below can't be handle correctly for now.
            #if SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_PREPASS
            #if !defined(_DISABLE_SSR_TRANSPARENT) && !defined(SHADER_UNLIT)
                #define WRITE_NORMAL_BUFFER
            #endif
            #endif
        
            // See Lit.shader
            #if SHADERPASS == SHADERPASS_MOTION_VECTORS && defined(WRITE_DECAL_BUFFER_AND_RENDERING_LAYER)
                #define WRITE_DECAL_BUFFER
            #endif
        
            #ifndef DEBUG_DISPLAY
                // In case of opaque we don't want to perform the alpha test, it is done in depth prepass and we use depth equal for ztest (setup from UI)
                // Don't do it with debug display mode as it is possible there is no depth prepass in this case
                #if !defined(_SURFACE_TYPE_TRANSPARENT)
                    #if SHADERPASS == SHADERPASS_FORWARD
                    #define SHADERPASS_FORWARD_BYPASS_ALPHA_TEST
                    #elif SHADERPASS == SHADERPASS_GBUFFER
                    #define SHADERPASS_GBUFFER_BYPASS_ALPHA_TEST
                    #endif
                #endif
            #endif
        
            // Define _DEFERRED_CAPABLE_MATERIAL for shader capable to run in deferred pass
            #if defined(SHADER_LIT) && !defined(_SURFACE_TYPE_TRANSPARENT)
                #define _DEFERRED_CAPABLE_MATERIAL
            #endif
        
            // Translate transparent motion vector define
            #if (defined(_TRANSPARENT_WRITES_MOTION_VEC) || defined(_TRANSPARENT_REFRACTIVE_SORT)) && defined(_SURFACE_TYPE_TRANSPARENT)
                #define _WRITE_TRANSPARENT_MOTION_VECTOR
            #endif
        
            // -- Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _VoidColor;
        float _Speed;
        float _FresnelPower;
        float4 _BorderColor;
        float2 _Seed;
        float4 _TwirlTexture_TexelSize;
        float _InnerEdge;
        float4 _EmissionColor;
        float _UseShadowThreshold;
        float4 _DoubleSidedConstants;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        float _BlendMode;
        float _EnableBlendModePreserveSpecularLighting;
        CBUFFER_END
        
        
        // Object and Global properties
        TEXTURE2D(_TwirlTexture);
        SAMPLER(sampler_TwirlTexture);
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Includes
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Debug/DebugDisplay.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/Unlit.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/ClassicNoise3D.hlsl"
        #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi3D.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonLighting.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            struct AttributesMesh
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct VaryingsMeshToPS
        {
            SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
             float3 positionRWS;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct SurfaceDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 WorldSpaceNormal;
             float3 ObjectSpaceViewDirection;
             float3 WorldSpaceViewDirection;
             float3 AbsoluteWorldSpacePosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct PackedVaryingsMeshToPS
        {
            SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
             float4 tangentWS : INTERP0;
             float4 texCoord0 : INTERP1;
             float3 positionRWS : INTERP2;
             float3 normalWS : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
        };
        
            PackedVaryingsMeshToPS PackVaryingsMeshToPS (VaryingsMeshToPS input)
        {
            PackedVaryingsMeshToPS output;
            ZERO_INITIALIZE(PackedVaryingsMeshToPS, output);
            output.positionCS = input.positionCS;
            output.tangentWS.xyzw = input.tangentWS;
            output.texCoord0.xyzw = input.texCoord0;
            output.positionRWS.xyz = input.positionRWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            return output;
        }
        
        VaryingsMeshToPS UnpackVaryingsMeshToPS (PackedVaryingsMeshToPS input)
        {
            VaryingsMeshToPS output;
            output.positionCS = input.positionCS;
            output.tangentWS = input.tangentWS.xyzw;
            output.texCoord0 = input.texCoord0.xyzw;
            output.positionRWS = input.positionRWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
        
            // Graph Functions
            
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_DotProduct_float3(float3 A, float3 B, out float Out)
        {
            Out = dot(A, B);
        }
        
        void Unity_SampleGradientV1_float(Gradient Gradient, float Time, out float4 Out)
        {
            // convert to OkLab if we need perceptual color space.
            float3 color = lerp(Gradient.colors[0].rgb, LinearToOklab(Gradient.colors[0].rgb), Gradient.type == 2);
        
            [unroll]
            for (int c = 1; c < Gradient.colorsLength; c++)
            {
                float colorPos = saturate((Time - Gradient.colors[c - 1].w) / (Gradient.colors[c].w - Gradient.colors[c - 1].w)) * step(c, Gradient.colorsLength - 1);
                float3 color2 = lerp(Gradient.colors[c].rgb, LinearToOklab(Gradient.colors[c].rgb), Gradient.type == 2);
                color = lerp(color, color2, lerp(colorPos, step(0.01, colorPos), Gradient.type % 2)); // grad.type == 1 is fixed, 0 and 2 are blends.
            }
            color = lerp(color, OklabToLinear(color), Gradient.type == 2);
        
        #ifdef UNITY_COLORSPACE_GAMMA
            color = LinearToSRGB(color);
        #endif
        
            float alpha = Gradient.alphas[0].x;
            [unroll]
            for (int a = 1; a < Gradient.alphasLength; a++)
            {
                float alphaPos = saturate((Time - Gradient.alphas[a - 1].y) / (Gradient.alphas[a].y - Gradient.alphas[a - 1].y)) * step(a, Gradient.alphasLength - 1);
                alpha = lerp(alpha, Gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), Gradient.type % 2));
            }
        
            Out = float4(color, alpha);
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
        Out = A * B;
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        struct Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float
        {
        };
        
        void SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(float3 Vector3_7940555B, float Vector1_1B8B9078, Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float IN, out float Value_0)
        {
        float3 _Property_44999cc87708de82a26b39ae1da975ec_Out_0_Vector3 = Vector3_7940555B;
        float _Property_dad5add45a7fa785be976f925bc5a5da_Out_0_Float = Vector1_1B8B9078;
        float3 _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3;
        Unity_Multiply_float3_float3(_Property_44999cc87708de82a26b39ae1da975ec_Out_0_Vector3, (_Property_dad5add45a7fa785be976f925bc5a5da_Out_0_Float.xxx), _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3);
        float _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float;
        PerlinNoise3D_float(_Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3, _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float);
        float _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float;
        Unity_Remap_float(_PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float, float2 (-1.15, 1.15), float2 (0, 1), _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float);
        Value_0 = _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_RandomRange_float(float2 Seed, float Min, float Max, out float Out)
        {
             float randomno =  frac(sin(dot(Seed, float2(12.9898, 78.233)))*43758.5453);
             Out = lerp(Min, Max, randomno);
        }
        
        struct Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float
        {
        };
        
        void SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(float3 Vector3_375394F7, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float IN, out float Value_1, out float Cells_2)
        {
        float3 _Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3 = Vector3_375394F7;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        VoronoiPrecise3D_float(_Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }
        
        float3 Unity_HDRP_GetEmissionHDRColor_float(float3 ldrColor, float luminanceIntensity, float exposureWeight)
        {
            float3 hdrColor = ldrColor * luminanceIntensity;
        
            #ifdef SHADERGRAPH_PREVIEW
            float inverseExposureMultiplier = 1.0;
            #else
            float inverseExposureMultiplier = GetInverseCurrentExposureMultiplier();
            #endif
        
            // Inverse pre-expose using _EmissiveExposureWeight weight
            hdrColor = lerp(hdrColor * inverseExposureMultiplier, hdrColor, exposureWeight);
            return hdrColor;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float3 Emission;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_d09e7f2c2a774dbd93cc943fa6a50940_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_VoidColor) : _VoidColor;
            float _Property_16eef79ff58443149895b35d0a88bdb2_Out_0_Float = _InnerEdge;
            float4 _UV_ce55b36e72754a339073e4c05a64b551_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_054563945a0a4f688fce11f0c067b0c8_Out_1_Vector2 = _UV_ce55b36e72754a339073e4c05a64b551_Out_0_Vector4.xy;
            float _Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float;
            Unity_Distance_float2(_Swizzle_054563945a0a4f688fce11f0c067b0c8_Out_1_Vector2, float2(0.5, 0.5), _Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float);
            float _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float;
            Unity_OneMinus_float(_Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float, _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float);
            float _Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float;
            Unity_Smoothstep_float(_Property_16eef79ff58443149895b35d0a88bdb2_Out_0_Float, float(1), _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float, _Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float);
            Gradient _Gradient_c34cf660310448768ec81f4600b23ad8_Out_0_Gradient = NewGradient(0, 4, 2, float4(0, 0, 0, 0.271931),float4(0.01568037, 0.01568037, 0.01568037, 0.4678416),float4(0.1071531, 0.1071531, 0.1071531, 0.6052644),float4(1, 1, 1, 1),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0), float2(1, 0),float2(1, 1),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0));
            float _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float;
            Unity_DotProduct_float3(IN.ObjectSpaceViewDirection, IN.ObjectSpaceNormal, _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float);
            float4 _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4;
            Unity_SampleGradientV1_float(_Gradient_c34cf660310448768ec81f4600b23ad8_Out_0_Gradient, _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float, _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4);
            float4 _Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float.xxxx), _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4, _Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4);
            Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a;
            float _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float;
            SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(IN.ObjectSpaceViewDirection, float(10), _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a, _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float);
            float3 _Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3;
            Unity_Multiply_float3_float3(IN.ObjectSpaceViewDirection, (_Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float.xxx), _Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3);
            float _Property_ba2d1376c19c40fbadc820d0fb721b2e_Out_0_Float = _Speed;
            float _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_ba2d1376c19c40fbadc820d0fb721b2e_Out_0_Float, _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float);
            float2 _Property_2cfd9c61936548ee85383d83df6beba0_Out_0_Vector2 = _Seed;
            float _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float;
            Unity_RandomRange_float(_Property_2cfd9c61936548ee85383d83df6beba0_Out_0_Vector2, float(26.96), float(45.6), _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float);
            Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a;
            float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Value_1_Float;
            float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float;
            SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(_Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3, _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float, _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Value_1_Float, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float);
            float4 _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4, (_Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float.xxxx), _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4);
            float4 _Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_d09e7f2c2a774dbd93cc943fa6a50940_Out_0_Vector4, _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4, _Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4);
            float4 _Property_e0f9565d1eaa452fb638faa34cc517eb_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_BorderColor) : _BorderColor;
            float3 _Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3;
            Unity_Absolute_float3(IN.AbsoluteWorldSpacePosition, _Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3);
            Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1;
            float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Value_1_Float;
            float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float;
            SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(_Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3, IN.TimeParameters.x, float(6.2), _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1, _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Value_1_Float, _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float);
            float _Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float;
            Unity_Smoothstep_float(float(-0.41), float(1), _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float, _Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float);
            float _Property_82bfb359e6494ff08a54e8c08976a6f1_Out_0_Float = _FresnelPower;
            float _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_82bfb359e6494ff08a54e8c08976a6f1_Out_0_Float, _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float);
            float _Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float, _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float, _Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float);
            float4 _Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_e0f9565d1eaa452fb638faa34cc517eb_Out_0_Vector4, (_Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float.xxxx), _Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4);
            float3 _EmissionNode_87074dafd5ff4d39bf4005b1089a56e4_Output_0_Vector3 = Unity_HDRP_GetEmissionHDRColor_float((_Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4.xyz).xyz, float(3), float(0));
            float4 _Add_0b088d988049415c84334d987e692959_Out_2_Vector4;
            Unity_Add_float4(_Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4, (_Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float.xxxx), _Add_0b088d988049415c84334d987e692959_Out_2_Vector4);
            surface.BaseColor = (_Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4.xyz);
            surface.Emission = _EmissionNode_87074dafd5ff4d39bf4005b1089a56e4_Output_0_Vector3;
            surface.Alpha = (_Add_0b088d988049415c84334d987e692959_Out_2_Vector4).x;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES AttributesMesh
            #define VaryingsMeshType VaryingsMeshToPS
            #define VFX_SRP_VARYINGS VaryingsMeshType
            #define VFX_SRP_SURFACE_INPUTS FragInputs
            #endif
            
        VertexDescriptionInputs AttributesMeshToVertexDescriptionInputs(AttributesMesh input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else
        #endif
        
            return output;
        }
        
        VertexDescription GetVertexDescription(AttributesMesh input, float3 timeParameters
        #ifdef HAVE_VFX_MODIFICATION
            , AttributesElement element
        #endif
        )
        {
            // build graph inputs
            VertexDescriptionInputs vertexDescriptionInputs = AttributesMeshToVertexDescriptionInputs(input);
            // Override time parameters with used one (This is required to correctly handle motion vectors for vertex animation based on time)
        
            // evaluate vertex graph
        #ifdef HAVE_VFX_MODIFICATION
            GraphProperties properties;
            ZERO_INITIALIZE(GraphProperties, properties);
        
            // Fetch the vertex graph properties for the particle instance.
            GetElementVertexProperties(element, properties);
        
            VertexDescription vertexDescription = VertexDescriptionFunction(vertexDescriptionInputs, properties);
        #else
            VertexDescription vertexDescription = VertexDescriptionFunction(vertexDescriptionInputs);
        #endif
            return vertexDescription;
        
        }
        
        AttributesMesh ApplyMeshModification(AttributesMesh input, float3 timeParameters
        #ifdef USE_CUSTOMINTERP_SUBSTRUCT
            #ifdef TESSELLATION_ON
            , inout VaryingsMeshToDS varyings
            #else
            , inout VaryingsMeshToPS varyings
            #endif
        #endif
        #ifdef HAVE_VFX_MODIFICATION
                , AttributesElement element
        #endif
            )
        {
            VertexDescription vertexDescription = GetVertexDescription(input, timeParameters
        #ifdef HAVE_VFX_MODIFICATION
                , element
        #endif
            );
        
            // copy graph output to the results
            input.positionOS = vertexDescription.Position;
            input.normalOS = vertexDescription.Normal;
            input.tangentOS.xyz = vertexDescription.Tangent;
        
            
        
            return input;
        }
        
        #if defined(_ADD_CUSTOM_VELOCITY) // For shader graph custom velocity
        // Return precomputed Velocity in object space
        float3 GetCustomVelocity(AttributesMesh input
        #ifdef HAVE_VFX_MODIFICATION
            , AttributesElement element
        #endif
        )
        {
            VertexDescription vertexDescription = GetVertexDescription(input, _TimeParameters.xyz
        #ifdef HAVE_VFX_MODIFICATION
                , element
        #endif
            );
            return vertexDescription.CustomVelocity;
        }
        #endif
        
        FragInputs BuildFragInputs(VaryingsMeshToPS input)
        {
            FragInputs output;
            ZERO_INITIALIZE(FragInputs, output);
        
            // Init to some default value to make the computer quiet (else it output 'divide by zero' warning even if value is not used).
            // TODO: this is a really poor workaround, but the variable is used in a bunch of places
            // to compute normals which are then passed on elsewhere to compute other values...
            output.tangentToWorld = k_identity3x3;
            output.positionSS = input.positionCS;       // input.positionCS is SV_Position
        
            output.positionRWS =                input.positionRWS;
            output.tangentToWorld =             BuildTangentToWorld(input.tangentWS, input.normalWS);
            output.texCoord0 =                  input.texCoord0;
        
        #if UNITY_ANY_INSTANCING_ENABLED
        #else
        #endif
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
        
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            // splice point to copy custom interpolator fields from varyings to frag inputs
            
        
            return output;
        }
        
        // existing HDRP code uses the combined function to go directly from packed to frag inputs
        FragInputs UnpackVaryingsMeshToFragInputs(PackedVaryingsMeshToPS input)
        {
            UNITY_SETUP_INSTANCE_ID(input);
        #if defined(HAVE_VFX_MODIFICATION) && defined(UNITY_INSTANCING_ENABLED)
            unity_InstanceID = input.instanceID;
        #endif
            VaryingsMeshToPS unpacked = UnpackVaryingsMeshToPS(input);
            return BuildFragInputs(unpacked);
        }
            SurfaceDescriptionInputs FragInputsToSurfaceDescriptionInputs(FragInputs input, float3 viewWS)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            output.WorldSpaceNormal =                           normalize(input.tangentToWorld[2].xyz);
            #if defined(SHADER_STAGE_RAY_TRACING)
            output.ObjectSpaceNormal =                          mul(output.WorldSpaceNormal, (float3x3) ObjectToWorld3x4());
            #else
            output.ObjectSpaceNormal =                          normalize(mul(output.WorldSpaceNormal, (float3x3) UNITY_MATRIX_M));           // transposed multiplication by inverse matrix to handle normal scale
            #endif
            output.WorldSpaceViewDirection =                    normalize(viewWS);
            output.ObjectSpaceViewDirection =                   TransformWorldToObjectDir(output.WorldSpaceViewDirection);
        #if SHADERPASS != SHADERPASS_FOG_VOLUME_VOXELIZATION
        #else
        #endif
            output.AbsoluteWorldSpacePosition =                 GetAbsolutePositionWS(input.positionRWS);
        #if SHADERPASS != SHADERPASS_FOG_VOLUME_VOXELIZATION
        #else
        #endif
        
        #if UNITY_UV_STARTS_AT_TOP
        #else
        #endif
        
        
            output.uv0 =                                        input.texCoord0;
            output.TimeParameters =                             _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        
            // splice point to copy frag inputs custom interpolator pack into the SDI
            
        
            return output;
        }
        
            // --------------------------------------------------
            // Build Surface Data (Specific Material)
        
        void ApplyDecalToSurfaceDataNoNormal(DecalSurfaceData decalSurfaceData, inout SurfaceData surfaceData);
        
        void ApplyDecalAndGetNormal(FragInputs fragInputs, PositionInputs posInput, SurfaceDescription surfaceDescription,
            inout SurfaceData surfaceData)
        {
            float3 doubleSidedConstants = GetDoubleSidedConstants();
        
        #ifdef DECAL_NORMAL_BLENDING
            // SG nodes don't ouptut surface gradients, so if decals require surf grad blending, we have to convert
            // the normal to gradient before applying the decal. We then have to resolve the gradient back to world space
            float3 normalTS;
        
        
        
            #if HAVE_DECALS
            if (_EnableDecals)
            {
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs, alpha);
                ApplyDecalToSurfaceNormal(decalSurfaceData, fragInputs.tangentToWorld[2], normalTS);
                ApplyDecalToSurfaceDataNoNormal(decalSurfaceData, surfaceData);
            }
            #endif
        
            GetNormalWS_SG(fragInputs, normalTS, surfaceData.normalWS, doubleSidedConstants);
        #else
            // normal delivered to master node
        
            #if HAVE_DECALS
            if (_EnableDecals)
            {
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                // Both uses and modifies 'surfaceData.normalWS'.
                DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs, alpha);
                ApplyDecalToSurfaceNormal(decalSurfaceData, surfaceData.normalWS.xyz);
                ApplyDecalToSurfaceDataNoNormal(decalSurfaceData, surfaceData);
            }
            #endif
        #endif
        }
        void BuildSurfaceData(FragInputs fragInputs, inout SurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData)
        {
            // setup defaults -- these are used if the graph doesn't output a value
            ZERO_INITIALIZE(SurfaceData, surfaceData);
        
            // copy across graph values, if defined
            surfaceData.color = surfaceDescription.BaseColor;
        
            #ifdef WRITE_NORMAL_BUFFER
            // When we need to export the normal (in the depth prepass, we write the geometry one)
            surfaceData.normalWS = fragInputs.tangentToWorld[2];
            #endif
        
            #if defined(DEBUG_DISPLAY)
            #if !defined(SHADER_STAGE_RAY_TRACING)
            // Mipmap mode debugging isn't supported with ray tracing as it relies on derivatives
                if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                {
                    #ifdef FRAG_INPUTS_USE_TEXCOORD0
                        surfaceData.color = GET_TEXTURE_STREAMING_DEBUG(posInput.positionSS, fragInputs.texCoord0);
                    #else
                        surfaceData.color = GET_TEXTURE_STREAMING_DEBUG_NO_UV(posInput.positionSS);
                    #endif
                }
            #endif
            #endif
        
            #ifdef _ENABLE_SHADOW_MATTE
        
                #if (SHADERPASS == SHADERPASS_FORWARD_UNLIT) || (SHADERPASS == SHADERPASS_RAYTRACING_GBUFFER) || (SHADERPASS == SHADERPASS_RAYTRACING_INDIRECT) || (SHADERPASS == SHADERPASS_RAYTRACING_FORWARD)
        
                    HDShadowContext shadowContext = InitShadowContext();
        
                    // Evaluate the shadow, the normal is guaranteed if shadow matte is enabled on this shader.
                    float3 shadow3;
                    ShadowLoopMin(shadowContext, posInput, normalize(fragInputs.tangentToWorld[2]), asuint(_ShadowMatteFilter), GetMeshRenderingLayerMask(), shadow3);
        
                    // Compute the average value in the fourth channel
                    float4 shadow = float4(shadow3, dot(shadow3, float3(1.0/3.0, 1.0/3.0, 1.0/3.0)));
        
                    float4 shadowColor = (1.0 - shadow) * surfaceDescription.ShadowTint.rgba;
                    float  localAlpha  = saturate(shadowColor.a + surfaceDescription.Alpha);
        
                    // Keep the nested lerp
                    // With no Color (bsdfData.color.rgb, bsdfData.color.a == 0.0f), just use ShadowColor*Color to avoid a ring of "white" around the shadow
                    // And mix color to consider the Color & ShadowColor alpha (from texture or/and color picker)
                    #ifdef _SURFACE_TYPE_TRANSPARENT
                        surfaceData.color = lerp(shadowColor.rgb * surfaceData.color, lerp(lerp(shadowColor.rgb, surfaceData.color, 1.0 - surfaceDescription.ShadowTint.a), surfaceData.color, shadow.rgb), surfaceDescription.Alpha);
                    #else
                        surfaceData.color = lerp(lerp(shadowColor.rgb, surfaceData.color, 1.0 - surfaceDescription.ShadowTint.a), surfaceData.color, shadow.rgb);
                    #endif
                    localAlpha = ApplyBlendMode(surfaceData.color, localAlpha).a;
        
                    surfaceDescription.Alpha = localAlpha;
        
                #elif SHADERPASS == SHADERPASS_PATH_TRACING
        
                    surfaceData.normalWS = fragInputs.tangentToWorld[2];
                    surfaceData.shadowTint = surfaceDescription.ShadowTint.rgba;
        
                #endif
        
            #endif // _ENABLE_SHADOW_MATTE
        }
        
            // --------------------------------------------------
            // Get Surface And BuiltinData
        
            void GetSurfaceAndBuiltinData(FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData RAY_TRACING_OPTIONAL_PARAMETERS)
            {
                // Don't dither if displaced tessellation (we're fading out the displacement instead to match the next LOD)
                #if !defined(SHADER_STAGE_RAY_TRACING) && !defined(_TESSELLATION_DISPLACEMENT)
                #ifdef LOD_FADE_CROSSFADE // enable dithering LOD transition if user select CrossFade transition in LOD group
                LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                #endif
                #endif
        
                #ifndef SHADER_UNLIT
                #ifdef _DOUBLESIDED_ON
                    float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
                #else
                    float3 doubleSidedConstants = float3(1.0, 1.0, 1.0);
                #endif
        
                ApplyDoubleSidedFlipOrMirror(fragInputs, doubleSidedConstants); // Apply double sided flip on the vertex normal
                #endif // SHADER_UNLIT
        
                SurfaceDescriptionInputs surfaceDescriptionInputs = FragInputsToSurfaceDescriptionInputs(fragInputs, V);
        
                #if defined(HAVE_VFX_MODIFICATION)
                GraphProperties properties;
                ZERO_INITIALIZE(GraphProperties, properties);
        
                GetElementPixelProperties(fragInputs, properties);
        
                SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs, properties);
                #else
                SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);
                #endif
        
                #ifdef DEBUG_DISPLAY
                if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                {
                    surfaceDescription.Alpha = 1.0f;
                }
                #endif
        
                // Perform alpha test very early to save performance (a killed pixel will not sample textures)
                // TODO: split graph evaluation to grab just alpha dependencies first? tricky..
                #ifdef _ALPHATEST_ON
                    float alphaCutoff = surfaceDescription.AlphaClipThreshold;
                    #if SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_PREPASS
                    // The TransparentDepthPrepass is also used with SSR transparent.
                    // If an artists enable transaprent SSR but not the TransparentDepthPrepass itself, then we use AlphaClipThreshold
                    // otherwise if TransparentDepthPrepass is enabled we use AlphaClipThresholdDepthPrepass
                    #elif SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_POSTPASS
                    // DepthPostpass always use its own alpha threshold
                    alphaCutoff = surfaceDescription.AlphaClipThresholdDepthPostpass;
                    #elif (SHADERPASS == SHADERPASS_SHADOWS) || (SHADERPASS == SHADERPASS_RAYTRACING_VISIBILITY)
                    // If use shadow threshold isn't enable we don't allow any test
                    #endif
        
                    GENERIC_ALPHA_TEST(surfaceDescription.Alpha, alphaCutoff);
                #endif
        
                #if !defined(SHADER_STAGE_RAY_TRACING) && _DEPTHOFFSET_ON
                ApplyDepthOffsetPositionInput(V, surfaceDescription.DepthOffset, GetViewForwardDir(), GetWorldToHClipMatrix(), posInput);
                #endif
        
                #ifndef SHADER_UNLIT
                float3 bentNormalWS;
                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData, bentNormalWS);
        
                #ifdef FRAG_INPUTS_USE_TEXCOORD1
                    float4 lightmapTexCoord1 = fragInputs.texCoord1;
                #else
                    float4 lightmapTexCoord1 = float4(0,0,0,0);
                #endif
        
                #ifdef FRAG_INPUTS_USE_TEXCOORD2
                    float4 lightmapTexCoord2 = fragInputs.texCoord2;
                #else
                    float4 lightmapTexCoord2 = float4(0,0,0,0);
                #endif
        
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                // Builtin Data
                // For back lighting we use the oposite vertex normal
                InitBuiltinData(posInput, alpha, bentNormalWS, -fragInputs.tangentToWorld[2], lightmapTexCoord1, lightmapTexCoord2, builtinData);
        
                #else
                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData);
        
                ZERO_BUILTIN_INITIALIZE(builtinData); // No call to InitBuiltinData as we don't have any lighting
                builtinData.opacity = surfaceDescription.Alpha;
        
                #if defined(DEBUG_DISPLAY)
                    // Light Layers are currently not used for the Unlit shader (because it is not lit)
                    // But Unlit objects do cast shadows according to their rendering layer mask, which is what we want to
                    // display in the light layers visualization mode, therefore we need the renderingLayers
                    builtinData.renderingLayers = GetMeshRenderingLayerMask();
                #endif
        
                #endif // SHADER_UNLIT
        
                #ifdef _ALPHATEST_ON
                    // Used for sharpening by alpha to mask - Alpha to covertage is only used with depth only and forward pass (no shadow pass, no transparent pass)
                    builtinData.alphaClipTreshold = alphaCutoff;
                #endif
        
                // override sampleBakedGI - not used by Unlit
        		// When overriding GI, we need to force the isLightmap flag to make sure we don't add APV (sampled in the lightloop) on top of the overridden value (set at GBuffer stage)
        
                builtinData.emissiveColor = surfaceDescription.Emission;
        
                // Note this will not fully work on transparent surfaces (can check with _SURFACE_TYPE_TRANSPARENT define)
                // We will always overwrite vt feeback with the nearest. So behind transparent surfaces vt will not be resolved
                // This is a limitation of the current MRT approach.
                #ifdef UNITY_VIRTUAL_TEXTURING
                #endif
        
                #if _DEPTHOFFSET_ON
                builtinData.depthOffset = surfaceDescription.DepthOffset;
                #endif
        
                // TODO: We should generate distortion / distortionBlur for non distortion pass
                #if (SHADERPASS == SHADERPASS_DISTORTION)
                builtinData.distortion = surfaceDescription.Distortion;
                builtinData.distortionBlur = surfaceDescription.DistortionBlur;
                #endif
        
                #ifndef SHADER_UNLIT
                // PostInitBuiltinData call ApplyDebugToBuiltinData
                PostInitBuiltinData(V, posInput, surfaceData, builtinData);
                #else
                ApplyDebugToBuiltinData(builtinData);
                #endif
        
                RAY_TRACING_OPTIONAL_ALPHA_TEST_PASS
            }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassMotionVectors.hlsl"
        
            // --------------------------------------------------
            // Visual Effect Vertex Invocations
        
        	#ifdef HAVE_VFX_MODIFICATION
                #if !defined(SHADER_STAGE_RAY_TRACING)
        	    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/VisualEffectVertex.hlsl"
                #else
                #endif
        	#endif
        
            ENDHLSL
        }
        Pass
        {
            Name "DepthForwardOnly"
            Tags
            {
                "LightMode" = "DepthForwardOnly"
            }
        
            // Render State
            Cull [_CullMode]
        ZWrite On
        Stencil
        {
        WriteMask [_StencilWriteMaskDepth]
        Ref [_StencilRefDepth]
        CompFront Always
        PassFront Replace
        CompBack Always
        PassBack Replace
        }
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma instancing_options renderinglayer
        #pragma target 4.5
        #pragma vertex Vert
        #pragma fragment Frag
        #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch
        #pragma multi_compile_instancing
        
            // Keywords
            #pragma multi_compile_fragment _ WRITE_MSAA_DEPTH
            // GraphKeywords: <None>
        
            // Defines
            #define SHADERPASS SHADERPASS_DEPTH_ONLY
        #define SUPPORT_GLOBAL_MIP_BIAS 1
        
            // For custom interpolators to inject a substruct definition before FragInputs definition,
            // allowing for FragInputs to capture CI's intended for ShaderGraph's SDI.
            struct CustomInterpolators
        {
        };
        #define USE_CUSTOMINTERP_SUBSTRUCT
        
        
        
            // TODO: Merge FragInputsVFX substruct with CustomInterpolators.
        	#ifdef HAVE_VFX_MODIFICATION
        	struct FragInputsVFX
            {
                /* WARNING: $splice Could not find named fragment 'FragInputsVFX' */
            };
            #endif
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl" // Required by Tessellation.hlsl
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl" // Required to be include before we include properties as it define DECLARE_STACK_CB
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl" // Required before including properties as it defines UNITY_TEXTURE_STREAMING_DEBUG_VARS
            // Always include Shader Graph version
            // Always include last to avoid double macros
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl" // Need to be here for Gradient struct definition
        
            // --------------------------------------------------
            // Defines
        
            // Attribute
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TANGENT_TO_WORLD
            #define VARYINGS_NEED_TEXCOORD0
        
            #define HAVE_MESH_MODIFICATION
        
            //Strip down the FragInputs.hlsl (on graphics), so we can only optimize the interpolators we use.
            //if by accident something requests contents of FragInputs.hlsl, it will be caught as a compiler error
            //Frag inputs stripping is only enabled when FRAG_INPUTS_ENABLE_STRIPPING is set
            #if !defined(SHADER_STAGE_RAY_TRACING) && SHADERPASS != SHADERPASS_RAYTRACING_GBUFFER && SHADERPASS != SHADERPASS_FULL_SCREEN_DEBUG
            #define FRAG_INPUTS_ENABLE_STRIPPING
            #endif
            #define FRAG_INPUTS_USE_TEXCOORD0
        
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
        
        
        
        
            // Following two define are a workaround introduce in 10.1.x for RaytracingQualityNode
            // The ShaderGraph don't support correctly migration of this node as it serialize all the node data
            // in the json file making it impossible to uprgrade. Until we get a fix, we do a workaround here
            // to still allow us to rename the field and keyword of this node without breaking existing code.
            #ifdef RAYTRACING_SHADER_GRAPH_DEFAULT
            #define RAYTRACING_SHADER_GRAPH_HIGH
            #endif
        
            #ifdef RAYTRACING_SHADER_GRAPH_RAYTRACED
            #define RAYTRACING_SHADER_GRAPH_LOW
            #endif
            // end
        
            #ifndef SHADER_UNLIT
            // We need isFrontFace when using double sided - it is not required for unlit as in case of unlit double sided only drive the cullmode
            // VARYINGS_NEED_CULLFACE can be define by VaryingsMeshToPS.FaceSign input if a IsFrontFace Node is included in the shader graph.
            #if defined(_DOUBLESIDED_ON) && !defined(VARYINGS_NEED_CULLFACE)
                #define VARYINGS_NEED_CULLFACE
            #endif
            #endif
        
            // Specific Material Define
        // Setup a define to say we are an unlit shader
        #define SHADER_UNLIT
        
        // Following Macro are only used by Unlit material
        #if defined(_ENABLE_SHADOW_MATTE)
            #if SHADERPASS == SHADERPASS_FORWARD_UNLIT
                #pragma multi_compile_fragment USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST
            #elif SHADERPASS == SHADERPASS_PATH_TRACING
                #define LIGHTLOOP_DISABLE_TILE_AND_CLUSTER
            #endif
        
        // We don't want to have the lightloop defined for the ray tracing passes, but we do for the rasterisation and path tracing shader passes.
        #if !defined(SHADER_STAGE_RAY_TRACING) || SHADERPASS == SHADERPASS_PATH_TRACING
            #define HAS_LIGHTLOOP
        #endif
        #endif
            // Caution: we can use the define SHADER_UNLIT onlit after the above Material include as it is the Unlit template who define it
        
            // To handle SSR on transparent correctly with a possibility to enable/disable it per framesettings
            // we should have a code like this:
            // if !defined(_DISABLE_SSR_TRANSPARENT)
            // pragma multi_compile _ WRITE_NORMAL_BUFFER
            // endif
            // i.e we enable the multicompile only if we can receive SSR or not, and then C# code drive
            // it based on if SSR transparent in frame settings and not (and stripper can strip it).
            // this is currently not possible with our current preprocessor as _DISABLE_SSR_TRANSPARENT is a keyword not a define
            // so instead we used this and chose to pay the extra cost of normal write even if SSR transaprent is disabled.
            // Ideally the shader graph generator should handle it but condition below can't be handle correctly for now.
            #if SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_PREPASS
            #if !defined(_DISABLE_SSR_TRANSPARENT) && !defined(SHADER_UNLIT)
                #define WRITE_NORMAL_BUFFER
            #endif
            #endif
        
            // See Lit.shader
            #if SHADERPASS == SHADERPASS_MOTION_VECTORS && defined(WRITE_DECAL_BUFFER_AND_RENDERING_LAYER)
                #define WRITE_DECAL_BUFFER
            #endif
        
            #ifndef DEBUG_DISPLAY
                // In case of opaque we don't want to perform the alpha test, it is done in depth prepass and we use depth equal for ztest (setup from UI)
                // Don't do it with debug display mode as it is possible there is no depth prepass in this case
                #if !defined(_SURFACE_TYPE_TRANSPARENT)
                    #if SHADERPASS == SHADERPASS_FORWARD
                    #define SHADERPASS_FORWARD_BYPASS_ALPHA_TEST
                    #elif SHADERPASS == SHADERPASS_GBUFFER
                    #define SHADERPASS_GBUFFER_BYPASS_ALPHA_TEST
                    #endif
                #endif
            #endif
        
            // Define _DEFERRED_CAPABLE_MATERIAL for shader capable to run in deferred pass
            #if defined(SHADER_LIT) && !defined(_SURFACE_TYPE_TRANSPARENT)
                #define _DEFERRED_CAPABLE_MATERIAL
            #endif
        
            // Translate transparent motion vector define
            #if (defined(_TRANSPARENT_WRITES_MOTION_VEC) || defined(_TRANSPARENT_REFRACTIVE_SORT)) && defined(_SURFACE_TYPE_TRANSPARENT)
                #define _WRITE_TRANSPARENT_MOTION_VECTOR
            #endif
        
            // -- Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _VoidColor;
        float _Speed;
        float _FresnelPower;
        float4 _BorderColor;
        float2 _Seed;
        float4 _TwirlTexture_TexelSize;
        float _InnerEdge;
        float4 _EmissionColor;
        float _UseShadowThreshold;
        float4 _DoubleSidedConstants;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        float _BlendMode;
        float _EnableBlendModePreserveSpecularLighting;
        CBUFFER_END
        
        
        // Object and Global properties
        TEXTURE2D(_TwirlTexture);
        SAMPLER(sampler_TwirlTexture);
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Includes
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Debug/DebugDisplay.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/Unlit.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/ClassicNoise3D.hlsl"
        #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi3D.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonLighting.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            struct AttributesMesh
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct VaryingsMeshToPS
        {
            SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
             float3 positionRWS;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct SurfaceDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 WorldSpaceNormal;
             float3 ObjectSpaceViewDirection;
             float3 WorldSpaceViewDirection;
             float3 AbsoluteWorldSpacePosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct PackedVaryingsMeshToPS
        {
            SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
             float4 tangentWS : INTERP0;
             float4 texCoord0 : INTERP1;
             float3 positionRWS : INTERP2;
             float3 normalWS : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
        };
        
            PackedVaryingsMeshToPS PackVaryingsMeshToPS (VaryingsMeshToPS input)
        {
            PackedVaryingsMeshToPS output;
            ZERO_INITIALIZE(PackedVaryingsMeshToPS, output);
            output.positionCS = input.positionCS;
            output.tangentWS.xyzw = input.tangentWS;
            output.texCoord0.xyzw = input.texCoord0;
            output.positionRWS.xyz = input.positionRWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            return output;
        }
        
        VaryingsMeshToPS UnpackVaryingsMeshToPS (PackedVaryingsMeshToPS input)
        {
            VaryingsMeshToPS output;
            output.positionCS = input.positionCS;
            output.tangentWS = input.tangentWS.xyzw;
            output.texCoord0 = input.texCoord0.xyzw;
            output.positionRWS = input.positionRWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
        
            // Graph Functions
            
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_DotProduct_float3(float3 A, float3 B, out float Out)
        {
            Out = dot(A, B);
        }
        
        void Unity_SampleGradientV1_float(Gradient Gradient, float Time, out float4 Out)
        {
            // convert to OkLab if we need perceptual color space.
            float3 color = lerp(Gradient.colors[0].rgb, LinearToOklab(Gradient.colors[0].rgb), Gradient.type == 2);
        
            [unroll]
            for (int c = 1; c < Gradient.colorsLength; c++)
            {
                float colorPos = saturate((Time - Gradient.colors[c - 1].w) / (Gradient.colors[c].w - Gradient.colors[c - 1].w)) * step(c, Gradient.colorsLength - 1);
                float3 color2 = lerp(Gradient.colors[c].rgb, LinearToOklab(Gradient.colors[c].rgb), Gradient.type == 2);
                color = lerp(color, color2, lerp(colorPos, step(0.01, colorPos), Gradient.type % 2)); // grad.type == 1 is fixed, 0 and 2 are blends.
            }
            color = lerp(color, OklabToLinear(color), Gradient.type == 2);
        
        #ifdef UNITY_COLORSPACE_GAMMA
            color = LinearToSRGB(color);
        #endif
        
            float alpha = Gradient.alphas[0].x;
            [unroll]
            for (int a = 1; a < Gradient.alphasLength; a++)
            {
                float alphaPos = saturate((Time - Gradient.alphas[a - 1].y) / (Gradient.alphas[a].y - Gradient.alphas[a - 1].y)) * step(a, Gradient.alphasLength - 1);
                alpha = lerp(alpha, Gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), Gradient.type % 2));
            }
        
            Out = float4(color, alpha);
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
        Out = A * B;
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        struct Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float
        {
        };
        
        void SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(float3 Vector3_7940555B, float Vector1_1B8B9078, Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float IN, out float Value_0)
        {
        float3 _Property_44999cc87708de82a26b39ae1da975ec_Out_0_Vector3 = Vector3_7940555B;
        float _Property_dad5add45a7fa785be976f925bc5a5da_Out_0_Float = Vector1_1B8B9078;
        float3 _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3;
        Unity_Multiply_float3_float3(_Property_44999cc87708de82a26b39ae1da975ec_Out_0_Vector3, (_Property_dad5add45a7fa785be976f925bc5a5da_Out_0_Float.xxx), _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3);
        float _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float;
        PerlinNoise3D_float(_Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3, _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float);
        float _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float;
        Unity_Remap_float(_PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float, float2 (-1.15, 1.15), float2 (0, 1), _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float);
        Value_0 = _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_RandomRange_float(float2 Seed, float Min, float Max, out float Out)
        {
             float randomno =  frac(sin(dot(Seed, float2(12.9898, 78.233)))*43758.5453);
             Out = lerp(Min, Max, randomno);
        }
        
        struct Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float
        {
        };
        
        void SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(float3 Vector3_375394F7, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float IN, out float Value_1, out float Cells_2)
        {
        float3 _Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3 = Vector3_375394F7;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        VoronoiPrecise3D_float(_Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }
        
        float3 Unity_HDRP_GetEmissionHDRColor_float(float3 ldrColor, float luminanceIntensity, float exposureWeight)
        {
            float3 hdrColor = ldrColor * luminanceIntensity;
        
            #ifdef SHADERGRAPH_PREVIEW
            float inverseExposureMultiplier = 1.0;
            #else
            float inverseExposureMultiplier = GetInverseCurrentExposureMultiplier();
            #endif
        
            // Inverse pre-expose using _EmissiveExposureWeight weight
            hdrColor = lerp(hdrColor * inverseExposureMultiplier, hdrColor, exposureWeight);
            return hdrColor;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float3 Emission;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_d09e7f2c2a774dbd93cc943fa6a50940_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_VoidColor) : _VoidColor;
            float _Property_16eef79ff58443149895b35d0a88bdb2_Out_0_Float = _InnerEdge;
            float4 _UV_ce55b36e72754a339073e4c05a64b551_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_054563945a0a4f688fce11f0c067b0c8_Out_1_Vector2 = _UV_ce55b36e72754a339073e4c05a64b551_Out_0_Vector4.xy;
            float _Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float;
            Unity_Distance_float2(_Swizzle_054563945a0a4f688fce11f0c067b0c8_Out_1_Vector2, float2(0.5, 0.5), _Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float);
            float _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float;
            Unity_OneMinus_float(_Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float, _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float);
            float _Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float;
            Unity_Smoothstep_float(_Property_16eef79ff58443149895b35d0a88bdb2_Out_0_Float, float(1), _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float, _Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float);
            Gradient _Gradient_c34cf660310448768ec81f4600b23ad8_Out_0_Gradient = NewGradient(0, 4, 2, float4(0, 0, 0, 0.271931),float4(0.01568037, 0.01568037, 0.01568037, 0.4678416),float4(0.1071531, 0.1071531, 0.1071531, 0.6052644),float4(1, 1, 1, 1),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0), float2(1, 0),float2(1, 1),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0));
            float _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float;
            Unity_DotProduct_float3(IN.ObjectSpaceViewDirection, IN.ObjectSpaceNormal, _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float);
            float4 _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4;
            Unity_SampleGradientV1_float(_Gradient_c34cf660310448768ec81f4600b23ad8_Out_0_Gradient, _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float, _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4);
            float4 _Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float.xxxx), _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4, _Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4);
            Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a;
            float _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float;
            SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(IN.ObjectSpaceViewDirection, float(10), _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a, _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float);
            float3 _Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3;
            Unity_Multiply_float3_float3(IN.ObjectSpaceViewDirection, (_Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float.xxx), _Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3);
            float _Property_ba2d1376c19c40fbadc820d0fb721b2e_Out_0_Float = _Speed;
            float _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_ba2d1376c19c40fbadc820d0fb721b2e_Out_0_Float, _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float);
            float2 _Property_2cfd9c61936548ee85383d83df6beba0_Out_0_Vector2 = _Seed;
            float _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float;
            Unity_RandomRange_float(_Property_2cfd9c61936548ee85383d83df6beba0_Out_0_Vector2, float(26.96), float(45.6), _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float);
            Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a;
            float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Value_1_Float;
            float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float;
            SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(_Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3, _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float, _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Value_1_Float, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float);
            float4 _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4, (_Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float.xxxx), _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4);
            float4 _Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_d09e7f2c2a774dbd93cc943fa6a50940_Out_0_Vector4, _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4, _Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4);
            float4 _Property_e0f9565d1eaa452fb638faa34cc517eb_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_BorderColor) : _BorderColor;
            float3 _Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3;
            Unity_Absolute_float3(IN.AbsoluteWorldSpacePosition, _Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3);
            Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1;
            float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Value_1_Float;
            float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float;
            SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(_Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3, IN.TimeParameters.x, float(6.2), _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1, _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Value_1_Float, _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float);
            float _Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float;
            Unity_Smoothstep_float(float(-0.41), float(1), _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float, _Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float);
            float _Property_82bfb359e6494ff08a54e8c08976a6f1_Out_0_Float = _FresnelPower;
            float _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_82bfb359e6494ff08a54e8c08976a6f1_Out_0_Float, _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float);
            float _Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float, _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float, _Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float);
            float4 _Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_e0f9565d1eaa452fb638faa34cc517eb_Out_0_Vector4, (_Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float.xxxx), _Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4);
            float3 _EmissionNode_87074dafd5ff4d39bf4005b1089a56e4_Output_0_Vector3 = Unity_HDRP_GetEmissionHDRColor_float((_Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4.xyz).xyz, float(3), float(0));
            float4 _Add_0b088d988049415c84334d987e692959_Out_2_Vector4;
            Unity_Add_float4(_Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4, (_Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float.xxxx), _Add_0b088d988049415c84334d987e692959_Out_2_Vector4);
            surface.BaseColor = (_Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4.xyz);
            surface.Emission = _EmissionNode_87074dafd5ff4d39bf4005b1089a56e4_Output_0_Vector3;
            surface.Alpha = (_Add_0b088d988049415c84334d987e692959_Out_2_Vector4).x;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES AttributesMesh
            #define VaryingsMeshType VaryingsMeshToPS
            #define VFX_SRP_VARYINGS VaryingsMeshType
            #define VFX_SRP_SURFACE_INPUTS FragInputs
            #endif
            
        VertexDescriptionInputs AttributesMeshToVertexDescriptionInputs(AttributesMesh input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else
        #endif
        
            return output;
        }
        
        VertexDescription GetVertexDescription(AttributesMesh input, float3 timeParameters
        #ifdef HAVE_VFX_MODIFICATION
            , AttributesElement element
        #endif
        )
        {
            // build graph inputs
            VertexDescriptionInputs vertexDescriptionInputs = AttributesMeshToVertexDescriptionInputs(input);
            // Override time parameters with used one (This is required to correctly handle motion vectors for vertex animation based on time)
        
            // evaluate vertex graph
        #ifdef HAVE_VFX_MODIFICATION
            GraphProperties properties;
            ZERO_INITIALIZE(GraphProperties, properties);
        
            // Fetch the vertex graph properties for the particle instance.
            GetElementVertexProperties(element, properties);
        
            VertexDescription vertexDescription = VertexDescriptionFunction(vertexDescriptionInputs, properties);
        #else
            VertexDescription vertexDescription = VertexDescriptionFunction(vertexDescriptionInputs);
        #endif
            return vertexDescription;
        
        }
        
        AttributesMesh ApplyMeshModification(AttributesMesh input, float3 timeParameters
        #ifdef USE_CUSTOMINTERP_SUBSTRUCT
            #ifdef TESSELLATION_ON
            , inout VaryingsMeshToDS varyings
            #else
            , inout VaryingsMeshToPS varyings
            #endif
        #endif
        #ifdef HAVE_VFX_MODIFICATION
                , AttributesElement element
        #endif
            )
        {
            VertexDescription vertexDescription = GetVertexDescription(input, timeParameters
        #ifdef HAVE_VFX_MODIFICATION
                , element
        #endif
            );
        
            // copy graph output to the results
            input.positionOS = vertexDescription.Position;
            input.normalOS = vertexDescription.Normal;
            input.tangentOS.xyz = vertexDescription.Tangent;
        
            
        
            return input;
        }
        
        #if defined(_ADD_CUSTOM_VELOCITY) // For shader graph custom velocity
        // Return precomputed Velocity in object space
        float3 GetCustomVelocity(AttributesMesh input
        #ifdef HAVE_VFX_MODIFICATION
            , AttributesElement element
        #endif
        )
        {
            VertexDescription vertexDescription = GetVertexDescription(input, _TimeParameters.xyz
        #ifdef HAVE_VFX_MODIFICATION
                , element
        #endif
            );
            return vertexDescription.CustomVelocity;
        }
        #endif
        
        FragInputs BuildFragInputs(VaryingsMeshToPS input)
        {
            FragInputs output;
            ZERO_INITIALIZE(FragInputs, output);
        
            // Init to some default value to make the computer quiet (else it output 'divide by zero' warning even if value is not used).
            // TODO: this is a really poor workaround, but the variable is used in a bunch of places
            // to compute normals which are then passed on elsewhere to compute other values...
            output.tangentToWorld = k_identity3x3;
            output.positionSS = input.positionCS;       // input.positionCS is SV_Position
        
            output.positionRWS =                input.positionRWS;
            output.tangentToWorld =             BuildTangentToWorld(input.tangentWS, input.normalWS);
            output.texCoord0 =                  input.texCoord0;
        
        #if UNITY_ANY_INSTANCING_ENABLED
        #else
        #endif
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
        
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            // splice point to copy custom interpolator fields from varyings to frag inputs
            
        
            return output;
        }
        
        // existing HDRP code uses the combined function to go directly from packed to frag inputs
        FragInputs UnpackVaryingsMeshToFragInputs(PackedVaryingsMeshToPS input)
        {
            UNITY_SETUP_INSTANCE_ID(input);
        #if defined(HAVE_VFX_MODIFICATION) && defined(UNITY_INSTANCING_ENABLED)
            unity_InstanceID = input.instanceID;
        #endif
            VaryingsMeshToPS unpacked = UnpackVaryingsMeshToPS(input);
            return BuildFragInputs(unpacked);
        }
            SurfaceDescriptionInputs FragInputsToSurfaceDescriptionInputs(FragInputs input, float3 viewWS)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            output.WorldSpaceNormal =                           normalize(input.tangentToWorld[2].xyz);
            #if defined(SHADER_STAGE_RAY_TRACING)
            output.ObjectSpaceNormal =                          mul(output.WorldSpaceNormal, (float3x3) ObjectToWorld3x4());
            #else
            output.ObjectSpaceNormal =                          normalize(mul(output.WorldSpaceNormal, (float3x3) UNITY_MATRIX_M));           // transposed multiplication by inverse matrix to handle normal scale
            #endif
            output.WorldSpaceViewDirection =                    normalize(viewWS);
            output.ObjectSpaceViewDirection =                   TransformWorldToObjectDir(output.WorldSpaceViewDirection);
        #if SHADERPASS != SHADERPASS_FOG_VOLUME_VOXELIZATION
        #else
        #endif
            output.AbsoluteWorldSpacePosition =                 GetAbsolutePositionWS(input.positionRWS);
        #if SHADERPASS != SHADERPASS_FOG_VOLUME_VOXELIZATION
        #else
        #endif
        
        #if UNITY_UV_STARTS_AT_TOP
        #else
        #endif
        
        
            output.uv0 =                                        input.texCoord0;
            output.TimeParameters =                             _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        
            // splice point to copy frag inputs custom interpolator pack into the SDI
            
        
            return output;
        }
        
            // --------------------------------------------------
            // Build Surface Data (Specific Material)
        
        void ApplyDecalToSurfaceDataNoNormal(DecalSurfaceData decalSurfaceData, inout SurfaceData surfaceData);
        
        void ApplyDecalAndGetNormal(FragInputs fragInputs, PositionInputs posInput, SurfaceDescription surfaceDescription,
            inout SurfaceData surfaceData)
        {
            float3 doubleSidedConstants = GetDoubleSidedConstants();
        
        #ifdef DECAL_NORMAL_BLENDING
            // SG nodes don't ouptut surface gradients, so if decals require surf grad blending, we have to convert
            // the normal to gradient before applying the decal. We then have to resolve the gradient back to world space
            float3 normalTS;
        
        
        
            #if HAVE_DECALS
            if (_EnableDecals)
            {
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs, alpha);
                ApplyDecalToSurfaceNormal(decalSurfaceData, fragInputs.tangentToWorld[2], normalTS);
                ApplyDecalToSurfaceDataNoNormal(decalSurfaceData, surfaceData);
            }
            #endif
        
            GetNormalWS_SG(fragInputs, normalTS, surfaceData.normalWS, doubleSidedConstants);
        #else
            // normal delivered to master node
        
            #if HAVE_DECALS
            if (_EnableDecals)
            {
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                // Both uses and modifies 'surfaceData.normalWS'.
                DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs, alpha);
                ApplyDecalToSurfaceNormal(decalSurfaceData, surfaceData.normalWS.xyz);
                ApplyDecalToSurfaceDataNoNormal(decalSurfaceData, surfaceData);
            }
            #endif
        #endif
        }
        void BuildSurfaceData(FragInputs fragInputs, inout SurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData)
        {
            // setup defaults -- these are used if the graph doesn't output a value
            ZERO_INITIALIZE(SurfaceData, surfaceData);
        
            // copy across graph values, if defined
            surfaceData.color = surfaceDescription.BaseColor;
        
            #ifdef WRITE_NORMAL_BUFFER
            // When we need to export the normal (in the depth prepass, we write the geometry one)
            surfaceData.normalWS = fragInputs.tangentToWorld[2];
            #endif
        
            #if defined(DEBUG_DISPLAY)
            #if !defined(SHADER_STAGE_RAY_TRACING)
            // Mipmap mode debugging isn't supported with ray tracing as it relies on derivatives
                if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                {
                    #ifdef FRAG_INPUTS_USE_TEXCOORD0
                        surfaceData.color = GET_TEXTURE_STREAMING_DEBUG(posInput.positionSS, fragInputs.texCoord0);
                    #else
                        surfaceData.color = GET_TEXTURE_STREAMING_DEBUG_NO_UV(posInput.positionSS);
                    #endif
                }
            #endif
            #endif
        
            #ifdef _ENABLE_SHADOW_MATTE
        
                #if (SHADERPASS == SHADERPASS_FORWARD_UNLIT) || (SHADERPASS == SHADERPASS_RAYTRACING_GBUFFER) || (SHADERPASS == SHADERPASS_RAYTRACING_INDIRECT) || (SHADERPASS == SHADERPASS_RAYTRACING_FORWARD)
        
                    HDShadowContext shadowContext = InitShadowContext();
        
                    // Evaluate the shadow, the normal is guaranteed if shadow matte is enabled on this shader.
                    float3 shadow3;
                    ShadowLoopMin(shadowContext, posInput, normalize(fragInputs.tangentToWorld[2]), asuint(_ShadowMatteFilter), GetMeshRenderingLayerMask(), shadow3);
        
                    // Compute the average value in the fourth channel
                    float4 shadow = float4(shadow3, dot(shadow3, float3(1.0/3.0, 1.0/3.0, 1.0/3.0)));
        
                    float4 shadowColor = (1.0 - shadow) * surfaceDescription.ShadowTint.rgba;
                    float  localAlpha  = saturate(shadowColor.a + surfaceDescription.Alpha);
        
                    // Keep the nested lerp
                    // With no Color (bsdfData.color.rgb, bsdfData.color.a == 0.0f), just use ShadowColor*Color to avoid a ring of "white" around the shadow
                    // And mix color to consider the Color & ShadowColor alpha (from texture or/and color picker)
                    #ifdef _SURFACE_TYPE_TRANSPARENT
                        surfaceData.color = lerp(shadowColor.rgb * surfaceData.color, lerp(lerp(shadowColor.rgb, surfaceData.color, 1.0 - surfaceDescription.ShadowTint.a), surfaceData.color, shadow.rgb), surfaceDescription.Alpha);
                    #else
                        surfaceData.color = lerp(lerp(shadowColor.rgb, surfaceData.color, 1.0 - surfaceDescription.ShadowTint.a), surfaceData.color, shadow.rgb);
                    #endif
                    localAlpha = ApplyBlendMode(surfaceData.color, localAlpha).a;
        
                    surfaceDescription.Alpha = localAlpha;
        
                #elif SHADERPASS == SHADERPASS_PATH_TRACING
        
                    surfaceData.normalWS = fragInputs.tangentToWorld[2];
                    surfaceData.shadowTint = surfaceDescription.ShadowTint.rgba;
        
                #endif
        
            #endif // _ENABLE_SHADOW_MATTE
        }
        
            // --------------------------------------------------
            // Get Surface And BuiltinData
        
            void GetSurfaceAndBuiltinData(FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData RAY_TRACING_OPTIONAL_PARAMETERS)
            {
                // Don't dither if displaced tessellation (we're fading out the displacement instead to match the next LOD)
                #if !defined(SHADER_STAGE_RAY_TRACING) && !defined(_TESSELLATION_DISPLACEMENT)
                #ifdef LOD_FADE_CROSSFADE // enable dithering LOD transition if user select CrossFade transition in LOD group
                LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                #endif
                #endif
        
                #ifndef SHADER_UNLIT
                #ifdef _DOUBLESIDED_ON
                    float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
                #else
                    float3 doubleSidedConstants = float3(1.0, 1.0, 1.0);
                #endif
        
                ApplyDoubleSidedFlipOrMirror(fragInputs, doubleSidedConstants); // Apply double sided flip on the vertex normal
                #endif // SHADER_UNLIT
        
                SurfaceDescriptionInputs surfaceDescriptionInputs = FragInputsToSurfaceDescriptionInputs(fragInputs, V);
        
                #if defined(HAVE_VFX_MODIFICATION)
                GraphProperties properties;
                ZERO_INITIALIZE(GraphProperties, properties);
        
                GetElementPixelProperties(fragInputs, properties);
        
                SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs, properties);
                #else
                SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);
                #endif
        
                #ifdef DEBUG_DISPLAY
                if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                {
                    surfaceDescription.Alpha = 1.0f;
                }
                #endif
        
                // Perform alpha test very early to save performance (a killed pixel will not sample textures)
                // TODO: split graph evaluation to grab just alpha dependencies first? tricky..
                #ifdef _ALPHATEST_ON
                    float alphaCutoff = surfaceDescription.AlphaClipThreshold;
                    #if SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_PREPASS
                    // The TransparentDepthPrepass is also used with SSR transparent.
                    // If an artists enable transaprent SSR but not the TransparentDepthPrepass itself, then we use AlphaClipThreshold
                    // otherwise if TransparentDepthPrepass is enabled we use AlphaClipThresholdDepthPrepass
                    #elif SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_POSTPASS
                    // DepthPostpass always use its own alpha threshold
                    alphaCutoff = surfaceDescription.AlphaClipThresholdDepthPostpass;
                    #elif (SHADERPASS == SHADERPASS_SHADOWS) || (SHADERPASS == SHADERPASS_RAYTRACING_VISIBILITY)
                    // If use shadow threshold isn't enable we don't allow any test
                    #endif
        
                    GENERIC_ALPHA_TEST(surfaceDescription.Alpha, alphaCutoff);
                #endif
        
                #if !defined(SHADER_STAGE_RAY_TRACING) && _DEPTHOFFSET_ON
                ApplyDepthOffsetPositionInput(V, surfaceDescription.DepthOffset, GetViewForwardDir(), GetWorldToHClipMatrix(), posInput);
                #endif
        
                #ifndef SHADER_UNLIT
                float3 bentNormalWS;
                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData, bentNormalWS);
        
                #ifdef FRAG_INPUTS_USE_TEXCOORD1
                    float4 lightmapTexCoord1 = fragInputs.texCoord1;
                #else
                    float4 lightmapTexCoord1 = float4(0,0,0,0);
                #endif
        
                #ifdef FRAG_INPUTS_USE_TEXCOORD2
                    float4 lightmapTexCoord2 = fragInputs.texCoord2;
                #else
                    float4 lightmapTexCoord2 = float4(0,0,0,0);
                #endif
        
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                // Builtin Data
                // For back lighting we use the oposite vertex normal
                InitBuiltinData(posInput, alpha, bentNormalWS, -fragInputs.tangentToWorld[2], lightmapTexCoord1, lightmapTexCoord2, builtinData);
        
                #else
                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData);
        
                ZERO_BUILTIN_INITIALIZE(builtinData); // No call to InitBuiltinData as we don't have any lighting
                builtinData.opacity = surfaceDescription.Alpha;
        
                #if defined(DEBUG_DISPLAY)
                    // Light Layers are currently not used for the Unlit shader (because it is not lit)
                    // But Unlit objects do cast shadows according to their rendering layer mask, which is what we want to
                    // display in the light layers visualization mode, therefore we need the renderingLayers
                    builtinData.renderingLayers = GetMeshRenderingLayerMask();
                #endif
        
                #endif // SHADER_UNLIT
        
                #ifdef _ALPHATEST_ON
                    // Used for sharpening by alpha to mask - Alpha to covertage is only used with depth only and forward pass (no shadow pass, no transparent pass)
                    builtinData.alphaClipTreshold = alphaCutoff;
                #endif
        
                // override sampleBakedGI - not used by Unlit
        		// When overriding GI, we need to force the isLightmap flag to make sure we don't add APV (sampled in the lightloop) on top of the overridden value (set at GBuffer stage)
        
                builtinData.emissiveColor = surfaceDescription.Emission;
        
                // Note this will not fully work on transparent surfaces (can check with _SURFACE_TYPE_TRANSPARENT define)
                // We will always overwrite vt feeback with the nearest. So behind transparent surfaces vt will not be resolved
                // This is a limitation of the current MRT approach.
                #ifdef UNITY_VIRTUAL_TEXTURING
                #endif
        
                #if _DEPTHOFFSET_ON
                builtinData.depthOffset = surfaceDescription.DepthOffset;
                #endif
        
                // TODO: We should generate distortion / distortionBlur for non distortion pass
                #if (SHADERPASS == SHADERPASS_DISTORTION)
                builtinData.distortion = surfaceDescription.Distortion;
                builtinData.distortionBlur = surfaceDescription.DistortionBlur;
                #endif
        
                #ifndef SHADER_UNLIT
                // PostInitBuiltinData call ApplyDebugToBuiltinData
                PostInitBuiltinData(V, posInput, surfaceData, builtinData);
                #else
                ApplyDebugToBuiltinData(builtinData);
                #endif
        
                RAY_TRACING_OPTIONAL_ALPHA_TEST_PASS
            }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassDepthOnly.hlsl"
        
            // --------------------------------------------------
            // Visual Effect Vertex Invocations
        
        	#ifdef HAVE_VFX_MODIFICATION
                #if !defined(SHADER_STAGE_RAY_TRACING)
        	    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/VisualEffectVertex.hlsl"
                #else
                #endif
        	#endif
        
            ENDHLSL
        }
        Pass
        {
            Name "ForwardOnly"
            Tags
            {
                "LightMode" = "ForwardOnly"
            }
        
            // Render State
            Cull [_CullModeForward]
        Blend [_SrcBlend] [_DstBlend], [_AlphaSrcBlend] [_AlphaDstBlend]
        Blend 1 One OneMinusSrcAlpha
        Blend 2 One [_DstBlend2]
        Blend 3 One [_DstBlend2]
        Blend 4 One OneMinusSrcAlpha
        ZTest [_ZTestDepthEqualForOpaque]
        ZWrite [_ZWrite]
        ColorMask [_ColorMaskTransparentVelOne] 1
        ColorMask [_ColorMaskTransparentVelTwo] 2
        Stencil
        {
        ReadMask 128
        WriteMask 0
        Ref 128
        CompFront Equal
        PassFront Keep
        CompBack Equal
        PassBack Keep
        Fail Keep
        ZFail Keep
        }
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma instancing_options renderinglayer
        #pragma target 4.5
        #pragma vertex Vert
        #pragma fragment Frag
        #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch
        #pragma multi_compile_instancing
        
            // Keywords
            #pragma shader_feature _ _SURFACE_TYPE_TRANSPARENT
        #pragma shader_feature_local _ _ADD_PRECOMPUTED_VELOCITY
        #pragma shader_feature_local _ _TRANSPARENT_WRITES_MOTION_VEC _TRANSPARENT_REFRACTIVE_SORT
        #pragma shader_feature_local_fragment _ _ENABLE_FOG_ON_TRANSPARENT
        #pragma multi_compile _ DEBUG_DISPLAY
            // GraphKeywords: <None>
        
            // Defines
            #define SHADERPASS SHADERPASS_FORWARD_UNLIT
        #define RAYTRACING_SHADER_GRAPH_DEFAULT
        #define SUPPORT_GLOBAL_MIP_BIAS 1
        
            // For custom interpolators to inject a substruct definition before FragInputs definition,
            // allowing for FragInputs to capture CI's intended for ShaderGraph's SDI.
            struct CustomInterpolators
        {
        };
        #define USE_CUSTOMINTERP_SUBSTRUCT
        
        
        
            // TODO: Merge FragInputsVFX substruct with CustomInterpolators.
        	#ifdef HAVE_VFX_MODIFICATION
        	struct FragInputsVFX
            {
                /* WARNING: $splice Could not find named fragment 'FragInputsVFX' */
            };
            #endif
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl" // Required by Tessellation.hlsl
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl" // Required to be include before we include properties as it define DECLARE_STACK_CB
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl" // Required before including properties as it defines UNITY_TEXTURE_STREAMING_DEBUG_VARS
            // Always include Shader Graph version
            // Always include last to avoid double macros
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl" // Need to be here for Gradient struct definition
        
            // --------------------------------------------------
            // Defines
        
            // Attribute
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TANGENT_TO_WORLD
            #define VARYINGS_NEED_TEXCOORD0
        
            #define HAVE_MESH_MODIFICATION
        
            //Strip down the FragInputs.hlsl (on graphics), so we can only optimize the interpolators we use.
            //if by accident something requests contents of FragInputs.hlsl, it will be caught as a compiler error
            //Frag inputs stripping is only enabled when FRAG_INPUTS_ENABLE_STRIPPING is set
            #if !defined(SHADER_STAGE_RAY_TRACING) && SHADERPASS != SHADERPASS_RAYTRACING_GBUFFER && SHADERPASS != SHADERPASS_FULL_SCREEN_DEBUG
            #define FRAG_INPUTS_ENABLE_STRIPPING
            #endif
            #define FRAG_INPUTS_USE_TEXCOORD0
        
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
        
        
        
        
            // Following two define are a workaround introduce in 10.1.x for RaytracingQualityNode
            // The ShaderGraph don't support correctly migration of this node as it serialize all the node data
            // in the json file making it impossible to uprgrade. Until we get a fix, we do a workaround here
            // to still allow us to rename the field and keyword of this node without breaking existing code.
            #ifdef RAYTRACING_SHADER_GRAPH_DEFAULT
            #define RAYTRACING_SHADER_GRAPH_HIGH
            #endif
        
            #ifdef RAYTRACING_SHADER_GRAPH_RAYTRACED
            #define RAYTRACING_SHADER_GRAPH_LOW
            #endif
            // end
        
            #ifndef SHADER_UNLIT
            // We need isFrontFace when using double sided - it is not required for unlit as in case of unlit double sided only drive the cullmode
            // VARYINGS_NEED_CULLFACE can be define by VaryingsMeshToPS.FaceSign input if a IsFrontFace Node is included in the shader graph.
            #if defined(_DOUBLESIDED_ON) && !defined(VARYINGS_NEED_CULLFACE)
                #define VARYINGS_NEED_CULLFACE
            #endif
            #endif
        
            // Specific Material Define
        // Setup a define to say we are an unlit shader
        #define SHADER_UNLIT
        
        // Following Macro are only used by Unlit material
        #if defined(_ENABLE_SHADOW_MATTE)
            #if SHADERPASS == SHADERPASS_FORWARD_UNLIT
                #pragma multi_compile_fragment USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST
            #elif SHADERPASS == SHADERPASS_PATH_TRACING
                #define LIGHTLOOP_DISABLE_TILE_AND_CLUSTER
            #endif
        
        // We don't want to have the lightloop defined for the ray tracing passes, but we do for the rasterisation and path tracing shader passes.
        #if !defined(SHADER_STAGE_RAY_TRACING) || SHADERPASS == SHADERPASS_PATH_TRACING
            #define HAS_LIGHTLOOP
        #endif
        #endif
            // Caution: we can use the define SHADER_UNLIT onlit after the above Material include as it is the Unlit template who define it
        
            // To handle SSR on transparent correctly with a possibility to enable/disable it per framesettings
            // we should have a code like this:
            // if !defined(_DISABLE_SSR_TRANSPARENT)
            // pragma multi_compile _ WRITE_NORMAL_BUFFER
            // endif
            // i.e we enable the multicompile only if we can receive SSR or not, and then C# code drive
            // it based on if SSR transparent in frame settings and not (and stripper can strip it).
            // this is currently not possible with our current preprocessor as _DISABLE_SSR_TRANSPARENT is a keyword not a define
            // so instead we used this and chose to pay the extra cost of normal write even if SSR transaprent is disabled.
            // Ideally the shader graph generator should handle it but condition below can't be handle correctly for now.
            #if SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_PREPASS
            #if !defined(_DISABLE_SSR_TRANSPARENT) && !defined(SHADER_UNLIT)
                #define WRITE_NORMAL_BUFFER
            #endif
            #endif
        
            // See Lit.shader
            #if SHADERPASS == SHADERPASS_MOTION_VECTORS && defined(WRITE_DECAL_BUFFER_AND_RENDERING_LAYER)
                #define WRITE_DECAL_BUFFER
            #endif
        
            #ifndef DEBUG_DISPLAY
                // In case of opaque we don't want to perform the alpha test, it is done in depth prepass and we use depth equal for ztest (setup from UI)
                // Don't do it with debug display mode as it is possible there is no depth prepass in this case
                #if !defined(_SURFACE_TYPE_TRANSPARENT)
                    #if SHADERPASS == SHADERPASS_FORWARD
                    #define SHADERPASS_FORWARD_BYPASS_ALPHA_TEST
                    #elif SHADERPASS == SHADERPASS_GBUFFER
                    #define SHADERPASS_GBUFFER_BYPASS_ALPHA_TEST
                    #endif
                #endif
            #endif
        
            // Define _DEFERRED_CAPABLE_MATERIAL for shader capable to run in deferred pass
            #if defined(SHADER_LIT) && !defined(_SURFACE_TYPE_TRANSPARENT)
                #define _DEFERRED_CAPABLE_MATERIAL
            #endif
        
            // Translate transparent motion vector define
            #if (defined(_TRANSPARENT_WRITES_MOTION_VEC) || defined(_TRANSPARENT_REFRACTIVE_SORT)) && defined(_SURFACE_TYPE_TRANSPARENT)
                #define _WRITE_TRANSPARENT_MOTION_VECTOR
            #endif
        
            // -- Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _VoidColor;
        float _Speed;
        float _FresnelPower;
        float4 _BorderColor;
        float2 _Seed;
        float4 _TwirlTexture_TexelSize;
        float _InnerEdge;
        float4 _EmissionColor;
        float _UseShadowThreshold;
        float4 _DoubleSidedConstants;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        float _BlendMode;
        float _EnableBlendModePreserveSpecularLighting;
        CBUFFER_END
        
        
        // Object and Global properties
        TEXTURE2D(_TwirlTexture);
        SAMPLER(sampler_TwirlTexture);
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Includes
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Debug/DebugDisplay.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/Unlit.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/ClassicNoise3D.hlsl"
        #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi3D.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonLighting.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            struct AttributesMesh
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct VaryingsMeshToPS
        {
            SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
             float3 positionRWS;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct SurfaceDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 WorldSpaceNormal;
             float3 ObjectSpaceViewDirection;
             float3 WorldSpaceViewDirection;
             float3 AbsoluteWorldSpacePosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct PackedVaryingsMeshToPS
        {
            SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
             float4 tangentWS : INTERP0;
             float4 texCoord0 : INTERP1;
             float3 positionRWS : INTERP2;
             float3 normalWS : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
        };
        
            PackedVaryingsMeshToPS PackVaryingsMeshToPS (VaryingsMeshToPS input)
        {
            PackedVaryingsMeshToPS output;
            ZERO_INITIALIZE(PackedVaryingsMeshToPS, output);
            output.positionCS = input.positionCS;
            output.tangentWS.xyzw = input.tangentWS;
            output.texCoord0.xyzw = input.texCoord0;
            output.positionRWS.xyz = input.positionRWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            return output;
        }
        
        VaryingsMeshToPS UnpackVaryingsMeshToPS (PackedVaryingsMeshToPS input)
        {
            VaryingsMeshToPS output;
            output.positionCS = input.positionCS;
            output.tangentWS = input.tangentWS.xyzw;
            output.texCoord0 = input.texCoord0.xyzw;
            output.positionRWS = input.positionRWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
        
            // Graph Functions
            
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_DotProduct_float3(float3 A, float3 B, out float Out)
        {
            Out = dot(A, B);
        }
        
        void Unity_SampleGradientV1_float(Gradient Gradient, float Time, out float4 Out)
        {
            // convert to OkLab if we need perceptual color space.
            float3 color = lerp(Gradient.colors[0].rgb, LinearToOklab(Gradient.colors[0].rgb), Gradient.type == 2);
        
            [unroll]
            for (int c = 1; c < Gradient.colorsLength; c++)
            {
                float colorPos = saturate((Time - Gradient.colors[c - 1].w) / (Gradient.colors[c].w - Gradient.colors[c - 1].w)) * step(c, Gradient.colorsLength - 1);
                float3 color2 = lerp(Gradient.colors[c].rgb, LinearToOklab(Gradient.colors[c].rgb), Gradient.type == 2);
                color = lerp(color, color2, lerp(colorPos, step(0.01, colorPos), Gradient.type % 2)); // grad.type == 1 is fixed, 0 and 2 are blends.
            }
            color = lerp(color, OklabToLinear(color), Gradient.type == 2);
        
        #ifdef UNITY_COLORSPACE_GAMMA
            color = LinearToSRGB(color);
        #endif
        
            float alpha = Gradient.alphas[0].x;
            [unroll]
            for (int a = 1; a < Gradient.alphasLength; a++)
            {
                float alphaPos = saturate((Time - Gradient.alphas[a - 1].y) / (Gradient.alphas[a].y - Gradient.alphas[a - 1].y)) * step(a, Gradient.alphasLength - 1);
                alpha = lerp(alpha, Gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), Gradient.type % 2));
            }
        
            Out = float4(color, alpha);
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
        Out = A * B;
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        struct Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float
        {
        };
        
        void SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(float3 Vector3_7940555B, float Vector1_1B8B9078, Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float IN, out float Value_0)
        {
        float3 _Property_44999cc87708de82a26b39ae1da975ec_Out_0_Vector3 = Vector3_7940555B;
        float _Property_dad5add45a7fa785be976f925bc5a5da_Out_0_Float = Vector1_1B8B9078;
        float3 _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3;
        Unity_Multiply_float3_float3(_Property_44999cc87708de82a26b39ae1da975ec_Out_0_Vector3, (_Property_dad5add45a7fa785be976f925bc5a5da_Out_0_Float.xxx), _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3);
        float _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float;
        PerlinNoise3D_float(_Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3, _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float);
        float _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float;
        Unity_Remap_float(_PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float, float2 (-1.15, 1.15), float2 (0, 1), _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float);
        Value_0 = _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_RandomRange_float(float2 Seed, float Min, float Max, out float Out)
        {
             float randomno =  frac(sin(dot(Seed, float2(12.9898, 78.233)))*43758.5453);
             Out = lerp(Min, Max, randomno);
        }
        
        struct Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float
        {
        };
        
        void SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(float3 Vector3_375394F7, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float IN, out float Value_1, out float Cells_2)
        {
        float3 _Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3 = Vector3_375394F7;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        VoronoiPrecise3D_float(_Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }
        
        float3 Unity_HDRP_GetEmissionHDRColor_float(float3 ldrColor, float luminanceIntensity, float exposureWeight)
        {
            float3 hdrColor = ldrColor * luminanceIntensity;
        
            #ifdef SHADERGRAPH_PREVIEW
            float inverseExposureMultiplier = 1.0;
            #else
            float inverseExposureMultiplier = GetInverseCurrentExposureMultiplier();
            #endif
        
            // Inverse pre-expose using _EmissiveExposureWeight weight
            hdrColor = lerp(hdrColor * inverseExposureMultiplier, hdrColor, exposureWeight);
            return hdrColor;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float3 Emission;
            float Alpha;
            float4 VTPackedFeedback;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_d09e7f2c2a774dbd93cc943fa6a50940_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_VoidColor) : _VoidColor;
            float _Property_16eef79ff58443149895b35d0a88bdb2_Out_0_Float = _InnerEdge;
            float4 _UV_ce55b36e72754a339073e4c05a64b551_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_054563945a0a4f688fce11f0c067b0c8_Out_1_Vector2 = _UV_ce55b36e72754a339073e4c05a64b551_Out_0_Vector4.xy;
            float _Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float;
            Unity_Distance_float2(_Swizzle_054563945a0a4f688fce11f0c067b0c8_Out_1_Vector2, float2(0.5, 0.5), _Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float);
            float _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float;
            Unity_OneMinus_float(_Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float, _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float);
            float _Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float;
            Unity_Smoothstep_float(_Property_16eef79ff58443149895b35d0a88bdb2_Out_0_Float, float(1), _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float, _Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float);
            Gradient _Gradient_c34cf660310448768ec81f4600b23ad8_Out_0_Gradient = NewGradient(0, 4, 2, float4(0, 0, 0, 0.271931),float4(0.01568037, 0.01568037, 0.01568037, 0.4678416),float4(0.1071531, 0.1071531, 0.1071531, 0.6052644),float4(1, 1, 1, 1),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0), float2(1, 0),float2(1, 1),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0));
            float _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float;
            Unity_DotProduct_float3(IN.ObjectSpaceViewDirection, IN.ObjectSpaceNormal, _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float);
            float4 _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4;
            Unity_SampleGradientV1_float(_Gradient_c34cf660310448768ec81f4600b23ad8_Out_0_Gradient, _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float, _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4);
            float4 _Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float.xxxx), _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4, _Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4);
            Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a;
            float _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float;
            SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(IN.ObjectSpaceViewDirection, float(10), _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a, _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float);
            float3 _Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3;
            Unity_Multiply_float3_float3(IN.ObjectSpaceViewDirection, (_Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float.xxx), _Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3);
            float _Property_ba2d1376c19c40fbadc820d0fb721b2e_Out_0_Float = _Speed;
            float _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_ba2d1376c19c40fbadc820d0fb721b2e_Out_0_Float, _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float);
            float2 _Property_2cfd9c61936548ee85383d83df6beba0_Out_0_Vector2 = _Seed;
            float _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float;
            Unity_RandomRange_float(_Property_2cfd9c61936548ee85383d83df6beba0_Out_0_Vector2, float(26.96), float(45.6), _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float);
            Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a;
            float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Value_1_Float;
            float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float;
            SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(_Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3, _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float, _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Value_1_Float, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float);
            float4 _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4, (_Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float.xxxx), _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4);
            float4 _Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_d09e7f2c2a774dbd93cc943fa6a50940_Out_0_Vector4, _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4, _Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4);
            float4 _Property_e0f9565d1eaa452fb638faa34cc517eb_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_BorderColor) : _BorderColor;
            float3 _Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3;
            Unity_Absolute_float3(IN.AbsoluteWorldSpacePosition, _Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3);
            Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1;
            float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Value_1_Float;
            float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float;
            SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(_Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3, IN.TimeParameters.x, float(6.2), _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1, _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Value_1_Float, _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float);
            float _Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float;
            Unity_Smoothstep_float(float(-0.41), float(1), _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float, _Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float);
            float _Property_82bfb359e6494ff08a54e8c08976a6f1_Out_0_Float = _FresnelPower;
            float _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_82bfb359e6494ff08a54e8c08976a6f1_Out_0_Float, _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float);
            float _Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float, _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float, _Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float);
            float4 _Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_e0f9565d1eaa452fb638faa34cc517eb_Out_0_Vector4, (_Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float.xxxx), _Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4);
            float3 _EmissionNode_87074dafd5ff4d39bf4005b1089a56e4_Output_0_Vector3 = Unity_HDRP_GetEmissionHDRColor_float((_Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4.xyz).xyz, float(3), float(0));
            float4 _Add_0b088d988049415c84334d987e692959_Out_2_Vector4;
            Unity_Add_float4(_Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4, (_Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float.xxxx), _Add_0b088d988049415c84334d987e692959_Out_2_Vector4);
            surface.BaseColor = (_Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4.xyz);
            surface.Emission = _EmissionNode_87074dafd5ff4d39bf4005b1089a56e4_Output_0_Vector3;
            surface.Alpha = (_Add_0b088d988049415c84334d987e692959_Out_2_Vector4).x;
            {
                surface.VTPackedFeedback = float4(1.0f,1.0f,1.0f,1.0f);
            }
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES AttributesMesh
            #define VaryingsMeshType VaryingsMeshToPS
            #define VFX_SRP_VARYINGS VaryingsMeshType
            #define VFX_SRP_SURFACE_INPUTS FragInputs
            #endif
            
        VertexDescriptionInputs AttributesMeshToVertexDescriptionInputs(AttributesMesh input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else
        #endif
        
            return output;
        }
        
        VertexDescription GetVertexDescription(AttributesMesh input, float3 timeParameters
        #ifdef HAVE_VFX_MODIFICATION
            , AttributesElement element
        #endif
        )
        {
            // build graph inputs
            VertexDescriptionInputs vertexDescriptionInputs = AttributesMeshToVertexDescriptionInputs(input);
            // Override time parameters with used one (This is required to correctly handle motion vectors for vertex animation based on time)
        
            // evaluate vertex graph
        #ifdef HAVE_VFX_MODIFICATION
            GraphProperties properties;
            ZERO_INITIALIZE(GraphProperties, properties);
        
            // Fetch the vertex graph properties for the particle instance.
            GetElementVertexProperties(element, properties);
        
            VertexDescription vertexDescription = VertexDescriptionFunction(vertexDescriptionInputs, properties);
        #else
            VertexDescription vertexDescription = VertexDescriptionFunction(vertexDescriptionInputs);
        #endif
            return vertexDescription;
        
        }
        
        AttributesMesh ApplyMeshModification(AttributesMesh input, float3 timeParameters
        #ifdef USE_CUSTOMINTERP_SUBSTRUCT
            #ifdef TESSELLATION_ON
            , inout VaryingsMeshToDS varyings
            #else
            , inout VaryingsMeshToPS varyings
            #endif
        #endif
        #ifdef HAVE_VFX_MODIFICATION
                , AttributesElement element
        #endif
            )
        {
            VertexDescription vertexDescription = GetVertexDescription(input, timeParameters
        #ifdef HAVE_VFX_MODIFICATION
                , element
        #endif
            );
        
            // copy graph output to the results
            input.positionOS = vertexDescription.Position;
            input.normalOS = vertexDescription.Normal;
            input.tangentOS.xyz = vertexDescription.Tangent;
        
            
        
            return input;
        }
        
        #if defined(_ADD_CUSTOM_VELOCITY) // For shader graph custom velocity
        // Return precomputed Velocity in object space
        float3 GetCustomVelocity(AttributesMesh input
        #ifdef HAVE_VFX_MODIFICATION
            , AttributesElement element
        #endif
        )
        {
            VertexDescription vertexDescription = GetVertexDescription(input, _TimeParameters.xyz
        #ifdef HAVE_VFX_MODIFICATION
                , element
        #endif
            );
            return vertexDescription.CustomVelocity;
        }
        #endif
        
        FragInputs BuildFragInputs(VaryingsMeshToPS input)
        {
            FragInputs output;
            ZERO_INITIALIZE(FragInputs, output);
        
            // Init to some default value to make the computer quiet (else it output 'divide by zero' warning even if value is not used).
            // TODO: this is a really poor workaround, but the variable is used in a bunch of places
            // to compute normals which are then passed on elsewhere to compute other values...
            output.tangentToWorld = k_identity3x3;
            output.positionSS = input.positionCS;       // input.positionCS is SV_Position
        
            output.positionRWS =                input.positionRWS;
            output.tangentToWorld =             BuildTangentToWorld(input.tangentWS, input.normalWS);
            output.texCoord0 =                  input.texCoord0;
        
        #if UNITY_ANY_INSTANCING_ENABLED
        #else
        #endif
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
        
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            // splice point to copy custom interpolator fields from varyings to frag inputs
            
        
            return output;
        }
        
        // existing HDRP code uses the combined function to go directly from packed to frag inputs
        FragInputs UnpackVaryingsMeshToFragInputs(PackedVaryingsMeshToPS input)
        {
            UNITY_SETUP_INSTANCE_ID(input);
        #if defined(HAVE_VFX_MODIFICATION) && defined(UNITY_INSTANCING_ENABLED)
            unity_InstanceID = input.instanceID;
        #endif
            VaryingsMeshToPS unpacked = UnpackVaryingsMeshToPS(input);
            return BuildFragInputs(unpacked);
        }
            SurfaceDescriptionInputs FragInputsToSurfaceDescriptionInputs(FragInputs input, float3 viewWS)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            output.WorldSpaceNormal =                           normalize(input.tangentToWorld[2].xyz);
            #if defined(SHADER_STAGE_RAY_TRACING)
            output.ObjectSpaceNormal =                          mul(output.WorldSpaceNormal, (float3x3) ObjectToWorld3x4());
            #else
            output.ObjectSpaceNormal =                          normalize(mul(output.WorldSpaceNormal, (float3x3) UNITY_MATRIX_M));           // transposed multiplication by inverse matrix to handle normal scale
            #endif
            output.WorldSpaceViewDirection =                    normalize(viewWS);
            output.ObjectSpaceViewDirection =                   TransformWorldToObjectDir(output.WorldSpaceViewDirection);
        #if SHADERPASS != SHADERPASS_FOG_VOLUME_VOXELIZATION
        #else
        #endif
            output.AbsoluteWorldSpacePosition =                 GetAbsolutePositionWS(input.positionRWS);
        #if SHADERPASS != SHADERPASS_FOG_VOLUME_VOXELIZATION
        #else
        #endif
        
        #if UNITY_UV_STARTS_AT_TOP
        #else
        #endif
        
        
            output.uv0 =                                        input.texCoord0;
            output.TimeParameters =                             _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        
            // splice point to copy frag inputs custom interpolator pack into the SDI
            
        
            return output;
        }
        
            // --------------------------------------------------
            // Build Surface Data (Specific Material)
        
        void ApplyDecalToSurfaceDataNoNormal(DecalSurfaceData decalSurfaceData, inout SurfaceData surfaceData);
        
        void ApplyDecalAndGetNormal(FragInputs fragInputs, PositionInputs posInput, SurfaceDescription surfaceDescription,
            inout SurfaceData surfaceData)
        {
            float3 doubleSidedConstants = GetDoubleSidedConstants();
        
        #ifdef DECAL_NORMAL_BLENDING
            // SG nodes don't ouptut surface gradients, so if decals require surf grad blending, we have to convert
            // the normal to gradient before applying the decal. We then have to resolve the gradient back to world space
            float3 normalTS;
        
        
        
            #if HAVE_DECALS
            if (_EnableDecals)
            {
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs, alpha);
                ApplyDecalToSurfaceNormal(decalSurfaceData, fragInputs.tangentToWorld[2], normalTS);
                ApplyDecalToSurfaceDataNoNormal(decalSurfaceData, surfaceData);
            }
            #endif
        
            GetNormalWS_SG(fragInputs, normalTS, surfaceData.normalWS, doubleSidedConstants);
        #else
            // normal delivered to master node
        
            #if HAVE_DECALS
            if (_EnableDecals)
            {
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                // Both uses and modifies 'surfaceData.normalWS'.
                DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs, alpha);
                ApplyDecalToSurfaceNormal(decalSurfaceData, surfaceData.normalWS.xyz);
                ApplyDecalToSurfaceDataNoNormal(decalSurfaceData, surfaceData);
            }
            #endif
        #endif
        }
        void BuildSurfaceData(FragInputs fragInputs, inout SurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData)
        {
            // setup defaults -- these are used if the graph doesn't output a value
            ZERO_INITIALIZE(SurfaceData, surfaceData);
        
            // copy across graph values, if defined
            surfaceData.color = surfaceDescription.BaseColor;
        
            #ifdef WRITE_NORMAL_BUFFER
            // When we need to export the normal (in the depth prepass, we write the geometry one)
            surfaceData.normalWS = fragInputs.tangentToWorld[2];
            #endif
        
            #if defined(DEBUG_DISPLAY)
            #if !defined(SHADER_STAGE_RAY_TRACING)
            // Mipmap mode debugging isn't supported with ray tracing as it relies on derivatives
                if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                {
                    #ifdef FRAG_INPUTS_USE_TEXCOORD0
                        surfaceData.color = GET_TEXTURE_STREAMING_DEBUG(posInput.positionSS, fragInputs.texCoord0);
                    #else
                        surfaceData.color = GET_TEXTURE_STREAMING_DEBUG_NO_UV(posInput.positionSS);
                    #endif
                }
            #endif
            #endif
        
            #ifdef _ENABLE_SHADOW_MATTE
        
                #if (SHADERPASS == SHADERPASS_FORWARD_UNLIT) || (SHADERPASS == SHADERPASS_RAYTRACING_GBUFFER) || (SHADERPASS == SHADERPASS_RAYTRACING_INDIRECT) || (SHADERPASS == SHADERPASS_RAYTRACING_FORWARD)
        
                    HDShadowContext shadowContext = InitShadowContext();
        
                    // Evaluate the shadow, the normal is guaranteed if shadow matte is enabled on this shader.
                    float3 shadow3;
                    ShadowLoopMin(shadowContext, posInput, normalize(fragInputs.tangentToWorld[2]), asuint(_ShadowMatteFilter), GetMeshRenderingLayerMask(), shadow3);
        
                    // Compute the average value in the fourth channel
                    float4 shadow = float4(shadow3, dot(shadow3, float3(1.0/3.0, 1.0/3.0, 1.0/3.0)));
        
                    float4 shadowColor = (1.0 - shadow) * surfaceDescription.ShadowTint.rgba;
                    float  localAlpha  = saturate(shadowColor.a + surfaceDescription.Alpha);
        
                    // Keep the nested lerp
                    // With no Color (bsdfData.color.rgb, bsdfData.color.a == 0.0f), just use ShadowColor*Color to avoid a ring of "white" around the shadow
                    // And mix color to consider the Color & ShadowColor alpha (from texture or/and color picker)
                    #ifdef _SURFACE_TYPE_TRANSPARENT
                        surfaceData.color = lerp(shadowColor.rgb * surfaceData.color, lerp(lerp(shadowColor.rgb, surfaceData.color, 1.0 - surfaceDescription.ShadowTint.a), surfaceData.color, shadow.rgb), surfaceDescription.Alpha);
                    #else
                        surfaceData.color = lerp(lerp(shadowColor.rgb, surfaceData.color, 1.0 - surfaceDescription.ShadowTint.a), surfaceData.color, shadow.rgb);
                    #endif
                    localAlpha = ApplyBlendMode(surfaceData.color, localAlpha).a;
        
                    surfaceDescription.Alpha = localAlpha;
        
                #elif SHADERPASS == SHADERPASS_PATH_TRACING
        
                    surfaceData.normalWS = fragInputs.tangentToWorld[2];
                    surfaceData.shadowTint = surfaceDescription.ShadowTint.rgba;
        
                #endif
        
            #endif // _ENABLE_SHADOW_MATTE
        }
        
            // --------------------------------------------------
            // Get Surface And BuiltinData
        
            void GetSurfaceAndBuiltinData(FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData RAY_TRACING_OPTIONAL_PARAMETERS)
            {
                // Don't dither if displaced tessellation (we're fading out the displacement instead to match the next LOD)
                #if !defined(SHADER_STAGE_RAY_TRACING) && !defined(_TESSELLATION_DISPLACEMENT)
                #ifdef LOD_FADE_CROSSFADE // enable dithering LOD transition if user select CrossFade transition in LOD group
                LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                #endif
                #endif
        
                #ifndef SHADER_UNLIT
                #ifdef _DOUBLESIDED_ON
                    float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
                #else
                    float3 doubleSidedConstants = float3(1.0, 1.0, 1.0);
                #endif
        
                ApplyDoubleSidedFlipOrMirror(fragInputs, doubleSidedConstants); // Apply double sided flip on the vertex normal
                #endif // SHADER_UNLIT
        
                SurfaceDescriptionInputs surfaceDescriptionInputs = FragInputsToSurfaceDescriptionInputs(fragInputs, V);
        
                #if defined(HAVE_VFX_MODIFICATION)
                GraphProperties properties;
                ZERO_INITIALIZE(GraphProperties, properties);
        
                GetElementPixelProperties(fragInputs, properties);
        
                SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs, properties);
                #else
                SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);
                #endif
        
                #ifdef DEBUG_DISPLAY
                if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                {
                    surfaceDescription.Alpha = 1.0f;
                }
                #endif
        
                // Perform alpha test very early to save performance (a killed pixel will not sample textures)
                // TODO: split graph evaluation to grab just alpha dependencies first? tricky..
                #ifdef _ALPHATEST_ON
                    float alphaCutoff = surfaceDescription.AlphaClipThreshold;
                    #if SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_PREPASS
                    // The TransparentDepthPrepass is also used with SSR transparent.
                    // If an artists enable transaprent SSR but not the TransparentDepthPrepass itself, then we use AlphaClipThreshold
                    // otherwise if TransparentDepthPrepass is enabled we use AlphaClipThresholdDepthPrepass
                    #elif SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_POSTPASS
                    // DepthPostpass always use its own alpha threshold
                    alphaCutoff = surfaceDescription.AlphaClipThresholdDepthPostpass;
                    #elif (SHADERPASS == SHADERPASS_SHADOWS) || (SHADERPASS == SHADERPASS_RAYTRACING_VISIBILITY)
                    // If use shadow threshold isn't enable we don't allow any test
                    #endif
        
                    GENERIC_ALPHA_TEST(surfaceDescription.Alpha, alphaCutoff);
                #endif
        
                #if !defined(SHADER_STAGE_RAY_TRACING) && _DEPTHOFFSET_ON
                ApplyDepthOffsetPositionInput(V, surfaceDescription.DepthOffset, GetViewForwardDir(), GetWorldToHClipMatrix(), posInput);
                #endif
        
                #ifndef SHADER_UNLIT
                float3 bentNormalWS;
                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData, bentNormalWS);
        
                #ifdef FRAG_INPUTS_USE_TEXCOORD1
                    float4 lightmapTexCoord1 = fragInputs.texCoord1;
                #else
                    float4 lightmapTexCoord1 = float4(0,0,0,0);
                #endif
        
                #ifdef FRAG_INPUTS_USE_TEXCOORD2
                    float4 lightmapTexCoord2 = fragInputs.texCoord2;
                #else
                    float4 lightmapTexCoord2 = float4(0,0,0,0);
                #endif
        
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                // Builtin Data
                // For back lighting we use the oposite vertex normal
                InitBuiltinData(posInput, alpha, bentNormalWS, -fragInputs.tangentToWorld[2], lightmapTexCoord1, lightmapTexCoord2, builtinData);
        
                #else
                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData);
        
                ZERO_BUILTIN_INITIALIZE(builtinData); // No call to InitBuiltinData as we don't have any lighting
                builtinData.opacity = surfaceDescription.Alpha;
        
                #if defined(DEBUG_DISPLAY)
                    // Light Layers are currently not used for the Unlit shader (because it is not lit)
                    // But Unlit objects do cast shadows according to their rendering layer mask, which is what we want to
                    // display in the light layers visualization mode, therefore we need the renderingLayers
                    builtinData.renderingLayers = GetMeshRenderingLayerMask();
                #endif
        
                #endif // SHADER_UNLIT
        
                #ifdef _ALPHATEST_ON
                    // Used for sharpening by alpha to mask - Alpha to covertage is only used with depth only and forward pass (no shadow pass, no transparent pass)
                    builtinData.alphaClipTreshold = alphaCutoff;
                #endif
        
                // override sampleBakedGI - not used by Unlit
        		// When overriding GI, we need to force the isLightmap flag to make sure we don't add APV (sampled in the lightloop) on top of the overridden value (set at GBuffer stage)
        
                builtinData.emissiveColor = surfaceDescription.Emission;
        
                // Note this will not fully work on transparent surfaces (can check with _SURFACE_TYPE_TRANSPARENT define)
                // We will always overwrite vt feeback with the nearest. So behind transparent surfaces vt will not be resolved
                // This is a limitation of the current MRT approach.
                #ifdef UNITY_VIRTUAL_TEXTURING
                builtinData.vtPackedFeedback = surfaceDescription.VTPackedFeedback;
                #endif
        
                #if _DEPTHOFFSET_ON
                builtinData.depthOffset = surfaceDescription.DepthOffset;
                #endif
        
                // TODO: We should generate distortion / distortionBlur for non distortion pass
                #if (SHADERPASS == SHADERPASS_DISTORTION)
                builtinData.distortion = surfaceDescription.Distortion;
                builtinData.distortionBlur = surfaceDescription.DistortionBlur;
                #endif
        
                #ifndef SHADER_UNLIT
                // PostInitBuiltinData call ApplyDebugToBuiltinData
                PostInitBuiltinData(V, posInput, surfaceData, builtinData);
                #else
                ApplyDebugToBuiltinData(builtinData);
                #endif
        
                RAY_TRACING_OPTIONAL_ALPHA_TEST_PASS
            }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassForwardUnlit.hlsl"
        
            // --------------------------------------------------
            // Visual Effect Vertex Invocations
        
        	#ifdef HAVE_VFX_MODIFICATION
                #if !defined(SHADER_STAGE_RAY_TRACING)
        	    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/VisualEffectVertex.hlsl"
                #else
                #endif
        	#endif
        
            ENDHLSL
        }
        Pass
        {
            Name "FullScreenDebug"
            Tags
            {
                "LightMode" = "FullScreenDebug"
            }
        
            // Render State
            Cull [_CullMode]
        ZTest LEqual
        ZWrite Off
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma instancing_options renderinglayer
        #pragma target 4.5
        #pragma vertex Vert
        #pragma fragment Frag
        #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch
        #pragma multi_compile_instancing
        
            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>
        
            // Defines
            #define SHADERPASS SHADERPASS_FULL_SCREEN_DEBUG
        #define RAYTRACING_SHADER_GRAPH_DEFAULT
        #define SUPPORT_GLOBAL_MIP_BIAS 1
        
            // For custom interpolators to inject a substruct definition before FragInputs definition,
            // allowing for FragInputs to capture CI's intended for ShaderGraph's SDI.
            struct CustomInterpolators
        {
        };
        #define USE_CUSTOMINTERP_SUBSTRUCT
        
        
        
            // TODO: Merge FragInputsVFX substruct with CustomInterpolators.
        	#ifdef HAVE_VFX_MODIFICATION
        	struct FragInputsVFX
            {
                /* WARNING: $splice Could not find named fragment 'FragInputsVFX' */
            };
            #endif
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl" // Required by Tessellation.hlsl
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl" // Required to be include before we include properties as it define DECLARE_STACK_CB
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl" // Required before including properties as it defines UNITY_TEXTURE_STREAMING_DEBUG_VARS
            // Always include Shader Graph version
            // Always include last to avoid double macros
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl" // Need to be here for Gradient struct definition
        
            // --------------------------------------------------
            // Defines
        
            // Attribute
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TANGENT_TO_WORLD
            #define VARYINGS_NEED_TEXCOORD0
        
            #define HAVE_MESH_MODIFICATION
        
            //Strip down the FragInputs.hlsl (on graphics), so we can only optimize the interpolators we use.
            //if by accident something requests contents of FragInputs.hlsl, it will be caught as a compiler error
            //Frag inputs stripping is only enabled when FRAG_INPUTS_ENABLE_STRIPPING is set
            #if !defined(SHADER_STAGE_RAY_TRACING) && SHADERPASS != SHADERPASS_RAYTRACING_GBUFFER && SHADERPASS != SHADERPASS_FULL_SCREEN_DEBUG
            #define FRAG_INPUTS_ENABLE_STRIPPING
            #endif
            #define FRAG_INPUTS_USE_TEXCOORD0
        
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
        
        
        
        
            // Following two define are a workaround introduce in 10.1.x for RaytracingQualityNode
            // The ShaderGraph don't support correctly migration of this node as it serialize all the node data
            // in the json file making it impossible to uprgrade. Until we get a fix, we do a workaround here
            // to still allow us to rename the field and keyword of this node without breaking existing code.
            #ifdef RAYTRACING_SHADER_GRAPH_DEFAULT
            #define RAYTRACING_SHADER_GRAPH_HIGH
            #endif
        
            #ifdef RAYTRACING_SHADER_GRAPH_RAYTRACED
            #define RAYTRACING_SHADER_GRAPH_LOW
            #endif
            // end
        
            #ifndef SHADER_UNLIT
            // We need isFrontFace when using double sided - it is not required for unlit as in case of unlit double sided only drive the cullmode
            // VARYINGS_NEED_CULLFACE can be define by VaryingsMeshToPS.FaceSign input if a IsFrontFace Node is included in the shader graph.
            #if defined(_DOUBLESIDED_ON) && !defined(VARYINGS_NEED_CULLFACE)
                #define VARYINGS_NEED_CULLFACE
            #endif
            #endif
        
            // Specific Material Define
        // Setup a define to say we are an unlit shader
        #define SHADER_UNLIT
        
        // Following Macro are only used by Unlit material
        #if defined(_ENABLE_SHADOW_MATTE)
            #if SHADERPASS == SHADERPASS_FORWARD_UNLIT
                #pragma multi_compile_fragment USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST
            #elif SHADERPASS == SHADERPASS_PATH_TRACING
                #define LIGHTLOOP_DISABLE_TILE_AND_CLUSTER
            #endif
        
        // We don't want to have the lightloop defined for the ray tracing passes, but we do for the rasterisation and path tracing shader passes.
        #if !defined(SHADER_STAGE_RAY_TRACING) || SHADERPASS == SHADERPASS_PATH_TRACING
            #define HAS_LIGHTLOOP
        #endif
        #endif
            // Caution: we can use the define SHADER_UNLIT onlit after the above Material include as it is the Unlit template who define it
        
            // To handle SSR on transparent correctly with a possibility to enable/disable it per framesettings
            // we should have a code like this:
            // if !defined(_DISABLE_SSR_TRANSPARENT)
            // pragma multi_compile _ WRITE_NORMAL_BUFFER
            // endif
            // i.e we enable the multicompile only if we can receive SSR or not, and then C# code drive
            // it based on if SSR transparent in frame settings and not (and stripper can strip it).
            // this is currently not possible with our current preprocessor as _DISABLE_SSR_TRANSPARENT is a keyword not a define
            // so instead we used this and chose to pay the extra cost of normal write even if SSR transaprent is disabled.
            // Ideally the shader graph generator should handle it but condition below can't be handle correctly for now.
            #if SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_PREPASS
            #if !defined(_DISABLE_SSR_TRANSPARENT) && !defined(SHADER_UNLIT)
                #define WRITE_NORMAL_BUFFER
            #endif
            #endif
        
            // See Lit.shader
            #if SHADERPASS == SHADERPASS_MOTION_VECTORS && defined(WRITE_DECAL_BUFFER_AND_RENDERING_LAYER)
                #define WRITE_DECAL_BUFFER
            #endif
        
            #ifndef DEBUG_DISPLAY
                // In case of opaque we don't want to perform the alpha test, it is done in depth prepass and we use depth equal for ztest (setup from UI)
                // Don't do it with debug display mode as it is possible there is no depth prepass in this case
                #if !defined(_SURFACE_TYPE_TRANSPARENT)
                    #if SHADERPASS == SHADERPASS_FORWARD
                    #define SHADERPASS_FORWARD_BYPASS_ALPHA_TEST
                    #elif SHADERPASS == SHADERPASS_GBUFFER
                    #define SHADERPASS_GBUFFER_BYPASS_ALPHA_TEST
                    #endif
                #endif
            #endif
        
            // Define _DEFERRED_CAPABLE_MATERIAL for shader capable to run in deferred pass
            #if defined(SHADER_LIT) && !defined(_SURFACE_TYPE_TRANSPARENT)
                #define _DEFERRED_CAPABLE_MATERIAL
            #endif
        
            // Translate transparent motion vector define
            #if (defined(_TRANSPARENT_WRITES_MOTION_VEC) || defined(_TRANSPARENT_REFRACTIVE_SORT)) && defined(_SURFACE_TYPE_TRANSPARENT)
                #define _WRITE_TRANSPARENT_MOTION_VECTOR
            #endif
        
            // -- Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _VoidColor;
        float _Speed;
        float _FresnelPower;
        float4 _BorderColor;
        float2 _Seed;
        float4 _TwirlTexture_TexelSize;
        float _InnerEdge;
        float4 _EmissionColor;
        float _UseShadowThreshold;
        float4 _DoubleSidedConstants;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        float _BlendMode;
        float _EnableBlendModePreserveSpecularLighting;
        CBUFFER_END
        
        
        // Object and Global properties
        TEXTURE2D(_TwirlTexture);
        SAMPLER(sampler_TwirlTexture);
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Includes
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Debug/DebugDisplay.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/Unlit.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/ClassicNoise3D.hlsl"
        #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi3D.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonLighting.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            struct AttributesMesh
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct VaryingsMeshToPS
        {
            SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
             float3 positionRWS;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct SurfaceDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 WorldSpaceNormal;
             float3 ObjectSpaceViewDirection;
             float3 WorldSpaceViewDirection;
             float3 AbsoluteWorldSpacePosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct PackedVaryingsMeshToPS
        {
            SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
             float4 tangentWS : INTERP0;
             float4 texCoord0 : INTERP1;
             float3 positionRWS : INTERP2;
             float3 normalWS : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
        };
        
            PackedVaryingsMeshToPS PackVaryingsMeshToPS (VaryingsMeshToPS input)
        {
            PackedVaryingsMeshToPS output;
            ZERO_INITIALIZE(PackedVaryingsMeshToPS, output);
            output.positionCS = input.positionCS;
            output.tangentWS.xyzw = input.tangentWS;
            output.texCoord0.xyzw = input.texCoord0;
            output.positionRWS.xyz = input.positionRWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            return output;
        }
        
        VaryingsMeshToPS UnpackVaryingsMeshToPS (PackedVaryingsMeshToPS input)
        {
            VaryingsMeshToPS output;
            output.positionCS = input.positionCS;
            output.tangentWS = input.tangentWS.xyzw;
            output.texCoord0 = input.texCoord0.xyzw;
            output.positionRWS = input.positionRWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
        
            // Graph Functions
            
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_DotProduct_float3(float3 A, float3 B, out float Out)
        {
            Out = dot(A, B);
        }
        
        void Unity_SampleGradientV1_float(Gradient Gradient, float Time, out float4 Out)
        {
            // convert to OkLab if we need perceptual color space.
            float3 color = lerp(Gradient.colors[0].rgb, LinearToOklab(Gradient.colors[0].rgb), Gradient.type == 2);
        
            [unroll]
            for (int c = 1; c < Gradient.colorsLength; c++)
            {
                float colorPos = saturate((Time - Gradient.colors[c - 1].w) / (Gradient.colors[c].w - Gradient.colors[c - 1].w)) * step(c, Gradient.colorsLength - 1);
                float3 color2 = lerp(Gradient.colors[c].rgb, LinearToOklab(Gradient.colors[c].rgb), Gradient.type == 2);
                color = lerp(color, color2, lerp(colorPos, step(0.01, colorPos), Gradient.type % 2)); // grad.type == 1 is fixed, 0 and 2 are blends.
            }
            color = lerp(color, OklabToLinear(color), Gradient.type == 2);
        
        #ifdef UNITY_COLORSPACE_GAMMA
            color = LinearToSRGB(color);
        #endif
        
            float alpha = Gradient.alphas[0].x;
            [unroll]
            for (int a = 1; a < Gradient.alphasLength; a++)
            {
                float alphaPos = saturate((Time - Gradient.alphas[a - 1].y) / (Gradient.alphas[a].y - Gradient.alphas[a - 1].y)) * step(a, Gradient.alphasLength - 1);
                alpha = lerp(alpha, Gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), Gradient.type % 2));
            }
        
            Out = float4(color, alpha);
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
        Out = A * B;
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        struct Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float
        {
        };
        
        void SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(float3 Vector3_7940555B, float Vector1_1B8B9078, Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float IN, out float Value_0)
        {
        float3 _Property_44999cc87708de82a26b39ae1da975ec_Out_0_Vector3 = Vector3_7940555B;
        float _Property_dad5add45a7fa785be976f925bc5a5da_Out_0_Float = Vector1_1B8B9078;
        float3 _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3;
        Unity_Multiply_float3_float3(_Property_44999cc87708de82a26b39ae1da975ec_Out_0_Vector3, (_Property_dad5add45a7fa785be976f925bc5a5da_Out_0_Float.xxx), _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3);
        float _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float;
        PerlinNoise3D_float(_Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3, _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float);
        float _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float;
        Unity_Remap_float(_PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float, float2 (-1.15, 1.15), float2 (0, 1), _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float);
        Value_0 = _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_RandomRange_float(float2 Seed, float Min, float Max, out float Out)
        {
             float randomno =  frac(sin(dot(Seed, float2(12.9898, 78.233)))*43758.5453);
             Out = lerp(Min, Max, randomno);
        }
        
        struct Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float
        {
        };
        
        void SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(float3 Vector3_375394F7, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float IN, out float Value_1, out float Cells_2)
        {
        float3 _Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3 = Vector3_375394F7;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        VoronoiPrecise3D_float(_Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }
        
        float3 Unity_HDRP_GetEmissionHDRColor_float(float3 ldrColor, float luminanceIntensity, float exposureWeight)
        {
            float3 hdrColor = ldrColor * luminanceIntensity;
        
            #ifdef SHADERGRAPH_PREVIEW
            float inverseExposureMultiplier = 1.0;
            #else
            float inverseExposureMultiplier = GetInverseCurrentExposureMultiplier();
            #endif
        
            // Inverse pre-expose using _EmissiveExposureWeight weight
            hdrColor = lerp(hdrColor * inverseExposureMultiplier, hdrColor, exposureWeight);
            return hdrColor;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float3 Emission;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_d09e7f2c2a774dbd93cc943fa6a50940_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_VoidColor) : _VoidColor;
            float _Property_16eef79ff58443149895b35d0a88bdb2_Out_0_Float = _InnerEdge;
            float4 _UV_ce55b36e72754a339073e4c05a64b551_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_054563945a0a4f688fce11f0c067b0c8_Out_1_Vector2 = _UV_ce55b36e72754a339073e4c05a64b551_Out_0_Vector4.xy;
            float _Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float;
            Unity_Distance_float2(_Swizzle_054563945a0a4f688fce11f0c067b0c8_Out_1_Vector2, float2(0.5, 0.5), _Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float);
            float _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float;
            Unity_OneMinus_float(_Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float, _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float);
            float _Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float;
            Unity_Smoothstep_float(_Property_16eef79ff58443149895b35d0a88bdb2_Out_0_Float, float(1), _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float, _Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float);
            Gradient _Gradient_c34cf660310448768ec81f4600b23ad8_Out_0_Gradient = NewGradient(0, 4, 2, float4(0, 0, 0, 0.271931),float4(0.01568037, 0.01568037, 0.01568037, 0.4678416),float4(0.1071531, 0.1071531, 0.1071531, 0.6052644),float4(1, 1, 1, 1),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0), float2(1, 0),float2(1, 1),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0));
            float _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float;
            Unity_DotProduct_float3(IN.ObjectSpaceViewDirection, IN.ObjectSpaceNormal, _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float);
            float4 _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4;
            Unity_SampleGradientV1_float(_Gradient_c34cf660310448768ec81f4600b23ad8_Out_0_Gradient, _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float, _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4);
            float4 _Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float.xxxx), _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4, _Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4);
            Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a;
            float _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float;
            SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(IN.ObjectSpaceViewDirection, float(10), _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a, _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float);
            float3 _Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3;
            Unity_Multiply_float3_float3(IN.ObjectSpaceViewDirection, (_Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float.xxx), _Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3);
            float _Property_ba2d1376c19c40fbadc820d0fb721b2e_Out_0_Float = _Speed;
            float _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_ba2d1376c19c40fbadc820d0fb721b2e_Out_0_Float, _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float);
            float2 _Property_2cfd9c61936548ee85383d83df6beba0_Out_0_Vector2 = _Seed;
            float _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float;
            Unity_RandomRange_float(_Property_2cfd9c61936548ee85383d83df6beba0_Out_0_Vector2, float(26.96), float(45.6), _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float);
            Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a;
            float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Value_1_Float;
            float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float;
            SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(_Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3, _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float, _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Value_1_Float, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float);
            float4 _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4, (_Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float.xxxx), _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4);
            float4 _Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_d09e7f2c2a774dbd93cc943fa6a50940_Out_0_Vector4, _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4, _Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4);
            float4 _Property_e0f9565d1eaa452fb638faa34cc517eb_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_BorderColor) : _BorderColor;
            float3 _Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3;
            Unity_Absolute_float3(IN.AbsoluteWorldSpacePosition, _Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3);
            Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1;
            float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Value_1_Float;
            float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float;
            SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(_Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3, IN.TimeParameters.x, float(6.2), _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1, _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Value_1_Float, _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float);
            float _Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float;
            Unity_Smoothstep_float(float(-0.41), float(1), _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float, _Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float);
            float _Property_82bfb359e6494ff08a54e8c08976a6f1_Out_0_Float = _FresnelPower;
            float _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_82bfb359e6494ff08a54e8c08976a6f1_Out_0_Float, _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float);
            float _Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float, _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float, _Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float);
            float4 _Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_e0f9565d1eaa452fb638faa34cc517eb_Out_0_Vector4, (_Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float.xxxx), _Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4);
            float3 _EmissionNode_87074dafd5ff4d39bf4005b1089a56e4_Output_0_Vector3 = Unity_HDRP_GetEmissionHDRColor_float((_Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4.xyz).xyz, float(3), float(0));
            float4 _Add_0b088d988049415c84334d987e692959_Out_2_Vector4;
            Unity_Add_float4(_Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4, (_Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float.xxxx), _Add_0b088d988049415c84334d987e692959_Out_2_Vector4);
            surface.BaseColor = (_Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4.xyz);
            surface.Emission = _EmissionNode_87074dafd5ff4d39bf4005b1089a56e4_Output_0_Vector3;
            surface.Alpha = (_Add_0b088d988049415c84334d987e692959_Out_2_Vector4).x;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES AttributesMesh
            #define VaryingsMeshType VaryingsMeshToPS
            #define VFX_SRP_VARYINGS VaryingsMeshType
            #define VFX_SRP_SURFACE_INPUTS FragInputs
            #endif
            
        VertexDescriptionInputs AttributesMeshToVertexDescriptionInputs(AttributesMesh input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else
        #endif
        
            return output;
        }
        
        VertexDescription GetVertexDescription(AttributesMesh input, float3 timeParameters
        #ifdef HAVE_VFX_MODIFICATION
            , AttributesElement element
        #endif
        )
        {
            // build graph inputs
            VertexDescriptionInputs vertexDescriptionInputs = AttributesMeshToVertexDescriptionInputs(input);
            // Override time parameters with used one (This is required to correctly handle motion vectors for vertex animation based on time)
        
            // evaluate vertex graph
        #ifdef HAVE_VFX_MODIFICATION
            GraphProperties properties;
            ZERO_INITIALIZE(GraphProperties, properties);
        
            // Fetch the vertex graph properties for the particle instance.
            GetElementVertexProperties(element, properties);
        
            VertexDescription vertexDescription = VertexDescriptionFunction(vertexDescriptionInputs, properties);
        #else
            VertexDescription vertexDescription = VertexDescriptionFunction(vertexDescriptionInputs);
        #endif
            return vertexDescription;
        
        }
        
        AttributesMesh ApplyMeshModification(AttributesMesh input, float3 timeParameters
        #ifdef USE_CUSTOMINTERP_SUBSTRUCT
            #ifdef TESSELLATION_ON
            , inout VaryingsMeshToDS varyings
            #else
            , inout VaryingsMeshToPS varyings
            #endif
        #endif
        #ifdef HAVE_VFX_MODIFICATION
                , AttributesElement element
        #endif
            )
        {
            VertexDescription vertexDescription = GetVertexDescription(input, timeParameters
        #ifdef HAVE_VFX_MODIFICATION
                , element
        #endif
            );
        
            // copy graph output to the results
            input.positionOS = vertexDescription.Position;
            input.normalOS = vertexDescription.Normal;
            input.tangentOS.xyz = vertexDescription.Tangent;
        
            
        
            return input;
        }
        
        #if defined(_ADD_CUSTOM_VELOCITY) // For shader graph custom velocity
        // Return precomputed Velocity in object space
        float3 GetCustomVelocity(AttributesMesh input
        #ifdef HAVE_VFX_MODIFICATION
            , AttributesElement element
        #endif
        )
        {
            VertexDescription vertexDescription = GetVertexDescription(input, _TimeParameters.xyz
        #ifdef HAVE_VFX_MODIFICATION
                , element
        #endif
            );
            return vertexDescription.CustomVelocity;
        }
        #endif
        
        FragInputs BuildFragInputs(VaryingsMeshToPS input)
        {
            FragInputs output;
            ZERO_INITIALIZE(FragInputs, output);
        
            // Init to some default value to make the computer quiet (else it output 'divide by zero' warning even if value is not used).
            // TODO: this is a really poor workaround, but the variable is used in a bunch of places
            // to compute normals which are then passed on elsewhere to compute other values...
            output.tangentToWorld = k_identity3x3;
            output.positionSS = input.positionCS;       // input.positionCS is SV_Position
        
            output.positionRWS =                input.positionRWS;
            output.tangentToWorld =             BuildTangentToWorld(input.tangentWS, input.normalWS);
            output.texCoord0 =                  input.texCoord0;
        
        #if UNITY_ANY_INSTANCING_ENABLED
        #else
        #endif
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
        
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            // splice point to copy custom interpolator fields from varyings to frag inputs
            
        
            return output;
        }
        
        // existing HDRP code uses the combined function to go directly from packed to frag inputs
        FragInputs UnpackVaryingsMeshToFragInputs(PackedVaryingsMeshToPS input)
        {
            UNITY_SETUP_INSTANCE_ID(input);
        #if defined(HAVE_VFX_MODIFICATION) && defined(UNITY_INSTANCING_ENABLED)
            unity_InstanceID = input.instanceID;
        #endif
            VaryingsMeshToPS unpacked = UnpackVaryingsMeshToPS(input);
            return BuildFragInputs(unpacked);
        }
            SurfaceDescriptionInputs FragInputsToSurfaceDescriptionInputs(FragInputs input, float3 viewWS)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            output.WorldSpaceNormal =                           normalize(input.tangentToWorld[2].xyz);
            #if defined(SHADER_STAGE_RAY_TRACING)
            output.ObjectSpaceNormal =                          mul(output.WorldSpaceNormal, (float3x3) ObjectToWorld3x4());
            #else
            output.ObjectSpaceNormal =                          normalize(mul(output.WorldSpaceNormal, (float3x3) UNITY_MATRIX_M));           // transposed multiplication by inverse matrix to handle normal scale
            #endif
            output.WorldSpaceViewDirection =                    normalize(viewWS);
            output.ObjectSpaceViewDirection =                   TransformWorldToObjectDir(output.WorldSpaceViewDirection);
        #if SHADERPASS != SHADERPASS_FOG_VOLUME_VOXELIZATION
        #else
        #endif
            output.AbsoluteWorldSpacePosition =                 GetAbsolutePositionWS(input.positionRWS);
        #if SHADERPASS != SHADERPASS_FOG_VOLUME_VOXELIZATION
        #else
        #endif
        
        #if UNITY_UV_STARTS_AT_TOP
        #else
        #endif
        
        
            output.uv0 =                                        input.texCoord0;
            output.TimeParameters =                             _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        
            // splice point to copy frag inputs custom interpolator pack into the SDI
            
        
            return output;
        }
        
            // --------------------------------------------------
            // Build Surface Data (Specific Material)
        
        void ApplyDecalToSurfaceDataNoNormal(DecalSurfaceData decalSurfaceData, inout SurfaceData surfaceData);
        
        void ApplyDecalAndGetNormal(FragInputs fragInputs, PositionInputs posInput, SurfaceDescription surfaceDescription,
            inout SurfaceData surfaceData)
        {
            float3 doubleSidedConstants = GetDoubleSidedConstants();
        
        #ifdef DECAL_NORMAL_BLENDING
            // SG nodes don't ouptut surface gradients, so if decals require surf grad blending, we have to convert
            // the normal to gradient before applying the decal. We then have to resolve the gradient back to world space
            float3 normalTS;
        
        
        
            #if HAVE_DECALS
            if (_EnableDecals)
            {
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs, alpha);
                ApplyDecalToSurfaceNormal(decalSurfaceData, fragInputs.tangentToWorld[2], normalTS);
                ApplyDecalToSurfaceDataNoNormal(decalSurfaceData, surfaceData);
            }
            #endif
        
            GetNormalWS_SG(fragInputs, normalTS, surfaceData.normalWS, doubleSidedConstants);
        #else
            // normal delivered to master node
        
            #if HAVE_DECALS
            if (_EnableDecals)
            {
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                // Both uses and modifies 'surfaceData.normalWS'.
                DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs, alpha);
                ApplyDecalToSurfaceNormal(decalSurfaceData, surfaceData.normalWS.xyz);
                ApplyDecalToSurfaceDataNoNormal(decalSurfaceData, surfaceData);
            }
            #endif
        #endif
        }
        void BuildSurfaceData(FragInputs fragInputs, inout SurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData)
        {
            // setup defaults -- these are used if the graph doesn't output a value
            ZERO_INITIALIZE(SurfaceData, surfaceData);
        
            // copy across graph values, if defined
            surfaceData.color = surfaceDescription.BaseColor;
        
            #ifdef WRITE_NORMAL_BUFFER
            // When we need to export the normal (in the depth prepass, we write the geometry one)
            surfaceData.normalWS = fragInputs.tangentToWorld[2];
            #endif
        
            #if defined(DEBUG_DISPLAY)
            #if !defined(SHADER_STAGE_RAY_TRACING)
            // Mipmap mode debugging isn't supported with ray tracing as it relies on derivatives
                if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                {
                    #ifdef FRAG_INPUTS_USE_TEXCOORD0
                        surfaceData.color = GET_TEXTURE_STREAMING_DEBUG(posInput.positionSS, fragInputs.texCoord0);
                    #else
                        surfaceData.color = GET_TEXTURE_STREAMING_DEBUG_NO_UV(posInput.positionSS);
                    #endif
                }
            #endif
            #endif
        
            #ifdef _ENABLE_SHADOW_MATTE
        
                #if (SHADERPASS == SHADERPASS_FORWARD_UNLIT) || (SHADERPASS == SHADERPASS_RAYTRACING_GBUFFER) || (SHADERPASS == SHADERPASS_RAYTRACING_INDIRECT) || (SHADERPASS == SHADERPASS_RAYTRACING_FORWARD)
        
                    HDShadowContext shadowContext = InitShadowContext();
        
                    // Evaluate the shadow, the normal is guaranteed if shadow matte is enabled on this shader.
                    float3 shadow3;
                    ShadowLoopMin(shadowContext, posInput, normalize(fragInputs.tangentToWorld[2]), asuint(_ShadowMatteFilter), GetMeshRenderingLayerMask(), shadow3);
        
                    // Compute the average value in the fourth channel
                    float4 shadow = float4(shadow3, dot(shadow3, float3(1.0/3.0, 1.0/3.0, 1.0/3.0)));
        
                    float4 shadowColor = (1.0 - shadow) * surfaceDescription.ShadowTint.rgba;
                    float  localAlpha  = saturate(shadowColor.a + surfaceDescription.Alpha);
        
                    // Keep the nested lerp
                    // With no Color (bsdfData.color.rgb, bsdfData.color.a == 0.0f), just use ShadowColor*Color to avoid a ring of "white" around the shadow
                    // And mix color to consider the Color & ShadowColor alpha (from texture or/and color picker)
                    #ifdef _SURFACE_TYPE_TRANSPARENT
                        surfaceData.color = lerp(shadowColor.rgb * surfaceData.color, lerp(lerp(shadowColor.rgb, surfaceData.color, 1.0 - surfaceDescription.ShadowTint.a), surfaceData.color, shadow.rgb), surfaceDescription.Alpha);
                    #else
                        surfaceData.color = lerp(lerp(shadowColor.rgb, surfaceData.color, 1.0 - surfaceDescription.ShadowTint.a), surfaceData.color, shadow.rgb);
                    #endif
                    localAlpha = ApplyBlendMode(surfaceData.color, localAlpha).a;
        
                    surfaceDescription.Alpha = localAlpha;
        
                #elif SHADERPASS == SHADERPASS_PATH_TRACING
        
                    surfaceData.normalWS = fragInputs.tangentToWorld[2];
                    surfaceData.shadowTint = surfaceDescription.ShadowTint.rgba;
        
                #endif
        
            #endif // _ENABLE_SHADOW_MATTE
        }
        
            // --------------------------------------------------
            // Get Surface And BuiltinData
        
            void GetSurfaceAndBuiltinData(FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData RAY_TRACING_OPTIONAL_PARAMETERS)
            {
                // Don't dither if displaced tessellation (we're fading out the displacement instead to match the next LOD)
                #if !defined(SHADER_STAGE_RAY_TRACING) && !defined(_TESSELLATION_DISPLACEMENT)
                #ifdef LOD_FADE_CROSSFADE // enable dithering LOD transition if user select CrossFade transition in LOD group
                LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                #endif
                #endif
        
                #ifndef SHADER_UNLIT
                #ifdef _DOUBLESIDED_ON
                    float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
                #else
                    float3 doubleSidedConstants = float3(1.0, 1.0, 1.0);
                #endif
        
                ApplyDoubleSidedFlipOrMirror(fragInputs, doubleSidedConstants); // Apply double sided flip on the vertex normal
                #endif // SHADER_UNLIT
        
                SurfaceDescriptionInputs surfaceDescriptionInputs = FragInputsToSurfaceDescriptionInputs(fragInputs, V);
        
                #if defined(HAVE_VFX_MODIFICATION)
                GraphProperties properties;
                ZERO_INITIALIZE(GraphProperties, properties);
        
                GetElementPixelProperties(fragInputs, properties);
        
                SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs, properties);
                #else
                SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);
                #endif
        
                #ifdef DEBUG_DISPLAY
                if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                {
                    surfaceDescription.Alpha = 1.0f;
                }
                #endif
        
                // Perform alpha test very early to save performance (a killed pixel will not sample textures)
                // TODO: split graph evaluation to grab just alpha dependencies first? tricky..
                #ifdef _ALPHATEST_ON
                    float alphaCutoff = surfaceDescription.AlphaClipThreshold;
                    #if SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_PREPASS
                    // The TransparentDepthPrepass is also used with SSR transparent.
                    // If an artists enable transaprent SSR but not the TransparentDepthPrepass itself, then we use AlphaClipThreshold
                    // otherwise if TransparentDepthPrepass is enabled we use AlphaClipThresholdDepthPrepass
                    #elif SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_POSTPASS
                    // DepthPostpass always use its own alpha threshold
                    alphaCutoff = surfaceDescription.AlphaClipThresholdDepthPostpass;
                    #elif (SHADERPASS == SHADERPASS_SHADOWS) || (SHADERPASS == SHADERPASS_RAYTRACING_VISIBILITY)
                    // If use shadow threshold isn't enable we don't allow any test
                    #endif
        
                    GENERIC_ALPHA_TEST(surfaceDescription.Alpha, alphaCutoff);
                #endif
        
                #if !defined(SHADER_STAGE_RAY_TRACING) && _DEPTHOFFSET_ON
                ApplyDepthOffsetPositionInput(V, surfaceDescription.DepthOffset, GetViewForwardDir(), GetWorldToHClipMatrix(), posInput);
                #endif
        
                #ifndef SHADER_UNLIT
                float3 bentNormalWS;
                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData, bentNormalWS);
        
                #ifdef FRAG_INPUTS_USE_TEXCOORD1
                    float4 lightmapTexCoord1 = fragInputs.texCoord1;
                #else
                    float4 lightmapTexCoord1 = float4(0,0,0,0);
                #endif
        
                #ifdef FRAG_INPUTS_USE_TEXCOORD2
                    float4 lightmapTexCoord2 = fragInputs.texCoord2;
                #else
                    float4 lightmapTexCoord2 = float4(0,0,0,0);
                #endif
        
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                // Builtin Data
                // For back lighting we use the oposite vertex normal
                InitBuiltinData(posInput, alpha, bentNormalWS, -fragInputs.tangentToWorld[2], lightmapTexCoord1, lightmapTexCoord2, builtinData);
        
                #else
                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData);
        
                ZERO_BUILTIN_INITIALIZE(builtinData); // No call to InitBuiltinData as we don't have any lighting
                builtinData.opacity = surfaceDescription.Alpha;
        
                #if defined(DEBUG_DISPLAY)
                    // Light Layers are currently not used for the Unlit shader (because it is not lit)
                    // But Unlit objects do cast shadows according to their rendering layer mask, which is what we want to
                    // display in the light layers visualization mode, therefore we need the renderingLayers
                    builtinData.renderingLayers = GetMeshRenderingLayerMask();
                #endif
        
                #endif // SHADER_UNLIT
        
                #ifdef _ALPHATEST_ON
                    // Used for sharpening by alpha to mask - Alpha to covertage is only used with depth only and forward pass (no shadow pass, no transparent pass)
                    builtinData.alphaClipTreshold = alphaCutoff;
                #endif
        
                // override sampleBakedGI - not used by Unlit
        		// When overriding GI, we need to force the isLightmap flag to make sure we don't add APV (sampled in the lightloop) on top of the overridden value (set at GBuffer stage)
        
                builtinData.emissiveColor = surfaceDescription.Emission;
        
                // Note this will not fully work on transparent surfaces (can check with _SURFACE_TYPE_TRANSPARENT define)
                // We will always overwrite vt feeback with the nearest. So behind transparent surfaces vt will not be resolved
                // This is a limitation of the current MRT approach.
                #ifdef UNITY_VIRTUAL_TEXTURING
                #endif
        
                #if _DEPTHOFFSET_ON
                builtinData.depthOffset = surfaceDescription.DepthOffset;
                #endif
        
                // TODO: We should generate distortion / distortionBlur for non distortion pass
                #if (SHADERPASS == SHADERPASS_DISTORTION)
                builtinData.distortion = surfaceDescription.Distortion;
                builtinData.distortionBlur = surfaceDescription.DistortionBlur;
                #endif
        
                #ifndef SHADER_UNLIT
                // PostInitBuiltinData call ApplyDebugToBuiltinData
                PostInitBuiltinData(V, posInput, surfaceData, builtinData);
                #else
                ApplyDebugToBuiltinData(builtinData);
                #endif
        
                RAY_TRACING_OPTIONAL_ALPHA_TEST_PASS
            }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassFullScreenDebug.hlsl"
        
            // --------------------------------------------------
            // Visual Effect Vertex Invocations
        
        	#ifdef HAVE_VFX_MODIFICATION
                #if !defined(SHADER_STAGE_RAY_TRACING)
        	    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/VisualEffectVertex.hlsl"
                #else
                #endif
        	#endif
        
            ENDHLSL
        }
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="HDRenderPipeline"
            "RenderType"="HDUnlitShader"
            "Queue"="Transparent+0"
            "DisableBatching"="False"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="HDUnlitSubTarget"
        }
        Pass
        {
            Name "IndirectDXR"
            Tags
            {
                "LightMode" = "IndirectDXR"
            }
        
            // Render State
            // RenderState: <None>
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma target 5.0
        #pragma raytracing surface_shader
        #pragma only_renderers d3d11 xboxseries ps5
        
            // Keywords
            #pragma shader_feature _ _SURFACE_TYPE_TRANSPARENT
        #pragma multi_compile _ DEBUG_DISPLAY
            // GraphKeywords: <None>
        
            // Defines
            #define SHADERPASS SHADERPASS_RAYTRACING_INDIRECT
        #define PUNCTUAL_SHADOW_LOW
        #define DIRECTIONAL_SHADOW_LOW
        #define RAYTRACING_SHADER_GRAPH_RAYTRACED
        #define SUPPORT_GLOBAL_MIP_BIAS 1
        
            // For custom interpolators to inject a substruct definition before FragInputs definition,
            // allowing for FragInputs to capture CI's intended for ShaderGraph's SDI.
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
        
            // TODO: Merge FragInputsVFX substruct with CustomInterpolators.
        	#ifdef HAVE_VFX_MODIFICATION
        	struct FragInputsVFX
            {
                /* WARNING: $splice Could not find named fragment 'FragInputsVFX' */
            };
            #endif
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl" // Required by Tessellation.hlsl
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl" // Required to be include before we include properties as it define DECLARE_STACK_CB
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl" // Required before including properties as it defines UNITY_TEXTURE_STREAMING_DEBUG_VARS
            // Always include Shader Graph version
            // Always include last to avoid double macros
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl" // Need to be here for Gradient struct definition
        
            // --------------------------------------------------
            // Defines
        
            // Attribute
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TANGENT_TO_WORLD
            #define VARYINGS_NEED_TEXCOORD0
        
        
            //Strip down the FragInputs.hlsl (on graphics), so we can only optimize the interpolators we use.
            //if by accident something requests contents of FragInputs.hlsl, it will be caught as a compiler error
            //Frag inputs stripping is only enabled when FRAG_INPUTS_ENABLE_STRIPPING is set
            #if !defined(SHADER_STAGE_RAY_TRACING) && SHADERPASS != SHADERPASS_RAYTRACING_GBUFFER && SHADERPASS != SHADERPASS_FULL_SCREEN_DEBUG
            #define FRAG_INPUTS_ENABLE_STRIPPING
            #endif
            #define FRAG_INPUTS_USE_TEXCOORD0
        
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
        
        
        
        
            // Following two define are a workaround introduce in 10.1.x for RaytracingQualityNode
            // The ShaderGraph don't support correctly migration of this node as it serialize all the node data
            // in the json file making it impossible to uprgrade. Until we get a fix, we do a workaround here
            // to still allow us to rename the field and keyword of this node without breaking existing code.
            #ifdef RAYTRACING_SHADER_GRAPH_DEFAULT
            #define RAYTRACING_SHADER_GRAPH_HIGH
            #endif
        
            #ifdef RAYTRACING_SHADER_GRAPH_RAYTRACED
            #define RAYTRACING_SHADER_GRAPH_LOW
            #endif
            // end
        
            #ifndef SHADER_UNLIT
            // We need isFrontFace when using double sided - it is not required for unlit as in case of unlit double sided only drive the cullmode
            // VARYINGS_NEED_CULLFACE can be define by VaryingsMeshToPS.FaceSign input if a IsFrontFace Node is included in the shader graph.
            #if defined(_DOUBLESIDED_ON) && !defined(VARYINGS_NEED_CULLFACE)
                #define VARYINGS_NEED_CULLFACE
            #endif
            #endif
        
            // Specific Material Define
        // Setup a define to say we are an unlit shader
        #define SHADER_UNLIT
        
        // Following Macro are only used by Unlit material
        #if defined(_ENABLE_SHADOW_MATTE)
            #if SHADERPASS == SHADERPASS_FORWARD_UNLIT
                #pragma multi_compile_fragment USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST
            #elif SHADERPASS == SHADERPASS_PATH_TRACING
                #define LIGHTLOOP_DISABLE_TILE_AND_CLUSTER
            #endif
        
        // We don't want to have the lightloop defined for the ray tracing passes, but we do for the rasterisation and path tracing shader passes.
        #if !defined(SHADER_STAGE_RAY_TRACING) || SHADERPASS == SHADERPASS_PATH_TRACING
            #define HAS_LIGHTLOOP
        #endif
        #endif
            // Caution: we can use the define SHADER_UNLIT onlit after the above Material include as it is the Unlit template who define it
        
            // To handle SSR on transparent correctly with a possibility to enable/disable it per framesettings
            // we should have a code like this:
            // if !defined(_DISABLE_SSR_TRANSPARENT)
            // pragma multi_compile _ WRITE_NORMAL_BUFFER
            // endif
            // i.e we enable the multicompile only if we can receive SSR or not, and then C# code drive
            // it based on if SSR transparent in frame settings and not (and stripper can strip it).
            // this is currently not possible with our current preprocessor as _DISABLE_SSR_TRANSPARENT is a keyword not a define
            // so instead we used this and chose to pay the extra cost of normal write even if SSR transaprent is disabled.
            // Ideally the shader graph generator should handle it but condition below can't be handle correctly for now.
            #if SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_PREPASS
            #if !defined(_DISABLE_SSR_TRANSPARENT) && !defined(SHADER_UNLIT)
                #define WRITE_NORMAL_BUFFER
            #endif
            #endif
        
            // See Lit.shader
            #if SHADERPASS == SHADERPASS_MOTION_VECTORS && defined(WRITE_DECAL_BUFFER_AND_RENDERING_LAYER)
                #define WRITE_DECAL_BUFFER
            #endif
        
            #ifndef DEBUG_DISPLAY
                // In case of opaque we don't want to perform the alpha test, it is done in depth prepass and we use depth equal for ztest (setup from UI)
                // Don't do it with debug display mode as it is possible there is no depth prepass in this case
                #if !defined(_SURFACE_TYPE_TRANSPARENT)
                    #if SHADERPASS == SHADERPASS_FORWARD
                    #define SHADERPASS_FORWARD_BYPASS_ALPHA_TEST
                    #elif SHADERPASS == SHADERPASS_GBUFFER
                    #define SHADERPASS_GBUFFER_BYPASS_ALPHA_TEST
                    #endif
                #endif
            #endif
        
            // Define _DEFERRED_CAPABLE_MATERIAL for shader capable to run in deferred pass
            #if defined(SHADER_LIT) && !defined(_SURFACE_TYPE_TRANSPARENT)
                #define _DEFERRED_CAPABLE_MATERIAL
            #endif
        
            // Translate transparent motion vector define
            #if (defined(_TRANSPARENT_WRITES_MOTION_VEC) || defined(_TRANSPARENT_REFRACTIVE_SORT)) && defined(_SURFACE_TYPE_TRANSPARENT)
                #define _WRITE_TRANSPARENT_MOTION_VECTOR
            #endif
        
            // -- Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _VoidColor;
        float _Speed;
        float _FresnelPower;
        float4 _BorderColor;
        float2 _Seed;
        float4 _TwirlTexture_TexelSize;
        float _InnerEdge;
        float4 _EmissionColor;
        float _UseShadowThreshold;
        float4 _DoubleSidedConstants;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        float _BlendMode;
        float _EnableBlendModePreserveSpecularLighting;
        CBUFFER_END
        
        
        // Object and Global properties
        TEXTURE2D(_TwirlTexture);
        SAMPLER(sampler_TwirlTexture);
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Includes
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/RaytracingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/ShaderVariablesRaytracing.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/ShaderVariablesRaytracingLightLoop.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/RaytracingIntersection.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/Unlit.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/UnlitRaytracing.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/RayTracingCommon.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/ClassicNoise3D.hlsl"
        #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi3D.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonLighting.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            struct SurfaceDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 WorldSpaceNormal;
             float3 ObjectSpaceViewDirection;
             float3 WorldSpaceViewDirection;
             float3 AbsoluteWorldSpacePosition;
             float4 uv0;
             float3 TimeParameters;
        };
        
            //Interpolator Packs: <None>
        
            // --------------------------------------------------
            // Graph
        
        
            // Graph Functions
            
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_DotProduct_float3(float3 A, float3 B, out float Out)
        {
            Out = dot(A, B);
        }
        
        void Unity_SampleGradientV1_float(Gradient Gradient, float Time, out float4 Out)
        {
            // convert to OkLab if we need perceptual color space.
            float3 color = lerp(Gradient.colors[0].rgb, LinearToOklab(Gradient.colors[0].rgb), Gradient.type == 2);
        
            [unroll]
            for (int c = 1; c < Gradient.colorsLength; c++)
            {
                float colorPos = saturate((Time - Gradient.colors[c - 1].w) / (Gradient.colors[c].w - Gradient.colors[c - 1].w)) * step(c, Gradient.colorsLength - 1);
                float3 color2 = lerp(Gradient.colors[c].rgb, LinearToOklab(Gradient.colors[c].rgb), Gradient.type == 2);
                color = lerp(color, color2, lerp(colorPos, step(0.01, colorPos), Gradient.type % 2)); // grad.type == 1 is fixed, 0 and 2 are blends.
            }
            color = lerp(color, OklabToLinear(color), Gradient.type == 2);
        
        #ifdef UNITY_COLORSPACE_GAMMA
            color = LinearToSRGB(color);
        #endif
        
            float alpha = Gradient.alphas[0].x;
            [unroll]
            for (int a = 1; a < Gradient.alphasLength; a++)
            {
                float alphaPos = saturate((Time - Gradient.alphas[a - 1].y) / (Gradient.alphas[a].y - Gradient.alphas[a - 1].y)) * step(a, Gradient.alphasLength - 1);
                alpha = lerp(alpha, Gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), Gradient.type % 2));
            }
        
            Out = float4(color, alpha);
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
        Out = A * B;
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        struct Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float
        {
        };
        
        void SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(float3 Vector3_7940555B, float Vector1_1B8B9078, Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float IN, out float Value_0)
        {
        float3 _Property_44999cc87708de82a26b39ae1da975ec_Out_0_Vector3 = Vector3_7940555B;
        float _Property_dad5add45a7fa785be976f925bc5a5da_Out_0_Float = Vector1_1B8B9078;
        float3 _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3;
        Unity_Multiply_float3_float3(_Property_44999cc87708de82a26b39ae1da975ec_Out_0_Vector3, (_Property_dad5add45a7fa785be976f925bc5a5da_Out_0_Float.xxx), _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3);
        float _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float;
        PerlinNoise3D_float(_Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3, _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float);
        float _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float;
        Unity_Remap_float(_PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float, float2 (-1.15, 1.15), float2 (0, 1), _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float);
        Value_0 = _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_RandomRange_float(float2 Seed, float Min, float Max, out float Out)
        {
             float randomno =  frac(sin(dot(Seed, float2(12.9898, 78.233)))*43758.5453);
             Out = lerp(Min, Max, randomno);
        }
        
        struct Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float
        {
        };
        
        void SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(float3 Vector3_375394F7, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float IN, out float Value_1, out float Cells_2)
        {
        float3 _Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3 = Vector3_375394F7;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        VoronoiPrecise3D_float(_Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }
        
        float3 Unity_HDRP_GetEmissionHDRColor_float(float3 ldrColor, float luminanceIntensity, float exposureWeight)
        {
            float3 hdrColor = ldrColor * luminanceIntensity;
        
            #ifdef SHADERGRAPH_PREVIEW
            float inverseExposureMultiplier = 1.0;
            #else
            float inverseExposureMultiplier = GetInverseCurrentExposureMultiplier();
            #endif
        
            // Inverse pre-expose using _EmissiveExposureWeight weight
            hdrColor = lerp(hdrColor * inverseExposureMultiplier, hdrColor, exposureWeight);
            return hdrColor;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
            // Graph Vertex
            // GraphVertex: <None>
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float3 Emission;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_d09e7f2c2a774dbd93cc943fa6a50940_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_VoidColor) : _VoidColor;
            float _Property_16eef79ff58443149895b35d0a88bdb2_Out_0_Float = _InnerEdge;
            float4 _UV_ce55b36e72754a339073e4c05a64b551_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_054563945a0a4f688fce11f0c067b0c8_Out_1_Vector2 = _UV_ce55b36e72754a339073e4c05a64b551_Out_0_Vector4.xy;
            float _Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float;
            Unity_Distance_float2(_Swizzle_054563945a0a4f688fce11f0c067b0c8_Out_1_Vector2, float2(0.5, 0.5), _Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float);
            float _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float;
            Unity_OneMinus_float(_Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float, _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float);
            float _Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float;
            Unity_Smoothstep_float(_Property_16eef79ff58443149895b35d0a88bdb2_Out_0_Float, float(1), _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float, _Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float);
            Gradient _Gradient_c34cf660310448768ec81f4600b23ad8_Out_0_Gradient = NewGradient(0, 4, 2, float4(0, 0, 0, 0.271931),float4(0.01568037, 0.01568037, 0.01568037, 0.4678416),float4(0.1071531, 0.1071531, 0.1071531, 0.6052644),float4(1, 1, 1, 1),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0), float2(1, 0),float2(1, 1),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0));
            float _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float;
            Unity_DotProduct_float3(IN.ObjectSpaceViewDirection, IN.ObjectSpaceNormal, _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float);
            float4 _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4;
            Unity_SampleGradientV1_float(_Gradient_c34cf660310448768ec81f4600b23ad8_Out_0_Gradient, _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float, _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4);
            float4 _Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float.xxxx), _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4, _Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4);
            Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a;
            float _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float;
            SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(IN.ObjectSpaceViewDirection, float(10), _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a, _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float);
            float3 _Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3;
            Unity_Multiply_float3_float3(IN.ObjectSpaceViewDirection, (_Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float.xxx), _Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3);
            float _Property_ba2d1376c19c40fbadc820d0fb721b2e_Out_0_Float = _Speed;
            float _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_ba2d1376c19c40fbadc820d0fb721b2e_Out_0_Float, _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float);
            float2 _Property_2cfd9c61936548ee85383d83df6beba0_Out_0_Vector2 = _Seed;
            float _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float;
            Unity_RandomRange_float(_Property_2cfd9c61936548ee85383d83df6beba0_Out_0_Vector2, float(26.96), float(45.6), _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float);
            Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a;
            float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Value_1_Float;
            float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float;
            SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(_Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3, _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float, _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Value_1_Float, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float);
            float4 _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4, (_Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float.xxxx), _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4);
            float4 _Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_d09e7f2c2a774dbd93cc943fa6a50940_Out_0_Vector4, _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4, _Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4);
            float4 _Property_e0f9565d1eaa452fb638faa34cc517eb_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_BorderColor) : _BorderColor;
            float3 _Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3;
            Unity_Absolute_float3(IN.AbsoluteWorldSpacePosition, _Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3);
            Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1;
            float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Value_1_Float;
            float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float;
            SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(_Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3, IN.TimeParameters.x, float(6.2), _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1, _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Value_1_Float, _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float);
            float _Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float;
            Unity_Smoothstep_float(float(-0.41), float(1), _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float, _Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float);
            float _Property_82bfb359e6494ff08a54e8c08976a6f1_Out_0_Float = _FresnelPower;
            float _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_82bfb359e6494ff08a54e8c08976a6f1_Out_0_Float, _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float);
            float _Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float, _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float, _Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float);
            float4 _Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_e0f9565d1eaa452fb638faa34cc517eb_Out_0_Vector4, (_Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float.xxxx), _Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4);
            float3 _EmissionNode_87074dafd5ff4d39bf4005b1089a56e4_Output_0_Vector3 = Unity_HDRP_GetEmissionHDRColor_float((_Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4.xyz).xyz, float(3), float(0));
            float4 _Add_0b088d988049415c84334d987e692959_Out_2_Vector4;
            Unity_Add_float4(_Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4, (_Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float.xxxx), _Add_0b088d988049415c84334d987e692959_Out_2_Vector4);
            surface.BaseColor = (_Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4.xyz);
            surface.Emission = _EmissionNode_87074dafd5ff4d39bf4005b1089a56e4_Output_0_Vector3;
            surface.Alpha = (_Add_0b088d988049415c84334d987e692959_Out_2_Vector4).x;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES AttributesMesh
            #define VaryingsMeshType VaryingsMeshToPS
            #define VFX_SRP_VARYINGS VaryingsMeshType
            #define VFX_SRP_SURFACE_INPUTS FragInputs
            #endif
            SurfaceDescriptionInputs FragInputsToSurfaceDescriptionInputs(FragInputs input, float3 viewWS)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            output.WorldSpaceNormal =                           normalize(input.tangentToWorld[2].xyz);
            #if defined(SHADER_STAGE_RAY_TRACING)
            output.ObjectSpaceNormal =                          mul(output.WorldSpaceNormal, (float3x3) ObjectToWorld3x4());
            #else
            output.ObjectSpaceNormal =                          normalize(mul(output.WorldSpaceNormal, (float3x3) UNITY_MATRIX_M));           // transposed multiplication by inverse matrix to handle normal scale
            #endif
            output.WorldSpaceViewDirection =                    normalize(viewWS);
            output.ObjectSpaceViewDirection =                   TransformWorldToObjectDir(output.WorldSpaceViewDirection);
        #if SHADERPASS != SHADERPASS_FOG_VOLUME_VOXELIZATION
        #else
        #endif
            output.AbsoluteWorldSpacePosition =                 GetAbsolutePositionWS(input.positionRWS);
        #if SHADERPASS != SHADERPASS_FOG_VOLUME_VOXELIZATION
        #else
        #endif
        
        #if UNITY_UV_STARTS_AT_TOP
        #else
        #endif
        
        
            output.uv0 =                                        input.texCoord0;
            output.TimeParameters =                             _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        
            // splice point to copy frag inputs custom interpolator pack into the SDI
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorCopyToSDI' */
        
            return output;
        }
        
            // --------------------------------------------------
            // Build Surface Data (Specific Material)
        
        void ApplyDecalToSurfaceDataNoNormal(DecalSurfaceData decalSurfaceData, inout SurfaceData surfaceData);
        
        void ApplyDecalAndGetNormal(FragInputs fragInputs, PositionInputs posInput, SurfaceDescription surfaceDescription,
            inout SurfaceData surfaceData)
        {
            float3 doubleSidedConstants = GetDoubleSidedConstants();
        
        #ifdef DECAL_NORMAL_BLENDING
            // SG nodes don't ouptut surface gradients, so if decals require surf grad blending, we have to convert
            // the normal to gradient before applying the decal. We then have to resolve the gradient back to world space
            float3 normalTS;
        
        
        
            #if HAVE_DECALS
            if (_EnableDecals)
            {
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs, alpha);
                ApplyDecalToSurfaceNormal(decalSurfaceData, fragInputs.tangentToWorld[2], normalTS);
                ApplyDecalToSurfaceDataNoNormal(decalSurfaceData, surfaceData);
            }
            #endif
        
            GetNormalWS_SG(fragInputs, normalTS, surfaceData.normalWS, doubleSidedConstants);
        #else
            // normal delivered to master node
        
            #if HAVE_DECALS
            if (_EnableDecals)
            {
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                // Both uses and modifies 'surfaceData.normalWS'.
                DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs, alpha);
                ApplyDecalToSurfaceNormal(decalSurfaceData, surfaceData.normalWS.xyz);
                ApplyDecalToSurfaceDataNoNormal(decalSurfaceData, surfaceData);
            }
            #endif
        #endif
        }
        void BuildSurfaceData(FragInputs fragInputs, inout SurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData)
        {
            // setup defaults -- these are used if the graph doesn't output a value
            ZERO_INITIALIZE(SurfaceData, surfaceData);
        
            // copy across graph values, if defined
            surfaceData.color = surfaceDescription.BaseColor;
        
            #ifdef WRITE_NORMAL_BUFFER
            // When we need to export the normal (in the depth prepass, we write the geometry one)
            surfaceData.normalWS = fragInputs.tangentToWorld[2];
            #endif
        
            #if defined(DEBUG_DISPLAY)
            #if !defined(SHADER_STAGE_RAY_TRACING)
            // Mipmap mode debugging isn't supported with ray tracing as it relies on derivatives
                if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                {
                    #ifdef FRAG_INPUTS_USE_TEXCOORD0
                        surfaceData.color = GET_TEXTURE_STREAMING_DEBUG(posInput.positionSS, fragInputs.texCoord0);
                    #else
                        surfaceData.color = GET_TEXTURE_STREAMING_DEBUG_NO_UV(posInput.positionSS);
                    #endif
                }
            #endif
            #endif
        
            #ifdef _ENABLE_SHADOW_MATTE
        
                #if (SHADERPASS == SHADERPASS_FORWARD_UNLIT) || (SHADERPASS == SHADERPASS_RAYTRACING_GBUFFER) || (SHADERPASS == SHADERPASS_RAYTRACING_INDIRECT) || (SHADERPASS == SHADERPASS_RAYTRACING_FORWARD)
        
                    HDShadowContext shadowContext = InitShadowContext();
        
                    // Evaluate the shadow, the normal is guaranteed if shadow matte is enabled on this shader.
                    float3 shadow3;
                    ShadowLoopMin(shadowContext, posInput, normalize(fragInputs.tangentToWorld[2]), asuint(_ShadowMatteFilter), GetMeshRenderingLayerMask(), shadow3);
        
                    // Compute the average value in the fourth channel
                    float4 shadow = float4(shadow3, dot(shadow3, float3(1.0/3.0, 1.0/3.0, 1.0/3.0)));
        
                    float4 shadowColor = (1.0 - shadow) * surfaceDescription.ShadowTint.rgba;
                    float  localAlpha  = saturate(shadowColor.a + surfaceDescription.Alpha);
        
                    // Keep the nested lerp
                    // With no Color (bsdfData.color.rgb, bsdfData.color.a == 0.0f), just use ShadowColor*Color to avoid a ring of "white" around the shadow
                    // And mix color to consider the Color & ShadowColor alpha (from texture or/and color picker)
                    #ifdef _SURFACE_TYPE_TRANSPARENT
                        surfaceData.color = lerp(shadowColor.rgb * surfaceData.color, lerp(lerp(shadowColor.rgb, surfaceData.color, 1.0 - surfaceDescription.ShadowTint.a), surfaceData.color, shadow.rgb), surfaceDescription.Alpha);
                    #else
                        surfaceData.color = lerp(lerp(shadowColor.rgb, surfaceData.color, 1.0 - surfaceDescription.ShadowTint.a), surfaceData.color, shadow.rgb);
                    #endif
                    localAlpha = ApplyBlendMode(surfaceData.color, localAlpha).a;
        
                    surfaceDescription.Alpha = localAlpha;
        
                #elif SHADERPASS == SHADERPASS_PATH_TRACING
        
                    surfaceData.normalWS = fragInputs.tangentToWorld[2];
                    surfaceData.shadowTint = surfaceDescription.ShadowTint.rgba;
        
                #endif
        
            #endif // _ENABLE_SHADOW_MATTE
        }
        
            // --------------------------------------------------
            // Get Surface And BuiltinData
        
            void GetSurfaceAndBuiltinData(FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData RAY_TRACING_OPTIONAL_PARAMETERS)
            {
                // Don't dither if displaced tessellation (we're fading out the displacement instead to match the next LOD)
                #if !defined(SHADER_STAGE_RAY_TRACING) && !defined(_TESSELLATION_DISPLACEMENT)
                #ifdef LOD_FADE_CROSSFADE // enable dithering LOD transition if user select CrossFade transition in LOD group
                LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                #endif
                #endif
        
                #ifndef SHADER_UNLIT
                #ifdef _DOUBLESIDED_ON
                    float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
                #else
                    float3 doubleSidedConstants = float3(1.0, 1.0, 1.0);
                #endif
        
                ApplyDoubleSidedFlipOrMirror(fragInputs, doubleSidedConstants); // Apply double sided flip on the vertex normal
                #endif // SHADER_UNLIT
        
                SurfaceDescriptionInputs surfaceDescriptionInputs = FragInputsToSurfaceDescriptionInputs(fragInputs, V);
        
                #if defined(HAVE_VFX_MODIFICATION)
                GraphProperties properties;
                ZERO_INITIALIZE(GraphProperties, properties);
        
                GetElementPixelProperties(fragInputs, properties);
        
                SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs, properties);
                #else
                SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);
                #endif
        
                #ifdef DEBUG_DISPLAY
                if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                {
                    surfaceDescription.Alpha = 1.0f;
                }
                #endif
        
                // Perform alpha test very early to save performance (a killed pixel will not sample textures)
                // TODO: split graph evaluation to grab just alpha dependencies first? tricky..
                #ifdef _ALPHATEST_ON
                    float alphaCutoff = surfaceDescription.AlphaClipThreshold;
                    #if SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_PREPASS
                    // The TransparentDepthPrepass is also used with SSR transparent.
                    // If an artists enable transaprent SSR but not the TransparentDepthPrepass itself, then we use AlphaClipThreshold
                    // otherwise if TransparentDepthPrepass is enabled we use AlphaClipThresholdDepthPrepass
                    #elif SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_POSTPASS
                    // DepthPostpass always use its own alpha threshold
                    alphaCutoff = surfaceDescription.AlphaClipThresholdDepthPostpass;
                    #elif (SHADERPASS == SHADERPASS_SHADOWS) || (SHADERPASS == SHADERPASS_RAYTRACING_VISIBILITY)
                    // If use shadow threshold isn't enable we don't allow any test
                    #endif
        
                    GENERIC_ALPHA_TEST(surfaceDescription.Alpha, alphaCutoff);
                #endif
        
                #if !defined(SHADER_STAGE_RAY_TRACING) && _DEPTHOFFSET_ON
                ApplyDepthOffsetPositionInput(V, surfaceDescription.DepthOffset, GetViewForwardDir(), GetWorldToHClipMatrix(), posInput);
                #endif
        
                #ifndef SHADER_UNLIT
                float3 bentNormalWS;
                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData, bentNormalWS);
        
                #ifdef FRAG_INPUTS_USE_TEXCOORD1
                    float4 lightmapTexCoord1 = fragInputs.texCoord1;
                #else
                    float4 lightmapTexCoord1 = float4(0,0,0,0);
                #endif
        
                #ifdef FRAG_INPUTS_USE_TEXCOORD2
                    float4 lightmapTexCoord2 = fragInputs.texCoord2;
                #else
                    float4 lightmapTexCoord2 = float4(0,0,0,0);
                #endif
        
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                // Builtin Data
                // For back lighting we use the oposite vertex normal
                InitBuiltinData(posInput, alpha, bentNormalWS, -fragInputs.tangentToWorld[2], lightmapTexCoord1, lightmapTexCoord2, builtinData);
        
                #else
                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData);
        
                ZERO_BUILTIN_INITIALIZE(builtinData); // No call to InitBuiltinData as we don't have any lighting
                builtinData.opacity = surfaceDescription.Alpha;
        
                #if defined(DEBUG_DISPLAY)
                    // Light Layers are currently not used for the Unlit shader (because it is not lit)
                    // But Unlit objects do cast shadows according to their rendering layer mask, which is what we want to
                    // display in the light layers visualization mode, therefore we need the renderingLayers
                    builtinData.renderingLayers = GetMeshRenderingLayerMask();
                #endif
        
                #endif // SHADER_UNLIT
        
                #ifdef _ALPHATEST_ON
                    // Used for sharpening by alpha to mask - Alpha to covertage is only used with depth only and forward pass (no shadow pass, no transparent pass)
                    builtinData.alphaClipTreshold = alphaCutoff;
                #endif
        
                // override sampleBakedGI - not used by Unlit
        		// When overriding GI, we need to force the isLightmap flag to make sure we don't add APV (sampled in the lightloop) on top of the overridden value (set at GBuffer stage)
        
                builtinData.emissiveColor = surfaceDescription.Emission;
        
                // Note this will not fully work on transparent surfaces (can check with _SURFACE_TYPE_TRANSPARENT define)
                // We will always overwrite vt feeback with the nearest. So behind transparent surfaces vt will not be resolved
                // This is a limitation of the current MRT approach.
                #ifdef UNITY_VIRTUAL_TEXTURING
                #endif
        
                #if _DEPTHOFFSET_ON
                builtinData.depthOffset = surfaceDescription.DepthOffset;
                #endif
        
                // TODO: We should generate distortion / distortionBlur for non distortion pass
                #if (SHADERPASS == SHADERPASS_DISTORTION)
                builtinData.distortion = surfaceDescription.Distortion;
                builtinData.distortionBlur = surfaceDescription.DistortionBlur;
                #endif
        
                #ifndef SHADER_UNLIT
                // PostInitBuiltinData call ApplyDebugToBuiltinData
                PostInitBuiltinData(V, posInput, surfaceData, builtinData);
                #else
                ApplyDebugToBuiltinData(builtinData);
                #endif
        
                RAY_TRACING_OPTIONAL_ALPHA_TEST_PASS
            }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassRaytracingIndirect.hlsl"
        
            // --------------------------------------------------
            // Visual Effect Vertex Invocations
        
        	#ifdef HAVE_VFX_MODIFICATION
                #if !defined(SHADER_STAGE_RAY_TRACING)
        	    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/VisualEffectVertex.hlsl"
                #else
                #endif
        	#endif
        
            ENDHLSL
        }
        Pass
        {
            Name "VisibilityDXR"
            Tags
            {
                "LightMode" = "VisibilityDXR"
            }
        
            // Render State
            // RenderState: <None>
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma target 5.0
        #pragma raytracing surface_shader
        #pragma only_renderers d3d11 xboxseries ps5
        
            // Keywords
            #pragma multi_compile _ TRANSPARENT_COLOR_SHADOW
        #pragma shader_feature _ _SURFACE_TYPE_TRANSPARENT
            // GraphKeywords: <None>
        
            // Defines
            #define SHADERPASS SHADERPASS_RAYTRACING_VISIBILITY
        #define SUPPORT_GLOBAL_MIP_BIAS 1
        
            // For custom interpolators to inject a substruct definition before FragInputs definition,
            // allowing for FragInputs to capture CI's intended for ShaderGraph's SDI.
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
        
            // TODO: Merge FragInputsVFX substruct with CustomInterpolators.
        	#ifdef HAVE_VFX_MODIFICATION
        	struct FragInputsVFX
            {
                /* WARNING: $splice Could not find named fragment 'FragInputsVFX' */
            };
            #endif
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl" // Required by Tessellation.hlsl
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl" // Required to be include before we include properties as it define DECLARE_STACK_CB
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl" // Required before including properties as it defines UNITY_TEXTURE_STREAMING_DEBUG_VARS
            // Always include Shader Graph version
            // Always include last to avoid double macros
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl" // Need to be here for Gradient struct definition
        
            // --------------------------------------------------
            // Defines
        
            // Attribute
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TANGENT_TO_WORLD
            #define VARYINGS_NEED_TEXCOORD0
        
        
            //Strip down the FragInputs.hlsl (on graphics), so we can only optimize the interpolators we use.
            //if by accident something requests contents of FragInputs.hlsl, it will be caught as a compiler error
            //Frag inputs stripping is only enabled when FRAG_INPUTS_ENABLE_STRIPPING is set
            #if !defined(SHADER_STAGE_RAY_TRACING) && SHADERPASS != SHADERPASS_RAYTRACING_GBUFFER && SHADERPASS != SHADERPASS_FULL_SCREEN_DEBUG
            #define FRAG_INPUTS_ENABLE_STRIPPING
            #endif
            #define FRAG_INPUTS_USE_TEXCOORD0
        
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
        
        
        
        
            // Following two define are a workaround introduce in 10.1.x for RaytracingQualityNode
            // The ShaderGraph don't support correctly migration of this node as it serialize all the node data
            // in the json file making it impossible to uprgrade. Until we get a fix, we do a workaround here
            // to still allow us to rename the field and keyword of this node without breaking existing code.
            #ifdef RAYTRACING_SHADER_GRAPH_DEFAULT
            #define RAYTRACING_SHADER_GRAPH_HIGH
            #endif
        
            #ifdef RAYTRACING_SHADER_GRAPH_RAYTRACED
            #define RAYTRACING_SHADER_GRAPH_LOW
            #endif
            // end
        
            #ifndef SHADER_UNLIT
            // We need isFrontFace when using double sided - it is not required for unlit as in case of unlit double sided only drive the cullmode
            // VARYINGS_NEED_CULLFACE can be define by VaryingsMeshToPS.FaceSign input if a IsFrontFace Node is included in the shader graph.
            #if defined(_DOUBLESIDED_ON) && !defined(VARYINGS_NEED_CULLFACE)
                #define VARYINGS_NEED_CULLFACE
            #endif
            #endif
        
            // Specific Material Define
        // Setup a define to say we are an unlit shader
        #define SHADER_UNLIT
        
        // Following Macro are only used by Unlit material
        #if defined(_ENABLE_SHADOW_MATTE)
            #if SHADERPASS == SHADERPASS_FORWARD_UNLIT
                #pragma multi_compile_fragment USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST
            #elif SHADERPASS == SHADERPASS_PATH_TRACING
                #define LIGHTLOOP_DISABLE_TILE_AND_CLUSTER
            #endif
        
        // We don't want to have the lightloop defined for the ray tracing passes, but we do for the rasterisation and path tracing shader passes.
        #if !defined(SHADER_STAGE_RAY_TRACING) || SHADERPASS == SHADERPASS_PATH_TRACING
            #define HAS_LIGHTLOOP
        #endif
        #endif
            // Caution: we can use the define SHADER_UNLIT onlit after the above Material include as it is the Unlit template who define it
        
            // To handle SSR on transparent correctly with a possibility to enable/disable it per framesettings
            // we should have a code like this:
            // if !defined(_DISABLE_SSR_TRANSPARENT)
            // pragma multi_compile _ WRITE_NORMAL_BUFFER
            // endif
            // i.e we enable the multicompile only if we can receive SSR or not, and then C# code drive
            // it based on if SSR transparent in frame settings and not (and stripper can strip it).
            // this is currently not possible with our current preprocessor as _DISABLE_SSR_TRANSPARENT is a keyword not a define
            // so instead we used this and chose to pay the extra cost of normal write even if SSR transaprent is disabled.
            // Ideally the shader graph generator should handle it but condition below can't be handle correctly for now.
            #if SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_PREPASS
            #if !defined(_DISABLE_SSR_TRANSPARENT) && !defined(SHADER_UNLIT)
                #define WRITE_NORMAL_BUFFER
            #endif
            #endif
        
            // See Lit.shader
            #if SHADERPASS == SHADERPASS_MOTION_VECTORS && defined(WRITE_DECAL_BUFFER_AND_RENDERING_LAYER)
                #define WRITE_DECAL_BUFFER
            #endif
        
            #ifndef DEBUG_DISPLAY
                // In case of opaque we don't want to perform the alpha test, it is done in depth prepass and we use depth equal for ztest (setup from UI)
                // Don't do it with debug display mode as it is possible there is no depth prepass in this case
                #if !defined(_SURFACE_TYPE_TRANSPARENT)
                    #if SHADERPASS == SHADERPASS_FORWARD
                    #define SHADERPASS_FORWARD_BYPASS_ALPHA_TEST
                    #elif SHADERPASS == SHADERPASS_GBUFFER
                    #define SHADERPASS_GBUFFER_BYPASS_ALPHA_TEST
                    #endif
                #endif
            #endif
        
            // Define _DEFERRED_CAPABLE_MATERIAL for shader capable to run in deferred pass
            #if defined(SHADER_LIT) && !defined(_SURFACE_TYPE_TRANSPARENT)
                #define _DEFERRED_CAPABLE_MATERIAL
            #endif
        
            // Translate transparent motion vector define
            #if (defined(_TRANSPARENT_WRITES_MOTION_VEC) || defined(_TRANSPARENT_REFRACTIVE_SORT)) && defined(_SURFACE_TYPE_TRANSPARENT)
                #define _WRITE_TRANSPARENT_MOTION_VECTOR
            #endif
        
            // -- Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _VoidColor;
        float _Speed;
        float _FresnelPower;
        float4 _BorderColor;
        float2 _Seed;
        float4 _TwirlTexture_TexelSize;
        float _InnerEdge;
        float4 _EmissionColor;
        float _UseShadowThreshold;
        float4 _DoubleSidedConstants;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        float _BlendMode;
        float _EnableBlendModePreserveSpecularLighting;
        CBUFFER_END
        
        
        // Object and Global properties
        TEXTURE2D(_TwirlTexture);
        SAMPLER(sampler_TwirlTexture);
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Includes
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/RaytracingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/ShaderVariablesRaytracing.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/ShaderVariablesRaytracingLightLoop.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/RaytracingIntersection.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/Unlit.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/UnlitRaytracing.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/RayTracingCommon.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/ClassicNoise3D.hlsl"
        #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi3D.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonLighting.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            struct SurfaceDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 WorldSpaceNormal;
             float3 ObjectSpaceViewDirection;
             float3 WorldSpaceViewDirection;
             float3 AbsoluteWorldSpacePosition;
             float4 uv0;
             float3 TimeParameters;
        };
        
            //Interpolator Packs: <None>
        
            // --------------------------------------------------
            // Graph
        
        
            // Graph Functions
            
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_DotProduct_float3(float3 A, float3 B, out float Out)
        {
            Out = dot(A, B);
        }
        
        void Unity_SampleGradientV1_float(Gradient Gradient, float Time, out float4 Out)
        {
            // convert to OkLab if we need perceptual color space.
            float3 color = lerp(Gradient.colors[0].rgb, LinearToOklab(Gradient.colors[0].rgb), Gradient.type == 2);
        
            [unroll]
            for (int c = 1; c < Gradient.colorsLength; c++)
            {
                float colorPos = saturate((Time - Gradient.colors[c - 1].w) / (Gradient.colors[c].w - Gradient.colors[c - 1].w)) * step(c, Gradient.colorsLength - 1);
                float3 color2 = lerp(Gradient.colors[c].rgb, LinearToOklab(Gradient.colors[c].rgb), Gradient.type == 2);
                color = lerp(color, color2, lerp(colorPos, step(0.01, colorPos), Gradient.type % 2)); // grad.type == 1 is fixed, 0 and 2 are blends.
            }
            color = lerp(color, OklabToLinear(color), Gradient.type == 2);
        
        #ifdef UNITY_COLORSPACE_GAMMA
            color = LinearToSRGB(color);
        #endif
        
            float alpha = Gradient.alphas[0].x;
            [unroll]
            for (int a = 1; a < Gradient.alphasLength; a++)
            {
                float alphaPos = saturate((Time - Gradient.alphas[a - 1].y) / (Gradient.alphas[a].y - Gradient.alphas[a - 1].y)) * step(a, Gradient.alphasLength - 1);
                alpha = lerp(alpha, Gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), Gradient.type % 2));
            }
        
            Out = float4(color, alpha);
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
        Out = A * B;
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        struct Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float
        {
        };
        
        void SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(float3 Vector3_7940555B, float Vector1_1B8B9078, Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float IN, out float Value_0)
        {
        float3 _Property_44999cc87708de82a26b39ae1da975ec_Out_0_Vector3 = Vector3_7940555B;
        float _Property_dad5add45a7fa785be976f925bc5a5da_Out_0_Float = Vector1_1B8B9078;
        float3 _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3;
        Unity_Multiply_float3_float3(_Property_44999cc87708de82a26b39ae1da975ec_Out_0_Vector3, (_Property_dad5add45a7fa785be976f925bc5a5da_Out_0_Float.xxx), _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3);
        float _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float;
        PerlinNoise3D_float(_Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3, _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float);
        float _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float;
        Unity_Remap_float(_PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float, float2 (-1.15, 1.15), float2 (0, 1), _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float);
        Value_0 = _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_RandomRange_float(float2 Seed, float Min, float Max, out float Out)
        {
             float randomno =  frac(sin(dot(Seed, float2(12.9898, 78.233)))*43758.5453);
             Out = lerp(Min, Max, randomno);
        }
        
        struct Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float
        {
        };
        
        void SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(float3 Vector3_375394F7, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float IN, out float Value_1, out float Cells_2)
        {
        float3 _Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3 = Vector3_375394F7;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        VoronoiPrecise3D_float(_Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }
        
        float3 Unity_HDRP_GetEmissionHDRColor_float(float3 ldrColor, float luminanceIntensity, float exposureWeight)
        {
            float3 hdrColor = ldrColor * luminanceIntensity;
        
            #ifdef SHADERGRAPH_PREVIEW
            float inverseExposureMultiplier = 1.0;
            #else
            float inverseExposureMultiplier = GetInverseCurrentExposureMultiplier();
            #endif
        
            // Inverse pre-expose using _EmissiveExposureWeight weight
            hdrColor = lerp(hdrColor * inverseExposureMultiplier, hdrColor, exposureWeight);
            return hdrColor;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
            // Graph Vertex
            // GraphVertex: <None>
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float3 Emission;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_d09e7f2c2a774dbd93cc943fa6a50940_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_VoidColor) : _VoidColor;
            float _Property_16eef79ff58443149895b35d0a88bdb2_Out_0_Float = _InnerEdge;
            float4 _UV_ce55b36e72754a339073e4c05a64b551_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_054563945a0a4f688fce11f0c067b0c8_Out_1_Vector2 = _UV_ce55b36e72754a339073e4c05a64b551_Out_0_Vector4.xy;
            float _Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float;
            Unity_Distance_float2(_Swizzle_054563945a0a4f688fce11f0c067b0c8_Out_1_Vector2, float2(0.5, 0.5), _Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float);
            float _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float;
            Unity_OneMinus_float(_Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float, _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float);
            float _Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float;
            Unity_Smoothstep_float(_Property_16eef79ff58443149895b35d0a88bdb2_Out_0_Float, float(1), _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float, _Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float);
            Gradient _Gradient_c34cf660310448768ec81f4600b23ad8_Out_0_Gradient = NewGradient(0, 4, 2, float4(0, 0, 0, 0.271931),float4(0.01568037, 0.01568037, 0.01568037, 0.4678416),float4(0.1071531, 0.1071531, 0.1071531, 0.6052644),float4(1, 1, 1, 1),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0), float2(1, 0),float2(1, 1),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0));
            float _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float;
            Unity_DotProduct_float3(IN.ObjectSpaceViewDirection, IN.ObjectSpaceNormal, _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float);
            float4 _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4;
            Unity_SampleGradientV1_float(_Gradient_c34cf660310448768ec81f4600b23ad8_Out_0_Gradient, _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float, _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4);
            float4 _Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float.xxxx), _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4, _Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4);
            Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a;
            float _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float;
            SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(IN.ObjectSpaceViewDirection, float(10), _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a, _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float);
            float3 _Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3;
            Unity_Multiply_float3_float3(IN.ObjectSpaceViewDirection, (_Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float.xxx), _Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3);
            float _Property_ba2d1376c19c40fbadc820d0fb721b2e_Out_0_Float = _Speed;
            float _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_ba2d1376c19c40fbadc820d0fb721b2e_Out_0_Float, _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float);
            float2 _Property_2cfd9c61936548ee85383d83df6beba0_Out_0_Vector2 = _Seed;
            float _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float;
            Unity_RandomRange_float(_Property_2cfd9c61936548ee85383d83df6beba0_Out_0_Vector2, float(26.96), float(45.6), _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float);
            Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a;
            float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Value_1_Float;
            float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float;
            SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(_Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3, _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float, _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Value_1_Float, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float);
            float4 _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4, (_Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float.xxxx), _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4);
            float4 _Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_d09e7f2c2a774dbd93cc943fa6a50940_Out_0_Vector4, _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4, _Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4);
            float4 _Property_e0f9565d1eaa452fb638faa34cc517eb_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_BorderColor) : _BorderColor;
            float3 _Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3;
            Unity_Absolute_float3(IN.AbsoluteWorldSpacePosition, _Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3);
            Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1;
            float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Value_1_Float;
            float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float;
            SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(_Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3, IN.TimeParameters.x, float(6.2), _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1, _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Value_1_Float, _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float);
            float _Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float;
            Unity_Smoothstep_float(float(-0.41), float(1), _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float, _Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float);
            float _Property_82bfb359e6494ff08a54e8c08976a6f1_Out_0_Float = _FresnelPower;
            float _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_82bfb359e6494ff08a54e8c08976a6f1_Out_0_Float, _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float);
            float _Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float, _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float, _Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float);
            float4 _Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_e0f9565d1eaa452fb638faa34cc517eb_Out_0_Vector4, (_Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float.xxxx), _Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4);
            float3 _EmissionNode_87074dafd5ff4d39bf4005b1089a56e4_Output_0_Vector3 = Unity_HDRP_GetEmissionHDRColor_float((_Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4.xyz).xyz, float(3), float(0));
            float4 _Add_0b088d988049415c84334d987e692959_Out_2_Vector4;
            Unity_Add_float4(_Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4, (_Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float.xxxx), _Add_0b088d988049415c84334d987e692959_Out_2_Vector4);
            surface.BaseColor = (_Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4.xyz);
            surface.Emission = _EmissionNode_87074dafd5ff4d39bf4005b1089a56e4_Output_0_Vector3;
            surface.Alpha = (_Add_0b088d988049415c84334d987e692959_Out_2_Vector4).x;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES AttributesMesh
            #define VaryingsMeshType VaryingsMeshToPS
            #define VFX_SRP_VARYINGS VaryingsMeshType
            #define VFX_SRP_SURFACE_INPUTS FragInputs
            #endif
            SurfaceDescriptionInputs FragInputsToSurfaceDescriptionInputs(FragInputs input, float3 viewWS)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            output.WorldSpaceNormal =                           normalize(input.tangentToWorld[2].xyz);
            #if defined(SHADER_STAGE_RAY_TRACING)
            output.ObjectSpaceNormal =                          mul(output.WorldSpaceNormal, (float3x3) ObjectToWorld3x4());
            #else
            output.ObjectSpaceNormal =                          normalize(mul(output.WorldSpaceNormal, (float3x3) UNITY_MATRIX_M));           // transposed multiplication by inverse matrix to handle normal scale
            #endif
            output.WorldSpaceViewDirection =                    normalize(viewWS);
            output.ObjectSpaceViewDirection =                   TransformWorldToObjectDir(output.WorldSpaceViewDirection);
        #if SHADERPASS != SHADERPASS_FOG_VOLUME_VOXELIZATION
        #else
        #endif
            output.AbsoluteWorldSpacePosition =                 GetAbsolutePositionWS(input.positionRWS);
        #if SHADERPASS != SHADERPASS_FOG_VOLUME_VOXELIZATION
        #else
        #endif
        
        #if UNITY_UV_STARTS_AT_TOP
        #else
        #endif
        
        
            output.uv0 =                                        input.texCoord0;
            output.TimeParameters =                             _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        
            // splice point to copy frag inputs custom interpolator pack into the SDI
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorCopyToSDI' */
        
            return output;
        }
        
            // --------------------------------------------------
            // Build Surface Data (Specific Material)
        
        void ApplyDecalToSurfaceDataNoNormal(DecalSurfaceData decalSurfaceData, inout SurfaceData surfaceData);
        
        void ApplyDecalAndGetNormal(FragInputs fragInputs, PositionInputs posInput, SurfaceDescription surfaceDescription,
            inout SurfaceData surfaceData)
        {
            float3 doubleSidedConstants = GetDoubleSidedConstants();
        
        #ifdef DECAL_NORMAL_BLENDING
            // SG nodes don't ouptut surface gradients, so if decals require surf grad blending, we have to convert
            // the normal to gradient before applying the decal. We then have to resolve the gradient back to world space
            float3 normalTS;
        
        
        
            #if HAVE_DECALS
            if (_EnableDecals)
            {
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs, alpha);
                ApplyDecalToSurfaceNormal(decalSurfaceData, fragInputs.tangentToWorld[2], normalTS);
                ApplyDecalToSurfaceDataNoNormal(decalSurfaceData, surfaceData);
            }
            #endif
        
            GetNormalWS_SG(fragInputs, normalTS, surfaceData.normalWS, doubleSidedConstants);
        #else
            // normal delivered to master node
        
            #if HAVE_DECALS
            if (_EnableDecals)
            {
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                // Both uses and modifies 'surfaceData.normalWS'.
                DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs, alpha);
                ApplyDecalToSurfaceNormal(decalSurfaceData, surfaceData.normalWS.xyz);
                ApplyDecalToSurfaceDataNoNormal(decalSurfaceData, surfaceData);
            }
            #endif
        #endif
        }
        void BuildSurfaceData(FragInputs fragInputs, inout SurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData)
        {
            // setup defaults -- these are used if the graph doesn't output a value
            ZERO_INITIALIZE(SurfaceData, surfaceData);
        
            // copy across graph values, if defined
            surfaceData.color = surfaceDescription.BaseColor;
        
            #ifdef WRITE_NORMAL_BUFFER
            // When we need to export the normal (in the depth prepass, we write the geometry one)
            surfaceData.normalWS = fragInputs.tangentToWorld[2];
            #endif
        
            #if defined(DEBUG_DISPLAY)
            #if !defined(SHADER_STAGE_RAY_TRACING)
            // Mipmap mode debugging isn't supported with ray tracing as it relies on derivatives
                if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                {
                    #ifdef FRAG_INPUTS_USE_TEXCOORD0
                        surfaceData.color = GET_TEXTURE_STREAMING_DEBUG(posInput.positionSS, fragInputs.texCoord0);
                    #else
                        surfaceData.color = GET_TEXTURE_STREAMING_DEBUG_NO_UV(posInput.positionSS);
                    #endif
                }
            #endif
            #endif
        
            #ifdef _ENABLE_SHADOW_MATTE
        
                #if (SHADERPASS == SHADERPASS_FORWARD_UNLIT) || (SHADERPASS == SHADERPASS_RAYTRACING_GBUFFER) || (SHADERPASS == SHADERPASS_RAYTRACING_INDIRECT) || (SHADERPASS == SHADERPASS_RAYTRACING_FORWARD)
        
                    HDShadowContext shadowContext = InitShadowContext();
        
                    // Evaluate the shadow, the normal is guaranteed if shadow matte is enabled on this shader.
                    float3 shadow3;
                    ShadowLoopMin(shadowContext, posInput, normalize(fragInputs.tangentToWorld[2]), asuint(_ShadowMatteFilter), GetMeshRenderingLayerMask(), shadow3);
        
                    // Compute the average value in the fourth channel
                    float4 shadow = float4(shadow3, dot(shadow3, float3(1.0/3.0, 1.0/3.0, 1.0/3.0)));
        
                    float4 shadowColor = (1.0 - shadow) * surfaceDescription.ShadowTint.rgba;
                    float  localAlpha  = saturate(shadowColor.a + surfaceDescription.Alpha);
        
                    // Keep the nested lerp
                    // With no Color (bsdfData.color.rgb, bsdfData.color.a == 0.0f), just use ShadowColor*Color to avoid a ring of "white" around the shadow
                    // And mix color to consider the Color & ShadowColor alpha (from texture or/and color picker)
                    #ifdef _SURFACE_TYPE_TRANSPARENT
                        surfaceData.color = lerp(shadowColor.rgb * surfaceData.color, lerp(lerp(shadowColor.rgb, surfaceData.color, 1.0 - surfaceDescription.ShadowTint.a), surfaceData.color, shadow.rgb), surfaceDescription.Alpha);
                    #else
                        surfaceData.color = lerp(lerp(shadowColor.rgb, surfaceData.color, 1.0 - surfaceDescription.ShadowTint.a), surfaceData.color, shadow.rgb);
                    #endif
                    localAlpha = ApplyBlendMode(surfaceData.color, localAlpha).a;
        
                    surfaceDescription.Alpha = localAlpha;
        
                #elif SHADERPASS == SHADERPASS_PATH_TRACING
        
                    surfaceData.normalWS = fragInputs.tangentToWorld[2];
                    surfaceData.shadowTint = surfaceDescription.ShadowTint.rgba;
        
                #endif
        
            #endif // _ENABLE_SHADOW_MATTE
        }
        
            // --------------------------------------------------
            // Get Surface And BuiltinData
        
            void GetSurfaceAndBuiltinData(FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData RAY_TRACING_OPTIONAL_PARAMETERS)
            {
                // Don't dither if displaced tessellation (we're fading out the displacement instead to match the next LOD)
                #if !defined(SHADER_STAGE_RAY_TRACING) && !defined(_TESSELLATION_DISPLACEMENT)
                #ifdef LOD_FADE_CROSSFADE // enable dithering LOD transition if user select CrossFade transition in LOD group
                LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                #endif
                #endif
        
                #ifndef SHADER_UNLIT
                #ifdef _DOUBLESIDED_ON
                    float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
                #else
                    float3 doubleSidedConstants = float3(1.0, 1.0, 1.0);
                #endif
        
                ApplyDoubleSidedFlipOrMirror(fragInputs, doubleSidedConstants); // Apply double sided flip on the vertex normal
                #endif // SHADER_UNLIT
        
                SurfaceDescriptionInputs surfaceDescriptionInputs = FragInputsToSurfaceDescriptionInputs(fragInputs, V);
        
                #if defined(HAVE_VFX_MODIFICATION)
                GraphProperties properties;
                ZERO_INITIALIZE(GraphProperties, properties);
        
                GetElementPixelProperties(fragInputs, properties);
        
                SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs, properties);
                #else
                SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);
                #endif
        
                #ifdef DEBUG_DISPLAY
                if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                {
                    surfaceDescription.Alpha = 1.0f;
                }
                #endif
        
                // Perform alpha test very early to save performance (a killed pixel will not sample textures)
                // TODO: split graph evaluation to grab just alpha dependencies first? tricky..
                #ifdef _ALPHATEST_ON
                    float alphaCutoff = surfaceDescription.AlphaClipThreshold;
                    #if SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_PREPASS
                    // The TransparentDepthPrepass is also used with SSR transparent.
                    // If an artists enable transaprent SSR but not the TransparentDepthPrepass itself, then we use AlphaClipThreshold
                    // otherwise if TransparentDepthPrepass is enabled we use AlphaClipThresholdDepthPrepass
                    #elif SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_POSTPASS
                    // DepthPostpass always use its own alpha threshold
                    alphaCutoff = surfaceDescription.AlphaClipThresholdDepthPostpass;
                    #elif (SHADERPASS == SHADERPASS_SHADOWS) || (SHADERPASS == SHADERPASS_RAYTRACING_VISIBILITY)
                    // If use shadow threshold isn't enable we don't allow any test
                    #endif
        
                    GENERIC_ALPHA_TEST(surfaceDescription.Alpha, alphaCutoff);
                #endif
        
                #if !defined(SHADER_STAGE_RAY_TRACING) && _DEPTHOFFSET_ON
                ApplyDepthOffsetPositionInput(V, surfaceDescription.DepthOffset, GetViewForwardDir(), GetWorldToHClipMatrix(), posInput);
                #endif
        
                #ifndef SHADER_UNLIT
                float3 bentNormalWS;
                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData, bentNormalWS);
        
                #ifdef FRAG_INPUTS_USE_TEXCOORD1
                    float4 lightmapTexCoord1 = fragInputs.texCoord1;
                #else
                    float4 lightmapTexCoord1 = float4(0,0,0,0);
                #endif
        
                #ifdef FRAG_INPUTS_USE_TEXCOORD2
                    float4 lightmapTexCoord2 = fragInputs.texCoord2;
                #else
                    float4 lightmapTexCoord2 = float4(0,0,0,0);
                #endif
        
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                // Builtin Data
                // For back lighting we use the oposite vertex normal
                InitBuiltinData(posInput, alpha, bentNormalWS, -fragInputs.tangentToWorld[2], lightmapTexCoord1, lightmapTexCoord2, builtinData);
        
                #else
                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData);
        
                ZERO_BUILTIN_INITIALIZE(builtinData); // No call to InitBuiltinData as we don't have any lighting
                builtinData.opacity = surfaceDescription.Alpha;
        
                #if defined(DEBUG_DISPLAY)
                    // Light Layers are currently not used for the Unlit shader (because it is not lit)
                    // But Unlit objects do cast shadows according to their rendering layer mask, which is what we want to
                    // display in the light layers visualization mode, therefore we need the renderingLayers
                    builtinData.renderingLayers = GetMeshRenderingLayerMask();
                #endif
        
                #endif // SHADER_UNLIT
        
                #ifdef _ALPHATEST_ON
                    // Used for sharpening by alpha to mask - Alpha to covertage is only used with depth only and forward pass (no shadow pass, no transparent pass)
                    builtinData.alphaClipTreshold = alphaCutoff;
                #endif
        
                // override sampleBakedGI - not used by Unlit
        		// When overriding GI, we need to force the isLightmap flag to make sure we don't add APV (sampled in the lightloop) on top of the overridden value (set at GBuffer stage)
        
                builtinData.emissiveColor = surfaceDescription.Emission;
        
                // Note this will not fully work on transparent surfaces (can check with _SURFACE_TYPE_TRANSPARENT define)
                // We will always overwrite vt feeback with the nearest. So behind transparent surfaces vt will not be resolved
                // This is a limitation of the current MRT approach.
                #ifdef UNITY_VIRTUAL_TEXTURING
                #endif
        
                #if _DEPTHOFFSET_ON
                builtinData.depthOffset = surfaceDescription.DepthOffset;
                #endif
        
                // TODO: We should generate distortion / distortionBlur for non distortion pass
                #if (SHADERPASS == SHADERPASS_DISTORTION)
                builtinData.distortion = surfaceDescription.Distortion;
                builtinData.distortionBlur = surfaceDescription.DistortionBlur;
                #endif
        
                #ifndef SHADER_UNLIT
                // PostInitBuiltinData call ApplyDebugToBuiltinData
                PostInitBuiltinData(V, posInput, surfaceData, builtinData);
                #else
                ApplyDebugToBuiltinData(builtinData);
                #endif
        
                RAY_TRACING_OPTIONAL_ALPHA_TEST_PASS
            }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassRaytracingVisibility.hlsl"
        
            // --------------------------------------------------
            // Visual Effect Vertex Invocations
        
        	#ifdef HAVE_VFX_MODIFICATION
                #if !defined(SHADER_STAGE_RAY_TRACING)
        	    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/VisualEffectVertex.hlsl"
                #else
                #endif
        	#endif
        
            ENDHLSL
        }
        Pass
        {
            Name "ForwardDXR"
            Tags
            {
                "LightMode" = "ForwardDXR"
            }
        
            // Render State
            // RenderState: <None>
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma target 5.0
        #pragma raytracing surface_shader
        #pragma only_renderers d3d11 xboxseries ps5
        
            // Keywords
            #pragma shader_feature _ _SURFACE_TYPE_TRANSPARENT
        #pragma multi_compile _ DEBUG_DISPLAY
            // GraphKeywords: <None>
        
            // Defines
            #define SHADERPASS SHADERPASS_RAYTRACING_FORWARD
        #define PUNCTUAL_SHADOW_LOW
        #define DIRECTIONAL_SHADOW_LOW
        #define RAYTRACING_SHADER_GRAPH_RAYTRACED
        #define SUPPORT_GLOBAL_MIP_BIAS 1
        
            // For custom interpolators to inject a substruct definition before FragInputs definition,
            // allowing for FragInputs to capture CI's intended for ShaderGraph's SDI.
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
        
            // TODO: Merge FragInputsVFX substruct with CustomInterpolators.
        	#ifdef HAVE_VFX_MODIFICATION
        	struct FragInputsVFX
            {
                /* WARNING: $splice Could not find named fragment 'FragInputsVFX' */
            };
            #endif
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl" // Required by Tessellation.hlsl
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl" // Required to be include before we include properties as it define DECLARE_STACK_CB
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl" // Required before including properties as it defines UNITY_TEXTURE_STREAMING_DEBUG_VARS
            // Always include Shader Graph version
            // Always include last to avoid double macros
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl" // Need to be here for Gradient struct definition
        
            // --------------------------------------------------
            // Defines
        
            // Attribute
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TANGENT_TO_WORLD
            #define VARYINGS_NEED_TEXCOORD0
        
        
            //Strip down the FragInputs.hlsl (on graphics), so we can only optimize the interpolators we use.
            //if by accident something requests contents of FragInputs.hlsl, it will be caught as a compiler error
            //Frag inputs stripping is only enabled when FRAG_INPUTS_ENABLE_STRIPPING is set
            #if !defined(SHADER_STAGE_RAY_TRACING) && SHADERPASS != SHADERPASS_RAYTRACING_GBUFFER && SHADERPASS != SHADERPASS_FULL_SCREEN_DEBUG
            #define FRAG_INPUTS_ENABLE_STRIPPING
            #endif
            #define FRAG_INPUTS_USE_TEXCOORD0
        
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
        
        
        
        
            // Following two define are a workaround introduce in 10.1.x for RaytracingQualityNode
            // The ShaderGraph don't support correctly migration of this node as it serialize all the node data
            // in the json file making it impossible to uprgrade. Until we get a fix, we do a workaround here
            // to still allow us to rename the field and keyword of this node without breaking existing code.
            #ifdef RAYTRACING_SHADER_GRAPH_DEFAULT
            #define RAYTRACING_SHADER_GRAPH_HIGH
            #endif
        
            #ifdef RAYTRACING_SHADER_GRAPH_RAYTRACED
            #define RAYTRACING_SHADER_GRAPH_LOW
            #endif
            // end
        
            #ifndef SHADER_UNLIT
            // We need isFrontFace when using double sided - it is not required for unlit as in case of unlit double sided only drive the cullmode
            // VARYINGS_NEED_CULLFACE can be define by VaryingsMeshToPS.FaceSign input if a IsFrontFace Node is included in the shader graph.
            #if defined(_DOUBLESIDED_ON) && !defined(VARYINGS_NEED_CULLFACE)
                #define VARYINGS_NEED_CULLFACE
            #endif
            #endif
        
            // Specific Material Define
        // Setup a define to say we are an unlit shader
        #define SHADER_UNLIT
        
        // Following Macro are only used by Unlit material
        #if defined(_ENABLE_SHADOW_MATTE)
            #if SHADERPASS == SHADERPASS_FORWARD_UNLIT
                #pragma multi_compile_fragment USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST
            #elif SHADERPASS == SHADERPASS_PATH_TRACING
                #define LIGHTLOOP_DISABLE_TILE_AND_CLUSTER
            #endif
        
        // We don't want to have the lightloop defined for the ray tracing passes, but we do for the rasterisation and path tracing shader passes.
        #if !defined(SHADER_STAGE_RAY_TRACING) || SHADERPASS == SHADERPASS_PATH_TRACING
            #define HAS_LIGHTLOOP
        #endif
        #endif
            // Caution: we can use the define SHADER_UNLIT onlit after the above Material include as it is the Unlit template who define it
        
            // To handle SSR on transparent correctly with a possibility to enable/disable it per framesettings
            // we should have a code like this:
            // if !defined(_DISABLE_SSR_TRANSPARENT)
            // pragma multi_compile _ WRITE_NORMAL_BUFFER
            // endif
            // i.e we enable the multicompile only if we can receive SSR or not, and then C# code drive
            // it based on if SSR transparent in frame settings and not (and stripper can strip it).
            // this is currently not possible with our current preprocessor as _DISABLE_SSR_TRANSPARENT is a keyword not a define
            // so instead we used this and chose to pay the extra cost of normal write even if SSR transaprent is disabled.
            // Ideally the shader graph generator should handle it but condition below can't be handle correctly for now.
            #if SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_PREPASS
            #if !defined(_DISABLE_SSR_TRANSPARENT) && !defined(SHADER_UNLIT)
                #define WRITE_NORMAL_BUFFER
            #endif
            #endif
        
            // See Lit.shader
            #if SHADERPASS == SHADERPASS_MOTION_VECTORS && defined(WRITE_DECAL_BUFFER_AND_RENDERING_LAYER)
                #define WRITE_DECAL_BUFFER
            #endif
        
            #ifndef DEBUG_DISPLAY
                // In case of opaque we don't want to perform the alpha test, it is done in depth prepass and we use depth equal for ztest (setup from UI)
                // Don't do it with debug display mode as it is possible there is no depth prepass in this case
                #if !defined(_SURFACE_TYPE_TRANSPARENT)
                    #if SHADERPASS == SHADERPASS_FORWARD
                    #define SHADERPASS_FORWARD_BYPASS_ALPHA_TEST
                    #elif SHADERPASS == SHADERPASS_GBUFFER
                    #define SHADERPASS_GBUFFER_BYPASS_ALPHA_TEST
                    #endif
                #endif
            #endif
        
            // Define _DEFERRED_CAPABLE_MATERIAL for shader capable to run in deferred pass
            #if defined(SHADER_LIT) && !defined(_SURFACE_TYPE_TRANSPARENT)
                #define _DEFERRED_CAPABLE_MATERIAL
            #endif
        
            // Translate transparent motion vector define
            #if (defined(_TRANSPARENT_WRITES_MOTION_VEC) || defined(_TRANSPARENT_REFRACTIVE_SORT)) && defined(_SURFACE_TYPE_TRANSPARENT)
                #define _WRITE_TRANSPARENT_MOTION_VECTOR
            #endif
        
            // -- Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _VoidColor;
        float _Speed;
        float _FresnelPower;
        float4 _BorderColor;
        float2 _Seed;
        float4 _TwirlTexture_TexelSize;
        float _InnerEdge;
        float4 _EmissionColor;
        float _UseShadowThreshold;
        float4 _DoubleSidedConstants;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        float _BlendMode;
        float _EnableBlendModePreserveSpecularLighting;
        CBUFFER_END
        
        
        // Object and Global properties
        TEXTURE2D(_TwirlTexture);
        SAMPLER(sampler_TwirlTexture);
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Includes
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/RaytracingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/ShaderVariablesRaytracing.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/ShaderVariablesRaytracingLightLoop.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/RaytracingIntersection.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/Unlit.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/UnlitRaytracing.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/RayTracingCommon.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/ClassicNoise3D.hlsl"
        #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi3D.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonLighting.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            struct SurfaceDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 WorldSpaceNormal;
             float3 ObjectSpaceViewDirection;
             float3 WorldSpaceViewDirection;
             float3 AbsoluteWorldSpacePosition;
             float4 uv0;
             float3 TimeParameters;
        };
        
            //Interpolator Packs: <None>
        
            // --------------------------------------------------
            // Graph
        
        
            // Graph Functions
            
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_DotProduct_float3(float3 A, float3 B, out float Out)
        {
            Out = dot(A, B);
        }
        
        void Unity_SampleGradientV1_float(Gradient Gradient, float Time, out float4 Out)
        {
            // convert to OkLab if we need perceptual color space.
            float3 color = lerp(Gradient.colors[0].rgb, LinearToOklab(Gradient.colors[0].rgb), Gradient.type == 2);
        
            [unroll]
            for (int c = 1; c < Gradient.colorsLength; c++)
            {
                float colorPos = saturate((Time - Gradient.colors[c - 1].w) / (Gradient.colors[c].w - Gradient.colors[c - 1].w)) * step(c, Gradient.colorsLength - 1);
                float3 color2 = lerp(Gradient.colors[c].rgb, LinearToOklab(Gradient.colors[c].rgb), Gradient.type == 2);
                color = lerp(color, color2, lerp(colorPos, step(0.01, colorPos), Gradient.type % 2)); // grad.type == 1 is fixed, 0 and 2 are blends.
            }
            color = lerp(color, OklabToLinear(color), Gradient.type == 2);
        
        #ifdef UNITY_COLORSPACE_GAMMA
            color = LinearToSRGB(color);
        #endif
        
            float alpha = Gradient.alphas[0].x;
            [unroll]
            for (int a = 1; a < Gradient.alphasLength; a++)
            {
                float alphaPos = saturate((Time - Gradient.alphas[a - 1].y) / (Gradient.alphas[a].y - Gradient.alphas[a - 1].y)) * step(a, Gradient.alphasLength - 1);
                alpha = lerp(alpha, Gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), Gradient.type % 2));
            }
        
            Out = float4(color, alpha);
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
        Out = A * B;
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        struct Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float
        {
        };
        
        void SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(float3 Vector3_7940555B, float Vector1_1B8B9078, Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float IN, out float Value_0)
        {
        float3 _Property_44999cc87708de82a26b39ae1da975ec_Out_0_Vector3 = Vector3_7940555B;
        float _Property_dad5add45a7fa785be976f925bc5a5da_Out_0_Float = Vector1_1B8B9078;
        float3 _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3;
        Unity_Multiply_float3_float3(_Property_44999cc87708de82a26b39ae1da975ec_Out_0_Vector3, (_Property_dad5add45a7fa785be976f925bc5a5da_Out_0_Float.xxx), _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3);
        float _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float;
        PerlinNoise3D_float(_Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3, _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float);
        float _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float;
        Unity_Remap_float(_PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float, float2 (-1.15, 1.15), float2 (0, 1), _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float);
        Value_0 = _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_RandomRange_float(float2 Seed, float Min, float Max, out float Out)
        {
             float randomno =  frac(sin(dot(Seed, float2(12.9898, 78.233)))*43758.5453);
             Out = lerp(Min, Max, randomno);
        }
        
        struct Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float
        {
        };
        
        void SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(float3 Vector3_375394F7, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float IN, out float Value_1, out float Cells_2)
        {
        float3 _Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3 = Vector3_375394F7;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        VoronoiPrecise3D_float(_Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }
        
        float3 Unity_HDRP_GetEmissionHDRColor_float(float3 ldrColor, float luminanceIntensity, float exposureWeight)
        {
            float3 hdrColor = ldrColor * luminanceIntensity;
        
            #ifdef SHADERGRAPH_PREVIEW
            float inverseExposureMultiplier = 1.0;
            #else
            float inverseExposureMultiplier = GetInverseCurrentExposureMultiplier();
            #endif
        
            // Inverse pre-expose using _EmissiveExposureWeight weight
            hdrColor = lerp(hdrColor * inverseExposureMultiplier, hdrColor, exposureWeight);
            return hdrColor;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
            // Graph Vertex
            // GraphVertex: <None>
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float3 Emission;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_d09e7f2c2a774dbd93cc943fa6a50940_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_VoidColor) : _VoidColor;
            float _Property_16eef79ff58443149895b35d0a88bdb2_Out_0_Float = _InnerEdge;
            float4 _UV_ce55b36e72754a339073e4c05a64b551_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_054563945a0a4f688fce11f0c067b0c8_Out_1_Vector2 = _UV_ce55b36e72754a339073e4c05a64b551_Out_0_Vector4.xy;
            float _Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float;
            Unity_Distance_float2(_Swizzle_054563945a0a4f688fce11f0c067b0c8_Out_1_Vector2, float2(0.5, 0.5), _Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float);
            float _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float;
            Unity_OneMinus_float(_Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float, _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float);
            float _Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float;
            Unity_Smoothstep_float(_Property_16eef79ff58443149895b35d0a88bdb2_Out_0_Float, float(1), _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float, _Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float);
            Gradient _Gradient_c34cf660310448768ec81f4600b23ad8_Out_0_Gradient = NewGradient(0, 4, 2, float4(0, 0, 0, 0.271931),float4(0.01568037, 0.01568037, 0.01568037, 0.4678416),float4(0.1071531, 0.1071531, 0.1071531, 0.6052644),float4(1, 1, 1, 1),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0), float2(1, 0),float2(1, 1),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0));
            float _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float;
            Unity_DotProduct_float3(IN.ObjectSpaceViewDirection, IN.ObjectSpaceNormal, _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float);
            float4 _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4;
            Unity_SampleGradientV1_float(_Gradient_c34cf660310448768ec81f4600b23ad8_Out_0_Gradient, _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float, _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4);
            float4 _Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float.xxxx), _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4, _Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4);
            Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a;
            float _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float;
            SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(IN.ObjectSpaceViewDirection, float(10), _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a, _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float);
            float3 _Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3;
            Unity_Multiply_float3_float3(IN.ObjectSpaceViewDirection, (_Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float.xxx), _Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3);
            float _Property_ba2d1376c19c40fbadc820d0fb721b2e_Out_0_Float = _Speed;
            float _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_ba2d1376c19c40fbadc820d0fb721b2e_Out_0_Float, _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float);
            float2 _Property_2cfd9c61936548ee85383d83df6beba0_Out_0_Vector2 = _Seed;
            float _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float;
            Unity_RandomRange_float(_Property_2cfd9c61936548ee85383d83df6beba0_Out_0_Vector2, float(26.96), float(45.6), _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float);
            Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a;
            float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Value_1_Float;
            float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float;
            SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(_Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3, _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float, _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Value_1_Float, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float);
            float4 _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4, (_Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float.xxxx), _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4);
            float4 _Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_d09e7f2c2a774dbd93cc943fa6a50940_Out_0_Vector4, _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4, _Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4);
            float4 _Property_e0f9565d1eaa452fb638faa34cc517eb_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_BorderColor) : _BorderColor;
            float3 _Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3;
            Unity_Absolute_float3(IN.AbsoluteWorldSpacePosition, _Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3);
            Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1;
            float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Value_1_Float;
            float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float;
            SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(_Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3, IN.TimeParameters.x, float(6.2), _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1, _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Value_1_Float, _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float);
            float _Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float;
            Unity_Smoothstep_float(float(-0.41), float(1), _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float, _Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float);
            float _Property_82bfb359e6494ff08a54e8c08976a6f1_Out_0_Float = _FresnelPower;
            float _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_82bfb359e6494ff08a54e8c08976a6f1_Out_0_Float, _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float);
            float _Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float, _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float, _Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float);
            float4 _Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_e0f9565d1eaa452fb638faa34cc517eb_Out_0_Vector4, (_Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float.xxxx), _Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4);
            float3 _EmissionNode_87074dafd5ff4d39bf4005b1089a56e4_Output_0_Vector3 = Unity_HDRP_GetEmissionHDRColor_float((_Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4.xyz).xyz, float(3), float(0));
            float4 _Add_0b088d988049415c84334d987e692959_Out_2_Vector4;
            Unity_Add_float4(_Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4, (_Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float.xxxx), _Add_0b088d988049415c84334d987e692959_Out_2_Vector4);
            surface.BaseColor = (_Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4.xyz);
            surface.Emission = _EmissionNode_87074dafd5ff4d39bf4005b1089a56e4_Output_0_Vector3;
            surface.Alpha = (_Add_0b088d988049415c84334d987e692959_Out_2_Vector4).x;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES AttributesMesh
            #define VaryingsMeshType VaryingsMeshToPS
            #define VFX_SRP_VARYINGS VaryingsMeshType
            #define VFX_SRP_SURFACE_INPUTS FragInputs
            #endif
            SurfaceDescriptionInputs FragInputsToSurfaceDescriptionInputs(FragInputs input, float3 viewWS)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            output.WorldSpaceNormal =                           normalize(input.tangentToWorld[2].xyz);
            #if defined(SHADER_STAGE_RAY_TRACING)
            output.ObjectSpaceNormal =                          mul(output.WorldSpaceNormal, (float3x3) ObjectToWorld3x4());
            #else
            output.ObjectSpaceNormal =                          normalize(mul(output.WorldSpaceNormal, (float3x3) UNITY_MATRIX_M));           // transposed multiplication by inverse matrix to handle normal scale
            #endif
            output.WorldSpaceViewDirection =                    normalize(viewWS);
            output.ObjectSpaceViewDirection =                   TransformWorldToObjectDir(output.WorldSpaceViewDirection);
        #if SHADERPASS != SHADERPASS_FOG_VOLUME_VOXELIZATION
        #else
        #endif
            output.AbsoluteWorldSpacePosition =                 GetAbsolutePositionWS(input.positionRWS);
        #if SHADERPASS != SHADERPASS_FOG_VOLUME_VOXELIZATION
        #else
        #endif
        
        #if UNITY_UV_STARTS_AT_TOP
        #else
        #endif
        
        
            output.uv0 =                                        input.texCoord0;
            output.TimeParameters =                             _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        
            // splice point to copy frag inputs custom interpolator pack into the SDI
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorCopyToSDI' */
        
            return output;
        }
        
            // --------------------------------------------------
            // Build Surface Data (Specific Material)
        
        void ApplyDecalToSurfaceDataNoNormal(DecalSurfaceData decalSurfaceData, inout SurfaceData surfaceData);
        
        void ApplyDecalAndGetNormal(FragInputs fragInputs, PositionInputs posInput, SurfaceDescription surfaceDescription,
            inout SurfaceData surfaceData)
        {
            float3 doubleSidedConstants = GetDoubleSidedConstants();
        
        #ifdef DECAL_NORMAL_BLENDING
            // SG nodes don't ouptut surface gradients, so if decals require surf grad blending, we have to convert
            // the normal to gradient before applying the decal. We then have to resolve the gradient back to world space
            float3 normalTS;
        
        
        
            #if HAVE_DECALS
            if (_EnableDecals)
            {
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs, alpha);
                ApplyDecalToSurfaceNormal(decalSurfaceData, fragInputs.tangentToWorld[2], normalTS);
                ApplyDecalToSurfaceDataNoNormal(decalSurfaceData, surfaceData);
            }
            #endif
        
            GetNormalWS_SG(fragInputs, normalTS, surfaceData.normalWS, doubleSidedConstants);
        #else
            // normal delivered to master node
        
            #if HAVE_DECALS
            if (_EnableDecals)
            {
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                // Both uses and modifies 'surfaceData.normalWS'.
                DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs, alpha);
                ApplyDecalToSurfaceNormal(decalSurfaceData, surfaceData.normalWS.xyz);
                ApplyDecalToSurfaceDataNoNormal(decalSurfaceData, surfaceData);
            }
            #endif
        #endif
        }
        void BuildSurfaceData(FragInputs fragInputs, inout SurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData)
        {
            // setup defaults -- these are used if the graph doesn't output a value
            ZERO_INITIALIZE(SurfaceData, surfaceData);
        
            // copy across graph values, if defined
            surfaceData.color = surfaceDescription.BaseColor;
        
            #ifdef WRITE_NORMAL_BUFFER
            // When we need to export the normal (in the depth prepass, we write the geometry one)
            surfaceData.normalWS = fragInputs.tangentToWorld[2];
            #endif
        
            #if defined(DEBUG_DISPLAY)
            #if !defined(SHADER_STAGE_RAY_TRACING)
            // Mipmap mode debugging isn't supported with ray tracing as it relies on derivatives
                if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                {
                    #ifdef FRAG_INPUTS_USE_TEXCOORD0
                        surfaceData.color = GET_TEXTURE_STREAMING_DEBUG(posInput.positionSS, fragInputs.texCoord0);
                    #else
                        surfaceData.color = GET_TEXTURE_STREAMING_DEBUG_NO_UV(posInput.positionSS);
                    #endif
                }
            #endif
            #endif
        
            #ifdef _ENABLE_SHADOW_MATTE
        
                #if (SHADERPASS == SHADERPASS_FORWARD_UNLIT) || (SHADERPASS == SHADERPASS_RAYTRACING_GBUFFER) || (SHADERPASS == SHADERPASS_RAYTRACING_INDIRECT) || (SHADERPASS == SHADERPASS_RAYTRACING_FORWARD)
        
                    HDShadowContext shadowContext = InitShadowContext();
        
                    // Evaluate the shadow, the normal is guaranteed if shadow matte is enabled on this shader.
                    float3 shadow3;
                    ShadowLoopMin(shadowContext, posInput, normalize(fragInputs.tangentToWorld[2]), asuint(_ShadowMatteFilter), GetMeshRenderingLayerMask(), shadow3);
        
                    // Compute the average value in the fourth channel
                    float4 shadow = float4(shadow3, dot(shadow3, float3(1.0/3.0, 1.0/3.0, 1.0/3.0)));
        
                    float4 shadowColor = (1.0 - shadow) * surfaceDescription.ShadowTint.rgba;
                    float  localAlpha  = saturate(shadowColor.a + surfaceDescription.Alpha);
        
                    // Keep the nested lerp
                    // With no Color (bsdfData.color.rgb, bsdfData.color.a == 0.0f), just use ShadowColor*Color to avoid a ring of "white" around the shadow
                    // And mix color to consider the Color & ShadowColor alpha (from texture or/and color picker)
                    #ifdef _SURFACE_TYPE_TRANSPARENT
                        surfaceData.color = lerp(shadowColor.rgb * surfaceData.color, lerp(lerp(shadowColor.rgb, surfaceData.color, 1.0 - surfaceDescription.ShadowTint.a), surfaceData.color, shadow.rgb), surfaceDescription.Alpha);
                    #else
                        surfaceData.color = lerp(lerp(shadowColor.rgb, surfaceData.color, 1.0 - surfaceDescription.ShadowTint.a), surfaceData.color, shadow.rgb);
                    #endif
                    localAlpha = ApplyBlendMode(surfaceData.color, localAlpha).a;
        
                    surfaceDescription.Alpha = localAlpha;
        
                #elif SHADERPASS == SHADERPASS_PATH_TRACING
        
                    surfaceData.normalWS = fragInputs.tangentToWorld[2];
                    surfaceData.shadowTint = surfaceDescription.ShadowTint.rgba;
        
                #endif
        
            #endif // _ENABLE_SHADOW_MATTE
        }
        
            // --------------------------------------------------
            // Get Surface And BuiltinData
        
            void GetSurfaceAndBuiltinData(FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData RAY_TRACING_OPTIONAL_PARAMETERS)
            {
                // Don't dither if displaced tessellation (we're fading out the displacement instead to match the next LOD)
                #if !defined(SHADER_STAGE_RAY_TRACING) && !defined(_TESSELLATION_DISPLACEMENT)
                #ifdef LOD_FADE_CROSSFADE // enable dithering LOD transition if user select CrossFade transition in LOD group
                LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                #endif
                #endif
        
                #ifndef SHADER_UNLIT
                #ifdef _DOUBLESIDED_ON
                    float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
                #else
                    float3 doubleSidedConstants = float3(1.0, 1.0, 1.0);
                #endif
        
                ApplyDoubleSidedFlipOrMirror(fragInputs, doubleSidedConstants); // Apply double sided flip on the vertex normal
                #endif // SHADER_UNLIT
        
                SurfaceDescriptionInputs surfaceDescriptionInputs = FragInputsToSurfaceDescriptionInputs(fragInputs, V);
        
                #if defined(HAVE_VFX_MODIFICATION)
                GraphProperties properties;
                ZERO_INITIALIZE(GraphProperties, properties);
        
                GetElementPixelProperties(fragInputs, properties);
        
                SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs, properties);
                #else
                SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);
                #endif
        
                #ifdef DEBUG_DISPLAY
                if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                {
                    surfaceDescription.Alpha = 1.0f;
                }
                #endif
        
                // Perform alpha test very early to save performance (a killed pixel will not sample textures)
                // TODO: split graph evaluation to grab just alpha dependencies first? tricky..
                #ifdef _ALPHATEST_ON
                    float alphaCutoff = surfaceDescription.AlphaClipThreshold;
                    #if SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_PREPASS
                    // The TransparentDepthPrepass is also used with SSR transparent.
                    // If an artists enable transaprent SSR but not the TransparentDepthPrepass itself, then we use AlphaClipThreshold
                    // otherwise if TransparentDepthPrepass is enabled we use AlphaClipThresholdDepthPrepass
                    #elif SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_POSTPASS
                    // DepthPostpass always use its own alpha threshold
                    alphaCutoff = surfaceDescription.AlphaClipThresholdDepthPostpass;
                    #elif (SHADERPASS == SHADERPASS_SHADOWS) || (SHADERPASS == SHADERPASS_RAYTRACING_VISIBILITY)
                    // If use shadow threshold isn't enable we don't allow any test
                    #endif
        
                    GENERIC_ALPHA_TEST(surfaceDescription.Alpha, alphaCutoff);
                #endif
        
                #if !defined(SHADER_STAGE_RAY_TRACING) && _DEPTHOFFSET_ON
                ApplyDepthOffsetPositionInput(V, surfaceDescription.DepthOffset, GetViewForwardDir(), GetWorldToHClipMatrix(), posInput);
                #endif
        
                #ifndef SHADER_UNLIT
                float3 bentNormalWS;
                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData, bentNormalWS);
        
                #ifdef FRAG_INPUTS_USE_TEXCOORD1
                    float4 lightmapTexCoord1 = fragInputs.texCoord1;
                #else
                    float4 lightmapTexCoord1 = float4(0,0,0,0);
                #endif
        
                #ifdef FRAG_INPUTS_USE_TEXCOORD2
                    float4 lightmapTexCoord2 = fragInputs.texCoord2;
                #else
                    float4 lightmapTexCoord2 = float4(0,0,0,0);
                #endif
        
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                // Builtin Data
                // For back lighting we use the oposite vertex normal
                InitBuiltinData(posInput, alpha, bentNormalWS, -fragInputs.tangentToWorld[2], lightmapTexCoord1, lightmapTexCoord2, builtinData);
        
                #else
                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData);
        
                ZERO_BUILTIN_INITIALIZE(builtinData); // No call to InitBuiltinData as we don't have any lighting
                builtinData.opacity = surfaceDescription.Alpha;
        
                #if defined(DEBUG_DISPLAY)
                    // Light Layers are currently not used for the Unlit shader (because it is not lit)
                    // But Unlit objects do cast shadows according to their rendering layer mask, which is what we want to
                    // display in the light layers visualization mode, therefore we need the renderingLayers
                    builtinData.renderingLayers = GetMeshRenderingLayerMask();
                #endif
        
                #endif // SHADER_UNLIT
        
                #ifdef _ALPHATEST_ON
                    // Used for sharpening by alpha to mask - Alpha to covertage is only used with depth only and forward pass (no shadow pass, no transparent pass)
                    builtinData.alphaClipTreshold = alphaCutoff;
                #endif
        
                // override sampleBakedGI - not used by Unlit
        		// When overriding GI, we need to force the isLightmap flag to make sure we don't add APV (sampled in the lightloop) on top of the overridden value (set at GBuffer stage)
        
                builtinData.emissiveColor = surfaceDescription.Emission;
        
                // Note this will not fully work on transparent surfaces (can check with _SURFACE_TYPE_TRANSPARENT define)
                // We will always overwrite vt feeback with the nearest. So behind transparent surfaces vt will not be resolved
                // This is a limitation of the current MRT approach.
                #ifdef UNITY_VIRTUAL_TEXTURING
                #endif
        
                #if _DEPTHOFFSET_ON
                builtinData.depthOffset = surfaceDescription.DepthOffset;
                #endif
        
                // TODO: We should generate distortion / distortionBlur for non distortion pass
                #if (SHADERPASS == SHADERPASS_DISTORTION)
                builtinData.distortion = surfaceDescription.Distortion;
                builtinData.distortionBlur = surfaceDescription.DistortionBlur;
                #endif
        
                #ifndef SHADER_UNLIT
                // PostInitBuiltinData call ApplyDebugToBuiltinData
                PostInitBuiltinData(V, posInput, surfaceData, builtinData);
                #else
                ApplyDebugToBuiltinData(builtinData);
                #endif
        
                RAY_TRACING_OPTIONAL_ALPHA_TEST_PASS
            }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassRaytracingForward.hlsl"
        
            // --------------------------------------------------
            // Visual Effect Vertex Invocations
        
        	#ifdef HAVE_VFX_MODIFICATION
                #if !defined(SHADER_STAGE_RAY_TRACING)
        	    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/VisualEffectVertex.hlsl"
                #else
                #endif
        	#endif
        
            ENDHLSL
        }
        Pass
        {
            Name "GBufferDXR"
            Tags
            {
                "LightMode" = "GBufferDXR"
            }
        
            // Render State
            // RenderState: <None>
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma target 5.0
        #pragma raytracing surface_shader
        #pragma only_renderers d3d11 xboxseries ps5
        
            // Keywords
            #pragma multi_compile _ DEBUG_DISPLAY
            // GraphKeywords: <None>
        
            // Defines
            #define SHADERPASS SHADERPASS_RAYTRACING_GBUFFER
        #define PUNCTUAL_SHADOW_LOW
        #define DIRECTIONAL_SHADOW_LOW
        #define RAYTRACING_SHADER_GRAPH_RAYTRACED
        #define LIGHTLOOP_DISABLE_TILE_AND_CLUSTER 1
        #define PATH_TRACING_CLUSTERED_DECALS 1
        #define SUPPORT_GLOBAL_MIP_BIAS 1
        
            // For custom interpolators to inject a substruct definition before FragInputs definition,
            // allowing for FragInputs to capture CI's intended for ShaderGraph's SDI.
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
        
            // TODO: Merge FragInputsVFX substruct with CustomInterpolators.
        	#ifdef HAVE_VFX_MODIFICATION
        	struct FragInputsVFX
            {
                /* WARNING: $splice Could not find named fragment 'FragInputsVFX' */
            };
            #endif
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl" // Required by Tessellation.hlsl
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl" // Required to be include before we include properties as it define DECLARE_STACK_CB
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl" // Required before including properties as it defines UNITY_TEXTURE_STREAMING_DEBUG_VARS
            // Always include Shader Graph version
            // Always include last to avoid double macros
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl" // Need to be here for Gradient struct definition
        
            // --------------------------------------------------
            // Defines
        
            // Attribute
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TANGENT_TO_WORLD
            #define VARYINGS_NEED_TEXCOORD0
        
        
            //Strip down the FragInputs.hlsl (on graphics), so we can only optimize the interpolators we use.
            //if by accident something requests contents of FragInputs.hlsl, it will be caught as a compiler error
            //Frag inputs stripping is only enabled when FRAG_INPUTS_ENABLE_STRIPPING is set
            #if !defined(SHADER_STAGE_RAY_TRACING) && SHADERPASS != SHADERPASS_RAYTRACING_GBUFFER && SHADERPASS != SHADERPASS_FULL_SCREEN_DEBUG
            #define FRAG_INPUTS_ENABLE_STRIPPING
            #endif
            #define FRAG_INPUTS_USE_TEXCOORD0
        
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
        
        
        
        
            // Following two define are a workaround introduce in 10.1.x for RaytracingQualityNode
            // The ShaderGraph don't support correctly migration of this node as it serialize all the node data
            // in the json file making it impossible to uprgrade. Until we get a fix, we do a workaround here
            // to still allow us to rename the field and keyword of this node without breaking existing code.
            #ifdef RAYTRACING_SHADER_GRAPH_DEFAULT
            #define RAYTRACING_SHADER_GRAPH_HIGH
            #endif
        
            #ifdef RAYTRACING_SHADER_GRAPH_RAYTRACED
            #define RAYTRACING_SHADER_GRAPH_LOW
            #endif
            // end
        
            #ifndef SHADER_UNLIT
            // We need isFrontFace when using double sided - it is not required for unlit as in case of unlit double sided only drive the cullmode
            // VARYINGS_NEED_CULLFACE can be define by VaryingsMeshToPS.FaceSign input if a IsFrontFace Node is included in the shader graph.
            #if defined(_DOUBLESIDED_ON) && !defined(VARYINGS_NEED_CULLFACE)
                #define VARYINGS_NEED_CULLFACE
            #endif
            #endif
        
            // Specific Material Define
        // Setup a define to say we are an unlit shader
        #define SHADER_UNLIT
        
        // Following Macro are only used by Unlit material
        #if defined(_ENABLE_SHADOW_MATTE)
            #if SHADERPASS == SHADERPASS_FORWARD_UNLIT
                #pragma multi_compile_fragment USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST
            #elif SHADERPASS == SHADERPASS_PATH_TRACING
                #define LIGHTLOOP_DISABLE_TILE_AND_CLUSTER
            #endif
        
        // We don't want to have the lightloop defined for the ray tracing passes, but we do for the rasterisation and path tracing shader passes.
        #if !defined(SHADER_STAGE_RAY_TRACING) || SHADERPASS == SHADERPASS_PATH_TRACING
            #define HAS_LIGHTLOOP
        #endif
        #endif
            // Caution: we can use the define SHADER_UNLIT onlit after the above Material include as it is the Unlit template who define it
        
            // To handle SSR on transparent correctly with a possibility to enable/disable it per framesettings
            // we should have a code like this:
            // if !defined(_DISABLE_SSR_TRANSPARENT)
            // pragma multi_compile _ WRITE_NORMAL_BUFFER
            // endif
            // i.e we enable the multicompile only if we can receive SSR or not, and then C# code drive
            // it based on if SSR transparent in frame settings and not (and stripper can strip it).
            // this is currently not possible with our current preprocessor as _DISABLE_SSR_TRANSPARENT is a keyword not a define
            // so instead we used this and chose to pay the extra cost of normal write even if SSR transaprent is disabled.
            // Ideally the shader graph generator should handle it but condition below can't be handle correctly for now.
            #if SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_PREPASS
            #if !defined(_DISABLE_SSR_TRANSPARENT) && !defined(SHADER_UNLIT)
                #define WRITE_NORMAL_BUFFER
            #endif
            #endif
        
            // See Lit.shader
            #if SHADERPASS == SHADERPASS_MOTION_VECTORS && defined(WRITE_DECAL_BUFFER_AND_RENDERING_LAYER)
                #define WRITE_DECAL_BUFFER
            #endif
        
            #ifndef DEBUG_DISPLAY
                // In case of opaque we don't want to perform the alpha test, it is done in depth prepass and we use depth equal for ztest (setup from UI)
                // Don't do it with debug display mode as it is possible there is no depth prepass in this case
                #if !defined(_SURFACE_TYPE_TRANSPARENT)
                    #if SHADERPASS == SHADERPASS_FORWARD
                    #define SHADERPASS_FORWARD_BYPASS_ALPHA_TEST
                    #elif SHADERPASS == SHADERPASS_GBUFFER
                    #define SHADERPASS_GBUFFER_BYPASS_ALPHA_TEST
                    #endif
                #endif
            #endif
        
            // Define _DEFERRED_CAPABLE_MATERIAL for shader capable to run in deferred pass
            #if defined(SHADER_LIT) && !defined(_SURFACE_TYPE_TRANSPARENT)
                #define _DEFERRED_CAPABLE_MATERIAL
            #endif
        
            // Translate transparent motion vector define
            #if (defined(_TRANSPARENT_WRITES_MOTION_VEC) || defined(_TRANSPARENT_REFRACTIVE_SORT)) && defined(_SURFACE_TYPE_TRANSPARENT)
                #define _WRITE_TRANSPARENT_MOTION_VECTOR
            #endif
        
            // -- Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _VoidColor;
        float _Speed;
        float _FresnelPower;
        float4 _BorderColor;
        float2 _Seed;
        float _TwirlStrength;
        float4 _TwirlTexture_TexelSize;
        float _InnerEdge;
        float4 _EmissionColor;
        float _UseShadowThreshold;
        float4 _DoubleSidedConstants;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        float _BlendMode;
        float _EnableBlendModePreserveSpecularLighting;
        CBUFFER_END
        
        
        // Object and Global properties
        TEXTURE2D(_TwirlTexture);
        SAMPLER(sampler_TwirlTexture);
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Includes
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/RaytracingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/ShaderVariablesRaytracing.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/ShaderVariablesRaytracingLightLoop.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/Deferred/RaytracingIntersectonGBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/Unlit.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/NormalBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/StandardLit/StandardLit.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/UnlitRaytracing.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/RayTracingCommon.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/ClassicNoise3D.hlsl"
        #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi3D.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonLighting.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            struct SurfaceDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 WorldSpaceNormal;
             float3 ObjectSpaceViewDirection;
             float3 WorldSpaceViewDirection;
             float3 AbsoluteWorldSpacePosition;
             float4 uv0;
             float3 TimeParameters;
        };
        
            //Interpolator Packs: <None>
        
            // --------------------------------------------------
            // Graph
        
        
            // Graph Functions
            
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_DotProduct_float3(float3 A, float3 B, out float Out)
        {
            Out = dot(A, B);
        }
        
        void Unity_SampleGradientV1_float(Gradient Gradient, float Time, out float4 Out)
        {
            // convert to OkLab if we need perceptual color space.
            float3 color = lerp(Gradient.colors[0].rgb, LinearToOklab(Gradient.colors[0].rgb), Gradient.type == 2);
        
            [unroll]
            for (int c = 1; c < Gradient.colorsLength; c++)
            {
                float colorPos = saturate((Time - Gradient.colors[c - 1].w) / (Gradient.colors[c].w - Gradient.colors[c - 1].w)) * step(c, Gradient.colorsLength - 1);
                float3 color2 = lerp(Gradient.colors[c].rgb, LinearToOklab(Gradient.colors[c].rgb), Gradient.type == 2);
                color = lerp(color, color2, lerp(colorPos, step(0.01, colorPos), Gradient.type % 2)); // grad.type == 1 is fixed, 0 and 2 are blends.
            }
            color = lerp(color, OklabToLinear(color), Gradient.type == 2);
        
        #ifdef UNITY_COLORSPACE_GAMMA
            color = LinearToSRGB(color);
        #endif
        
            float alpha = Gradient.alphas[0].x;
            [unroll]
            for (int a = 1; a < Gradient.alphasLength; a++)
            {
                float alphaPos = saturate((Time - Gradient.alphas[a - 1].y) / (Gradient.alphas[a].y - Gradient.alphas[a - 1].y)) * step(a, Gradient.alphasLength - 1);
                alpha = lerp(alpha, Gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), Gradient.type % 2));
            }
        
            Out = float4(color, alpha);
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
        Out = A * B;
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        struct Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float
        {
        };
        
        void SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(float3 Vector3_7940555B, float Vector1_1B8B9078, Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float IN, out float Value_0)
        {
        float3 _Property_44999cc87708de82a26b39ae1da975ec_Out_0_Vector3 = Vector3_7940555B;
        float _Property_dad5add45a7fa785be976f925bc5a5da_Out_0_Float = Vector1_1B8B9078;
        float3 _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3;
        Unity_Multiply_float3_float3(_Property_44999cc87708de82a26b39ae1da975ec_Out_0_Vector3, (_Property_dad5add45a7fa785be976f925bc5a5da_Out_0_Float.xxx), _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3);
        float _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float;
        PerlinNoise3D_float(_Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3, _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float);
        float _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float;
        Unity_Remap_float(_PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float, float2 (-1.15, 1.15), float2 (0, 1), _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float);
        Value_0 = _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_RandomRange_float(float2 Seed, float Min, float Max, out float Out)
        {
             float randomno =  frac(sin(dot(Seed, float2(12.9898, 78.233)))*43758.5453);
             Out = lerp(Min, Max, randomno);
        }
        
        struct Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float
        {
        };
        
        void SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(float3 Vector3_375394F7, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float IN, out float Value_1, out float Cells_2)
        {
        float3 _Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3 = Vector3_375394F7;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        VoronoiPrecise3D_float(_Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }
        
        float3 Unity_HDRP_GetEmissionHDRColor_float(float3 ldrColor, float luminanceIntensity, float exposureWeight)
        {
            float3 hdrColor = ldrColor * luminanceIntensity;
        
            #ifdef SHADERGRAPH_PREVIEW
            float inverseExposureMultiplier = 1.0;
            #else
            float inverseExposureMultiplier = GetInverseCurrentExposureMultiplier();
            #endif
        
            // Inverse pre-expose using _EmissiveExposureWeight weight
            hdrColor = lerp(hdrColor * inverseExposureMultiplier, hdrColor, exposureWeight);
            return hdrColor;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
            // Graph Vertex
            // GraphVertex: <None>
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float3 Emission;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_d09e7f2c2a774dbd93cc943fa6a50940_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_VoidColor) : _VoidColor;
            float _Property_16eef79ff58443149895b35d0a88bdb2_Out_0_Float = _InnerEdge;
            float4 _UV_ce55b36e72754a339073e4c05a64b551_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_054563945a0a4f688fce11f0c067b0c8_Out_1_Vector2 = _UV_ce55b36e72754a339073e4c05a64b551_Out_0_Vector4.xy;
            float _Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float;
            Unity_Distance_float2(_Swizzle_054563945a0a4f688fce11f0c067b0c8_Out_1_Vector2, float2(0.5, 0.5), _Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float);
            float _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float;
            Unity_OneMinus_float(_Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float, _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float);
            float _Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float;
            Unity_Smoothstep_float(_Property_16eef79ff58443149895b35d0a88bdb2_Out_0_Float, float(1), _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float, _Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float);
            Gradient _Gradient_c34cf660310448768ec81f4600b23ad8_Out_0_Gradient = NewGradient(0, 4, 2, float4(0, 0, 0, 0.271931),float4(0.01568037, 0.01568037, 0.01568037, 0.4678416),float4(0.1071531, 0.1071531, 0.1071531, 0.6052644),float4(1, 1, 1, 1),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0), float2(1, 0),float2(1, 1),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0));
            float _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float;
            Unity_DotProduct_float3(IN.ObjectSpaceViewDirection, IN.ObjectSpaceNormal, _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float);
            float4 _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4;
            Unity_SampleGradientV1_float(_Gradient_c34cf660310448768ec81f4600b23ad8_Out_0_Gradient, _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float, _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4);
            float4 _Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float.xxxx), _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4, _Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4);
            Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a;
            float _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float;
            SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(IN.ObjectSpaceViewDirection, float(10), _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a, _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float);
            float3 _Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3;
            Unity_Multiply_float3_float3(IN.ObjectSpaceViewDirection, (_Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float.xxx), _Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3);
            float _Property_ba2d1376c19c40fbadc820d0fb721b2e_Out_0_Float = _Speed;
            float _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_ba2d1376c19c40fbadc820d0fb721b2e_Out_0_Float, _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float);
            float2 _Property_2cfd9c61936548ee85383d83df6beba0_Out_0_Vector2 = _Seed;
            float _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float;
            Unity_RandomRange_float(_Property_2cfd9c61936548ee85383d83df6beba0_Out_0_Vector2, float(26.96), float(45.6), _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float);
            Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a;
            float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Value_1_Float;
            float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float;
            SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(_Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3, _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float, _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Value_1_Float, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float);
            float4 _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4, (_Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float.xxxx), _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4);
            float4 _Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_d09e7f2c2a774dbd93cc943fa6a50940_Out_0_Vector4, _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4, _Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4);
            float4 _Property_e0f9565d1eaa452fb638faa34cc517eb_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_BorderColor) : _BorderColor;
            float3 _Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3;
            Unity_Absolute_float3(IN.AbsoluteWorldSpacePosition, _Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3);
            Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1;
            float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Value_1_Float;
            float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float;
            SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(_Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3, IN.TimeParameters.x, float(6.2), _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1, _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Value_1_Float, _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float);
            float _Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float;
            Unity_Smoothstep_float(float(-0.41), float(1), _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float, _Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float);
            float _Property_82bfb359e6494ff08a54e8c08976a6f1_Out_0_Float = _FresnelPower;
            float _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_82bfb359e6494ff08a54e8c08976a6f1_Out_0_Float, _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float);
            float _Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float, _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float, _Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float);
            float4 _Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_e0f9565d1eaa452fb638faa34cc517eb_Out_0_Vector4, (_Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float.xxxx), _Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4);
            float3 _EmissionNode_87074dafd5ff4d39bf4005b1089a56e4_Output_0_Vector3 = Unity_HDRP_GetEmissionHDRColor_float((_Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4.xyz).xyz, float(3), float(0));
            float4 _Add_0b088d988049415c84334d987e692959_Out_2_Vector4;
            Unity_Add_float4(_Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4, (_Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float.xxxx), _Add_0b088d988049415c84334d987e692959_Out_2_Vector4);
            surface.BaseColor = (_Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4.xyz);
            surface.Emission = _EmissionNode_87074dafd5ff4d39bf4005b1089a56e4_Output_0_Vector3;
            surface.Alpha = (_Add_0b088d988049415c84334d987e692959_Out_2_Vector4).x;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES AttributesMesh
            #define VaryingsMeshType VaryingsMeshToPS
            #define VFX_SRP_VARYINGS VaryingsMeshType
            #define VFX_SRP_SURFACE_INPUTS FragInputs
            #endif
            SurfaceDescriptionInputs FragInputsToSurfaceDescriptionInputs(FragInputs input, float3 viewWS)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            output.WorldSpaceNormal =                           normalize(input.tangentToWorld[2].xyz);
            #if defined(SHADER_STAGE_RAY_TRACING)
            output.ObjectSpaceNormal =                          mul(output.WorldSpaceNormal, (float3x3) ObjectToWorld3x4());
            #else
            output.ObjectSpaceNormal =                          normalize(mul(output.WorldSpaceNormal, (float3x3) UNITY_MATRIX_M));           // transposed multiplication by inverse matrix to handle normal scale
            #endif
            output.WorldSpaceViewDirection =                    normalize(viewWS);
            output.ObjectSpaceViewDirection =                   TransformWorldToObjectDir(output.WorldSpaceViewDirection);
        #if SHADERPASS != SHADERPASS_FOG_VOLUME_VOXELIZATION
        #else
        #endif
            output.AbsoluteWorldSpacePosition =                 GetAbsolutePositionWS(input.positionRWS);
        #if SHADERPASS != SHADERPASS_FOG_VOLUME_VOXELIZATION
        #else
        #endif
        
        #if UNITY_UV_STARTS_AT_TOP
        #else
        #endif
        
        
            output.uv0 =                                        input.texCoord0;
            output.TimeParameters =                             _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        
            // splice point to copy frag inputs custom interpolator pack into the SDI
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorCopyToSDI' */
        
            return output;
        }
        
            // --------------------------------------------------
            // Build Surface Data (Specific Material)
        
        void ApplyDecalToSurfaceDataNoNormal(DecalSurfaceData decalSurfaceData, inout SurfaceData surfaceData);
        
        void ApplyDecalAndGetNormal(FragInputs fragInputs, PositionInputs posInput, SurfaceDescription surfaceDescription,
            inout SurfaceData surfaceData)
        {
            float3 doubleSidedConstants = GetDoubleSidedConstants();
        
        #ifdef DECAL_NORMAL_BLENDING
            // SG nodes don't ouptut surface gradients, so if decals require surf grad blending, we have to convert
            // the normal to gradient before applying the decal. We then have to resolve the gradient back to world space
            float3 normalTS;
        
        
        
            #if HAVE_DECALS
            if (_EnableDecals)
            {
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs, alpha);
                ApplyDecalToSurfaceNormal(decalSurfaceData, fragInputs.tangentToWorld[2], normalTS);
                ApplyDecalToSurfaceDataNoNormal(decalSurfaceData, surfaceData);
            }
            #endif
        
            GetNormalWS_SG(fragInputs, normalTS, surfaceData.normalWS, doubleSidedConstants);
        #else
            // normal delivered to master node
        
            #if HAVE_DECALS
            if (_EnableDecals)
            {
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                // Both uses and modifies 'surfaceData.normalWS'.
                DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs, alpha);
                ApplyDecalToSurfaceNormal(decalSurfaceData, surfaceData.normalWS.xyz);
                ApplyDecalToSurfaceDataNoNormal(decalSurfaceData, surfaceData);
            }
            #endif
        #endif
        }
        void BuildSurfaceData(FragInputs fragInputs, inout SurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData)
        {
            // setup defaults -- these are used if the graph doesn't output a value
            ZERO_INITIALIZE(SurfaceData, surfaceData);
        
            // copy across graph values, if defined
            surfaceData.color = surfaceDescription.BaseColor;
        
            #ifdef WRITE_NORMAL_BUFFER
            // When we need to export the normal (in the depth prepass, we write the geometry one)
            surfaceData.normalWS = fragInputs.tangentToWorld[2];
            #endif
        
            #if defined(DEBUG_DISPLAY)
            #if !defined(SHADER_STAGE_RAY_TRACING)
            // Mipmap mode debugging isn't supported with ray tracing as it relies on derivatives
                if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                {
                    #ifdef FRAG_INPUTS_USE_TEXCOORD0
                        surfaceData.color = GET_TEXTURE_STREAMING_DEBUG(posInput.positionSS, fragInputs.texCoord0);
                    #else
                        surfaceData.color = GET_TEXTURE_STREAMING_DEBUG_NO_UV(posInput.positionSS);
                    #endif
                }
            #endif
            #endif
        
            #ifdef _ENABLE_SHADOW_MATTE
        
                #if (SHADERPASS == SHADERPASS_FORWARD_UNLIT) || (SHADERPASS == SHADERPASS_RAYTRACING_GBUFFER) || (SHADERPASS == SHADERPASS_RAYTRACING_INDIRECT) || (SHADERPASS == SHADERPASS_RAYTRACING_FORWARD)
        
                    HDShadowContext shadowContext = InitShadowContext();
        
                    // Evaluate the shadow, the normal is guaranteed if shadow matte is enabled on this shader.
                    float3 shadow3;
                    ShadowLoopMin(shadowContext, posInput, normalize(fragInputs.tangentToWorld[2]), asuint(_ShadowMatteFilter), GetMeshRenderingLayerMask(), shadow3);
        
                    // Compute the average value in the fourth channel
                    float4 shadow = float4(shadow3, dot(shadow3, float3(1.0/3.0, 1.0/3.0, 1.0/3.0)));
        
                    float4 shadowColor = (1.0 - shadow) * surfaceDescription.ShadowTint.rgba;
                    float  localAlpha  = saturate(shadowColor.a + surfaceDescription.Alpha);
        
                    // Keep the nested lerp
                    // With no Color (bsdfData.color.rgb, bsdfData.color.a == 0.0f), just use ShadowColor*Color to avoid a ring of "white" around the shadow
                    // And mix color to consider the Color & ShadowColor alpha (from texture or/and color picker)
                    #ifdef _SURFACE_TYPE_TRANSPARENT
                        surfaceData.color = lerp(shadowColor.rgb * surfaceData.color, lerp(lerp(shadowColor.rgb, surfaceData.color, 1.0 - surfaceDescription.ShadowTint.a), surfaceData.color, shadow.rgb), surfaceDescription.Alpha);
                    #else
                        surfaceData.color = lerp(lerp(shadowColor.rgb, surfaceData.color, 1.0 - surfaceDescription.ShadowTint.a), surfaceData.color, shadow.rgb);
                    #endif
                    localAlpha = ApplyBlendMode(surfaceData.color, localAlpha).a;
        
                    surfaceDescription.Alpha = localAlpha;
        
                #elif SHADERPASS == SHADERPASS_PATH_TRACING
        
                    surfaceData.normalWS = fragInputs.tangentToWorld[2];
                    surfaceData.shadowTint = surfaceDescription.ShadowTint.rgba;
        
                #endif
        
            #endif // _ENABLE_SHADOW_MATTE
        }
        
            // --------------------------------------------------
            // Get Surface And BuiltinData
        
            void GetSurfaceAndBuiltinData(FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData RAY_TRACING_OPTIONAL_PARAMETERS)
            {
                // Don't dither if displaced tessellation (we're fading out the displacement instead to match the next LOD)
                #if !defined(SHADER_STAGE_RAY_TRACING) && !defined(_TESSELLATION_DISPLACEMENT)
                #ifdef LOD_FADE_CROSSFADE // enable dithering LOD transition if user select CrossFade transition in LOD group
                LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                #endif
                #endif
        
                #ifndef SHADER_UNLIT
                #ifdef _DOUBLESIDED_ON
                    float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
                #else
                    float3 doubleSidedConstants = float3(1.0, 1.0, 1.0);
                #endif
        
                ApplyDoubleSidedFlipOrMirror(fragInputs, doubleSidedConstants); // Apply double sided flip on the vertex normal
                #endif // SHADER_UNLIT
        
                SurfaceDescriptionInputs surfaceDescriptionInputs = FragInputsToSurfaceDescriptionInputs(fragInputs, V);
        
                #if defined(HAVE_VFX_MODIFICATION)
                GraphProperties properties;
                ZERO_INITIALIZE(GraphProperties, properties);
        
                GetElementPixelProperties(fragInputs, properties);
        
                SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs, properties);
                #else
                SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);
                #endif
        
                #ifdef DEBUG_DISPLAY
                if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                {
                    surfaceDescription.Alpha = 1.0f;
                }
                #endif
        
                // Perform alpha test very early to save performance (a killed pixel will not sample textures)
                // TODO: split graph evaluation to grab just alpha dependencies first? tricky..
                #ifdef _ALPHATEST_ON
                    float alphaCutoff = surfaceDescription.AlphaClipThreshold;
                    #if SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_PREPASS
                    // The TransparentDepthPrepass is also used with SSR transparent.
                    // If an artists enable transaprent SSR but not the TransparentDepthPrepass itself, then we use AlphaClipThreshold
                    // otherwise if TransparentDepthPrepass is enabled we use AlphaClipThresholdDepthPrepass
                    #elif SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_POSTPASS
                    // DepthPostpass always use its own alpha threshold
                    alphaCutoff = surfaceDescription.AlphaClipThresholdDepthPostpass;
                    #elif (SHADERPASS == SHADERPASS_SHADOWS) || (SHADERPASS == SHADERPASS_RAYTRACING_VISIBILITY)
                    // If use shadow threshold isn't enable we don't allow any test
                    #endif
        
                    GENERIC_ALPHA_TEST(surfaceDescription.Alpha, alphaCutoff);
                #endif
        
                #if !defined(SHADER_STAGE_RAY_TRACING) && _DEPTHOFFSET_ON
                ApplyDepthOffsetPositionInput(V, surfaceDescription.DepthOffset, GetViewForwardDir(), GetWorldToHClipMatrix(), posInput);
                #endif
        
                #ifndef SHADER_UNLIT
                float3 bentNormalWS;
                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData, bentNormalWS);
        
                #ifdef FRAG_INPUTS_USE_TEXCOORD1
                    float4 lightmapTexCoord1 = fragInputs.texCoord1;
                #else
                    float4 lightmapTexCoord1 = float4(0,0,0,0);
                #endif
        
                #ifdef FRAG_INPUTS_USE_TEXCOORD2
                    float4 lightmapTexCoord2 = fragInputs.texCoord2;
                #else
                    float4 lightmapTexCoord2 = float4(0,0,0,0);
                #endif
        
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                // Builtin Data
                // For back lighting we use the oposite vertex normal
                InitBuiltinData(posInput, alpha, bentNormalWS, -fragInputs.tangentToWorld[2], lightmapTexCoord1, lightmapTexCoord2, builtinData);
        
                #else
                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData);
        
                ZERO_BUILTIN_INITIALIZE(builtinData); // No call to InitBuiltinData as we don't have any lighting
                builtinData.opacity = surfaceDescription.Alpha;
        
                #if defined(DEBUG_DISPLAY)
                    // Light Layers are currently not used for the Unlit shader (because it is not lit)
                    // But Unlit objects do cast shadows according to their rendering layer mask, which is what we want to
                    // display in the light layers visualization mode, therefore we need the renderingLayers
                    builtinData.renderingLayers = GetMeshRenderingLayerMask();
                #endif
        
                #endif // SHADER_UNLIT
        
                #ifdef _ALPHATEST_ON
                    // Used for sharpening by alpha to mask - Alpha to covertage is only used with depth only and forward pass (no shadow pass, no transparent pass)
                    builtinData.alphaClipTreshold = alphaCutoff;
                #endif
        
                // override sampleBakedGI - not used by Unlit
        		// When overriding GI, we need to force the isLightmap flag to make sure we don't add APV (sampled in the lightloop) on top of the overridden value (set at GBuffer stage)
        
                builtinData.emissiveColor = surfaceDescription.Emission;
        
                // Note this will not fully work on transparent surfaces (can check with _SURFACE_TYPE_TRANSPARENT define)
                // We will always overwrite vt feeback with the nearest. So behind transparent surfaces vt will not be resolved
                // This is a limitation of the current MRT approach.
                #ifdef UNITY_VIRTUAL_TEXTURING
                #endif
        
                #if _DEPTHOFFSET_ON
                builtinData.depthOffset = surfaceDescription.DepthOffset;
                #endif
        
                // TODO: We should generate distortion / distortionBlur for non distortion pass
                #if (SHADERPASS == SHADERPASS_DISTORTION)
                builtinData.distortion = surfaceDescription.Distortion;
                builtinData.distortionBlur = surfaceDescription.DistortionBlur;
                #endif
        
                #ifndef SHADER_UNLIT
                // PostInitBuiltinData call ApplyDebugToBuiltinData
                PostInitBuiltinData(V, posInput, surfaceData, builtinData);
                #else
                ApplyDebugToBuiltinData(builtinData);
                #endif
        
                RAY_TRACING_OPTIONAL_ALPHA_TEST_PASS
            }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassRaytracingGBuffer.hlsl"
        
            // --------------------------------------------------
            // Visual Effect Vertex Invocations
        
        	#ifdef HAVE_VFX_MODIFICATION
                #if !defined(SHADER_STAGE_RAY_TRACING)
        	    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/VisualEffectVertex.hlsl"
                #else
                #endif
        	#endif
        
            ENDHLSL
        }
        Pass
        {
            Name "DebugDXR"
            Tags
            {
                "LightMode" = "DebugDXR"
            }
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma target 5.0
        #pragma raytracing surface_shader
        #pragma only_renderers d3d11 xboxseries ps5
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
        
            // Includes
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/RaytracingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/ShaderVariablesRaytracing.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/RaytracingIntersection.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/RayTracingCommon.hlsl"
        	#include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/ClassicNoise3D.hlsl"
        #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi3D.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonLighting.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassRayTracingDebug.hlsl"
        
            ENDHLSL
        }
        Pass
        {
            Name "PathTracingDXR"
            Tags
            {
                "LightMode" = "PathTracingDXR"
            }
        
            // Render State
            // RenderState: <None>
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma target 5.0
        #pragma raytracing surface_shader
        #pragma only_renderers d3d11 xboxseries ps5
        
            // Keywords
            #pragma shader_feature _ _SURFACE_TYPE_TRANSPARENT
        #pragma multi_compile _ DEBUG_DISPLAY
            // GraphKeywords: <None>
        
            // Defines
            #define SHADERPASS SHADERPASS_PATH_TRACING
        #define SUPPORT_GLOBAL_MIP_BIAS 1
        
            // For custom interpolators to inject a substruct definition before FragInputs definition,
            // allowing for FragInputs to capture CI's intended for ShaderGraph's SDI.
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
        
            // TODO: Merge FragInputsVFX substruct with CustomInterpolators.
        	#ifdef HAVE_VFX_MODIFICATION
        	struct FragInputsVFX
            {
                /* WARNING: $splice Could not find named fragment 'FragInputsVFX' */
            };
            #endif
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl" // Required by Tessellation.hlsl
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl" // Required to be include before we include properties as it define DECLARE_STACK_CB
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl" // Required before including properties as it defines UNITY_TEXTURE_STREAMING_DEBUG_VARS
            // Always include Shader Graph version
            // Always include last to avoid double macros
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl" // Need to be here for Gradient struct definition
        
            // --------------------------------------------------
            // Defines
        
            // Attribute
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TANGENT_TO_WORLD
            #define VARYINGS_NEED_TEXCOORD0
        
        
            //Strip down the FragInputs.hlsl (on graphics), so we can only optimize the interpolators we use.
            //if by accident something requests contents of FragInputs.hlsl, it will be caught as a compiler error
            //Frag inputs stripping is only enabled when FRAG_INPUTS_ENABLE_STRIPPING is set
            #if !defined(SHADER_STAGE_RAY_TRACING) && SHADERPASS != SHADERPASS_RAYTRACING_GBUFFER && SHADERPASS != SHADERPASS_FULL_SCREEN_DEBUG
            #define FRAG_INPUTS_ENABLE_STRIPPING
            #endif
            #define FRAG_INPUTS_USE_TEXCOORD0
        
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
        
        
        
        
            // Following two define are a workaround introduce in 10.1.x for RaytracingQualityNode
            // The ShaderGraph don't support correctly migration of this node as it serialize all the node data
            // in the json file making it impossible to uprgrade. Until we get a fix, we do a workaround here
            // to still allow us to rename the field and keyword of this node without breaking existing code.
            #ifdef RAYTRACING_SHADER_GRAPH_DEFAULT
            #define RAYTRACING_SHADER_GRAPH_HIGH
            #endif
        
            #ifdef RAYTRACING_SHADER_GRAPH_RAYTRACED
            #define RAYTRACING_SHADER_GRAPH_LOW
            #endif
            // end
        
            #ifndef SHADER_UNLIT
            // We need isFrontFace when using double sided - it is not required for unlit as in case of unlit double sided only drive the cullmode
            // VARYINGS_NEED_CULLFACE can be define by VaryingsMeshToPS.FaceSign input if a IsFrontFace Node is included in the shader graph.
            #if defined(_DOUBLESIDED_ON) && !defined(VARYINGS_NEED_CULLFACE)
                #define VARYINGS_NEED_CULLFACE
            #endif
            #endif
        
            // Specific Material Define
        // Setup a define to say we are an unlit shader
        #define SHADER_UNLIT
        
        // Following Macro are only used by Unlit material
        #if defined(_ENABLE_SHADOW_MATTE)
            #if SHADERPASS == SHADERPASS_FORWARD_UNLIT
                #pragma multi_compile_fragment USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST
            #elif SHADERPASS == SHADERPASS_PATH_TRACING
                #define LIGHTLOOP_DISABLE_TILE_AND_CLUSTER
            #endif
        
        // We don't want to have the lightloop defined for the ray tracing passes, but we do for the rasterisation and path tracing shader passes.
        #if !defined(SHADER_STAGE_RAY_TRACING) || SHADERPASS == SHADERPASS_PATH_TRACING
            #define HAS_LIGHTLOOP
        #endif
        #endif
            // Caution: we can use the define SHADER_UNLIT onlit after the above Material include as it is the Unlit template who define it
        
            // To handle SSR on transparent correctly with a possibility to enable/disable it per framesettings
            // we should have a code like this:
            // if !defined(_DISABLE_SSR_TRANSPARENT)
            // pragma multi_compile _ WRITE_NORMAL_BUFFER
            // endif
            // i.e we enable the multicompile only if we can receive SSR or not, and then C# code drive
            // it based on if SSR transparent in frame settings and not (and stripper can strip it).
            // this is currently not possible with our current preprocessor as _DISABLE_SSR_TRANSPARENT is a keyword not a define
            // so instead we used this and chose to pay the extra cost of normal write even if SSR transaprent is disabled.
            // Ideally the shader graph generator should handle it but condition below can't be handle correctly for now.
            #if SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_PREPASS
            #if !defined(_DISABLE_SSR_TRANSPARENT) && !defined(SHADER_UNLIT)
                #define WRITE_NORMAL_BUFFER
            #endif
            #endif
        
            // See Lit.shader
            #if SHADERPASS == SHADERPASS_MOTION_VECTORS && defined(WRITE_DECAL_BUFFER_AND_RENDERING_LAYER)
                #define WRITE_DECAL_BUFFER
            #endif
        
            #ifndef DEBUG_DISPLAY
                // In case of opaque we don't want to perform the alpha test, it is done in depth prepass and we use depth equal for ztest (setup from UI)
                // Don't do it with debug display mode as it is possible there is no depth prepass in this case
                #if !defined(_SURFACE_TYPE_TRANSPARENT)
                    #if SHADERPASS == SHADERPASS_FORWARD
                    #define SHADERPASS_FORWARD_BYPASS_ALPHA_TEST
                    #elif SHADERPASS == SHADERPASS_GBUFFER
                    #define SHADERPASS_GBUFFER_BYPASS_ALPHA_TEST
                    #endif
                #endif
            #endif
        
            // Define _DEFERRED_CAPABLE_MATERIAL for shader capable to run in deferred pass
            #if defined(SHADER_LIT) && !defined(_SURFACE_TYPE_TRANSPARENT)
                #define _DEFERRED_CAPABLE_MATERIAL
            #endif
        
            // Translate transparent motion vector define
            #if (defined(_TRANSPARENT_WRITES_MOTION_VEC) || defined(_TRANSPARENT_REFRACTIVE_SORT)) && defined(_SURFACE_TYPE_TRANSPARENT)
                #define _WRITE_TRANSPARENT_MOTION_VECTOR
            #endif
        
            // -- Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _VoidColor;
        float _Speed;
        float _FresnelPower;
        float4 _BorderColor;
        float2 _Seed;
        float _TwirlStrength;
        float4 _TwirlTexture_TexelSize;
        float _InnerEdge;
        float4 _EmissionColor;
        float _UseShadowThreshold;
        float4 _DoubleSidedConstants;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        float _BlendMode;
        float _EnableBlendModePreserveSpecularLighting;
        CBUFFER_END
        
        
        // Object and Global properties
        TEXTURE2D(_TwirlTexture);
        SAMPLER(sampler_TwirlTexture);
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Includes
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/RaytracingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/ShaderVariablesRaytracing.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/ShaderVariablesRaytracingLightLoop.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/RaytracingIntersection.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/Unlit.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/RayTracingCommon.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/ClassicNoise3D.hlsl"
        #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi3D.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonLighting.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            struct SurfaceDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 WorldSpaceNormal;
             float3 ObjectSpaceViewDirection;
             float3 WorldSpaceViewDirection;
             float3 AbsoluteWorldSpacePosition;
             float4 uv0;
             float3 TimeParameters;
        };
        
            //Interpolator Packs: <None>
        
            // --------------------------------------------------
            // Graph
        
        
            // Graph Functions
            
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_DotProduct_float3(float3 A, float3 B, out float Out)
        {
            Out = dot(A, B);
        }
        
        void Unity_SampleGradientV1_float(Gradient Gradient, float Time, out float4 Out)
        {
            // convert to OkLab if we need perceptual color space.
            float3 color = lerp(Gradient.colors[0].rgb, LinearToOklab(Gradient.colors[0].rgb), Gradient.type == 2);
        
            [unroll]
            for (int c = 1; c < Gradient.colorsLength; c++)
            {
                float colorPos = saturate((Time - Gradient.colors[c - 1].w) / (Gradient.colors[c].w - Gradient.colors[c - 1].w)) * step(c, Gradient.colorsLength - 1);
                float3 color2 = lerp(Gradient.colors[c].rgb, LinearToOklab(Gradient.colors[c].rgb), Gradient.type == 2);
                color = lerp(color, color2, lerp(colorPos, step(0.01, colorPos), Gradient.type % 2)); // grad.type == 1 is fixed, 0 and 2 are blends.
            }
            color = lerp(color, OklabToLinear(color), Gradient.type == 2);
        
        #ifdef UNITY_COLORSPACE_GAMMA
            color = LinearToSRGB(color);
        #endif
        
            float alpha = Gradient.alphas[0].x;
            [unroll]
            for (int a = 1; a < Gradient.alphasLength; a++)
            {
                float alphaPos = saturate((Time - Gradient.alphas[a - 1].y) / (Gradient.alphas[a].y - Gradient.alphas[a - 1].y)) * step(a, Gradient.alphasLength - 1);
                alpha = lerp(alpha, Gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), Gradient.type % 2));
            }
        
            Out = float4(color, alpha);
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
        Out = A * B;
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        struct Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float
        {
        };
        
        void SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(float3 Vector3_7940555B, float Vector1_1B8B9078, Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float IN, out float Value_0)
        {
        float3 _Property_44999cc87708de82a26b39ae1da975ec_Out_0_Vector3 = Vector3_7940555B;
        float _Property_dad5add45a7fa785be976f925bc5a5da_Out_0_Float = Vector1_1B8B9078;
        float3 _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3;
        Unity_Multiply_float3_float3(_Property_44999cc87708de82a26b39ae1da975ec_Out_0_Vector3, (_Property_dad5add45a7fa785be976f925bc5a5da_Out_0_Float.xxx), _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3);
        float _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float;
        PerlinNoise3D_float(_Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2_Vector3, _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float);
        float _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float;
        Unity_Remap_float(_PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1_Float, float2 (-1.15, 1.15), float2 (0, 1), _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float);
        Value_0 = _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3_Float;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_RandomRange_float(float2 Seed, float Min, float Max, out float Out)
        {
             float randomno =  frac(sin(dot(Seed, float2(12.9898, 78.233)))*43758.5453);
             Out = lerp(Min, Max, randomno);
        }
        
        struct Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float
        {
        };
        
        void SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(float3 Vector3_375394F7, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float IN, out float Value_1, out float Cells_2)
        {
        float3 _Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3 = Vector3_375394F7;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        VoronoiPrecise3D_float(_Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _VoronoiPrecise3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }
        
        float3 Unity_HDRP_GetEmissionHDRColor_float(float3 ldrColor, float luminanceIntensity, float exposureWeight)
        {
            float3 hdrColor = ldrColor * luminanceIntensity;
        
            #ifdef SHADERGRAPH_PREVIEW
            float inverseExposureMultiplier = 1.0;
            #else
            float inverseExposureMultiplier = GetInverseCurrentExposureMultiplier();
            #endif
        
            // Inverse pre-expose using _EmissiveExposureWeight weight
            hdrColor = lerp(hdrColor * inverseExposureMultiplier, hdrColor, exposureWeight);
            return hdrColor;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
            // Graph Vertex
            // GraphVertex: <None>
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float3 Emission;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_d09e7f2c2a774dbd93cc943fa6a50940_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_VoidColor) : _VoidColor;
            float _Property_16eef79ff58443149895b35d0a88bdb2_Out_0_Float = _InnerEdge;
            float4 _UV_ce55b36e72754a339073e4c05a64b551_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_054563945a0a4f688fce11f0c067b0c8_Out_1_Vector2 = _UV_ce55b36e72754a339073e4c05a64b551_Out_0_Vector4.xy;
            float _Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float;
            Unity_Distance_float2(_Swizzle_054563945a0a4f688fce11f0c067b0c8_Out_1_Vector2, float2(0.5, 0.5), _Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float);
            float _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float;
            Unity_OneMinus_float(_Distance_c4ffb139341a42d3b5cf9eacb7ebf32c_Out_2_Float, _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float);
            float _Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float;
            Unity_Smoothstep_float(_Property_16eef79ff58443149895b35d0a88bdb2_Out_0_Float, float(1), _OneMinus_ddb5d9db7caf4b62a568bde921ce8df6_Out_1_Float, _Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float);
            Gradient _Gradient_c34cf660310448768ec81f4600b23ad8_Out_0_Gradient = NewGradient(0, 4, 2, float4(0, 0, 0, 0.271931),float4(0.01568037, 0.01568037, 0.01568037, 0.4678416),float4(0.1071531, 0.1071531, 0.1071531, 0.6052644),float4(1, 1, 1, 1),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0),float4(0, 0, 0, 0), float2(1, 0),float2(1, 1),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0),float2(0, 0));
            float _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float;
            Unity_DotProduct_float3(IN.ObjectSpaceViewDirection, IN.ObjectSpaceNormal, _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float);
            float4 _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4;
            Unity_SampleGradientV1_float(_Gradient_c34cf660310448768ec81f4600b23ad8_Out_0_Gradient, _DotProduct_a9da63c888204935a4f48734c8f433f6_Out_2_Float, _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4);
            float4 _Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Smoothstep_c41c6d39dc2d4ffdb25085c12ad99efd_Out_3_Float.xxxx), _SampleGradient_55be3ad199724950a8ea6fef206482f3_Out_2_Vector4, _Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4);
            Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a;
            float _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float;
            SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(IN.ObjectSpaceViewDirection, float(10), _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a, _Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float);
            float3 _Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3;
            Unity_Multiply_float3_float3(IN.ObjectSpaceViewDirection, (_Perlinnoise3D_2ef838cde24a44709261ea9a8b27b71a_Value_0_Float.xxx), _Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3);
            float _Property_ba2d1376c19c40fbadc820d0fb721b2e_Out_0_Float = _Speed;
            float _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_ba2d1376c19c40fbadc820d0fb721b2e_Out_0_Float, _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float);
            float2 _Property_2cfd9c61936548ee85383d83df6beba0_Out_0_Vector2 = _Seed;
            float _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float;
            Unity_RandomRange_float(_Property_2cfd9c61936548ee85383d83df6beba0_Out_0_Vector2, float(26.96), float(45.6), _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float);
            Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a;
            float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Value_1_Float;
            float _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float;
            SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(_Multiply_69f96c9f7a0c42dc849cb6a4562c1f6e_Out_2_Vector3, _Multiply_972bec3a2e4045fabdfe031341bb86a6_Out_2_Float, _RandomRange_f106151978f44fb29a96aa5d6ed49c70_Out_3_Float, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Value_1_Float, _Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float);
            float4 _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_41ac645f27f44e63bad0c6171eae2689_Out_2_Vector4, (_Voronoiprecisenoise3D_f47c074d44614ac082fde0af92af746a_Cells_2_Float.xxxx), _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4);
            float4 _Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_d09e7f2c2a774dbd93cc943fa6a50940_Out_0_Vector4, _Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4, _Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4);
            float4 _Property_e0f9565d1eaa452fb638faa34cc517eb_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_BorderColor) : _BorderColor;
            float3 _Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3;
            Unity_Absolute_float3(IN.AbsoluteWorldSpacePosition, _Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3);
            Bindings_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1;
            float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Value_1_Float;
            float _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float;
            SG_Voronoiprecisenoise3D_cf19a184f7b476448807f17724b543af_float(_Absolute_63648fa9872644aa973fb29c5f563d60_Out_1_Vector3, IN.TimeParameters.x, float(6.2), _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1, _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Value_1_Float, _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float);
            float _Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float;
            Unity_Smoothstep_float(float(-0.41), float(1), _Voronoiprecisenoise3D_558c89b2c8864e7989b946a30894f5f1_Cells_2_Float, _Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float);
            float _Property_82bfb359e6494ff08a54e8c08976a6f1_Out_0_Float = _FresnelPower;
            float _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_82bfb359e6494ff08a54e8c08976a6f1_Out_0_Float, _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float);
            float _Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_5e97616f0a19459b9491b96a21be8928_Out_3_Float, _FresnelEffect_2d6f355635c540ddb1979786af942137_Out_3_Float, _Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float);
            float4 _Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_e0f9565d1eaa452fb638faa34cc517eb_Out_0_Vector4, (_Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float.xxxx), _Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4);
            float3 _EmissionNode_87074dafd5ff4d39bf4005b1089a56e4_Output_0_Vector3 = Unity_HDRP_GetEmissionHDRColor_float((_Multiply_c830107a56a244ddb418529801929bdf_Out_2_Vector4.xyz).xyz, float(3), float(0));
            float4 _Add_0b088d988049415c84334d987e692959_Out_2_Vector4;
            Unity_Add_float4(_Multiply_bb7e464894574e7bb998e736a52f3bfa_Out_2_Vector4, (_Multiply_4f82bb0eed3147b38e80899ad2bdfff3_Out_2_Float.xxxx), _Add_0b088d988049415c84334d987e692959_Out_2_Vector4);
            surface.BaseColor = (_Multiply_c99894632a3d4233816d32eda945021b_Out_2_Vector4.xyz);
            surface.Emission = _EmissionNode_87074dafd5ff4d39bf4005b1089a56e4_Output_0_Vector3;
            surface.Alpha = (_Add_0b088d988049415c84334d987e692959_Out_2_Vector4).x;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES AttributesMesh
            #define VaryingsMeshType VaryingsMeshToPS
            #define VFX_SRP_VARYINGS VaryingsMeshType
            #define VFX_SRP_SURFACE_INPUTS FragInputs
            #endif
            SurfaceDescriptionInputs FragInputsToSurfaceDescriptionInputs(FragInputs input, float3 viewWS)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            output.WorldSpaceNormal =                           normalize(input.tangentToWorld[2].xyz);
            #if defined(SHADER_STAGE_RAY_TRACING)
            output.ObjectSpaceNormal =                          mul(output.WorldSpaceNormal, (float3x3) ObjectToWorld3x4());
            #else
            output.ObjectSpaceNormal =                          normalize(mul(output.WorldSpaceNormal, (float3x3) UNITY_MATRIX_M));           // transposed multiplication by inverse matrix to handle normal scale
            #endif
            output.WorldSpaceViewDirection =                    normalize(viewWS);
            output.ObjectSpaceViewDirection =                   TransformWorldToObjectDir(output.WorldSpaceViewDirection);
        #if SHADERPASS != SHADERPASS_FOG_VOLUME_VOXELIZATION
        #else
        #endif
            output.AbsoluteWorldSpacePosition =                 GetAbsolutePositionWS(input.positionRWS);
        #if SHADERPASS != SHADERPASS_FOG_VOLUME_VOXELIZATION
        #else
        #endif
        
        #if UNITY_UV_STARTS_AT_TOP
        #else
        #endif
        
        
            output.uv0 =                                        input.texCoord0;
            output.TimeParameters =                             _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        
            // splice point to copy frag inputs custom interpolator pack into the SDI
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorCopyToSDI' */
        
            return output;
        }
        
            // --------------------------------------------------
            // Build Surface Data (Specific Material)
        
        void ApplyDecalToSurfaceDataNoNormal(DecalSurfaceData decalSurfaceData, inout SurfaceData surfaceData);
        
        void ApplyDecalAndGetNormal(FragInputs fragInputs, PositionInputs posInput, SurfaceDescription surfaceDescription,
            inout SurfaceData surfaceData)
        {
            float3 doubleSidedConstants = GetDoubleSidedConstants();
        
        #ifdef DECAL_NORMAL_BLENDING
            // SG nodes don't ouptut surface gradients, so if decals require surf grad blending, we have to convert
            // the normal to gradient before applying the decal. We then have to resolve the gradient back to world space
            float3 normalTS;
        
        
        
            #if HAVE_DECALS
            if (_EnableDecals)
            {
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs, alpha);
                ApplyDecalToSurfaceNormal(decalSurfaceData, fragInputs.tangentToWorld[2], normalTS);
                ApplyDecalToSurfaceDataNoNormal(decalSurfaceData, surfaceData);
            }
            #endif
        
            GetNormalWS_SG(fragInputs, normalTS, surfaceData.normalWS, doubleSidedConstants);
        #else
            // normal delivered to master node
        
            #if HAVE_DECALS
            if (_EnableDecals)
            {
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                // Both uses and modifies 'surfaceData.normalWS'.
                DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs, alpha);
                ApplyDecalToSurfaceNormal(decalSurfaceData, surfaceData.normalWS.xyz);
                ApplyDecalToSurfaceDataNoNormal(decalSurfaceData, surfaceData);
            }
            #endif
        #endif
        }
        void BuildSurfaceData(FragInputs fragInputs, inout SurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData)
        {
            // setup defaults -- these are used if the graph doesn't output a value
            ZERO_INITIALIZE(SurfaceData, surfaceData);
        
            // copy across graph values, if defined
            surfaceData.color = surfaceDescription.BaseColor;
        
            #ifdef WRITE_NORMAL_BUFFER
            // When we need to export the normal (in the depth prepass, we write the geometry one)
            surfaceData.normalWS = fragInputs.tangentToWorld[2];
            #endif
        
            #if defined(DEBUG_DISPLAY)
            #if !defined(SHADER_STAGE_RAY_TRACING)
            // Mipmap mode debugging isn't supported with ray tracing as it relies on derivatives
                if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                {
                    #ifdef FRAG_INPUTS_USE_TEXCOORD0
                        surfaceData.color = GET_TEXTURE_STREAMING_DEBUG(posInput.positionSS, fragInputs.texCoord0);
                    #else
                        surfaceData.color = GET_TEXTURE_STREAMING_DEBUG_NO_UV(posInput.positionSS);
                    #endif
                }
            #endif
            #endif
        
            #ifdef _ENABLE_SHADOW_MATTE
        
                #if (SHADERPASS == SHADERPASS_FORWARD_UNLIT) || (SHADERPASS == SHADERPASS_RAYTRACING_GBUFFER) || (SHADERPASS == SHADERPASS_RAYTRACING_INDIRECT) || (SHADERPASS == SHADERPASS_RAYTRACING_FORWARD)
        
                    HDShadowContext shadowContext = InitShadowContext();
        
                    // Evaluate the shadow, the normal is guaranteed if shadow matte is enabled on this shader.
                    float3 shadow3;
                    ShadowLoopMin(shadowContext, posInput, normalize(fragInputs.tangentToWorld[2]), asuint(_ShadowMatteFilter), GetMeshRenderingLayerMask(), shadow3);
        
                    // Compute the average value in the fourth channel
                    float4 shadow = float4(shadow3, dot(shadow3, float3(1.0/3.0, 1.0/3.0, 1.0/3.0)));
        
                    float4 shadowColor = (1.0 - shadow) * surfaceDescription.ShadowTint.rgba;
                    float  localAlpha  = saturate(shadowColor.a + surfaceDescription.Alpha);
        
                    // Keep the nested lerp
                    // With no Color (bsdfData.color.rgb, bsdfData.color.a == 0.0f), just use ShadowColor*Color to avoid a ring of "white" around the shadow
                    // And mix color to consider the Color & ShadowColor alpha (from texture or/and color picker)
                    #ifdef _SURFACE_TYPE_TRANSPARENT
                        surfaceData.color = lerp(shadowColor.rgb * surfaceData.color, lerp(lerp(shadowColor.rgb, surfaceData.color, 1.0 - surfaceDescription.ShadowTint.a), surfaceData.color, shadow.rgb), surfaceDescription.Alpha);
                    #else
                        surfaceData.color = lerp(lerp(shadowColor.rgb, surfaceData.color, 1.0 - surfaceDescription.ShadowTint.a), surfaceData.color, shadow.rgb);
                    #endif
                    localAlpha = ApplyBlendMode(surfaceData.color, localAlpha).a;
        
                    surfaceDescription.Alpha = localAlpha;
        
                #elif SHADERPASS == SHADERPASS_PATH_TRACING
        
                    surfaceData.normalWS = fragInputs.tangentToWorld[2];
                    surfaceData.shadowTint = surfaceDescription.ShadowTint.rgba;
        
                #endif
        
            #endif // _ENABLE_SHADOW_MATTE
        }
        
            // --------------------------------------------------
            // Get Surface And BuiltinData
        
            void GetSurfaceAndBuiltinData(FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData RAY_TRACING_OPTIONAL_PARAMETERS)
            {
                // Don't dither if displaced tessellation (we're fading out the displacement instead to match the next LOD)
                #if !defined(SHADER_STAGE_RAY_TRACING) && !defined(_TESSELLATION_DISPLACEMENT)
                #ifdef LOD_FADE_CROSSFADE // enable dithering LOD transition if user select CrossFade transition in LOD group
                LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                #endif
                #endif
        
                #ifndef SHADER_UNLIT
                #ifdef _DOUBLESIDED_ON
                    float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
                #else
                    float3 doubleSidedConstants = float3(1.0, 1.0, 1.0);
                #endif
        
                ApplyDoubleSidedFlipOrMirror(fragInputs, doubleSidedConstants); // Apply double sided flip on the vertex normal
                #endif // SHADER_UNLIT
        
                SurfaceDescriptionInputs surfaceDescriptionInputs = FragInputsToSurfaceDescriptionInputs(fragInputs, V);
        
                #if defined(HAVE_VFX_MODIFICATION)
                GraphProperties properties;
                ZERO_INITIALIZE(GraphProperties, properties);
        
                GetElementPixelProperties(fragInputs, properties);
        
                SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs, properties);
                #else
                SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);
                #endif
        
                #ifdef DEBUG_DISPLAY
                if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                {
                    surfaceDescription.Alpha = 1.0f;
                }
                #endif
        
                // Perform alpha test very early to save performance (a killed pixel will not sample textures)
                // TODO: split graph evaluation to grab just alpha dependencies first? tricky..
                #ifdef _ALPHATEST_ON
                    float alphaCutoff = surfaceDescription.AlphaClipThreshold;
                    #if SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_PREPASS
                    // The TransparentDepthPrepass is also used with SSR transparent.
                    // If an artists enable transaprent SSR but not the TransparentDepthPrepass itself, then we use AlphaClipThreshold
                    // otherwise if TransparentDepthPrepass is enabled we use AlphaClipThresholdDepthPrepass
                    #elif SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_POSTPASS
                    // DepthPostpass always use its own alpha threshold
                    alphaCutoff = surfaceDescription.AlphaClipThresholdDepthPostpass;
                    #elif (SHADERPASS == SHADERPASS_SHADOWS) || (SHADERPASS == SHADERPASS_RAYTRACING_VISIBILITY)
                    // If use shadow threshold isn't enable we don't allow any test
                    #endif
        
                    GENERIC_ALPHA_TEST(surfaceDescription.Alpha, alphaCutoff);
                #endif
        
                #if !defined(SHADER_STAGE_RAY_TRACING) && _DEPTHOFFSET_ON
                ApplyDepthOffsetPositionInput(V, surfaceDescription.DepthOffset, GetViewForwardDir(), GetWorldToHClipMatrix(), posInput);
                #endif
        
                #ifndef SHADER_UNLIT
                float3 bentNormalWS;
                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData, bentNormalWS);
        
                #ifdef FRAG_INPUTS_USE_TEXCOORD1
                    float4 lightmapTexCoord1 = fragInputs.texCoord1;
                #else
                    float4 lightmapTexCoord1 = float4(0,0,0,0);
                #endif
        
                #ifdef FRAG_INPUTS_USE_TEXCOORD2
                    float4 lightmapTexCoord2 = fragInputs.texCoord2;
                #else
                    float4 lightmapTexCoord2 = float4(0,0,0,0);
                #endif
        
                float alpha = 1.0;
                alpha = surfaceDescription.Alpha;
        
                // Builtin Data
                // For back lighting we use the oposite vertex normal
                InitBuiltinData(posInput, alpha, bentNormalWS, -fragInputs.tangentToWorld[2], lightmapTexCoord1, lightmapTexCoord2, builtinData);
        
                #else
                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData);
        
                ZERO_BUILTIN_INITIALIZE(builtinData); // No call to InitBuiltinData as we don't have any lighting
                builtinData.opacity = surfaceDescription.Alpha;
        
                #if defined(DEBUG_DISPLAY)
                    // Light Layers are currently not used for the Unlit shader (because it is not lit)
                    // But Unlit objects do cast shadows according to their rendering layer mask, which is what we want to
                    // display in the light layers visualization mode, therefore we need the renderingLayers
                    builtinData.renderingLayers = GetMeshRenderingLayerMask();
                #endif
        
                #endif // SHADER_UNLIT
        
                #ifdef _ALPHATEST_ON
                    // Used for sharpening by alpha to mask - Alpha to covertage is only used with depth only and forward pass (no shadow pass, no transparent pass)
                    builtinData.alphaClipTreshold = alphaCutoff;
                #endif
        
                // override sampleBakedGI - not used by Unlit
        		// When overriding GI, we need to force the isLightmap flag to make sure we don't add APV (sampled in the lightloop) on top of the overridden value (set at GBuffer stage)
        
                builtinData.emissiveColor = surfaceDescription.Emission;
        
                // Note this will not fully work on transparent surfaces (can check with _SURFACE_TYPE_TRANSPARENT define)
                // We will always overwrite vt feeback with the nearest. So behind transparent surfaces vt will not be resolved
                // This is a limitation of the current MRT approach.
                #ifdef UNITY_VIRTUAL_TEXTURING
                #endif
        
                #if _DEPTHOFFSET_ON
                builtinData.depthOffset = surfaceDescription.DepthOffset;
                #endif
        
                // TODO: We should generate distortion / distortionBlur for non distortion pass
                #if (SHADERPASS == SHADERPASS_DISTORTION)
                builtinData.distortion = surfaceDescription.Distortion;
                builtinData.distortionBlur = surfaceDescription.DistortionBlur;
                #endif
        
                #ifndef SHADER_UNLIT
                // PostInitBuiltinData call ApplyDebugToBuiltinData
                PostInitBuiltinData(V, posInput, surfaceData, builtinData);
                #else
                ApplyDebugToBuiltinData(builtinData);
                #endif
        
                RAY_TRACING_OPTIONAL_ALPHA_TEST_PASS
            }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassPathTracing.hlsl"
        
            // --------------------------------------------------
            // Visual Effect Vertex Invocations
        
        	#ifdef HAVE_VFX_MODIFICATION
                #if !defined(SHADER_STAGE_RAY_TRACING)
        	    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/VisualEffectVertex.hlsl"
                #else
                #endif
        	#endif
        
            ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    CustomEditorForRenderPipeline "Rendering.HighDefinition.HDUnlitGUI" "UnityEngine.Rendering.HighDefinition.HDRenderPipelineAsset"
    FallBack "Hidden/Shader Graph/FallbackError"
}