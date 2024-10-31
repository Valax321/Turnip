#ifndef RADISH_DROP_SHADOW_H
#define RADISH_DROP_SHADOW_H

#include "Assets/Game/Shaders/Common.hlsl"

struct Attributes
{
    float4 positionOS : POSITION;
};

struct Varyings
{
    float4 positionCS : SV_POSITION;
    float4 color : COLOR0;
    float4 texcoord : TEXCOORD0;
    float3 ray : TEXCOORD1;
};

CBUFFER_START(UnityPerMaterial)
    float4 _Color;
CBUFFER_END

RADISH_DECLARE_TEX2D_SAMPLER(_MainTex);
RADISH_DECLARE_TEX2D_SAMPLER(_CameraDepthTexture);

Varyings DropShadow_Vert(in Attributes IN)
{
    Varyings OUT;

    OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
    OUT.color = _Color;
    OUT.texcoord = ComputeClipSpacePosition(OUT.positionCS.xyz);
    OUT.ray = mul(UNITY_MATRIX_MV, IN.positionOS).xyz * float3(-1, -1, 1);

    return OUT;
}

float4 DropShadow_Frag(in Varyings IN) : SV_TARGET
{
    float3 fixedRay = IN.ray * (_ProjectionParams.z / IN.ray.z);
    float2 screenUV = IN.texcoord.xy / IN.texcoord.w;
    float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, screenUV);
    depth = Linear01Depth(depth, _ZBufferParams);
    float4 vpos = float4(fixedRay * depth, 1.0);
    float3 wpos = TransformObjectToWorld(vpos.xyz);
    float3 clipPos = TransformWorldToObject(wpos);
    clip(0.5 - abs(clipPos.xyz));
    float2 texUV = clipPos.xz + 0.5;

    float4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, texUV) * IN.color;
    return c;
}

#endif