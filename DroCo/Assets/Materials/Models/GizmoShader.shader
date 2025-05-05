Shader "Unlit/GizmoShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        ZTest Always
        ZWrite Off
        Lighting Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            Color [_Color]
        }
    }
}
