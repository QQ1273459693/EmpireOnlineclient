using UnityEditor;
using UnityEngine;
using LeafUtils;

namespace SmoothCam
{
    [CustomEditor(typeof(SmoothPixelPerfectCamera))]
    public class SmoothPixelPerfectCameraEditor : Editor
    {

        SmoothPixelPerfectCamera cam;

        const int space1 = 10;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            SmoothPixelPerfectCamera cam = (SmoothPixelPerfectCamera)target;
            this.cam = cam;

            bool show = false;
            using (new LayoutUtils.FoldoutScope(false, cam.gen.GetAB(),out show, "Camera Settings", true ))
            {
                if (show) Settings();
            }

            bool show2 = false;
            using (new LayoutUtils.FoldoutScope(false, cam.gen.GetAB(), out show2, "Cropped margins Settings", true))
            {
                if (show2) CroppedFrameSettings();
            }

            bool show1 = false;
            using (new LayoutUtils.FoldoutScope(false, cam.gen.GetAB(), out show1, "Debug Settings", true))
            {
                if (show1) DebugSettings();
            }

            cam.gen.EndFrame();
        }

        private void Settings() 
        {
            GUIStyle bold = new GUIStyle();
            bold.fontStyle = FontStyle.Bold;
            bold.normal.textColor = Color.white;

            EditorGUILayout.LabelField("Variables : ", bold);

            GUIContent g1 = new GUIContent("sprite pixels per unit", "you can check it in your sprites texture, it's used for calculations");
            cam.pixelsPerUnit.value = EditorGUILayout.IntField(g1, cam.pixelsPerUnit.value);
           
            EditorGUILayout.Space(space1);
                
            GUIContent g0 = new GUIContent("scaling mode of your camera", "decides the scaling mode of your camera, for example, if you choose the target Resolution mode, then the camera will scale to match the target resolution as close as it can");
            ScalingMode mode = cam.scalingMode;
            cam.scalingMode = (ScalingMode)EditorGUILayout.EnumPopup(g0, cam.scalingMode);
       
            if (mode != cam.scalingMode)
            {
                cam.targetResolution.onValueChanged.Invoke();
            }

            if (cam.scalingMode == ScalingMode.targetResolution)
            {
                GUIContent g2 = new GUIContent("target resolution", "pixel perfect resolution you aim for");
                cam.targetResolution.value = EditorGUILayout.Vector2IntField(g2, cam.targetResolution.value);
            }

            if (cam.scalingMode == ScalingMode.targetCameraSize)
            {
                GUIContent g3 = new GUIContent("target camera size", "camera size you aim for");
                cam.targetCameraSize.value = EditorGUILayout.FloatField(g3, cam.targetCameraSize.value);
            }

            EditorGUILayout.Space(space1);

            FilteringMode c1 = cam.filteringMode;
            cam.filteringMode = (FilteringMode)EditorGUILayout.EnumPopup("Filter Mode", cam.filteringMode);
            if (cam.filteringMode != c1) cam.targetCameraSize.onValueChanged.Invoke();

            EditorGUILayout.Space(space1);

            EditorGUILayout.BeginHorizontal();
            GUIContent g33 = new GUIContent("max camera size delta", "maximum difference that pixlel-perfect-camera size can have to the target size");
            cam.enableCameraMaxSizeDifference.value = EditorGUILayout.Toggle(g33, cam.enableCameraMaxSizeDifference.value);
            if (cam.enableCameraMaxSizeDifference.value) cam.maxCameraSizeDifference.value = EditorGUILayout.FloatField(cam.maxCameraSizeDifference.value);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(space1);

            cropFrame c = cam.crop;
            cam.crop = (cropFrame)EditorGUILayout.EnumPopup("Crop Frame",cam.crop);
            if (cam.crop != c) cam.onCropSettingsChanged();

            GUIContent g6 = new GUIContent("smooth camera", "enables sub-pixel camera movement. Sprites are still pixel perfect, while camera can move between pixels");
            cam.smoothCamera.value = EditorGUILayout.Toggle(g6, cam.smoothCamera.value);
          

            EditorGUILayout.Space(space1);
 
            cam.showCameraData = EditorGUILayout.Toggle("show data", cam.showCameraData);

            if (cam.showCameraData)
            {
                EditorGUILayout.LabelField("Target Resolution : " + cam.targetResolution.value.ToString(), LayoutUtils.ReadOnly());
                EditorGUILayout.LabelField("Pixel Perfect Target Resolution : " + cam.ppRes.ToString(), LayoutUtils.ReadOnly());

                EditorGUILayout.LabelField("margins : " + cam.GetMargins().ToString(), LayoutUtils.ReadOnly());

                EditorGUILayout.LabelField("Upscale Factor : " + cam.factorReal.ToString(), LayoutUtils.ReadOnly());

                EditorGUILayout.LabelField("Upscale Rounded Factor : " + cam.factorIdeal.ToString(), LayoutUtils.ReadOnly());
                EditorGUILayout.LabelField("error delta : " + cam.delta.ToString(), LayoutUtils.ReadOnly());

                GUILayout.Space(10);

                if (cam.crop != cropFrame.none)
                {
                    EditorGUILayout.LabelField("cropped margins : " + cam.cropMargins().ToString(), LayoutUtils.ReadOnly());
                }

            }

        }

        private void CroppedFrameSettings()
        {
            GUIContent g1 = new GUIContent("enable margin shaders", "lets you assign a custom compute shader that paints the margins of cropped screen (takes a bit of preformance since it requires an additional dispatch every frame + preformance depends on margins size )");
            cam.enableCustomMargins.value = EditorGUILayout.Toggle(g1, cam.enableCustomMargins.value);
            if (cam.enableCustomMargins.value)
            {
                cam.marginsShader = (Shader)EditorGUILayout.ObjectField(cam.marginsShader, typeof(Shader), true);
            }
        }

        private void DebugSettings()
        {
            cam.debug = EditorGUILayout.Toggle("enable debug functions", cam.debug);
            if (!cam.debug) return;

            cam.rt = (RenderTexture)EditorGUILayout.ObjectField(cam.rt, typeof(RenderTexture),true);
            cam.rtScaled = (RenderTexture)EditorGUILayout.ObjectField(cam.rtScaled, typeof(RenderTexture),true);
            cam.rtScaledAndCropped = (RenderTexture)EditorGUILayout.ObjectField(cam.rtScaledAndCropped, typeof(RenderTexture),true);

            cam.debugMov = EditorGUILayout.Toggle("enable camera debug movement", cam.debugMov);
            if (cam.debugMov) cam.debugSpeed = EditorGUILayout.Vector2Field("camera speed", cam.debugSpeed);
        }
    }

}
