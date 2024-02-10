#if KAMGAM_RENDER_PIPELINE_HDRP
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Kamgam.UGUIBlurredBackground
{
    /// <summary>
    /// Screens space pass is always created. WorldAndCamera space pass only when needed.
    /// </summary>
    public class BlurRendererHDRP : IBlurRenderer
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

        protected BlurredBackgroundPassHDRPVolume _screenSpaceVolume;
        public BlurredBackgroundPassHDRPVolume ScreenSpaceVolume
        {
            get
            {
                if (_screenSpaceVolume == null || _screenSpaceVolume.gameObject == null)
                {
                    var volume = BlurredBackgroundPassHDRPVolume.FindOrCreate(CustomPassInjectionPoint.AfterPostProcess);
                    if(volume != null)
                    {
                        var pass = volume.GetOrCreatePass(Quality, Resolution, Offset, Iterations, AdditiveColor);

                        // Since the screen space buffer may be created on-demand (in the editor) we have to hand over all the config values to sync it.
                        pass.enabled = Active;
                        pass.Iterations = Iterations;
                        pass.Offset = Offset;
                        pass.Resolution = Resolution;
                        pass.Quality = Quality;
                        pass.AdditiveColor = AdditiveColor;

                        // listen for render event
                        pass.OnPostRender += onPostRender;
                    }
                    _screenSpaceVolume = volume;
                }
                return _screenSpaceVolume;
            }
        }

        protected void onPostRender()
        {
            OnPostRender?.Invoke();
        }

        protected BlurredBackgroundPassHDRP getScreenSpacePass()
        {
            if (ScreenSpaceVolume == null)
                return null;

            return ScreenSpaceVolume.GetOrCreatePass(Quality, Resolution, Offset, Iterations, AdditiveColor);
        }

        protected BlurredBackgroundPassHDRPVolume _worldAndCameraSpaceVolume;
        public BlurredBackgroundPassHDRPVolume WorldAndCameraSpaceVolume
        {
            get
            {
                if (_worldAndCameraSpaceVolume == null || _worldAndCameraSpaceVolume.gameObject == null)
                {
                    var volume = BlurredBackgroundPassHDRPVolume.FindOrCreate(CustomPassInjectionPoint.BeforePreRefraction);
                    if (volume != null)
                    {
                        var pass = volume.GetOrCreatePass(Quality, Resolution, Offset, Iterations, AdditiveColor);

                        // Since the world space buffer is created on-demand we have to hand over all the config values to sync it.
                        var overlayPass = ScreenSpaceVolume.GetOrCreatePass(Quality, Resolution, Offset, Iterations, AdditiveColor);
                        if (overlayPass != null)
                        {
                            pass.enabled = overlayPass.enabled;
                            pass.Iterations = overlayPass.Iterations;
                            pass.Offset = overlayPass.Offset;
                            pass.Resolution = overlayPass.Resolution;
                            pass.Quality = overlayPass.Quality;
                            pass.AdditiveColor = overlayPass.AdditiveColor;
                        }
                    }
                    _worldAndCameraSpaceVolume = volume;
                }

                return _worldAndCameraSpaceVolume;
            }
        }

        protected BlurredBackgroundPassHDRP getWorldSpacePass()
        {
            if (WorldAndCameraSpaceVolume == null)
                return null;

            return WorldAndCameraSpaceVolume.GetOrCreatePass(Quality, Resolution, Offset, Iterations, AdditiveColor);
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

                    if (_screenSpaceVolume != null)
                    {
                        _screenSpaceVolume.enabled = value;
                    }

                    if (_worldAndCameraSpaceVolume != null)
                    {
                        _worldAndCameraSpaceVolume.enabled = value;
                    }
                }
            }
        }

        protected int _iterations = 1;
        public int Iterations
        {
            get
            {
                return _iterations;
            }

            set
            {
                _iterations = value;

                if (_screenSpaceVolume != null)
                    getScreenSpacePass().Iterations = value;

                if (_worldAndCameraSpaceVolume != null)
                    getWorldSpacePass().Iterations = value;
            }
        }

        protected float _offset = 1.5f;
        public float Offset
        {
            get
            {
                return _offset;
            }

            set
            {
                _offset = value;

                if (_screenSpaceVolume != null)
                    getScreenSpacePass().Offset = value;

                if (_worldAndCameraSpaceVolume != null)
                    getWorldSpacePass().Offset = value;
            }
        }

        protected  Vector2Int _resolution = new Vector2Int(512, 512);
        public Vector2Int Resolution
        {
            get
            {
                return _resolution;
            }
            set
            {
                _resolution = value;

                if (_screenSpaceVolume != null)
                    getScreenSpacePass().Resolution = value;

                if (_worldAndCameraSpaceVolume != null)
                    getWorldSpacePass().Resolution = value;
            }
        }

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

                if (_screenSpaceVolume != null)
                    getScreenSpacePass().Quality = value;

                if (_worldAndCameraSpaceVolume != null)
                    getWorldSpacePass().Quality = value;
            }
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

                if (_screenSpaceVolume != null)
                    getScreenSpacePass().AdditiveColor = value;

                if (_worldAndCameraSpaceVolume != null)
                    getWorldSpacePass().AdditiveColor = value;
            }
        }

#if UNITY_EDITOR
        // Used to memorize that we have to also create the world space pass volume.
        [System.NonSerialized]
        protected bool _requestedWorldSpacePass;
#endif

        public Texture GetBlurredTexture(RenderMode renderMode)
        {
#if UNITY_EDITOR
            if (EditorPlayState.IsInBetween) 
                return null;
#endif

            if (renderMode == RenderMode.ScreenSpaceOverlay)
            {
                return getScreenSpacePass().GetBlurredTexture();
            }
            else 
            {
#if UNITY_EDITOR
                _requestedWorldSpacePass = true;
#endif
                return getWorldSpacePass().GetBlurredTexture();
            }
        }

        public BlurRendererHDRP()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged += playModeChanged;
#endif
        }

#if UNITY_EDITOR
        protected void playModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode || state == PlayModeStateChange.ExitingPlayMode)
            {
                if (_screenSpaceVolume != null)
                {
                    GameObject.DestroyImmediate(_screenSpaceVolume.gameObject);
                    _screenSpaceVolume = null;
                }
                if (_worldAndCameraSpaceVolume != null)
                {
                    GameObject.DestroyImmediate(_worldAndCameraSpaceVolume.gameObject);
                    _worldAndCameraSpaceVolume = null;
                }
            }
            else if (state == PlayModeStateChange.EnteredPlayMode)
            {
                getScreenSpacePass();
                if (_requestedWorldSpacePass)
                    getWorldSpacePass();
            }
            else if (state == PlayModeStateChange.EnteredEditMode)
            {
                getScreenSpacePass();
                if (_requestedWorldSpacePass)
                    getWorldSpacePass();
            }
        }
#endif

        public void ClearVolumeCache()
        {
            _screenSpaceVolume = null;
            _worldAndCameraSpaceVolume = null;
        }

        /// <summary>
        /// Called in the Update loop.
        /// </summary>
        public void Update()
        {
        }

        ~BlurRendererHDRP()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged -= playModeChanged;
#endif

            if (_screenSpaceVolume != null && _screenSpaceVolume.GetPass() != null)
            {
                _screenSpaceVolume.GetPass().OnPostRender -= onPostRender;
                Utils.SmartDestroy(_screenSpaceVolume);
                _screenSpaceVolume = null;
            }

            if (_worldAndCameraSpaceVolume != null)
            {
                Utils.SmartDestroy(_worldAndCameraSpaceVolume.gameObject);
                _worldAndCameraSpaceVolume = null;
            }
        }
    }
}
#endif