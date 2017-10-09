// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

#ifndef TERRAIN_SPLATMAP_CUSTOM_CGINC_INCLUDED
#define TERRAIN_SPLATMAP_CUSTOM_CGINC_INCLUDED

#ifdef TERRAIN_STANDARD_SHADER
void SplatmapMixCustom(Input IN, half4 defaultAlpha, out half4 splat_control, out half weight, out fixed4 mixedDiffuse, inout fixed3 mixedNormal)
#else
void SplatmapMixCustom(Input IN, out half4 splat_control, out half weight, out fixed4 mixedDiffuse, inout fixed3 mixedNormal)
#endif
{
    splat_control = tex2D(_Control, IN.tc_Control);
    weight = dot(splat_control, half4(1,1,1,1));

    #if !defined(SHADER_API_MOBILE)
        clip(weight == 0.0f ? -1 : 1);
    #endif

    // Normalize weights before lighting and restore weights in final modifier functions so that the overal
    // lighting result can be correctly weighted.
    splat_control /= (weight + 1e-3f);

    mixedDiffuse = 0.0f;
    #ifdef TERRAIN_STANDARD_SHADER
        mixedDiffuse += splat_control.r * tex2D(_Splat0, IN.uv_Splat0) * half4(1.0, 1.0, 1.0, defaultAlpha.r);
        mixedDiffuse += splat_control.g * tex2D(_Splat1, IN.uv_Splat1) * half4(1.0, 1.0, 1.0, defaultAlpha.g);
        mixedDiffuse += splat_control.b * tex2D(_Splat2, IN.uv_Splat2) * half4(1.0, 1.0, 1.0, defaultAlpha.b);
        mixedDiffuse += splat_control.a * tex2D(_Splat3, IN.uv_Splat3) * half4(1.0, 1.0, 1.0, defaultAlpha.a);
    #else
        mixedDiffuse += splat_control.r * tex2D(_Splat0, IN.uv_Splat0);
        mixedDiffuse += splat_control.g * tex2D(_Splat1, IN.uv_Splat1);
        mixedDiffuse += splat_control.b * tex2D(_Splat2, IN.uv_Splat2);
        mixedDiffuse += splat_control.a * tex2D(_Splat3, IN.uv_Splat3);
    #endif

    #ifdef _TERRAIN_NORMAL_MAP
        fixed4 nrm = 0.0f;
        nrm += splat_control.r * tex2D(_Normal0, IN.uv_Splat0);
        nrm += splat_control.g * tex2D(_Normal1, IN.uv_Splat1);
        nrm += splat_control.b * tex2D(_Normal2, IN.uv_Splat2);
        nrm += splat_control.a * tex2D(_Normal3, IN.uv_Splat3);
        mixedNormal = UnpackNormal(nrm);
    #endif
}

#endif // TERRAIN_SPLATMAP_CUSTOM_CGINC_INCLUDED
