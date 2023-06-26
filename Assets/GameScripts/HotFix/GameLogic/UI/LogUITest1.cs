using System;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TEngine
{
    [Window(UILayer.Main)]
    class LogUITest1 : UIWindow,UIWindow.IInitData<string>,UIWindow.IInitData2<string,int>
    {
        #region 脚本工具生成的代码
        private Text m_textError;
        private Button m_btnClose;
        GameObject mBtnCloseOBJ;
        int Count;
        int HelpEvent = 10020;
        string A;
        public override void ScriptGenerator()
        {
            Log.Debug("首次进来窗口1");
            m_textError = FindChildComponent<Text>("m_textError");
            mBtnCloseOBJ =FindChild("m_btnClose").gameObject;
            GameEvent.AddEventListener(HelpEvent, EventTest);
            RegisterEventClick(mBtnCloseOBJ, OpenWindow);
            //m_btnClose.onClick.AddListener(OpenWindow/*UniTask.UnityAction(OnClickCloseBtn)*/);
        }
        #endregion
        #region 事件
        private async UniTaskVoid OnClickCloseBtn()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            
            Close();
        }
        void EventTest()
        {
            Log.Debug("收到事件了");
        }

        void OpenWindow(GameObject obj, PointerEventData eventData)
        {
            Log.Debug("查询是否有窗口:"+ GameModule.UI.HasWindow<LogUITest2>());
            GameModule.UI.ShowUI<LogUITest2,string>("窗口2");
        }
        #endregion

        public override void AfterShow()
        {
            base.AfterShow();
            Count++;
            m_textError.text = $"这是:{A},已经打开了:{Count}次";
        }

        public void InitData(string a)
        {
            A = a;
        }

        public void InitData(string a, int b)
        {
            A = a;
            Count = b;
        }
    }
}
