using System;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TEngine
{
    [Window(UILayer.Normal)]
    class Normal_TapToStart : UIWindow
    {
        GameObject mBtnStartGame;
        public override void ScriptGenerator()
        {
            mBtnStartGame = FindChild("TapToStart/Button_Start").gameObject;
            RegisterEventClick(mBtnStartGame, OpenHeroSelect);
        }

        void OpenHeroSelect(GameObject obj, PointerEventData eventData)
        {
            GameModule.UI.ShowUI<Normal_CharacterSelect>();
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
