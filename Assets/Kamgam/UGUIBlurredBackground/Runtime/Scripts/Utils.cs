using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Kamgam.UGUIBlurredBackground
{
    public static class Utils
    {
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

        public static void SmartDontDestroyOnLoad(GameObject go)
        {
            if (go == null)
            {
                return;
            }

#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                GameObject.DontDestroyOnLoad(go);
            }
#else
            GameObject.DontDestroyOnLoad(go);
#endif
        }

        private static List<GameObject> _tmpSceneObjects = new List<GameObject>();

        public static List<T> FindRootObjectsByType<T>(bool includeInactive) where T : Component
        {
            var results = new List<T>();
            FindRootObjectsByType(includeInactive, results);
            return results;
        }

        /// <summary>
        /// A simple replacement for GameObject.FindObjectsOfType<T>. It checks the ROOT objects in ALL opened or loaded scenes.
        /// </summary>
        /// <param name="includeInactive"></param>
        /// <param name="results">A list that will be cleared and then filled with the results.</param>
        /// <returns></returns>
        public static void FindRootObjectsByType<T>(bool includeInactive, IList<T> results) where T : Component
        {
            if (results == null)
            {
                results = new List<T>();
            }
            else
            {
                results.Clear();
            }

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

                    results.Add(comp);
                }
            }
        }

        public static bool IsAnySceneAccessible()
        {
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (!scene.IsValid())
                    return false;

                try
                {
                    scene.GetRootGameObjects(_tmpSceneObjects);
                }
                catch
                {
                    return false;
                }
            }

            return UnityEngine.SceneManagement.SceneManager.sceneCount > 0;
        }

        /// <summary>
        /// A simple replacement for GameObject.FindObjectsOfType<T>. It checks the ROOT objects in ALL opened or loaded scenes.
        /// </summary>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
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
    }
}