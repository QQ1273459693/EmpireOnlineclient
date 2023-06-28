using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;


namespace GameLogic
{
    public static class SpineHelper
    {
        static Dictionary<string, UnitSpineInfo> mUnitSpineInfoDict=new Dictionary<string, UnitSpineInfo>();
        static Dictionary<string, SkeletonDataAsset> mUnitSkeletonDataAssetDict = new Dictionary<string, SkeletonDataAsset>();
        public static void PreloadInit()
        {

        }
    }
}
