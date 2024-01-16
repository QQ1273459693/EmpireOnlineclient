using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Water2D
{

    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    public class ObstructorManager : MonoBehaviour
    {
        [SerializeField] [HideInInspector] public static ObstructorManager instance;

        #region Singleton

        private void Awake()
        {
            Singleton();
            UpdateReflectionsShader();
        }

        void Singleton()
        {
            if (instance == null) { instance = this; }
            else if (this == instance) return;
            else DestroyImmediate(gameObject);
        }

        public static ObstructorManager GetInstance()
        {
            if (instance == null)
            {
                if (FindObjectOfType<ObstructorManager>()!= null) instance = FindObjectOfType<ObstructorManager>();
                else throw new Exception("obstructor System Platformer Instance couldn't be found in the scene");
            }
            return instance;
        }
        #endregion

        #region Variables

        public void UpdateSettings(ObstructorSettings os)
        {
            obstructionObjectsVisible.value = os.obstructionObjectsVisible.value;
            cameraVisible.value = os.cameraVisible.value;
            textureResolution.value = os.textureResolution.value;
            mainCamera = os.mainCamera;
        }


        [HideInInspector][SerializeField] public WaterCryo<bool> overrideMainCamera = new WaterCryo<bool>(false);
        [HideInInspector][SerializeField] public WaterCryo<bool> obstructionObjectsVisible = new WaterCryo<bool>(false);
        [HideInInspector][SerializeField] public WaterCryo<bool> cameraVisible = new WaterCryo<bool>(false);
        [HideInInspector][SerializeField] public WaterCryo<float> textureResolution = new WaterCryo<float>(1);
        [HideInInspector][SerializeField] public Camera cam;

        [HideInInspector][SerializeField] private Camera _mainCamera;
        [HideInInspector][SerializeField] public Camera mainCamera
        {
            get { if (_mainCamera == null) SetMainCam(); return _mainCamera; }
            set { _mainCamera = value; }
        }

        public const string rlayer = "Obstructors";
        public const string redMatPath = "Materials/Red";
        
        private Material _red;
        public Material red
        {
            get { if (_red == null) _red = (Material)Resources.Load(redMatPath, typeof(Material));  return _red; }
            set { _red = value; }
        }

        [SerializeField][HideInInspector] LayerRenderer _layerRenderer;

        [SerializeField]
        [HideInInspector]
        public LayerRenderer layerRenderer
        {
            get
            {
                if (_layerRenderer == null)
                {
                    _layerRenderer = new LayerRenderer();
                    _layerRenderer.Setup(mainCamera, transform, rlayer, textureResolution.value, RenderTextureFormat.R8, 1.25f);
                }
                return _layerRenderer;
            }
            set { _layerRenderer = value; }
        }

        [SerializeField] Dictionary<Transform, ObstructorSO> _obstructors = new Dictionary<Transform, ObstructorSO>();

        [HideInInspector][SerializeField] bool _run;
        [HideInInspector] [SerializeField] internal bool run 
        {
            get { return _run; }
            set 
            {
                _layerRenderer.run = value;
                _run = value;
            }
        }

        Dictionary<Transform, ObstructorSO> obstructors
        {
            get { if (_obstructors == null) _obstructors = new Dictionary<Transform, ObstructorSO>(); return _obstructors; }
            set { _obstructors = value; }
        }


        // contains the created obstruction textures
        [SerializeField][HideInInspector] private static Dictionary<int, ObstructorPair> _obstructionSprites;

        private static Dictionary<int, ObstructorPair> obstructionSprites
        {
            get { if (_obstructionSprites == null) _obstructionSprites = new Dictionary<int, ObstructorPair>(); return _obstructionSprites; }
            set { _obstructionSprites = value; }
        }

        #endregion

        #region Callbacks

        void SetCallbacks()
        {
            overrideMainCamera.onValueChanged = OnSettingsChangedScene;
            obstructionObjectsVisible.onValueChanged = OnSettingsChangedScene;
            cameraVisible.onValueChanged = OnSettingsChangedScene;

        }

        private void OnSettingsChangedScene()
        {
            GetAllObstructors();
            ObstructionObjectsVisible(obstructionObjectsVisible.value);
        }

        private void ObstructionObjectsVisible(bool value)
        {
            foreach (var t in obstructors) t.Value.child.gameObject.hideFlags = value ? HideFlags.None : HideFlags.HideInHierarchy;
        }

        #endregion

        #region Common 

        public void AddObstructor(ObstructorSO obs)
        {
            if (!obstructors.ContainsKey(obs.source)) obstructors.Add(obs.source, obs);
        }

        public void RemoveObstructor(Transform t)
        {
            if (obstructors.ContainsKey(t)) obstructors.Remove(t);
        }

        public void GetAllObstructors()
        {
            foreach (var r in GameObject.FindObjectsOfType<Obstructor>(true))
            {
                if (!obstructors.ContainsKey(r.transform)) AddObstructor(r.GetComponent<Obstructor>().data);
            }
        }

        private void SetMainCam() 
        {
            if (_mainCamera == null) _mainCamera = Camera.main;
        } 

        #endregion

        #region Disable&Enable

        private void OnEnable()
        {
#if UNITY_EDITOR
            EditorApplication.update += Update;
#endif
            Singleton();
            SetCallbacks();
            if (mainCamera==null) mainCamera = Camera.main;
            layerRenderer.Setup(mainCamera, transform, rlayer, textureResolution.value,  RenderTextureFormat.R8, 1.25f);
            UpdateReflectionsShader();
        }


        private void Start()
        {
            GetAllObstructors();
        }

        private void OnDisable()
        {
            layerRenderer.Release();
#if UNITY_EDITOR
            EditorApplication.update -= Update;
#endif
        }

        #endregion

        #region Update

        public void Update()
        {
            _layerRenderer.run = run;
            _layerRenderer.Loop();
            if (!run) return;

            if (cam==null) cam = GetComponent<Camera>();
            cam.hideFlags = cameraVisible.value ? HideFlags.None : HideFlags.HideInInspector;

            UpdateObstructionSprites();
        }

        private void UpdateObstructionSprites()
        {
            foreach (var obs in obstructors)
            {
                if (obs.Key == null) continue;

                if (obs.Value.childSr == null || obs.Value.sourceSr == null) continue;
                if (obs.Value.childSr.flipX != obs.Value.sourceSr.flipX) obs.Value.childSr.flipX = obs.Value.sourceSr.flipX;

                //change/create and change sprite
                if (obs.Value.sourceSr.sprite != obs.Value.childSr.sprite)
                {
                    obs.Value.childSr.sprite = obs.Value.sourceSr.sprite;

                    MaterialPropertyBlock prop = new MaterialPropertyBlock();
                    obs.Value.childSr.GetPropertyBlock(prop);
                    float texH = obs.Value.childSr.sprite.texture.height;
                    prop.SetFloat("_ss", (obs.Value.childSr.sprite.texture.height <= obs.Value.childSr.sprite.rect.height ? 0 : 1));
                    prop.SetFloat("_minY", obs.Value.childSr.sprite.rect.yMin / texH);
                    prop.SetFloat("_maxY", obs.Value.childSr.sprite.rect.yMax / texH);
                    obs.Value.childSr.SetPropertyBlock(prop);
                }
            }
        }

        private void UpdateReflectionsShader()
        {
            if (!Shader.IsKeywordEnabled(WaterShaderIdsOBS.OBStexture)) Shader.SetGlobalTexture(WaterShaderIdsOBS.OBStexture, layerRenderer.LayerTexture());
        }

        #endregion

    }
}