using LeafUtilsRays;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace LightRays2D
{

    [Serializable]
    public class RaysParticlesSettings
    {
        public void SetAllCryoCallbacks(UnityAction a)
        {
            speed.onValueChanged = a; size.onValueChanged = a; density.onValueChanged = a;
            color.onValueChanged = a; noiseStrength.onValueChanged = a; width.onValueChanged = a;
            lifetime.onValueChanged = a; leftMargin.onValueChanged = a; rightMargin.onValueChanged = a;
            topMargin.onValueChanged = a; bottomMargin.onValueChanged = a; enable.onValueChanged = a;
            offsetY.onValueChanged = a;
        }

        //particles look settings
        public RayParticlesType type = RayParticlesType.surface;

        public Cryo<bool> enable = new Cryo<bool>(false);
        public Cryo<float> speed = new Cryo<float>(1f);
        public Cryo<float> size = new Cryo<float>(0.4f);
        public Cryo<float> density = new Cryo<float>(1);
        public Cryo<Color> color = new Cryo<Color>(Color.white);
        public Sprite sprite;
        public Cryo<float> lifetime = new Cryo<float>(5f);
        public Cryo<float> noiseStrength = new Cryo<float>(0.1f);

        //particle system shape settings
        public Cryo<float> width = new Cryo<float>(1f);
        public Cryo<float> offsetY = new Cryo<float>(0f);
        public Cryo<float> leftMargin = new Cryo<float>(0);
        public Cryo<float> rightMargin = new Cryo<float>(0);
        public Cryo<float> topMargin = new Cryo<float>(0);
        public Cryo<float> bottomMargin = new Cryo<float>(0);
    }

}