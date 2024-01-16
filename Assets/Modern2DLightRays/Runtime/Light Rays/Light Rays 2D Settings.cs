using LeafUtilsRays;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace LightRays2D
{

    [type: Serializable]
    public class LightRays2DSettings : IShaderSetter
    {

        public void SetAllCryoCallbacks(UnityAction a)
        {
            alpha.onValueChanged = a; raysAlpha1.onValueChanged = a; raysAlpha2.onValueChanged = a; raysAlpha3.onValueChanged = a; raysAlpha4.onValueChanged = a;
            raysSpeed1.onValueChanged = a; raysSpeed2.onValueChanged = a; raysSpeed3.onValueChanged = a; raysSpeed4.onValueChanged = a;
            raysDensity1.onValueChanged = a; raysDensity2.onValueChanged = a; raysDensity3.onValueChanged = a; raysDensity4.onValueChanged = a;
            extendedSinWaves.onValueChanged = a; pixelPerfect.onValueChanged = a; colorBottomRight.onValueChanged = a; colorBottomLeft.onValueChanged = a;
            colorTopRight.onValueChanged = a; colorTopLeft.onValueChanged = a; leftFalloffStart.onValueChanged = a; leftFalloffEnd.onValueChanged = a; rightFalloffStart.onValueChanged = a;
            rightFalloffEnd.onValueChanged = a; topFalloffStart.onValueChanged = a; topFalloffEnd.onValueChanged = a; bottomFalloffStart.onValueChanged = a; bottomFalloffEnd.onValueChanged = a; numberOfPixels.onValueChanged = a;
            waviness.onValueChanged = a; colorWithGradients.onValueChanged = a;
            useScrollingTexture.onValueChanged = a; harderEdges.onValueChanged = a; edgeAlphaMin.onValueChanged = a; edgeAlphaMax.onValueChanged = a; gradientX.onValueChanged = a; gradientY.onValueChanged = a;
        }

        public void SetPropertiesToMaterial(MaterialPropertyBlock mat, Material globalMat = null)
        {
            mat.SetFloat("_rays_alpha", alpha.value);

            mat.SetFloat("_alpha1", raysAlpha1.value);
            mat.SetFloat("_alpha2", raysAlpha2.value);
            mat.SetFloat("_alpha3", raysAlpha3.value);
            mat.SetFloat("_alpha4", raysAlpha4.value);

            mat.SetFloat("_speed1", raysSpeed1.value);
            mat.SetFloat("_speed2", raysSpeed2.value);
            mat.SetFloat("_speed3", raysSpeed3.value);
            mat.SetFloat("_speed4", raysSpeed4.value);

            mat.SetFloat("_density1", raysDensity1.value);
            mat.SetFloat("_density2", raysDensity2.value);
            mat.SetFloat("_density3", raysDensity3.value);
            mat.SetFloat("_density4", raysDensity4.value);

            mat.SetColor("_ColorBottomRight", colorBottomRight.value);
            mat.SetColor("_ColorBottomLeft", colorBottomLeft.value);
            mat.SetColor("_ColorTopRight", colorTopRight.value);
            mat.SetColor("_ColorTopLeft", colorTopLeft.value);

            mat.SetFloat("_rightFalloffStart", rightFalloffStart.value);
            mat.SetFloat("_rightFalloffEnd", rightFalloffEnd.value);
            mat.SetFloat("_leftFalloffStart", leftFalloffStart.value);
            mat.SetFloat("_leftFalloffEnd", leftFalloffEnd.value);
            mat.SetFloat("_topFalloffStart", topFalloffStart.value);
            mat.SetFloat("_topFalloffEnd", topFalloffEnd.value);
            mat.SetFloat("_bottomFalloffStart", bottomFalloffStart.value);
            mat.SetFloat("_bottomFalloffEnd", bottomFalloffEnd.value);

            mat.SetFloat("_numOfPixels", numberOfPixels.value);
            mat.SetFloat("_waviness", waviness.value);

            mat.SetFloat("_smoothstepAlphaMin", harderEdges.value ? edgeAlphaMin.value : 0);
            mat.SetFloat("_smoothstepAlphaMax", harderEdges.value ? edgeAlphaMax.value : 0);

            if (colorWithGradients.value)
            {
                if (gradientX.value != null && gradientX.value != default(Gradient) && gradientY.value != null && gradientY.value != default(Gradient))
                {
                    gradientXTex = gradientX.value.ToTexture2D();
                    gradientYTex = gradientY.value.ToTexture2D();
                    mat.SetTexture("_gradientX", gradientXTex);
                    mat.SetTexture("_gradientY", gradientYTex);
                }
                else if (gradientX != null && gradientX.value != default(Gradient))
                {
                    gradientXTex = gradientX.value.ToTexture2D();
                    gradientYTex = gradientX.value.ToTexture2D();
                    mat.SetTexture("_gradientX", gradientXTex);
                    mat.SetTexture("_gradientY", gradientYTex);
                }
                else if (gradientY != null && gradientY.value != default(Gradient))
                {
                    gradientXTex = gradientY.value.ToTexture2D();
                    gradientYTex = gradientY.value.ToTexture2D();
                    mat.SetTexture("_gradientX", gradientXTex);
                    mat.SetTexture("_gradientY", gradientYTex);
                }
            }

            mat.SetInt("_pixelPerfect1", pixelPerfect.value ? 1 : 0);
            mat.SetInt("_extended_sin_waves", extendedSinWaves.value ? 1 : 0);
            mat.SetInt("_useGradients", colorWithGradients.value ? 1 : 0);
            mat.SetInt("_smoothstepEdges", harderEdges.value ? 1 : 0);
            mat.SetInt("_useRaysTex", useScrollingTexture.value && raysTexture != null ? 1 : 0);

            if (raysTexture != null) mat.SetTexture("_raysTex", raysTexture);
        }

        public Cryo<float> alpha = new Cryo<float>(0.8f);

        public Cryo<float> raysAlpha1 = new Cryo<float>(0.24f);
        public Cryo<float> raysAlpha2 = new Cryo<float>(0.218f);
        public Cryo<float> raysAlpha3 = new Cryo<float>(0.284f);
        public Cryo<float> raysAlpha4 = new Cryo<float>(0.62f);

        public Cryo<float> raysSpeed1 = new Cryo<float>(1.13f);
        public Cryo<float> raysSpeed2 = new Cryo<float>(2.96f);
        public Cryo<float> raysSpeed3 = new Cryo<float>(2.52f);
        public Cryo<float> raysSpeed4 = new Cryo<float>(0.7f);

        public Cryo<float> raysDensity1 = new Cryo<float>(1.58f);
        public Cryo<float> raysDensity2 = new Cryo<float>(3.93f);
        public Cryo<float> raysDensity3 = new Cryo<float>(2.83f);
        public Cryo<float> raysDensity4 = new Cryo<float>(1.16f);

        public Cryo<bool> extendedSinWaves = new Cryo<bool>(true);
        public Cryo<bool> pixelPerfect = new Cryo<bool>(false);
        public Cryo<bool> harderEdges = new Cryo<bool>(false);
        public Cryo<bool> colorWithGradients = new Cryo<bool>(false);
        public Cryo<bool> useScrollingTexture = new Cryo<bool>(false);

        public Cryo<Gradient> gradientX = new Cryo<Gradient>(new Gradient());
        public Cryo<Gradient> gradientY = new Cryo<Gradient>(new Gradient());

        public Texture gradientXTex;
        public Texture gradientYTex;

        public Texture raysTexture;

        public Cryo<Color> colorBottomRight = new Cryo<Color>(ColorUtils.HexToRGB("FFE092"));
        public Cryo<Color> colorBottomLeft = new Cryo<Color>(ColorUtils.HexToRGB("FFE092"));
        public Cryo<Color> colorTopRight = new Cryo<Color>(Color.white);
        public Cryo<Color> colorTopLeft = new Cryo<Color>(Color.white);

        public Cryo<float> rightFalloffStart = new Cryo<float>(0.8f);
        public Cryo<float> rightFalloffEnd = new Cryo<float>(1f);

        public Cryo<float> leftFalloffStart = new Cryo<float>(0.0f);
        public Cryo<float> leftFalloffEnd = new Cryo<float>(0.2f);

        public Cryo<float> topFalloffStart = new Cryo<float>(0.92f);
        public Cryo<float> topFalloffEnd = new Cryo<float>(1f);

        public Cryo<float> bottomFalloffStart = new Cryo<float>(0.0f);
        public Cryo<float> bottomFalloffEnd = new Cryo<float>(0.8f);

        public Cryo<float> edgeAlphaMin = new Cryo<float>(0.25f);
        public Cryo<float> edgeAlphaMax = new Cryo<float>(0.30f);

        public Cryo<int> numberOfPixels = new Cryo<int>(200);
        public Cryo<float> waviness = new Cryo<float>(0.0f);
    }

    public static class GradientUtils
    {

        public static Texture2D ToTexture2D(this Gradient grad, int width = 32, int height = 1)
        {
            var gradTex = new Texture2D(width, height, TextureFormat.ARGB32, false);
            gradTex.filterMode = FilterMode.Bilinear;
            float inv = 1f / (width - 1);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var t = x * inv;
                    Color col = grad.Evaluate(t);
                    gradTex.SetPixel(x, y, col);
                }
            }
            gradTex.Apply();
            return gradTex;
        }

    }

}