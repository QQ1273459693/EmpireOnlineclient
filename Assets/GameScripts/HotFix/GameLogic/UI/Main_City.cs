using System;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TEngine
{
    [Window(UILayer.Main)]
    class Main_City : UIWindow
    {
        GameObject mBtnBag;
        public override void ScriptGenerator()
        {
            mBtnBag = FindChild("Home/HomeMenu/Inventory").gameObject;
            RegisterEventClick(mBtnBag,OnBagClick);
            //m_btnClose.onClick.AddListener(OpenWindow/*UniTask.UnityAction(OnClickCloseBtn)*/);
        }

        void OnBagClick(GameObject obj, PointerEventData eventData)
        {
            Log.Debug("点击了");
            GameModule.UI.ShowUI<Normal_Bag>();
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
