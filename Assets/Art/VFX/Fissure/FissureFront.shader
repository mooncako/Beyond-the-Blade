Shader "Shader/FissureFront"
{
    Properties
    {
        [NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
        _FissureProgress("FissureProgress", Range(0, 1)) = 0.44
        _NoiseOffset("NoiseOffset", Float) = 10
        _NoiseDensity("NoiseDensity", Float) = 10
        [HDR]_FissureColor("FissureColor", Color) = (19.08669, 0, 103.9683, 0)
        _Color("Color", Color) = (0, 0, 0, 0)
        _Edge("Edge", Float) = 0.28
        _Offset("Offset", Float) = 0.05
        [HideInInspector]_EmissionColor("Color", Color) = (1, 1, 1, 1)
        [HideInInspector]_RenderQueueType("Float", Float) = 1
        [HideInInspector][ToggleUI]_AddPrecomputedVelocity("Boolean", Float) = 0
        [HideInInspector][ToggleUI]_DepthOffsetEnable("Boolean", Float) = 0
        [HideInInspector][ToggleUI]_ConservativeDepthOffsetEnable("Boolean", Float) = 0
        [HideInInspector][ToggleUI]_TransparentWritingMotionVec("Boolean", Float) = 0
        [HideInInspector][ToggleUI]_AlphaCutoffEnable("Boolean", Float) = 1
        [HideInInspector]_TransparentSortPriority("_TransparentSortPriority", Float) = 0
        [HideInInspector][ToggleUI]_UseShadowThreshold("Boolean", Float) = 0
        [HideInInspector][ToggleUI]_DoubleSidedEnable("Boolean", Float) = 1
        [HideInInspector][Enum(Flip, 0, Mirror, 1, None, 2)]_DoubleSidedNormalMode("Float", Float) = 2
        [HideInInspector]_DoubleSidedConstants("Vector4", Vector) = (1, 1, -1, 0)
        [HideInInspector][Enum(Auto, 0, On, 1, Off, 2)]_DoubleSidedGIMode("Float", Float) = 0
        [HideInInspector][ToggleUI]_TransparentDepthPrepassEnable("Boolean", Float) = 0
        [HideInInspector][ToggleUI]_TransparentDepthPostpassEnable("Boolean", Float) = 0
        [HideInInspector][ToggleUI]_PerPixelSorting("Boolean", Float) = 0
        [HideInInspector]_SurfaceType("Float", Float) = 0
        [HideInInspector]_BlendMode("Float", Float) = 0
        [HideInInspector]_SrcBlend("Float", Float) = 1
        [HideInInspector]_DstBlend("Float", Float) = 0
        [HideInInspector]_DstBlend2("Float", Float) = 0
        [HideInInspector]_AlphaSrcBlend("Float", Float) = 1
        [HideInInspector]_AlphaDstBlend("Float", Float) = 0
        [HideInInspector][ToggleUI]_ZWrite("Boolean", Float) = 1
        [HideInInspector][ToggleUI]_TransparentZWrite("Boolean", Float) = 0
        [HideInInspector]_CullMode("Float", Float) = 2
        [HideInInspector][ToggleUI]_EnableFogOnTransparent("Boolean", Float) = 1
        [HideInInspector]_CullModeForward("Float", Float) = 2
        [HideInInspector][Enum(Front, 1, Back, 2)]_TransparentCullMode("Float", Float) = 2
        [HideInInspector][Enum(UnityEngine.Rendering.HighDefinition.OpaqueCullMode)]_OpaqueCullMode("Float", Float) = 2
        [HideInInspector]_ZTestDepthEqualForOpaque("Float", Int) = 3
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
            "Queue"="AlphaTest+0"
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
            #pragma shader_feature_local _ _ALPHATEST_ON
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
        float4 _MainTex_TexelSize;
        float _FissureProgress;
        float _NoiseOffset;
        float _NoiseDensity;
        float4 _FissureColor;
        float4 _Color;
        float _Edge;
        float _Offset;
        float4 _EmissionColor;
        float _UseShadowThreshold;
        float4 _DoubleSidedConstants;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        float _BlendMode;
        float _EnableBlendModePreserveSpecularLighting;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
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
            #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi2D.hlsl"
        
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
             float4 uv0;
        };
        struct PackedVaryingsMeshToPS
        {
            SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
        };
        
            PackedVaryingsMeshToPS PackVaryingsMeshToPS (VaryingsMeshToPS input)
        {
            PackedVaryingsMeshToPS output;
            ZERO_INITIALIZE(PackedVaryingsMeshToPS, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            return output;
        }
        
        VaryingsMeshToPS UnpackVaryingsMeshToPS (PackedVaryingsMeshToPS input)
        {
            VaryingsMeshToPS output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
        
            // Graph Functions
            
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Step_float(float Edge, float In, out float Out)
        {
            Out = step(Edge, In);
        }
        
        struct Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float
        {
        };
        
        void SG_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float(float2 Vector2_DDA00F47, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float IN, out float Value_1, out float Cells_2)
        {
        float2 _Property_87196192b55192848c90533525880767_Out_0_Vector2 = Vector2_DDA00F47;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        VoronoiPrecise2D_float(_Property_87196192b55192848c90533525880767_Out_0_Vector2, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
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
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.tex, _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.samplerstate, _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_R_4_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.r;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_G_5_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.g;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_B_6_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.b;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_A_7_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.a;
            float _Property_43276f8cd0bb43c791ef6a5569938c47_Out_0_Float = _FissureProgress;
            float _Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float;
            Unity_Lerp_float(float(1), float(0.35), _Property_43276f8cd0bb43c791ef6a5569938c47_Out_0_Float, _Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float);
            float4 _UV_f886bd3ce6e14698b48085645e8892a4_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2 = _UV_f886bd3ce6e14698b48085645e8892a4_Out_0_Vector4.xy;
            float _Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float;
            Unity_Distance_float2(_Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2, float2(0.5, 0.5), _Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float);
            float _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float;
            Unity_OneMinus_float(_Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float, _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float);
            float _Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float;
            Unity_Step_float(_Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float, _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float, _Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float);
            float _Property_65f904d6199d4350a89b9904628b7f25_Out_0_Float = _FissureProgress;
            float _Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float;
            Unity_Lerp_float(float(3), float(0), _Property_65f904d6199d4350a89b9904628b7f25_Out_0_Float, _Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float);
            float _Property_a85bc01ad5de45428bea3152b546512d_Out_0_Float = _NoiseOffset;
            float _Property_b39de3d823764eb4b0936e62e6fe5074_Out_0_Float = _NoiseDensity;
            Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b;
            float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float;
            float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float;
            SG_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float(_Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2, _Property_a85bc01ad5de45428bea3152b546512d_Out_0_Float, _Property_b39de3d823764eb4b0936e62e6fe5074_Out_0_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float);
            float _Step_34f306488de6462093199997839c7cc3_Out_2_Float;
            Unity_Step_float(_Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float, _Step_34f306488de6462093199997839c7cc3_Out_2_Float);
            float _Property_7ea25f3e615d42c5b48c4467642bef17_Out_0_Float = _FissureProgress;
            float _Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float;
            Unity_Lerp_float(float(0.04), float(0.01), _Property_7ea25f3e615d42c5b48c4467642bef17_Out_0_Float, _Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float);
            float _Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float;
            Unity_Step_float(_Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float, _Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float);
            float _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float;
            Unity_OneMinus_float(_Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float, _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float);
            float _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float;
            Unity_Add_float(_Step_34f306488de6462093199997839c7cc3_Out_2_Float, _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float, _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float);
            float _Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float;
            Unity_Multiply_float_float(_Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float, _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float, _Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float);
            float4 _Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4, (_Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float.xxxx), _Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4);
            surface.Alpha = (_Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4).x;
            surface.AlphaClipThreshold = float(0.5);
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
        
            #if defined(SHADER_STAGE_RAY_TRACING)
            #else
            #endif
        #if SHADERPASS != SHADERPASS_FOG_VOLUME_VOXELIZATION
        #else
        #endif
        #if SHADERPASS != SHADERPASS_FOG_VOLUME_VOXELIZATION
        #else
        #endif
        
        #if UNITY_UV_STARTS_AT_TOP
        #else
        #endif
        
        
            output.uv0 =                                        input.texCoord0;
        
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
        #pragma shader_feature_local _ _ALPHATEST_ON
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
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define ATTRIBUTES_NEED_TEXCOORD2
            #define ATTRIBUTES_NEED_TEXCOORD3
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_POSITIONPREDISPLACEMENT_WS
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
        float4 _MainTex_TexelSize;
        float _FissureProgress;
        float _NoiseOffset;
        float _NoiseDensity;
        float4 _FissureColor;
        float4 _Color;
        float _Edge;
        float _Offset;
        float4 _EmissionColor;
        float _UseShadowThreshold;
        float4 _DoubleSidedConstants;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        float _BlendMode;
        float _EnableBlendModePreserveSpecularLighting;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
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
            #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi2D.hlsl"
        #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi3D.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonLighting.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            struct AttributesMesh
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
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
             float3 AbsoluteWorldSpacePosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct PackedVaryingsMeshToPS
        {
            SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 texCoord1 : INTERP1;
             float4 texCoord2 : INTERP2;
             float4 texCoord3 : INTERP3;
             float3 positionRWS : INTERP4;
             float3 positionPredisplacementRWS : INTERP5;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
        };
        
            PackedVaryingsMeshToPS PackVaryingsMeshToPS (VaryingsMeshToPS input)
        {
            PackedVaryingsMeshToPS output;
            ZERO_INITIALIZE(PackedVaryingsMeshToPS, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.texCoord1.xyzw = input.texCoord1;
            output.texCoord2.xyzw = input.texCoord2;
            output.texCoord3.xyzw = input.texCoord3;
            output.positionRWS.xyz = input.positionRWS;
            output.positionPredisplacementRWS.xyz = input.positionPredisplacementRWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            return output;
        }
        
        VaryingsMeshToPS UnpackVaryingsMeshToPS (PackedVaryingsMeshToPS input)
        {
            VaryingsMeshToPS output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.texCoord1 = input.texCoord1.xyzw;
            output.texCoord2 = input.texCoord2.xyzw;
            output.texCoord3 = input.texCoord3.xyzw;
            output.positionRWS = input.positionRWS.xyz;
            output.positionPredisplacementRWS = input.positionPredisplacementRWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
        
            // Graph Functions
            
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
        struct Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float
        {
        };
        
        void SG_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float(float2 Vector2_DDA00F47, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float IN, out float Value_1, out float Cells_2)
        {
        float2 _Property_87196192b55192848c90533525880767_Out_0_Vector2 = Vector2_DDA00F47;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        VoronoiPrecise2D_float(_Property_87196192b55192848c90533525880767_Out_0_Vector2, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Step_float(float Edge, float In, out float Out)
        {
            Out = step(Edge, In);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        struct Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float
        {
        };
        
        void SG_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float(float3 Vector3_375394F7, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float IN, out float Value_1, out float Cells_2)
        {
        float3 _Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3 = Vector3_375394F7;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        Voronoi3D_float(_Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
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
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_683e3b8c31014b2397e10acee02ca4a2_Out_0_Vector4 = _Color;
            UnityTexture2D _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.tex, _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.samplerstate, _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_R_4_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.r;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_G_5_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.g;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_B_6_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.b;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_A_7_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.a;
            float4 _Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_683e3b8c31014b2397e10acee02ca4a2_Out_0_Vector4, _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4, _Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4);
            float _Property_65f904d6199d4350a89b9904628b7f25_Out_0_Float = _FissureProgress;
            float _Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float;
            Unity_Lerp_float(float(3), float(0), _Property_65f904d6199d4350a89b9904628b7f25_Out_0_Float, _Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float);
            float4 _UV_f886bd3ce6e14698b48085645e8892a4_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2 = _UV_f886bd3ce6e14698b48085645e8892a4_Out_0_Vector4.xy;
            float _Property_a85bc01ad5de45428bea3152b546512d_Out_0_Float = _NoiseOffset;
            float _Property_b39de3d823764eb4b0936e62e6fe5074_Out_0_Float = _NoiseDensity;
            Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b;
            float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float;
            float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float;
            SG_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float(_Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2, _Property_a85bc01ad5de45428bea3152b546512d_Out_0_Float, _Property_b39de3d823764eb4b0936e62e6fe5074_Out_0_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float);
            float _Step_34f306488de6462093199997839c7cc3_Out_2_Float;
            Unity_Step_float(_Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float, _Step_34f306488de6462093199997839c7cc3_Out_2_Float);
            float _Property_7ea25f3e615d42c5b48c4467642bef17_Out_0_Float = _FissureProgress;
            float _Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float;
            Unity_Lerp_float(float(0.04), float(0.01), _Property_7ea25f3e615d42c5b48c4467642bef17_Out_0_Float, _Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float);
            float _Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float;
            Unity_Step_float(_Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float, _Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float);
            float _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float;
            Unity_OneMinus_float(_Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float, _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float);
            float _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float;
            Unity_Add_float(_Step_34f306488de6462093199997839c7cc3_Out_2_Float, _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float, _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float);
            float4 _Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_FissureColor) : _FissureColor;
            float4 _Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Add_1896dee97c9e464eb43461caea40c052_Out_2_Float.xxxx), _Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4, _Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4);
            float _Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float = _Edge;
            float4 _UV_9adc53fa249a4a5ab32d3a050acbfdaf_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_2029f5a6f61a4f00a6eb3e4c29b5b6af_Out_1_Vector2 = _UV_9adc53fa249a4a5ab32d3a050acbfdaf_Out_0_Vector4.xy;
            float _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float;
            Unity_Distance_float2(_Swizzle_2029f5a6f61a4f00a6eb3e4c29b5b6af_Out_1_Vector2, float2(0.5, 0.5), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float);
            float _Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float;
            Unity_Smoothstep_float(_Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float, float(1), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float, _Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float);
            float3 _Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3;
            Unity_Absolute_float3(IN.AbsoluteWorldSpacePosition, _Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3);
            Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7;
            float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Value_1_Float;
            float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float;
            SG_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float(_Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3, IN.TimeParameters.x, float(4.1), _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Value_1_Float, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float);
            float _Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float, _Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float);
            float _Property_4b01ac9dd6b047d2b9cf45fca7f0e366_Out_0_Float = _Offset;
            float _Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float;
            Unity_Add_float(_Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float, _Property_4b01ac9dd6b047d2b9cf45fca7f0e366_Out_0_Float, _Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float);
            float _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float;
            Unity_Smoothstep_float(_Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float, float(1), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float, _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float);
            float _Add_1f600a25927b426195eb7a73491937ac_Out_2_Float;
            Unity_Add_float(_Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float, _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float, _Add_1f600a25927b426195eb7a73491937ac_Out_2_Float);
            float4 _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4, (_Add_1f600a25927b426195eb7a73491937ac_Out_2_Float.xxxx), _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4);
            float4 _Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4, _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4, _Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4);
            float3 _EmissionNode_3e929fcc60694caa888171a6f04c27d9_Output_0_Vector3 = Unity_HDRP_GetEmissionHDRColor_float((_Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4.xyz).xyz, float(0.5), float(0.9));
            float _Property_43276f8cd0bb43c791ef6a5569938c47_Out_0_Float = _FissureProgress;
            float _Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float;
            Unity_Lerp_float(float(1), float(0.35), _Property_43276f8cd0bb43c791ef6a5569938c47_Out_0_Float, _Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float);
            float _Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float;
            Unity_Distance_float2(_Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2, float2(0.5, 0.5), _Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float);
            float _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float;
            Unity_OneMinus_float(_Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float, _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float);
            float _Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float;
            Unity_Step_float(_Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float, _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float, _Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float);
            float _Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float;
            Unity_Multiply_float_float(_Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float, _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float, _Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float);
            float4 _Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4, (_Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float.xxxx), _Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4);
            surface.BaseColor = (_Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4.xyz);
            surface.Emission = _EmissionNode_3e929fcc60694caa888171a6f04c27d9_Output_0_Vector3;
            surface.Alpha = (_Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4).x;
            surface.AlphaClipThreshold = float(0.5);
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
        
            #if defined(SHADER_STAGE_RAY_TRACING)
            #else
            #endif
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
            #pragma shader_feature_local _ _ALPHATEST_ON
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
        float4 _MainTex_TexelSize;
        float _FissureProgress;
        float _NoiseOffset;
        float _NoiseDensity;
        float4 _FissureColor;
        float4 _Color;
        float _Edge;
        float _Offset;
        float4 _EmissionColor;
        float _UseShadowThreshold;
        float4 _DoubleSidedConstants;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        float _BlendMode;
        float _EnableBlendModePreserveSpecularLighting;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
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
            #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi2D.hlsl"
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
            
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
        struct Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float
        {
        };
        
        void SG_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float(float2 Vector2_DDA00F47, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float IN, out float Value_1, out float Cells_2)
        {
        float2 _Property_87196192b55192848c90533525880767_Out_0_Vector2 = Vector2_DDA00F47;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        VoronoiPrecise2D_float(_Property_87196192b55192848c90533525880767_Out_0_Vector2, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Step_float(float Edge, float In, out float Out)
        {
            Out = step(Edge, In);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        struct Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float
        {
        };
        
        void SG_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float(float3 Vector3_375394F7, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float IN, out float Value_1, out float Cells_2)
        {
        float3 _Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3 = Vector3_375394F7;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        Voronoi3D_float(_Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
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
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_683e3b8c31014b2397e10acee02ca4a2_Out_0_Vector4 = _Color;
            UnityTexture2D _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.tex, _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.samplerstate, _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_R_4_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.r;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_G_5_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.g;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_B_6_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.b;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_A_7_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.a;
            float4 _Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_683e3b8c31014b2397e10acee02ca4a2_Out_0_Vector4, _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4, _Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4);
            float _Property_65f904d6199d4350a89b9904628b7f25_Out_0_Float = _FissureProgress;
            float _Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float;
            Unity_Lerp_float(float(3), float(0), _Property_65f904d6199d4350a89b9904628b7f25_Out_0_Float, _Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float);
            float4 _UV_f886bd3ce6e14698b48085645e8892a4_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2 = _UV_f886bd3ce6e14698b48085645e8892a4_Out_0_Vector4.xy;
            float _Property_a85bc01ad5de45428bea3152b546512d_Out_0_Float = _NoiseOffset;
            float _Property_b39de3d823764eb4b0936e62e6fe5074_Out_0_Float = _NoiseDensity;
            Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b;
            float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float;
            float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float;
            SG_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float(_Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2, _Property_a85bc01ad5de45428bea3152b546512d_Out_0_Float, _Property_b39de3d823764eb4b0936e62e6fe5074_Out_0_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float);
            float _Step_34f306488de6462093199997839c7cc3_Out_2_Float;
            Unity_Step_float(_Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float, _Step_34f306488de6462093199997839c7cc3_Out_2_Float);
            float _Property_7ea25f3e615d42c5b48c4467642bef17_Out_0_Float = _FissureProgress;
            float _Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float;
            Unity_Lerp_float(float(0.04), float(0.01), _Property_7ea25f3e615d42c5b48c4467642bef17_Out_0_Float, _Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float);
            float _Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float;
            Unity_Step_float(_Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float, _Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float);
            float _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float;
            Unity_OneMinus_float(_Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float, _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float);
            float _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float;
            Unity_Add_float(_Step_34f306488de6462093199997839c7cc3_Out_2_Float, _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float, _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float);
            float4 _Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_FissureColor) : _FissureColor;
            float4 _Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Add_1896dee97c9e464eb43461caea40c052_Out_2_Float.xxxx), _Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4, _Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4);
            float _Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float = _Edge;
            float4 _UV_9adc53fa249a4a5ab32d3a050acbfdaf_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_2029f5a6f61a4f00a6eb3e4c29b5b6af_Out_1_Vector2 = _UV_9adc53fa249a4a5ab32d3a050acbfdaf_Out_0_Vector4.xy;
            float _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float;
            Unity_Distance_float2(_Swizzle_2029f5a6f61a4f00a6eb3e4c29b5b6af_Out_1_Vector2, float2(0.5, 0.5), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float);
            float _Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float;
            Unity_Smoothstep_float(_Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float, float(1), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float, _Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float);
            float3 _Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3;
            Unity_Absolute_float3(IN.AbsoluteWorldSpacePosition, _Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3);
            Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7;
            float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Value_1_Float;
            float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float;
            SG_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float(_Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3, IN.TimeParameters.x, float(4.1), _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Value_1_Float, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float);
            float _Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float, _Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float);
            float _Property_4b01ac9dd6b047d2b9cf45fca7f0e366_Out_0_Float = _Offset;
            float _Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float;
            Unity_Add_float(_Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float, _Property_4b01ac9dd6b047d2b9cf45fca7f0e366_Out_0_Float, _Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float);
            float _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float;
            Unity_Smoothstep_float(_Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float, float(1), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float, _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float);
            float _Add_1f600a25927b426195eb7a73491937ac_Out_2_Float;
            Unity_Add_float(_Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float, _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float, _Add_1f600a25927b426195eb7a73491937ac_Out_2_Float);
            float4 _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4, (_Add_1f600a25927b426195eb7a73491937ac_Out_2_Float.xxxx), _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4);
            float4 _Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4, _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4, _Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4);
            float3 _EmissionNode_3e929fcc60694caa888171a6f04c27d9_Output_0_Vector3 = Unity_HDRP_GetEmissionHDRColor_float((_Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4.xyz).xyz, float(0.5), float(0.9));
            float _Property_43276f8cd0bb43c791ef6a5569938c47_Out_0_Float = _FissureProgress;
            float _Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float;
            Unity_Lerp_float(float(1), float(0.35), _Property_43276f8cd0bb43c791ef6a5569938c47_Out_0_Float, _Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float);
            float _Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float;
            Unity_Distance_float2(_Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2, float2(0.5, 0.5), _Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float);
            float _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float;
            Unity_OneMinus_float(_Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float, _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float);
            float _Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float;
            Unity_Step_float(_Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float, _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float, _Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float);
            float _Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float;
            Unity_Multiply_float_float(_Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float, _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float, _Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float);
            float4 _Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4, (_Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float.xxxx), _Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4);
            surface.BaseColor = (_Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4.xyz);
            surface.Emission = _EmissionNode_3e929fcc60694caa888171a6f04c27d9_Output_0_Vector3;
            surface.Alpha = (_Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4).x;
            surface.AlphaClipThreshold = float(0.5);
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
        
            #if defined(SHADER_STAGE_RAY_TRACING)
            #else
            #endif
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
            #pragma shader_feature_local _ _ALPHATEST_ON
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
        float4 _MainTex_TexelSize;
        float _FissureProgress;
        float _NoiseOffset;
        float _NoiseDensity;
        float4 _FissureColor;
        float4 _Color;
        float _Edge;
        float _Offset;
        float4 _EmissionColor;
        float _UseShadowThreshold;
        float4 _DoubleSidedConstants;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        float _BlendMode;
        float _EnableBlendModePreserveSpecularLighting;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
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
            #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi2D.hlsl"
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
             float3 AbsoluteWorldSpacePosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct PackedVaryingsMeshToPS
        {
            SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float3 positionRWS : INTERP1;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
        };
        
            PackedVaryingsMeshToPS PackVaryingsMeshToPS (VaryingsMeshToPS input)
        {
            PackedVaryingsMeshToPS output;
            ZERO_INITIALIZE(PackedVaryingsMeshToPS, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.positionRWS.xyz = input.positionRWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            return output;
        }
        
        VaryingsMeshToPS UnpackVaryingsMeshToPS (PackedVaryingsMeshToPS input)
        {
            VaryingsMeshToPS output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.positionRWS = input.positionRWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
        
            // Graph Functions
            
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
        struct Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float
        {
        };
        
        void SG_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float(float2 Vector2_DDA00F47, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float IN, out float Value_1, out float Cells_2)
        {
        float2 _Property_87196192b55192848c90533525880767_Out_0_Vector2 = Vector2_DDA00F47;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        VoronoiPrecise2D_float(_Property_87196192b55192848c90533525880767_Out_0_Vector2, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Step_float(float Edge, float In, out float Out)
        {
            Out = step(Edge, In);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        struct Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float
        {
        };
        
        void SG_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float(float3 Vector3_375394F7, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float IN, out float Value_1, out float Cells_2)
        {
        float3 _Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3 = Vector3_375394F7;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        Voronoi3D_float(_Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
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
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_683e3b8c31014b2397e10acee02ca4a2_Out_0_Vector4 = _Color;
            UnityTexture2D _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.tex, _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.samplerstate, _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_R_4_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.r;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_G_5_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.g;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_B_6_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.b;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_A_7_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.a;
            float4 _Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_683e3b8c31014b2397e10acee02ca4a2_Out_0_Vector4, _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4, _Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4);
            float _Property_65f904d6199d4350a89b9904628b7f25_Out_0_Float = _FissureProgress;
            float _Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float;
            Unity_Lerp_float(float(3), float(0), _Property_65f904d6199d4350a89b9904628b7f25_Out_0_Float, _Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float);
            float4 _UV_f886bd3ce6e14698b48085645e8892a4_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2 = _UV_f886bd3ce6e14698b48085645e8892a4_Out_0_Vector4.xy;
            float _Property_a85bc01ad5de45428bea3152b546512d_Out_0_Float = _NoiseOffset;
            float _Property_b39de3d823764eb4b0936e62e6fe5074_Out_0_Float = _NoiseDensity;
            Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b;
            float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float;
            float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float;
            SG_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float(_Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2, _Property_a85bc01ad5de45428bea3152b546512d_Out_0_Float, _Property_b39de3d823764eb4b0936e62e6fe5074_Out_0_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float);
            float _Step_34f306488de6462093199997839c7cc3_Out_2_Float;
            Unity_Step_float(_Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float, _Step_34f306488de6462093199997839c7cc3_Out_2_Float);
            float _Property_7ea25f3e615d42c5b48c4467642bef17_Out_0_Float = _FissureProgress;
            float _Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float;
            Unity_Lerp_float(float(0.04), float(0.01), _Property_7ea25f3e615d42c5b48c4467642bef17_Out_0_Float, _Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float);
            float _Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float;
            Unity_Step_float(_Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float, _Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float);
            float _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float;
            Unity_OneMinus_float(_Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float, _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float);
            float _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float;
            Unity_Add_float(_Step_34f306488de6462093199997839c7cc3_Out_2_Float, _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float, _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float);
            float4 _Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_FissureColor) : _FissureColor;
            float4 _Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Add_1896dee97c9e464eb43461caea40c052_Out_2_Float.xxxx), _Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4, _Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4);
            float _Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float = _Edge;
            float4 _UV_9adc53fa249a4a5ab32d3a050acbfdaf_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_2029f5a6f61a4f00a6eb3e4c29b5b6af_Out_1_Vector2 = _UV_9adc53fa249a4a5ab32d3a050acbfdaf_Out_0_Vector4.xy;
            float _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float;
            Unity_Distance_float2(_Swizzle_2029f5a6f61a4f00a6eb3e4c29b5b6af_Out_1_Vector2, float2(0.5, 0.5), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float);
            float _Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float;
            Unity_Smoothstep_float(_Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float, float(1), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float, _Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float);
            float3 _Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3;
            Unity_Absolute_float3(IN.AbsoluteWorldSpacePosition, _Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3);
            Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7;
            float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Value_1_Float;
            float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float;
            SG_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float(_Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3, IN.TimeParameters.x, float(4.1), _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Value_1_Float, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float);
            float _Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float, _Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float);
            float _Property_4b01ac9dd6b047d2b9cf45fca7f0e366_Out_0_Float = _Offset;
            float _Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float;
            Unity_Add_float(_Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float, _Property_4b01ac9dd6b047d2b9cf45fca7f0e366_Out_0_Float, _Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float);
            float _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float;
            Unity_Smoothstep_float(_Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float, float(1), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float, _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float);
            float _Add_1f600a25927b426195eb7a73491937ac_Out_2_Float;
            Unity_Add_float(_Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float, _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float, _Add_1f600a25927b426195eb7a73491937ac_Out_2_Float);
            float4 _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4, (_Add_1f600a25927b426195eb7a73491937ac_Out_2_Float.xxxx), _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4);
            float4 _Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4, _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4, _Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4);
            float3 _EmissionNode_3e929fcc60694caa888171a6f04c27d9_Output_0_Vector3 = Unity_HDRP_GetEmissionHDRColor_float((_Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4.xyz).xyz, float(0.5), float(0.9));
            float _Property_43276f8cd0bb43c791ef6a5569938c47_Out_0_Float = _FissureProgress;
            float _Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float;
            Unity_Lerp_float(float(1), float(0.35), _Property_43276f8cd0bb43c791ef6a5569938c47_Out_0_Float, _Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float);
            float _Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float;
            Unity_Distance_float2(_Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2, float2(0.5, 0.5), _Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float);
            float _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float;
            Unity_OneMinus_float(_Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float, _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float);
            float _Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float;
            Unity_Step_float(_Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float, _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float, _Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float);
            float _Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float;
            Unity_Multiply_float_float(_Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float, _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float, _Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float);
            float4 _Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4, (_Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float.xxxx), _Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4);
            surface.BaseColor = (_Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4.xyz);
            surface.Emission = _EmissionNode_3e929fcc60694caa888171a6f04c27d9_Output_0_Vector3;
            surface.Alpha = (_Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4).x;
            surface.AlphaClipThreshold = float(0.5);
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
        
            #if defined(SHADER_STAGE_RAY_TRACING)
            #else
            #endif
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
        AlphaToMask [_AlphaCutoffEnable]
        
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
            #pragma shader_feature_local _ _ALPHATEST_ON
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
        float4 _MainTex_TexelSize;
        float _FissureProgress;
        float _NoiseOffset;
        float _NoiseDensity;
        float4 _FissureColor;
        float4 _Color;
        float _Edge;
        float _Offset;
        float4 _EmissionColor;
        float _UseShadowThreshold;
        float4 _DoubleSidedConstants;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        float _BlendMode;
        float _EnableBlendModePreserveSpecularLighting;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
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
            #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi2D.hlsl"
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
            
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
        struct Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float
        {
        };
        
        void SG_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float(float2 Vector2_DDA00F47, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float IN, out float Value_1, out float Cells_2)
        {
        float2 _Property_87196192b55192848c90533525880767_Out_0_Vector2 = Vector2_DDA00F47;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        VoronoiPrecise2D_float(_Property_87196192b55192848c90533525880767_Out_0_Vector2, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Step_float(float Edge, float In, out float Out)
        {
            Out = step(Edge, In);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        struct Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float
        {
        };
        
        void SG_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float(float3 Vector3_375394F7, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float IN, out float Value_1, out float Cells_2)
        {
        float3 _Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3 = Vector3_375394F7;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        Voronoi3D_float(_Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
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
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_683e3b8c31014b2397e10acee02ca4a2_Out_0_Vector4 = _Color;
            UnityTexture2D _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.tex, _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.samplerstate, _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_R_4_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.r;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_G_5_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.g;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_B_6_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.b;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_A_7_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.a;
            float4 _Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_683e3b8c31014b2397e10acee02ca4a2_Out_0_Vector4, _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4, _Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4);
            float _Property_65f904d6199d4350a89b9904628b7f25_Out_0_Float = _FissureProgress;
            float _Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float;
            Unity_Lerp_float(float(3), float(0), _Property_65f904d6199d4350a89b9904628b7f25_Out_0_Float, _Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float);
            float4 _UV_f886bd3ce6e14698b48085645e8892a4_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2 = _UV_f886bd3ce6e14698b48085645e8892a4_Out_0_Vector4.xy;
            float _Property_a85bc01ad5de45428bea3152b546512d_Out_0_Float = _NoiseOffset;
            float _Property_b39de3d823764eb4b0936e62e6fe5074_Out_0_Float = _NoiseDensity;
            Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b;
            float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float;
            float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float;
            SG_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float(_Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2, _Property_a85bc01ad5de45428bea3152b546512d_Out_0_Float, _Property_b39de3d823764eb4b0936e62e6fe5074_Out_0_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float);
            float _Step_34f306488de6462093199997839c7cc3_Out_2_Float;
            Unity_Step_float(_Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float, _Step_34f306488de6462093199997839c7cc3_Out_2_Float);
            float _Property_7ea25f3e615d42c5b48c4467642bef17_Out_0_Float = _FissureProgress;
            float _Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float;
            Unity_Lerp_float(float(0.04), float(0.01), _Property_7ea25f3e615d42c5b48c4467642bef17_Out_0_Float, _Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float);
            float _Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float;
            Unity_Step_float(_Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float, _Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float);
            float _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float;
            Unity_OneMinus_float(_Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float, _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float);
            float _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float;
            Unity_Add_float(_Step_34f306488de6462093199997839c7cc3_Out_2_Float, _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float, _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float);
            float4 _Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_FissureColor) : _FissureColor;
            float4 _Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Add_1896dee97c9e464eb43461caea40c052_Out_2_Float.xxxx), _Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4, _Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4);
            float _Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float = _Edge;
            float4 _UV_9adc53fa249a4a5ab32d3a050acbfdaf_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_2029f5a6f61a4f00a6eb3e4c29b5b6af_Out_1_Vector2 = _UV_9adc53fa249a4a5ab32d3a050acbfdaf_Out_0_Vector4.xy;
            float _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float;
            Unity_Distance_float2(_Swizzle_2029f5a6f61a4f00a6eb3e4c29b5b6af_Out_1_Vector2, float2(0.5, 0.5), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float);
            float _Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float;
            Unity_Smoothstep_float(_Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float, float(1), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float, _Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float);
            float3 _Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3;
            Unity_Absolute_float3(IN.AbsoluteWorldSpacePosition, _Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3);
            Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7;
            float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Value_1_Float;
            float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float;
            SG_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float(_Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3, IN.TimeParameters.x, float(4.1), _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Value_1_Float, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float);
            float _Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float, _Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float);
            float _Property_4b01ac9dd6b047d2b9cf45fca7f0e366_Out_0_Float = _Offset;
            float _Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float;
            Unity_Add_float(_Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float, _Property_4b01ac9dd6b047d2b9cf45fca7f0e366_Out_0_Float, _Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float);
            float _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float;
            Unity_Smoothstep_float(_Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float, float(1), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float, _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float);
            float _Add_1f600a25927b426195eb7a73491937ac_Out_2_Float;
            Unity_Add_float(_Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float, _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float, _Add_1f600a25927b426195eb7a73491937ac_Out_2_Float);
            float4 _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4, (_Add_1f600a25927b426195eb7a73491937ac_Out_2_Float.xxxx), _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4);
            float4 _Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4, _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4, _Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4);
            float3 _EmissionNode_3e929fcc60694caa888171a6f04c27d9_Output_0_Vector3 = Unity_HDRP_GetEmissionHDRColor_float((_Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4.xyz).xyz, float(0.5), float(0.9));
            float _Property_43276f8cd0bb43c791ef6a5569938c47_Out_0_Float = _FissureProgress;
            float _Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float;
            Unity_Lerp_float(float(1), float(0.35), _Property_43276f8cd0bb43c791ef6a5569938c47_Out_0_Float, _Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float);
            float _Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float;
            Unity_Distance_float2(_Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2, float2(0.5, 0.5), _Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float);
            float _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float;
            Unity_OneMinus_float(_Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float, _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float);
            float _Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float;
            Unity_Step_float(_Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float, _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float, _Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float);
            float _Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float;
            Unity_Multiply_float_float(_Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float, _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float, _Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float);
            float4 _Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4, (_Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float.xxxx), _Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4);
            surface.BaseColor = (_Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4.xyz);
            surface.Emission = _EmissionNode_3e929fcc60694caa888171a6f04c27d9_Output_0_Vector3;
            surface.Alpha = (_Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4).x;
            surface.AlphaClipThreshold = float(0.5);
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
        
            #if defined(SHADER_STAGE_RAY_TRACING)
            #else
            #endif
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
        ReadMask 128
        WriteMask 128
        Ref 128
        CompFront Always
        PassFront Replace
        CompBack Always
        PassBack Replace
        Fail Keep
        ZFail Keep
        }
        AlphaToMask [_AlphaCutoffEnable]
        
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
            #pragma shader_feature_local _ _ALPHATEST_ON
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
        float4 _MainTex_TexelSize;
        float _FissureProgress;
        float _NoiseOffset;
        float _NoiseDensity;
        float4 _FissureColor;
        float4 _Color;
        float _Edge;
        float _Offset;
        float4 _EmissionColor;
        float _UseShadowThreshold;
        float4 _DoubleSidedConstants;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        float _BlendMode;
        float _EnableBlendModePreserveSpecularLighting;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
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
            #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi2D.hlsl"
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
            
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
        struct Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float
        {
        };
        
        void SG_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float(float2 Vector2_DDA00F47, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float IN, out float Value_1, out float Cells_2)
        {
        float2 _Property_87196192b55192848c90533525880767_Out_0_Vector2 = Vector2_DDA00F47;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        VoronoiPrecise2D_float(_Property_87196192b55192848c90533525880767_Out_0_Vector2, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Step_float(float Edge, float In, out float Out)
        {
            Out = step(Edge, In);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        struct Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float
        {
        };
        
        void SG_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float(float3 Vector3_375394F7, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float IN, out float Value_1, out float Cells_2)
        {
        float3 _Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3 = Vector3_375394F7;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        Voronoi3D_float(_Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
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
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_683e3b8c31014b2397e10acee02ca4a2_Out_0_Vector4 = _Color;
            UnityTexture2D _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.tex, _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.samplerstate, _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_R_4_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.r;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_G_5_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.g;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_B_6_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.b;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_A_7_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.a;
            float4 _Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_683e3b8c31014b2397e10acee02ca4a2_Out_0_Vector4, _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4, _Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4);
            float _Property_65f904d6199d4350a89b9904628b7f25_Out_0_Float = _FissureProgress;
            float _Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float;
            Unity_Lerp_float(float(3), float(0), _Property_65f904d6199d4350a89b9904628b7f25_Out_0_Float, _Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float);
            float4 _UV_f886bd3ce6e14698b48085645e8892a4_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2 = _UV_f886bd3ce6e14698b48085645e8892a4_Out_0_Vector4.xy;
            float _Property_a85bc01ad5de45428bea3152b546512d_Out_0_Float = _NoiseOffset;
            float _Property_b39de3d823764eb4b0936e62e6fe5074_Out_0_Float = _NoiseDensity;
            Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b;
            float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float;
            float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float;
            SG_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float(_Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2, _Property_a85bc01ad5de45428bea3152b546512d_Out_0_Float, _Property_b39de3d823764eb4b0936e62e6fe5074_Out_0_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float);
            float _Step_34f306488de6462093199997839c7cc3_Out_2_Float;
            Unity_Step_float(_Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float, _Step_34f306488de6462093199997839c7cc3_Out_2_Float);
            float _Property_7ea25f3e615d42c5b48c4467642bef17_Out_0_Float = _FissureProgress;
            float _Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float;
            Unity_Lerp_float(float(0.04), float(0.01), _Property_7ea25f3e615d42c5b48c4467642bef17_Out_0_Float, _Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float);
            float _Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float;
            Unity_Step_float(_Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float, _Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float);
            float _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float;
            Unity_OneMinus_float(_Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float, _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float);
            float _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float;
            Unity_Add_float(_Step_34f306488de6462093199997839c7cc3_Out_2_Float, _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float, _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float);
            float4 _Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_FissureColor) : _FissureColor;
            float4 _Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Add_1896dee97c9e464eb43461caea40c052_Out_2_Float.xxxx), _Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4, _Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4);
            float _Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float = _Edge;
            float4 _UV_9adc53fa249a4a5ab32d3a050acbfdaf_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_2029f5a6f61a4f00a6eb3e4c29b5b6af_Out_1_Vector2 = _UV_9adc53fa249a4a5ab32d3a050acbfdaf_Out_0_Vector4.xy;
            float _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float;
            Unity_Distance_float2(_Swizzle_2029f5a6f61a4f00a6eb3e4c29b5b6af_Out_1_Vector2, float2(0.5, 0.5), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float);
            float _Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float;
            Unity_Smoothstep_float(_Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float, float(1), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float, _Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float);
            float3 _Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3;
            Unity_Absolute_float3(IN.AbsoluteWorldSpacePosition, _Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3);
            Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7;
            float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Value_1_Float;
            float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float;
            SG_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float(_Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3, IN.TimeParameters.x, float(4.1), _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Value_1_Float, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float);
            float _Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float, _Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float);
            float _Property_4b01ac9dd6b047d2b9cf45fca7f0e366_Out_0_Float = _Offset;
            float _Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float;
            Unity_Add_float(_Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float, _Property_4b01ac9dd6b047d2b9cf45fca7f0e366_Out_0_Float, _Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float);
            float _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float;
            Unity_Smoothstep_float(_Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float, float(1), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float, _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float);
            float _Add_1f600a25927b426195eb7a73491937ac_Out_2_Float;
            Unity_Add_float(_Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float, _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float, _Add_1f600a25927b426195eb7a73491937ac_Out_2_Float);
            float4 _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4, (_Add_1f600a25927b426195eb7a73491937ac_Out_2_Float.xxxx), _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4);
            float4 _Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4, _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4, _Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4);
            float3 _EmissionNode_3e929fcc60694caa888171a6f04c27d9_Output_0_Vector3 = Unity_HDRP_GetEmissionHDRColor_float((_Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4.xyz).xyz, float(0.5), float(0.9));
            float _Property_43276f8cd0bb43c791ef6a5569938c47_Out_0_Float = _FissureProgress;
            float _Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float;
            Unity_Lerp_float(float(1), float(0.35), _Property_43276f8cd0bb43c791ef6a5569938c47_Out_0_Float, _Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float);
            float _Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float;
            Unity_Distance_float2(_Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2, float2(0.5, 0.5), _Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float);
            float _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float;
            Unity_OneMinus_float(_Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float, _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float);
            float _Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float;
            Unity_Step_float(_Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float, _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float, _Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float);
            float _Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float;
            Unity_Multiply_float_float(_Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float, _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float, _Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float);
            float4 _Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4, (_Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float.xxxx), _Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4);
            surface.BaseColor = (_Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4.xyz);
            surface.Emission = _EmissionNode_3e929fcc60694caa888171a6f04c27d9_Output_0_Vector3;
            surface.Alpha = (_Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4).x;
            surface.AlphaClipThreshold = float(0.5);
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
        
            #if defined(SHADER_STAGE_RAY_TRACING)
            #else
            #endif
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
        WriteMask 128
        Ref 128
        CompFront Always
        PassFront Replace
        CompBack Always
        PassBack Replace
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
            #pragma shader_feature_local _ _ALPHATEST_ON
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
        float4 _MainTex_TexelSize;
        float _FissureProgress;
        float _NoiseOffset;
        float _NoiseDensity;
        float4 _FissureColor;
        float4 _Color;
        float _Edge;
        float _Offset;
        float4 _EmissionColor;
        float _UseShadowThreshold;
        float4 _DoubleSidedConstants;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        float _BlendMode;
        float _EnableBlendModePreserveSpecularLighting;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
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
            #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi2D.hlsl"
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
             float3 AbsoluteWorldSpacePosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct PackedVaryingsMeshToPS
        {
            SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float3 positionRWS : INTERP1;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
        };
        
            PackedVaryingsMeshToPS PackVaryingsMeshToPS (VaryingsMeshToPS input)
        {
            PackedVaryingsMeshToPS output;
            ZERO_INITIALIZE(PackedVaryingsMeshToPS, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.positionRWS.xyz = input.positionRWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            return output;
        }
        
        VaryingsMeshToPS UnpackVaryingsMeshToPS (PackedVaryingsMeshToPS input)
        {
            VaryingsMeshToPS output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.positionRWS = input.positionRWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
        
            // Graph Functions
            
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
        struct Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float
        {
        };
        
        void SG_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float(float2 Vector2_DDA00F47, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float IN, out float Value_1, out float Cells_2)
        {
        float2 _Property_87196192b55192848c90533525880767_Out_0_Vector2 = Vector2_DDA00F47;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        VoronoiPrecise2D_float(_Property_87196192b55192848c90533525880767_Out_0_Vector2, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Step_float(float Edge, float In, out float Out)
        {
            Out = step(Edge, In);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        struct Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float
        {
        };
        
        void SG_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float(float3 Vector3_375394F7, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float IN, out float Value_1, out float Cells_2)
        {
        float3 _Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3 = Vector3_375394F7;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        Voronoi3D_float(_Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
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
            float AlphaClipThreshold;
            float4 VTPackedFeedback;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_683e3b8c31014b2397e10acee02ca4a2_Out_0_Vector4 = _Color;
            UnityTexture2D _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.tex, _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.samplerstate, _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_R_4_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.r;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_G_5_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.g;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_B_6_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.b;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_A_7_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.a;
            float4 _Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_683e3b8c31014b2397e10acee02ca4a2_Out_0_Vector4, _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4, _Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4);
            float _Property_65f904d6199d4350a89b9904628b7f25_Out_0_Float = _FissureProgress;
            float _Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float;
            Unity_Lerp_float(float(3), float(0), _Property_65f904d6199d4350a89b9904628b7f25_Out_0_Float, _Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float);
            float4 _UV_f886bd3ce6e14698b48085645e8892a4_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2 = _UV_f886bd3ce6e14698b48085645e8892a4_Out_0_Vector4.xy;
            float _Property_a85bc01ad5de45428bea3152b546512d_Out_0_Float = _NoiseOffset;
            float _Property_b39de3d823764eb4b0936e62e6fe5074_Out_0_Float = _NoiseDensity;
            Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b;
            float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float;
            float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float;
            SG_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float(_Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2, _Property_a85bc01ad5de45428bea3152b546512d_Out_0_Float, _Property_b39de3d823764eb4b0936e62e6fe5074_Out_0_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float);
            float _Step_34f306488de6462093199997839c7cc3_Out_2_Float;
            Unity_Step_float(_Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float, _Step_34f306488de6462093199997839c7cc3_Out_2_Float);
            float _Property_7ea25f3e615d42c5b48c4467642bef17_Out_0_Float = _FissureProgress;
            float _Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float;
            Unity_Lerp_float(float(0.04), float(0.01), _Property_7ea25f3e615d42c5b48c4467642bef17_Out_0_Float, _Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float);
            float _Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float;
            Unity_Step_float(_Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float, _Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float);
            float _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float;
            Unity_OneMinus_float(_Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float, _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float);
            float _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float;
            Unity_Add_float(_Step_34f306488de6462093199997839c7cc3_Out_2_Float, _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float, _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float);
            float4 _Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_FissureColor) : _FissureColor;
            float4 _Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Add_1896dee97c9e464eb43461caea40c052_Out_2_Float.xxxx), _Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4, _Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4);
            float _Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float = _Edge;
            float4 _UV_9adc53fa249a4a5ab32d3a050acbfdaf_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_2029f5a6f61a4f00a6eb3e4c29b5b6af_Out_1_Vector2 = _UV_9adc53fa249a4a5ab32d3a050acbfdaf_Out_0_Vector4.xy;
            float _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float;
            Unity_Distance_float2(_Swizzle_2029f5a6f61a4f00a6eb3e4c29b5b6af_Out_1_Vector2, float2(0.5, 0.5), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float);
            float _Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float;
            Unity_Smoothstep_float(_Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float, float(1), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float, _Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float);
            float3 _Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3;
            Unity_Absolute_float3(IN.AbsoluteWorldSpacePosition, _Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3);
            Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7;
            float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Value_1_Float;
            float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float;
            SG_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float(_Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3, IN.TimeParameters.x, float(4.1), _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Value_1_Float, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float);
            float _Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float, _Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float);
            float _Property_4b01ac9dd6b047d2b9cf45fca7f0e366_Out_0_Float = _Offset;
            float _Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float;
            Unity_Add_float(_Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float, _Property_4b01ac9dd6b047d2b9cf45fca7f0e366_Out_0_Float, _Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float);
            float _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float;
            Unity_Smoothstep_float(_Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float, float(1), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float, _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float);
            float _Add_1f600a25927b426195eb7a73491937ac_Out_2_Float;
            Unity_Add_float(_Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float, _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float, _Add_1f600a25927b426195eb7a73491937ac_Out_2_Float);
            float4 _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4, (_Add_1f600a25927b426195eb7a73491937ac_Out_2_Float.xxxx), _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4);
            float4 _Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4, _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4, _Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4);
            float3 _EmissionNode_3e929fcc60694caa888171a6f04c27d9_Output_0_Vector3 = Unity_HDRP_GetEmissionHDRColor_float((_Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4.xyz).xyz, float(0.5), float(0.9));
            float _Property_43276f8cd0bb43c791ef6a5569938c47_Out_0_Float = _FissureProgress;
            float _Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float;
            Unity_Lerp_float(float(1), float(0.35), _Property_43276f8cd0bb43c791ef6a5569938c47_Out_0_Float, _Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float);
            float _Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float;
            Unity_Distance_float2(_Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2, float2(0.5, 0.5), _Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float);
            float _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float;
            Unity_OneMinus_float(_Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float, _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float);
            float _Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float;
            Unity_Step_float(_Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float, _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float, _Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float);
            float _Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float;
            Unity_Multiply_float_float(_Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float, _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float, _Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float);
            float4 _Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4, (_Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float.xxxx), _Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4);
            surface.BaseColor = (_Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4.xyz);
            surface.Emission = _EmissionNode_3e929fcc60694caa888171a6f04c27d9_Output_0_Vector3;
            surface.Alpha = (_Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4).x;
            surface.AlphaClipThreshold = float(0.5);
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
        
            #if defined(SHADER_STAGE_RAY_TRACING)
            #else
            #endif
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
            #pragma shader_feature_local _ _ALPHATEST_ON
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
        float4 _MainTex_TexelSize;
        float _FissureProgress;
        float _NoiseOffset;
        float _NoiseDensity;
        float4 _FissureColor;
        float4 _Color;
        float _Edge;
        float _Offset;
        float4 _EmissionColor;
        float _UseShadowThreshold;
        float4 _DoubleSidedConstants;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        float _BlendMode;
        float _EnableBlendModePreserveSpecularLighting;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
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
            #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi2D.hlsl"
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
             float3 AbsoluteWorldSpacePosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct PackedVaryingsMeshToPS
        {
            SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float3 positionRWS : INTERP1;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
        };
        
            PackedVaryingsMeshToPS PackVaryingsMeshToPS (VaryingsMeshToPS input)
        {
            PackedVaryingsMeshToPS output;
            ZERO_INITIALIZE(PackedVaryingsMeshToPS, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.positionRWS.xyz = input.positionRWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            return output;
        }
        
        VaryingsMeshToPS UnpackVaryingsMeshToPS (PackedVaryingsMeshToPS input)
        {
            VaryingsMeshToPS output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.positionRWS = input.positionRWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
        
            // Graph Functions
            
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
        struct Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float
        {
        };
        
        void SG_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float(float2 Vector2_DDA00F47, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float IN, out float Value_1, out float Cells_2)
        {
        float2 _Property_87196192b55192848c90533525880767_Out_0_Vector2 = Vector2_DDA00F47;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        VoronoiPrecise2D_float(_Property_87196192b55192848c90533525880767_Out_0_Vector2, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Step_float(float Edge, float In, out float Out)
        {
            Out = step(Edge, In);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        struct Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float
        {
        };
        
        void SG_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float(float3 Vector3_375394F7, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float IN, out float Value_1, out float Cells_2)
        {
        float3 _Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3 = Vector3_375394F7;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        Voronoi3D_float(_Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
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
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_683e3b8c31014b2397e10acee02ca4a2_Out_0_Vector4 = _Color;
            UnityTexture2D _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.tex, _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.samplerstate, _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_R_4_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.r;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_G_5_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.g;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_B_6_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.b;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_A_7_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.a;
            float4 _Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_683e3b8c31014b2397e10acee02ca4a2_Out_0_Vector4, _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4, _Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4);
            float _Property_65f904d6199d4350a89b9904628b7f25_Out_0_Float = _FissureProgress;
            float _Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float;
            Unity_Lerp_float(float(3), float(0), _Property_65f904d6199d4350a89b9904628b7f25_Out_0_Float, _Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float);
            float4 _UV_f886bd3ce6e14698b48085645e8892a4_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2 = _UV_f886bd3ce6e14698b48085645e8892a4_Out_0_Vector4.xy;
            float _Property_a85bc01ad5de45428bea3152b546512d_Out_0_Float = _NoiseOffset;
            float _Property_b39de3d823764eb4b0936e62e6fe5074_Out_0_Float = _NoiseDensity;
            Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b;
            float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float;
            float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float;
            SG_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float(_Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2, _Property_a85bc01ad5de45428bea3152b546512d_Out_0_Float, _Property_b39de3d823764eb4b0936e62e6fe5074_Out_0_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float);
            float _Step_34f306488de6462093199997839c7cc3_Out_2_Float;
            Unity_Step_float(_Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float, _Step_34f306488de6462093199997839c7cc3_Out_2_Float);
            float _Property_7ea25f3e615d42c5b48c4467642bef17_Out_0_Float = _FissureProgress;
            float _Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float;
            Unity_Lerp_float(float(0.04), float(0.01), _Property_7ea25f3e615d42c5b48c4467642bef17_Out_0_Float, _Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float);
            float _Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float;
            Unity_Step_float(_Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float, _Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float);
            float _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float;
            Unity_OneMinus_float(_Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float, _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float);
            float _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float;
            Unity_Add_float(_Step_34f306488de6462093199997839c7cc3_Out_2_Float, _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float, _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float);
            float4 _Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_FissureColor) : _FissureColor;
            float4 _Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Add_1896dee97c9e464eb43461caea40c052_Out_2_Float.xxxx), _Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4, _Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4);
            float _Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float = _Edge;
            float4 _UV_9adc53fa249a4a5ab32d3a050acbfdaf_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_2029f5a6f61a4f00a6eb3e4c29b5b6af_Out_1_Vector2 = _UV_9adc53fa249a4a5ab32d3a050acbfdaf_Out_0_Vector4.xy;
            float _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float;
            Unity_Distance_float2(_Swizzle_2029f5a6f61a4f00a6eb3e4c29b5b6af_Out_1_Vector2, float2(0.5, 0.5), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float);
            float _Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float;
            Unity_Smoothstep_float(_Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float, float(1), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float, _Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float);
            float3 _Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3;
            Unity_Absolute_float3(IN.AbsoluteWorldSpacePosition, _Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3);
            Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7;
            float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Value_1_Float;
            float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float;
            SG_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float(_Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3, IN.TimeParameters.x, float(4.1), _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Value_1_Float, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float);
            float _Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float, _Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float);
            float _Property_4b01ac9dd6b047d2b9cf45fca7f0e366_Out_0_Float = _Offset;
            float _Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float;
            Unity_Add_float(_Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float, _Property_4b01ac9dd6b047d2b9cf45fca7f0e366_Out_0_Float, _Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float);
            float _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float;
            Unity_Smoothstep_float(_Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float, float(1), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float, _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float);
            float _Add_1f600a25927b426195eb7a73491937ac_Out_2_Float;
            Unity_Add_float(_Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float, _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float, _Add_1f600a25927b426195eb7a73491937ac_Out_2_Float);
            float4 _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4, (_Add_1f600a25927b426195eb7a73491937ac_Out_2_Float.xxxx), _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4);
            float4 _Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4, _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4, _Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4);
            float3 _EmissionNode_3e929fcc60694caa888171a6f04c27d9_Output_0_Vector3 = Unity_HDRP_GetEmissionHDRColor_float((_Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4.xyz).xyz, float(0.5), float(0.9));
            float _Property_43276f8cd0bb43c791ef6a5569938c47_Out_0_Float = _FissureProgress;
            float _Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float;
            Unity_Lerp_float(float(1), float(0.35), _Property_43276f8cd0bb43c791ef6a5569938c47_Out_0_Float, _Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float);
            float _Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float;
            Unity_Distance_float2(_Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2, float2(0.5, 0.5), _Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float);
            float _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float;
            Unity_OneMinus_float(_Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float, _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float);
            float _Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float;
            Unity_Step_float(_Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float, _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float, _Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float);
            float _Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float;
            Unity_Multiply_float_float(_Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float, _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float, _Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float);
            float4 _Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4, (_Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float.xxxx), _Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4);
            surface.BaseColor = (_Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4.xyz);
            surface.Emission = _EmissionNode_3e929fcc60694caa888171a6f04c27d9_Output_0_Vector3;
            surface.Alpha = (_Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4).x;
            surface.AlphaClipThreshold = float(0.5);
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
        
            #if defined(SHADER_STAGE_RAY_TRACING)
            #else
            #endif
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
            "Queue"="AlphaTest+0"
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
            #pragma shader_feature_local _ _ALPHATEST_ON
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
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_POSITION_WS
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
        float4 _MainTex_TexelSize;
        float _FissureProgress;
        float _NoiseOffset;
        float _NoiseDensity;
        float4 _FissureColor;
        float4 _Color;
        float _Edge;
        float _Offset;
        float4 _EmissionColor;
        float _UseShadowThreshold;
        float4 _DoubleSidedConstants;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        float _BlendMode;
        float _EnableBlendModePreserveSpecularLighting;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
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
            #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi2D.hlsl"
        #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi3D.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonLighting.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            struct SurfaceDescriptionInputs
        {
             float3 AbsoluteWorldSpacePosition;
             float4 uv0;
             float3 TimeParameters;
        };
        
            //Interpolator Packs: <None>
        
            // --------------------------------------------------
            // Graph
        
        
            // Graph Functions
            
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
        struct Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float
        {
        };
        
        void SG_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float(float2 Vector2_DDA00F47, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float IN, out float Value_1, out float Cells_2)
        {
        float2 _Property_87196192b55192848c90533525880767_Out_0_Vector2 = Vector2_DDA00F47;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        VoronoiPrecise2D_float(_Property_87196192b55192848c90533525880767_Out_0_Vector2, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Step_float(float Edge, float In, out float Out)
        {
            Out = step(Edge, In);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        struct Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float
        {
        };
        
        void SG_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float(float3 Vector3_375394F7, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float IN, out float Value_1, out float Cells_2)
        {
        float3 _Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3 = Vector3_375394F7;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        Voronoi3D_float(_Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
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
        
            // Graph Vertex
            // GraphVertex: <None>
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float3 Emission;
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_683e3b8c31014b2397e10acee02ca4a2_Out_0_Vector4 = _Color;
            UnityTexture2D _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.tex, _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.samplerstate, _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_R_4_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.r;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_G_5_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.g;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_B_6_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.b;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_A_7_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.a;
            float4 _Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_683e3b8c31014b2397e10acee02ca4a2_Out_0_Vector4, _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4, _Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4);
            float _Property_65f904d6199d4350a89b9904628b7f25_Out_0_Float = _FissureProgress;
            float _Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float;
            Unity_Lerp_float(float(3), float(0), _Property_65f904d6199d4350a89b9904628b7f25_Out_0_Float, _Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float);
            float4 _UV_f886bd3ce6e14698b48085645e8892a4_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2 = _UV_f886bd3ce6e14698b48085645e8892a4_Out_0_Vector4.xy;
            float _Property_a85bc01ad5de45428bea3152b546512d_Out_0_Float = _NoiseOffset;
            float _Property_b39de3d823764eb4b0936e62e6fe5074_Out_0_Float = _NoiseDensity;
            Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b;
            float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float;
            float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float;
            SG_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float(_Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2, _Property_a85bc01ad5de45428bea3152b546512d_Out_0_Float, _Property_b39de3d823764eb4b0936e62e6fe5074_Out_0_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float);
            float _Step_34f306488de6462093199997839c7cc3_Out_2_Float;
            Unity_Step_float(_Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float, _Step_34f306488de6462093199997839c7cc3_Out_2_Float);
            float _Property_7ea25f3e615d42c5b48c4467642bef17_Out_0_Float = _FissureProgress;
            float _Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float;
            Unity_Lerp_float(float(0.04), float(0.01), _Property_7ea25f3e615d42c5b48c4467642bef17_Out_0_Float, _Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float);
            float _Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float;
            Unity_Step_float(_Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float, _Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float);
            float _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float;
            Unity_OneMinus_float(_Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float, _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float);
            float _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float;
            Unity_Add_float(_Step_34f306488de6462093199997839c7cc3_Out_2_Float, _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float, _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float);
            float4 _Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_FissureColor) : _FissureColor;
            float4 _Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Add_1896dee97c9e464eb43461caea40c052_Out_2_Float.xxxx), _Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4, _Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4);
            float _Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float = _Edge;
            float4 _UV_9adc53fa249a4a5ab32d3a050acbfdaf_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_2029f5a6f61a4f00a6eb3e4c29b5b6af_Out_1_Vector2 = _UV_9adc53fa249a4a5ab32d3a050acbfdaf_Out_0_Vector4.xy;
            float _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float;
            Unity_Distance_float2(_Swizzle_2029f5a6f61a4f00a6eb3e4c29b5b6af_Out_1_Vector2, float2(0.5, 0.5), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float);
            float _Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float;
            Unity_Smoothstep_float(_Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float, float(1), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float, _Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float);
            float3 _Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3;
            Unity_Absolute_float3(IN.AbsoluteWorldSpacePosition, _Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3);
            Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7;
            float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Value_1_Float;
            float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float;
            SG_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float(_Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3, IN.TimeParameters.x, float(4.1), _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Value_1_Float, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float);
            float _Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float, _Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float);
            float _Property_4b01ac9dd6b047d2b9cf45fca7f0e366_Out_0_Float = _Offset;
            float _Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float;
            Unity_Add_float(_Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float, _Property_4b01ac9dd6b047d2b9cf45fca7f0e366_Out_0_Float, _Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float);
            float _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float;
            Unity_Smoothstep_float(_Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float, float(1), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float, _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float);
            float _Add_1f600a25927b426195eb7a73491937ac_Out_2_Float;
            Unity_Add_float(_Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float, _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float, _Add_1f600a25927b426195eb7a73491937ac_Out_2_Float);
            float4 _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4, (_Add_1f600a25927b426195eb7a73491937ac_Out_2_Float.xxxx), _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4);
            float4 _Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4, _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4, _Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4);
            float3 _EmissionNode_3e929fcc60694caa888171a6f04c27d9_Output_0_Vector3 = Unity_HDRP_GetEmissionHDRColor_float((_Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4.xyz).xyz, float(0.5), float(0.9));
            float _Property_43276f8cd0bb43c791ef6a5569938c47_Out_0_Float = _FissureProgress;
            float _Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float;
            Unity_Lerp_float(float(1), float(0.35), _Property_43276f8cd0bb43c791ef6a5569938c47_Out_0_Float, _Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float);
            float _Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float;
            Unity_Distance_float2(_Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2, float2(0.5, 0.5), _Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float);
            float _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float;
            Unity_OneMinus_float(_Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float, _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float);
            float _Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float;
            Unity_Step_float(_Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float, _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float, _Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float);
            float _Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float;
            Unity_Multiply_float_float(_Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float, _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float, _Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float);
            float4 _Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4, (_Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float.xxxx), _Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4);
            surface.BaseColor = (_Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4.xyz);
            surface.Emission = _EmissionNode_3e929fcc60694caa888171a6f04c27d9_Output_0_Vector3;
            surface.Alpha = (_Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4).x;
            surface.AlphaClipThreshold = float(0.5);
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
        
            #if defined(SHADER_STAGE_RAY_TRACING)
            #else
            #endif
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
        #pragma shader_feature_local _ _ALPHATEST_ON
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
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_POSITION_WS
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
        float4 _MainTex_TexelSize;
        float _FissureProgress;
        float _NoiseOffset;
        float _NoiseDensity;
        float4 _FissureColor;
        float4 _Color;
        float _Edge;
        float _Offset;
        float4 _EmissionColor;
        float _UseShadowThreshold;
        float4 _DoubleSidedConstants;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        float _BlendMode;
        float _EnableBlendModePreserveSpecularLighting;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
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
            #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi2D.hlsl"
        #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi3D.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonLighting.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            struct SurfaceDescriptionInputs
        {
             float3 AbsoluteWorldSpacePosition;
             float4 uv0;
             float3 TimeParameters;
        };
        
            //Interpolator Packs: <None>
        
            // --------------------------------------------------
            // Graph
        
        
            // Graph Functions
            
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
        struct Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float
        {
        };
        
        void SG_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float(float2 Vector2_DDA00F47, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float IN, out float Value_1, out float Cells_2)
        {
        float2 _Property_87196192b55192848c90533525880767_Out_0_Vector2 = Vector2_DDA00F47;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        VoronoiPrecise2D_float(_Property_87196192b55192848c90533525880767_Out_0_Vector2, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Step_float(float Edge, float In, out float Out)
        {
            Out = step(Edge, In);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        struct Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float
        {
        };
        
        void SG_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float(float3 Vector3_375394F7, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float IN, out float Value_1, out float Cells_2)
        {
        float3 _Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3 = Vector3_375394F7;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        Voronoi3D_float(_Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
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
        
            // Graph Vertex
            // GraphVertex: <None>
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float3 Emission;
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_683e3b8c31014b2397e10acee02ca4a2_Out_0_Vector4 = _Color;
            UnityTexture2D _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.tex, _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.samplerstate, _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_R_4_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.r;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_G_5_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.g;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_B_6_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.b;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_A_7_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.a;
            float4 _Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_683e3b8c31014b2397e10acee02ca4a2_Out_0_Vector4, _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4, _Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4);
            float _Property_65f904d6199d4350a89b9904628b7f25_Out_0_Float = _FissureProgress;
            float _Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float;
            Unity_Lerp_float(float(3), float(0), _Property_65f904d6199d4350a89b9904628b7f25_Out_0_Float, _Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float);
            float4 _UV_f886bd3ce6e14698b48085645e8892a4_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2 = _UV_f886bd3ce6e14698b48085645e8892a4_Out_0_Vector4.xy;
            float _Property_a85bc01ad5de45428bea3152b546512d_Out_0_Float = _NoiseOffset;
            float _Property_b39de3d823764eb4b0936e62e6fe5074_Out_0_Float = _NoiseDensity;
            Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b;
            float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float;
            float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float;
            SG_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float(_Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2, _Property_a85bc01ad5de45428bea3152b546512d_Out_0_Float, _Property_b39de3d823764eb4b0936e62e6fe5074_Out_0_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float);
            float _Step_34f306488de6462093199997839c7cc3_Out_2_Float;
            Unity_Step_float(_Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float, _Step_34f306488de6462093199997839c7cc3_Out_2_Float);
            float _Property_7ea25f3e615d42c5b48c4467642bef17_Out_0_Float = _FissureProgress;
            float _Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float;
            Unity_Lerp_float(float(0.04), float(0.01), _Property_7ea25f3e615d42c5b48c4467642bef17_Out_0_Float, _Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float);
            float _Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float;
            Unity_Step_float(_Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float, _Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float);
            float _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float;
            Unity_OneMinus_float(_Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float, _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float);
            float _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float;
            Unity_Add_float(_Step_34f306488de6462093199997839c7cc3_Out_2_Float, _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float, _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float);
            float4 _Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_FissureColor) : _FissureColor;
            float4 _Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Add_1896dee97c9e464eb43461caea40c052_Out_2_Float.xxxx), _Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4, _Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4);
            float _Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float = _Edge;
            float4 _UV_9adc53fa249a4a5ab32d3a050acbfdaf_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_2029f5a6f61a4f00a6eb3e4c29b5b6af_Out_1_Vector2 = _UV_9adc53fa249a4a5ab32d3a050acbfdaf_Out_0_Vector4.xy;
            float _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float;
            Unity_Distance_float2(_Swizzle_2029f5a6f61a4f00a6eb3e4c29b5b6af_Out_1_Vector2, float2(0.5, 0.5), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float);
            float _Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float;
            Unity_Smoothstep_float(_Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float, float(1), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float, _Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float);
            float3 _Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3;
            Unity_Absolute_float3(IN.AbsoluteWorldSpacePosition, _Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3);
            Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7;
            float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Value_1_Float;
            float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float;
            SG_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float(_Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3, IN.TimeParameters.x, float(4.1), _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Value_1_Float, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float);
            float _Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float, _Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float);
            float _Property_4b01ac9dd6b047d2b9cf45fca7f0e366_Out_0_Float = _Offset;
            float _Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float;
            Unity_Add_float(_Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float, _Property_4b01ac9dd6b047d2b9cf45fca7f0e366_Out_0_Float, _Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float);
            float _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float;
            Unity_Smoothstep_float(_Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float, float(1), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float, _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float);
            float _Add_1f600a25927b426195eb7a73491937ac_Out_2_Float;
            Unity_Add_float(_Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float, _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float, _Add_1f600a25927b426195eb7a73491937ac_Out_2_Float);
            float4 _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4, (_Add_1f600a25927b426195eb7a73491937ac_Out_2_Float.xxxx), _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4);
            float4 _Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4, _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4, _Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4);
            float3 _EmissionNode_3e929fcc60694caa888171a6f04c27d9_Output_0_Vector3 = Unity_HDRP_GetEmissionHDRColor_float((_Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4.xyz).xyz, float(0.5), float(0.9));
            float _Property_43276f8cd0bb43c791ef6a5569938c47_Out_0_Float = _FissureProgress;
            float _Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float;
            Unity_Lerp_float(float(1), float(0.35), _Property_43276f8cd0bb43c791ef6a5569938c47_Out_0_Float, _Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float);
            float _Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float;
            Unity_Distance_float2(_Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2, float2(0.5, 0.5), _Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float);
            float _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float;
            Unity_OneMinus_float(_Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float, _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float);
            float _Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float;
            Unity_Step_float(_Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float, _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float, _Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float);
            float _Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float;
            Unity_Multiply_float_float(_Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float, _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float, _Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float);
            float4 _Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4, (_Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float.xxxx), _Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4);
            surface.BaseColor = (_Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4.xyz);
            surface.Emission = _EmissionNode_3e929fcc60694caa888171a6f04c27d9_Output_0_Vector3;
            surface.Alpha = (_Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4).x;
            surface.AlphaClipThreshold = float(0.5);
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
        
            #if defined(SHADER_STAGE_RAY_TRACING)
            #else
            #endif
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
            #pragma shader_feature_local _ _ALPHATEST_ON
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
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_POSITION_WS
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
        float4 _MainTex_TexelSize;
        float _FissureProgress;
        float _NoiseOffset;
        float _NoiseDensity;
        float4 _FissureColor;
        float4 _Color;
        float _Edge;
        float _Offset;
        float4 _EmissionColor;
        float _UseShadowThreshold;
        float4 _DoubleSidedConstants;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        float _BlendMode;
        float _EnableBlendModePreserveSpecularLighting;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
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
            #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi2D.hlsl"
        #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi3D.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonLighting.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            struct SurfaceDescriptionInputs
        {
             float3 AbsoluteWorldSpacePosition;
             float4 uv0;
             float3 TimeParameters;
        };
        
            //Interpolator Packs: <None>
        
            // --------------------------------------------------
            // Graph
        
        
            // Graph Functions
            
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
        struct Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float
        {
        };
        
        void SG_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float(float2 Vector2_DDA00F47, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float IN, out float Value_1, out float Cells_2)
        {
        float2 _Property_87196192b55192848c90533525880767_Out_0_Vector2 = Vector2_DDA00F47;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        VoronoiPrecise2D_float(_Property_87196192b55192848c90533525880767_Out_0_Vector2, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Step_float(float Edge, float In, out float Out)
        {
            Out = step(Edge, In);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        struct Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float
        {
        };
        
        void SG_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float(float3 Vector3_375394F7, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float IN, out float Value_1, out float Cells_2)
        {
        float3 _Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3 = Vector3_375394F7;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        Voronoi3D_float(_Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
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
        
            // Graph Vertex
            // GraphVertex: <None>
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float3 Emission;
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_683e3b8c31014b2397e10acee02ca4a2_Out_0_Vector4 = _Color;
            UnityTexture2D _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.tex, _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.samplerstate, _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_R_4_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.r;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_G_5_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.g;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_B_6_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.b;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_A_7_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.a;
            float4 _Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_683e3b8c31014b2397e10acee02ca4a2_Out_0_Vector4, _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4, _Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4);
            float _Property_65f904d6199d4350a89b9904628b7f25_Out_0_Float = _FissureProgress;
            float _Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float;
            Unity_Lerp_float(float(3), float(0), _Property_65f904d6199d4350a89b9904628b7f25_Out_0_Float, _Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float);
            float4 _UV_f886bd3ce6e14698b48085645e8892a4_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2 = _UV_f886bd3ce6e14698b48085645e8892a4_Out_0_Vector4.xy;
            float _Property_a85bc01ad5de45428bea3152b546512d_Out_0_Float = _NoiseOffset;
            float _Property_b39de3d823764eb4b0936e62e6fe5074_Out_0_Float = _NoiseDensity;
            Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b;
            float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float;
            float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float;
            SG_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float(_Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2, _Property_a85bc01ad5de45428bea3152b546512d_Out_0_Float, _Property_b39de3d823764eb4b0936e62e6fe5074_Out_0_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float);
            float _Step_34f306488de6462093199997839c7cc3_Out_2_Float;
            Unity_Step_float(_Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float, _Step_34f306488de6462093199997839c7cc3_Out_2_Float);
            float _Property_7ea25f3e615d42c5b48c4467642bef17_Out_0_Float = _FissureProgress;
            float _Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float;
            Unity_Lerp_float(float(0.04), float(0.01), _Property_7ea25f3e615d42c5b48c4467642bef17_Out_0_Float, _Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float);
            float _Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float;
            Unity_Step_float(_Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float, _Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float);
            float _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float;
            Unity_OneMinus_float(_Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float, _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float);
            float _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float;
            Unity_Add_float(_Step_34f306488de6462093199997839c7cc3_Out_2_Float, _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float, _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float);
            float4 _Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_FissureColor) : _FissureColor;
            float4 _Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Add_1896dee97c9e464eb43461caea40c052_Out_2_Float.xxxx), _Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4, _Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4);
            float _Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float = _Edge;
            float4 _UV_9adc53fa249a4a5ab32d3a050acbfdaf_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_2029f5a6f61a4f00a6eb3e4c29b5b6af_Out_1_Vector2 = _UV_9adc53fa249a4a5ab32d3a050acbfdaf_Out_0_Vector4.xy;
            float _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float;
            Unity_Distance_float2(_Swizzle_2029f5a6f61a4f00a6eb3e4c29b5b6af_Out_1_Vector2, float2(0.5, 0.5), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float);
            float _Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float;
            Unity_Smoothstep_float(_Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float, float(1), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float, _Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float);
            float3 _Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3;
            Unity_Absolute_float3(IN.AbsoluteWorldSpacePosition, _Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3);
            Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7;
            float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Value_1_Float;
            float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float;
            SG_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float(_Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3, IN.TimeParameters.x, float(4.1), _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Value_1_Float, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float);
            float _Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float, _Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float);
            float _Property_4b01ac9dd6b047d2b9cf45fca7f0e366_Out_0_Float = _Offset;
            float _Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float;
            Unity_Add_float(_Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float, _Property_4b01ac9dd6b047d2b9cf45fca7f0e366_Out_0_Float, _Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float);
            float _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float;
            Unity_Smoothstep_float(_Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float, float(1), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float, _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float);
            float _Add_1f600a25927b426195eb7a73491937ac_Out_2_Float;
            Unity_Add_float(_Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float, _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float, _Add_1f600a25927b426195eb7a73491937ac_Out_2_Float);
            float4 _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4, (_Add_1f600a25927b426195eb7a73491937ac_Out_2_Float.xxxx), _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4);
            float4 _Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4, _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4, _Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4);
            float3 _EmissionNode_3e929fcc60694caa888171a6f04c27d9_Output_0_Vector3 = Unity_HDRP_GetEmissionHDRColor_float((_Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4.xyz).xyz, float(0.5), float(0.9));
            float _Property_43276f8cd0bb43c791ef6a5569938c47_Out_0_Float = _FissureProgress;
            float _Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float;
            Unity_Lerp_float(float(1), float(0.35), _Property_43276f8cd0bb43c791ef6a5569938c47_Out_0_Float, _Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float);
            float _Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float;
            Unity_Distance_float2(_Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2, float2(0.5, 0.5), _Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float);
            float _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float;
            Unity_OneMinus_float(_Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float, _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float);
            float _Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float;
            Unity_Step_float(_Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float, _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float, _Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float);
            float _Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float;
            Unity_Multiply_float_float(_Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float, _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float, _Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float);
            float4 _Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4, (_Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float.xxxx), _Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4);
            surface.BaseColor = (_Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4.xyz);
            surface.Emission = _EmissionNode_3e929fcc60694caa888171a6f04c27d9_Output_0_Vector3;
            surface.Alpha = (_Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4).x;
            surface.AlphaClipThreshold = float(0.5);
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
        
            #if defined(SHADER_STAGE_RAY_TRACING)
            #else
            #endif
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
            #pragma shader_feature_local _ _ALPHATEST_ON
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
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_POSITION_WS
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
        float4 _MainTex_TexelSize;
        float _FissureProgress;
        float _NoiseOffset;
        float _NoiseDensity;
        float4 _FissureColor;
        float4 _Color;
        float _Edge;
        float _Offset;
        float4 _EmissionColor;
        float _UseShadowThreshold;
        float4 _DoubleSidedConstants;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        float _BlendMode;
        float _EnableBlendModePreserveSpecularLighting;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
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
            #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi2D.hlsl"
        #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi3D.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonLighting.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            struct SurfaceDescriptionInputs
        {
             float3 AbsoluteWorldSpacePosition;
             float4 uv0;
             float3 TimeParameters;
        };
        
            //Interpolator Packs: <None>
        
            // --------------------------------------------------
            // Graph
        
        
            // Graph Functions
            
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
        struct Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float
        {
        };
        
        void SG_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float(float2 Vector2_DDA00F47, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float IN, out float Value_1, out float Cells_2)
        {
        float2 _Property_87196192b55192848c90533525880767_Out_0_Vector2 = Vector2_DDA00F47;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        VoronoiPrecise2D_float(_Property_87196192b55192848c90533525880767_Out_0_Vector2, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Step_float(float Edge, float In, out float Out)
        {
            Out = step(Edge, In);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        struct Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float
        {
        };
        
        void SG_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float(float3 Vector3_375394F7, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float IN, out float Value_1, out float Cells_2)
        {
        float3 _Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3 = Vector3_375394F7;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        Voronoi3D_float(_Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
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
        
            // Graph Vertex
            // GraphVertex: <None>
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float3 Emission;
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_683e3b8c31014b2397e10acee02ca4a2_Out_0_Vector4 = _Color;
            UnityTexture2D _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.tex, _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.samplerstate, _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_R_4_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.r;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_G_5_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.g;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_B_6_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.b;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_A_7_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.a;
            float4 _Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_683e3b8c31014b2397e10acee02ca4a2_Out_0_Vector4, _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4, _Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4);
            float _Property_65f904d6199d4350a89b9904628b7f25_Out_0_Float = _FissureProgress;
            float _Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float;
            Unity_Lerp_float(float(3), float(0), _Property_65f904d6199d4350a89b9904628b7f25_Out_0_Float, _Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float);
            float4 _UV_f886bd3ce6e14698b48085645e8892a4_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2 = _UV_f886bd3ce6e14698b48085645e8892a4_Out_0_Vector4.xy;
            float _Property_a85bc01ad5de45428bea3152b546512d_Out_0_Float = _NoiseOffset;
            float _Property_b39de3d823764eb4b0936e62e6fe5074_Out_0_Float = _NoiseDensity;
            Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b;
            float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float;
            float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float;
            SG_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float(_Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2, _Property_a85bc01ad5de45428bea3152b546512d_Out_0_Float, _Property_b39de3d823764eb4b0936e62e6fe5074_Out_0_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float);
            float _Step_34f306488de6462093199997839c7cc3_Out_2_Float;
            Unity_Step_float(_Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float, _Step_34f306488de6462093199997839c7cc3_Out_2_Float);
            float _Property_7ea25f3e615d42c5b48c4467642bef17_Out_0_Float = _FissureProgress;
            float _Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float;
            Unity_Lerp_float(float(0.04), float(0.01), _Property_7ea25f3e615d42c5b48c4467642bef17_Out_0_Float, _Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float);
            float _Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float;
            Unity_Step_float(_Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float, _Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float);
            float _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float;
            Unity_OneMinus_float(_Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float, _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float);
            float _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float;
            Unity_Add_float(_Step_34f306488de6462093199997839c7cc3_Out_2_Float, _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float, _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float);
            float4 _Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_FissureColor) : _FissureColor;
            float4 _Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Add_1896dee97c9e464eb43461caea40c052_Out_2_Float.xxxx), _Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4, _Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4);
            float _Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float = _Edge;
            float4 _UV_9adc53fa249a4a5ab32d3a050acbfdaf_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_2029f5a6f61a4f00a6eb3e4c29b5b6af_Out_1_Vector2 = _UV_9adc53fa249a4a5ab32d3a050acbfdaf_Out_0_Vector4.xy;
            float _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float;
            Unity_Distance_float2(_Swizzle_2029f5a6f61a4f00a6eb3e4c29b5b6af_Out_1_Vector2, float2(0.5, 0.5), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float);
            float _Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float;
            Unity_Smoothstep_float(_Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float, float(1), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float, _Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float);
            float3 _Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3;
            Unity_Absolute_float3(IN.AbsoluteWorldSpacePosition, _Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3);
            Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7;
            float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Value_1_Float;
            float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float;
            SG_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float(_Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3, IN.TimeParameters.x, float(4.1), _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Value_1_Float, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float);
            float _Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float, _Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float);
            float _Property_4b01ac9dd6b047d2b9cf45fca7f0e366_Out_0_Float = _Offset;
            float _Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float;
            Unity_Add_float(_Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float, _Property_4b01ac9dd6b047d2b9cf45fca7f0e366_Out_0_Float, _Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float);
            float _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float;
            Unity_Smoothstep_float(_Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float, float(1), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float, _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float);
            float _Add_1f600a25927b426195eb7a73491937ac_Out_2_Float;
            Unity_Add_float(_Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float, _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float, _Add_1f600a25927b426195eb7a73491937ac_Out_2_Float);
            float4 _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4, (_Add_1f600a25927b426195eb7a73491937ac_Out_2_Float.xxxx), _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4);
            float4 _Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4, _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4, _Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4);
            float3 _EmissionNode_3e929fcc60694caa888171a6f04c27d9_Output_0_Vector3 = Unity_HDRP_GetEmissionHDRColor_float((_Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4.xyz).xyz, float(0.5), float(0.9));
            float _Property_43276f8cd0bb43c791ef6a5569938c47_Out_0_Float = _FissureProgress;
            float _Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float;
            Unity_Lerp_float(float(1), float(0.35), _Property_43276f8cd0bb43c791ef6a5569938c47_Out_0_Float, _Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float);
            float _Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float;
            Unity_Distance_float2(_Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2, float2(0.5, 0.5), _Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float);
            float _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float;
            Unity_OneMinus_float(_Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float, _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float);
            float _Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float;
            Unity_Step_float(_Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float, _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float, _Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float);
            float _Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float;
            Unity_Multiply_float_float(_Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float, _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float, _Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float);
            float4 _Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4, (_Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float.xxxx), _Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4);
            surface.BaseColor = (_Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4.xyz);
            surface.Emission = _EmissionNode_3e929fcc60694caa888171a6f04c27d9_Output_0_Vector3;
            surface.Alpha = (_Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4).x;
            surface.AlphaClipThreshold = float(0.5);
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
        
            #if defined(SHADER_STAGE_RAY_TRACING)
            #else
            #endif
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
        	#include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi2D.hlsl"
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
            #pragma shader_feature_local _ _ALPHATEST_ON
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
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_POSITION_WS
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
        float4 _MainTex_TexelSize;
        float _FissureProgress;
        float _NoiseOffset;
        float _NoiseDensity;
        float4 _FissureColor;
        float4 _Color;
        float _Edge;
        float _Offset;
        float4 _EmissionColor;
        float _UseShadowThreshold;
        float4 _DoubleSidedConstants;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        float _BlendMode;
        float _EnableBlendModePreserveSpecularLighting;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
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
            #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi2D.hlsl"
        #include_with_pragmas "Assets/SharedAssets/Shaders/NoisyNodes/HLSL/Voronoi3D.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonLighting.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            struct SurfaceDescriptionInputs
        {
             float3 AbsoluteWorldSpacePosition;
             float4 uv0;
             float3 TimeParameters;
        };
        
            //Interpolator Packs: <None>
        
            // --------------------------------------------------
            // Graph
        
        
            // Graph Functions
            
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
        struct Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float
        {
        };
        
        void SG_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float(float2 Vector2_DDA00F47, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float IN, out float Value_1, out float Cells_2)
        {
        float2 _Property_87196192b55192848c90533525880767_Out_0_Vector2 = Vector2_DDA00F47;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        VoronoiPrecise2D_float(_Property_87196192b55192848c90533525880767_Out_0_Vector2, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _VoronoiPrecise2DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Step_float(float Edge, float In, out float Out)
        {
            Out = step(Edge, In);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        struct Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float
        {
        };
        
        void SG_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float(float3 Vector3_375394F7, float Vector1_B47BD908, float Vector1_AEE7F28E, Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float IN, out float Value_1, out float Cells_2)
        {
        float3 _Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3 = Vector3_375394F7;
        float _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float = Vector1_B47BD908;
        float _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float = Vector1_AEE7F28E;
        float _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        float _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        Voronoi3D_float(_Property_c0874fd2920f9c8baf4b91be875c7ebb_Out_0_Vector3, _Property_1d30cf63fc420f8f99a0cbe60bb88392_Out_0_Float, _Property_01df2bf4a6ebe48c8c34049cb8b5c130_Out_0_Float, _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float, _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float);
        Value_1 = _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Value_3_Float;
        Cells_2 = _Voronoi3DCustomFunction_f3778cac851e5b82b5979141914b8445_Cells_4_Float;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
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
        
            // Graph Vertex
            // GraphVertex: <None>
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float3 Emission;
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_683e3b8c31014b2397e10acee02ca4a2_Out_0_Vector4 = _Color;
            UnityTexture2D _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.tex, _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.samplerstate, _Property_bed3354374a0479e8b74d37e42708300_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_R_4_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.r;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_G_5_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.g;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_B_6_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.b;
            float _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_A_7_Float = _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4.a;
            float4 _Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_683e3b8c31014b2397e10acee02ca4a2_Out_0_Vector4, _SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4, _Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4);
            float _Property_65f904d6199d4350a89b9904628b7f25_Out_0_Float = _FissureProgress;
            float _Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float;
            Unity_Lerp_float(float(3), float(0), _Property_65f904d6199d4350a89b9904628b7f25_Out_0_Float, _Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float);
            float4 _UV_f886bd3ce6e14698b48085645e8892a4_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2 = _UV_f886bd3ce6e14698b48085645e8892a4_Out_0_Vector4.xy;
            float _Property_a85bc01ad5de45428bea3152b546512d_Out_0_Float = _NoiseOffset;
            float _Property_b39de3d823764eb4b0936e62e6fe5074_Out_0_Float = _NoiseDensity;
            Bindings_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b;
            float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float;
            float _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float;
            SG_Voronoiprecisenoise2D_cc44dcf79959c4e42b803dce71e81be8_float(_Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2, _Property_a85bc01ad5de45428bea3152b546512d_Out_0_Float, _Property_b39de3d823764eb4b0936e62e6fe5074_Out_0_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float);
            float _Step_34f306488de6462093199997839c7cc3_Out_2_Float;
            Unity_Step_float(_Lerp_06f5b45a25b845bfbed3c4fe3dc6c536_Out_3_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Cells_2_Float, _Step_34f306488de6462093199997839c7cc3_Out_2_Float);
            float _Property_7ea25f3e615d42c5b48c4467642bef17_Out_0_Float = _FissureProgress;
            float _Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float;
            Unity_Lerp_float(float(0.04), float(0.01), _Property_7ea25f3e615d42c5b48c4467642bef17_Out_0_Float, _Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float);
            float _Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float;
            Unity_Step_float(_Lerp_47d917246ac64940a62197fbca3e877a_Out_3_Float, _Voronoiprecisenoise2D_f6c583d8a6754991bac2c21c46bdc80b_Value_1_Float, _Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float);
            float _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float;
            Unity_OneMinus_float(_Step_873b24e259e74389bf2431ff8745f88d_Out_2_Float, _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float);
            float _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float;
            Unity_Add_float(_Step_34f306488de6462093199997839c7cc3_Out_2_Float, _OneMinus_c07d929428cd47888787008ad4b312de_Out_1_Float, _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float);
            float4 _Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_FissureColor) : _FissureColor;
            float4 _Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Add_1896dee97c9e464eb43461caea40c052_Out_2_Float.xxxx), _Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4, _Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4);
            float _Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float = _Edge;
            float4 _UV_9adc53fa249a4a5ab32d3a050acbfdaf_Out_0_Vector4 = IN.uv0;
            float2 _Swizzle_2029f5a6f61a4f00a6eb3e4c29b5b6af_Out_1_Vector2 = _UV_9adc53fa249a4a5ab32d3a050acbfdaf_Out_0_Vector4.xy;
            float _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float;
            Unity_Distance_float2(_Swizzle_2029f5a6f61a4f00a6eb3e4c29b5b6af_Out_1_Vector2, float2(0.5, 0.5), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float);
            float _Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float;
            Unity_Smoothstep_float(_Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float, float(1), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float, _Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float);
            float3 _Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3;
            Unity_Absolute_float3(IN.AbsoluteWorldSpacePosition, _Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3);
            Bindings_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7;
            float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Value_1_Float;
            float _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float;
            SG_Voronoinoise3D_92001a1a051ec8247a12e151ca32427b_float(_Absolute_77f82f3690004cac92a7a2a437379c2a_Out_1_Vector3, IN.TimeParameters.x, float(4.1), _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Value_1_Float, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float);
            float _Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_31efcced45514b459dffdcd3fa97a947_Out_3_Float, _Voronoinoise3D_97499cb141f7429cbf90036701e4b3b7_Cells_2_Float, _Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float);
            float _Property_4b01ac9dd6b047d2b9cf45fca7f0e366_Out_0_Float = _Offset;
            float _Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float;
            Unity_Add_float(_Property_3cda8bc3ec7048a0adebacb3d56ca186_Out_0_Float, _Property_4b01ac9dd6b047d2b9cf45fca7f0e366_Out_0_Float, _Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float);
            float _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float;
            Unity_Smoothstep_float(_Add_0073c3b6d8d9439aafb3da6d10ef41b9_Out_2_Float, float(1), _Distance_7c742906bf194d0ca9ab47773f532f5b_Out_2_Float, _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float);
            float _Add_1f600a25927b426195eb7a73491937ac_Out_2_Float;
            Unity_Add_float(_Multiply_61727a50d49a476180482d8c329bc5a7_Out_2_Float, _Smoothstep_72f4a79cb76440beab6ee477478030d3_Out_3_Float, _Add_1f600a25927b426195eb7a73491937ac_Out_2_Float);
            float4 _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_fccf6e94d8004d2583c6f080a41a6fec_Out_0_Vector4, (_Add_1f600a25927b426195eb7a73491937ac_Out_2_Float.xxxx), _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4);
            float4 _Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_019cf978a8c1461db7da8383665c5afd_Out_2_Vector4, _Multiply_1d307f039c964335b19f4b6e7202d250_Out_2_Vector4, _Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4);
            float3 _EmissionNode_3e929fcc60694caa888171a6f04c27d9_Output_0_Vector3 = Unity_HDRP_GetEmissionHDRColor_float((_Multiply_f128f639b6a546e89858bd701820c18f_Out_2_Vector4.xyz).xyz, float(0.5), float(0.9));
            float _Property_43276f8cd0bb43c791ef6a5569938c47_Out_0_Float = _FissureProgress;
            float _Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float;
            Unity_Lerp_float(float(1), float(0.35), _Property_43276f8cd0bb43c791ef6a5569938c47_Out_0_Float, _Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float);
            float _Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float;
            Unity_Distance_float2(_Swizzle_c5bfcf1a7b2b4053b4ec896dc4b7965e_Out_1_Vector2, float2(0.5, 0.5), _Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float);
            float _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float;
            Unity_OneMinus_float(_Distance_8b0442c2b40947559506ce209e71c061_Out_2_Float, _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float);
            float _Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float;
            Unity_Step_float(_Lerp_0f12fed4977a4d3aace1adf8e8948148_Out_3_Float, _OneMinus_9e455f45d55f416b9f4802c444a3f980_Out_1_Float, _Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float);
            float _Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float;
            Unity_Multiply_float_float(_Step_313d38f0560649d899764bf5b10d09ea_Out_2_Float, _Add_1896dee97c9e464eb43461caea40c052_Out_2_Float, _Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float);
            float4 _Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_17980355455f4c8eb2aead770f89bd08_RGBA_0_Vector4, (_Multiply_d996b0ea6ad940939228ded5743c405e_Out_2_Float.xxxx), _Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4);
            surface.BaseColor = (_Multiply_f710e6394f4b4b45b5825841346e5b5a_Out_2_Vector4.xyz);
            surface.Emission = _EmissionNode_3e929fcc60694caa888171a6f04c27d9_Output_0_Vector3;
            surface.Alpha = (_Multiply_a077e2fac89e48029af7eda168f62bff_Out_2_Vector4).x;
            surface.AlphaClipThreshold = float(0.5);
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
        
            #if defined(SHADER_STAGE_RAY_TRACING)
            #else
            #endif
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