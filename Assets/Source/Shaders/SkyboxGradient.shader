Shader "Custom/SkyboxGradient"
{
    Properties
    {
        _HorizonColor ("Horizon Color", Color) = (0.37, 0.2, 0.2, 1)
        _ZenithColor ("Zenith Color", Color) = (0.55, 0.35, 0.3, 1)
        _GradientPower ("Gradient Power", Range(0.5, 8)) = 1.2
        [HDR] _SunColor ("Sun Color", Color) = (4, 3.2, 2.4, 1)
        _SunSize ("Sun Size", Range(64, 4096)) = 256
        _SunLatitude ("Sun Latitude", Range(-90, 90)) = 50
        _SunLongitude ("Sun Longitude", Range(0, 360)) = 150
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Background"
            "Queue" = "Background"
            "PreviewType" = "Skybox"
        }

        Pass
        {
            ZWrite Off
            ZTest LEqual
            Cull Off
            Fog { Mode Off }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            #define PI 3.14159265359

            fixed4 _HorizonColor;
            fixed4 _ZenithColor;
            float _GradientPower;
            fixed4 _SunColor;
            float _SunSize;
            float _SunLatitude;
            float _SunLongitude;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            float3 SunDirectionFromLatLong(float latitude, float longitude)
            {
                float lat = latitude * PI / 180.0;
                float lon = longitude * PI / 180.0;
                float cosLat = cos(lat);
                return float3(cosLat * sin(lon), sin(lat), cosLat * cos(lon));
            }

            v2f vert(appdata v)
            {
                v2f o;
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = worldPos.xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 viewDir = normalize(i.worldPos - _WorldSpaceCameraPos.xyz);
                float height = saturate(viewDir.y);
                float t = pow(height, _GradientPower);
                float3 sky = lerp(_HorizonColor.rgb, _ZenithColor.rgb, t);

                float3 sunDir = SunDirectionFromLatLong(_SunLatitude, _SunLongitude);
                float sunDot = saturate(dot(viewDir, sunDir));
                float sunMask = pow(sunDot, _SunSize) * _SunColor.a;
                sky = lerp(sky, _SunColor.rgb, sunMask);

                return fixed4(sky, 1);
            }
            ENDCG
        }
    }
}
