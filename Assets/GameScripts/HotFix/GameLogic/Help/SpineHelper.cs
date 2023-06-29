using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace GameLogic
{
    public static class SpineHelper
    {
        static Dictionary<string, UnitSpineInfo> mUnitSpineInfoDict=new Dictionary<string, UnitSpineInfo>();
        static Dictionary<string, SkeletonDataAsset> mUnitSkeletonDataAssetDict = new Dictionary<string, SkeletonDataAsset>();
        static bool mIsInit = true;
        public static void PreloadInit()
        {
            if (!mIsInit)
            {
                return;
            }
            mIsInit = false;
            var SpineResList = ConfigLoader.Instance.Tables.TbEnemySpine.DataList;
            UnitSpineInfo tmpSpineInfo = null;

            for (int i = 0; i < SpineResList.Count; i++)
            {
                string unitIDStr = SpineResList[i].SpineResName;
                GameModule.Resource.LoadAssetAsync<UnitSpineInfo>(unitIDStr,(handle) =>
                {
                    tmpSpineInfo= (UnitSpineInfo)handle.AssetObject;
                    if (mUnitSpineInfoDict.ContainsKey(unitIDStr))
                    {
                        mUnitSpineInfoDict[unitIDStr] = tmpSpineInfo;
                    }
                    else
                    {
                        mUnitSpineInfoDict.Add(unitIDStr, tmpSpineInfo);
                    }
                    ReturnSkeletonDataAsset(tmpSpineInfo,null, unitIDStr);
                });
            }
        }
        static void ReturnSkeletonDataAsset(UnitSpineInfo tmpSpineInfo, Action<SkeletonDataAsset> callback, string unitStr)
        {
            SpineAtlasAsset tmpAtlasAsset = SpineAtlasAsset.CreateRuntimeInstance(tmpSpineInfo.Atlas, tmpSpineInfo.Textures, tmpSpineInfo.Materials, true);
            SkeletonDataAsset tmpSkeletonData = SkeletonDataAsset.CreateRuntimeInstance(tmpSpineInfo.SkelData, tmpAtlasAsset, true);

            if (mUnitSkeletonDataAssetDict.ContainsKey(unitStr))
            {
                mUnitSkeletonDataAssetDict[unitStr] = tmpSkeletonData;
            }
            else
            {
                mUnitSkeletonDataAssetDict.Add(unitStr, tmpSkeletonData);
            }
            callback?.Invoke(tmpSkeletonData);
        }
        /// <summary>
        /// 获取动画数据DataAsset
        /// </summary>
        public static void GetSpineAnimation(string unitName, Action<SkeletonDataAsset,Material> callback)
        {
            SkeletonDataAsset skeletonData;
            if (!mUnitSkeletonDataAssetDict.TryGetValue(unitName,out skeletonData))
            {
                TEngine.Log.Error($"获取Spine动画出错!没有{unitName}动画资源名");
                return;
            }
            else
            {
                callback?.Invoke(skeletonData, mUnitSpineInfoDict[unitName].Materials);
            }
        }
        /// <summary>
        /// 获取动画父节点,统一使用怪物名称+ROOT后缀
        /// </summary>
        /// <returns></returns>
        public static GameObject GetSpineAnimationRoot(string unitName)
        {
            return GameModule.Resource.LoadAsset<GameObject>(unitName + "Root");
        }
    }
}
