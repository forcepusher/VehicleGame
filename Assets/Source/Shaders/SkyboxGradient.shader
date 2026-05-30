Shader "Custom/SkyboxGradient"
{
    Properties
    {
        _HorizonColor ("Horizon Color", Color) = (0.37, 0.2, 0.2, 1)
        _ZenithColor ("Zenith Color", Color) = (0.55, 0.35, 0.3, 1)
        _GradientPower ("Gradient Power", Range(0.5, 8)) = 3
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
            Cull Front
            Fog { Mode Off }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _HorizonColor;
            fixed4 _ZenithColor;
            float _GradientPower;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float height : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                float3 dir = normalize(v.vertex.xyz);
                o.height = dir.y * 0.5 + 0.5;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float t = pow(saturate(i.height), _GradientPower);
                return lerp(_HorizonColor, _ZenithColor, t);
            }
            ENDCG
        }
    }
}
