void MainLight_half(half3 WorldPos, out half3 Direction, out half3 Color,
    out half DistanceAtten, out half ShadowAtten)
{
#ifdef SHADERGRAPH_PREVIEW
    Direction = half3(0.5, 0.5, 0);
    Color = half3(1.0, 1.0, 1.0);
    DistanceAtten = 1.0;
    ShadowAtten = 1.0;
#else
    half4 shadowCoord;
    
#if SHADOWS_SCREEN
    half4 clipPos = TransformWorldToHClip(WorldPos);
    shadowCoord = ComputeScreenPos(clipPos);
#else
    shadowCoord = TransformWorldToShadowCoord(WorldPos);
#endif

    Light mainLight = GetMainLight(shadowCoord);
 
    Direction = mainLight.direction;
    Color = mainLight.color;
    DistanceAtten = mainLight.distanceAttenuation;
    ShadowAtten = mainLight.shadowAttenuation;
#endif
}
