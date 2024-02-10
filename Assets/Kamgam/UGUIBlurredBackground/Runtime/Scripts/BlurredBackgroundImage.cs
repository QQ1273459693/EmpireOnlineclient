using UnityEngine;
using UnityEngine.UI;

namespace Kamgam.UGUIBlurredBackground
{
    [ExecuteAlways]
    [AddComponentMenu("UI/Kamgam/Blurred Background Image")]
    public partial class BlurredBackgroundImage : Image
    {
        public RenderMode GetRenderMode()
        {
            var renderMode = RenderMode.ScreenSpaceOverlay;

            if (canvas == null)
                renderMode = RenderMode.ScreenSpaceOverlay; 
            else
                renderMode = canvas.renderMode;

            // Check if the camera matches the render mode and if not alter the render mode.
            if (renderMode == RenderMode.ScreenSpaceCamera && canvas != null)
            {
                Camera cam = canvas.worldCamera;
                // If no camera is specified then it will behave like screen scpae OVERLAY.
                if (cam == null)
                    renderMode = RenderMode.ScreenSpaceOverlay;
            }

            return renderMode;
        }

        protected override void OnCanvasHierarchyChanged()
        {
            base.OnCanvasHierarchyChanged();
            SetVerticesDirty(); 
        }

        protected override void Awake()
        {
            base.Awake();

            BlurManager.Instance.RegisterImage(this);
#if UNITY_EDITOR
            refreshAfterLoadInEditor();
#endif
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            BlurManager.Instance.RegisterImage(this);
            BlurManager.Instance.ApplyValues(this);
            BlurManager.Instance.Renderer.OnPostRender += onPostRender;
        }

        protected override void Start()
        {
            base.Start();
            SetVerticesDirty();
        }

#if UNITY_EDITOR
        void refreshAfterLoadInEditor() 
        {
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode && !EditorScheduler.HasId("UGUIBlurredBg.EditorRefresh"))
            {
                EditorScheduler.Schedule(0.2f, () =>
                {
                    //if (this == null || gameObject == null)
                    {
                      //  return;
                    }

                    SetVerticesDirty();
                    EditorUtils.RefreshGameView();
                },
                "UGUIBlurredBg.EditorRefresh");
            }
        }
#endif

        [System.NonSerialized]
        protected Matrix4x4 _lastWorldToLocalMatrix = Matrix4x4.identity;

        [System.NonSerialized]
        protected Matrix4x4 _lastWorldToCameraMatrix = Matrix4x4.identity;

        protected void onPostRender()
        {
            if (this == null || this.gameObject == null)
                return;

            // Since the texture is buffered (two render textures swappping all the time) we have
            // to update the used texture after every render. Using only the overridden "mainTexture"
            // sadly is not enough.
            var texture = BlurManager.Instance.GetBlurredTexture(GetRenderMode());
            if (texture != null)
                canvasRenderer.SetTexture(texture);

            // Update UVs on world space canvases if camera changed or any movement happened.
            if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
            {
                var cam = RenderUtils.GetGameViewCamera(this);

#if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlaying)
                {
                    SetVerticesDirty();
                }
#endif

                if (cam != null && cam.cameraType == CameraType.Game && cam.worldToCameraMatrix != _lastWorldToCameraMatrix)
                {
                    // Camera changed
                    _lastWorldToCameraMatrix = cam.worldToCameraMatrix;
                    SetVerticesDirty();
                }
            }

            // Update UVs if the transforms changed (pos, rot, scale).
            if (transform.worldToLocalMatrix != _lastWorldToLocalMatrix)
            {
                // World canvas changed
                _lastWorldToLocalMatrix = transform.worldToLocalMatrix;
                SetVerticesDirty();
            }
        }

        public override Texture mainTexture
        {
            get
            {
                var texture = BlurManager.Instance.GetBlurredTexture(GetRenderMode());
                if (texture != null)
                    return texture;
                else
                    return base.mainTexture;
            }
        }

        protected override void OnDisable()
        {
            if (BlurManager.HasInstance())
            {
                BlurManager.Instance.UnregisterImage(this);

                if (BlurManager.Instance.Renderer != null)
                    BlurManager.Instance.Renderer.OnPostRender -= onPostRender;
            }

            material = null;

            base.OnDisable();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}