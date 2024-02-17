using System;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using GameLogic;
using TMPro;
using System.Collections.Generic;
using GameConfig.NpcConfigBase;
using GabrielBigardi.SpriteAnimator;
using YooAsset;
using Spine;

namespace TEngine
{
    [Window(UILayer.Tips)]
    class TipsWnd_FightWoodenPile : UIWindow
    {
        UIGridTool m_UiGridTool;
        //数据
        Dictionary<GameObject, common_WoodFightSeletUnitPool> m_PoolList = new Dictionary<GameObject, common_WoodFightSeletUnitPool>();
        class common_WoodFightSeletUnitPool
        {
            public GameObject Btn;
            public int m_EnemyID;
            public Image m_Imag;
            SpriteAnimator animation;
            AssetOperationHandle SpriteAnimationHandle;
            EventTriggerListener m_EventTriggerListener;

            public EventTriggerListener GetClickTipEvent()
            {
                if (m_EventTriggerListener == null)
                {
                    m_EventTriggerListener = EventTriggerListener.Get(Btn);
                }
                return m_EventTriggerListener;
            }
            public void Init(GameObject obj)
            {
                Btn = obj;
                m_Imag = Btn.transform.Find("Image").GetComponent<Image>();
                animation = m_Imag.GetComponent<SpriteAnimator>();
            }
            public void RefreshInfo(int EnmeyID,string ResName)
            {
                m_EnemyID = EnmeyID;
                SpriteAnimationHandle = GameModule.Resource.LoadAssetGetOperation<SpriteAnimationObject>(ResName);
                SpriteAnimationObject spriteAnimation = SpriteAnimationHandle.AssetObject as SpriteAnimationObject;
                animation.ChangeAnimationObject(spriteAnimation);
                animation.enabled = true;

            }
            public void Dipose()
            {
                m_EventTriggerListener?.RemoveUIListener();
                SpriteAnimationHandle?.Release();
                animation.enabled = false;
            }
        }
        GameObject m_EnterBtn;
        List<int> m_SelectEnemyID = new List<int>();
        public override void ScriptGenerator()
        {
            m_UiGridTool = new UIGridTool(FindChild("Tips/FightUnitSelectBg/Content").gameObject, "common_WoodFightSeletUnit");
            m_EnterBtn = FindChild("Tips/FightUnitSelectBg/m_EnterBtnImg").gameObject;
            RegisterEventClick(FindChild("Bg").gameObject, CancleBtn);
            RegisterEventClick(m_EnterBtn, EnterBtn);
        }

        void CancleBtn(GameObject obj, PointerEventData eventData)
        {
            Close();
        }
        void EnterBtn(GameObject obj, PointerEventData eventData)
        {
            //进入战斗界面
            FightRoundWindow.Instance.EnterFight();
            Close();
        }
        public override void AfterShow()
        {
            base.AfterShow();
            var Config = ConfigLoader.Instance.Tables.TbEnemyModelBase.DataList;
            int Count = Config.Count;
            m_UiGridTool.GenerateElem(Count);
            for (int i = 0; i < Count; i++)
            {
                common_WoodFightSeletUnitPool pool = new common_WoodFightSeletUnitPool();
                pool.Init(m_UiGridTool.Get(i));
                pool.GetClickTipEvent().onClick.AddListener((go,arg) =>
                {
                    if (m_SelectEnemyID.Contains(pool.m_EnemyID))
                    {
                        pool.m_Imag.color = Color.white;
                        m_SelectEnemyID.Remove(pool.m_EnemyID);
                    }
                    else
                    {
                        pool.m_Imag.color = Color.red;
                        m_SelectEnemyID.Add(pool.m_EnemyID);
                    }
                });
                var ConfigData = Config[i];

                pool.RefreshInfo(ConfigData.Id, ConfigData.ResName);
                m_PoolList.Add(pool.Btn,pool);
            }
        }
        public override void BeforeClose()
        {
            base.BeforeClose();
            m_UiGridTool.Clear();
            foreach (var item in m_PoolList.Values)
            {
                item.Dipose();
            }
            m_PoolList.Clear();
            m_SelectEnemyID.Clear();
        }
    }
}
