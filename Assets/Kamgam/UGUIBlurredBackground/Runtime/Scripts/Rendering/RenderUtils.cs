using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kamgam.UGUIBlurredBackground
{
    public static class RenderUtils
    {
        static Camera _cachedGameViewCam;

        static Camera[] _tmpAllCameras = new Camera[10];

        public static Camera GetGameViewCamera(BlurredBackgroundImage image = null)
        {
            var cam = Camera.main;

            // If the current camera is different from the main camera then use it.
            var currentCam = Camera.current;
            if (currentCam != null && currentCam.cameraType == CameraType.Game && currentCam != cam)
            {
                cam = currentCam;
            }

            // In the editor (if not playing) then also check all the cameras and find the one that is
            // the most likely to be the current renderin camera of the game view.
            // This is done since Camera.current is not always set (often null) in Editor.
#if UNITY_EDITOR
            // We sort by depth and start from the back
            // because we assume that among cameras with equal depth the last takes precedence.
            float maxDepth = float.MinValue;
            int allCamerasCount = Camera.GetAllCameras(_tmpAllCameras);
            for (int i = allCamerasCount - 1; i >= 0; i--)
            {
                if (!_tmpAllCameras[i].isActiveAndEnabled)
                    continue;

                if (_tmpAllCameras[i].depth > maxDepth)
                {
                    maxDepth = _tmpAllCameras[i].depth;
                    currentCam = _tmpAllCameras[i];
                }
            }
            if (currentCam != null && currentCam != Camera.main)
            {
                cam = currentCam;
            }
#endif

            // Use the camera of the last activated image in a screen space camera canvas (if there is one).
            if (image != null && image.GetRenderMode() == RenderMode.ScreenSpaceCamera && image.canvas != null)
            {
                cam = image.canvas.worldCamera;
            }

            // cache game view camera
            if (cam != null && cam.cameraType == CameraType.Game)
                _cachedGameViewCam = cam;

            if (cam == null)
                return _cachedGameViewCam;

            return cam;
        }
    }
}