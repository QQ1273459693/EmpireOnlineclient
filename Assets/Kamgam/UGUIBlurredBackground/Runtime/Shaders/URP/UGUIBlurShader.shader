Shader "Kamgam/UGUI/URP/Blur Shader"
{
    Properties
    {
        // MainTex is only used in ShaderGraph < 15
        _MainTex("Texture", 2D) = "white" {}
        _BlurOffset("Blur Offset", Vector) = (1.0, 1.0, 0)
        [KeywordEnum(Low, Medium, High)] _Samples("Sample Amount", Float) = 1
        _AdditiveColor("Additive Color", Color) = (0, 0, 0, 0)
    }

    HLSLINCLUDE

    #if _SAMPLES_LOW

        #define SAMPLES 10

    #elif _SAMPLES_MEDIUM

        #define SAMPLES 30

    #else

        #define SAMPLES 100

    #endif

    // TODO: Actually the only difference is the default source name _MainTex > _BlitTexture, so we could just extract that and use it in both.

#if UNITY_VERSION >= 202220 
    // Version compare, see: https://forum.unity.com/threads/urp-version-defines.1218915/#post-9021178

    // Shader Graph >= 14.x

    // Based on the default Hidden/Universal Render Pipleline/Blit shader found under
    // packages/com.unity.render-pipelines.universal@x.y.z/Shaders/Utils/Blit.shader

    #pragma multi_compile_fragment _ _LINEAR_TO_SRGB_CONVERSION
    #pragma multi_compile _SAMPLES_LOW _SAMPLES_MEDIUM _SAMPLES_HIGH

    // Core.hlsl for XR dependencies
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

    SAMPLER(sampler_BlitTexture);
#if UNITY_VERSION <= 202320 
    float4 _BlitTexture_TexelSize;
#endif
    float4 _BlurOffset;
    float4 _AdditiveColor;

    half4 BlurHorizontal(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        float2 uv = input.texcoord;

        // See: https://forum.unity.com/threads/_maintex_texelsize-whats-the-meaning.110278/
        // For a 1024 x 1024 texture this will be 1 / 1024.
        float2 uv2px = _BlitTexture_TexelSize.xy;

        // star form, blur with a sample for every step
        half4 color;
        int sampleDiv = SAMPLES - 1;
        float weightSum = 0;
        for (float i = 0; i < SAMPLES; i++)
        {
            // Linear kernel weight interpolation
            float weight = 0.5 + (0.5 - abs(i / sampleDiv - 0.5));
            weightSum += weight;

            // x
            color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, uv + float2((i / sampleDiv - 0.5) * _BlurOffset.x, 0.0) * uv2px) * weight;
        }
        color /= weightSum;
        color.a = 1;


        #ifdef _LINEAR_TO_SRGB_CONVERSION
        color = LinearToSRGB(color);
        #endif

        return color;
    }

    half4 BlurVertical(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        float2 uv = input.texcoord;

        // See: https://forum.unity.com/threads/_maintex_texelsize-whats-the-meaning.110278/
        // For a 1024 x 1024 texture this will be 1 / 1024.
        float2 uv2px = _BlitTexture_TexelSize.xy;

        // star form, blur with a sample for every step
        half4 color;
        int sampleDiv = SAMPLES - 1;
        float weightSum = 0;
        for (float i = 0; i < SAMPLES; i++)
        {
            // Linear kernel weight interpolation
            float weight = 0.5 + (0.5 - abs(i / sampleDiv - 0.5));
            weightSum += weight;

            // y
            color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, uv + float2(0.0, (i / sampleDiv - 0.5) * _BlurOffset.y) * uv2px) * weight;
        }
        color /= weightSum;
        color.a = 1;
        
        color += _AdditiveColor;

        #ifdef _LINEAR_TO_SRGB_CONVERSION
        color = LinearToSRGB(color);
        #endif

        return color;
    }

#else

    // Shader Graph < 15

    // Based on the default Hidden/Universal Render Pipleline/Blit shader found under
    // packages/com.unity.render-pipelines.universal@x.y.z/Shaders/Utils/Blit.shader

    #pragma multi_compile_fragment _ _LINEAR_TO_SRGB_CONVERSION
    #pragma multi_compile _ _USE_DRAW_PROCEDURAL
    #pragma multi_compile _SAMPLES_LOW _SAMPLES_MEDIUM _SAMPLES_HIGH

    #include "Packages/com.unity.render-pipelines.universal/Shaders/Utils/Fullscreen.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

    TEXTURE2D_X(_MainTex);
    SAMPLER(sampler_MainTex);
    float4 _MainTex_TexelSize;
    float4 _BlurOffset;
    float4 _AdditiveColor;

    half4 BlurHorizontal(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        float2 uv = input.uv;

        // See: https://forum.unity.com/threads/_maintex_texelsize-whats-the-meaning.110278/
        // For a 1024 x 1024 texture this will be 1 / 1024.
        float2 uv2px = _MainTex_TexelSize.xy;

        // star form, blur with a sample for every step
        half4 color;
        int sampleDiv = SAMPLES - 1;
        float weightSum = 0;
        for (float i = 0; i < SAMPLES; i++)
        {
            // Linear kernel weight interpolation
            float weight = 0.5 + (0.5 - abs(i / sampleDiv - 0.5));
            weightSum += weight;

            // x
            color += SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, uv + float2((i / sampleDiv - 0.5) * _BlurOffset.x, 0.0) * uv2px) * weight;
        }
        color /= weightSum;
        color.a = 1;

        #ifdef _LINEAR_TO_SRGB_CONVERSION
        color = LinearToSRGB(color);
        #endif

        return color;
    }

    half4 BlurVertical(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        float2 uv = input.uv;

        // See: https://forum.unity.com/threads/_maintex_texelsize-whats-the-meaning.110278/
        // For a 1024 x 1024 texture this will be 1 / 1024.
        float2 uv2px = _MainTex_TexelSize.xy;

        // star form, blur with a sample for every step
        half4 color;
        int sampleDiv = SAMPLES - 1;
        float weightSum = 0;
        for (float i = 0; i < SAMPLES; i++)
        {
            // Linear kernel weight interpolation
            float weight = 0.5 + (0.5 - abs(i / sampleDiv - 0.5));
            weightSum += weight;

            // y
            color += SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, uv + float2(0.0, (i / sampleDiv - 0.5) * _BlurOffset.y) * uv2px) * weight;
        }
        color /= weightSum;
        color.a = 1;

        color += _AdditiveColor;

        #ifdef _LINEAR_TO_SRGB_CONVERSION
        color = LinearToSRGB(color);
        #endif

        return color;
    }

#endif

    ENDHLSL




    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        ZTest Always
        ZWrite Off
        Cull Off

        Pass
        {
            Name "Blur Horizontal"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment BlurHorizontal

            ENDHLSL
        }

        Pass
        {
            Name "Blur Vertical"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment BlurVertical

            ENDHLSL
        }
    }
}

