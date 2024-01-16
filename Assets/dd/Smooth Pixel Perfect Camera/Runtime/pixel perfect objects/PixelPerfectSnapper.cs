using UnityEngine;
using UnityEngine.Rendering;

namespace SmoothCam
{
    public class PixelPerfectSnapper : MonoBehaviour
    {
        SmoothPixelPerfectCamera _ppc;
        SmoothPixelPerfectCamera ppc
        {
            get
            {
                if (_ppc == null) _ppc = FindObjectOfType<SmoothPixelPerfectCamera>();
                if (_ppc == null) { Debug.LogWarning("Please add a _smooth-pixel-perfect-camera_ for _pixel-perfect-snapper_ to work!"); }
                return _ppc;
            }
            set { _ppc = value; }
        }

        Vector2 positionDelta;
        bool frame;

        private int GetPPU() => ppc != null ? ppc.pixelsPerUnit.value : 32;

        private Vector2 Delta(Vector2 position, int ppu)
        {
            if (ppu <= 0) return Vector2.zero;

            Vector2 res = new Vector2(0, 0);
            float xx = position.x * (float)ppu;
            float xy = position.y * (float)ppu;

            float roundedxx;
            float roundedxy;

            if (Mathf.Round(xx) != 0)
            {
                roundedxx = (float)Mathf.Round(xx) / (float)ppu + 0.01f;
            }
            else
            {
                roundedxx = 0;
            }

            if (Mathf.Round(xy) != 0)
            {
                roundedxy = (float)Mathf.Round(xy) / (float)ppu + 0.01f;
            }
            else
            {
                roundedxy = 0;
            }

            return new Vector2(roundedxx - position.x, roundedxy - position.y);
        }

        private void ApplyDelta()
        {
            positionDelta = Delta(transform.position, GetPPU());
            transform.position += new Vector3(positionDelta.x, positionDelta.y, 0);
            frame = true;
        }

        private void ReturnPosition(ScriptableRenderContext contex, Camera cam)
        {
            if (!frame) return; frame = false;
            transform.position -= new Vector3(positionDelta.x, positionDelta.y, 0);
        }

        private void LateUpdate() => ApplyDelta();

        private void OnEnable() => RenderPipelineManager.endCameraRendering += ReturnPosition;

        private void OnDestroy() => RenderPipelineManager.endCameraRendering -= ReturnPosition;
    }
}