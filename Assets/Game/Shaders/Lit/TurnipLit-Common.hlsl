#ifndef TURNIP_LIT_H
#define TURNIP_LIT_H

#include "Assets/Game/Shaders/Common.hlsl"
#include "Assets/Game/Shaders/RadishShadingModel.hlsl"

#if RADISH_USE_VERTEX_COLOR_ALBEDO
#define RADISH_NEEDS_VERTEX_COLOR
#endif

struct Attributes
{
    float4 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float2 texcoord0 : TEXCOORD0;
    #if RADISH_NEEDS_VERTEX_COLOR
    float3 color : COLOR0;
    #endif
};

struct Varyings
{
    float4 positionCS : SV_POSITION;
    float3 normalWS : NORMAL0;
    float3 positionWS : NORMAL1;
    float2 texcoord0 : TEXCOORD0;
    float4 color : COLOR0;
};

CBUFFER_START(UnityPerMaterial)
    float4 _Color;
    float4 _BaseMap_ST;
#if RADISH_PASS_ALPHA_TEST
    float _Clip;
#endif
CBUFFER_END

RADISH_DECLARE_TEX2D_SAMPLER(_BaseMap);

MaterialProperties BuildMaterialProperties(in Varyings IN)
{
    MaterialProperties props;

    props.normalWS = IN.normalWS;
    props.positionWS = IN.positionWS;
    
    float4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.texcoord0);
    
    props.albedo = baseColor.rgb * IN.color.rgb;
    #if RADISH_PASS_TRANSPARENT || RADISH_PASS_ALPHA_TEST
    props.alpha = baseColor.a * IN.color.a;
    #else
    props.alpha = 1;
    #endif

    return props;
}

Varyings RadishLit_Vert(in Attributes IN)
{
    Varyings OUT;

    OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
    OUT.positionCS = TransformWorldToHClip(OUT.positionWS);
    OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
    OUT.texcoord0 = TRANSFORM_TEX(IN.texcoord0, _BaseMap);
    OUT.color = _Color;

    #if RADISH_USE_VERTEX_COLOR_ALBEDO
    OUT.color *= IN.color;
    #endif

    return OUT;
}

float4 RadishLit_Frag(in Varyings IN) : SV_TARGET
{
    MaterialProperties materialProps = BuildMaterialProperties(IN);

    #if RADISH_PASS_ALPHA_TEST
    clip(materialProps.alpha - _Clip);
    #endif

    float4 shadedColor = ComputeLighting(materialProps);
    return shadedColor;
}

#endif