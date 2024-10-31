#ifndef RADISH_RP_COMMON_H
#define RADISH_RP_COMMON_H

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Assets/Game/Shaders/Input.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

#define RADISH_DECLARE_TEX2D_SAMPLER(baseName) TEXTURE2D(baseName); SAMPLER(sampler##baseName);
#define RADISH_DECLARE_TEX2D_NOSAMPLER(baseName) TEXTURE2D(baseName);

// Built-in lighting info
float4 _LightColor0;
float4 _WorldSpaceLightPos0;

#endif