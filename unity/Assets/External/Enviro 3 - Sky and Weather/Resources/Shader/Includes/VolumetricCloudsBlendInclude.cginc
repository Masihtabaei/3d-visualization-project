
#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
UNITY_DECLARE_TEX2DARRAY(_EnviroClouds);
#else
sampler2D _EnviroClouds;  
#endif

float3 _AmbientColor;
float3 _DirectLightColor;
float _AtmosphereColorSaturateDistance;

float3 ApplyClouds(float3 sceneColor, float2 uv, float3 worldPos)
{
    float3 viewDir = normalize(worldPos.xyz - _WorldSpaceCameraPos);

    float4 cloudsColor = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_EnviroClouds, uv);
    float3 sunColor = pow(_DirectLightColor.rgb,2) * 2.0f;
    float3 skyColor = GetSkyColor(viewDir,0.005f);
    float4 finalColor = float4(cloudsColor.r * sunColor + _AmbientColor, cloudsColor.a);

    float atmosphericBlendFactor = saturate(exp(-cloudsColor.g / _AtmosphereColorSaturateDistance));
    finalColor.rgb = lerp(skyColor, finalColor.rgb, atmosphericBlendFactor); 
    
    return sceneColor.rgb * saturate(1 - finalColor.a) + finalColor.rgb * finalColor.a;
}
