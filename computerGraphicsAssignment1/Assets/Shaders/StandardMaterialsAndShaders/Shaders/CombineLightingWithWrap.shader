Shader "Custom/CombineLightingWithWrap"
{
    Properties
    {
        _Color("Colour", Color) = (1, 1, 1, 1) // Base color of the object
        _SpecColor("Specular Colour", Color) = (1, 1, 1, 1) // Specular color
        _Shininess("Shininess", Range(1, 100)) = 10 // Shininess factor for specular highlights
        _MainTex("Main Texture", 2D) = "white" {} // Base texture of the object
        _BumpMap("Bump Texture", 2D) = "bump" {} // Bump texture property
        _BumpScale("Scale", Float) = 1.000000 // Modifies the scale of the bump effect
        _ToggleAmbient("Toggle Ambient", Range(0, 1)) = 1 // Toggle for ambient lighting
        _ToggleSpecular("Toggle Specular", Range(0, 1)) = 1 // Toggle for specular lighting
        _ToggleDiffuse("Toggle Diffuse", Range(0, 1)) = 1 // Toggle for diffuse lighting
        _ToggleDiffuseWrap("Toggle Diffuse Wrap", Range(0, 1)) = 1.0 // Toggle for diffuse wrap effect
    }
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        // First Pass for Rendering
        Pass
        {
            Tags { "LightMode" = "ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc" // Required for Unity functions
            #include "UnityLightingCommon.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            // Properties
            sampler2D _MainTex; // Main texture
            sampler2D _BumpMap; // Normal map
            float _BumpScale; // Scale for normal map
            uniform float4 _Color; // Base color
            uniform float _Shininess; // Shininess factor
            uniform float _ToggleAmbient; // Toggle for ambient lighting
            uniform float _ToggleSpecular; // Toggle for specular lighting
            uniform float _ToggleDiffuse; // Toggle for diffuse lighting
            uniform float _ToggleDiffuseWrap; // Toggle for diffuse wrap effect

            // Input structure for vertex shader
            struct Vin
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            // Output structure for vertex shader
            struct Vout
            {
                float4 pos : SV_POSITION;
                fixed4 diff: Color0;
                float3 normal : TEXCOORD1;
                float3 lightDir : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
                float2 uv : TEXCOORD0;
                SHADOW_COORDS(1)
            };

            // Vertex shader
             Vout vert(Vin v)
            {
                Vout o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                // Transform the normal to world space

                o.normal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));

                // Get light and view direction
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.lightDir = normalize(_WorldSpaceLightPos0.xyz - worldPos);
                o.viewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = nl * _LightColor0;
                TRANSFER_SHADOW(o);
                return o;
            }

            // Fragment shader
            float4 frag(Vout i) : SV_Target
            {
                float4 finalColor = tex2D(_MainTex, i.uv); // Sample texture
                float4 TexColour = finalColor * _Color;
                float4 diffuseColour;

                float4 normalMapSample = tex2D(_BumpMap, i.uv) * 2.0 - 1.0; 
                normalMapSample.xy *= _BumpScale; // Scale the normal map

                float3 normal = normalize(i.normal + float3(normalMapSample.xy, 0.0)); // Add the normal map to the interpolated normal
                normal = normalize(normal); // Normalize the final normal


                float diff = 0.0;
                float wrapDiff = 0.0;


                // Check if diffuse lighting is enabled
                if (_ToggleDiffuse > 0.5)
                {
                    diff = max(0.0, dot(i.normal, i.lightDir));  

                    fixed shadow = SHADOW_ATTENUATION(i);
                    finalColor.rgb *= _LightColor0.xyz * _Color.rgb * shadow; // Apply diffuse lighting
                }
                
                if (_ToggleDiffuseWrap > 0.5)   //toggle diffuse wrap
                {
                    float wrapFactor = 0.5; // A value to control how the lighting wraps around the object
                    diff = (diff + wrapFactor) / (1.0 + wrapFactor); // Apply wrapping effect
                  
                }

                finalColor.rgb *= diff;

                // Ambient lighting
                if (_ToggleAmbient > 0.5)
                {
                    finalColor.rgb += TexColour.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb; //Apply ambient lighting only if toggled on.
                }

                // Specular lighting
                if (_ToggleSpecular > 0.5) // Toggle specular lighting if toggled on
 {
     float3 reflectDir = reflect(-i.lightDir, normalize(normal));
     float spec = pow(max(dot(reflectDir, i.viewDir), 0.0), _Shininess);
     finalColor.rgb += _SpecColor.rgb * spec;
 }

                return finalColor;
            }
           
            ENDCG
        }
        Pass
        {
            Tags {"LightMode"="ShadowCaster"}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"

            struct v2f {
                V2F_SHADOW_CASTER;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                //object space
                //v.vertex.z += 1000.;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
