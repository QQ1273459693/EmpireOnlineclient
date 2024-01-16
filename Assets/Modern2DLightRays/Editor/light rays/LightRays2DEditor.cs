using LeafUtilsRays;
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;

namespace LightRays2D
{

    [CustomEditor(typeof(LightRays2D)), CanEditMultipleObjects]
    public class LightRays2DEditor : Editor
    {

        private AnimBool[] animBs = new AnimBool[64];
        public AnimBool GetAnimBs(int idx) { if (animBs[idx] == null) animBs[idx] = new AnimBool(); return animBs[idx]; }

        private Color background;
        private GUIStyle text;
        private string textHex = "CFCFCF";
        private GUIStyle textExtension;

        LightRays2D rays;
        EditableSpriteQuad quad;
        LightRays2DSettings settings;
        RaysParticlesSettings particles;

        LightRays2D[] raysArr;
        EditableSpriteQuad[] quadArr;
        LightRays2DSettings[] settingsArr;
        RaysParticlesSettings[] particlesArr;

        //override geometry only works in start and onGUI methods

        bool quadOpen = false;


        public override void OnInspectorGUI()
        {
            text = LayoutUtils.Text(ColorUtils.HexToRGB("CFCFCF"), 10);
            textExtension = LayoutUtils.Text(ColorUtils.HexToRGB("E26D5C"), 10);

            base.OnInspectorGUI();

            rays = target as LightRays2D;
            quad = rays.spriteQuadGenerator;
            settings = rays.settings;
            particles = rays.particlesSettings;

            if (targets.Length - 1 > 0)
            {
                raysArr = new LightRays2D[targets.Length - 1];
                quadArr = new EditableSpriteQuad[targets.Length - 1];
                settingsArr = new LightRays2DSettings[targets.Length - 1];
                particlesArr = new RaysParticlesSettings[targets.Length - 1];

                for (int i = 0, i1 = 0; i < targets.Length; i++, i1++)
                {
                    if (targets[i] == target) { i1--; continue; }
                    raysArr[i1] = ((LightRays2D)targets[i]);
                    quadArr[i1] = ((LightRays2D)targets[i]).spriteQuadGenerator;
                    settingsArr[i1] = ((LightRays2D)targets[i]).settings;
                    particlesArr[i1] = ((LightRays2D)targets[i]).particlesSettings;
                }
            }

            InspectorHeader();

            using (new LayoutUtils.FoldoutScope(false, GetAnimBs(0), out var shouldDraw, "Camera (global)"))
            {
                if (shouldDraw) GlobalSettings();
            }

            using (new LayoutUtils.FoldoutScope(false, GetAnimBs(1), out var shouldDraw, "Light Rays"))
            {
                if (shouldDraw && settings.useScrollingTexture.value) LightRaysSettingsTexture();
                else if (shouldDraw) LightRaysSettingsDefault();
            }

            using (new LayoutUtils.FoldoutScope(false, GetAnimBs(2), out var shouldDraw, "Particles"))
            {
                if (shouldDraw) Particles();
            }

            using (new LayoutUtils.FoldoutScope(false, GetAnimBs(4), out var shouldDraw, "URP Lighting"))
            {
                if (shouldDraw) LightingSettings();
            }

            using (new LayoutUtils.FoldoutScope(false, GetAnimBs(3), out var shouldDraw, "Quad"))
            {
                if (shouldDraw) { QuadSettings(); quadOpen = true; }
                else quadOpen = false;
            }

            InspectorFooter();
        }

        void InspectorHeader()
        {
            LayoutUtils.Banner((Texture)Resources.Load("Sprites/editor sprites/banner"));
        }

        void GlobalSettings()
        {
            GUILayout.Space(5);
            CameraOpaque2DManager.overrideMainCamera.value = EditorGUILayout.Toggle("override main camera", CameraOpaque2DManager.overrideMainCamera.value);
            if (CameraOpaque2DManager.overrideMainCamera.value) CameraOpaque2DManager.overrideCamera = (Camera)EditorGUILayout.ObjectField(CameraOpaque2DManager.overrideCamera, typeof(Camera), true);
            GUILayout.Space(5);
            CameraOpaque2DManager.filterMode = (FilterMode)EditorGUILayout.EnumPopup("Camera texture filtering mode", CameraOpaque2DManager.filterMode);
            GUILayout.Space(5);
        }

        void LightRaysSettingsTexture()
        {
            GeneralSettings();
            TextureSettings();
            EdgesSettings();
            ColorSettings();
            SettingsOther();
        }

        void LightRaysSettingsDefault()
        {
            GeneralSettings();
            AlphaSettings();
            DensitySettings();
            SpeedSettings();
            EdgesSettings();
            ColorSettings();
            SettingsOther();
        }

        void GeneralSettings()
        {
            EditorGUILayout.LabelField("General :", LayoutUtils.Header(12, TextAnchor.MiddleLeft, textHex));
            GUILayout.Space(4);
            LayoutUtils.Indent();

            settings.extendedSinWaves.value = EditorGUILayout.Toggle("additional two layers", settings.extendedSinWaves.value);
            GUILayout.Space(5);

            settings.useScrollingTexture.value = EditorGUILayout.Toggle("use scrolling texture as light rays", settings.useScrollingTexture.value);
            GUILayout.Space(5);

            int sortingLayer = Mathf.Max(System.Array.IndexOf(GetSortingLayerNames(), rays.sortinglayerName.value), 0);

            rays.sortinglayerName.value = GetSortingLayerNames()[EditorGUILayout.Popup("sorting layer", sortingLayer, GetSortingLayerNames())];
            rays.sortinglayerOrder.value = EditorGUILayout.IntField("order in layer", rays.sortinglayerOrder.value);

            GUILayout.Space(5);

            var a = rays.blendingMode;
            rays.blendingMode = (BlendingMode)EditorGUILayout.EnumPopup("blending mode", rays.blendingMode);
            if (a != rays.blendingMode) settings.alpha.onValueChanged.Invoke();

            GUILayout.Space(5);
            LayoutUtils.EndIndent();
        }

        void TextureSettings()
        {
            EditorGUILayout.LabelField("Texture Settings :", LayoutUtils.Header(12, TextAnchor.MiddleLeft, textHex));
            GUILayout.Space(4);
            LayoutUtils.Indent();

            settings.raysTexture = (Texture)EditorGUILayout.ObjectField("texture: ", settings.raysTexture, typeof(Texture), true); GUILayout.Space(5);
            settings.alpha.value = EditorGUILayout.Slider("Alpha", settings.alpha.value, 0, 2f, null); GUILayout.Space(5);
            settings.raysSpeed1.value = EditorGUILayout.FloatField("Scrolling speed", settings.raysSpeed1.value); GUILayout.Space(5);
            settings.raysDensity1.value = EditorGUILayout.FloatField("Rays density", settings.raysDensity1.value); GUILayout.Space(5);

            LayoutUtils.EndIndent();
        }

        void AlphaSettings()
        {
            EditorGUILayout.LabelField("Alphas :", LayoutUtils.Header(12, TextAnchor.MiddleLeft, textHex));
            GUILayout.Space(4);
            LayoutUtils.Indent();

            settings.alpha.value = EditorGUILayout.Slider("final alpha", settings.alpha.value, 0, 2f, null); GUILayout.Space(5);

            GUILayout.Space(20);
            settings.raysAlpha1.value = EditorGUILayout.Slider(settings.raysAlpha1.value, 0, 1f);
            settings.raysAlpha2.value = EditorGUILayout.Slider(settings.raysAlpha2.value, 0, 1f);

            if (settings.extendedSinWaves.value)
            {
                settings.raysAlpha3.value = EditorGUILayout.Slider(settings.raysAlpha3.value, 0, 1f);
                settings.raysAlpha4.value = EditorGUILayout.Slider(settings.raysAlpha4.value, 0, 1f);
            }

            LayoutUtils.EndIndent();
            GUILayout.Space(20);
        }

        void DensitySettings()
        {
            EditorGUILayout.LabelField("Densities :", LayoutUtils.Header(12, TextAnchor.MiddleLeft, textHex));
            GUILayout.Space(4);
            LayoutUtils.Indent();

            settings.raysDensity1.value = EditorGUILayout.FloatField("layer 1 density", settings.raysDensity1.value);
            settings.raysDensity2.value = EditorGUILayout.FloatField("layer 2 density", settings.raysDensity2.value);
            if (settings.extendedSinWaves.value) settings.raysDensity3.value = EditorGUILayout.FloatField("layer 3 density", settings.raysDensity3.value);
            if (settings.extendedSinWaves.value) settings.raysDensity4.value = EditorGUILayout.FloatField("layer 4 density", settings.raysDensity4.value);

            GUILayout.Space(10);
            LayoutUtils.EndIndent();
        }

        void SpeedSettings()
        {
            EditorGUILayout.LabelField("Speeds :", LayoutUtils.Header(12, TextAnchor.MiddleLeft, textHex));
            GUILayout.Space(4);
            LayoutUtils.Indent();

            settings.raysSpeed1.value = EditorGUILayout.FloatField("rays speed", settings.raysSpeed1.value);
            settings.raysSpeed2.value = EditorGUILayout.FloatField("layer 2 speed", settings.raysSpeed2.value);
            if (settings.extendedSinWaves.value) settings.raysSpeed3.value = EditorGUILayout.FloatField("layer 3 speed", settings.raysSpeed3.value);
            if (settings.extendedSinWaves.value) settings.raysSpeed4.value = EditorGUILayout.FloatField("layer 4 speed", settings.raysSpeed4.value);

            LayoutUtils.EndIndent();
            GUILayout.Space(10);
        }

        void EdgesSettings()
        {
            EditorGUILayout.LabelField("Falloffs :", LayoutUtils.Header(12, TextAnchor.MiddleLeft, textHex));
            GUILayout.Space(4);
            LayoutUtils.Indent();

            LayoutUtils.MinMaxSliderCryo("falloff right", ref settings.rightFalloffStart, ref settings.rightFalloffEnd, 0.5f, 1f);
            LayoutUtils.MinMaxSliderCryo("falloff left", ref settings.leftFalloffStart, ref settings.leftFalloffEnd, 0f, 0.5f);
            LayoutUtils.MinMaxSliderCryo("falloff top", ref settings.topFalloffStart, ref settings.topFalloffEnd, 0.5f, 1f);
            LayoutUtils.MinMaxSliderCryo("falloff bottom", ref settings.bottomFalloffStart, ref settings.bottomFalloffEnd, 0f, 1f);

            GUILayout.Space(10);

            settings.harderEdges.value = EditorGUILayout.Toggle("harderEdges", settings.harderEdges.value);
            if (settings.harderEdges.value)
            {
                LayoutUtils.MinMaxSliderCryo("edgesAlpha", ref settings.edgeAlphaMin, ref settings.edgeAlphaMax, 0.0f, 1f);
            }

            LayoutUtils.EndIndent();
            GUILayout.Space(10);
        }

        void ColorSettings()
        {
            EditorGUILayout.LabelField("Colors : (4 corners)", LayoutUtils.Header(12, TextAnchor.MiddleLeft, textHex));
            GUILayout.Space(4);
            LayoutUtils.Indent();

            settings.colorWithGradients.value = EditorGUILayout.Toggle("color with gradients", settings.colorWithGradients.value);
            if (settings.colorWithGradients.value)
            {
                settings.gradientY.value = EditorGUILayout.GradientField("gradient Top :", settings.gradientY.value);
                settings.gradientX.value = EditorGUILayout.GradientField("gradient Bottom :", settings.gradientX.value);
            }
            else
            {
                GUILayout.BeginHorizontal();
                settings.colorTopLeft.value = EditorGUILayout.ColorField(settings.colorTopLeft.value);
                settings.colorTopRight.value = EditorGUILayout.ColorField(settings.colorTopRight.value);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                settings.colorBottomLeft.value = EditorGUILayout.ColorField(settings.colorBottomLeft.value);
                settings.colorBottomRight.value = EditorGUILayout.ColorField(settings.colorBottomRight.value);
                GUILayout.EndHorizontal();
            }

            LayoutUtils.EndIndent();
            GUILayout.Space(10);

        }

        void SettingsOther()
        {
            EditorGUILayout.LabelField("Additional :", LayoutUtils.Header(12, TextAnchor.MiddleLeft, textHex));
            GUILayout.Space(4);
            LayoutUtils.Indent();

            settings.pixelPerfect.value = EditorGUILayout.Toggle("pixel art", settings.pixelPerfect.value);
            if (settings.pixelPerfect.value) settings.numberOfPixels.value = EditorGUILayout.IntSlider("number of pixels", settings.numberOfPixels.value, 8, 512);
            GUILayout.Space(4);
            settings.waviness.value = EditorGUILayout.Slider("waviness", settings.waviness.value, 0f, 6f);
            GUILayout.Space(10);
            LayoutUtils.EndIndent();
        }

        void Particles()
        {
            EditorGUILayout.LabelField("Particle System :", LayoutUtils.Header(12, TextAnchor.MiddleLeft, textHex));
            GUILayout.Space(4);
            LayoutUtils.Indent();

            var t = particles.type;
            particles.type = (RayParticlesType)EditorGUILayout.EnumPopup("type of particles", particles.type);
            if (particles.type != t) particles.enable.onValueChanged.Invoke();

            particles.enable.value = EditorGUILayout.Toggle("enable", particles.enable.value);
            LayoutUtils.EndIndent();
            EditorGUILayout.LabelField("Looks :", LayoutUtils.Header(12, TextAnchor.MiddleLeft, textHex));
            GUILayout.Space(4);
            LayoutUtils.Indent();
            particles.speed.value = EditorGUILayout.FloatField("speed", particles.speed.value);
            particles.size.value = EditorGUILayout.FloatField("size", particles.size.value);
            particles.density.value = EditorGUILayout.FloatField("density", particles.density.value);
            particles.color.value = EditorGUILayout.ColorField("color", particles.color.value);
            particles.sprite = (Sprite)EditorGUILayout.ObjectField("sprite", particles.sprite, typeof(Sprite), true);
            particles.lifetime.value = EditorGUILayout.FloatField("lifetime", particles.lifetime.value);
            particles.noiseStrength.value = EditorGUILayout.FloatField("noise strength", particles.noiseStrength.value);
            LayoutUtils.EndIndent();
            EditorGUILayout.LabelField("Shape :", LayoutUtils.Header(12, TextAnchor.MiddleLeft, textHex));
            GUILayout.Space(4);
            LayoutUtils.Indent();
            if (particles.type == RayParticlesType.surface || particles.type == RayParticlesType.surfaceClimbing)
            {
                particles.leftMargin.value = EditorGUILayout.Slider("left margin", particles.leftMargin.value, -0.5f, 0.5f);
                particles.rightMargin.value = EditorGUILayout.Slider("right margin", particles.rightMargin.value, -0.5f, 0.5f);
                particles.topMargin.value = EditorGUILayout.Slider("top margin", particles.topMargin.value, -0.5f, 0.5f);
                particles.bottomMargin.value = EditorGUILayout.Slider("bottom margin", particles.bottomMargin.value, -0.5f, 0.5f);
            }
            else if (particles.type == RayParticlesType.bottomClimbing || particles.type == RayParticlesType.topFalling)
            {
                particles.width.value = EditorGUILayout.Slider("width", particles.width.value, 0f, 3f);
                particles.offsetY.value = EditorGUILayout.Slider("offset Y", particles.offsetY.value, -0.25f, 0.25f);
            }
            LayoutUtils.EndIndent();
        }

        void LightingSettings()
        {
            rays.raysLight.value = EditorGUILayout.Toggle("enable rays urp lighting", rays.raysLight.value);
        }

        void QuadSettings()
        {
            GUILayout.Space(10);
            if (GUILayout.Button("Reset Shape to default")) rays.GenerateQuad();
            GUILayout.Space(10);


            quad.vertBottomLeft.value = EditorGUILayout.Vector3Field("bottom left", quad.vertBottomLeft.value);
            quad.vertBottomRight.value = EditorGUILayout.Vector3Field("bottom right", quad.vertBottomRight.value);
            quad.vertTopRight.value = EditorGUILayout.Vector3Field("top right", quad.vertTopRight.value);
            quad.vertTopLeft.value = EditorGUILayout.Vector3Field("top left", quad.vertTopLeft.value);
        }

        void InspectorFooter()
        {
            if (GUI.changed && !Application.isPlaying)
            {
                EditorUtility.SetDirty(rays);
                EditorSceneManager.MarkSceneDirty(rays.gameObject.scene);
            }

            const string message = "Categories overview : \n" +
            "Camera -> Global camera settings used for getting main camera texture and blending mode, defaults to Main Camera\n" +
            "Light Rays -> settings for customizing your light rays\n" +
            "Particles -> settings for turning on and customizing your particles\n" +
            "Quad -> settings for changing the shape of your light rays";
            EditorGUI.indentLevel += 1;
            GUILayout.Label(message, text);
            EditorGUI.indentLevel -= 1;
        }

        private void OnSceneGUI()
        {
            if (rays == null) return;
            QuadOnSceneGUI();
        }


        void QuadOnSceneGUI()
        {
            Vector2[] raysCorners = MeshUtils.GetWorldCorners2D(rays.meshRenderer);
            float size = HandleUtility.GetHandleSize(rays.mesh.sharedMesh.vertices[0]) * 0.2f;
            var color = new Color(1, 0.8f, 0.4f, 1);

            if (!quadOpen)
            {
                Vector2 topTextPos = new Vector2(raysCorners[3].x, raysCorners[3].y + 0.6f);
                Handles.Label(topTextPos, "edit the shape in the 'Quad' category of the inspector", LayoutUtils.Text(color, 12, FontStyle.Bold));
            }
            else
            {
                Vector3[] verts = MeshUtils.GeVertsWorldSpace(rays.mesh);

                for (int i = 0; i < verts.Length; i++)
                {
                    var fmh_396_65_638410153128427877 = Quaternion.identity; verts[i] = Handles.FreeMoveHandle(verts[i], size, Vector2.zero, Handles.CircleHandleCap);
                }

                verts = MeshUtils.GeVertsLocalSpace(verts, rays.mesh);

                rays.mesh.sharedMesh.vertices = verts;

                quad.vertBottomLeft.value = verts[0];
                quad.vertBottomRight.value = verts[1];
                quad.vertTopRight.value = verts[2];
                quad.vertTopLeft.value = verts[3];
            }

            Vector2 buttonPosition = new Vector2(raysCorners[0].x, raysCorners[0].y - 0.6f);
        }

        private string[] GetSortingLayerNames()
        {
            Type t = typeof(InternalEditorUtility);
            PropertyInfo prop = t.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
            return (string[])prop.GetValue(null, null);
        }
    }

}