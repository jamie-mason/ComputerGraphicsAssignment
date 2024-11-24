Shader "Custom/RimLightingShader"
{
    Properties
    {
        _RimColour ("Rim Colour", Color) = (1, 0, 0, 1) 
        _RimPower ("Rim Power", Range(0.1, 10.0)) = 2.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 200
        Pass
        {
            Blend SrcAlpha One

            CGPROGRAM
            #pragma fragment frag
            #pragma vertex vert
            
            uniform float3 UnityWorldSpaceCameraPos;
            uniform float4 _RimColour;
            float _RimPower;


            struct Vin{
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 color : COLOR; 

            };

            struct Vout{
                float4 pos : SV_POSITION;
                float3 worldNormal: TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 viewDir : TEXCOORD2;


            };
            

            Vout vert (Vin v){

                Vout o;
                o.pos = UnityObjectToClipPos(v.vertex); // Transform vertex position to clip space
                o.worldNormal = normalize(mul(v.normal, (float3x3)unity_ObjectToWorld)); // Transform normal to world space
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(_WorldSpaceCameraPos - o.worldPos);

                return o; // Return output to fragment shader


            }
            float4 frag(Vout i) : SV_Target
            {
                
                float rimFactor = 1.0 - saturate(dot(i.worldNormal, i.viewDir));
                rimFactor = pow(rimFactor, _RimPower);

                float4 rimLighting = _RimColour * rimFactor;
                return rimLighting;
            

            }


            ENDCG
        }
    }
    FallBack "Diffuse"
}
