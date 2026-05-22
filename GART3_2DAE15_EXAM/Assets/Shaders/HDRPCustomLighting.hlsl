#ifndef HDRP_CUSTOM_LIGHTING_INCLUDED
#define HDRP_CUSTOM_LIGHTING_INCLUDED

float4 _MainLightDirection;
float4 _MainLightColor;

void MainDirectionalLight_float(
    float3 WorldNormal,
    out float3 Dir,
    out float3 Col,
    out float  Shade)
{
#if SHADERGRAPH_PREVIEW
    Dir   = float3(0.707, 0.707, 0);
    Col   = float3(1, 1, 1);
    Shade = 0.5;
#else
    Dir   = normalize(_MainLightDirection.xyz);
    Col   = _MainLightColor.rgb;
    Shade = saturate(dot(normalize(WorldNormal), Dir));
#endif
}

void MainDirectionalLight_half(
    half3  WorldNormal,
    out half3  Dir,
    out half3  Col,
    out half   Shade)
{
#if SHADERGRAPH_PREVIEW
    Dir   = half3(0.707, 0.707, 0);
    Col   = half3(1, 1, 1);
    Shade = 0.5;
#else
    Dir   = normalize((half3)_MainLightDirection.xyz);
    Col   = (half3)_MainLightColor.rgb;
    Shade = saturate(dot(normalize(WorldNormal), Dir));
#endif
}

#endif
