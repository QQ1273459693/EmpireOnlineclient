using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Kamgam.UGUIBlurredBackground.EditorSingleton
{
    [ExecuteAlways]
    public class EditorMonoBehaviour : MonoBehaviour
    {
        public bool IsDestroyed { get; protected set; }

        public Action OnEnableCallback;
        public Action OnUpdateCallback;
        public Action OnDisableCallback;
        public Action OnDestroyCallback;

        static EditorMonoBehaviour _instance;
        public static EditorMonoBehaviour Instance // This is triggered by the UGUI Image
        {
            get
            {
                if (_instance == null || _instance.IsDestroyed)
                {
                    _instance = FindRootObjectByType<EditorMonoBehaviour>(includeInactive: true);

                    if (_instance == null || _instance.gameObject == null || _instance.IsDestroyed)
                    {
                        //Debug.Log("new EditorMonoBehaviour");
                        var go = new GameObject(typeof(EditorMonoBehaviour).FullName);
#if UNITY_EDITOR
                        if (EditorApplication.isPlaying)
                        {
#endif
                            GameObject.DontDestroyOnLoad(go);
#if UNITY_EDITOR
                        }
#endif
                        go.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;

                        _instance = go.AddComponent<EditorMonoBehaviour>();
                    }
                }

                return _instance;
            }
        }

        private static List<GameObject> _tmpSceneObjects = new List<GameObject>(20);

        public static T FindRootObjectByType<T>(bool includeInactive) where T : Component
        {
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

                if (!scene.IsValid())
                    continue;

                // OnEnable() is called on object BEFORE the scene is marked as loaded.
                // However, the scene is already available so we can use it.
                // If we want to use this method in OnEnable we must not abort if isLoaded
                // is false.
                // 
                // The excpetion is in the EDITOR when the scene is reloaded after EXITING
                // play mode. There the scene is not loaded and not accessible so we have to
                // abort.
#if UNITY_EDITOR
                if (!scene.isLoaded && !EditorApplication.isPlayingOrWillChangePlaymode)
                    continue;
#endif

                scene.GetRootGameObjects(_tmpSceneObjects);

                foreach (var obj in _tmpSceneObjects)
                {
                    var comp = obj.GetComponent<T>();
                    if (comp == null)
                        continue;

                    if (!includeInactive && !comp.gameObject.activeInHierarchy)
                        continue;

                    return comp;
                }
            }

            return default;
        }

        public void OnEnable()
        {
            if (IsDestroyed)
                return;

            OnEnableCallback?.Invoke();
        }

        public void Update()
        {
            if (IsDestroyed)
                return;

            OnUpdateCallback?.Invoke();
        }

        public void OnDisable()
        {
            if (IsDestroyed)
                return;

            OnDisableCallback?.Invoke();
        }

        public void OnDestroy()
        {
            IsDestroyed = true;
            OnDestroyCallback?.Invoke();
        }       
    }
}