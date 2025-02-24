﻿using System;
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
        GameObject mFightBtn;
        public override void ScriptGenerator()
        {
            mBtnBag = FindChild("Home/HomeMenu/Inventory").gameObject;
            mFightBtn = FindChild("Home/Button_Play").gameObject;
            RegisterEventClick(mBtnBag,OnBagClick);
            RegisterEventClick(mFightBtn, OnFightClick);
            //m_btnClose.onClick.AddListener(OpenWindow/*UniTask.UnityAction(OnClickCloseBtn)*/);
        }

        void OnBagClick(GameObject obj, PointerEventData eventData)
        {
            GameModule.UI.ShowUI<Normal_Bag>();
        }
        void OnFightClick(GameObject obj, PointerEventData eventData)
        {
            GameEvent.Send(GameLogic.GameProcedureEvent.LoadFightStateEvent.EventId);
        }

        public override void AfterShow()
        {
            base.AfterShow();
            Log.Debug("当前流程是:" + GameModule.Procedure.CurrentProcedure);
        }
        public override void BeforeClose()
        {
            base.BeforeClose();
        }
    }
}
