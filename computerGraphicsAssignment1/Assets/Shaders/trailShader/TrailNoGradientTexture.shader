Shader "Custom/BasketballTrailNoGradient"
{
    Properties
    {
        _MainColor ("Trail Color", Color) = (1, 0.5, 0, 1) // Orange
        _Opacity ("Opacity", Range(0, 1)) = 1.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            fixed4 _MainColor;
            float _Opacity;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Fade effect along the trail's length
                float fade = 1.0 - i.uv.x; // UV.x determines length along the trail
                fixed4 color = _MainColor;
                color.a *= fade * _Opacity; // Apply fade to alpha
                return color;
            }
            ENDCG
        }
    }
}
