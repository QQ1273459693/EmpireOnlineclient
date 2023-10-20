using System;
using System.Collections;
using System.Collections.Generic;
using TEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    public class common_BagEquipBox : IPoolObject
    {
        Image IconImg;
        ImageLoad IconLoad;
        public long Id { get; set ; }
        public bool IsFromPool { get ; set ; }

        GameObject m_GO;
        EventTriggerListener m_EventTriggerListener;


        //Êý¾Ý²ã
        public Slot mSlot { get; set; }
        public ItemData item;
        public long mIdx;

        public void OnInit()
        {
            
        }

        public void OnRelease()
        {

        }
        public void IntObj(GameObject OBJ)
        {
            m_GO=OBJ;
            IconImg = OBJ.transform.Find("EquipIcon").GetComponent<Image>();
            
        }
        public EventTriggerListener GetClickTipEvent()
        {
            if (m_EventTriggerListener==null)
            {
                m_EventTriggerListener = EventTriggerListener.Get(IconImg.gameObject);
            }
            return m_EventTriggerListener;
        }
        public void RefreshData(Slot slot)
        {
            if (slot==null)
            {
                IconImg.color = Color.clear;
                mSlot = null;
                return;
            }
            mSlot=slot;
            mSlot.idx = -1;
            mIdx = -10;
            item = mSlot.itemData;
            var ItemBase = ConfigLoader.Instance.Tables.TbItem1.Get(item.item.itemId);
            if (IconLoad == null)
            {               
                IconLoad = ImageLoad.Create(ItemBase.Icon, IconImg);
            }
            else
            {
                IconLoad.LoadSprite(ItemBase.Icon, IconImg);
            }
            IconImg.color = Color.white;
        }
        public void OnSpawn()
        {
            
        }

        public void OnUnspawn()
        {
            IconLoad?.Clear();
            IconLoad = null;
            m_EventTriggerListener?.RemoveUIListener();
            m_EventTriggerListener = null;
        }
    }
}
