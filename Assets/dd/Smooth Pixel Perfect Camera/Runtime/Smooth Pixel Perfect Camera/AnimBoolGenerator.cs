#if UNITY_EDITOR

using UnityEngine;
using System;
using System.Collections.Generic;


using UnityEditor.AnimatedValues;


namespace SmoothCam
{
    [Serializable]
    public class AnimBoolGenerator
    {
        [SerializeField] List<AnimBool> animBools = new List<AnimBool>();
        [SerializeField] List<bool> bools = new List<bool>();
        int indexerAB = 0;
        int indexerB = 0;

        public bool GetB()
        {
            indexerB++;
            if (bools.Count > indexerB - 1) return bools[indexerB - 1];
            bools.Add(false);
            return bools[indexerB - 1];
        }

        public AnimBool GetAB()
        {
            indexerAB++;
            if (animBools.Count > indexerAB - 1) return animBools[indexerAB - 1];
            animBools.Add(new AnimBool());
            return animBools[indexerAB - 1];
        }

        public void EndFrame()
        {
            indexerAB = 0;
            indexerB = 0;
        }
    }
}

#endif