Shader "Custom/SkyboxGradient"
{
    Properties
    {
        _HorizonColor ("Horizon Color", Color) = (0.37, 0.2, 0.2, 1)
        _ZenithColor ("Zenith Color", Color) = (0.55, 0.35, 0.3, 1)
        _GradientPower ("Gradient Power", Range(0.5, 8)) = 1.2
        [HDR] _SunColor ("Sun Color", Color) = (4, 3.2, 2.4, 1)
        _SunSize ("Sun Size (degrees)", Range(0.1, 15)) = 2.5
        _SunLatitude ("Sun Latitude", Range(-90, 90)) = 50
        _SunLongitude ("Sun Longitude", Range(0, 360)) = 150
        _CloudColor ("Cloud Color", Color) = (0.55, 0.38, 0.35, 1)
        _CloudStrength ("Cloud Strength", Range(0, 1)) = 0.2
        _CloudScale ("Cloud Scale", Range(0.2, 8)) = 2
        _CloudSpeed ("Cloud Speed", Range(0, 0.1)) = 0.015
        _CloudThreshold ("Cloud Threshold", Range(0, 1)) = 0.55
        _CloudSoftness ("Cloud Softness", Range(0.01, 0.5)) = 0.12
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
            fixed4 _CloudColor;
            float _CloudStrength;
            float _CloudScale;
            float _CloudSpeed;
            float _CloudThreshold;
            float _CloudSoftness;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            float Hash21(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            float ValueNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);

                float a = Hash21(i);
                float b = Hash21(i + float2(1.0, 0.0));
                float c = Hash21(i + float2(0.0, 1.0));
                float d = Hash21(i + float2(1.0, 1.0));

                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }

            float CloudFractionalBrownianMotion(float2 p)
            {
                float value = 0.0;
                float amplitude = 0.5;
                [unroll]
                for (int octave = 0; octave < 4; octave++)
                {
                    value += amplitude * ValueNoise(p);
                    p = p * 2.03 + 17.0;
                    amplitude *= 0.5;
                }
                return value;
            }

            float SampleClouds(float3 viewDir, float time)
            {
                float2 scroll = float2(time * _CloudSpeed, time * _CloudSpeed * 0.65);
                float2 uvA = viewDir.xz * _CloudScale + scroll;
                float2 uvB = (viewDir.xy + viewDir.zx * 0.35) * _CloudScale * 0.85 - scroll * 0.7;
                float noise = CloudFractionalBrownianMotion(uvA) * 0.6 + CloudFractionalBrownianMotion(uvB) * 0.4;
                return smoothstep(
                    _CloudThreshold - _CloudSoftness,
                    _CloudThreshold + _CloudSoftness,
                    noise);
            }

            float CloudHeightMask(float viewY)
            {
                float aboveHorizon = smoothstep(0.08, 0.35, viewY);
                float belowZenith = 1.0 - smoothstep(0.65, 0.92, viewY);
                return aboveHorizon * belowZenith;
            }

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

                float cloudMask = SampleClouds(viewDir, _Time.y)
                    * CloudHeightMask(height)
                    * _CloudStrength
                    * _CloudColor.a;
                sky = lerp(sky, _CloudColor.rgb, cloudMask);

                float3 sunDir = SunDirectionFromLatLong(_SunLatitude, _SunLongitude);
                float sunDot = saturate(dot(viewDir, sunDir));
                float sunHalfAngle = _SunSize * PI / 360.0;
                float cosHalfAngle = cos(sunHalfAngle);
                float sunMask = smoothstep(cosHalfAngle, 1.0, sunDot) * _SunColor.a;
                sky = lerp(sky, _SunColor.rgb, sunMask);

                return fixed4(sky, 1);
            }
            ENDCG
        }
    }
}
