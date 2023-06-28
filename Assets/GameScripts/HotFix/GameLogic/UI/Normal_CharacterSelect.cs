using System;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using GameLogic;

namespace TEngine
{
    [Window(UILayer.Normal)]
    class Normal_CharacterSelect : UIWindow
    {
        GameObject mBtnStartGame;


        public override void ScriptGenerator()
        {
            mBtnStartGame = FindChild("Button_Ready").gameObject;
            RegisterEventClick(mBtnStartGame, OpenMainCity);
        }

        void OpenMainCity(GameObject obj, PointerEventData eventData)
        {
            GameEvent.Send(GameProcedureEvent.LoadMainStateEvent.EventId);
            //GameModule.UI.ShowUI<Main_City>();
        }
        void LoadMainUI()
        {
            GameModule.UI.ShowUI<Main_City>();
        }

        public override void AfterShow()
        {
            base.AfterShow();
            GameEvent.AddEventListener(GameProcedureEvent.LoadMainCityUIEvent.EventId, LoadMainUI);
        }
        public override void BeforeClose()
        {
            base.BeforeClose();
            GameEvent.RemoveEventListener(GameProcedureEvent.LoadMainCityUIEvent.EventId, LoadMainUI);
        }
    }
}
