using System;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

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
            GameModule.UI.ShowUI<Main_City>();
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
