using System;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Pool;
using GameLogic;
using DG.Tweening;
using System.Collections.Generic;

namespace TEngine
{
    class Normal_SkillInfoViewData
    {
        RectTransform m_Content;//被动格子


        RectTransform m_PassivityObjP;//被动格子
        RectTransform m_InitiativeObjP;//主动格子
        RectTransform m_AutoObjP;//自动格子

        RectTransform m_PassivityObj;//被动格子
        RectTransform m_InitiativeObj;//主动格子
        RectTransform m_AutoObj;//自动格子
        UIGridTool m_PassivityGridTool;
        UIGridTool m_InitiativeGridTool;
        UIGridTool m_AutoGridTool;
        //数据层
        List<Common_SkillSlot> m_PassivityDataList = new List<Common_SkillSlot>();
        List<Common_SkillSlot> m_InitiativeDataList = new List<Common_SkillSlot>();
        List<Common_SkillSlot> m_AutoDataList = new List<Common_SkillSlot>();



        class Common_SkillSlot
        {
            GameObject m_Slot;
            Text m_SkillNameText;
            Image m_SkillIcon;
            ImageLoad m_ImageLoad;
            Text m_LvText;
            EventTriggerListener m_EventTriggerListener;
            public SkillData skill;
            public void Init(GameObject OBJ)
            {
                m_Slot = OBJ;
                m_SkillNameText = DUnityUtil.FindChildComponent<Text>(OBJ.transform, "Text (Legacy)");
                m_SkillIcon = DUnityUtil.FindChildComponent<Image>(OBJ.transform, "Icon");
                m_LvText= DUnityUtil.FindChildComponent<Text>(OBJ.transform, "Text (Legacy) (1)");
            }
            public EventTriggerListener GetClickTipEvent()
            {
                if (m_EventTriggerListener == null)
                {
                    m_EventTriggerListener = EventTriggerListener.Get(m_Slot);
                }
                return m_EventTriggerListener;
            }
            public void Refresh(SkillData skillData)
            {
                skill=skillData;
                var SkillBase = ConfigLoader.Instance.Tables.TbSwordSkillBase.Get(skillData.SkID);
                m_SkillNameText.text = SkillBase.Name[skillData.Lv];
                m_LvText.text = skillData.Lv + "级";
                if (m_ImageLoad == null)
                {
                    m_ImageLoad = ImageLoad.Create(SkillBase.Icon, m_SkillIcon);
                }
                else
                {
                    m_ImageLoad.LoadSprite(SkillBase.Icon, m_SkillIcon);
                }
            }
            public void Dipose()
            {
                m_EventTriggerListener?.RemoveUIListener();
                m_EventTriggerListener = null;
                m_ImageLoad?.Clear();
                m_ImageLoad = null;
            }
        }

        public void Init(GameObject obj)
        {
            m_Content= DUnityUtil.FindChildComponent<RectTransform>(obj.transform, "ViewContent/Content");
            m_PassivityObjP = DUnityUtil.FindChildComponent<RectTransform>(obj.transform, "ViewContent/Content/PassivitySkill");
            m_InitiativeObjP = DUnityUtil.FindChildComponent<RectTransform>(obj.transform, "ViewContent/Content/initiativeSkill");
            m_AutoObjP = DUnityUtil.FindChildComponent<RectTransform>(obj.transform, "ViewContent/Content/AutoSkill");



            m_PassivityObj = DUnityUtil.FindChildComponent<RectTransform>(m_PassivityObjP.transform, "LoopSkill");
            m_InitiativeObj = DUnityUtil.FindChildComponent<RectTransform>(m_InitiativeObjP.transform,"LoopSkill");
            m_AutoObj = DUnityUtil.FindChildComponent<RectTransform>(m_AutoObjP.transform,"LoopSkill");

            m_PassivityGridTool = new UIGridTool(m_PassivityObj.gameObject, "common_SkillSlot");
            m_InitiativeGridTool = new UIGridTool(m_InitiativeObj.gameObject, "common_SkillSlot");
            m_AutoGridTool = new UIGridTool(m_AutoObj.gameObject, "common_SkillSlot");

        }
        public void AfterShow()
        {
            var m_CharData = GameDataController.Instance.m_CharacterData;
            m_PassivityGridTool.GenerateElem(m_CharData.PassiveSkills.Count);
            m_InitiativeGridTool.GenerateElem(m_CharData.ActiveSkills.Count);
            m_AutoGridTool.GenerateElem(m_CharData.AutoSkills.Count);

            for (int i = 0; i < m_CharData.PassiveSkills.Count; i++)
            {
                Common_SkillSlot common_Skill = new Common_SkillSlot();
                common_Skill.Init(m_PassivityGridTool.Get(i));
                common_Skill.GetClickTipEvent().onClick.AddListener((go, arg) =>
                {
                    GameModule.UI.ShowUI<TipsWnd_TipsInfo, SkillData>(common_Skill.skill);

                });
                common_Skill.Refresh(m_CharData.PassiveSkills[i]);
            }

            for (int i = 0; i < m_CharData.ActiveSkills.Count; i++)
            {
                Common_SkillSlot common_Skill = new Common_SkillSlot();
                common_Skill.Init(m_InitiativeGridTool.Get(i));
                common_Skill.GetClickTipEvent().onClick.AddListener((go, arg) =>
                {
                    GameModule.UI.ShowUI<TipsWnd_TipsInfo, SkillData>(common_Skill.skill);

                });
                common_Skill.Refresh(m_CharData.ActiveSkills[i]);
            }

            for (int i = 0; i < m_CharData.AutoSkills.Count; i++)
            {
                Common_SkillSlot common_Skill = new Common_SkillSlot();
                common_Skill.Init(m_AutoGridTool.Get(i));
                common_Skill.GetClickTipEvent().onClick.AddListener((go, arg) =>
                {
                    GameModule.UI.ShowUI<TipsWnd_TipsInfo, SkillData>(common_Skill.skill);

                });
                common_Skill.Refresh(m_CharData.AutoSkills[i]);
            }
            Log.Info("已经点击了");
           
        }
        public void ClickRefresh()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_Content);
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_PassivityObj);
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_InitiativeObj);
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_AutoObj);
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_AutoObjP);
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_InitiativeObjP);
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_PassivityObjP);
        }
        public  void BeforeClose()
        {
            for (int i = 0; i < m_PassivityDataList.Count; i++)
            {
                m_PassivityDataList[i].Dipose();
            }
            m_PassivityDataList.Clear();

            for (int i = 0; i < m_InitiativeDataList.Count; i++)
            {
                m_InitiativeDataList[i].Dipose();
            }
            m_InitiativeDataList.Clear();

            for (int i = 0; i < m_AutoDataList.Count; i++)
            {
                m_AutoDataList[i].Dipose();
            }
            m_AutoDataList.Clear();


            m_PassivityGridTool.Clear();
            m_InitiativeGridTool.Clear();
            m_AutoGridTool.Clear();
        }
    }
}
