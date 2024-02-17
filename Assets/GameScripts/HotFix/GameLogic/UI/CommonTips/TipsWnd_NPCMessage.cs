using System;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using GameLogic;
using TMPro;
using System.Collections.Generic;
using GameConfig.NpcConfigBase;
using static TEngine.Utility;
using Text = UnityEngine.UI.Text;

namespace TEngine
{
    [Window(UILayer.Tips)]
    class TipsWnd_NPCMessage : UIWindow,UIWindow.IInitData<int>
    {
        Text m_TalkDesText;
        RectTransform BgRect;
        UIGridTool m_UiGridTool;
        //数据
        int m_NpcID;
        Dictionary<GameObject,common_NpcTalkButnPool> m_TalkButnPoolList = new Dictionary<GameObject, common_NpcTalkButnPool>();
        class common_NpcTalkButnPool
        {
            public GameObject Btn;
            public int JumpActionID;
            Text m_BtnText;
            public string m_JumpDes;
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
                m_BtnText = Btn.transform.Find("Text").GetComponent<Text>();
            }
            public void InitRefreshTalkInfo(string BtnDes,string JumpDes,int ActionID)
            {
                m_BtnText.text = BtnDes;
                JumpActionID = ActionID;
                m_JumpDes = JumpDes;
            }
            public void Dipose()
            {
                m_EventTriggerListener?.RemoveUIListener();
            }
        }
        public override void ScriptGenerator()
        {
            m_TalkDesText = FindChildComponent<Text>("Tips/Bg/TalkImg/Text");
            BgRect= FindChildComponent<RectTransform>("Tips/Bg");
            m_UiGridTool = new UIGridTool(FindChild("Tips/Bg/Content").gameObject, "common_NpcTalkButton");

            RegisterEventClick(FindChild("Bg").gameObject, CancleBtn);
        }

        void CancleBtn(GameObject obj, PointerEventData eventData)
        {
            Close();
        }

        public override void AfterShow()
        {
            base.AfterShow();
            var NpcConfig = ConfigLoader.Instance.Tables.TBNpcConfigBase.Get(m_NpcID);
            if (NpcConfig==null)
            {
                Log.Error("NPC对话表找不到:"+m_NpcID);
                return;
            }
            m_TalkDesText.text = NpcConfig.Description;
            int Count = NpcConfig.TalkParam.TalkArray.Count;
            m_UiGridTool.GenerateElem(Count+1);
            LayoutRebuilder.ForceRebuildLayoutImmediate(BgRect);
            for (int i = 0; i < Count; i++)
            {
                common_NpcTalkButnPool pool = new common_NpcTalkButnPool();
                pool.Init(m_UiGridTool.Get(i));
                pool.GetClickTipEvent().onClick.AddListener((go,arg) =>
                {
                    switch (pool.JumpActionID)
                    {
                        case 0:
                            //转换描述
                            m_TalkDesText.text = pool.m_JumpDes;
                            m_UiGridTool.GenerateElem(1);
                            LayoutRebuilder.ForceRebuildLayoutImmediate(BgRect);
                            var Pool = m_TalkButnPoolList[m_UiGridTool.Get(0)];
                            Pool.InitRefreshTalkInfo("离开", "", 0);
                            Pool.GetClickTipEvent().onClick.AddListener((go, arg) =>
                            {
                                Close();
                            });
                            break;
                        case 1:
                            //写死指定页面打开
                            GameModule.UI.ShowUI<TipsWnd_FightWoodenPile>();
                            Close();
                            break;
                        case 2:
                            break;
                    }
                });
                var ConfigData = NpcConfig.TalkParam.TalkArray[i];
                pool.InitRefreshTalkInfo(ConfigData.TxtLine, ConfigData.TXTContent, ConfigData.Int);
                m_TalkButnPoolList.Add(pool.Btn,pool);
            }
            //离开单独按钮
            common_NpcTalkButnPool ClosePool = new common_NpcTalkButnPool();
            ClosePool.Init(m_UiGridTool.Get(Count));
            ClosePool.GetClickTipEvent().onClick.AddListener((go, arg) =>
            {
                Close();
            });
            ClosePool.InitRefreshTalkInfo("离开", "",0);
            m_TalkButnPoolList.Add(ClosePool.Btn, ClosePool);
        }
        public override void BeforeClose()
        {
            base.BeforeClose();
            m_UiGridTool.Clear();
            foreach (var item in m_TalkButnPoolList.Values)
            {
                item.Dipose();
            }
            m_TalkButnPoolList.Clear();
        }

        public void InitData(int a)
        {
            m_NpcID = a;
        }
    }
}
