Shader "Custom/BakedTerrain"
{
    Properties
    {
        [MainTexture] _MainTex ("Baked Diffuse", 2D) = "white" {}

        _DetailAlbedoMap ("Detail Albedo", 2D) = "gray" {}
        _DetailStrength ("Detail Strength", Range(0, 2)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }

        LOD 200

        Pass
        {
            Tags { "LightMode" = "ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _DetailAlbedoMap;
            float4 _DetailAlbedoMap_ST;
            half _DetailStrength;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uvMain : TEXCOORD0;
                float2 uvDetail : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                UNITY_FOG_COORDS(3)
                UNITY_SHADOW_COORDS(4)
            };

            half3 BlendDetailAlbedo(half3 albedo, half3 detail)
            {
                // Same multiply-x2 detail blend as Unity Standard shader.
                half3 detailScaled = detail * unity_ColorSpaceDouble.rgb;
                return lerp(albedo, albedo * detailScaled, _DetailStrength);
            }

            half3 BlendRealtimeShadow(half3 bakedColor, half shadowAttenuation)
            {
                half3 shadowTint = bakedColor * unity_ShadowColor.rgb;
                return lerp(shadowTint, bakedColor, shadowAttenuation);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uvMain = TRANSFORM_TEX(v.uv, _MainTex);
                o.uvDetail = TRANSFORM_TEX(v.uv, _DetailAlbedoMap);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                UNITY_TRANSFER_SHADOW(o, o.uvMain);
                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 baked = tex2D(_MainTex, i.uvMain);
                half3 detail = tex2D(_DetailAlbedoMap, i.uvDetail).rgb;
                baked.rgb = BlendDetailAlbedo(baked.rgb, detail);

                half shadow = UNITY_SHADOW_ATTENUATION(i, i.worldPos);
                baked.rgb = BlendRealtimeShadow(baked.rgb, shadow);

                UNITY_APPLY_FOG(i.fogCoord, baked);
                return baked;
            }
            ENDCG
        }
    }

    FallBack Off
}
