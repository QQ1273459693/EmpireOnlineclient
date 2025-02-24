﻿using System;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using GameLogic;
using TMPro;
using System.Collections.Generic;

namespace TEngine
{
    [Window(UILayer.Tips)]
    class TipsWnd_ItemTips : UIWindow,UIWindow.IInitData<Slot>
    {
        GameObject m_CloseBtn;
        GameObject m_UseBtn;//使用按钮
        GameObject m_WearBtn;//穿戴装备按钮
        Text m_WearBtnText;//穿戴卸载文本
        Text m_ItemInfoText;
        RectTransform m_ItemEffectRect;
        UIGridTool m_ItemEffectGrid;


        Image m_ItemBG;
        Image IconImg;
        ImageLoad IconLoad;
        ImageLoad BgLoad;

        string[] EquipMentTranslate = { "大剑", "护手", "胸甲", "护盾", "头盔", "护肩", "鞋子", "护腿" };
        string[] ItemTypeTranslate = { "道具", "货币", "材料","装备" };
        //数据层
        Slot mSlot { get; set; }
        ItemData m_ItemData;
        ItemType m_ItemType;
        int m_EquipPosIdx;//如果是装备则有装备位置
        List<common_ItemTipsTextPool> m_TipsEffectList = new List<common_ItemTipsTextPool>();


        class common_ItemTipsTextPool
        {
            public int Index;
            string TextDes;

            Text m_Text;
            public void Init(GameObject obj)
            {
                m_Text = obj.GetComponent<Text>();
                m_Text.text = TextDes;
            }
            public void InitRefreshText(int index,string Effect)
            {
                Index= index;
                TextDes = Effect;
            }
        }


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
            m_WearBtnText = FindChildComponent<Text>(m_WearBtn.transform, "Text");

            m_ItemBG = FindChildComponent<Image>("Bg/Tips/Top/common_Item1");
            IconImg = FindChildComponent<Image>("Bg/Tips/Top/common_Item1/ItemIcon");

            m_ItemEffectGrid = new UIGridTool(m_ItemEffectRect.gameObject, "common_ItemTipsText");

            RegisterEventClick(m_CloseBtn, CancleBtn);
            RegisterEventClick(m_WearBtn, WearOnBtn);
        }

        void CancleBtn(GameObject obj, PointerEventData eventData)
        {
            Close();
        }
        void WearOnBtn(GameObject obj, PointerEventData eventData)
        {
            BagDataController.Instance.ReqEquipWear(mSlot.idx>0?true:false, m_EquipPosIdx, mSlot.idx);
            Close();
        }

        public override void AfterShow()
        {
            base.AfterShow();
            RefreshInfo();


        }
        /// <summary>
        /// 插入数据
        /// </summary>
        public void SetRefreshInfo(Slot slot)
        {
            mSlot = slot;
            m_ItemData = mSlot.itemData;
            RefreshInfo();

        }
        void RefreshInfo()
        {
            var ItemBase= ConfigLoader.Instance.Tables.TbItem1.Get(m_ItemData.item.itemId);
            m_ItemType = (ItemType)ItemBase.Type;
            var ItemBackBase = ConfigLoader.Instance.Tables.TbItemBgGround1.Get(ItemBase.Quality);
            string ItemColorFmt = ItemBackBase.ItemColor;
            m_ItemInfoText.text = m_ItemType == ItemType.Equipment ? string.Format(ItemColorFmt, ItemBase.Name): $"{string.Format(ItemColorFmt, ItemBase.Name)}X{m_ItemData.count}";

            if (IconLoad == null)
            {
                IconLoad = ImageLoad.Create(ItemBase.Icon, IconImg);
            }
            else
            {
                IconLoad.LoadSprite(ItemBase.Icon, IconImg);
            }
            if (BgLoad == null)
            {
                BgLoad = ImageLoad.Create(ItemBackBase.ItemBoxLine, m_ItemBG);
            }
            else
            {
                BgLoad.LoadSprite(ItemBackBase.ItemBoxLine, m_ItemBG);
            }
            m_TipsEffectList.Clear();
            int Count = 0;//1,2,4
            //添加文本描述,1:写死品质,2:写死描述,3写死宝石镶嵌次数如果有4:还要加宝石效果字段,5:写死物品种类或者装备类型,6:读取多段效果文本,7:装备写死等级需求否则为空

            common_ItemTipsTextPool textPool1 = new common_ItemTipsTextPool();
            common_ItemTipsTextPool textPool2 = new common_ItemTipsTextPool();
            common_ItemTipsTextPool textPool3 = new common_ItemTipsTextPool();

            textPool1.InitRefreshText(1, $"品质:{ItemBackBase.QualityName}");
            textPool2.InitRefreshText(2, ItemBase.Desc);

            m_TipsEffectList.Add(textPool1);
            m_TipsEffectList.Add(textPool2);
            m_TipsEffectList.Add(textPool3);


            if (m_ItemType== ItemType.Equipment)
            {
                m_WearBtn.SetActive(true);
                m_WearBtnText.text = mSlot.idx > 0 ? "穿戴" : "卸载";
                var EquipBase = ConfigLoader.Instance.Tables.TbEquipmentBase.Get(ItemBase.Id);
                m_EquipPosIdx = EquipBase.SlotPos;
                textPool3.InitRefreshText(5, $"装备类型:{EquipMentTranslate[(int)(EquipBase.EquipType-1)]}");
                //这件物品是装备
                //有宝石介绍,有等级需求
                common_ItemTipsTextPool textPool4 = new common_ItemTipsTextPool();
                textPool4.InitRefreshText(3,$"宝石镶嵌次数:{m_ItemData.item.equipData.slv}/{20}");
                m_TipsEffectList.Add(textPool4);

                if (m_ItemData.item.equipData.slv>0)
                {
                    //说明至少有一颗宝石,添加加成效果文本
                   
                    common_ItemTipsTextPool textPool5 = new common_ItemTipsTextPool();
                    textPool5.InitRefreshText(4,$"宝石效果:{ConfigLoader.Instance.Tables.TbFightingBase.Get(m_ItemData.item.equipData.GemAttributeID).Name}:+{450+ m_ItemData.item.equipData.GemAttributeID}");
                    m_TipsEffectList.Add(textPool5);
                }

                //基础属性字段文本
                var BaseButeList = EquipBase.BaseAttriBute.BaseAttribute;
                for (int i = 0; i < BaseButeList.Count; i++)
                {
                    common_ItemTipsTextPool text = new common_ItemTipsTextPool();
                    var Bute = BaseButeList[i];
                    //这里应该写死,如果是武器伤害就单行显示,另外一种是魔法武器伤害都是单行显示
                    var Value = ConfigLoader.Instance.Tables.TbFightingBase.Get(Bute.AttriID);
                    if (Value.Id == 20)
                    {
                        //是近战武器伤害直接显示 :200-300
                        text.InitRefreshText(6 + i, $"攻击力:{Bute.MixValue}~{Bute.MaxValue}");
                    }
                    else
                    {
                        text.InitRefreshText(6 + i, $"{Value.Name}:+{Bute.MixValue}");
                    }
                    m_TipsEffectList.Add(text);
                }

                for (int i = 0; i < EquipBase.Attribute.MainAttribute.Count; i++)
                {
                    common_ItemTipsTextPool text = new common_ItemTipsTextPool();
                    var Bute = EquipBase.Attribute.MainAttribute[i];
                    var Value = ConfigLoader.Instance.Tables.TbFightingBase.Get(Bute.AttriID);
                    text.InitRefreshText(7 + i, $"效果:{Value.Name}+{Bute.Value}{(Bute.Percent == 1 ? "%" : "")}");
                    m_TipsEffectList.Add(text);
                }

                


                common_ItemTipsTextPool textPool100 = new common_ItemTipsTextPool();
                textPool100.InitRefreshText(20, $"需求:{ItemBase.LevelLimit}级");
                m_TipsEffectList.Add(textPool100);
            }
            else
            {
                m_WearBtn.SetActive(false);
                for (int i = 0; i < ItemBase.ItemTipsDes.Count; i++)
                {
                    common_ItemTipsTextPool text = new common_ItemTipsTextPool();
                    text.InitRefreshText(6 + i, $"效果:{ItemBase.ItemTipsDes[i].EffectDes}");
                    m_TipsEffectList.Add(text);
                }
                //这件物品是非装备
                textPool3.InitRefreshText(5, $"种类:{ItemTypeTranslate[ItemBase.Type-1]}");
            }
            
            

            Count = m_TipsEffectList.Count;

            m_TipsEffectList.Sort((a, b) =>
            {
                return a.Index.CompareTo(b.Index);
            });


            m_ItemEffectGrid.GenerateElem(Count);

            for (int i = 0; i < Count; i++)
            {
                m_TipsEffectList[i].Init(m_ItemEffectGrid.Get(i));
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_ItemEffectRect);

        }
        public override void BeforeClose()
        {
            base.BeforeClose();
            m_TipsEffectList.Clear();
            IconLoad.Clear();
            BgLoad.Clear();
            IconLoad = null;
            BgLoad = null;
            m_ItemEffectGrid.Clear();
        }

        public void InitData(Slot a)
        {
            mSlot = a;
            m_ItemData = mSlot.itemData;
        }
        public static void ShowTip(Slot slot)
        {
            var Window=GameModule.UI.FindWindow<TipsWnd_ItemTips>();
            if (Window!=null&&Window.Visible)
            {
                //窗口不为空并且已经显示
                Window.SetRefreshInfo(slot);
            }
            else
            {
                GameModule.UI.ShowUI<TipsWnd_ItemTips,Slot>(slot);
            }
        }
    }
}
