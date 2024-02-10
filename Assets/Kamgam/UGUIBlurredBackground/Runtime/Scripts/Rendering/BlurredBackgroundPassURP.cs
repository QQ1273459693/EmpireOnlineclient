#if KAMGAM_RENDER_PIPELINE_URP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Kamgam.UGUIBlurredBackground
{
    public class BlurredBackgroundPassURP : ScriptableRenderPass
    {
        public System.Action OnPostRender;

        public bool Active = false;

        protected int _iterations;
        public int Iterations
        {
            get => _iterations;
            set
            {
                if (_iterations != value)
                {
                    _iterations = value;
                }
            }
        }

        protected float _offset = 1.5f;
        public float Offset
        {
            get => _offset;
            set
            {
                _offset = value;
                setOffset(value);
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

        void setAdditiveColor(Material material, Color color)
        {
            if (material == null)
                return;

            material.SetColor("_AdditiveColor", color);
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

        public const string ShaderName = "Kamgam/UGUI/URP/Blur Shader";

        protected ShaderQuality _quality = ShaderQuality.Medium;
        public ShaderQuality Quality
        {
            get => _quality;
            set
            {
                _quality = value;
                _material = null;
            }
        }

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

                        switch (_quality)
                        {
                            case ShaderQuality.Low:
                                _material.SetKeyword(new LocalKeyword(shader, "_SAMPLES_LOW"), true);
                                _material.SetKeyword(new LocalKeyword(shader, "_SAMPLES_MEDIUM"), false);
                                _material.SetKeyword(new LocalKeyword(shader, "_SAMPLES_HIGH"), false);
                                break;

                            case ShaderQuality.Medium:
                                _material.SetKeyword(new LocalKeyword(shader, "_SAMPLES_LOW"), false);
                                _material.SetKeyword(new LocalKeyword(shader, "_SAMPLES_MEDIUM"), true);
                                _material.SetKeyword(new LocalKeyword(shader, "_SAMPLES_HIGH"), false);
                                break;

                            case ShaderQuality.High:
                                _material.SetKeyword(new LocalKeyword(shader, "_SAMPLES_LOW"), false);
                                _material.SetKeyword(new LocalKeyword(shader, "_SAMPLES_MEDIUM"), false);
                                _material.SetKeyword(new LocalKeyword(shader, "_SAMPLES_HIGH"), true);
                                break;

                            default:
                                break;
                        }

                        setOffset(_offset);
                        setAdditiveColor(_material,AdditiveColor);
                    }
                }
                return _material;
            }

            set
            {
                _material = value;
            }
        }

        void setOffset(float value)
        {
            if (_material != null)
                _material.SetVector("_BlurOffset", new Vector4(value, value, 0f, 0f));
        }

        [System.NonSerialized]
        protected RenderTexture _renderTargetBlurredA;
        public RenderTexture RenderTargetBlurredA
        {
            get
            {
                if (_renderTargetBlurredA == null)
                {
                    _renderTargetBlurredA = createRenderTexture();

                    if (_renderTargetHandleA != null)
                    {
                        _renderTargetHandleA.Release();
                        _renderTargetHandleA = null;
                    }
                }

                return _renderTargetBlurredA;
            }
        }

        [System.NonSerialized]
        protected RenderTexture _renderTargetBlurredB;
        public RenderTexture RenderTargetBlurredB
        {
            get
            {
                if (_renderTargetBlurredB == null)
                {
                    _renderTargetBlurredB = createRenderTexture();

                    if (_renderTargetHandleB != null)
                    {
                        _renderTargetHandleB.Release();
                        _renderTargetHandleB = null;
                    }
                }

                return _renderTargetBlurredB;
            }
        }

        [System.NonSerialized]
        protected RTHandle _renderTargetHandleA;
        public RTHandle RenderTargetHandleA
        {
            get
            {
                if (_renderTargetHandleA == null)
                    _renderTargetHandleA = RTHandles.Alloc(RenderTargetBlurredA);

                return _renderTargetHandleA;
            }
        }

        [System.NonSerialized]
        protected RTHandle _renderTargetHandleB;
        public RTHandle RenderTargetHandleB
        {
            get
            {
                if (_renderTargetHandleB == null)
                    _renderTargetHandleB = RTHandles.Alloc(RenderTargetBlurredB);

                return _renderTargetHandleB;
            }
        }

        RenderTexture createRenderTexture()
        {
            var texture = new RenderTexture(Resolution.x, Resolution.y, 0);
            texture.filterMode = FilterMode.Bilinear;

            return texture;
        }

        public void ClearRenderTargets()
        {
            if (_renderTargetHandleA != null)
            {
                _renderTargetHandleA.Release();
                _renderTargetHandleA = null;
            }
            if (_renderTargetBlurredA != null)
            {
                _renderTargetBlurredA.Release();
                _renderTargetBlurredA = null;
            }

            if (_renderTargetHandleB != null)
            {
                _renderTargetHandleB.Release();
                _renderTargetHandleB = null;
            }
            if (_renderTargetBlurredB != null)
            {
                _renderTargetBlurredB.Release();
                _renderTargetBlurredB = null;
            }
        }

        public Texture GetBlurredTexture()
        {
            return RenderTargetBlurredA;
        }


        // Actual Render Pass stuff starts here:
        // -------------------------------------------------------------

        // Turns out profiling scopes should NOT be mixed with CommandBuffers, see: 
        // https://forum.unity.com/threads/how-to-use-profilingscope-correctly.1366812/#post-8621289
        // ProfilingSampler _profilingSampler = new ProfilingSampler("UGUI Blurred Background Pass");

#if KAMGAM_RENDER_PIPELINE_URP_13
        RTHandle _cameraColorTarget;
#endif

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ConfigureInput(ScriptableRenderPassInput.Color);

#if KAMGAM_RENDER_PIPELINE_URP_13
            _cameraColorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;
#endif
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!Active || _iterations == 0 || Offset <= 0f)
                return;

            // Do not render while switching play modes.
#if UNITY_EDITOR
            if (EditorPlayState.State != EditorPlayState.PlayState.Playing && EditorPlayState.State != EditorPlayState.PlayState.Editing)
                return;
#endif

            // Skip rendering in scene view or preview. Why? Because rendering in these
            // makes the scene view flicker if not in play mode.
            // See: https://forum.unity.com/threads/urp-custom-pass-blit-flickering-in-scene-view.1461932/
#if UNITY_EDITOR
            if (   renderingData.cameraData.cameraType == CameraType.SceneView
                || renderingData.cameraData.cameraType == CameraType.Preview)
                return;
#endif


#if !KAMGAM_RENDER_PIPELINE_URP_13
            var source = renderingData.cameraData.renderer.cameraColorTarget;
#else
            var source = renderingData.cameraData.renderer.cameraColorTargetHandle;

            // Check if source is null, if yes then try to fetch it from the set target. Otherwise abort.
            if (renderingData.cameraData.cameraType != CameraType.Game || source == null)
            {
                source = _cameraColorTarget;
                if (source == null)
                {
#if UNITY_EDITOR
                    // TODO: Investigate: This is happening in URP 14 though it has no effect (everything works).
                    // Logger.LogWarning("Camera color target source is null. Will skip blur rendering. Please investigate this issue.");
#endif
                    return;
                }
            }
#endif

            CommandBuffer cmd = CommandBufferPool.Get(name: "UGUI Blurred Background Pass");

            cmd.Clear();

            // Notice: Do not use cmd.Blit() in SPRs, see:
            // https://forum.unity.com/threads/how-to-blit-in-urp-documentation-unity-blog-post-on-every-blit-function.1211508/#post-7735527
            // Blit Implementation can be found here:
            // https://github.com/Unity-Technologies/Graphics/blob/b57fcac51bb88e1e589b01e32fd610c991f16de9/Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blitter.cs#L221

            // First pass scales down the image
            Blit(cmd, source, RenderTargetHandleA);

            // 2 pass blur A > B, B > A
            for (int i = 0; i < Iterations; i++)
            {
                // Blur horizontal (pass 0)
                Blit(cmd, RenderTargetHandleA, RenderTargetHandleB, Material, 0);
                // Blur vertical (pass 1)
                Blit(cmd, RenderTargetHandleB, RenderTargetHandleA, Material, 1);
            }
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            CommandBufferPool.Release(cmd);

            OnPostRender?.Invoke();
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            base.OnCameraCleanup(cmd);
        }
    }
}
#endif