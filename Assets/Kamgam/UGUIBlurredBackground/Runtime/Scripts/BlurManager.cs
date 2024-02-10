using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.UGUIBlurredBackground
{
    /// <summary>
    /// This manager keeps track of whether or not the blur is needed and disables the
    /// rendering if not. This is done to save performance when no blurred UI is shown.
    /// <br /><br />
    /// It also pumps the renderer Update() loop every frame.
    /// </summary>
    public class BlurManager
    {
        static BlurManager _instance;
        public static BlurManager Instance // This is triggered by the UGUI Image
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BlurManager();
                    _instance.ApplyValues();
                    BlurManagerUpdater.Instance.OnUpdate += _instance.Update;

#if UNITY_EDITOR && KAMGAM_RENDER_PIPELINE_URP
                    _instance.onEditorEnable();
#endif
                }
                return _instance;
            }
        }

        public static bool HasInstance()
        {
            return _instance != null;
        }

        ~BlurManager()
        {
#if UNITY_EDITOR && KAMGAM_RENDER_PIPELINE_URP
            onEditorDisable();
#endif
        }

        #region Editor Play Mode Exit Refresh Fix
        // This whole Editor section only exists to update the game view properly
        // after exiting the play mode. Without this the mesh of the BlurredImages
        // is not drawn or is white. TODO: Find out why and add a proper logic based
        // fix. Only necessary in URP
#if UNITY_EDITOR && KAMGAM_RENDER_PIPELINE_URP
        protected void onEditorEnable()
        {
            UnityEditor.EditorApplication.playModeStateChanged += onPlayModeStateChanged;
        }

        protected void onPlayModeStateChanged(UnityEditor.PlayModeStateChange state)
        {
            if (state == UnityEditor.PlayModeStateChange.EnteredEditMode)
            {

                EditorScheduler.Schedule(0.1f, () =>
                {
                    UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                    UnityEditor.SceneView.RepaintAll();
                    getGameView().Repaint();
                }, "BlurRendererURP.onPlayModeStateChanged");
            }
        }

        protected System.Type editorGameViewType;

        protected UnityEditor.EditorWindow getGameView()
        {
            if (editorGameViewType == null)
            {
                var assembly = typeof(UnityEditor.EditorWindow).Assembly;
                editorGameViewType = assembly.GetType("UnityEditor.GameView");
            }

            var gameview = UnityEditor.EditorWindow.GetWindow(editorGameViewType);
            return gameview;
        }

        protected void onEditorDisable()
        {
            UnityEditor.EditorApplication.playModeStateChanged -= onPlayModeStateChanged;
        }
#endif
        #endregion

        // -------------------

        /// <summary>
        /// Defines how often the blur will be applied. Use with caution. It drains performance quickly.")]
        /// </summary>
        [System.NonSerialized]
        protected int _iterations = 1;
        public int Iterations
        {
            get
            {
                return _iterations;
            }

            set
            {
                if (value < 0)
                    value = 0;

                _iterations = value;
                Renderer.Iterations = value;
            }
        }

        [System.NonSerialized]
        protected float _offset = 10f;
        public float Offset
        {
            get
            {
                return _offset;
            }

            set
            {
                if (value < 0f)
                    value = 0f;

                _offset = value;
                Renderer.Offset = value;
            }
        }

        [System.NonSerialized]
        protected Vector2Int _resolution = new Vector2Int(512, 512);
        public Vector2Int Resolution
        {
            get
            {
                return _resolution;
            }

            set
            {
                if (value.x < 2 || value.y < 2)
                    value = new Vector2Int(2, 2);

                _resolution = value;
                Renderer.Resolution = value;
            }
        }

        [System.NonSerialized]
        protected ShaderQuality _quality = ShaderQuality.Medium;
        public ShaderQuality Quality
        {
            get
            {
                return _quality;
            }

            set
            {
                _quality = value;
                Renderer.Quality = _quality;
            }
        }

        [System.NonSerialized]
        protected Color _additiveColor = new Color(0,0,0,0);
        public Color AdditiveColor
        {
            get
            {
                return _additiveColor;
            }

            set
            {
                _additiveColor = value;
                Renderer.AdditiveColor = _additiveColor;
            }
        }

        public void ApplyValues()
        {
            Renderer.Iterations = Iterations;
            Renderer.Offset = Offset;
            Renderer.Resolution = Resolution;
            Renderer.Quality = Quality;
            Renderer.AdditiveColor = AdditiveColor;
        }

        public void ApplyValues(BlurredBackgroundImage img)
        {
            Iterations = img.Iterations;
            Offset = img.Strength;
            Resolution = img.Resolution.ToResolution();
            Quality = img.Quality;
            AdditiveColor = img.AdditiveColor;

            ApplyValues();
        }

        public Texture GetBlurredTexture(RenderMode renderMode)
        {
            return Renderer.GetBlurredTexture(renderMode);
        }

        [System.NonSerialized]
        protected IBlurRenderer _renderer;
        public IBlurRenderer Renderer
        {
            get
            {
                if (_renderer == null)
                {
                    if (RenderPipelineDetector.IsBuiltIn())
                        _renderer = new BlurRendererBuiltIn(); // BuiltIn

#if KAMGAM_RENDER_PIPELINE_URP
                    if (RenderPipelineDetector.IsURP())
                        _renderer = new BlurRendererURP(); // URP
#endif
#if KAMGAM_RENDER_PIPELINE_HDRP
                    if (RenderPipelineDetector.IsHDRP())
                        _renderer = new BlurRendererHDRP(); // HDRP
#endif
                }
                return _renderer;
            }

            set
            {
                _renderer = value;
            }
        }

        /// <summary>
        /// Keeps track of how many elements use the blurred texture. If none are using it then
        /// the rendering will be paused to save performance.
        /// </summary>
        protected List<BlurredBackgroundImage> _images = new List<BlurredBackgroundImage>();

        protected BlurredBackgroundImage _lastRegisteredImage;

        public void RegisterImage(BlurredBackgroundImage img)
        {
            if (!_images.Contains(img))
            {
                _images.Add(img);
            }

            _lastRegisteredImage = img;

            if (Renderer != null)
            {
                Renderer.SetImage(img);
                Renderer.Active = shouldBeActive();
                RefreshRenderModeInfos();
            }



            // Make sure we recreate the Updater if needed.
            BlurManagerUpdater.Instance.Refresh();
        }

        /// <summary>
        /// Call this after you have changed one of your canvases render mode at runtime.
        /// </summary>
        public void RefreshRenderModeInfos()
        {
            if (Renderer != null)
            {
                _usesWorldOrCameraSpaceCanvases = null;
            }
        }

        protected bool? _usesWorldOrCameraSpaceCanvases;

        public bool UsesWorldOrCameraSpaceCanvases()
        {
            if (_usesWorldOrCameraSpaceCanvases.HasValue)
                return _usesWorldOrCameraSpaceCanvases.Value;

            foreach (var img in _images)
            {
                var renderMode = img.GetRenderMode();
                if (renderMode == RenderMode.ScreenSpaceCamera || renderMode == RenderMode.WorldSpace)
                {
                    _usesWorldOrCameraSpaceCanvases = true;
                    return true;
                }
            }

            _usesWorldOrCameraSpaceCanvases = false;
            return false;
        }

        public void UnregisterImage(BlurredBackgroundImage img)
        {
            if (_images.Contains(img))
            {
                _images.Remove(img);
            }

            if (Renderer != null)
            {
                Renderer.Active = shouldBeActive();
                RefreshRenderModeInfos();
            }
        }

        protected bool shouldBeActive()
        {
            // Count the visible elements
            int activeImages = 0;
            foreach (var img in _images)
            {
                if (img != null && img.gameObject != null && img.isActiveAndEnabled && img.gameObject.activeInHierarchy)
                {
                    activeImages++;
                    break;
                }
            }

            return activeImages > 0 && _iterations > 0 && _offset > 0.0f;
        }

        public void Update()
        {
            // Disable rendering is no elements with blurred background are visible.
            Renderer.Active = shouldBeActive();

            // Keep the renderer in sync with the current camera.
            if (Renderer.Active)
            {
                Renderer.Update();
            }
        }
    }
}