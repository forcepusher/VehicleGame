Shader "RealEngine/MegascamSurface"
{
    Properties
    {
        // Level 1 - Color + AO. Barely acceptable potato. The lightmap alone gets the job done.
        _MainTex ("Base Color (RGB) + AO*Cavity (A)", 2D) = "white" {}
        
        // Level 2 - Normals + PBR Specular. Looks good, about 20% performance hit from potato.
        _SpecGlossMap ("Specular (RGB) + Glossiness (A)", 2D) = "black" {}
        _NormalMap ("Normal Map (RGB)", 2D) = "bump" {}

        // Level 3 - Parallax shadows and displacement. Looks pretty much same as Unreal Engine.
        _DisplacementMap ("Displacement Map", 2D) = "black" {}

        // Self-shadowing properties (Quality 3 only)
        _SelfShadowDisplacementScale ("SelfShadowing Displacement Scale", Range(0, 1.0)) = 0.05
        _ShadowIntensity ("Shadow Intensity", Range(0, 1)) = 1.0
        _ShadowDistance ("Shadow Distance", Range(0.001, 0.05)) = 0.01
        _ShadowBias ("Shadow Bias", Range(0.001, 0.02)) = 0.003
        _DistanceFalloffScale ("Distance Falloff Scale", Range(0, 1)) = 0.0

        // Parallax Occlusion Mapping properties (Quality 3 only)
        _MaxDisplacementMeters ("Max Displacement (meters)", Range(0.0, 1.0)) = 0.005
        _ParallaxDepthDisplacement ("Parallax Depth Displacement", Range(0.0, 1.0)) = 1.0
        _ParallaxCounterDisplacement ("Parallax Counter Displacement", Range(0.0, 1.0)) = 0.0
        _MetersPerUntiledUV ("Meters Per Untiled UV", Float) = 1.0
        _ObjectScale ("Object Scale", Float) = 1.0
        _ParallaxLayers ("Parallax Layers", Range(4, 64)) = 16
        _ParallaxAngleAntiArtifact ("Parallax Angle Anti Artifact", Range(0.0, 1.0)) = 0.0

        // Quality control
        [KeywordEnum(Level1, Level2, Level3)] _Quality ("Quality Level", Float) = 0
        
        // Final color adjustment
        _BrightnessMultiplier ("Brightness Multiplier", Range(0.1, 5.0)) = 1.0
        _SpecularMultiplier ("Specular Multiplier", Range(0.0, 10.0)) = 1.0
        _GlossinessMultiplier ("Glossiness Multiplier", Range(0.0, 10.0)) = 1.0
        
        // Debug options
        [Toggle] _DebugVertexColor ("Debug Vertex Color", Float) = 0
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "LightMode"="ForwardBase" }
        LOD 200
        
        // Main Pass
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _QUALITY_LEVEL1 _QUALITY_LEVEL2 _QUALITY_LEVEL3
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma target 3.0
            
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            
            // Textures
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NormalMap;
            float4 _NormalMap_ST;
            sampler2D _SpecGlossMap;
            float4 _SpecGlossMap_ST;
            
            #ifdef _QUALITY_LEVEL3
            sampler2D _DisplacementMap;
            float4 _DisplacementMap_ST;
            float4 _DisplacementMap_TexelSize;
            
            // Self-shadowing properties (Level 3 only)
            float _SelfShadowDisplacementScale;
            float _ShadowIntensity;
            float _ShadowDistance;
            float _ShadowBias;
            float _DistanceFalloffScale;
            
            // Parallax Occlusion Mapping properties (Level 3 only)
            float _MaxDisplacementMeters;
            float _ParallaxDepthDisplacement;
            float _ParallaxCounterDisplacement;
            float _MetersPerUntiledUV;
            float _ObjectScale;
            float _ParallaxLayers;
            float _ParallaxAngleAntiArtifact;
            #endif
            
            // Properties
            float _BrightnessMultiplier;
            float _SpecularMultiplier;
            float _GlossinessMultiplier;
            float _DebugVertexColor;

            struct VertexInput
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 color : COLOR;
                #ifdef LIGHTMAP_ON
                float2 lightmapUV : TEXCOORD1;
                #endif
            };
            
            struct VertexOutput
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldTangent : TEXCOORD2;
                float3 worldBitangent : TEXCOORD3;
                float3 worldPos : TEXCOORD4;
                float4 vertexColor : COLOR;
                #ifdef _QUALITY_LEVEL3
                float3 viewDirTangent : TEXCOORD5;
                float3 lightDir : TEXCOORD6;
                #endif
                #ifdef LIGHTMAP_ON
                float2 lightmapUV : TEXCOORD7;
                #endif
                SHADOW_COORDS(9)
                UNITY_FOG_COORDS(8)
            };
            
            #ifdef _QUALITY_LEVEL3
            struct FragmentOutput
            {
                fixed4 color : SV_Target;
                float depth : SV_Depth;
            };
            #endif
            
            #ifdef _QUALITY_LEVEL3
            // Calculate appropriate LOD level for tex2Dlod sampling
            float CalculateLOD(float3 worldPos, float3 worldNormal, float3 viewDir)
            {
                // Distance-based LOD
                float distance = length(_WorldSpaceCameraPos - worldPos);
                float distanceLOD = log2(distance * 0.3); // Adjust multiplier as needed
                
                // Angle-based LOD - more detail when viewing straight on
                float viewAngle = abs(dot(worldNormal, viewDir));
                float angleLOD = log2(2.0 / max(viewAngle, 0.1));
                
                // Combine and clamp
                float finalLOD = max(0, min(distanceLOD + angleLOD * 0.5, 8.0));
                return finalLOD;
            }
            
            // Self-shadowing functions (Level 3 only)
            float SampleHeight(float2 uv)
            {
                return tex2Dlod(_DisplacementMap, float4(uv, 0, 0)).r * _SelfShadowDisplacementScale;
            }
            
            // Parallax Occlusion Mapping with Contact Refinement (Level 3 only)
            float2 ParallaxOcclusionMapping(float2 uv, float3 viewDir, float maxDisplacement)
            {
                float numLayers = _ParallaxLayers;
                
                float layerDepth = 1.0 / numLayers;
                float currentLayerDepth = 0.0;
                
                // Physically correct parallax using UV density, texture tiling, and object scale
                float2 P = viewDir.xy / viewDir.z * maxDisplacement / _MetersPerUntiledUV * _MainTex_ST.xy / _ObjectScale;
                float2 deltaTexCoords = P / numLayers;
                
                float2 currentTexCoords = uv;
                float currentDepthMapValue = 1.0 - tex2Dlod(_DisplacementMap, float4(currentTexCoords, 0, 0)).r;
                
                // Initial ray marching
                [unroll(4)]
                for(int i = 0; i < numLayers && currentLayerDepth < currentDepthMapValue; i++)
                {
                    currentTexCoords -= deltaTexCoords;
                    currentDepthMapValue = 1.0 - tex2Dlod(_DisplacementMap, float4(currentTexCoords, 0, 0)).r;
                    currentLayerDepth += layerDepth;
                }
                
                // Contact refinement using binary search
                float2 prevTexCoords = currentTexCoords + deltaTexCoords;
                float prevLayerDepth = currentLayerDepth - layerDepth;
                
                // Binary search refinement (4 iterations for good quality/performance balance)
                [unroll(4)]
                for(int j = 0; j < 4; j++)
                {
                    float2 midTexCoords = (currentTexCoords + prevTexCoords) * 0.5;
                    float midLayerDepth = (currentLayerDepth + prevLayerDepth) * 0.5;
                    float midDepthMapValue = 1.0 - tex2Dlod(_DisplacementMap, float4(midTexCoords, 0, 0)).r;
                    
                    if(midLayerDepth < midDepthMapValue)
                    {
                        // Ray hasn't hit surface yet, continue forward
                        prevTexCoords = midTexCoords;
                        prevLayerDepth = midLayerDepth;
                    }
                    else
                    {
                        // Ray has passed through surface, step back
                        currentTexCoords = midTexCoords;
                        currentLayerDepth = midLayerDepth;
                        currentDepthMapValue = midDepthMapValue;
                    }
                }
                
                // Final linear interpolation for sub-pixel accuracy
                float afterDepth = currentDepthMapValue - currentLayerDepth;
                float beforeDepth = (1.0 - tex2Dlod(_DisplacementMap, float4(prevTexCoords, 0, 0)).r) - prevLayerDepth;
                
                // Improved weight calculation with safety checks
                float denominator = afterDepth - beforeDepth;
                float weight = abs(denominator) > 1e-6 ? afterDepth / denominator : 0.5;
                weight = saturate(weight);
                
                float2 finalTexCoords = lerp(currentTexCoords, prevTexCoords, weight);
                
                return finalTexCoords;
            }
            
            // Self-Shadow calculation (Level 3 only)
            float CalculateSelfShadow(float2 parallaxUV, float3 worldNormal, float3 worldTangent, float3 worldBitangent, float3 lightDir)
            {
                float3x3 worldToTangent = float3x3(worldTangent, worldBitangent, worldNormal);
                float3 tangentLightDir = mul(worldToTangent, lightDir);
                
                if (tangentLightDir.z <= 0.0)
                    return 1.0;
                
                float numLayers = _ParallaxLayers;
                float2 shadowStepUV = tangentLightDir.xy / tangentLightDir.z * _ShadowDistance / numLayers;
                float stepLength = length(shadowStepUV);
                if (stepLength > 0.1) shadowStepUV = normalize(shadowStepUV) * 0.1;
                
                float length_D_xy = length(tangentLightDir.xy);
                float currentHeight = tex2Dlod(_DisplacementMap, float4(parallaxUV, 0, 0)).r * _SelfShadowDisplacementScale;
                float selfShadow = 1.0;
                
                // Check if texture is tiled by examining the tiling values
                bool isTiled = (_MainTex_ST.x > 1.0 || _MainTex_ST.y > 1.0);
                
                [unroll(4)]
                for (int j = 1; j < numLayers; j++)
                {
                    float2 shadowUV = parallaxUV + shadowStepUV * j;
                    
                    // Only check bounds if texture is not tiled
                    if (!isTiled && any(saturate(shadowUV) != shadowUV))
                        break;
                    
                    float shadowHeight = tex2Dlod(_DisplacementMap, float4(shadowUV, 0, 0)).r * _SelfShadowDisplacementScale;
                    float stepDistance = length(shadowStepUV * j);
                    
                    float expectedHeight = currentHeight;
                    if (length_D_xy > 0.0001) 
                    {
                        expectedHeight += (stepDistance * tangentLightDir.z) / length_D_xy;
                    }
                    expectedHeight += _ShadowBias;
                    
                    if (shadowHeight > expectedHeight)
                    {
                        float heightDiff = shadowHeight - expectedHeight;
                        float shadowAmount = saturate(heightDiff / (_SelfShadowDisplacementScale * 0.2));
                        
                        float distanceFalloff = 1.0 - saturate(stepDistance / _ShadowDistance);
                        distanceFalloff = lerp(1.0, distanceFalloff, _DistanceFalloffScale);
                        selfShadow = min(selfShadow, 1.0 - shadowAmount * _ShadowIntensity * distanceFalloff);
                    }
                }
                
                float surfaceToLightDirectionSimilarity = saturate(tangentLightDir.z);
                float shadowAlpha = pow(saturate(surfaceToLightDirectionSimilarity - 0.0), 0.3);
                return lerp(1.0, selfShadow, shadowAlpha);
            }
            #endif
            
            VertexOutput vert(VertexInput v)
            {
                VertexOutput o;
                
                // Apply main texture tiling and offset to all UV coordinates
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                // Vertex displacement along normals
                float4 displacedVertex = v.vertex;
                #ifdef _QUALITY_LEVEL3
                if (_ParallaxCounterDisplacement > 0.0)
                {
                    float displacementAmount = _MaxDisplacementMeters * _ParallaxCounterDisplacement / _ObjectScale;
                    displacedVertex.xyz += v.normal * displacementAmount;
                }
                #endif
                
                o.pos = UnityObjectToClipPos(displacedVertex);

                float3 vertexWorldPos = mul(unity_ObjectToWorld, displacedVertex).xyz;
                float3 vertexWorldNormal = UnityObjectToWorldNormal(v.normal);
                float3 vertexWorldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                float3 vertexWorldBitangent = cross(vertexWorldNormal, vertexWorldTangent) * v.tangent.w;

                o.worldPos = vertexWorldPos;
                o.worldNormal = vertexWorldNormal;
                o.worldTangent = vertexWorldTangent;
                o.worldBitangent = vertexWorldBitangent;
                
                #ifdef _QUALITY_LEVEL3
                o.lightDir = normalize(UnityWorldSpaceLightDir(vertexWorldPos));
                
                float3 worldVertexToCameraOffset = _WorldSpaceCameraPos - vertexWorldPos;
                o.viewDirTangent = float3(
                    dot(worldVertexToCameraOffset, vertexWorldTangent),
                    dot(worldVertexToCameraOffset, vertexWorldBitangent),
                    dot(worldVertexToCameraOffset, vertexWorldNormal)
                );
                #endif
                
                o.vertexColor = v.color;
                
                #ifdef LIGHTMAP_ON
                o.lightmapUV = v.lightmapUV * unity_LightmapST.xy + unity_LightmapST.zw;
                #endif
                
                TRANSFER_SHADOW(o);
                UNITY_TRANSFER_FOG(o, o.pos);
                
                return o;
            }
            
            #ifdef _QUALITY_LEVEL3
            FragmentOutput frag(VertexOutput input)
            #else
            fixed4 frag(VertexOutput input) : SV_Target
            #endif
            {
                float3 color;
                
                #ifdef _QUALITY_LEVEL1
                    fixed4 baseColor = tex2D(_MainTex, input.uv);
                    float ambientOcclusion = baseColor.a;
                    float3 albedo = baseColor.rgb;
                    
                    float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                    float normalDotLight = max(0, dot(input.worldNormal, lightDirection));

                    float realtimeShadow = SHADOW_ATTENUATION(input);

                    #ifdef LIGHTMAP_ON
                        float shadowmap = UNITY_SAMPLE_TEX2D(unity_ShadowMask, input.lightmapUV).r;
                        float combinedShadow = min(realtimeShadow, shadowmap);
                        float3 globalIlluminationLightmap = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, input.lightmapUV));
                        ambientOcclusion = max(ambientOcclusion, Luminance(globalIlluminationLightmap));
                        color = albedo * ambientOcclusion * (normalDotLight * _LightColor0.rgb * combinedShadow + globalIlluminationLightmap);
                    #else
                        float combinedShadow = realtimeShadow;
                        float3 ambientLight = unity_AmbientSky.rgb;
                        ambientOcclusion = max(ambientOcclusion, Luminance(ambientLight));
                        color = albedo * ambientOcclusion * (normalDotLight * _LightColor0.rgb * combinedShadow + ambientLight);
                    #endif
                    
                #elif defined(_QUALITY_LEVEL2)
                    fixed4 baseColor = tex2D(_MainTex, input.uv);
                    float ambientOcclusion = baseColor.a;
                    float3 albedo = baseColor.rgb;

                    fixed4 specGloss = tex2D(_SpecGlossMap, input.uv);
                    float3 specularColor = specGloss.rgb;
                    float glossiness = specGloss.a * _GlossinessMultiplier;

                    float3 normalMap = UnpackNormal(tex2D(_NormalMap, input.uv));
                    float3 worldNormal = normalize(input.worldTangent * normalMap.x + 
                                                 input.worldBitangent * normalMap.y + 
                                                 input.worldNormal * normalMap.z);

                    float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                    float3 viewDirection = normalize(UnityWorldSpaceViewDir(input.worldPos));
                    float3 halfVector = normalize(lightDirection + viewDirection);

                    float3 lightColor = _LightColor0.rgb;

                    float NdotL = max(0.0, dot(worldNormal, lightDirection));
                    float3 diffuse = albedo * NdotL * lightColor;

                    float NdotV = max(0.0, dot(worldNormal, viewDirection));
                    float NdotH = max(0.0, dot(worldNormal, halfVector));
                    float VdotH = max(0.0, dot(viewDirection, halfVector));

                    float roughness = 1.0 - glossiness;
                    float alpha = roughness * roughness;

                    float denom = NdotH * NdotH * (alpha * alpha - 1.0) + 1.0;
                    float D = (alpha * alpha) / (UNITY_PI * denom * denom);

                    float k = (roughness + 1.0);
                    k = k * k / 8.0;
                    float G_NdotL = NdotL / (NdotL * (1.0 - k) + k);
                    float G_NdotV = NdotV / (NdotV * (1.0 - k) + k);
                    float G = G_NdotL * G_NdotV;

                    float3 F0 = specularColor;
                    float F = F0 + (1.0 - F0) * pow(1.0 - VdotH, 5.0);

                    float3 specular = (D * G * F) / max(0.001, 4.0 * NdotL * NdotV);
                    specular *= _SpecularMultiplier;

                    float realtimeShadow = SHADOW_ATTENUATION(input);
                    
                    #ifdef LIGHTMAP_ON
                        float shadowmap = UNITY_SAMPLE_TEX2D(unity_ShadowMask, input.lightmapUV).r;
                        float combinedShadow = min(realtimeShadow, shadowmap);
                        float3 globalIlluminationLightmap = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, input.lightmapUV));
                        ambientOcclusion = max(ambientOcclusion, Luminance(globalIlluminationLightmap));
                        color = albedo * ambientOcclusion * (NdotL * lightColor * combinedShadow + globalIlluminationLightmap) + specular * sqrt(ambientOcclusion * combinedShadow);
                    #else
                        float combinedShadow = realtimeShadow;
                        float3 ambientLight = unity_AmbientSky.rgb;
                        ambientOcclusion = max(ambientOcclusion, Luminance(ambientLight));
                        color = albedo * ambientOcclusion * (NdotL * lightColor * combinedShadow + ambientLight) + specular * sqrt(ambientOcclusion * combinedShadow);
                    #endif
                    
                #elif defined(_QUALITY_LEVEL3)
                    // Anti-artifact calculations for parallax mapping
                    float3 viewDirTangent = normalize(input.viewDirTangent);
                    float3 baseWorldNormal = normalize(input.worldNormal);
                    float3 viewDirWorld = normalize(UnityWorldSpaceViewDir(input.worldPos));
                    
                    // Angle limiting - reduce parallax on steep viewing angles
                    float viewAngle = saturate(dot(baseWorldNormal, viewDirWorld));
                    // Use vertex color to modulate parallax strength
                    // White (smooth) = full parallax, Black (uneven) = reduced parallax
                    float invertedUnevenness = saturate(input.vertexColor.r); // Using red channel

                    float angleArtifactMitigation = lerp(1, smoothstep(0, 1, viewAngle), _ParallaxAngleAntiArtifact);

                    //invertedUnevenness = 1;
                    //viewAngle = 1;
                    float dynamicScale = _MaxDisplacementMeters * angleArtifactMitigation * invertedUnevenness;
                    
                    float2 parallaxUV = ParallaxOcclusionMapping(input.uv, viewDirTangent, dynamicScale);
                    bool isTiled = (_MainTex_ST.x > 1.0 || _MainTex_ST.y > 1.0);
                    if (!isTiled)
                        if(parallaxUV.x > 1.0 || parallaxUV.y > 1.0 || parallaxUV.x < 0.0 || parallaxUV.y < 0.0)
                            discard;

                    float selfShadow = CalculateSelfShadow(parallaxUV, input.worldNormal, input.worldTangent, input.worldBitangent, input.lightDir);
                    
                    // Calculate LOD for texture sampling
                    float textureLOD = CalculateLOD(input.worldPos, input.worldNormal, viewDirWorld);
                    
                    // Sample all textures using parallax-displaced UVs with explicit LOD
                    fixed4 baseColor = tex2Dlod(_MainTex, float4(parallaxUV, 0, textureLOD));
                    float ambientOcclusion = baseColor.a;
                    float3 albedo = baseColor.rgb;

                    fixed4 specGloss = tex2Dlod(_SpecGlossMap, float4(parallaxUV, 0, textureLOD));
                    float3 specularColor = specGloss.rgb;
                    float glossiness = specGloss.a * _GlossinessMultiplier;

                    float3 normalMap = UnpackNormal(tex2Dlod(_NormalMap, float4(parallaxUV, 0, textureLOD)));
                    float3 worldNormal = normalize(input.worldTangent * normalMap.x + 
                                                 input.worldBitangent * normalMap.y + 
                                                 input.worldNormal * normalMap.z);

                    float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                    float3 viewDirection = normalize(UnityWorldSpaceViewDir(input.worldPos));
                    float3 halfVector = normalize(lightDirection + viewDirection);

                    float3 lightColor = _LightColor0.rgb;

                    float NdotL = max(0.0, dot(worldNormal, lightDirection));
                    float3 diffuse = albedo * NdotL * lightColor;

                    float NdotV = max(0.0, dot(worldNormal, viewDirection));
                    float NdotH = max(0.0, dot(worldNormal, halfVector));
                    float VdotH = max(0.0, dot(viewDirection, halfVector));

                    float roughness = 1.0 - glossiness;
                    float alpha = roughness * roughness;

                    float denom = NdotH * NdotH * (alpha * alpha - 1.0) + 1.0;
                    float D = (alpha * alpha) / (UNITY_PI * denom * denom);

                    float k = (roughness + 1.0);
                    k = k * k / 8.0;
                    float G_NdotL = NdotL / (NdotL * (1.0 - k) + k);
                    float G_NdotV = NdotV / (NdotV * (1.0 - k) + k);
                    float G = G_NdotL * G_NdotV;

                    float3 F0 = specularColor;
                    float F = F0 + (1.0 - F0) * pow(1.0 - VdotH, 5.0);

                    float3 specular = (D * G * F) / max(0.001, 4.0 * NdotL * NdotV);
                    specular *= _SpecularMultiplier;

                    float realtimeShadow = SHADOW_ATTENUATION(input);
                    
                    #ifdef LIGHTMAP_ON
                        float shadowmap = UNITY_SAMPLE_TEX2D(unity_ShadowMask, input.lightmapUV).r;
                        float combinedShadow = min(min(selfShadow, shadowmap), realtimeShadow);
                        float3 globalIlluminationLightmap = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, input.lightmapUV));
                        ambientOcclusion = max(ambientOcclusion, Luminance(globalIlluminationLightmap));
                        color = albedo * ambientOcclusion * (NdotL * lightColor * combinedShadow + globalIlluminationLightmap) + specular * sqrt(ambientOcclusion * combinedShadow);
                    #else
                        float combinedShadow = min(selfShadow, realtimeShadow);
                        float3 ambientLight = unity_AmbientSky.rgb;
                        ambientOcclusion = max(ambientOcclusion, Luminance(ambientLight));
                        color = albedo * ambientOcclusion * (NdotL * lightColor * combinedShadow + ambientLight) + specular * sqrt(ambientOcclusion * combinedShadow);
                    #endif
                #endif
                
                UNITY_APPLY_FOG(input.fogCoord, color);
                color *= _BrightnessMultiplier;
                
                // Debug vertex color output
                if (_DebugVertexColor > 0.5)
                {
                    color = input.vertexColor.rgb;
                }

                #ifdef _QUALITY_LEVEL3
                    FragmentOutput output;
                    //color = textureLOD / 4;
                    output.color = fixed4(color, 1.0);
                    
                    // Calculate depth offset for parallax mapping
                    float displacementMapSample = tex2Dlod(_DisplacementMap, float4(parallaxUV, 0, 0)).r;
                    //float sampleDepthOffset = displacementMapSample * _MaxDisplacementMeters;
                    float sampleDepthOffset = (1.0 - displacementMapSample) * _MaxDisplacementMeters;
                    //  maxDisplacement / _MetersPerUntiledUV * _MainTex_ST.xy / _ObjectScale
                    // // Calculate camera-angle dependent offset
                    // float3 viewDirWorld = normalize(UnityWorldSpaceViewDir(input.worldPos));
                    // float3 worldNormal = normalize(input.worldNormal);
                    
                    // Project displacement along surface normal
                    float3 viewDir = normalize(UnityWorldSpaceViewDir(input.worldPos));
                    float3 surfaceNormal = normalize(input.worldNormal);
                    
                    // How much of the view direction aligns with the surface normal?
                    float normalComponent = abs(dot(viewDir, surfaceNormal));
                    
                    // Scale the offset so the normal component equals the desired displacement
                    float scaledOffset = sampleDepthOffset / max(normalComponent, 0.001);
                    float3 depthOffset = -viewDir * scaledOffset * _ParallaxDepthDisplacement;

                    // Project the displacement along the view direction
                    float4 clipPos = mul(UNITY_MATRIX_VP, float4(input.worldPos + depthOffset, 1.0));
                    
                    // Handle depth range differences between platforms
                    float ndcDepth = clipPos.z / clipPos.w;
                    
                    #if defined(SHADER_API_GLES) || defined(SHADER_API_GLES3) || defined(SHADER_API_WEBGL)
                        // WebGL/OpenGL uses [-1,1] NDC range, convert to [0,1] for depth buffer
                        output.depth = ndcDepth * 0.5 + 0.5;
                    #else
                        // DirectX already uses [0,1] range
                        output.depth = ndcDepth;
                    #endif
                    
                    //output.color = clipPos;
                    return output;
                #else
                    return fixed4(color, 1.0);
                #endif
            }
            ENDCG
        }
        
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Back
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _QUALITY_LEVEL1 _QUALITY_LEVEL2 _QUALITY_LEVEL3
            #pragma multi_compile_shadowcaster
            #pragma target 3.0
            
            #include "UnityCG.cginc"
            
            // Textures for displacement (Level 3 only)
            #ifdef _QUALITY_LEVEL3
            sampler2D _DisplacementMap;
            float4 _DisplacementMap_ST;
            float _MaxDisplacementMeters;
            float _ParallaxCounterDisplacement;
            float _ObjectScale;
            #endif
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            struct VertexInput
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct VertexOutput
            {
                V2F_SHADOW_CASTER;
                #ifdef _QUALITY_LEVEL3
                float2 uv : TEXCOORD1;
                #endif
            };
            
            VertexOutput vert(VertexInput v)
            {
                VertexOutput o;
                
                // Apply vertex displacement for Level 3 quality
                float4 displacedVertex = v.vertex;
                #ifdef _QUALITY_LEVEL3
                if (_ParallaxCounterDisplacement > 0.0)
                {
                    float displacementAmount = _MaxDisplacementMeters * _ParallaxCounterDisplacement / _ObjectScale;
                    displacedVertex.xyz += v.normal * displacementAmount;
                }
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                #endif
                
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }
            
            float4 frag(VertexOutput i) : SV_Target
            {
                #ifdef _QUALITY_LEVEL3
                // Sample alpha for potential alpha testing
                fixed4 texcol = tex2D(_MainTex, i.uv);
                clip(texcol.a - 0.001);
                #endif
                
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
}
