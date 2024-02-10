using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Kamgam.UGUIBlurredBackground
{
    /// <summary>
    /// Uses command buffers to hook into the rendering camera and extract a blurred image.
    /// </summary>
    public class BlurredBackgroundBufferBuiltIn
    {
        public const string ShaderName = "Kamgam/UGUI/BuiltIn/Blur Shader";

        public const CameraEvent CameraEventForScreenSpaceOverlayCanvases = CameraEvent.AfterEverything;
        public const CameraEvent CameraEventForWorldOrCameraCanvases = CameraEvent.BeforeForwardAlpha;

        protected Camera _camera;
        protected CameraEvent _cameraEvent;
        protected CommandBuffer _buffer;

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
                        ClearBuffers();
                    }
                    else
                    {
                        if (_camera != null)
                            AddBuffer(_camera, _cameraEvent);
                    }
                }
            }
        }

        protected int _iterations = 1;
        public int Iterations
        {
            get => _iterations;
            set 
            {
                if (value != _iterations)
                {
                    _iterations = value;
                    RecreateBuffers();
                }
            }
        }

        protected float _offset = 10f;
        public float Offset
        {
            get => _offset;
            set
            {
                _offset = value;
                setOffset(value);
            }
        }

        protected Vector2Int _resolution = new Vector2Int(512, 512);
        /// <summary>
        /// The texture resolution of the blurred image. Default is 512 x 512. Please use 2^n values like 256, 512, 1024, 2048. Reducing this will increase performance but decrease quality. Every frame your rendered image will be copied, resized and then blurred [BlurStrength] times.
        /// </summary>
        public Vector2Int Resolution
        {
            get => _resolution;
            set
            {
                _resolution = value;
                updateRenderTextureResolutions();
                setOffset(_offset); // We have to update offset here because the _worldMaterial offset depends on _resolution.
            }
        }

        void updateRenderTextureResolutions()
        {
            if (_renderTargetBlurredA != null)
            {
                _renderTargetBlurredA.Release();
                _renderTargetBlurredA.width = _resolution.x;
                _renderTargetBlurredA.height = _resolution.y;
                _renderTargetBlurredA.Create();
            }

            if (_renderTargetBlurredB != null)
            {
                _renderTargetBlurredB.Release();
                _renderTargetBlurredB.width = _resolution.x;
                _renderTargetBlurredB.height = _resolution.y;
                _renderTargetBlurredB.Create();
            }
        }

        protected Shader _blurShader;
        public Shader BlurShader
        {
            get
            {
                if (_blurShader == null)
                {
                    _blurShader = Shader.Find(ShaderName);
                }

                return _blurShader;
            }
        }

        protected ShaderQuality _quality = ShaderQuality.Medium;
        public ShaderQuality Quality
        {
            get => _quality;
            set
            {
                if (_quality != value)
                {
                    _quality = value;

                    setQualityOfMaterial(_material, _quality);
                }
            }
        }

        protected Color _additiveColor = new Color(0f, 0f, 0f, 0f);
        public Color AdditiveColor
        {
            get => _additiveColor;
            set
            {
                _additiveColor = value;
                setAdditiveColor(_material, value);
            }
        }

        /// <summary>
        /// The material is used in screen space overlay canvases.
        /// </summary>
        [System.NonSerialized]
        protected Material _material;
        public Material Material
        {
            get
            {
                if (_material == null)
                {
                    // Create material with shader
                    var shader = Shader.Find(ShaderName);
                    if (shader != null)
                    {
                        _material = new Material(shader);
                        _material.color = Color.white;
                        _material.hideFlags = HideFlags.HideAndDontSave;

                        setQualityOfMaterial(_material, _quality);
                        setFlipVerticalOfMaterial(_material, shouldFlipInShaderDependingOnProjectionParams());
                        setAdditiveColor(_material, AdditiveColor);
                        setOffset(_offset);
                    }
                }
                return _material;
            }

            set
            {
                _material = value;
            }
        }

        void setQualityOfMaterial(Material material, ShaderQuality quality)
        {
            if (material == null)
                return;

            switch (quality)
            {
                case ShaderQuality.Low:
                    material.SetKeyword(new LocalKeyword(material.shader, "_SAMPLES_LOW"), true);
                    material.SetKeyword(new LocalKeyword(material.shader, "_SAMPLES_MEDIUM"), false);
                    material.SetKeyword(new LocalKeyword(material.shader, "_SAMPLES_HIGH"), false);
                    break;

                case ShaderQuality.Medium:
                    material.SetKeyword(new LocalKeyword(material.shader, "_SAMPLES_LOW"), false);
                    material.SetKeyword(new LocalKeyword(material.shader, "_SAMPLES_MEDIUM"), true);
                    material.SetKeyword(new LocalKeyword(material.shader, "_SAMPLES_HIGH"), false);
                    break;

                case ShaderQuality.High:
                    material.SetKeyword(new LocalKeyword(material.shader, "_SAMPLES_LOW"), false);
                    material.SetKeyword(new LocalKeyword(material.shader, "_SAMPLES_MEDIUM"), false);
                    material.SetKeyword(new LocalKeyword(material.shader, "_SAMPLES_HIGH"), true);
                    break;

                default:
                    break;
            }
        }

        public BlurredBackgroundBufferBuiltIn(CameraEvent evt)
        {
            if (evt != CameraEventForScreenSpaceOverlayCanvases && evt != CameraEventForWorldOrCameraCanvases)
                throw new System.Exception("Only " + CameraEventForScreenSpaceOverlayCanvases + " and " + CameraEventForWorldOrCameraCanvases + " events are supported.");

            _cameraEvent = evt;
        }

        bool shouldFlipInShaderDependingOnProjectionParams()
        {
            // If I use DirectX (Win 10 Pc) or Vulkan (Win 10 Pc) or Metal (on an M1) it is flipped.
            // If I use OpenGL it works fine for all events (CameraEvent.AfterEverything and CameraEvent.BeforeForwardAlpha)
            // See: https://forum.unity.com/threads/command-buffer-blit-render-texture-result-is-upside-down.1463063/#post-9159080

            // If on OpenGL then always enable flipping because OpenGL platforms do the flipping
            // correctly via _ProjectionParams in all cases.
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLCore
#if !UNITY_2023_1_OR_NEWER
                || SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2
#endif
                || SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3)
            {
                return true;
            }

            // On other platforms enable flipping via _ProjectionParams only if the
            // event is CameraEvent.AfterEverything (i.e. after post processing)
            return _cameraEvent == CameraEvent.AfterEverything;
        }

        void setFlipVerticalOfMaterial(Material material, bool flip)
        {
            if (material == null)
                return;

            material.SetFloat("_FlipVertical", flip ? 1f : 0f);
        }

        void setAdditiveColor(Material material, Color color)
        {
            if (material == null)
                return;

            material.SetColor("_AdditiveColor", color);
        }

        void setOffset(float value)
        {
            if (_material != null)
                _material.SetVector("_BlurOffset", new Vector4(value, value, 0f, 0f));
        }

        [System.NonSerialized]
        protected RenderTexture _renderTargetBlurredA;
        protected RenderTexture renderTargetBlurredA
        {
            get
            {
#if UNITY_EDITOR
                releaseTexturesIfInWrongColorSpace();
#endif

                if (_renderTargetBlurredA == null)
                    _renderTargetBlurredA = createRenderTexture();

                return _renderTargetBlurredA;
            }
        }

#if UNITY_EDITOR
        protected void releaseTexturesIfInWrongColorSpace()
        {
            if (_renderTargetBlurredA != null)
            {
                // If the current sRGB settings does not match the color space then recreate the render textures.
                if ((_renderTargetBlurredA.sRGB && QualitySettings.activeColorSpace == ColorSpace.Gamma)
                    || (!_renderTargetBlurredA.sRGB && QualitySettings.activeColorSpace == ColorSpace.Linear))
                {
                    _renderTargetBlurredA?.Release();
                    _renderTargetBlurredA = null;
                    _renderTargetBlurredB?.Release();
                    _renderTargetBlurredB = null;
                }
            }
        }
#endif

        [System.NonSerialized]
        protected RenderTexture _renderTargetBlurredB;
        protected RenderTexture renderTargetBlurredB
        {
            get
            {
                if (_renderTargetBlurredB == null)
                    _renderTargetBlurredB = createRenderTexture();

                return _renderTargetBlurredB;
            }
        }

        RenderTexture createRenderTexture()
        {
            var rw = QualitySettings.activeColorSpace == ColorSpace.Linear ? RenderTextureReadWrite.sRGB : RenderTextureReadWrite.Default;
            var texture = new RenderTexture(Resolution.x, Resolution.y, 0, RenderTextureFormat.Default, rw);
            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;

            return texture;
        }

        public Texture GetBlurredTexture()
        {
            // Debugging textures
#if UNITY_EDITOR
            var settings = UGUIBlurredBackgroundSettings.GetOrCreateSettings();
            if (settings.DebugRenderTextureScreen != null && _cameraEvent == CameraEventForScreenSpaceOverlayCanvases)
            {
                if (renderTargetBlurredA.width == settings.DebugRenderTextureScreen.width)
                {
                    Graphics.CopyTexture(renderTargetBlurredA, settings.DebugRenderTextureScreen);
                }
                else
                {
                    Debug.LogWarning("Debugging render texture width does not match blur render texture width. Debug texture will remain empty.");
                }
            }
            if (settings.DebugRenderTextureWorld != null && _cameraEvent == CameraEventForWorldOrCameraCanvases)
            {
                if (renderTargetBlurredA.width == settings.DebugRenderTextureWorld.width)
                {
                    Graphics.CopyTexture(renderTargetBlurredA, settings.DebugRenderTextureWorld);
                }
                else
                {
                    Debug.LogWarning("Debugging render texture width does not match blur render texture width. Debug texture will remain empty.");
                }
            }
#endif

            return renderTargetBlurredA;
        }

        public void ClearBuffers()
        {
            if (_camera != null && _buffer != null)
                _camera.RemoveCommandBuffer(_cameraEvent, _buffer);
        }

        public void AddBuffer(Camera cam)
        {
            AddBuffer(cam, _cameraEvent);
        }

        public void AddBuffer(Camera cam, CameraEvent evt)
        {
            if (cam == null)
                return;

            // Seach for old buffers and remove them
            var buffers = cam.GetCommandBuffers(evt);
            foreach (var buf in buffers)
            {
                if (buf.name.StartsWith("Kamgam.UGUI Blur"))
                {
                    cam.RemoveCommandBuffer(_cameraEvent, buf);
                    buf.Dispose();
                }
            }

            // Create buffer if needed
            // Debug.Log("Creating Command Buffer on " + cam);
            _buffer = createBuffer("Kamgam.UGUI Blur (" + evt + ")");
            cam.AddCommandBuffer(evt, _buffer);

            // Done to avoid flipped (upside down) render results, see:
            // https://forum.unity.com/threads/commandbuffer-rendering-scene-flipped-upside-down-in-forward-rendering.415922/#post-3114571
            cam.forceIntoRenderTexture = true;
        }

        public CommandBuffer createBuffer(string name)
        {
            CommandBuffer buf = new CommandBuffer();
            buf.name = name;

            // copy screen into temporary RT
            int screenCopyID = Shader.PropertyToID("_ScreenCopyTexture");
            var desc = new RenderTextureDescriptor(-1, -1);
            desc.depthBufferBits = 0;
            desc.useMipMap = false;
            desc.autoGenerateMips = false;
            desc.colorFormat = RenderTextureFormat.Default;
            // Makes sure to properly support linear color space.
            desc.sRGB = QualitySettings.activeColorSpace == ColorSpace.Linear;
            buf.GetTemporaryRT(screenCopyID, desc, FilterMode.Bilinear);
            buf.Blit(BuiltinRenderTextureType.CurrentActive, screenCopyID);

            // Copy from source to A (Sets _MainTex and scales the target down to our blur texture size).
            buf.Blit(screenCopyID, renderTargetBlurredA);

            // 2 pass blur (A > B > A)
            int iterations = Iterations * 2 - 1; // Necessary do compensate for flipping of Material (iterations need
                                                 // to be odd or else the image is upside down if shouldFlip() is true).
            for (int i = 0; i < iterations; i++)
            {
                buf.Blit(renderTargetBlurredA, renderTargetBlurredB, Material, 0);
                buf.Blit(renderTargetBlurredB, renderTargetBlurredA, Material, 1);
            }

            buf.ReleaseTemporaryRT(screenCopyID);

            return buf;
        }

        public void UpdateActiveCamera(Camera cam)
        {
            if (cam != null && _camera != cam)
            {
                // Debug.Log("Setting new camera: " + cam);

                ClearBuffers();
                _camera = cam;
                AddBuffer(_camera, _cameraEvent);
            }
        }

        public void RecreateBuffers()
        {
            ClearBuffers();

            if (_camera != null)
                AddBuffer(_camera);
        }
    }
}