using LeafUtilsRays;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering.Universal;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LightRays2D
{

    [ExecuteAlways]
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class LightRays2D : MonoBehaviour
    {


#if UNITY_EDITOR
        [MenuItem("Light Rays 2D/Create")]
        public static void CreateLightRay()
        {
            GameObject g = new GameObject();
            g.name = "LightRays2D";
            g.AddComponent<LightRays2D>();
        }

        [MenuItem("Light Rays 2D/Documentation")]
        public static void WebDocumentation()
        {
            Application.OpenURL("https://leafousio.github.io/2DLightRays");
        }
#endif



        public const string layer = "lightrays2d";

        [SerializeField][HideInInspector] public BlendingMode blendingMode = BlendingMode.screen;
        [SerializeField][HideInInspector] public Cryo<bool> raysLight = new Cryo<bool>(false);
        [SerializeField][HideInInspector] public Cryo<string> sortinglayerName = new Cryo<string>("default");
        [SerializeField][HideInInspector] public Cryo<int> sortinglayerOrder = new Cryo<int>(1000);


        [SerializeField][HideInInspector] private EditableSpriteQuad _spriteQuadGenerator;
        public EditableSpriteQuad spriteQuadGenerator
        {
            get
            {
                if (_spriteQuadGenerator == null || !_spriteQuadGenerator.Validate(gameObject))
                {
                    _spriteQuadGenerator = new EditableSpriteQuad(gameObject);
                    _spriteQuadGenerator.SetAllCryoCallbacks(OnQuadSettingsChanged);
                    GenerateQuad();

                }
                return _spriteQuadGenerator;
            }
            set
            {
                _spriteQuadGenerator = value;
            }
        }

        [SerializeField]
        [HideInInspector]
        private LightRays2DSettings _settings;
        public LightRays2DSettings settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new LightRays2DSettings();
                    _settings.SetAllCryoCallbacks(OnLightRaysSettingsChanged);
                }
                return _settings;
            }
            set
            {
                _settings = value;
            }
        }

        [SerializeField]
        [HideInInspector]
        private ParticleSystem _psystem;
        public ParticleSystem psystem
        {
            get
            {
                return _psystem;
            }
            set
            {
                _psystem = value;
            }
        }

        [SerializeField]
        [HideInInspector]
        private RaysParticlesSettings _particlesSettings;
        public RaysParticlesSettings particlesSettings
        {
            get
            {
                if (_particlesSettings == null)
                {
                    _particlesSettings = new RaysParticlesSettings();
                    _particlesSettings.SetAllCryoCallbacks(OnParticleSettingsChanged);
                }
                return _particlesSettings;
            }
            set
            {
                _particlesSettings = value;
            }
        }

        [SerializeField]
        [HideInInspector]
        private MeshFilter _mesh;
        public MeshFilter mesh
        {
            get
            {
                if (_mesh == null)
                {
                    if (GetComponent<MeshFilter>() == null) transform.gameObject.AddComponent<MeshFilter>();
                    _mesh = GetComponent<MeshFilter>();
                    _mesh.hideFlags = HideFlags.HideInInspector;
                    if (_mesh == null) throw new System.Exception($"Object {gameObject.name} with LightRays2D Component does not have a mesh filter as a component");
                }
                else if (_mesh != GetComponent<MeshFilter>()) mesh = GetComponent<MeshFilter>();

                return _mesh;
            }
            set { _mesh = value; }
        }

        [SerializeField]
        [HideInInspector]
        private MeshRenderer _meshRenderer;
        public MeshRenderer meshRenderer
        {
            get
            {
                if (_meshRenderer == null)
                {
                    if (GetComponent<MeshRenderer>() == null) transform.gameObject.AddComponent<MeshRenderer>();
                    _meshRenderer = GetComponent<MeshRenderer>();
                    _meshRenderer.hideFlags = HideFlags.HideInInspector;
                    _meshRenderer.material = mat;
                    if (_meshRenderer == null) throw new System.Exception($"Object {gameObject.name} with LightRays2D Component does not have a mesh Renderer as a component");
                }
                else if (_meshRenderer != GetComponent<MeshRenderer>()) _meshRenderer = GetComponent<MeshRenderer>();

                return _meshRenderer;
            }
            set { _meshRenderer = value; }
        }

        [SerializeField]
        [HideInInspector]
        private Material _mat;
        public Material mat
        {
            get
            {
                if (_mat == null)
                {
                    if (_mat == null) _mat = (Material)Resources.Load("Materials/light rays 2d");
                    if (_mat == null) throw new System.Exception($"{gameObject.name} : light rays 2d material could not be found in the Resources/Materials folder");
                }
                else if (_mat != GetComponent<MeshRenderer>().sharedMaterial) _mat = GetComponent<MeshRenderer>().sharedMaterial;

                return _mat;
            }
            set { _mat = value; }
        }

        void OnLightSettingsChanged()
        {
            //set sorting layers
            meshRenderer.sortingLayerName = sortinglayerName.value;
            meshRenderer.sortingOrder = sortinglayerOrder.value;

            //destroy
            if (!raysLight.value)
            {
                if (GetComponent<Light2D>() != null) DestroyUtils.DestroyAlways(GetComponent<Light2D>());
                return;
            }

            //create
            if (GetComponent<Light2D>() == null) transform.gameObject.AddComponent<Light2D>();

            //get our light
            var light = transform.GetComponent<Light2D>();

            //set our light shape to light rays shape
            SetURPLightShape(ref light);
        }

        void SetURPLightShape(ref Light2D light)
        {
            light.lightType = Light2D.LightType.Freeform;

            light.shapePath[0] = spriteQuadGenerator.vertBottomRight.value;
            light.shapePath[1] = spriteQuadGenerator.vertTopRight.value;
            light.shapePath[2] = spriteQuadGenerator.vertTopLeft.value;
            light.shapePath[3] = spriteQuadGenerator.vertBottomLeft.value;
        }


        void OnLightRaysSettingsChanged()
        {
            SetShaderPropertyBlock();
        }


        void OnQuadSettingsChanged()
        {
            if (mesh.sharedMesh == null) mesh.sharedMesh = new Mesh();
            mesh.sharedMesh.vertices = spriteQuadGenerator.GetVerts();
            spriteQuadGenerator.UpdateQuadUvs(mesh.sharedMesh);
            OnLightSettingsChanged();
        }

        public void GenerateQuad()
        {
            if (mesh.sharedMesh != null && mesh.sharedMesh.vertices != null && mesh.sharedMesh.vertices.Length == 4) mesh.sharedMesh = _spriteQuadGenerator.GenerateSpriteQuad(mesh.sharedMesh.vertices);
            else mesh.sharedMesh = _spriteQuadGenerator.GenerateSpriteQuad(null, 1);
        }

        void SetShaderPropertyBlock()
        {
            var propBlock = new MaterialPropertyBlock();
            meshRenderer.GetPropertyBlock(propBlock);
            settings.SetPropertiesToMaterial(propBlock, mat);
            propBlock.SetInt("_mode", (int)blendingMode);
            meshRenderer.SetPropertyBlock(propBlock);

        }

        private void OnParticleSettingsChanged()
        {
            ParticleSystem system;
            if (particlesSettings.enable.value)
            {
                RayParticlesUtils.DeleteExistingRayParticles(transform);
                system = RayParticlesUtils.InstantiateRayParticles(particlesSettings.type, transform);
                psystem = system;
            }
            else
            {
                RayParticlesUtils.DeleteExistingRayParticles(transform);
                return;
            }

            if (particlesSettings.enable.value) RayParticlesUtils.SetParticlesSettings(psystem, particlesSettings.type, transform, spriteQuadGenerator, particlesSettings);
        }




        private void OnEnable()
        {
            gameObject.layer = LayerMask.NameToLayer(layer);
            CameraOpaque2DManager.UpdateCamera();
            OnQuadSettingsChanged();
            OnLightRaysSettingsChanged();
            OnParticleSettingsChanged();

            raysLight.onValueChanged = OnLightSettingsChanged;
            sortinglayerName.onValueChanged = OnLightSettingsChanged;
            sortinglayerOrder.onValueChanged = OnLightSettingsChanged;

            _settings.SetAllCryoCallbacks(OnLightRaysSettingsChanged);

            _spriteQuadGenerator.SetAllCryoCallbacks(OnQuadSettingsChanged);

            _particlesSettings.SetAllCryoCallbacks(OnParticleSettingsChanged);


        }

    }

}