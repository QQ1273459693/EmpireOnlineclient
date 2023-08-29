using System;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using GameLogic;
using TMPro;

namespace TEngine
{
    [Window(UILayer.Tips)]
    class TipsWnd_MessageBox : UIWindow
    {
        GameObject m_CancleBtn;
        GameObject m_ConfirmBtn;
        TMP_Text m_MessageText;
        Action m_OkAction;

        [Flags]
        public enum ButtonType
        {
            None,
            Ok,
            No,
            All = Ok | No
        }

        public override void ScriptGenerator()
        {
            m_CancleBtn = FindChild("Tips/Bg/CancleBtn").gameObject;
            m_ConfirmBtn = FindChild("Tips/Bg/ConfirBtn").gameObject;
            m_MessageText = FindChildComponent<TMP_Text>("Tips/Bg/Bg/TitleText");
            RegisterEventClick(m_CancleBtn,CancleBtn);
            RegisterEventClick(m_ConfirmBtn,ConfirmBtn);
        }

        void CancleBtn(GameObject obj, PointerEventData eventData)
        {
            Close();
        }
        public void SetShowData(ButtonType buttonType,string content, Action okAction = null)
        {
            m_MessageText.text = content;
            SetActive(m_CancleBtn, ButtonType.None != (ButtonType.No & buttonType));
            SetActive(m_ConfirmBtn, ButtonType.None != (ButtonType.Ok & buttonType));
            m_OkAction = okAction;
        }
        public static void Show(ButtonType buttonType, string content, Action okAction = null)
        {
            GameModule.UI.ShowUI<TipsWnd_MessageBox>();
            TipsWnd_MessageBox messageBoxWindow = GameModule.UI.FindWindow<TipsWnd_MessageBox>();
            //if (messageBoxWindow == null || !messageBoxWindow.Visible)
            //{
            //    messageBoxWindow = Game.WindowsMgr.ShowWindow<PopupWnd_MessageBox>();
            //}
            messageBoxWindow.SetShowData(buttonType, content, okAction);
        }
        void ConfirmBtn(GameObject obj, PointerEventData eventData)
        {
            m_OkAction?.Invoke();
            Close();
        }

        public override void AfterShow()
        {
            base.AfterShow();
        }
        public override void BeforeClose()
        {
            base.BeforeClose();
        }
    }
}
