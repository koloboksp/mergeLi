Shader "Unlit/CastleShow"
{
    Properties
    {
        [PerRendererData][NoScaleOffset] _MainTex ("Main", 2D) = "white" {}
        [NoScaleOffset] _Mask("Mask", 2D) = "white"{}

        _Color("Color", Color) = (0,1,0,0)
        _Fill("Fill", Range(0,1)) = .5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}

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
                float2 locpos : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _Mask;
            float4 _Mask_ST;

            fixed4 _Color;
            fixed _Fill;

            v2f vert(appdata v)
            {
                v2f o;

                float3 wPos = mul(unity_ObjectToWorld, v.vertex);
                
                // v.vertex.x /= 2;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv; // TRANSFORM_TEX(v.uv, _MainTex);

                o.locpos = v.vertex.xy;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                float2 uvMask = (i.uv - _Mask_ST.zw) * _Mask_ST.xy;
                fixed3 mask = tex2D(_Mask, uvMask);
                col.a *= smoothstep(.2, .4, mask);

                // Fill Power

                col.rgb = lerp(col.rgb, _Color.rgb, step(mask.g, _Fill) * _Color.a);

                // BorderColor
                col.rgb = lerp(col.rgb, _Color.rgb, (1 - mask.r) * _Color.a * 2);

                
                // col.a = 1;
                // col.rgb = 0;
                // col.rg = i.locpos;

                return col;
            }
            ENDCG
        }
    }
}
