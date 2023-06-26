using System;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using GameLogic;
using System.Collections.Generic;

namespace TEngine
{
    [Window(UILayer.Popup)]
    class Popup_ItemTips : UIWindow,UIWindow.IInitData<S_Item>
    {
        GameObject mCloseBtn;
        TMP_Text m_TitleText;

        //��ť�б�
        GameObject m_SellBtn;//���۰�ť
        GameObject m_UnloadBtn;//ж�ذ�ť
        GameObject m_EquipBtn;//װ����ť
        //��ť�б�

        //����ͼƬ
        Image m_ItemColor;
        Image m_ItemBg;
        Image m_ItemIcon;
        TMP_Text m_ItemLvText;
        TMP_Text m_ItemNameText;
        ImageLoad imageLoad;
        ImageLoad IconBgimageLoad;
        //����ͼƬ

        UIGridTool m_EquipAttriGrid;
        TMP_Text m_ItemDes;//
        GameObject m_ItemDesObj;//�л���ť����
        GameObject m_AttriGridObj;

        //���ݲ�
        S_Item itemData;

        public override void ScriptGenerator()
        {
            mCloseBtn = FindChild("ScreenDimmed").gameObject;
            m_TitleText=FindChildComponent<TMP_Text>("Popup/ItemTitle");
            m_SellBtn = FindChild("Popup/GameObject/Button_Sell").gameObject;
            m_UnloadBtn= FindChild("Popup/GameObject/Button_Fuse").gameObject;
            m_EquipBtn= FindChild("Popup/GameObject/Button_Equip").gameObject;

            m_ItemColor = FindChildComponent<Image>("Popup/Item");
            m_ItemBg = FindChildComponent<Image>("Popup/Item/GradeFrame");
            m_ItemIcon= FindChildComponent<Image>("Popup/Item/GradeFrame/Item");
            m_ItemLvText= FindChildComponent<TMP_Text>("Popup/Item/Text (TMP)");
            m_ItemNameText= FindChildComponent<TMP_Text>("Popup/Text_ItemName");
            m_ItemDes= FindChildComponent<TMP_Text>("Popup/ItemDesText");
            m_AttriGridObj = FindChild("Popup/Item_Stats").gameObject;
            m_EquipAttriGrid = new UIGridTool(m_AttriGridObj, "common_AttackDamage");
            m_ItemDesObj = m_ItemDes.gameObject;

            RegisterEventClick(mCloseBtn, CloseBtn);
            RegisterEventClick(m_EquipBtn, EquipBtn);
            RegisterEventClick(m_UnloadBtn, ChangeItemDesBtn);
        }
        /// <summary>
        /// �л�����
        /// </summary>
        void ChangeItemDesBtn(GameObject obj, PointerEventData eventData)
        {
            bool isShow = m_ItemDesObj.activeSelf;
            m_ItemDesObj.SetActive(!isShow);
            m_AttriGridObj.SetActive(isShow);
        }
        /// <summary>
        /// ������ť
        /// </summary>
        void EquipBtn(GameObject obj, PointerEventData eventData)
        {
            GameEvent.Send("BagEquipWear", itemData.item.itemID);
            Close();
        }
        void CloseBtn(GameObject obj, PointerEventData eventData)
        {
            Close();
        }
        //class ItemTipsEquipAttriValue
        //{

        //}
        void ItemTipsEquipAttriValue(TMP_Text AttriName, TMP_Text ValueText,int DictIndex,int Value)
        {
            string Name="";
            switch (DictIndex)
            {
                case 0:
                    Name = "����ֵ";
                    break;
                case 1:
                    Name = "MP";
                    break;
                case 2:
                    Name = "������";
                    break;
                case 3:
                    Name = "������";
                    break;
            }
            AttriName.text = Name;
            ValueText.text = "+"+Value;
        }
        /// <summary>
        /// ˢ����Ϣ��
        /// </summary>
        void RefreshInfo()
        {
            m_EquipAttriGrid.Clear();
            m_ItemDesObj.SetActive(true);
            m_AttriGridObj.SetActive(false);
            var ItemBase = ConfigLoader.Instance.Tables.TbItem.Get(itemData.item.itemID);
            var ItemBgBase = ConfigLoader.Instance.Tables.TbItemBgGround.Get(ItemBase.Quality);
            Color color;
            ColorUtility.TryParseHtmlString(ItemBgBase.ItemColor, out color);
            m_ItemColor.color = color;
            m_ItemNameText.color = color;
            if (imageLoad == null)
            {
                imageLoad = ImageLoad.Create(ItemBase.Icon, m_ItemIcon);
            }
            else
            {
                imageLoad.LoadSprite(ItemBase.Icon, m_ItemIcon);
            }
            if (IconBgimageLoad == null)
            {
                IconBgimageLoad = ImageLoad.Create(ItemBgBase.ItemBoxLine, m_ItemBg);
            }
            else
            {
                IconBgimageLoad.LoadSprite(ItemBgBase.ItemBoxLine, m_ItemBg);
            }
            m_ItemNameText.text = ItemBase.Name;
            m_ItemDes.text = ItemBase.Desc;
            if (itemData.item.isEquipment)
            {
                var EquipBase = ConfigLoader.Instance.Tables.TbEquipment.Get(itemData.item.itemID);
                m_ItemLvText.text = "LV." + itemData.item.equipmentData.Lv;
                m_TitleText.text = "װ��";
                m_EquipBtn.SetActive(true);
                var EquipDict = itemData.item.equipmentData.m_Attribute;
                m_EquipAttriGrid.GenerateElem(EquipDict.Count);
                int Index = 0;
                foreach (var item in EquipDict)
                {
                    GameObject obj = m_EquipAttriGrid.Get(Index);
                    var NameText = obj.transform.Find("Text_Name").GetComponent<TMP_Text>();
                    var ValueNumText = obj.transform.Find("Text_Value").GetComponent<TMP_Text>();
                    ItemTipsEquipAttriValue(NameText, ValueNumText,item.Key,item.Value);
                    Index++;
                }
            }
            else
            {
                m_ItemLvText.text = itemData.item.count.ToString();
                m_EquipBtn.SetActive(false);
                m_TitleText.text = "����";
            }
        }
        public override void AfterShow()
        {
            base.AfterShow();
            RefreshInfo();
        }
        public override void BeforeClose()
        {
            base.BeforeClose();
            m_EquipAttriGrid.Clear();
            IconBgimageLoad.Clear();
            imageLoad.Clear();
            IconBgimageLoad = null;
            imageLoad = null;
        }

        public void InitData(S_Item a)
        {
            Log.Debug("Init������ID��:"+a.item.itemID);
            itemData = a;
        }
    }
}
