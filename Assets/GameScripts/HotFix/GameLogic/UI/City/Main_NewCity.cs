using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using GameLogic;

namespace TEngine
{
    [Window(UILayer.Main)]
    class Main_NewCity : UIWindow
    {
        TMP_Text m_LvText;
        TMP_Text m_GoldNumText;
        TMP_Text m_GemNumText;
        private SlicedFilledImage m_ImageExpFill;//经验条
        private SlicedFilledImage m_ImageHpFill;//血量条
        private SlicedFilledImage m_ImageMpFill;//法力条

        GameObject m_BagClickBtn;//背包按钮

        //数据层结构
        CharacterData m_CharacterData;




        public override void ScriptGenerator()
        {
            m_LvText = FindChildComponent<TMP_Text>("Top/PlayerInfo/LvText");
            m_GoldNumText = FindChildComponent<TMP_Text>("Top/GoldInfo/Num");
            m_GemNumText = FindChildComponent<TMP_Text>("Top/GemInfo/Num");
            m_ImageExpFill = FindChildComponent<SlicedFilledImage>("Top/PlayerInfo/Slider/Progress Bar_EXP/Loading Bar");
            m_ImageHpFill = FindChildComponent<SlicedFilledImage>("Top/PlayerInfo/Slider/Progress Bar_HP/Loading Bar");
            m_ImageMpFill = FindChildComponent<SlicedFilledImage>("Top/PlayerInfo/Slider/Progress Bar_MP/Loading Bar");

            m_BagClickBtn = FindChild("Bottom/SystemList/Bag").gameObject;

            RegisterEventClick(m_BagClickBtn, OnBagClick);
            //RegisterEventClick(mFightBtn, OnFightClick);
            //m_btnClose.onClick.AddListener(OpenWindow/*UniTask.UnityAction(OnClickCloseBtn)*/);
        }

        void OnBagClick(GameObject obj, PointerEventData eventData)
        {
            BagDataController.Instance.ReqGetBagInfo();

            //GameModule.UI.ShowUI<Normal_Bag>();
        }
        void OnFightClick(GameObject obj, PointerEventData eventData)
        {
            GameEvent.Send(GameLogic.GameProcedureEvent.LoadFightStateEvent.EventId);
        }
        void RefreshUI()
        {
            m_CharacterData = GameDataController.Instance.m_CharacterData;
            m_LvText.text = $"Lv.{m_CharacterData.Level}";
            m_GoldNumText.text = m_CharacterData.Gold.ToString();
            m_GemNumText.text=m_CharacterData.Diamond.ToString();
            float HP= (float)m_CharacterData.PlayerAttribute.Hp / (float)m_CharacterData.PlayerAttribute.MaxHp; ;
            m_ImageHpFill.fillAmount = HP;
            m_ImageExpFill.fillAmount = (float)m_CharacterData.Exp / (float)100;
            m_ImageMpFill.fillAmount = (float)m_CharacterData.PlayerAttribute.Mp / (float)m_CharacterData.PlayerAttribute.MaxMp;
            var ItemData = ConfigLoader.Instance.Tables.TbItem1.DataList;
            for (int i = 0; i < ItemData.Count; i++)
            {
                Log.Info($"第{i + 1}个物品,ID:{ItemData[i].Id},名称:{ItemData[i].Name}");
            }


        }
        
        public override void AfterShow()
        {
            base.AfterShow();
            RefreshUI();
            Log.Debug("当前流程是:" + GameModule.Procedure.CurrentProcedure);
        }
        public override void BeforeClose()
        {
            base.BeforeClose();
            
        }
    }
}
