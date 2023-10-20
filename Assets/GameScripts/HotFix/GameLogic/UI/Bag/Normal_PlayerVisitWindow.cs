using System;
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




        Normal_BagViewData BagWindow = new Normal_BagViewData();
        Normal_RoleInfoViewData RoleInfoWindow=new Normal_RoleInfoViewData();

        public override void ScriptGenerator()
        {
            m_Close= FindChild("Close").gameObject;
            m_BagToggle = FindChildComponent<Toggle>("TagList/Tag (1)");
            m_RoleInfoToggle= FindChildComponent<Toggle>("TagList/Tag");

            RegisterEventClick(m_Close, OnExitClick);

            m_BagToggle.onValueChanged.AddListener(BagToggleValue);
            m_RoleInfoToggle.onValueChanged.AddListener(RoleInfoToggleValue);


            //数据页签加载
            BagWindow.Init(FindChild("Normal_NewBag").gameObject);
            RoleInfoWindow.Init(FindChild("Normal_RoleInfo").gameObject);


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
        public override void AfterShow()
        {
            base.AfterShow();
            //数据页签加载
            BagWindow.AfterShow();
            RoleInfoWindow.AfterShow();
        }
        public override void BeforeClose()
        {
            base.BeforeClose();
            BagWindow.BeforeClose();
            RoleInfoWindow.BeforeClose();
        }
    }
}
