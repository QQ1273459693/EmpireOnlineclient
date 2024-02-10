using System;
using UnityEngine;

namespace Kamgam.UGUIBlurredBackground
{
    /// <summary>
    /// Uses command buffers to hook into the rendering camera and extract a blurred image.
    /// </summary>
    public class BlurRendererBuiltIn : IBlurRenderer
    {
        public event Action OnPostRender;

        public BlurredBackgroundImage _image;

        /// <summary>
        /// Sets the image that is controlling the blur properties.
        /// </summary>
        /// <param name="image"></param>
        public void SetImage(BlurredBackgroundImage image)
        {
            _image = image;
        }

        protected BlurredBackgroundBufferBuiltIn _screenSpaceBuffer;
        public BlurredBackgroundBufferBuiltIn ScreenSpaceBuffer
        {
            get
            {
                if (_screenSpaceBuffer == null)
                {
                    _screenSpaceBuffer = new BlurredBackgroundBufferBuiltIn(BlurredBackgroundBufferBuiltIn.CameraEventForScreenSpaceOverlayCanvases);
                }
                return _screenSpaceBuffer;
            }
        }

        protected BlurredBackgroundBufferBuiltIn _worldAndCameraSpaceBuffer;
        public BlurredBackgroundBufferBuiltIn WorldAndCameraSpaceBuffer
        {
            get
            {
                if (_worldAndCameraSpaceBuffer == null)
                {
                    _worldAndCameraSpaceBuffer = new BlurredBackgroundBufferBuiltIn(BlurredBackgroundBufferBuiltIn.CameraEventForWorldOrCameraCanvases);

                    // Since the world space buffer is created on-demand we have to hand over all the config values to sync it.
                    _worldAndCameraSpaceBuffer.Active     = _screenSpaceBuffer.Active;
                    _worldAndCameraSpaceBuffer.Iterations = _screenSpaceBuffer.Iterations;
                    _worldAndCameraSpaceBuffer.Offset     = _screenSpaceBuffer.Offset;
                    _worldAndCameraSpaceBuffer.Resolution = _screenSpaceBuffer.Resolution;
                    _worldAndCameraSpaceBuffer.Quality    = _screenSpaceBuffer.Quality;
                }
                return _worldAndCameraSpaceBuffer;
            }
        }

        protected bool _active;

        /// <summary>
        /// Activate or deactivate the renderer. Disable to save performance (no rendering will be done).
        /// </summary>
        public bool Active
        {
            get => _active;
            set
            {
                if (value != _active)
                {
                    _active = value;
                    if (!_active)
                    {
                        ScreenSpaceBuffer.Active = value;
                        ScreenSpaceBuffer.ClearBuffers();
                        if (_worldAndCameraSpaceBuffer != null)
                        {
                            _worldAndCameraSpaceBuffer.Active = value;
                            _worldAndCameraSpaceBuffer.ClearBuffers();
                        }
                    }
                    else
                    {
                        var cam = RenderUtils.GetGameViewCamera(_image);
                        ScreenSpaceBuffer.Active = value; 
                        ScreenSpaceBuffer.AddBuffer(cam);
                        if (_worldAndCameraSpaceBuffer != null)
                        {
                            _worldAndCameraSpaceBuffer.Active = value;
                            _worldAndCameraSpaceBuffer.AddBuffer(cam);
                        }
                    }
                }
            }
        }

        public int Iterations
        {
            get
            {
                return ScreenSpaceBuffer.Iterations;
            }

            set
            {
                ScreenSpaceBuffer.Iterations = value;
                if (_worldAndCameraSpaceBuffer != null)
                    _worldAndCameraSpaceBuffer.Iterations = value;
            }
        }

        public float Offset
        {
            get
            {
                return ScreenSpaceBuffer.Offset;
            }

            set
            {
                ScreenSpaceBuffer.Offset = value;
                if (_worldAndCameraSpaceBuffer != null)
                    _worldAndCameraSpaceBuffer.Offset = value;
            }
        }

        public Vector2Int Resolution
        {
            get
            {
                return ScreenSpaceBuffer.Resolution;
            }
            set
            {
                ScreenSpaceBuffer.Resolution = value;
                if (_worldAndCameraSpaceBuffer != null)
                    _worldAndCameraSpaceBuffer.Resolution = value;
            }
        }

        public ShaderQuality Quality
        {
            get
            {
                return ScreenSpaceBuffer.Quality;
            }
            set
            {
                ScreenSpaceBuffer.Quality = value;
                if (_worldAndCameraSpaceBuffer != null)
                    _worldAndCameraSpaceBuffer.Quality = value;
            }
        }

        /// <summary>
        /// The material is used in screen space overlay canvases.
        /// </summary>
        public Material GetMaterial(RenderMode renderMode)
        {
            if (renderMode == RenderMode.ScreenSpaceOverlay)
                return ScreenSpaceBuffer.Material;
            else
                return WorldAndCameraSpaceBuffer.Material;
        }

        public Texture GetBlurredTexture(RenderMode renderMode)
        {
            if (renderMode == RenderMode.ScreenSpaceOverlay)
                return ScreenSpaceBuffer.GetBlurredTexture();
            else
                return WorldAndCameraSpaceBuffer.GetBlurredTexture();
        }

        protected Color _additiveColor = new Color(0, 0, 0, 0);
        public Color AdditiveColor
        {
            get
            {
                return _additiveColor;
            }
            set
            {
                _additiveColor = value;

                ScreenSpaceBuffer.AdditiveColor = value;

                if (_worldAndCameraSpaceBuffer != null)
                    WorldAndCameraSpaceBuffer.AdditiveColor = value;
            }
        }

        /// <summary>
        /// Called in the Update loop.
        /// </summary>
        public void Update()
        {
            var gameCam = RenderUtils.GetGameViewCamera(_image);
            _screenSpaceBuffer?.UpdateActiveCamera(gameCam);
            _worldAndCameraSpaceBuffer?.UpdateActiveCamera(gameCam);

            OnPostRender?.Invoke();
        }

        ~BlurRendererBuiltIn()
        {
            _screenSpaceBuffer?.ClearBuffers();
            _worldAndCameraSpaceBuffer?.ClearBuffers();
        }
    }
}