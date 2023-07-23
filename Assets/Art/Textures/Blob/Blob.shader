Shader "Unlit/Blob"
{
    Properties
    {
        [PerRendererData][HideInInspector] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
            "DisableBatching"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color  : COLOR;
                float2 uv   : TEXCOORD0;
                float2 wPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D_half _RampMap;

            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.wPos = mul(unity_ObjectToWorld, v.vertex).xy;
                o.uv = v.uv;
                o.color = v.color * _Color;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half4 col = (tex2D(_MainTex, i.uv) + _TextureSampleAdd);
                col.r = smoothstep(0, 1, col.r);
                col.rgb = lerp(i.color, 1, col.r);
                // col.rgb *= i.color * 2;

                #ifdef UNITY_UI_CLIP_RECT
                col.a *= UnityGet2DClipping(i.wPos, _ClipRect);
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