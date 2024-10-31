Shader "Turnip/Lit"
{
    Properties
    {
        [Main(AlbedoGroup, _, off, off)] _albedoGroup ("Material", float) = 0
        [Sub(AlbedoGroup)] [MainTexture] _BaseMap ("Albedo", 2D) = "white" {}
        [Sub(AlbedoGroup)] [MainColor] _Color ("Color", Color) = (1, 1, 1, 1)
        [SubToggle(AlbedoGroup, RADISH_USE_VERTEX_COLOR_ALBEDO)] _UseVertexAlbedo ("Albedo Uses Vertex Color", float) = 0
        [Sub(AlbedoGroup)] [ShowIf(_RenderMode, Equal, 1)] _Clip ("Alpha Cutoff", Range(0, 1)) = 0.5
        
        [Space]
		[Title(Shader Properties)]
		[KWEnum(Opaque, _, Alpha Test, RADISH_PASS_ALPHA_TEST, Transparent, RADISH_PASS_TRANSPARENT)] _RenderMode ("Render Mode", float) = 0
		[Enum(UnityEngine.Rendering.CullMode)]_Cull("Cull Mode", Float) = 2
        [Preset(LWGUI_BlendModePreset)] _BlendMode ("Blend Mode Preset", float) = 0 
		[Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend("Blend (Source)", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)]_DstBlend("Blend (Dest)", Float) = 0
		[Toggle(_)]_ZWrite("ZWrite ", Float) = 1
    }
    SubShader
    {
        Tags {
            "RenderPipeline"="Turnip"
            "LightMode"="RadishLit"
        }
        
        Cull [_Cull]
        ZWrite [_ZWrite]
        Blend [_SrcBlend] [_DstBlend]

        Pass
        {
            Name "Lit"
            
            HLSLPROGRAM

            #pragma shader_feature_local __ RADISH_PASS_ALPHA_TEST RADISH_PASS_TRANSPARENT
            #pragma shader_feature_local __ RADISH_USE_VERTEX_COLOR_ALBEDO
            
            #include "Assets/Game/Shaders/Lit/TurnipLit-Common.hlsl"

            #pragma vertex RadishLit_Vert
            #pragma fragment RadishLit_Frag
            
            ENDHLSL
        }
    }

    CustomEditor "LWGUI.LWGUI"
}
