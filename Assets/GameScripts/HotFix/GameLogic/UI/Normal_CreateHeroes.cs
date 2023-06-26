using System;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TEngine
{
    [Window(UILayer.Normal)]
    class Normal_CreateHeroes : UIWindow
    {
        GameObject mBtnStartGame;
        public override void ScriptGenerator()
        {
            mBtnStartGame = FindChild("TapToStart/Button_Start").gameObject;
            RegisterEventClick(mBtnStartGame, OpenMainCity);
        }

        void OpenMainCity(GameObject obj, PointerEventData eventData)
        {
            GameModule.UI.ShowUI<Main_City>();
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
