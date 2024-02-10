// This shader is used for ScreenSpaceOverly images. It's the most efficient one.

Shader "Kamgam/UGUI/BuiltIn/Blur Shader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _BlurOffset("Blur Offset", Vector) = (1.0, 1.0, 0)
        [KeywordEnum(Low, Medium, High)] _Samples("Sample Amount", Float) = 1
        _AdditiveColor("Additive Color", Color) = (0, 0, 0, 0)
        [MaterialToggle] _FlipVertical("flipVertical", Float) = 1
    }

    CGINCLUDE

    #if _SAMPLES_LOW

        #define SAMPLES 10

    #elif _SAMPLES_MEDIUM

        #define SAMPLES 30

    #else

        #define SAMPLES 100

    #endif


    #pragma multi_compile _SAMPLES_LOW _SAMPLES_MEDIUM _SAMPLES_HIGH
    #pragma vertex vert
    #pragma fragment frag

    #include "UnityCG.cginc"

    struct appdata
    {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
    };

    struct v2f
    {
        float2 uv : TEXCOORD0;
        float4 vertex : SV_POSITION;
    };

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;
    float4 _MainTex_ST;
    float4 _AdditiveColor;
    float2 _BlurOffset;
    float _FlipVertical;

    // Based on linear sampling on the GPU.
    // Weights from this excellent article:
    // https://www.rastergrid.com/blog/2010/09/efficient-gaussian-blur-with-linear-sampling/

    static const float offset[3] = { 0.0, 1.3846153846, 3.2307692308 };
    static const float weight[3] = { 0.2270270270, 0.3162162162, 0.0702702703 };

    float4 vert(float2 uv : TEXCOORD0) : SV_POSITION
    {
        float4 pos;
        pos.xy = uv;
        // This example is rendering with upside-down flipped projection,
        // so flip the vertical UV coordinate too
        if (_ProjectionParams.x < 0)
            pos.y = 1 - pos.y;
        pos.z = 0;
        pos.w = 1;
        return pos;
    }

    v2f Vert(appdata v)
    {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        return o;
    }

    fixed4 BlurHorizontal(v2f input) : SV_Target
    {
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
            float2 uv = input.uv + float2((i / sampleDiv - 0.5) * _BlurOffset.x, 0.0) * uv2px;

            color += tex2D(_MainTex, uv) * weight;
        }
        color /= weightSum;
        color.a = 1;

        return color;
    }

    fixed4 BlurVertical(v2f input) : SV_Target
    {
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
            float2 uv = input.uv + float2(0.0, (i / sampleDiv - 0.5) * _BlurOffset.y) * uv2px;

            // Flip UVs if necessary, see
            // https://docs.unity3d.com/Manual/SL-PlatformDifferences.html
            // https://forum.unity.com/threads/how-does-unity-handle-the-uv-coordinate-inconsistency-across-different-api.979794/#post-6366516
            // https://forum.unity.com/threads/command-buffer-blit-render-texture-result-is-upside-down.1463063/
            if (_FlipVertical && _ProjectionParams.x < 0)
            {
                uv.y = 1 - uv.y;
            }

            color += tex2D(_MainTex, uv) * weight;
        }
        color /= weightSum;
        color.a = tex2D(_MainTex, input.uv).a;

        color += _AdditiveColor;

        return color;
    }

    ENDCG

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        ZWrite Off
        ZTest Always
        Blend Off
        Cull Off

        Pass
        {
            Name "Blur Horizontal"

            CGPROGRAM

            v2f vert(appdata v)
            {
                return Vert(v);
            }

            fixed4 frag(v2f input) : SV_Target
            {
                return BlurHorizontal(input);
            }

            ENDCG
        }

        Pass
        {
            Name "Blur Vertical"

            CGPROGRAM

            v2f vert(appdata v)
            {
                return Vert(v);
            }

            fixed4 frag(v2f input) : SV_Target
            {
                return BlurVertical(input); 
            }

            ENDCG
        }
    }
}
