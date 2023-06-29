using UnityEngine;
using Spine.Unity;
using GameLogic;

namespace TEngine
{
    public class SpineAnimLoader : ReferenceDisposer
    {
        //动画组件
        protected SkeletonGraphic m_UIAnimation;//动画层级
        protected SkeletonAnimation m_WRAnimation;//世界坐标层级
        public SkeletonGraphic UIAnimation
        {
            get { return m_UIAnimation; }
        }
        public SkeletonAnimation WRAnimation
        {
            get { return m_WRAnimation; }
        }
        GameObject ItemGO;
        GameObject m_SpineParentGO;
        bool m_UIView;
        /// <summary>
        /// 初始化骨骼动画
        /// </summary>
        /// <param name="go">节点</param>
        /// <param name="SpineName">动画资源名</param>
        /// <param name="UIModle">是否是UI层级</param>
        public void Load(GameObject go,string SpineName,bool UIModle=false)
        {
            m_UIView = UIModle;
            ItemGO = go;
            LoadSpineRes(ItemGO, SpineName);
        }
        void LoadSpineRes(GameObject go, string _UnitName)
        {
            m_SpineParentGO = SpineHelper.GetSpineAnimationRoot(_UnitName);
            SpineHelper.GetSpineAnimation(_UnitName, LoadAnimationData);
        }
        /// <summary>
        /// 加载动画资源
        /// </summary>
        void LoadAnimationData(SkeletonDataAsset skeletonDataAsset,Material material)
        {
            if (m_UIView)
            {
                //UI模型
                m_UIAnimation = SkeletonGraphic.NewSkeletonGraphicGameObject(skeletonDataAsset, m_SpineParentGO.transform, material);
            }
            else
            {
                //非UI模型
                m_WRAnimation = SkeletonAnimation.NewSkeletonAnimationGameObject(skeletonDataAsset);
                Transform AnimTr= m_WRAnimation.transform;
                m_SpineParentGO.transform.SetParent(ItemGO.transform, false);
                AnimTr.SetParent(m_SpineParentGO.transform.Find("Animation"));
                AnimTr.localScale = Vector3.one;
                AnimTr.localPosition = Vector3.zero;
                AnimTr.localRotation = Quaternion.identity;

            }
            PlayAnimation("Idle",true);

            //IEnumeratorTool.StartCoroutine(Show());
        }
        /// <summary>
        /// 根据名称播放动画
        /// </summary>
        private void PlayAnimation(string AnimName,bool isLoop=false)
        {
            if (m_UIView)
            {
                m_UIAnimation.AnimationState.SetAnimation(0, AnimName, isLoop);
            }
            else
            {
                m_WRAnimation.AnimationState.SetAnimation(0, AnimName, isLoop);
            }
        }
        public override void Dispose()
        {
            base.Dispose();
            m_UIAnimation = null;
            m_WRAnimation = null;
        }
    }
}