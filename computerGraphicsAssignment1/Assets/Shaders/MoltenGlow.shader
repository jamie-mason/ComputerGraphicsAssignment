Shader "Custom/MoltenGlow"
{
    Properties
    {
        _BaseColor ("Color", Color) = (1,1,1,1)
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"
        
        float4 _BaseColor;

        struct Vin
        {
            float2 uv : TEXCOORD0;
            float4 vertex : POSITION
            float4 normal : NORMAL
        };
        struct Vout
        {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
        };

        Vout vert(Vin i){
            Vout o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv = i.uv;

            return o;
        }

        float4 frag(Vout o){


        }

        ENDCG
    }
    FallBack "Diffuse"
}
