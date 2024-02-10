#if KAMGAM_RENDER_PIPELINE_HDRP
// Based on: https://github.com/alelievr/HDRP-Custom-Passes/blob/2021.2/Assets/CustomPasses/CopyPass/CopyPass.cs#L67
// as recommended by antoinel_unity in https://forum.unity.com/threads/custom-pass-into-render-texture-into-custom-aov.1146872/#post-7362314

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using static UnityEngine.Rendering.HighDefinition.CustomPass;

namespace Kamgam.UGUIBlurredBackground
{
    [ExecuteInEditMode]
    public class BlurredBackgroundPassHDRPVolume : MonoBehaviour
    {
        public CustomPassVolume Volume;

        public CustomPassInjectionPoint InjectionPoint;

        protected BlurredBackgroundPassHDRP _pass;

        protected const HideFlags _hideFlags = HideFlags.DontSave | HideFlags.NotEditable;

        public static BlurredBackgroundPassHDRPVolume FindOrCreate(CustomPassInjectionPoint injectionPoint)
        {
            // Find volume
            var wrappers = Utils.FindRootObjectsByType<BlurredBackgroundPassHDRPVolume>(includeInactive: false);
            foreach (var wVolume in wrappers)
            {
                if (wVolume.InjectionPoint == injectionPoint)
                {
                    return wVolume;
                }
            }

            // Not found -> create
            var go = new GameObject("UGUI BlurredBackground Custom Pass Volume (" + injectionPoint + ")");
            go.hideFlags = _hideFlags;
            Utils.SmartDontDestroyOnLoad(go);

            var volume = go.AddComponent<CustomPassVolume>();
            volume.hideFlags = _hideFlags;
            volume.isGlobal = true;
            volume.injectionPoint = injectionPoint;
            volume.priority = 0;

            var wrapper = go.AddComponent<BlurredBackgroundPassHDRPVolume>();
            wrapper.hideFlags = _hideFlags;
            wrapper.InjectionPoint = injectionPoint;
            wrapper.Volume = volume;

            return wrapper;
        }

        public BlurredBackgroundPassHDRP GetPass()
        {
            return _pass;
        }

        public BlurredBackgroundPassHDRP GetOrCreatePass(ShaderQuality quality, Vector2Int resolution, float offset, int iterations, Color additiveColor)
        {
            if (_pass == null && Volume != null)
            {
                // create pass
                var pass = Volume.AddPassOfType<BlurredBackgroundPassHDRP>();
                pass.enabled = true;
                pass.targetColorBuffer = TargetBuffer.Camera;
                pass.targetDepthBuffer = TargetBuffer.Camera;
                pass.clearFlags = ClearFlag.None;

                _pass = pass as BlurredBackgroundPassHDRP;

                // Init parameters
                _pass.Quality = quality;
                _pass.Resolution = resolution;
                _pass.Offset = offset;
                _pass.Iterations = iterations;
                _pass.AdditiveColor = additiveColor;
            }

            return _pass;
        }

#if UNITY_EDITOR
        protected void OnDestroy()
        {
            if (EditorUtils.WasDestroyedWhileEditing(this))
            {
                FindOrCreate(InjectionPoint);

                var renderer = BlurManager.Instance.Renderer as BlurRendererHDRP;
                renderer?.ClearVolumeCache();

#if UNITY_2023_1_OR_NEWER
                var images = GameObject.FindObjectsByType<BlurredBackgroundImage>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
                var images = GameObject.FindObjectsOfType<BlurredBackgroundImage>();
#endif
                foreach (var img in images)
                {
                    img.SetVerticesDirty();
                }
            }
        }
#endif
    }
}
#endif