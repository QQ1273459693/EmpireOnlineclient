﻿using System;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TEngine
{
    [Window(UILayer.Normal)]
    class Normal_PlayerVisitWindow : UIWindow
    {
        GameObject m_Close;
        Toggle m_BagToggle;
        Toggle m_RoleInfoToggle;
        Toggle m_SkillInfoToggle;



        Normal_BagViewData BagWindow = new Normal_BagViewData();
        Normal_RoleInfoViewData RoleInfoWindow=new Normal_RoleInfoViewData();
        Normal_SkillInfoViewData SkillInfoWindow = new Normal_SkillInfoViewData();

        public override void ScriptGenerator()
        {
            m_Close= FindChild("Close").gameObject;
            m_BagToggle = FindChildComponent<Toggle>("TagList/Tag (1)");
            m_RoleInfoToggle= FindChildComponent<Toggle>("TagList/Tag");
            m_SkillInfoToggle= FindChildComponent<Toggle>("TagList/Tag (2)");

            RegisterEventClick(m_Close, OnExitClick);

            m_BagToggle.onValueChanged.AddListener(BagToggleValue);
            m_RoleInfoToggle.onValueChanged.AddListener(RoleInfoToggleValue);
            m_SkillInfoToggle.onValueChanged.AddListener(SkillInfoToggleValue);

            //数据页签加载
            BagWindow.Init(FindChild("Normal_NewBag").gameObject);
            RoleInfoWindow.Init(FindChild("Normal_RoleInfo").gameObject);
            SkillInfoWindow.Init(FindChild("Normal_SkillInfo").gameObject);

        }
        void OnExitClick(GameObject obj, PointerEventData eventData)
        {
            Close();
        }
        void BagToggleValue(bool isOn)
        {
        }
        void RoleInfoToggleValue(bool isOn)
        {
            
        }
        void SkillInfoToggleValue(bool isOn)
        {
            if (isOn)
            {
                SkillInfoWindow.ClickRefresh();
            }
        }
        public override void AfterShow()
        {
            base.AfterShow();
            //数据页签加载
            BagWindow.AfterShow();
            RoleInfoWindow.AfterShow();
            SkillInfoWindow.AfterShow();
        }
        public override void BeforeClose()
        {
            base.BeforeClose();
            BagWindow.BeforeClose();
            RoleInfoWindow.BeforeClose();
            SkillInfoWindow.BeforeClose();
        }
    }
}
