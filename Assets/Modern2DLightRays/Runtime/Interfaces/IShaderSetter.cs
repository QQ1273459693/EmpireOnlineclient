using UnityEngine;

namespace LightRays2D
{

    public interface IShaderSetter
    {
        public void SetPropertiesToMaterial(MaterialPropertyBlock mat, Material globalMat = null);
    }

}