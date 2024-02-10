using UnityEngine;
namespace Kamgam.UGUIBlurredBackground
{
    public interface IBlurRenderer
    {
        /// <summary>
        /// Defines how often the blur will be applied. Use with caution."
        /// </summary>
        int Iterations { get; set; }

        /// <summary>
        /// Defines how far out the sampling foes and thus the blur strength for each pass.
        /// </summary>
        float Offset { get; set; }

        /// <summary>
        /// The square texture resolution of the blurred image. Default is 512 x 512. Please use 2^n values like 256, 512, 1024, 2048. Reducing this will increase performance but decrease quality. Every frame your rendered image will be copied, resized and then blurred [BlurStrength] times.
        /// </summary>
        Vector2Int Resolution { get; set; }

        /// <summary>
        /// Defines how may samples are taken per pass. The higher the quality the more texels will be sampled and the lower the performance will be.
        /// </summary>
        ShaderQuality Quality { get; set; }

        /// <summary>
        /// Set the color that should be ADDED to the blur image. This is NOT a tint.
        /// </summary>
        Color AdditiveColor { get; set; }

        /// <summary>
        /// Enables or disabled the renderer. This will save performance as it deactivates the render hooks.
        /// </summary>
        bool Active { get; set; }

        /// <summary>
        /// Returns the render texture that contains the blurred image.
        /// </summary>
        /// <param name="renderMode"></param>
        /// <returns></returns>
        Texture GetBlurredTexture(RenderMode renderMode);

        /// <summary>
        /// Is called every frame. Checks for changes like new active cameras etc.
        /// </summary>
        void Update();

        /// <summary>
        /// A delegate that is being call immediately after rendering has been done.
        /// </summary>
        event System.Action OnPostRender;

        /// <summary>
        /// Sets the image that is controlling the blur properties.
        /// </summary>
        /// <param name="image"></param>
        void SetImage(BlurredBackgroundImage image);
    }
}