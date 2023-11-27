Shader "Unlit/Castle"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        [NoScaleOffset] _Mask("Stage Mask", 2D) = "black"{}
        [IntRange] _Stage("Stage", Range(0,31)) = 0


        [Space(10)]
        _BarColor("Bar Color", Color) = (0,1,0,1)
        _GlowColor("Glow Color", Color) = (1,1,0,1)

        [Space(10)]
        _BarBorn("Bar Born", Range(0, 1)) = .5
        _BarLoad("Bar Load", Range(0, 1)) = .5
        _BarOver("Bar Over", Range(0, 1)) = .5

       
        _Glow("Glow", Range(0, 1)) = .5
        // _Gray("Gray", Range(0, 1)) = .5 

        [Space(10)]
        [NoScaleOffset] _BackMask("Back Mask", 2D) = "white"{}
        [NoScaleOffset] _BackTex("Back Tex", 2D) = "black"{}
        _BackSize("Back Size", float) = 1
        _BackSpeed("Back Speed", float) = 0
        _BackColor("Border Color", Color) = (1,1,1,1)
        
        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255

        _ColorMask("Color Mask", Float) = 15

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
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 uvBack : TEXCOORD1;
                half4  mask : TEXCOORD2;
            };

            sampler2D_half _MainTex;
            
            // twice sample mask - point for stages, linear for others
            Texture2D _Mask;
            float4 _Mask_TexelSize;
            SamplerState _Mask_Point_Clamp_Sampler;
            SamplerState _Mask_Linear_Clamp_Sampler;

            uint _Stage;

            fixed4 _BarColor;
            fixed4 _GlowColor;

            fixed _BarBorn;
            fixed _BarLoad;
            fixed _BarOver;

            fixed _Glow;
            // fixed _Gray;

            sampler2D_half _BackMask;
            sampler2D_half _BackTex;
            fixed4 _BackColor;
            fixed _BackSize;
            fixed _BackSpeed;
            float4 _ClipRect;
            float _UIMaskSoftnessX;
            float _UIMaskSoftnessY;


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                float3 wPos = mul(unity_ObjectToWorld, v.vertex);
                o.uvBack.xy = wPos.xy / _BackSize + _BackSpeed * _Time.x;
                o.uvBack.zw = wPos.xy / _BackSize * 1.37 + float2(-1, 1) * _BackSpeed * 1.37 * _Time.x;

                float2 pixelSize = o.vertex.w;
                pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));
                float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                float2 maskUV = (v.vertex.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);
                
                o.mask = half4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));

                
                return o;
            }

            fixed2 SoftRange(fixed size, fixed soft, fixed val)
            {
                fixed2 r = fixed2(lerp(-soft, 1, size), lerp(0, 1 + soft, size));
                return smoothstep(r.x, r.y, val);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = 1;
                fixed4 main = tex2D(_MainTex, i.uv);
                fixed gray = (main.x + main.y + main.z) / 20 + .1;
                col.a = main.a;

                fixed4 mask;
                mask.r = _Mask.Sample(_Mask_Point_Clamp_Sampler, i.uv).r;
                mask.gba = _Mask.Sample(_Mask_Linear_Clamp_Sampler, i.uv).gba;
                fixed stage = 1.0 - (8.0 * _Stage) / 255.0;


                // Clip by opened stages
                fixed loadMask = step(stage, mask.r);
                // clip(loadMask - .01);

                // Current stage mask
                fixed stageMask = step(abs(stage - mask.r), .02);

                // apply bar color
                fixed ringBorn = SoftRange(1 - _BarBorn, .2, mask.b);
                ringBorn = lerp(1, ringBorn, stageMask);
                // clip(lerp(ringBorn - .01);

                fixed barColorMask = 1 - ringBorn * smoothstep(1, .5, mask.a) * step(_BarLoad, mask.g);
                col.rgb = lerp(gray, _BarColor.rgb, barColorMask);

                // apply bar over
                fixed ringOver = SoftRange(1 - _BarOver, .1, mask.b);
                col.rgb = lerp(col.rgb, main.rgb, ringOver);
                
                // apply local blink
                col.rgb += _Glow * _GlowColor.rgb * _GlowColor.a;

                // mix with open part
                col.rgb = lerp(main.rgb, col.rgb, stageMask);

                // global blink
                if (_Stage == 31)
                    col.rgb += _Glow * _GlowColor.rgb * _GlowColor.a;

                // dark after using
                // col.rgb = lerp(col.rgb, gray, _Gray);

                // sample back
                fixed3 backTex = tex2D(_BackTex, i.uvBack.xy);
                fixed3 backTex2 = tex2D(_BackTex, i.uvBack.zw);
                backTex = max(backTex, backTex2); // lerp(backTex, backTex2, .4);
                
                fixed backMask = tex2D(_BackMask, i.uv).r;
                backTex = lerp(backTex, _BackColor.rgb * 2, backMask.r * _BackColor.a); 
                col.rgb = lerp(backTex, col.rgb, loadMask);
#ifdef UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
                col.a *= m.x * m.y;
                col.a *= m.x * m.y;
#endif
                return col;
            }
            ENDCG
        }
    }
}
