using LeafUtilsRays;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LightRays2D
{

    [ExecuteAlways]
    public class CameraOpaque2DManager : MonoBehaviour
    {
        #region Singleton

        private void OnEnable()
        {
            if (_instance != null && _instance != this) Destroy(gameObject);
            else if (_instance == null) _instance = this;

#if UNITY_EDITOR
            EditorApplication.update += Update;
#endif
        }

        CameraOpaque2DManager _instance;
        public CameraOpaque2DManager instance
        {
            get { if (_instance == null) _instance = FindObjectOfType<CameraOpaque2DManager>(); return _instance; }
            set { _instance = value; }
        }

        #endregion

        #region MonoBehaviour

        private void Update()
        {
            UpdateCamera();
            Material lightRaysMat = (Material)Resources.Load("Materials/light rays 2d");
            lightRaysMat.SetMatrix("_projMat", opaqueCam.projectionMatrix);
            lightRaysMat.SetMatrix("_worldToCam", opaqueCam.worldToCameraMatrix);
            lightRaysMat.SetTexture("_screenTex", opaqueTex);

#if UNITY_EDITOR
            if (!Layers.LayerExists(LightRays2D.layer))
            {
                Layers.CreateLayer(LightRays2D.layer);
                SetCameraSettings(cam, opaqueCam);
            }
#endif
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            EditorApplication.update -= Update;
#endif
            if (opaqueTex != null) opaqueTex.Release();
        }

        #endregion

        #region StaticManager

        public const string textureID = "leafOpaque2D";

        [SerializeField][HideInInspector] private static Camera _cam;
        public static Camera cam
        {
            get
            {
                if (overrideMainCamera.value && overrideCamera != null && overrideCamera != _cam) _cam = overrideCamera;
                else if (!overrideMainCamera.value) _cam = Camera.main;
                else if (overrideMainCamera.value && overrideCamera == null) Debug.LogError("override camera is not set in LightRays2D component");
                return _cam;
            }
            set { _cam = value; }
        }

        [SerializeField][HideInInspector] private static Camera _opaqueCam;
        public static Camera opaqueCam
        {
            get
            {
                if (_opaqueCam == null) _opaqueCam = FindObjectOfType<CameraOpaque2DManager>() == null ? null : FindObjectOfType<CameraOpaque2DManager>().GetComponent<Camera>();
                if (_opaqueCam == null) _opaqueCam = CreateCamera();
                return _opaqueCam;
            }
            set { _opaqueCam = value; }
        }

        [SerializeField][HideInInspector] private static RenderTexture _opaqueTex;
        public static RenderTexture opaqueTex
        {
            get
            {
                if (_opaqueTex == null) _opaqueTex = CreateRenderTexture(cam, opaqueCam);
                else if (_opaqueTex != null && (!RenderTextureSettingsValidation(_opaqueTex))) { _opaqueTex.Release(); _opaqueTex = CreateRenderTexture(cam, opaqueCam); }
                return _opaqueTex;
            }
            set { _opaqueTex = value; }
        }

        [SerializeField][HideInInspector] public static Cryo<bool> overrideMainCamera = new Cryo<bool>(false);
        public static Camera overrideCamera;

        //settings to change
        [SerializeField][HideInInspector] public static FilterMode filterMode = FilterMode.Bilinear;

        //function is meant to be added as a listener to one object that needs opaque texture
        public static bool updating = false;
        public static void UpdateCamera()
        {
            if (!CameraExistenceValidation(cam, opaqueCam)) ResetCameras();
            if (!CameraSettingsValidation(cam, opaqueCam)) SetCameraSettings(cam, opaqueCam);
            if (!RenderTextureSettingsValidation(opaqueTex)) opaqueTex = CreateRenderTexture(cam, opaqueCam);
            opaqueCam.targetTexture = opaqueTex;
        }

        private static Camera CreateCamera()
        {
            GameObject resG = new GameObject("OpaqueCamera");
            Camera res = resG.AddComponent<Camera>();
            resG.transform.parent = cam.transform;
            resG.transform.localPosition = Vector3.zero;
            resG.AddComponent<CameraOpaque2DManager>();
            return res;
        }


        private static void SetCameraSettings(Camera mainCamera, Camera opaqueCamera)
        {
            opaqueCamera.orthographicSize = mainCamera.orthographicSize;
            opaqueCamera.orthographic = mainCamera.orthographic;
            opaqueCamera.aspect = mainCamera.aspect;
            opaqueCamera.backgroundColor = mainCamera.backgroundColor;
            opaqueCamera.clearFlags = mainCamera.clearFlags;
            opaqueCamera.cullingMask = mainCamera.cullingMask;
            mainCamera.cullingMask = BitUtils.SetBit(mainCamera.cullingMask, LayerMask.NameToLayer(LightRays2D.layer));
            opaqueCamera.cullingMask = BitUtils.RemoveBit(mainCamera.cullingMask, LayerMask.NameToLayer(LightRays2D.layer));
            opaqueCamera.targetTexture = opaqueTex;
        }


        private static RenderTexture CreateRenderTexture(Camera mainCamera, Camera opaqueCamera)
        {
            RenderTexture tex = new RenderTexture(ScreenUtils.ScreenResolution().x, ScreenUtils.ScreenResolution().y, 0, RenderTextureFormat.ARGB32, 0);
            tex.filterMode = filterMode;
            tex.Create();
            Material lightRaysMat = (Material)Resources.Load("Materials/light rays 2d");
            lightRaysMat.SetTexture("_screenTex", tex);
            return tex;
        }

        private static bool RenderTextureSettingsValidation(RenderTexture tex)
        {
            if ((tex.width != ScreenUtils.ScreenResolution().x || tex.height != ScreenUtils.ScreenResolution().y)) return false;
            if (tex.filterMode != filterMode) return false;
            return true;
        }

        private static bool CameraExistenceValidation(Camera mainCamera, Camera opaqueCamera)
        {
            if (mainCamera != overrideCamera && overrideCamera != null && overrideMainCamera.value) return false;
            if (mainCamera.transform != opaqueCam.transform.parent) return false;
            return true;
        }

        private static void ResetCameras()
        {
            DestroyUtils.DestroyAlways(_opaqueCam.gameObject);
            _opaqueCam = CreateCamera();
        }

        private static bool CameraSettingsValidation(Camera mainCamera, Camera opaqueCamera)
        {
            if (mainCamera.orthographicSize != opaqueCamera.orthographicSize) return false;
            if (mainCamera.orthographic != opaqueCamera.orthographic) return false;
            if (mainCamera.aspect != opaqueCamera.aspect) return false;
            if (opaqueCamera.backgroundColor != mainCamera.backgroundColor) return false;
            if (opaqueCamera.clearFlags != mainCamera.clearFlags) return false;
            if (opaqueCamera.targetTexture == null) return false;
            if (!BitUtils.IsBitSet(mainCamera.cullingMask, LayerMask.NameToLayer(LightRays2D.layer))) return false;
            else if (BitUtils.IsBitSet(opaqueCamera.cullingMask, LayerMask.NameToLayer(LightRays2D.layer))) return false;
            return true;
        }

        #endregion

    }

}