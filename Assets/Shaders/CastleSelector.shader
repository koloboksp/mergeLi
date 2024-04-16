Shader "Calc/CastleSelector"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        _Step("Step", Range(0, 1)) = 0 
        _Range("Range", Range(0,1)) = 0
    }

    SubShader
    {
        Tags
        {
            "PreviewType"="Plane"
        }
            
        Pass
        {
            ColorMask R
            
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Step;
            float _Range;

            fixed frag(v2f_img i) : SV_Target
            {
                return step(abs(tex2D(_MainTex, i.uv).r - _Step), _Range);
            }

            ENDCG
        }
    }
}
