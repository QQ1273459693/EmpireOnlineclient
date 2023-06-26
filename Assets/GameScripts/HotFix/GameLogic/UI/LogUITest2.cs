using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TEngine
{
    [Window(UILayer.Popup)]
    class LogUITest2 : UIWindow,UIWindow.IInitData<string>
    {
        private Text m_textError;
        private Button m_btnClose;
        int Count;
        string A;
        public override void ScriptGenerator()
        {
            Log.Debug("首次进来窗口2");
            m_textError = FindChildComponent<Text>("m_textError");
            m_btnClose = FindChildComponent<Button>("m_btnClose");
            RegisterEventClick(m_btnClose.gameObject, CloseClick);
        }
        private async UniTaskVoid OnClickCloseBtn()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            
            //Close();
        }
        void CloseClick(GameObject obj, PointerEventData eventData)
        {
            GameEvent.Send(10020);
            Close();
        }

        public override void AfterShow()
        {
            base.AfterShow();
            Count++;
            m_textError.text = $"这是:{A},已经打开了:{Count}次";
        }
        public override void BeforeClose()
        {
            base.BeforeClose();
            Log.Debug("窗口关闭前触发");
        }

        public void InitData(string a)
        {
            A = a;
        }
    }
}
