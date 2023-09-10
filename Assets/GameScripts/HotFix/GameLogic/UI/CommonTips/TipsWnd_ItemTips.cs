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
    class TipsWnd_ItemTips : UIWindow,UIWindow.IInitData<ItemData>
    {
        GameObject m_CloseBtn;
        GameObject m_UseBtn;//使用按钮
        GameObject m_WearBtn;//穿戴装备按钮
        Text m_ItemInfoText;
        RectTransform m_ItemEffectRect;
        UIGridTool m_ItemEffectGrid;

        //数据层

        [Flags]
        public enum ItemType
        {
            UseProps=1,//使用道具
            Currency=2,//货币
            Materials=3,//材料
            Equipment=4,//装备
        }

        public override void ScriptGenerator()
        {
            m_ItemInfoText = FindChildComponent<Text>("Bg/Tips/Top/ItemInfoText");
            m_ItemEffectRect = FindChildComponent<RectTransform>("Bg/Tips/RectScroll/Mid");
            m_CloseBtn = FindChild("Bg/Tips/Top/CloseBtn").gameObject;
            m_UseBtn = FindChild("Bg/Tips/ButtonList/UseBtn").gameObject;
            m_WearBtn = FindChild("Bg/Tips/ButtonList/WearBtn").gameObject;

            m_ItemEffectGrid = new UIGridTool(m_ItemEffectRect.gameObject, "common_ItemTipsText");

            RegisterEventClick(m_CloseBtn, CancleBtn);
        }

        void CancleBtn(GameObject obj, PointerEventData eventData)
        {
            Close();
        }

        public override void AfterShow()
        {
            base.AfterShow();
        }
        public override void BeforeClose()
        {
            base.BeforeClose();


            m_ItemEffectGrid.Clear();
        }

        public void InitData(ItemData a)
        {
            throw new NotImplementedException();
        }
    }
}
