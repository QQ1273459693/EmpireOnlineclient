using UnityEngine;
using UnityEngine.Rendering;
using LeafUtils;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SmoothCam
{

    [ExecuteAlways] //  works in editor too
    [RequireComponent(typeof(Camera))]  //  requires camera to work  
    [DefaultExecutionOrder(-100)]
    public class SmoothPixelPerfectCamera : MonoBehaviour
    {
        // 1. sets cameras and full screen quad
        // 2. renders output to render texture
        // 3. renders full screen quad to second camera

        //debug
        [HideInInspector] public bool debug = false;
        [HideInInspector] public bool debugMov = true;
        [HideInInspector] public Vector2 debugSpeed = new Vector2(0f, 0f);

        //constants

        // padding added to pixel perfect texture
        // in order to have smooth camera functionality
        const int padding = 2;

        // keeps the pixel perfect camera and quad away from scene objects
        // (only 1000 units to avoid floating-point errors in rendering)
        public static Vector3 ppCamPos = new Vector3(0, 0, -5000); 

        //variables
        [Header("settings")]

        [Space(10)]
        public const bool forcePixelPerfectResolution = true;
        [SerializeField][HideInInspector] public Cryo<bool> smoothCamera = new Cryo<bool>(true);
        [SerializeField][HideInInspector] public Cryo<int> pixelsPerUnit = new Cryo<int>(32);    //pixels per unit of your sprites


        [SerializeField][HideInInspector] public ScalingMode scalingMode = ScalingMode.targetCameraSize;
        [SerializeField][HideInInspector] public FilteringMode filteringMode = FilteringMode.point;
        [SerializeField][HideInInspector] public Cryo<float> targetCameraSize = new Cryo<float>(3); 
        [SerializeField][HideInInspector] public Cryo<bool> enableCameraMaxSizeDifference = new Cryo<bool>(true); 
        [SerializeField][HideInInspector] public Cryo<float> maxCameraSizeDifference = new Cryo<float>(2f); 
        [SerializeField][HideInInspector] public Cryo<Vector2Int> targetResolution = new Cryo<Vector2Int>(new Vector2Int(320, 180));     //pixel perfect resolution you want

        [SerializeField][HideInInspector] public Cryo<bool> enableCustomMargins = new Cryo<bool>(false);

        [Space(10)]
        [SerializeField][HideInInspector] public cropFrame crop = cropFrame.none;

        [SerializeField][HideInInspector] public RenderTexture rt;                  //  pixel perfect texture with padding
        [SerializeField][HideInInspector] public RenderTexture rtScaled;            //  upscaled pixel perfect texture with padding
        [SerializeField][HideInInspector] public RenderTexture rtScaledAndCropped;  //  upscaled screen size pixel perfect texture without padding
        [SerializeField][HideInInspector] public Vector2 positionDelta;   //  delta of camera from pixel grid

        [SerializeField][HideInInspector] public Shader marginsShader;

        [SerializeField][HideInInspector] Material _blitMat;    
        [SerializeField][HideInInspector] Material _SPPCamMat;   
        [SerializeField][HideInInspector] Material _MarginsMat;   

        [SerializeField][HideInInspector] Camera _orgCam;      //  this object camera
        [SerializeField][HideInInspector] Camera _cam2;        //  camera rendering final texture
        [SerializeField][HideInInspector] GameObject _quad;    //  quad for final texture
        
        [SerializeField][HideInInspector] public Vector2 delta;
        [SerializeField][HideInInspector] public Vector2 factorIdeal;
        [SerializeField][HideInInspector] public Vector2 factorReal;

#if UNITY_EDITOR
        [SerializeField][HideInInspector] public AnimBoolGenerator gen = new AnimBoolGenerator(); //used to save editor foldouts state even after closing the editor window
#endif

        [SerializeField][HideInInspector] public bool showCameraData; //used to save camera data foldout state even after closing the editor window

        SPPCinemachineSupport cinemachineSupport;

        [SerializeField][HideInInspector] static Camera _Camera;
        [SerializeField] [HideInInspector] public static Camera Camera 
        {
            get { return _Camera; }
            set { _Camera = value; }
        }

        //getters and setters
        [System.Obsolete]
        Camera orgCam
        {
            get { if (_orgCam == null) _orgCam = GetComponent<Camera>(); return _orgCam; }
            set { _orgCam = value; }
            
        }
        [System.Obsolete]
        Camera cam2
        {
            get
            {
                if (_cam2 == null) CreateFullScreenCamera();
                return _cam2;
            }
            set { _cam2 = value; if (orgCam == Camera.main) Camera = value; }
        }
        [System.Obsolete]
        GameObject quad
        {
            get { if (_quad == null) CreateFullScreenQuad(); return _quad; }
            set { _quad = value; }
        }
        Material blitMat
        {
            get { if (_blitMat == null) { _blitMat = new Material(Shader.Find("Unlit/rescale")); _blitMat.SetTexture("_screenTex", rt); } return _blitMat; }
            set { _blitMat = value; }
        }
        Material SPPCamMat
        {
            get { if (_SPPCamMat == null) { _SPPCamMat = new Material(Shader.Find("SPPC2D/SmoothPixelPerfectCamera"));  } return _SPPCamMat; }
            set { _SPPCamMat = value; }
        }
        Material MarginsMat
        {
            get 
            {
                if ((marginsShader != null) && (_MarginsMat==null || (marginsShader.name != _MarginsMat.shader.name)) ) _MarginsMat = new Material(marginsShader);
                else if (marginsShader == null) _MarginsMat = null;

                return _MarginsMat; 
            }
            set { _MarginsMat = value; }
        }

        float scaleX;   //  upscale x-ratio
        float scaleY;   //  upscale y-ratio
        [SerializeField][HideInInspector] public int cropMarginX = 0;
        [SerializeField][HideInInspector] public int cropMarginY = 0;
        public Vector2Int cropMargins() => new Vector2Int(cropMarginX, cropMarginY);
        bool frame = false; //  quick-fix, don't touch 

        [System.Obsolete]
        private void SetCallbacks() 
        {
            targetResolution.onValueChanged = OnTargetResolutionChanged;
            pixelsPerUnit.onValueChanged = OnSettingsChanged;
            targetCameraSize.onValueChanged = OnSettingsChanged;
            maxCameraSizeDifference.onValueChanged = OnSettingsChanged;
            enableCameraMaxSizeDifference.onValueChanged = OnSettingsChanged;
        }

        //keeps the resolution divisible by two (needed for calculations and pixel perfect movement)
        [System.Obsolete]
        private void OnTargetResolutionChanged() 
        {
            SetTargetResolutionToDivBy2();
            OnSettingsChanged();
        }

        [System.Obsolete]
        public void onCropSettingsChanged()
        {
            SetTargetResolutionToDivBy2();
            OnSettingsChanged();
        }

        [System.Obsolete]
        private void OnSettingsChanged()
        {
            CameraSetup();
            ExtenstionsSetup();
        }


        [System.Obsolete]
        private void SetTargetResolutionToDivBy2() 
        {

            if (targetResolution.value.x % 2 != 0) targetResolution.value += new Vector2Int(1, 0);
            if (targetResolution.value.y % 2 != 0) targetResolution.value += new Vector2Int(0, 1);
        }

        //creates a full screen camera rendering out pixel art scene 
        [System.Obsolete]
        private void CreateFullScreenCamera()
        {
            //clean
            for (int i = 0; i < transform.childCount; i++) if (transform.GetChild(i).name == name + "( for canvas )") if (Application.isPlaying) Destroy(transform.GetChild(i).gameObject); else DestroyImmediate(transform.GetChild(i).gameObject);

            //create
            GameObject go = new GameObject(name + "( for canvas )");
            go.transform.parent = this.transform;

            Camera cam = go.AddComponent<Camera>();
            cam.CopyFrom(orgCam);
            cam.orthographicSize = orgCam.orthographicSize;
            cam.aspect = ScreenUtils.ScreenResolution().x / (float)ScreenUtils.ScreenResolution().y;

            cam.depth = orgCam.depth + 1;
            go.transform.localPosition = ppCamPos + new Vector3(0, 0, -2f);
            cam.targetTexture = null;
            go.hideFlags = HideFlags.HideInHierarchy;

            cam2 = cam;
        }

        [System.Obsolete]
        public float AspectRatio()
        {
            Vector2 res = AddMargins(CalculateResolution());
            return res.x / res.y;
        }

        ////////////////////////////////////////////////////////////////////////    Resolution Calcs

        Vector2Int screenResolution;
        [SerializeField][HideInInspector] public Vector2Int ppRes;
        int factorCrop = 1;
        /// <summary>
        /// Function that calculates the size of pixel perfect render texture with all data taken into account
        /// </summary>
        /// <returns></returns>
        [System.Obsolete]
        private Vector2Int CalculateResolution() 
        {
            screenResolution = ScreenUtils.ScreenResolution();
            float aspect = (screenResolution.x / (float)screenResolution.y);

            //target resoultion
            Vector2Int targetResolution = this.targetResolution.value;

            //camera size target resolution
            if (scalingMode == ScalingMode.targetCameraSize)
            {
                targetResolution = new Vector2(aspect * pixelsPerUnit.value * targetCameraSize.value * 2, pixelsPerUnit.value * targetCameraSize.value * 2).Vector2Int();
                SetResoultionDiv2(ref targetResolution);
                this.targetResolution.On = false;
                this.targetResolution.value = targetResolution;
                this.targetResolution.On = true;
            }

            //for pillarbox and letterbox x/y screen fill
            CalculateFactors(screenResolution, targetResolution);

            //cropping on
            if (crop != cropFrame.none) 
            {
                //pillarbox (y to max)
                if (crop == cropFrame.pllarbox)
                {
                    targetResolution = new Vector2Int(targetResolution.x, (int)(screenResolution.y / (float)factorCrop));
                }

                //letterbox (x to max)
                if (crop == cropFrame.letterbox)
                {
                    targetResolution = new Vector2Int((int)(screenResolution.x / (float)factorCrop), targetResolution.y);
                }

                CalculateCropMargin(screenResolution,targetResolution);
                return targetResolution;
            }

            //cropping off
            targetResolution = StrictPixelPefrectCalc(screenResolution, targetResolution);
            SetResoultionDiv2(ref targetResolution);

          

            // max size delta
            if (MinMaxRequired(targetResolution))
            {
                targetResolution = CalculateMinMaxedResolution(targetResolution, aspect);
                SetResoultionDiv2(ref targetResolution);
                CalculateFactors(screenResolution, targetResolution);
                CalculateCropMargin(screenResolution, targetResolution);
                stretchAnyway = true;
            }
            else stretchAnyway = false;


            return targetResolution;
        }

        [System.Obsolete]
        private bool MinMaxRequired(Vector2Int targetResolution)
        {
            if (enableCameraMaxSizeDifference.value)
            {
                float camsize = (float)targetResolution.y / pixelsPerUnit.value / 2f;
                float targetCamSize = scalingMode == ScalingMode.targetCameraSize ? targetCameraSize.value : this.targetResolution.value.y / 2f;

                if ((camsize > targetCamSize + maxCameraSizeDifference.value) || (camsize < targetCamSize - maxCameraSizeDifference.value)) return true;
                else return false;

            }
            else return false;
        }

        [SerializeField] [HideInInspector] bool stretchAnyway = false;
        [System.Obsolete]
        private Vector2Int CalculateMinMaxedResolution(Vector2Int targetResolution, float aspect)
        {
            if (enableCameraMaxSizeDifference.value)
            {
                float camsize = (float)targetResolution.y / pixelsPerUnit.value / 2f;
                float targetCamSize = scalingMode == ScalingMode.targetCameraSize ? targetCameraSize.value : this.targetResolution.value.y / 2f / pixelsPerUnit.value;

                if (camsize > targetCamSize + maxCameraSizeDifference.value )
                {
                    stretchAnyway = true;
                    float newRes = (targetCamSize + maxCameraSizeDifference.value  ) * pixelsPerUnit.value;
                    targetResolution = new Vector2(aspect * newRes * 2f , newRes * 2f).Vector2Int();
                }
                else if (camsize < targetCamSize - maxCameraSizeDifference.value )
                {
                    stretchAnyway = true;
                    float newRes = (targetCamSize - maxCameraSizeDifference.value  ) * pixelsPerUnit.value;
                    targetResolution = new Vector2(aspect * newRes * 2f, newRes * 2f).Vector2Int();
                }
                else
                { stretchAnyway = false; }
            }
            return targetResolution;
        }

        [System.Obsolete]
        private void SetResoultionDiv2(ref Vector2Int res)
        {
            if (res.x % 2 != 0) res += new Vector2Int(1, 0);
            if (res.y % 2 != 0) res += new Vector2Int(0, 1);
        }

        [System.Obsolete]
        private Vector2Int CalculateResolutionUpscaled(Vector2Int targetResolution)
        {
            return new Vector2Int((rt.width) * factorCrop, (rt.height) * factorCrop);
        }

        [System.Obsolete]
        private void CalculateFactors(Vector2Int screenResolution, Vector2Int targetResolution) 
        {
            float factorX = (float)screenResolution.x / (float)targetResolution.x;
            float factorY = (float)screenResolution.y / (float)targetResolution.y;

            int factorXInt = Mathf.Max(1, Mathf.FloorToInt(factorX));
            int factorYInt = Mathf.Max(1, Mathf.FloorToInt(factorY));

            if (factorXInt < factorYInt) factorCrop = factorYInt = factorXInt;
            else factorCrop = factorXInt = factorYInt;
        }

        [System.Obsolete]
        private void CalculateCropMargin(Vector2Int screenResolution, Vector2Int targetResolution) 
        {
            CalculateFactors(screenResolution, targetResolution);
            this.delta = Vector2.zero;
            cropMarginX = (screenResolution.x - (factorCrop * targetResolution.x)) / 2;
            cropMarginY = (screenResolution.y - (factorCrop * targetResolution.y)) / 2;
        }

        [System.Obsolete]
        Vector2Int StrictPixelPefrectCalc(Vector2Int screenResolution, Vector2Int targetResolution)
        {
            Vector2Int res = (crop == cropFrame.none ?  GetClosestResolution(screenResolution, targetResolution) : GetClosestResolutionBiggerThanTarget(screenResolution, targetResolution) );

            factorCrop = screenResolution.x / res.x;

            if (crop != cropFrame.none)
            {
                cropMarginX = (screenResolution.x - (factorCrop * targetResolution.x)) / 2;
                cropMarginY = (screenResolution.y - (factorCrop * targetResolution.y)) / 2;
            }

            this.delta = Vector2.zero;
            this.factorIdeal = new Vector2(Mathf.FloorToInt((float)screenResolution.x + 000.1f) / res.x , Mathf.FloorToInt((float)screenResolution.y + 000.1f) / res.y);
            this.factorReal = new Vector2((float)screenResolution.x / (float)targetResolution.x, (float)screenResolution.y / (float)targetResolution.y);
            return res;
        }

        [System.Obsolete]
        Vector2Int GetClosestResolution(Vector2Int screen, Vector2Int target) 
        {
            int idx; //last index in array with valid element
            Vector2Int[] resolutions = StrictResolutions(screen, out idx);
            
            Vector2Int closest = screen;
            int mindelta = Mathf.Abs(screen.x - target.x);

            for (int i = 0; i <= idx; i++)
            {
                if (Mathf.Abs(resolutions[i].x - target.x) < mindelta)
                {
                    mindelta = Mathf.Abs(resolutions[i].x - target.x);
                    closest = resolutions[i];
                }
            }
            return closest;
        }

        [System.Obsolete]
        Vector2Int GetClosestResolutionBiggerThanTarget(Vector2Int screen, Vector2Int target)
        {
            int idx; //last index in array with valid element
            Vector2Int[] resolutions = StrictResolutions(screen, out idx);

            Vector2Int closest = screen;
            int mindelta = Mathf.Abs(screen.x - target.x);

            for (int i = 0; i <= idx; i++)
            {
                if (Mathf.Abs(resolutions[i].x - target.x) < mindelta && resolutions[i].x > target.x && resolutions[i].y > target.y)
                {
                    mindelta = Mathf.Abs(resolutions[i].x - target.x);
                    closest = resolutions[i];
                }
            }
            return closest;
        }

        [System.Obsolete]
        Vector2Int[] StrictResolutions(Vector2Int res, out int idx) 
        {
            idx = -1;
            float epsilon = 0.01f;
            Vector2Int[] resolutions = new Vector2Int[32];
            for(float i = 0; i < 32; i++)
            {
                if (Mathf.Abs(res.x / i - Mathf.Round(res.x / i)) < epsilon && Mathf.Abs(res.y / i - Mathf.Round(res.y / i)) < epsilon)
                    resolutions[++idx] = new Vector2Int(res.x / (int)i, res.y / (int)i);
            }
            return resolutions;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        //creates a full screen quad rendering out screen texture
        [System.Obsolete]
        private void CreateFullScreenQuad()
        {
            //clean
            for (int i = 0; i < transform.childCount; i++) if (transform.GetChild(i).name == "quadR") if (Application.isPlaying) Destroy(transform.GetChild(i).gameObject); else DestroyImmediate(transform.GetChild(i).gameObject);

            GameObject go = Instantiate(Resources.Load("prefabs/SPPC2DQuad") as GameObject);
            go.name = "quadR";
            go.hideFlags = HideFlags.HideInHierarchy;
            go.transform.parent = transform;
            go.transform.localPosition = ppCamPos + new Vector3(0, 0, -1f);
            go.transform.localScale = new Vector3(cam2.orthographicSize * 2f * cam2.aspect, cam2.orthographicSize * 2f, 1);
            _quad = go;

        }

        [System.Obsolete]
        private void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += UpscaleRT;
            Canvas.willRenderCanvases += PreRenderCanvas;
            RenderPipelineManager.endCameraRendering += AfterRenderCanvas;
            RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
#if UNITY_EDITOR
            EditorApplication.update += Update;
#endif
            ExtenstionsSetup();
            CameraSetup();

        }

        [System.Obsolete]
        void ExtenstionsSetup() 
        {
            if (SPPCinemachineSupport.RequireSupport(orgCam))
            {
                if (cinemachineSupport == null) cinemachineSupport = new SPPCinemachineSupport(gameObject);
                cinemachineSupport.CinemachineStart(PixelPerfectCameraSize());
            }
            else if (cinemachineSupport != null) cinemachineSupport = null;
        }

        #region Setup



        [System.Obsolete]
        private void CameraSetup() 
        {
            SetCallbacks();

            Resize();
            CreateRenderTextures();

            //setup camera and blit material
            blitMat.SetTexture("_screenTex", rt);
            _orgCam = GetComponent<Camera>();

            //create quad and second camera
            if (_cam2 != null) GameObject.DestroyImmediate(cam2.gameObject);
            if (_quad != null) GameObject.DestroyImmediate(quad);
            CreateScreenRenderTexture();
            CreateFullScreenCamera();
            CreateFullScreenQuad();

            //hide unimportant information
            if (!debug)
            {
                cam2.gameObject.hideFlags = HideFlags.HideInHierarchy;
                quad.hideFlags = HideFlags.HideInHierarchy;
            }
            else
            {
                cam2.gameObject.hideFlags = HideFlags.None;
                quad.hideFlags = HideFlags.None;
            }

            SetQuadShader();
        }

     
        #endregion
        [System.Obsolete]
        private void RenderUpscaledAndCroppedTexture()
        {
            if (rtScaled == null || rtScaledAndCropped == null) CreateRenderTextures();

            SPPCamMat.SetTexture("_Tex", rtScaled);
            SPPCamMat.SetFloat("marginX", GetMargins().x);
            SPPCamMat.SetFloat("marginY", GetMargins().y);
            SPPCamMat.SetVector("delta", positionDelta);
            SPPCamMat.SetFloat("ppu", pixelsPerUnit.value);
            SPPCamMat.SetFloat("width", rtScaledAndCropped.width);
            SPPCamMat.SetFloat("heigth", rtScaledAndCropped.height);
            SPPCamMat.SetFloat("width2", rtScaled.width);
            SPPCamMat.SetFloat("heigth2", rtScaled.height);
            SPPCamMat.SetVector("ps", new Vector4(1f / (float)rtScaledAndCropped.width, 1f / (float)rtScaledAndCropped.height));
            SPPCamMat.SetVector("ps2", new Vector4(1f / (float)rtScaled.width, 1f / (float)rtScaled.height));
            SPPCamMat.SetFloat("cropMarginX", crop != cropFrame.none ? cropMarginX : 0);
            SPPCamMat.SetFloat("cropMarginY", crop != cropFrame.none ? cropMarginY : 0);
            SPPCamMat.SetFloat("stretch", crop == cropFrame.stretchFill || stretchAnyway ? 1 : 0);
            SPPCamMat.SetFloat("snapMovement", !smoothCamera.value ? 1 : 0);

            RenderUtils.RenderToRT2D(new Rect(0, 0, rtScaledAndCropped.width, rtScaledAndCropped.height), SPPCamMat, rtScaledAndCropped);
   
            if (crop != cropFrame.none && enableCustomMargins.value && MarginsMat != null) RenderMargins(); //custom effects for camera margin
        }

        [System.Obsolete]
        private void RenderMargins() 
        {
            //creates temporary render texture since need to read and write from the same texture
            RenderTexture tempRT = rtScaledAndCropped.DeepCopy();

            _MarginsMat = new Material(marginsShader);
            MarginsMat.SetTexture("tex", tempRT);
            MarginsMat.SetFloat("ppu", pixelsPerUnit.value);
            MarginsMat.SetVector("ps", new Vector4(1f / (float)rtScaledAndCropped.width, 1f / (float)rtScaledAndCropped.height));
            MarginsMat.SetFloat("cropMarginX", crop != cropFrame.none ? cropMarginX : 0);
            MarginsMat.SetFloat("cropMarginY", crop != cropFrame.none ? cropMarginY : 0);
            MarginsMat.SetFloat("width", rtScaledAndCropped.width);
            MarginsMat.SetFloat("heigth", rtScaledAndCropped.height);
            MarginsMat.SetFloat("scale", (int)this.factorIdeal.x );
            MarginsMat.SetFloat("time",Time.timeSinceLevelLoad );
            MarginsMat.SetFloat("stretch", crop == cropFrame.stretchFill || stretchAnyway ? 1 : 0);

            RenderUtils.RenderToRT2D(new Rect(0, 0, rtScaledAndCropped.width, rtScaledAndCropped.height), MarginsMat, rtScaledAndCropped);
           
            tempRT.Release(); 
        }



        [System.Obsolete]
        public Vector2Int GetMargins()
        {
            Vector2 targetResolution = CalculateResolution();
            Vector2 screenRes = ScreenUtils.ScreenResolution();
            scaleX = screenRes.x / targetResolution.x;
            scaleY = screenRes.y / targetResolution.y;

            int pixelMarginX = Mathf.FloorToInt(scaleX);
            int pixelMarginY = Mathf.FloorToInt(scaleY);

            if (crop != cropFrame.none) { pixelMarginX = factorCrop; pixelMarginY = factorCrop; }

            int mx = pixelMarginX * 2;
            int my = pixelMarginY * 2;
            return new Vector2Int(mx, my);
        }

        [SerializeField] [HideInInspector] private Vector2 screenSize;
        [System.Obsolete]
        private void Update()
        {
     
            //  debug camera pixel perfect movement
            if (debugMov && Application.isPlaying) transform.position += new Vector3(debugSpeed.x * Time.deltaTime, debugSpeed.y * Time.deltaTime, 0f);

            Vector2 screenRes = ScreenUtils.ScreenResolution();
            //  updates the system if camera size has changed
            if (screenRes.x != screenSize.x || screenRes.y != screenSize.y || !QuadTextureSet() )
            {
                screenSize = screenRes;
                CameraSetup();
            }
 
            if (cinemachineSupport != null) cinemachineSupport.CinemachineUpdate(PixelPerfectCameraSize());
        }

        //snap position of camera before rendering
        [System.Obsolete]
        private void LateUpdate()
        {
            if(_quad!=null && _quad.GetComponent<MeshRenderer>()!=null ) _quad.GetComponent<MeshRenderer>().sharedMaterial.SetVector("_delta", positionDelta);
            ApplyDelta();
        }

        //sets quad shader variables
        [System.Obsolete]
        void SetQuadShader()
        {
            Material mat = _quad.GetComponent<MeshRenderer>().sharedMaterial;
            mat.SetTexture("_MainTex", rtScaledAndCropped);
        }

        bool QuadTextureSet() => (_quad == null || _quad.GetComponent<MeshRenderer>().sharedMaterial.GetTexture("_MainTex") != null);

        //return position of camera after rendering for urp
        [System.Obsolete]
        void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (camera == cam2)
            {
                ReturnCameraPosition();
            }
        }

        //calculates delta to grid
        [System.Obsolete]
        Vector2 Delta(Vector2 position, int ppu)
        {
            if (ppu <= 0) return Vector2.zero;

            Vector2 res = new Vector2(0, 0);
            float xx = position.x * (float)ppu;
            float xy = position.y * (float)ppu;

            float roundedxx;
            float roundedxy;

            if (Mathf.Floor(xx) != 0)
            {
                roundedxx = (float)Mathf.Floor(xx) / (float)ppu;
            }
            else
            {
                roundedxx = 0;
            }

            if (Mathf.Floor(xy) != 0)
            {
                roundedxy = (float)Mathf.Floor(xy) / (float)ppu;
            }
            else
            {
                roundedxy = 0;
            }

            return new Vector2(roundedxx - position.x, roundedxy - position.y);
        }

        //calculates the delta between camera current position and the target (on pixel perfect grid) position
        [System.Obsolete]
        void ApplyDelta()
        {
            positionDelta = Delta(transform.position, pixelsPerUnit.value);
            transform.position += new Vector3(positionDelta.x, positionDelta.y, 0);
            frame = true;
        }

        //returns the camera to grid-free position
        [System.Obsolete]
        void ReturnCameraPosition()
        {

            if (!frame) return; frame = false;
            transform.position -= new Vector3(positionDelta.x, positionDelta.y, 0);
        }


        [System.Obsolete]
        private void CreateRenderTextures()
        {
            Vector2Int screenRes = ScreenUtils.ScreenResolution();
            screenSize = screenRes;


            if (rt != null) rt.Release();
            if (rtScaled != null) rtScaled.Release();
         
            //rt1
            Vector2Int targetResolution = CalculateResolution();
            ppRes = targetResolution;

            rt = new RenderTexture(AddMargins(targetResolution).x, AddMargins(targetResolution).y, 0, RenderTextureFormat.ARGB32);
            rt.filterMode = FilterMode.Point;   //for pixel perfect filtering
            rt.name = "PixelPerfect" + Time.timeSinceLevelLoad.ToString();
            rt.Create();
            orgCam.targetTexture = rt;

            if (targetResolution.x == 0 || targetResolution.y == 0) targetResolution = new Vector2Int(320, 180);

            //rt2
        
            Vector2Int resUpsc = CalculateResolutionUpscaled(targetResolution);
  
            rtScaled = new RenderTexture(resUpsc.x, resUpsc.y, 0, RenderTextureFormat.ARGB32);
            rtScaled.filterMode = filteringMode.ToFilterMode();
            rtScaled.name = "ScaledPixelPerfect" + Time.timeSinceLevelLoad.ToString();
            rtScaled.Create();
        }

        [System.Obsolete]
        private void MoveUIElementsToScaledCamera() 
        {
            //we're moving the elements only in playmode, so the developer can still change/modify them in editor without issue
            if (!Application.isPlaying ) return;
            if (FindObjectOfType<Canvas>() == null) return;

            foreach (var canva in FindObjectsOfType<Canvas>() )
            {
                if ((canva.renderMode != RenderMode.ScreenSpaceCamera || canva.worldCamera != _orgCam) && canva.worldCamera != null) return;
                canva.worldCamera = cam2;
 
            }

        }


        [System.Obsolete]
        private void MoveUIElementsToPixelPerfectCamera()
        {
            //we're moving the elements only in playmode, so the developer can still change/modify them in editor without issue
            if (!Application.isPlaying ) return;
            if (FindObjectOfType<Canvas>() == null) return;

            foreach (var canva in FindObjectsOfType<Canvas>())
            {
                if ( (canva.renderMode != RenderMode.ScreenSpaceCamera || canva.worldCamera != cam2) && canva.worldCamera != null) return;
                canva.worldCamera = _orgCam;

            }
            Canvas.ForceUpdateCanvases();
        }


        [System.Obsolete]
        private void CreateScreenRenderTexture()
        {
            Vector2Int screenRes = ScreenUtils.ScreenResolution();
            if (rtScaledAndCropped != null) rtScaledAndCropped.Release();
            rtScaledAndCropped = new RenderTexture(screenRes.x, screenRes.y, 0, RenderTextureFormat.ARGB32);
            rtScaledAndCropped.filterMode = FilterMode.Point;
            rtScaledAndCropped.name = "ScaledAndCroppedPixelPerfect" + Time.timeSinceLevelLoad.ToString();
            rtScaledAndCropped.Create();
        }

        //sets correct resolution
        [System.Obsolete]
        void Resize()
        {
            Camera cam = orgCam;
            cam.orthographic = true;
            cam.aspect = AspectRatio();
            cam.orthographicSize = PixelPerfectCameraSize();
        }

        [System.Obsolete]
        float PixelPerfectCameraSize()
        {
            Vector2Int targetResolution = CalculateResolution();
            Vector2 paddedRes = AddMargins(targetResolution);
            return paddedRes.y / pixelsPerUnit.value / 2f;
        }

        //adds margins to rendered image for smooth camera movement
        [System.Obsolete]
        Vector2Int AddMargins(Vector2Int res)
        {
            return new Vector2Int(res.x + padding, res.y + padding);
        }

        [System.Obsolete]
        private void UpscaleRT(ScriptableRenderContext context, Camera currentCamera)
        {
            orgCam.targetTexture = rt;

            if (currentCamera != cam2) return;

            Rect viewport = new Rect(0, 0, rtScaled.width, rtScaled.height) ;

            RenderUtils.RenderToRT2D(viewport, blitMat, rtScaled);

            RenderUpscaledAndCroppedTexture();
        }

        [System.Obsolete]
        private void PreRenderCanvas() 
        {
            MoveUIElementsToPixelPerfectCamera();
        }

        [System.Obsolete]
        private void AfterRenderCanvas(ScriptableRenderContext c, Camera cam) 
        {
            if (cam==orgCam) MoveUIElementsToScaledCamera();
        }

        //draws the rendered area
        [System.Obsolete]
        private void OnDrawGizmos()
        {
            Vector2 targetResolution = CalculateResolution();
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position - new Vector3(positionDelta.x, positionDelta.y, 0), new Vector3(targetResolution.x / pixelsPerUnit.value, targetResolution.y / pixelsPerUnit.value, 0f));
        }

        //removes callbacks that would otherwise result in errors
        //destroys child components

        [System.Obsolete]
        private void OnDisable()
        {
#if UNITY_EDITOR
            EditorApplication.update -= Update;
#endif
            RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
            RenderPipelineManager.beginCameraRendering -= UpscaleRT;
            RenderPipelineManager.endCameraRendering -= AfterRenderCanvas;
            Canvas.willRenderCanvases -= PreRenderCanvas;

            if (_orgCam!=null)orgCam.targetTexture = null;
            if(_cam2!=null)DestroyUtils.DestroyAlways(cam2.gameObject);
            if(_quad!=null)DestroyUtils.DestroyAlways(quad);
        }

    }
}
