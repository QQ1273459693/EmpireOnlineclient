#if KAMGAM_RENDER_PIPELINE_URP
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace Kamgam.UGUIBlurredBackground
{
    public class BlurRendererURP : IBlurRenderer
    {
        public event System.Action OnPostRender;

        public BlurredBackgroundImage _image;

        /// <summary>
        /// Sets the image that is controlling the blur properties.
        /// </summary>
        /// <param name="image"></param>
        public void SetImage(BlurredBackgroundImage image)
        {
            _image = image;
        }

        protected BlurredBackgroundPassURP _screenSpacePass;
        public BlurredBackgroundPassURP ScreenSpacePass
        {
            get
            {
                if (_screenSpacePass == null)
                {
                    _screenSpacePass = new BlurredBackgroundPassURP();
                    // NOTICE: This is now overridden in onBeginCameraRendering().
                    _screenSpacePass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

                    _screenSpacePass.OnPostRender += onPostRender;
                }
                return _screenSpacePass;
            }
        }

        // The world pass is optional and only create if there are world or camera space canvases.
        protected BlurredBackgroundPassURP _worldOrCameraSpacePass;
        public BlurredBackgroundPassURP WorldOrCameraSpacePass
        {
            get
            {
                if (_worldOrCameraSpacePass == null)
                {
                    _worldOrCameraSpacePass = new BlurredBackgroundPassURP();
                    _worldOrCameraSpacePass.renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;

                    // Since the world space pass is created on-demand we have to hand over all the config values to sync it.
                    if (_screenSpacePass != null)
                    {
                        _worldOrCameraSpacePass.Active = _screenSpacePass.Active;
                        _worldOrCameraSpacePass.Iterations = _screenSpacePass.Iterations;
                        _worldOrCameraSpacePass.Offset = _screenSpacePass.Offset;
                        _worldOrCameraSpacePass.Resolution = _screenSpacePass.Resolution;
                        _worldOrCameraSpacePass.Quality = _screenSpacePass.Quality;
                    }
                }
                return _worldOrCameraSpacePass;
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

                    ScreenSpacePass.Active = value;

                    if (_worldOrCameraSpacePass != null)
                    {
                        _worldOrCameraSpacePass.Active = value;
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

                ScreenSpacePass.Iterations = value;

                if (_worldOrCameraSpacePass != null)
                    WorldOrCameraSpacePass.Iterations = value;
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

                ScreenSpacePass.Offset = value;

                if (_worldOrCameraSpacePass != null)
                    WorldOrCameraSpacePass.Offset = value;
            }
        }

        protected Vector2Int _resolution = new Vector2Int(512, 512);
        public Vector2Int Resolution
        {
            get
            {
                return _resolution;
            }
            set
            {
                _resolution = value;

                ScreenSpacePass.Resolution = value;

                if (_worldOrCameraSpacePass != null)
                    WorldOrCameraSpacePass.Resolution = value;
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

                ScreenSpacePass.Quality = value;

                if (_worldOrCameraSpacePass != null)
                    WorldOrCameraSpacePass.Quality = value;
            }
        }

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

                ScreenSpacePass.AdditiveColor = value;

                if (_worldOrCameraSpacePass != null)
                    WorldOrCameraSpacePass.AdditiveColor = value;
            }
        }

        /// <summary>
        /// The material is used in screen space overlay canvases.
        /// </summary>
        public Material GetMaterial(RenderMode renderMode)
        {
            if (renderMode == RenderMode.ScreenSpaceOverlay)
                return ScreenSpacePass.Material;
            else
                return WorldOrCameraSpacePass.Material;
        }

        public Texture GetBlurredTexture(RenderMode renderMode)
        {
            if (renderMode == RenderMode.ScreenSpaceOverlay)
                return ScreenSpacePass.GetBlurredTexture();
            else
                return WorldOrCameraSpacePass.GetBlurredTexture();
        }

        public BlurRendererURP()
        {
            RenderPipelineManager.beginCameraRendering += onBeginCameraRendering;

            if (ScreenSpacePass != null)
                ScreenSpacePass.OnPostRender += onPostRender;

            // Needed to avoid "Render Pipeline error : the XR layout still contains active passes. Executing XRSystem.EndLayout() right" Errors in Unity 2023
            // Also needed in normal URP to reset the render textures after play mode.
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged += onPlayModeChanged;
            UnityEditor.SceneManagement.EditorSceneManager.sceneOpened += onSceneOpened;
#endif
        }

        ~BlurRendererURP()
        {
            if (_screenSpacePass != null)
                _screenSpacePass.OnPostRender -= onPostRender;
        }

        protected void clearRenderTargets()
        {
            _screenSpacePass?.ClearRenderTargets();
            _worldOrCameraSpacePass?.ClearRenderTargets();
        }

#if UNITY_EDITOR
        void onPlayModeChanged(UnityEditor.PlayModeStateChange obj)
        {
            if (obj == UnityEditor.PlayModeStateChange.ExitingPlayMode || obj == UnityEditor.PlayModeStateChange.EnteredEditMode)
            {
                clearRenderTargets();
            }
        }

        void onSceneOpened(Scene scene, OpenSceneMode mode)
        {
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                clearRenderTargets();
            }
        }
#endif

        const string MainCameraTag = "MainCamera";
        const string Renderer2DTypeName = "Renderer2D";

        void onBeginCameraRendering(ScriptableRenderContext context, Camera cam)
        {
            if (    cam == null
                || !cam.isActiveAndEnabled
                || !cam.CompareTag(MainCameraTag))
                return;

            var data = cam.GetUniversalAdditionalCameraData();

            if (data == null)
                return;

            // Turns out the list is always empty and the enqueing is a per frame action (TODO: investigate)

            // Check if we are using the 2D renderer (skip check if already using "BeforeRenderingPostProcessing" event).
            if (cam.orthographic && ScreenSpacePass.renderPassEvent == RenderPassEvent.AfterRenderingPostProcessing)
            {
                if (cam.GetUniversalAdditionalCameraData().scriptableRenderer.GetType().Name.EndsWith(Renderer2DTypeName))
                {
                    // If yes then change the event from AfterRenderingPostProcessing to BeforeRenderingPostProcessing.
                    // Sadly accessing PostPro render results is not supported in URP 2D, see:
                    // https://forum.unity.com/threads/urp-2d-how-to-access-camera-target-after-post-processing.1465124/
                    // https://forum.unity.com/threads/7-3-1-renderpassevent-afterrenderingpostprocessing-is-broken.873604/#post-8422710
                    ScreenSpacePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
                }
            }

            data.scriptableRenderer.EnqueuePass(ScreenSpacePass);

            if (_worldOrCameraSpacePass != null)
            {
                data.scriptableRenderer.EnqueuePass(WorldOrCameraSpacePass);
            }
        }

        protected void onPostRender()
        {
            OnPostRender?.Invoke();
        }

        /// <summary>
        /// Not needed in SRPs.
        /// </summary>
        public void Update()
        {
        }
    }
}
#endif