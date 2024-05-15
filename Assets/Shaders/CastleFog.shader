Shader "Unlit/CastleFog"
{
    Properties
    {
        _FlipSpeed("Flip Speed", float) = 1
        _BackColor("Back Color", Color) = (1,1,1,1)
        [NoScaleOffset] _Back("Back", 2D) = "gray"{}

        [NoScaleOffset] _DistMap("Distort", 2D) = "gray"{}
        _DistSize("Dist Size", float) = 1
        _DistSpeed("Dist Speed", float) = 0
        _DistPower("Dist Power", float) = 0
        
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
            "RenderType"="Transparent" 
            "Queue"="Transparent"
            "PreviewType"="Plane"
        }

        Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
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
                float2 uvDist : TEXCOORD1;
                half4  mask : TEXCOORD2;
                float2 suv : TEXCOORD3;
            };


            // Cloud Background
            fixed _FlipSpeed;
            sampler2D _Back;
            fixed4  _BackColor;

            sampler2D _DistMap;
            float _DistSize;
            float _DistSpeed;
            float _DistPower;
            float4 _ClipRect;
            float _UIMaskSoftnessX;
            float _UIMaskSoftnessY;

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);

                float2 wPos = mul(unity_ObjectToWorld, v.vertex).xy;

                o.uv = v.uv;
                o.uvDist = wPos / _DistSize + _DistSpeed * _Time.x * float2(0, 1);

                float4 screenPos = ComputeScreenPos(o.vertex);	//w = depth	
                o.suv = screenPos.xy / screenPos.w;

                // UI working
                float2 pixelSize = o.vertex.w;
                pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));
                float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                float2 maskUV = (v.vertex.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);
                
                o.mask = half4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));

                

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = 1;
                
                float2 dist = (tex2D(_DistMap, i.uvDist).rg - .5) * _DistPower;
                fixed4 back = tex2D(_Back, i.uv + dist);

                float4 phase = frac(i.suv.y - i.suv.x + _Time.x * _FlipSpeed + float4(0, .25, .5, .75));
                phase = saturate(1 - abs(phase * 4 - 2));

                float dotPhase = dot(back, phase);
                col.rgb = dotPhase + _BackColor.rgb * _BackColor.a;
                col.a = smoothstep(0, .3, dotPhase);


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
