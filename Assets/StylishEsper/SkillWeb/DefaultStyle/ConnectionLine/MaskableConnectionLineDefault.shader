Shader "SkillWeb/MaskableConnectionLineDefault"
{
    Properties
    {
        [NoScaleOffset]_InactiveTexture("Inactive Texture", 2D) = "white" {}
        _InactiveColorTint("Inactive Color Tint", Color) = (1, 1, 1, 1)
        [NoScaleOffset]_ActiveTexture("Active Texture", 2D) = "white" {}
        _ActiveColorTint("Active Color Tint", Color) = (1, 1, 1, 1)
        _HighlightColor("Highlight Color", Color) = (1, 1, 1, 1)
        _HighlightStrength("Highlight Strength", Range(0, 1)) = 0
        [Toggle] _Active("Active", Float) = 0
        
        // Stencil properties
        _Stencil("Stencil ID", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comparison", Float) = 8
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilOp("Stencil Operation", Float) = 0
        _StencilReadMask("Stencil Read Mask", Float) = 255
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        [Enum(None,0,Alpha,1,Red,8,Green,4,Blue,2,RGB,14,RGBA,15)] _ColorMask("Color Mask", Float) = 15
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
        }
        
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        
        Pass
        {
            Name "Main"
            
            Stencil
            {
                Ref [_Stencil]
                Comp [_StencilComp]
                Pass [_StencilOp]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
            }
            ColorMask [_ColorMask]
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };
            
            sampler2D _InactiveTexture;
            float4 _InactiveTexture_ST;
            sampler2D _ActiveTexture;
            float4 _ActiveTexture_ST;
            fixed4 _InactiveColorTint;
            fixed4 _ActiveColorTint;
            fixed4 _HighlightColor;
            float _HighlightStrength;
            float _Active;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Debug: Test if _Active property is being read
                // Uncomment ONE of these debug lines to test:
                // return fixed4(_Active, _Active, _Active, 1); // Shows _Active value as grayscale
                // return _Active > 0.5 ? fixed4(1,0,0,1) : fixed4(0,1,0,1); // Red if active, green if inactive
                
                // Sample appropriate texture based on Active state
                fixed4 texColor;
                fixed4 tintColor;
                
                // More explicit comparison for Active state
                bool isActive = _Active > 0.5;
                
                if (isActive)
                {
                    texColor = tex2D(_ActiveTexture, i.uv);
                    tintColor = _ActiveColorTint;
                }
                else
                {
                    texColor = tex2D(_InactiveTexture, i.uv);
                    tintColor = _InactiveColorTint;
                }
                
                // Apply color tint
                fixed4 tintedColor = texColor * tintColor;
                
                // Apply highlight
                fixed4 finalColor = lerp(tintedColor, _HighlightColor, _HighlightStrength);
                
                // Apply vertex color
                finalColor *= i.color;
                
                // Apply fog
                UNITY_APPLY_FOG(i.fogCoord, finalColor);
                
                return finalColor;
            }
            ENDCG
        }
    }
    
    Fallback "Sprites/Default"
}