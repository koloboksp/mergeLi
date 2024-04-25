Shader "Unlit/CastleBit"
{
    Properties
    {
        _MainTex ("Main", 2D) = "white" {}
        [NoScaleOffset] _Mask("Mask", 2D) = "white"{}

        _Color("Color", Color) = (0,1,0,0)

        _Alpha("Alpha", Range(0, 1)) = 1
        _Gray("Gray", Range(0, 1)) = 0
            
        _Level("Level", Range(0, 1)) = .5
        _Ring("Ring", Range(0, 1)) = .5
        _Glow("Glow", Range(0, 1)) = 0
        _Scale("Scale", Range(0, 1)) = 0
        _Border("Border", Range(0, 1)) = 1

        [Space(10)]
        _BackColor("Back Color", Color) = (1,1,1,1)
        [NoScaleOffset] _Back("Back", 2D) = "gray"{}
        [NoScaleOffset] _DistMap("Distort", 2D) = "gray"{}
        _DistSize("Dist Size", float) = 1
        _DistSpeed("Dist Speed", float) = 0
        _DistPower("Dist Power", float) = 0
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
                float4 uv : TEXCOORD0;
                float2 uvDist : TEXCOORD1;
            };


            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _Mask;

            float2 _Pivot; // center of bit in world space 

            fixed4 _Color;
            fixed _Level;
            fixed _Ring;
            fixed _Gray;
            fixed _Alpha;
            fixed _Glow;
            fixed _Scale;
            fixed _Border;

            // Cloud Background
            sampler2D _Back;
            fixed4  _BackColor;

            sampler2D _DistMap;
            float _DistSize;
            float _DistSpeed;
            float _DistPower;

            v2f vert(appdata v)
            {
                v2f o;

                // Scale around setted _Pivot
                float4 wPos = mul(unity_ObjectToWorld, v.vertex);
                wPos.xy = (wPos.xy - _Pivot.xy) * (_Scale / 10 + 1) + _Pivot.xy;
                o.vertex = UnityWorldToClipPos(wPos);

                // o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv.zw = v.uv; // for mask

                o.uvDist = wPos.xy / _DistSize + _DistSpeed * _Time.x * float2(0, 1);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv.xy);
                fixed gray = (col.r + col.g + col.b) / 6;
                col.rgb = lerp(col.rgb, gray, _Gray);
                
                // Glow
                fixed glow = i.uv.w + 1 - 2 * _Glow;
                glow = 1 - abs(glow * 2 - 1);
                glow = smoothstep(.2, 1, glow);
                col.rgb += glow * _Color.rgb * .75;

                fixed mask = tex2D(_Mask, i.uv.zw).r;
                col.a *= smoothstep(.2, .4, mask); // *_Alpha;

                fixed borderMask = (1 - mask) * 2 * _Border;
                fixed levelMask = step(i.uv.w, _Level);
                
                fixed2 ringMask = abs(i.uv.zw * 2 - 1);
                ringMask = 1 - ringMask * ringMask;
                ringMask.x = step(ringMask.x * ringMask.y, _Ring); 
                
                fixed commonMask = saturate(borderMask + levelMask + ringMask.x);
                fixed3 byMaskColor = _Color.rgb * (gray + .75);
                col.rgb = lerp(col.rgb, byMaskColor, commonMask * _Color.a);

                
                float2 dist = (tex2D(_DistMap, i.uvDist).rg - .5) * _DistPower;
                mask = tex2D(_Mask, i.uv.zw + dist).r;
                fixed4 back = tex2D(_Back, i.uv.xy + dist);
                // back.rgb = lerp((back.r + back.g + back.b) / 3, _BackColor.rgb, _BackColor.a);
                
                back.rgb = (back.r + back.g + back.b) / 3 + _BackColor.rgb * _BackColor.a;



                back.a *= mask;
                col = lerp(back, col, _Alpha);

                return col;
            }
            ENDCG
        }
    }
}
