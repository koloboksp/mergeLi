Shader "Calc/MultAlpha"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags {}

        Pass
        {
            Blend DstColor Zero // Mult 
            
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;

            fixed4 frag(v2f_img i) : SV_Target
            {
                return tex2D(_MainTex, i.uv).a;
            }
            ENDCG
        }
    }
}
