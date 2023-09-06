using System;
using System.Collections;
using System.Collections.Generic;
using TEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    public class ItemBox : IPoolObject
    {
        Image m_ItemBG;
        Image IconImg;
        ImageLoad IconLoad;
        ImageLoad BgLoad;
        public long Id { get; set ; }
        public bool IsFromPool { get ; set ; }

        GameObject m_GO;
        Text m_NumText;
        EventTriggerListener m_EventTriggerListener;


        //数据层
        public ItemData item;

        public void OnInit()
        {
            
        }

        public void OnRelease()
        {

        }
        public void IntObj(GameObject OBJ)
        {
            m_GO=OBJ;
            m_ItemBG = OBJ.GetComponent<Image>();
            IconImg = OBJ.transform.Find("ItemIcon").GetComponent<Image>();
            m_NumText = OBJ.transform.Find("NumText").GetComponent<Text>();
        }
        public EventTriggerListener GetClickTipEvent()
        {
            if (m_EventTriggerListener==null)
            {
                m_EventTriggerListener = EventTriggerListener.Get(m_GO);
            }
            return m_EventTriggerListener;
        }
        public void RefreshData(ItemData itemData)
        {
            item=itemData;
            var ItemBase = ConfigLoader.Instance.Tables.TbItem1.Get(itemData.item.itemId);

            var ItemBgBase = ConfigLoader.Instance.Tables.TbItemBgGround1.Get(ItemBase.Quality);
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
                BgLoad = ImageLoad.Create(ItemBgBase.ItemBoxLine, m_ItemBG);
            }
            else
            {
                BgLoad.LoadSprite(ItemBgBase.ItemBoxLine, m_ItemBG);
            }
            m_NumText.text = itemData.count.ToString();
        }
        public void OnSpawn()
        {
            
        }

        public void OnUnspawn()
        {
            IconLoad.Clear();
            BgLoad.Clear();
            IconLoad = null;
            BgLoad = null;
            m_EventTriggerListener?.RemoveUIListener();
            m_EventTriggerListener = null;
            Log.Debug("正在退出对象池");
        }
    }
}
