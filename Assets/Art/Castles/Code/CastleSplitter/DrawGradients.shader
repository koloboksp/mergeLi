Shader "Calc/DrawGradients"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags {"PreviewType"="Plane"}

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            fixed4 frag (v2f_img i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.g = i.uv.y;

                float2 m = 1 - abs(i.uv * 2 - 1);
                col.b = m.x * m.y;

                return col;
            }
            ENDCG
        }
    }
}
