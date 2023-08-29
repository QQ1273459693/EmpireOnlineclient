using System;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using GameLogic;
using TMPro;

namespace TEngine
{
    [Window(UILayer.Main)]
    class Main_Login : UIWindow
    {
        GameObject m_LoginBtn;
        GameObject m_RegisterBtn;
        TMP_InputField m_UserNameText;
        TMP_InputField m_PassWordText;

        public override void ScriptGenerator()
        {
            m_RegisterBtn= FindChild("Grid/Register").gameObject;
            m_LoginBtn = FindChild("Grid/Login").gameObject;
            m_UserNameText = FindChildComponent<TMP_InputField>("Grid/UserName/InputField (TMP)");
            m_PassWordText= FindChildComponent<TMP_InputField>("Grid/Password/InputField (TMP)");
            RegisterEventClick(m_LoginBtn, LoginBtn);
            RegisterEventClick(m_RegisterBtn, RegisterBtn);
        }

        void LoginBtn(GameObject obj, PointerEventData eventData)
        {
            TipsWnd_MessageBox.Show(TipsWnd_MessageBox.ButtonType.All, "是否确认登陆?", () =>
            {
                Log.Info($"输入的账号:{m_UserNameText.text},密码:{m_PassWordText.text}");
                LoginController.Instance.ReqLogin(m_UserNameText.text, m_PassWordText.text);
            });
            
            //GameEvent.Send(GameProcedureEvent.LoadMainStateEvent.EventId);
            //GameModule.UI.ShowUI<Main_City>();
        }
        void RegisterBtn(GameObject obj, PointerEventData eventData)
        {
            TipsWnd_MessageBox.Show(TipsWnd_MessageBox.ButtonType.All, "是否确定注册账号?", () =>
            {
                LoginController.Instance.ReqRegister(m_UserNameText.text, m_PassWordText.text);
            });
            
        }

        public override void AfterShow()
        {
            base.AfterShow();
            //GameEvent.AddEventListener(GameProcedureEvent.LoadMainCityUIEvent.EventId, LoadMainUI);
        }
        public override void BeforeClose()
        {
            base.BeforeClose();
            //GameEvent.RemoveEventListener(GameProcedureEvent.LoadMainCityUIEvent.EventId, LoadMainUI);
        }
    }
}
