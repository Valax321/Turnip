#ifndef RADISH_SHADING_MODEL_H
#define RADISH_SHADING_MODEL_H

#include "Assets/Game/Shaders/Common.hlsl"

struct MaterialProperties
{
    float3 albedo;
    float alpha;
    float3 normalWS;
    float3 positionWS;
};

inline float3 GetAmbientLightColor(float3 normalWS)
{
    return float3(0.5, 0.5, 0.5);
}

float4 ComputeLighting(in MaterialProperties props)
{
    //TODO: acquire light direction
    float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

    // TODO: acquire light color
    float3 lightColor = _LightColor0.rgb;

    float3 ambient = GetAmbientLightColor(props.normalWS) * props.albedo;
    float3 nDotL = saturate(dot(lightDir, props.normalWS));

    return float4(ambient + (nDotL * lightColor), props.alpha);
}

#endif