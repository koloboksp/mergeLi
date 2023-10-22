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

        [Space(10)]
        _Glow("Glow", Range(0, 1)) = .5
        _Gray("Gray", Range(0, 1)) = .5 
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "PreviewType"="Plane"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            
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
            fixed _Gray;

            static const fixed _BlurCore[9] = { 1,2,1,2,4,2,1,2,1 };
            static const fixed2 _BlurUV[9] = {
                fixed2(-1, 1), fixed2(0,1),  fixed2(1,1),
                fixed2(-1,0),  fixed2(0,0),  fixed2(1,0),
                fixed2(-1,-1), fixed2(0,-1), fixed2(1,-1) };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            fixed2 SoftRange(fixed size, fixed soft, fixed val)
            {
                fixed2 r = fixed2(lerp(-soft, 1, size), lerp(0, 1 + soft, size));
                return smoothstep(r.x, r.y, val);
            }

            float RangeLineBold(float base, float sec, float bold) {
                float amp = (1.0 - abs(base * 2.0 - 1.0)) * bold;
                return (sec - .5) * amp + base;
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

                // float sumStage = 0;
                // float sumLoad = 0;
                // float cur = 0;
                // for (int j = 0; j < 9; j++)
                // {
                //     cur = _Mask.Sample(_Mask_Point_Clamp_Sampler, i.uv + _BlurUV[j] * _Mask_TexelSize.xy).r;
                //     sumStage += step(abs(stage - cur), .02) * _BlurCore[j];
                //     sumLoad += smoothstep(stage - .01, stage + .01, cur) * _BlurCore[j];
                // }

                // col.a = 1;
                // col.a = sumLoad / 16.0;
                // return col;

                // Clip by opened stages
                fixed loadMask = step(stage, mask.r);
                // loadMask = smoothstep(stage, stage + .05, mask.r);
                // loadMask = sumLoad / 16.0;
                clip(loadMask - .01);

                // Current stage mask
                fixed stageMask = step(abs(stage - mask.r), .02);
                // stageMask = sumStage / 16.0;

                // apply bar color
                fixed ringBorn = SoftRange(1 - _BarBorn, .2, mask.b);
                clip(lerp(1, ringBorn, stageMask) - .01);
                
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
                col.rgb = lerp(col.rgb, gray, _Gray);

                // col.rgb = loadMask;

                return col;
            }
            ENDCG
        }
    }
}
