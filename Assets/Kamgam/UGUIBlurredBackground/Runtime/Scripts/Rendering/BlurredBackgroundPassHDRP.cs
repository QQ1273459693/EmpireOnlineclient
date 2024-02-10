#if KAMGAM_RENDER_PIPELINE_HDRP
// Based on: https://github.com/alelievr/HDRP-Custom-Passes/blob/2021.2/Assets/CustomPasses/CopyPass/CopyPass.cs#L67
// as recommended by antoinel_unity in https://forum.unity.com/threads/custom-pass-into-render-texture-into-custom-aov.1146872/#post-7362314

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Kamgam.UGUIBlurredBackground
{
    public class BlurredBackgroundPassHDRP : CustomPass
    {
        public System.Action OnPostRender;

        public const string ShaderName = "Kamgam/UGUI/HDRP/Blur Shader";

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
                        _material = CoreUtils.CreateEngineMaterial(shader);
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

                        setOffset(Offset);
                        setAdditiveColor(_material, AdditiveColor);
                    }
                }
                return _material;
            }

            set
            {
                _material = value;
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

        [System.NonSerialized]
        protected int _iterations;
        public int Iterations
        {
            get => _iterations;
            set
            {
                if (_iterations != value)
                {
                    _iterations = value;
                    enabled = _iterations > 0;
                }
            }
        }

        protected float _offset;
        public float Offset
        {
            get => _offset;
            set
            {
                _offset = value;
                setOffset(value);
            }
        }


        void setOffset(float value)
        {
            if (Material != null)
                Material.SetVector("_BlurOffset", new Vector4(value, value, 0f, 0f));
        }

        protected ShaderQuality _quality;

        /// <summary>
        /// The used shader quality. The higher the more performance it will cost.
        /// </summary>
        public ShaderQuality Quality
        {
            get => _quality;
            set
            {
                _quality = value;
                _material = null;
            }
        }

        protected Vector2Int _resolution = new Vector2Int(512, 512);

        /// <summary>
        /// The texture resolution of the blurred image. Default is 512 x 512.
        /// Please use 2^n values like 256, 512, 1024, 2048. Reducing this will increase
        /// performance but decrease quality. Every frame your rendered image will be copied, 
        /// resized and then blurred [BlurStrength] times.
        /// </summary>
        public Vector2Int Resolution
        {
            get => _resolution;
            set
            {
                if (_resolution != value)
                {
                    _resolution = value;
                    UpdateRenderTextureResolutions();
                }
            }
        }

        public void UpdateRenderTextureResolutions()
        {
            if (_renderTargetBlurredA != null)
            {
                _renderTargetBlurredA.Release();
                _renderTargetBlurredA.width = Resolution.x;
                _renderTargetBlurredA.height = Resolution.y;
                _renderTargetBlurredA.Create();
            }

            if (_renderTargetBlurredB != null)
            {
                _renderTargetBlurredB.Release();
                _renderTargetBlurredB.width = Resolution.x;
                _renderTargetBlurredB.height = Resolution.y;
                _renderTargetBlurredB.Create();
            }
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

        public Texture GetBlurredTexture()
        {
            return RenderTargetBlurredA;
        }

        protected override bool executeInSceneView => false;

        protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
        {
            name = "UGUI Blurred Background";
        }

        protected override void Execute(CustomPassContext ctx)
        {
            if (Material == null || Iterations == 0 || Offset == 0)
                return;

            var source = ctx.cameraColorBuffer;

            // First pass is just a copy with the right scale (plus downsampling).
            var scale = RTHandles.rtHandleProperties.rtHandleScale;
            ctx.cmd.Blit(source, RenderTargetBlurredA, new Vector2(scale.x, scale.y), Vector2.zero, 0, 0);

            // 2 pass ping pong (A > B > A)
            for (int i = 0; i < Iterations; i++)
            {
                ctx.cmd.Blit(RenderTargetBlurredA, RenderTargetBlurredB, Material, 0);
                ctx.cmd.Blit(RenderTargetBlurredB, RenderTargetBlurredA, Material, 1);
            }

            OnPostRender?.Invoke();
        }

        protected override void Cleanup()
        {
            CoreUtils.Destroy(_material);

            if (_renderTargetBlurredA != null)
            {
                _renderTargetBlurredA.Release();
                _renderTargetBlurredA = null;
            }

            if (_renderTargetBlurredB != null)
            {
                _renderTargetBlurredB.Release();
                _renderTargetBlurredB = null;
            }

            if (_renderTargetHandleA != null)
            {
                _renderTargetHandleA.Release();
                _renderTargetHandleA = null;
            }

            if (_renderTargetHandleB != null)
            {
                _renderTargetHandleB.Release();
                _renderTargetHandleB = null;
            }

            base.Cleanup();
        }
    }
}
#endif