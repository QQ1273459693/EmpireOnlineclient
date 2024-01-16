#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Water2D
{
    [ExecuteAlways]
    [RequireComponent(typeof(SpriteRenderer))]
    public class ModernWater2D : MonoBehaviour
    {
        [HideInInspector][SerializeField] public static ModernWater2DSettings defaultSettings;

        [HideInInspector][SerializeField] public SimulationType _waterSimulationType = SimulationType.basic;
        [HideInInspector][SerializeField] private WaterSimulation _waterSimulation;
        [HideInInspector][SerializeField] private static ObstructorManager _obstructorManager;
        [HideInInspector][SerializeField] private static ReflectionsSystem _reflectionsManagerPlatformer;
        [HideInInspector][SerializeField] private static ReflectionsSystem _reflectionsManagerTopDown;

        public static ObstructorManager obstructorManager
        {
            get
            {
                if (_obstructorManager == null)
                {
                    if (FindObjectOfType<ObstructorManager>() != null) _obstructorManager = FindObjectOfType<ObstructorManager>();
                    else _obstructorManager = new GameObject("ObstructorManager").AddComponent<ObstructorManager>();
                }

                _obstructorManager.transform.parent = managersParent;
                return _obstructorManager;
            }
            set { }
        }
        public static ReflectionsSystem reflectionsManagerPlatformer
        {
            get
            {
                if (_reflectionsManagerPlatformer == null)
                {
                    foreach (var system in FindObjectsOfType<ReflectionsSystem>(true)) if (!system.topdown) _reflectionsManagerPlatformer = system;
                }
                if (_reflectionsManagerPlatformer == null)
                {
                    _reflectionsManagerPlatformer = new GameObject("ReflectionsManagerPL").AddComponent<ReflectionsSystem>();
                }
                _reflectionsManagerPlatformer.topdown = false;
                _reflectionsManagerPlatformer.transform.parent = managersParent;
                return _reflectionsManagerPlatformer;
            }
            set { _reflectionsManagerPlatformer = value; }
        }
        public static ReflectionsSystem reflectionsManagerTopDown
        {
            get
            {
                if (_reflectionsManagerTopDown == null)
                {
                    foreach (var system in FindObjectsOfType<ReflectionsSystem>(true)) if (system.topdown) _reflectionsManagerTopDown = system;
                    if (_reflectionsManagerTopDown == null)
                    {
                        _reflectionsManagerTopDown = new GameObject("ReflectionsManagerTD").AddComponent<ReflectionsSystem>();
                    }
                }
                _reflectionsManagerTopDown.topdown = true;
                _reflectionsManagerTopDown.transform.parent = managersParent;
                return _reflectionsManagerTopDown;
            }
            set { _reflectionsManagerTopDown = value; }
        }

        public WaterSimulation waterSimulation
        {
            get 
            {   
                if (_waterSimulation == null) SetWaterSim(ref _waterSimulation);
                return _waterSimulation;
            }
            set { _waterSimulation = value; }
        }

        [HideInInspector][SerializeField] private LayerRenderer _childPPLayerRenderer;

        //child for shader double pass
        [HideInInspector][SerializeField] private GameObject _childPP;
        public GameObject childPP
        {
            get
            {
                if (_childPP == null && (settings._blurSettings.useBlur.value)) CreateChildPP();
                return _childPP;
            }
            set
            { 
                _childPP = value; 
            }
        }

        [HideInInspector][SerializeField] private LayerRenderer _surfaceRenderer;

        //child for shader double pass
        [HideInInspector][SerializeField] public GameObject _surfaceRendererObject;
        public GameObject surfaceRendererObject
        {
            get
            {
                if (_surfaceRendererObject == null && (settings._waterSettings.enableBelowWater.value)) CreateChildSurfaceRenderer();
                return _surfaceRendererObject;
            }
            set
            {
                _surfaceRendererObject = value;
            }
        }

        [SerializeField][HideInInspector] public WaterCryo<bool> ManagersVisible = new WaterCryo<bool>(false);
        [HideInInspector][SerializeField] public WaterCryo<bool> enableObstruction = new WaterCryo<bool>(true);
        [HideInInspector][SerializeField] public WaterCryo<bool> enableReflections = new WaterCryo<bool>(true);
        [HideInInspector][SerializeField] public WaterCryo<bool> enableSimulation = new WaterCryo<bool>(false);
        [HideInInspector][SerializeField] public WaterCryo<bool> enableBlur = new WaterCryo<bool>(false);
        [HideInInspector][SerializeField] public WaterCryo<bool> normalsPreview = new WaterCryo<bool>(false);
        [HideInInspector][SerializeField] public WaterCryo<bool> overrideMainCamera = new WaterCryo<bool>(false);
        [HideInInspector][SerializeField] public Camera cameraOverride;

        [HideInInspector][SerializeField] public ModernWater2DSettings settings = new ModernWater2DSettings();
        [HideInInspector][SerializeField] public bool customWaterMaterial;

        [HideInInspector][SerializeField] Material _mat;
        [HideInInspector][SerializeField] public Material mat 
        {
            set { _mat = value; }
            get { if (_mat == null) _mat = new Material( Shader.Find("ModernWater2D/waterg") );  return _mat; }
        }

        [HideInInspector][SerializeField] Material _matb;
        [HideInInspector][SerializeField] public Material matb
        {
            set { _matb = value; }
            get { if (_matb == null) OnBlurMaterialChanged(); return _matb; }
        }


        [HideInInspector][SerializeField] private static Transform _managersParent;
        public static Transform managersParent
        {
            get
            {
                if (_managersParent == null)
                {
                    if (FindObjectOfType<ManagersParent>(true) != null) _managersParent = FindObjectOfType<ManagersParent>().transform;
                    else { _managersParent = new GameObject(managersParentName).transform; if (!_managersParent.GetComponent<ManagersParent>()) _managersParent.gameObject.AddComponent<ManagersParent>(); }
                }
                return _managersParent;
            }
            set { _managersParent = value; }
        }

        [HideInInspector][SerializeField] private SpriteRenderer _sr;

        public SpriteRenderer sr
        {
            get { if (_sr==null) _sr = GetComponent<SpriteRenderer>(); SetMaterials(); return _sr; }
            set { _sr = value; }
        }

        const string managersParentName = "2DWaterManagers";
        const string srLayer = "Water";
        const string sr2Layer = "WaterPostProcessing";

        private void OnEnable()
        {
            //record current resolution
            resolution = new Vector2(Screen.width, Screen.height);

            //create instance of water material
            mat = new Material(sr.sharedMaterial);
            sr.sharedMaterial = mat;
            sr.sharedMaterial.SetTexture("_simTex", (Texture2D)Resources.Load("Sprites/placeholders/blackTex"));

            //setup layer 
            SetLayers();

            //setup cryo class callbacks and water managers
            SetupManagers();
            SetCallbacks();

            //set camera matrices for blur sampling
            CameraSetup();

            //setup blur
            OnBlurMaterialChanged();


        }

        //sets the water layer for water
        void SetLayers() 
        {
#if UNITY_EDITOR
            if (!WaterLayers.LayerExists(srLayer)) WaterLayers.CreateLayer(srLayer);
            if(!WaterLayers.LayerExists(sr2Layer)) WaterLayers.CreateLayer(sr2Layer);
#endif
            gameObject.layer = Obstructor.GetLayerIdx(srLayer);
            SetCameraLayers();
            CreateDestroyPostProcessingCamera();
            CreateDestroySurfaceRenderingCamera();
        }

        void CreateDestroyPostProcessingCamera()
        {
            if (settings._blurSettings.useBlur.value && _childPP==null) CreateChildPP();
            else if (!settings._blurSettings.useBlur.value && _childPP != null) DestroyImmediate(_childPP);
        }

        void CreateDestroySurfaceRenderingCamera()
        {
            if (settings._waterSettings.enableBelowWater.value && _surfaceRendererObject == null) CreateChildSurfaceRenderer();
            else if (!settings._waterSettings.enableBelowWater.value && _surfaceRendererObject != null) DestroyImmediate(_surfaceRendererObject);
        }

        void CreateChildPP()
        {
            _childPP = new GameObject(name + " post processing");
            SpriteRenderer sr = _childPP.AddComponent<SpriteRenderer>();

            SetCameraAboveWaterTransform(_childPP.transform,5f);

            sr.color = Color.white;
            sr.sprite = this.sr.sprite;
            sr.sharedMaterial = matb;
            sr.sortingLayerName = this.sr.sortingLayerName; 
            sr.sortingOrder = this.sr.sortingOrder + 1;

            _childPP.AddComponent<Camera>().CopyFrom(GetCameraRenderingScreen());
    
            if (_childPPLayerRenderer == null) _childPPLayerRenderer = new LayerRenderer();
            _childPPLayerRenderer.Setup(GetCameraRenderingScreen(),sr, _childPP.transform, srLayer);

#if UNITY_EDITOR
            if (!WaterLayers.LayerExists(sr2Layer)) WaterLayers.CreateLayer(sr2Layer);
#endif
            _childPP.layer = Obstructor.GetLayerIdx(sr2Layer);

            SetCameraAboveWaterTransform(_childPP.transform,5f);
        }

        private void SetCameraAboveWaterTransform(Transform t, float deltaZ) 
        {
            t.parent = transform;
            t.localPosition = new Vector3(0, 0, -Mathf.Abs(deltaZ));
            t.rotation = Quaternion.identity;
            t.localScale = Vector3.one;
        }

        public void CreateChildSurfaceRenderer()
        {
            _surfaceRendererObject = new GameObject(name + " surface renderer");
            SetCameraAboveWaterTransform(_surfaceRendererObject.transform,3f);
            _surfaceRendererObject.AddComponent<Camera>().CopyFrom(Camera.main);

            if (_surfaceRenderer == null) _surfaceRenderer = new LayerRenderer();

            int bitmask = GetCameraRenderingScreen().cullingMask; //get main camera bitmask
            bitmask &= ~(1 << Obstructor.GetLayerIdx(srLayer));   //remove water
            bitmask &= ~(1 << Obstructor.GetLayerIdx(sr2Layer));  //remove water post processing

            _surfaceRenderer.Setup(GetCameraRenderingScreen(), sr, _surfaceRendererObject.transform, bitmask);
            SetCameraAboveWaterTransform(_surfaceRendererObject.transform,3f);



            sr.sharedMaterial.SetTexture("_belowWaterTex", _surfaceRenderer.LayerTexture());
        }

        private Camera GetCameraRenderingScreen() 
        {
            if (overrideMainCamera.value && cameraOverride != null) return cameraOverride;
            else return Camera.main; 
        }

        //includes or excludes the water layer in mainCamera 
        void SetCameraLayers() 
        {
            //include normal, exclude post processing layer
            if (settings._blurSettings.useBlur.value)
            {
                Camera.main.cullingMask &= ~(1 <<  Obstructor.GetLayerIdx(srLayer));
                Camera.main.cullingMask |= (1 <<  Obstructor.GetLayerIdx(sr2Layer));
            }
            else //inverses
            {
                Camera.main.cullingMask |= (1 <<  Obstructor.GetLayerIdx(srLayer));
                Camera.main.cullingMask &= ~(1 << Obstructor.GetLayerIdx(sr2Layer));
            }
        }

        public void SetWaterSim(ref WaterSimulation _waterSimulation)
        {
            switch (_waterSimulationType)
            {
                case SimulationType.basic:
                    _waterSimulation = new WaterSimulationSimple(); break;
                case SimulationType.advanced:
                    _waterSimulation = new WaterSimulationAdvanced(); break;
                default:
                    _waterSimulation = new WaterSimulationSimple(); break;
            }
        }


        private void Start()
        {
            if(Application.isPlaying)sr.sharedMaterial.SetTexture("_simTex", waterSimulation.GetRT());
        }


        void SetupManagers()
        {
            ObstructorManager.instance = obstructorManager;

            SimulationSetup();

            OnWaterChanged();
            OnReflectionsChanged();
            OnObstructionChanged();
            OnOSimulationChanged();
        }

        private void OnResolutionChanged() 
        {
          
        }

        Vector2 resolution;

        bool CheckForResolutionChanged()
        {
            if (resolution.x != Screen.width || resolution.y != Screen.height)
            {
                resolution = new Vector2(Screen.width, Screen.height);
                return true;
            }
            return false;
        }

        void SimulationSetup() 
        {
            settings._simulationSettings.sr = _sr;
            settings._simulationSettings.obstruction = ObstructorManager.instance.layerRenderer.LayerTexture();
            waterSimulation.Setup(settings._simulationSettings);
            sr.sharedMaterial.SetTexture("_simTex", waterSimulation.GetRT());
        }

        void CameraSetup() 
        {
            Camera cam = GetCameraRenderingScreen();
            sr.sharedMaterial.SetMatrix("_projectionMatrix", cam.projectionMatrix);
            sr.sharedMaterial.SetMatrix("_worldToCamMatrix", cam.worldToCameraMatrix);
            sr.sharedMaterial.SetVector("_camRect", new Vector4(cam.rect.x,cam.rect.y,cam.rect.width,cam.rect.height));
            sr.sharedMaterial.SetVector("_camSize", new Vector2(cam.pixelWidth, cam.pixelHeight));
            sr.sharedMaterial.SetVector("_obs_transform", !cam.orthographic ? new Vector4(1f, 1f, 0f, 0f) : new Vector4(0.8f, 0.8f, 0.1f, 0.1f));

        }

        void SetCallbacks()
        {
            enableObstruction.onValueChanged = OnWaterChanged;
            overrideMainCamera.onValueChanged = OnCameraSettingsChanged;
            enableReflections.onValueChanged = OnWaterChanged;
            enableSimulation.onValueChanged = OnWaterChanged;
            ManagersVisible.onValueChanged = OnInspectorSettingsChanged;

            settings._reflectionsSettings.onValueChanged(OnReflectionsChanged);
            settings._simulationSettings.onValueChanged(OnOSimulationChanged);
            settings._obstructorSettings.onValueChanged(OnObstructionChanged);
            settings._waterSettings.onValueChanged(OnWaterChanged);
            settings._blurSettings.onValueChanged(OnBlurChanged);
        }
        public void OnCameraSettingsChanged() 
        {
            bool changeFlag = false;
            if(overrideMainCamera.value == false)
            {
                settings._obstructorSettings.mainCamera = Camera.main;
                settings._reflectionsSettings.mainCamera = Camera.main;
                settings._simulationSettings.mainCam = Camera.main;
                changeFlag = true;
            }
            else if (cameraOverride != null)
            {
                settings._obstructorSettings.mainCamera = cameraOverride;
                settings._reflectionsSettings.mainCamera = cameraOverride;
                settings._simulationSettings.mainCam = cameraOverride;
                changeFlag = true;
            }

            if(changeFlag)
            {
                OnReflectionsChanged();
                OnObstructionChanged();
                OnObstructionChanged();
                CameraSetup();
            }
        }

        void OnOSimulationChanged()
        {
            SetWaterSim(ref _waterSimulation);
            if (!enableSimulation.value) return;
            waterSimulation.UpdateSettings(settings._simulationSettings);
            sr.sharedMaterial.SetFloat("_normStr", settings._simulationSettings.normalStrength.value);
            sr.sharedMaterial.SetColor("_simFoamColor", settings._simulationSettings.waveColor.value);
            sr.sharedMaterial.SetVector("_simMinMaxWavesHeightFoam", settings._simulationSettings.waveColorMinMaxHeight.value);
            sr.sharedMaterial.SetTexture("_simTex", waterSimulation.GetRT());
        }

        void OnObstructionChanged()
        {
            if (!enableObstruction.value) return;
            obstructorManager.UpdateSettings(settings._obstructorSettings);
        }

        void OnInspectorSettingsChanged() 
        {
            managersParent.gameObject.hideFlags = (ManagersVisible.value ? HideFlags.None : HideFlags.HideInHierarchy);
        }

        public void OnBlurMaterialChanged()
        {
            SetLayers();
            switch (settings._blurSettings.blurType)
            {
                case BlurSettings.BlurType.box:
                    matb = new Material(Shader.Find("hidden/box"));  
                    matb.name = "box blur";
                    break;
                case BlurSettings.BlurType.gaussian:
                    matb = new Material(Shader.Find("hidden/gaussian"));
                    matb.name = "gaussian blur";
                    break;
                case BlurSettings.BlurType.bokeh:
                    matb = new Material(Shader.Find("hidden/bokeh"));
                    matb.name = "bokeh blur";
                    break;
            }
            SetMaterials();
            OnBlurChanged();
            
            //don't work on all urp versions
            //if(settings._blurSettings.blurType == BlurSettings.BlurType.gaussian || settings._blurSettings.blurType == BlurSettings.BlurType.box) SetupTwoPass();
        }

        void SetMaterials() 
        {
            if (!settings._blurSettings.useBlur.value) return;
            SpriteRenderer sr = childPP.GetComponent<SpriteRenderer>();
            sr.sharedMaterial = matb;
        }

        void OnBlurChanged()
        {
            CreateDestroyPostProcessingCamera();
            SetLayers();
            if (!settings._blurSettings.useBlur.value) return;

            SpriteRenderer sr = childPP.GetComponent<SpriteRenderer>();

            switch (settings._blurSettings.blurType)
            {
                case BlurSettings.BlurType.box:
                    sr.sharedMaterial.SetTexture("_MainTex2", _childPPLayerRenderer.LayerTexture());
                    sr.sharedMaterial.SetInt("_area", settings._blurSettings.boxSamplingRange.value);
                    sr.sharedMaterial.SetFloat("_sigmaX", settings._blurSettings.boxStrength.value);
                    break;
                case BlurSettings.BlurType.gaussian:
                    sr.sharedMaterial.SetTexture("_MainTex2", _childPPLayerRenderer.LayerTexture());
                    sr.sharedMaterial.SetInt("_area", settings._blurSettings.gaussianSamplingRange.value);
                    sr.sharedMaterial.SetFloat("_sigmaX", settings._blurSettings.gaussianStrengthX.value);
                    break;
                case BlurSettings.BlurType.bokeh:
                    sr.sharedMaterial.SetTexture("_MainTex2", _childPPLayerRenderer.LayerTexture());
                    sr.sharedMaterial.SetFloat("_area", settings._blurSettings.bokehArea.value);
                    sr.sharedMaterial.SetFloat("_ratio", settings._blurSettings.bokehRatio.value);
                    sr.sharedMaterial.SetFloat("_hardness", settings._blurSettings.bokehHardness.value);
                    sr.sharedMaterial.SetFloat("_gamma", settings._blurSettings.bokehGamma.value);
                    sr.sharedMaterial.SetInt("_quality", settings._blurSettings.bokehQuality.value);
                    break;
            }

            sr.sharedMaterial.SetFloat("_falloffS", settings._blurSettings.falloffStart.value);
            sr.sharedMaterial.SetFloat("_falloffE", settings._blurSettings.falloffEnd.value);
            sr.sharedMaterial.SetFloat("_falloffP", settings._blurSettings.falloffStrength.value);
            sr.sharedMaterial.SetInt("_falloffU", settings._blurSettings.useFalloff.value ? 1 : 0);
        }


        void OnWaterChanged()
        {
            //set material
      
            sr.sharedMaterial = mat;
            //enable or disable cameras
            
            reflectionsManagerTopDown.run = enableReflections.value && settings._reflectionsSettings.enableTopDownReflections.value;
            reflectionsManagerPlatformer.run = enableReflections.value && settings._reflectionsSettings.enablePlatformerReflections.value;
            obstructorManager.run = enableObstruction.value;

            //set material variables

            sr.sharedMaterial.SetInt("_enable_obs", enableObstruction.value ? 1 : 0);
            sr.sharedMaterial.SetInt("_enable_sim", enableSimulation.value ? 1 : 0);
            sr.sharedMaterial.SetInt("_enable_nor", normalsPreview.value ? 1 : 0);
            sr.sharedMaterial.SetInt("_enable_ref", enableReflections.value ? 1 : 0);

            sr.sharedMaterial.SetColor("_color", settings._waterSettings.color.value);
            sr.sharedMaterial.SetFloat("_surfaceAlpha", settings._waterSettings.baseAlpha.value);
            sr.sharedMaterial.SetTexture("_alphaTexture", settings._waterSettings.alphaTexture);
            sr.sharedMaterial.SetVector("_tiling", settings._waterSettings.tiling.value);

            sr.sharedMaterial.SetFloat("_num_of_pixels", settings._waterSettings.numOfPixels.value);
            sr.sharedMaterial.SetInt("_pixel_perfect", settings._waterSettings.pixelPerfect.value ? 1 : 0);
            sr.sharedMaterial.SetInt("_lighting", settings._waterSettings._useLighting.value ? 1 : 0);

            sr.sharedMaterial.SetFloat("_obstruction_width", settings._waterSettings.obstructionWidth.value);
            sr.sharedMaterial.SetColor("_obstruction_color", settings._waterSettings.obstructionColor.value);
            sr.sharedMaterial.SetFloat("_obstruction_alpha", settings._waterSettings.obstructionAlpha.value);

            sr.sharedMaterial.SetColor("_deep_color", settings._waterSettings.depthColor.value);

            sr.sharedMaterial.SetColor("_foam_color", settings._waterSettings.foamColor.value);
            sr.sharedMaterial.SetFloat("_foam_size", settings._waterSettings.foamSize.value);
            sr.sharedMaterial.SetVector("_foam_speed", settings._waterSettings.foamSpeed.value);
            sr.sharedMaterial.SetFloat("_foam_density", settings._waterSettings.foamDensity.value);
            sr.sharedMaterial.SetFloat("_foam_alpha", settings._waterSettings.foamAlpha.value);

            CreateDestroySurfaceRenderingCamera();


            if (settings._waterSettings.enableBelowWater.value) sr.sharedMaterial.SetTexture("_belowWaterTex", _surfaceRenderer.LayerTexture());
            else sr.sharedMaterial.SetTexture("_belowWaterTex", null);
            sr.sharedMaterial.SetVector("_belowWaterTexUV",new Vector4(0f,0f,1f,1f));
            sr.sharedMaterial.SetFloat("_belowWaterTexDistortionStrength", settings._waterSettings.belowWaterDistortionStrength.value);
            sr.sharedMaterial.SetFloat("_belowWaterTexAlpha", settings._waterSettings.enableBelowWater.value? settings._waterSettings.belowWaterAlpha.value : 0f);

            sr.sharedMaterial.SetVector("_distortion_speed", settings._waterSettings.distortionSpeed.value);
            sr.sharedMaterial.SetVector("_distortion_strength", settings._waterSettings.distortionStrength.value);
            sr.sharedMaterial.SetVector("_distortion_tiling", settings._waterSettings.distortionTiling.value);
            sr.sharedMaterial.SetVector("_distortion_color", settings._waterSettings.distortionColor.value);
            sr.sharedMaterial.SetVector("_distortion_minmax", settings._waterSettings.distortionMinMax.value);
            sr.sharedMaterial.SetTexture("_distortion_tex", settings._waterSettings.distortionTexture);

            sr.sharedMaterial.SetTexture("_surfaceTex", settings._waterSettings.surfaceTexture);
            sr.sharedMaterial.SetFloat("_surfaceTexAlpha", settings._waterSettings.surfaceAlpha.value);
            sr.sharedMaterial.SetVector("_surfaceTexTiling", settings._waterSettings.surfaceTiling.value);
            sr.sharedMaterial.SetVector("_surfaceTexSpeed", settings._waterSettings.surfaceSpeed.value);
            sr.sharedMaterial.SetFloat("_useFoamSpeedForST", settings._waterSettings.useFoamSpeed.value ? 1.0f : 0.0f);
            sr.sharedMaterial.SetVector("_surfaceTexUV", new Vector4(0f,0f,1f,1f));

            sr.sharedMaterial.SetTexture("_sun_strips", settings._waterSettings.sunStripsTexture);
            sr.sharedMaterial.SetFloat("_strips_speed", settings._waterSettings.stripsSpeed.value);
            sr.sharedMaterial.SetFloat("_strips_scrolling_speed", settings._waterSettings.stripsScrollingSpeed.value);
            sr.sharedMaterial.SetFloat("_strips_size", settings._waterSettings.stripsSize.value);
            sr.sharedMaterial.SetFloat("_strips_alpha", settings._waterSettings.stripsAlpha.value);
            sr.sharedMaterial.SetFloat("_strips_density", settings._waterSettings.stripsDensity.value);
            sr.sharedMaterial.SetFloat("_strips_density", settings._waterSettings.stripsDensity.value);
        }

        void OnReflectionsChanged()
        {
            if (!enableReflections.value) return;

            sr.sharedMaterial.SetInt("_enable_td", settings._reflectionsSettings.enableTopDownReflections.value ? 1 : 0);
            sr.sharedMaterial.SetInt("_enable_pl", settings._reflectionsSettings.enablePlatformerReflections.value ? 1 : 0);
            sr.sharedMaterial.SetInt("_distortionFPRH", settings._reflectionsSettings.DistortionFPRH.value ? 1 : 0);

            reflectionsManagerTopDown.UpdateSettings(settings._reflectionsSettings,true);
            reflectionsManagerPlatformer.UpdateSettings(settings._reflectionsSettings,false);

            sr.sharedMaterial.SetFloat("_reflectionY", settings._reflectionsSettings.mirrorY.value);

            sr.sharedMaterial.SetInt("_usePerspective", settings._reflectionsSettings.usePerspective.value ? 1 : 0);
            sr.sharedMaterial.SetVector("_perspective", settings._reflectionsSettings.waterPerspective.value);
            sr.sharedMaterial.SetVector("_perspective2", settings._reflectionsSettings.reflectionsPerspective.value);

            sr.sharedMaterial.SetInt("_enableFalloff", settings._reflectionsSettings.enableFalloff.value ? 1 : 0);
            sr.sharedMaterial.SetFloat("_falloffStrength", settings._reflectionsSettings.falloffStrength.value);
            sr.sharedMaterial.SetFloat("_falloffStart", settings._reflectionsSettings.falloffStart.value);

            sr.sharedMaterial.SetInt("_enable_scrolling", settings._reflectionsSettings.enableScrolling.value ? 1 : 0);
            sr.sharedMaterial.SetFloat("_scrStrength", settings._reflectionsSettings.scrollingStrength.value);
            if (settings._reflectionsSettings.playerPosition != null) sr.sharedMaterial.SetVector("_playerPosition", settings._reflectionsSettings.playerPosition.position);
        }

        private void Update()
        {
            if(settings._reflectionsSettings.playerPosition != null) sr.sharedMaterial.SetVector("_playerPosition", settings._reflectionsSettings.playerPosition.position);
            CameraSetup();
            SurfaceSetup();
            if (enableSimulation.value) waterSimulation.UpdLoop();
            if (CheckForResolutionChanged()) OnResolutionChanged();
        }
        private void SurfaceSetup() 
        {
            if (settings._waterSettings.surfaceSprite != null)
            {
                sr.sharedMaterial.SetTexture("_surfaceTex", settings._waterSettings.surfaceSprite.sprite.texture);

                float x0 = sr.bounds.min.x;
                float y0 = sr.bounds.min.y;
                float x1 = sr.bounds.max.x;
                float y1 = sr.bounds.max.y;

                float xp0 = settings._waterSettings.surfaceSprite.bounds.min.x;
                float yp0 = settings._waterSettings.surfaceSprite.bounds.min.y;
                float xp1 = settings._waterSettings.surfaceSprite.bounds.max.x;
                float yp1 = settings._waterSettings.surfaceSprite.bounds.max.y;

                Vector4 uvs = new Vector4((x0 - xp0) / xp1, (x1 - xp0) / xp1, (y0 - yp0) / yp1, (y1 - yp0) / yp1);
                sr.sharedMaterial.SetVector("_surfaceTexUV", uvs );
            }
        }

        private void FixedUpdate()
        {
            if (enableSimulation.value) waterSimulation.Loop();
        }

        private void OnDrawGizmos()
        {
            
            if(waterSimulation!=null) waterSimulation.OnGizmos();

            //draw reflection plane if platformer reflections are on
            if (!settings._reflectionsSettings.enablePlatformerReflections.value) return;
            float x0 = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 10f)).x;
            float x1 = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, 10f)).x;
            float y = Mathf.Lerp(Camera.main.ViewportToWorldPoint(new Vector3(1f, 0f, 10f)).y,Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, 10f)).y, settings._reflectionsSettings.mirrorY.value);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(new Vector3(x0, y), new Vector3(x1, y));
        }

    }
}


