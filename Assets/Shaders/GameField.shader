Shader "Unlit/GameField"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        [NoScaleOffset] _MainTex ("Main", 2D) = "white" {}
        _MainTile("Main Tile", float) = 1

        [NoScaleOffset] _BorderMask("Border Mask (R)", 2D) = "gray"{}
        _BorderTile("Border Tile", float) = 1

        [Space(8)]
        [NoScaleOffset] _Checker("Checker", 2D) = "gray"{}
        
        _CountX("Count X", Range(1, 9)) = 9
        _CountY("Count Y", Range(1, 9)) = 9

        [Space(8)]
        _CellLevel("Cell Level", Range(-1, 1)) = 0
        _CellSoft("Cell Soft", Range(0, .5)) = 1
        _CellPower("Cell Power", Range(0, 1)) = 1

        [Space(8)]
        _BorderColor("Border Color", Color) = (1,1,1,0)
        _BorderGrow("Border Grow", Range(0, .2)) = 0
        _BorderWide("Border Wide", Range(0, .5)) = 0
        _BorderSoft("Border Soft", Range(0, .5)) = .1

        [Space(8)]
        [NoScaleOffset] _Distort("Distort Map", 2D) = "gray"{}
        _DistortTile("Distort Tile", float) = 1
        _DistortSpeed("Distort Speed", float) = 0
        _DistortPower("Distort Power", Range(0, .05)) = 0
        
        [HideInInspector] _StencilComp("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask("Stencil Read Mask", Float) = 255

        [HideInInspector] _ColorMask("Color Mask", Float) = 15

        [HideInInspector] [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "PreviewType" = "Plane"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        
        Pass
        {
            Cull Off
            Lighting Off
            ZWrite Off
            ZTest[unity_GUIZTestMode]
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask[_ColorMask]
            
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

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uvChecker : TEXCOORD1;
                half4  mask : TEXCOORD2;
            };

            fixed4 _Color;
            sampler2D_half _MainTex;
            fixed _MainTile;

            sampler2D_half _BorderMask;
            fixed _BorderTile;

            sampler2D_half _Checker;
            uint _CountX;
            uint _CountY;
            
            fixed _CellLevel;
            fixed _CellSoft;
            fixed _CellPower;

            fixed3 _BorderColor;
            fixed _BorderGrow;
            fixed _BorderWide;
            fixed _BorderSoft;

            sampler2D_half _Distort;
            fixed _DistortTile;
            fixed _DistortSpeed;
            fixed _DistortPower;

            float4 _ClipRect;
            float _UIMaskSoftnessX;
            float _UIMaskSoftnessY;
            
            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                v.vertex.xy *= 1.0 + _BorderGrow;
                
                o.uv = v.uv;
                o.vertex = UnityObjectToClipPos(v.vertex);

                v.uv = (v.uv - .5) * (1.0 + _BorderGrow) + .5;
                o.uvChecker = v.uv * half2(_CountX, _CountY) / 2;

                float2 pixelSize = o.vertex.w;
                pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));
                float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                float2 maskUV = (v.vertex.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);
                
                o.mask = half4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));

                return o;
            }

            float RangeLineBoldShort(float base, float sec)
            {
                return (sec - .5) * (1.0 - abs(base * 2.0 - 1.0)) + base;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv * _MainTile);
                
                fixed mask = (col.r + col.g + col.b) / 3 + _CellLevel;
                col.rgb = lerp(col.rgb, _Color.rgb, _Color.a);

                fixed checker = tex2D(_Checker, i.uvChecker).r;
                checker = RangeLineBoldShort(checker, mask);
                checker = smoothstep(.5 - _CellSoft, .5 + _CellSoft, checker);

                col.rgb *= (checker - .5) * _CellPower + 1;

                // sample distort
                half2 dist = tex2D(_Distort, i.uv * _DistortTile + _Time.x * _DistortSpeed).rg;
                dist = (dist - .5) * _DistortPower;

                // calc border
                fixed2 border = abs(i.uv * 2 - 1);
                border.x = 1 - max(border.x, border.y);
                border.x = saturate(border.x / _BorderWide);

                mask = 1 - tex2D(_BorderMask, i.uv * _BorderTile + dist).r;
                mask = RangeLineBoldShort(border.x, mask);
                col.a = smoothstep(.5 - _BorderSoft, .5 + _BorderSoft, mask);

                // internal shadow
                col.rgb *= lerp(_BorderColor.rgb, 1, border.x * col.a);

                #ifdef UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(i.mask.xy)) * i.mask.zw);
                col.a *= m.x * m.y;
                col.a *= m.x * m.y;
                #endif
                
                #ifdef UNITY_UI_ALPHACLIP
                clip (col.a - 0.001);
                #endif
                
                return col;
            }
            ENDCG
        }
    }
}
