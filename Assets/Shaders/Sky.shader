Shader "Unlit/Sky"
{
    Properties
    {
        [NoScaleOffset][PerRendererData] _MainTex ("Main", 2D) = "white" {}
        _Speed("Speed", float) = 0

        [NoScaleOffset] _Distort("Distort", 2D) = "bump"{}
        _DistParams("Distort XY:Speed, Z:Size, W:Power", vector) = (0,0,0,0)
        [Toggle] _DistUVMask("Distort UV Mask", float) = 0

        [HideInInspector] _StencilComp("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask("Stencil Read Mask", Float) = 255

        [HideInInspector] _ColorMask("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest[unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask[_ColorMask]

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 uvDist : TEXCOORD1;
            };

            sampler2D_half _MainTex;
            fixed _Speed;

            // Distort
            sampler2D_half _Distort;
            half4 _DistParams;
            fixed _DistUVMask;

            // UI
            float4 _ClipRect;

            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv + float2(_Speed * _Time.x, 0);

                float3 wPos = mul(unity_ObjectToWorld, v.vertex);
                o.uvDist.xy = wPos.xy / _DistParams.z + _DistParams.xy * _Time.x;
                o.uvDist.zw = 1;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 dist = tex2D(_Distort, i.uvDist).xy;
                dist = (dist - .5) * _DistParams.w;
                
                if (_DistUVMask)
                    dist *= i.uv.x;

                fixed4 col = tex2D(_MainTex, i.uv + dist);

                col.rgb += (dist.x + dist.y) * 2;

              //  #ifdef UNITY_UI_CLIP_RECT
              //  col.a *= UnityGet2DClipping(i.wPos, _ClipRect);
              //  #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(col.a - 0.001);
                #endif
                
                return col;
            }
            ENDCG
        }
    }
}
