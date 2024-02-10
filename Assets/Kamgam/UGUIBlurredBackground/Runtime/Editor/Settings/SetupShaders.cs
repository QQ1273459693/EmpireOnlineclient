#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Kamgam.UGUIBlurredBackground
{
    /// <summary>
    /// Since we do not add any objects directly referencing the materials/shaders we
    /// need to make sure the shaders are added to builds so they can be found at runtime.
    /// </summary>
    public static class SetupShaders
    {
        public enum RenderPiplelineType
        {
            URP = 0, HDRP = 1, BuiltIn = 2
        }

        public static RenderPiplelineType GetCurrentRenderPiplelineType()
        {
            var currentRP = GraphicsSettings.currentRenderPipeline;

            // currentRP will be null if built-in renderer is used.
            if (currentRP != null)
            {
                if (currentRP.GetType().Name.Contains("Universal"))
                {
                    return RenderPiplelineType.URP;
                }
                else
                {
                    return RenderPiplelineType.HDRP;
                }
            }

            return RenderPiplelineType.BuiltIn;
        }

        [InitializeOnLoadMethod]
        public static void InitOnLoad()
        {
            EditorApplication.delayCall += AddShaders;
        }

        [MenuItem("Tools/UGUI Blurred Background/Debug/Add shaders to always included shader", priority = 401)]
        public static void AddShaders()
        {
            var pipeline = GetCurrentRenderPiplelineType();

            // BuiltIn
            if (pipeline == RenderPiplelineType.BuiltIn)
                AddShaders(BlurredBackgroundBufferBuiltIn.ShaderName);

#if KAMGAM_RENDER_PIPELINE_URP
            // URP
            if (pipeline == RenderPiplelineType.URP)
                AddShaders(BlurredBackgroundPassURP.ShaderName);
#endif

#if KAMGAM_RENDER_PIPELINE_HDRP
            // HDRP
            if (pipeline == RenderPiplelineType.HDRP)
                AddShaders(BlurredBackgroundPassHDRP.ShaderName);
#endif
        }

        public static void AddShaders(string shaderName)
        {
            // Thanks to: https://forum.unity.com/threads/modify-always-included-shaders-with-pre-processor.509479/#post-3509413

            var shader = Shader.Find(shaderName);
            if (shader == null)
                return;

            var graphicsSettingsObj = AssetDatabase.LoadAssetAtPath<GraphicsSettings>("ProjectSettings/GraphicsSettings.asset");
            var serializedObject = new SerializedObject(graphicsSettingsObj);
            var arrayProp = serializedObject.FindProperty("m_AlwaysIncludedShaders");
            bool hasShader = false;
            for (int i = 0; i < arrayProp.arraySize; ++i)
            {
                var arrayElem = arrayProp.GetArrayElementAtIndex(i);
                if (shader == arrayElem.objectReferenceValue)
                {
                    hasShader = true;
                    break;
                }
            }

            if (!hasShader)
            {
                int arrayIndex = arrayProp.arraySize;
                arrayProp.InsertArrayElementAtIndex(arrayIndex);
                var arrayElem = arrayProp.GetArrayElementAtIndex(arrayIndex);
                arrayElem.objectReferenceValue = shader;

                serializedObject.ApplyModifiedProperties();

                AssetDatabase.SaveAssets();

                Debug.Log("Added the '"+ shaderName + "' shader to always included shaders (see Project Settings > Graphics). UGUI Blurred Background requires it to render the blur. Hope that's okay.");
            }
        }
    }
}
#endif
