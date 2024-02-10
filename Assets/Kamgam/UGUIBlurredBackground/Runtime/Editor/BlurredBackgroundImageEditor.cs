#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Kamgam.UGUIBlurredBackground
{
    [CustomEditor(typeof(BlurredBackgroundImage))]
    [CanEditMultipleObjects]
    public class BlurredBackgroundEditor : UnityEditor.Editor
    {
        BlurredBackgroundImage obj;

        public void OnEnable()
        {
            obj = target as BlurredBackgroundImage;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            obj._EditorApplyChangedValues();
        }
    }

#if UNITY_EDITOR
    public static class ImageCreator
    {
        [MenuItem("GameObject/UI/Blurred Background Image", priority = 2005)]
        public static void Create()
        {
            var go = new GameObject("Image (Blurred Background)");
            var image = go.AddComponent<BlurredBackgroundImage>();

            // Use seclection as parent
            var parent = Selection.activeGameObject;
            if (parent != null)
            {
                go.transform.SetParent(parent.transform);
            }

            // Use first canvas as parent

#if UNITY_2023_1_OR_NEWER
            var canvas = GameObject.FindFirstObjectByType<Canvas>(FindObjectsInactive.Include);
#else
            var canvas = GameObject.FindObjectOfType<Canvas>();
#endif
            if (canvas != null)
            {
                go.transform.SetParent(canvas.transform);
            }

            EditorScheduler.Schedule(1f, image.SetVerticesDirty);
            
        }
    }
    
#endif
}
#endif