Shader "Turnip/Drop Shadow"
{
    Properties
    {
        [NoScaleOffset] [PerInstanceData] _MainTex ("Shadow Shape", 2D) = "white" {}
        [PerInstanceData] _Color ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent-1"
            "RenderPipeline"="Turnip"
            "LightMode"="RadishLit"
        }
        
        Cull Front
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        ZTest GEqual

        Pass
        {
            Name "Drop Shadow"
            
            HLSLPROGRAM
            
            #include "Assets/Game/Shaders/DropShadow/DropShadow-Common.hlsl"

            #pragma vertex DropShadow_Vert
            #pragma fragment DropShadow_Frag

            ENDHLSL
        }
    }
}