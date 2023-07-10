Shader "Unlit/GameField"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        [NoScaleOffset] _MainTex ("Main", 2D) = "white" {}
        _MainTile("Main Tile", float) = 1

        [Space(8)]
        [NoScaleOffset] _Checker("Checker", 2D) = "gray"{}
        
        _CountX("Count X", Range(3, 9)) = 9
        _CountY("Count Y", Range(3, 9)) = 9

        [Space(8)]
        _Mid("Mid", Range(0, 1)) = .5
        _Soft("Soft", Range(0, 1)) = 1
        _Power("Power", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

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
                float2 uvChecker : TEXCOORD1;
            };

            fixed4 _Color;
            sampler2D_half _MainTex;
            fixed _MainTile;

            sampler2D_half _Checker;
            uint _CountX;
            uint _CountY;
            fixed _Mid;
            fixed _Soft;
            fixed _Power;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * _MainTile;

                o.uvChecker = v.uv * half2(_CountX, _CountY) / 2;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb = lerp(col.rgb, _Color.rgb, _Color.a);

                _Soft /= 2;
                fixed checker = tex2D(_Checker, i.uvChecker).r;
                checker *= col.a + checker;
                checker = smoothstep(_Mid - _Soft, _Mid + _Soft, checker);
                checker = (checker - .5) * _Power + 1;

                col.rgb *= checker;

                // col.rgb = checker;

                return col;
            }
            ENDCG
        }
    }
}
