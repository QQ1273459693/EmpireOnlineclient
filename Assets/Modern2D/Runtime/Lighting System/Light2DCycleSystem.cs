using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Modern2D
{
    [ExecuteAlways] //  works in editor too
    public class Light2DCycleSystem : MonoBehaviour
    {
        public float duration = 5f;
        [SerializeField]
        private Gradient gradientColor;
        public Light2D _light;
        public float _startTime;
        public float percentage;
        // Start is called before the first frame update
        void Start()
        {
            _startTime = Time.time;
        }

        private void LateUpdate()
        {
            _light.color = gradientColor.Evaluate(percentage);
        }
        ////// Update is called once per frame
        ////void Update()
        ////{
        ////    //var timeElapsed = Time.time - _startTime;
        ////    //var percentage = Mathf.Sin(timeElapsed/duration*Mathf.PI*2)*0.5F+0.5F;
        ////    //percentage = Mathf.Clamp01(percentage);
        ////    _light.color = gradientColor.Evaluate(percentage);
        ////}
    }
}
