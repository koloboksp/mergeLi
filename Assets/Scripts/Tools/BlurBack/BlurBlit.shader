Shader "Hidden/DropBlurBlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            static const fixed3 _BlurCore[9] = {
                fixed3(-1,1,1),  fixed3(0,1,2),  fixed3(1,1,1),
                fixed3(-1,0,2),  fixed3(0,0,4),  fixed3(1,0,2),
                fixed3(-1,-1,1), fixed3(0,-1,2), fixed3(1,-1,1) };

            fixed4 frag (v2f_img i) : SV_Target
            {
                float4 sum = 0;

                for (int j = 0; j < 9; j++)
                    sum += tex2D(_MainTex, i.uv + _MainTex_TexelSize.xy * _BlurCore[j].xy) * _BlurCore[j].z;

                sum.a = 16.0;

                return sum / 16.0;
            }

            ENDCG
        }
    }
}
