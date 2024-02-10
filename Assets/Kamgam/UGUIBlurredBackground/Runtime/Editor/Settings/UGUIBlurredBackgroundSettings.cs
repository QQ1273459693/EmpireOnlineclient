#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Kamgam.UGUIBlurredBackground
{
    // Create a new type of Settings Asset.
    public class UGUIBlurredBackgroundSettings : ScriptableObject
    {
        public enum ShaderVariant { Performance, Gaussian };

        public const string Version = "1.0.4"; 
        public const string SettingsFilePath = "Assets/UGUIBlurredBackgroundSettings.asset";

        [SerializeField, Tooltip(_logLevelTooltip)]
        public Logger.LogLevel LogLevel;
        public const string _logLevelTooltip = "Any log above this log level will not be shown. To turn off all logs choose 'NoLogs'";

        [Tooltip("Here you can specify a render texture for debugging.")]
        public RenderTexture DebugRenderTextureScreen;

        [Tooltip("Here you can specify a render texture for debugging.")]
        public RenderTexture DebugRenderTextureWorld;

        [RuntimeInitializeOnLoadMethod]
        static void bindLoggerLevelToSetting()
        {
            // Notice: This does not yet create a setting instance!
            Logger.OnGetLogLevel = () => GetOrCreateSettings().LogLevel;
        }

        [InitializeOnLoadMethod]
        static void autoCreateSettings()
        {
            GetOrCreateSettings();
        }

        static UGUIBlurredBackgroundSettings cachedSettings;

        public static UGUIBlurredBackgroundSettings GetOrCreateSettings()
        {
            if (cachedSettings == null)
            {
                string typeName = typeof(UGUIBlurredBackgroundSettings).Name;

                cachedSettings = AssetDatabase.LoadAssetAtPath<UGUIBlurredBackgroundSettings>(SettingsFilePath);

                // Still not found? Then search for it.
                if (cachedSettings == null)
                {
                    string[] results = AssetDatabase.FindAssets("t:" + typeName);
                    if (results.Length > 0)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(results[0]);
                        cachedSettings = AssetDatabase.LoadAssetAtPath<UGUIBlurredBackgroundSettings>(path);
                    }
                }

                if (cachedSettings != null)
                {
                    SessionState.EraseBool(typeName + "WaitingForReload");
                }

                // Still not found? Then create settings.
                if (cachedSettings == null)
                {
                    CompilationPipeline.compilationStarted -= onCompilationStarted;
                    CompilationPipeline.compilationStarted += onCompilationStarted;

                    // Are the settings waiting for a recompile to finish? If yes then return null;
                    // This is important if an external script tries to access the settings before they
                    // are deserialized after a re-compile.
                    bool isWaitingForReloadAfterCompilation = SessionState.GetBool(typeName + "WaitingForReload", false);
                    if (isWaitingForReloadAfterCompilation)
                    {
                        Debug.LogWarning(typeName + " is waiting for assembly reload.");
                        return null;
                    }

                    cachedSettings = ScriptableObject.CreateInstance<UGUIBlurredBackgroundSettings>();
                    cachedSettings.LogLevel = Logger.LogLevel.Warning;

                    AssetDatabase.CreateAsset(cachedSettings, SettingsFilePath);
                    AssetDatabase.SaveAssets();

                    Logger.OnGetLogLevel = () => cachedSettings.LogLevel;

                    // Import packages and then show welcome screen.
                    PackageImporter.ImportDelayed(onSettingsCreated);
                }
            }

            return cachedSettings;
        }

        private static void onCompilationStarted(object obj)
        {
            string typeName = typeof(UGUIBlurredBackgroundSettings).Name;
            SessionState.SetBool(typeName + "WaitingForReload", true);
        }

        // We use this callback instead of CompilationPipeline.compilationFinished because
        // compilationFinished runs before the assemply has been reloaded but DidReloadScripts
        // runs after. And only after we can access the Settings asset.
        [UnityEditor.Callbacks.DidReloadScripts(999000)]
        public static void DidReloadScripts()
        {
            string typeName = typeof(UGUIBlurredBackgroundSettings).Name;
            SessionState.EraseBool(typeName + "WaitingForReload");
        }

        static void onSettingsCreated()
        {
            SetupShaders.InitOnLoad();

            bool openManual = EditorUtility.DisplayDialog(
                    "UGUI Blurred Background",
                    "Thank you for choosing UGUI Blurred Background.\n\n" +
                    "You'll find the tool under Tools > UGUI Blurred Background > Open\n\n" +
                    "Please start by reading the manual.\n\n" +
                    "It would be great if you could find the time to leave a review.",
                    "Open manual", "Cancel"
                    );

            if (openManual)
            {
                OpenManual();
            }
        }

        [MenuItem("Tools/UGUI Blurred Background/Manual", priority = 101)]
        public static void OpenManual()
        {
            Application.OpenURL("https://kamgam.com/unity/UGUIBlurredBackgroundManual.pdf");
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        [MenuItem("Tools/UGUI Blurred Background/Settings", priority = 101)]
        public static void OpenSettings()
        {
            var settings = UGUIBlurredBackgroundSettings.GetOrCreateSettings();
            if (settings != null)
            {
                Selection.activeObject = settings;
                EditorGUIUtility.PingObject(settings);
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "UGUI Blurred Background Settings could not be found or created.", "Ok");
            }
        }

        [MenuItem("Tools/UGUI Blurred Background/Please leave a review :-)", priority = 410)]
        public static void LeaveReview()
        {
            Application.OpenURL("https://assetstore.unity.com/packages/slug/260862?aid=1100lqC54&pubref=asset");
        }

        [MenuItem("Tools/UGUI Blurred Background/More Asset by KAMGAM", priority = 420)]
        public static void MoreAssets()
        {
            Application.OpenURL("https://assetstore.unity.com/publishers/37829?aid=1100lqC54&pubref=asset");
        }

        [MenuItem("Tools/UGUI Blurred Background/Version: " + Version, priority = 510)]
        public static void LogVersion()
        {
            Debug.Log("UGUI Blurred Background Version: " + Version);
        }

        public void Save()
        {
            EditorUtility.SetDirty(this);
#if UNITY_2021_2_OR_NEWER
            AssetDatabase.SaveAssetIfDirty(this);
#else
            AssetDatabase.SaveAssets();
#endif
        }

    }


#if UNITY_EDITOR
    [CustomEditor(typeof(UGUIBlurredBackgroundSettings))]
    public class UGUIBlurredBackgroundSettingsEditor : Editor
    {
        public UGUIBlurredBackgroundSettings settings;

        public void OnEnable()
        {
            settings = target as UGUIBlurredBackgroundSettings;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Version: " + UGUIBlurredBackgroundSettings.Version);
            base.OnInspectorGUI();
        }
    }
#endif

    static class UGUIBlurredBackgroundSettingsProvider
    {
        [SettingsProvider]
        public static UnityEditor.SettingsProvider CreateUGUIBlurredBackgroundSettingsProvider()
        {
            var provider = new UnityEditor.SettingsProvider("Project/UGUI Blurred Background", SettingsScope.Project)
            {
                label = "UGUI Blurred Background",
                guiHandler = (searchContext) =>
                {
                    var settings = UGUIBlurredBackgroundSettings.GetSerializedSettings();

                    var style = new GUIStyle(GUI.skin.label);
                    style.wordWrap = true;

                    EditorGUILayout.LabelField("Version: " + UGUIBlurredBackgroundSettings.Version);
                    if (drawButton(" Open Manual ", icon: "_Help"))
                    {
                        UGUIBlurredBackgroundSettings.OpenManual();
                    }

                    var settingsObj = settings.targetObject as UGUIBlurredBackgroundSettings;

                    drawField("LogLevel", "Log Level", UGUIBlurredBackgroundSettings._logLevelTooltip, settings, style);

                    settings.ApplyModifiedProperties();
                },

                // Populate the search keywords to enable smart search filtering and label highlighting.
                keywords = new System.Collections.Generic.HashSet<string>(new[] { "shader", "blur", "blurr", "background", "canvas", "ugui", "image", "rendering" })
            };

            return provider;
        }

        static void drawField(string propertyName, string label, string tooltip, SerializedObject settings, GUIStyle style)
        {
            EditorGUILayout.PropertyField(settings.FindProperty(propertyName), new GUIContent(label));
            if (!string.IsNullOrEmpty(tooltip))
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label(tooltip, style);
                GUILayout.EndVertical();
            }
            GUILayout.Space(10);
        }

        static bool drawButton(string text, string tooltip = null, string icon = null, params GUILayoutOption[] options)
        {
            GUIContent content;

            // icon
            if (!string.IsNullOrEmpty(icon))
                content = EditorGUIUtility.IconContent(icon);
            else
                content = new GUIContent();

            // text
            content.text = text;

            // tooltip
            if (!string.IsNullOrEmpty(tooltip))
                content.tooltip = tooltip;

            return GUILayout.Button(content, options);
        }
    }
}
#endif