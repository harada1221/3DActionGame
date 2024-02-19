Shader "Custom/ColorTextureCoord"
{
Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _UpperColor ("Upper Half Color", Color) = (0,0,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        fixed4 _Color;
        fixed4 _UpperColor;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float3 worldNormal;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            // 法線のY成分が0以上の場合、上半分の色を適用
            if (IN.worldNormal.y >= 0)
                o.Albedo = _UpperColor;
            else
                o.Albedo = _Color;
        }
        ENDCG
    }
    FallBack "Diffuse"
}