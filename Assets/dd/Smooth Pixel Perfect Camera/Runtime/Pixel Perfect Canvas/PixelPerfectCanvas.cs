using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace SmoothCam
{

#if UNITY_EDITOR
    [CustomEditor(typeof(PixelPerfectCanvas))]
    public class PixelPerfectCanvasEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            PixelPerfectCanvas SPPCanvas = (PixelPerfectCanvas)target;

            SmoothPixelPerfectCamera c = (SmoothPixelPerfectCamera)EditorGUILayout.ObjectField("smooth camera", SPPCanvas.SPPCam, typeof(SmoothPixelPerfectCamera), true);
            if (c != SPPCanvas.SPPCam)
            {
                SPPCanvas.SPPCam = c;
                SPPCanvas.ConfigureCanvas();
            }
            else SPPCanvas.SPPCam = c;

        }
    }
#endif

    [ExecuteAlways]
    [RequireComponent(typeof(Canvas))]
    public class PixelPerfectCanvas : MonoBehaviour
    {
        Canvas _canvas;
        Canvas canvas
        {
            get { if (_canvas == null) _canvas = GetComponent<Canvas>(); return _canvas; }
            set { _canvas = value; }
        }

        CanvasScaler _canvasScaler;
        CanvasScaler canvasScaler
        {
            get { if (_canvasScaler == null) _canvasScaler = GetComponent<CanvasScaler>(); return _canvasScaler; }
            set { _canvasScaler = value; }
        }

        [HideInInspector] public SmoothPixelPerfectCamera SPPCam;

        private void OnEnable()
        {
            ConfigureCanvas();
        }

        private void Start()
        {
            ConfigureCanvas();
        }


        public void ConfigureCanvas()
        {
            Canvas c = canvas;
            CanvasScaler cs = canvasScaler;
            c.worldCamera = SPPCam.GetComponent<Camera>();
            c.renderMode = RenderMode.ScreenSpaceCamera;
            c.planeDistance = 100;

            cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referenceResolution = SPPCam.targetResolution.value + new Vector2Int(2, 2);
            cs.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        }

        public void OnDisable()
        {
            if (SPPCam == null) return;
        }

    }

}