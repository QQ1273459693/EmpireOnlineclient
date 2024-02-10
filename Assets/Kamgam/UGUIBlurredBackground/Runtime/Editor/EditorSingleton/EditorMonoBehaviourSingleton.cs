#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Kamgam.UGUIBlurredBackground.EditorSingleton
{
    public class EditorMonoBehaviourSingleton<T>
        where T : EditorMonoBehaviourSingleton<T>, new()
    {
        /// <summary>
        /// A MonoBehaviour wrapper that exists across all play mode changes and scene loads.<br />
        /// It executes always. While in Edit Mode it will be triggered by EditorApplication.update.<br />
        /// While in Play Moode it will be triggered by MonoBehaviour.Update.
        /// </summary>
        static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                }

                return _instance;
            }
        }

        public static void Destroy()
        {
            if(_instance != null)
            {
                SmartDestroy(_instance.MonoBehaviour);
                _instance = null;
            }
        }

        public static void SmartDestroy(UnityEngine.Object obj)
        {
            if (obj == null)
            {
                return;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                GameObject.DestroyImmediate(obj);
            }
            else
#endif
            {
                GameObject.Destroy(obj);
            }
        }

        public System.Action OnEnable;
        public System.Action OnUpdate;
        public System.Action OnDisable;
        public System.Action OnDestroy;

        public EditorMonoBehaviour MonoBehaviour;
        protected virtual string getMonoBehaviourName() { return null; }

        public EditorMonoBehaviourSingleton()
        {
#if UNITY_EDITOR
            if (isBuildingOrCompiling())
                return;

            EditorApplication.playModeStateChanged += onPlayModeStateChanged;

            if (EditorPlayState.State != EditorPlayState.PlayState.FromEditToPlay)
                createMonoBehaviour();
#else
            createMonoBehaviour();
#endif
        }

#if UNITY_EDITOR
        private void onPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode || state == PlayModeStateChange.ExitingPlayMode)
            {
                if (MonoBehaviour != null)
                {
                    OnDestroy?.Invoke();
                    GameObject.DestroyImmediate(MonoBehaviour);
                }
                MonoBehaviour = null;
            }
            else if (state == PlayModeStateChange.EnteredPlayMode)
            {
                createMonoBehaviour();
            }
            else if (state == PlayModeStateChange.EnteredEditMode)
            {
                createMonoBehaviour();
            }
        }
#endif

        ~EditorMonoBehaviourSingleton()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= onPlayModeStateChanged;
#endif
        }

#if UNITY_EDITOR
        protected bool isBuildingOrCompiling()
        {
            if (BuildPipeline.isBuildingPlayer || EditorApplication.isCompiling)
                return true;

            return false;
        }
#endif

        public void Refresh()
        {
#if UNITY_EDITOR
            if (EditorPlayState.IsPlaying || EditorPlayState.IsEditing)
                createMonoBehaviour();
#else
            createMonoBehaviour();
#endif
        }

        protected void createMonoBehaviour()
        {
#if UNITY_EDITOR
            if (isBuildingOrCompiling())
                return;
#endif

            if (MonoBehaviour == null || MonoBehaviour.IsDestroyed)
            {
                if (IsSceneAccessible())
                {
                    MonoBehaviour = EditorMonoBehaviour.Instance;

                    if (!string.IsNullOrEmpty(getMonoBehaviourName()))
                        MonoBehaviour.gameObject.name = getMonoBehaviourName();

                    MonoBehaviour.OnEnableCallback += onEnable;
                    MonoBehaviour.OnUpdateCallback += onUpdate;
                    MonoBehaviour.OnDisableCallback += onDisable;
                    MonoBehaviour.OnDestroyCallback += onDestroy;

                    onMonoBehaviourCreated();
                }
            }
        }



        protected virtual void onMonoBehaviourCreated() { }

        protected void onEnable()
        {
            OnEnable?.Invoke();
        }

        protected void onUpdate()
        {
            OnUpdate?.Invoke();
        }

        protected void onDisable()
        {
            OnDisable?.Invoke();
        }

        protected void onDestroy()
        {
            if (MonoBehaviour != null)
            {
                MonoBehaviour.OnEnableCallback -= onEnable;
                MonoBehaviour.OnUpdateCallback -= onUpdate;
                MonoBehaviour.OnDisableCallback -= onDisable;
                MonoBehaviour.OnDestroyCallback -= onDestroy;
            }
            MonoBehaviour = null;

            OnDestroy?.Invoke();
        }

        public static bool IsSceneAccessible()
        {
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

                if (!scene.IsValid())
                    return false;

                // OnEnable() is called on object BEFORE the scene is marked as loaded.
                // However, the scene is already available so we can use it.
                // If we want to use this method in OnEnable we must not abort if isLoaded
                // is false.
                // 
                // The exception is in the EDITOR when the scene is reloaded after EXITING
                // play mode. There the scene is not loaded and not accessible so we have to
                // abort.
#if UNITY_EDITOR
                if (!scene.isLoaded && EditorPlayState.State == EditorPlayState.PlayState.FromPlayToEdit)
                    return false;
#endif
            }

            return UnityEngine.SceneManagement.SceneManager.sceneCount > 0;
        }
    }
}