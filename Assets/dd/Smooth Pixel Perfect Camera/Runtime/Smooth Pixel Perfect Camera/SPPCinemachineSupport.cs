using UnityEngine;
using Cinemachine;
using System;


namespace SmoothCam
{
    [Serializable]
    [ExecuteAlways]
    public class SPPCinemachineSupport
    {
        [HideInInspector][SerializeField] GameObject cameraObj;
        public static bool RequireSupport(Camera camera) => camera.GetComponent<CinemachineBrain>() != null;

        public SPPCinemachineSupport(GameObject camera)
        {
            cameraObj = camera;
        }

        CinemachineBrain _brain;
        CinemachineBrain brain
        {
            get { if (_brain == null) _brain = cameraObj.GetComponent<CinemachineBrain>(); return _brain; }
        }

        CinemachineVirtualCamera liveCamera
        {
            get { return (CinemachineVirtualCamera)brain.ActiveVirtualCamera; }
        }

        public void CinemachineStart(float size)
        {
            brain.m_UpdateMethod = CinemachineBrain.UpdateMethod.ManualUpdate;
            if (liveCamera != null) liveCamera.m_Lens.OrthographicSize = size;
        }

        public void CinemachineUpdate(float size)
        {
            brain.ManualUpdate();
            if (liveCamera != null) liveCamera.m_Lens.OrthographicSize = size;
        }
    }
}
