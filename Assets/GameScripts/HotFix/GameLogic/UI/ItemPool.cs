using System;
using System.Collections;
using System.Collections.Generic;
using TEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    public class ItemPool : IPoolObject
    {
        Image m_IconColorImg;
        Image m_IconBGImg;
        Image IconImg;
        ImageLoad imageLoad;
        ImageLoad IconBgimageLoad;
        public long Id { get; set ; }
        public bool IsFromPool { get ; set ; }

        public RectTransform obj;
        GameObject m_SelectObj;
        TMP_Text text;
        public S_Item item;
        EventTriggerListener m_EventTriggerListener;

        public void OnInit()
        {
            
        }

        public void OnRelease()
        {

        }
        public void IntObj(GameObject OBJ)
        {
            obj = OBJ.GetComponent<RectTransform>();
            m_SelectObj = OBJ.transform.Find("SlectObj").gameObject;
            m_IconColorImg = OBJ.GetComponent<Image>();
            m_IconBGImg = OBJ.transform.Find("GradeFrame").GetComponent<Image>();
            IconImg = OBJ.transform.Find("GradeFrame/ItemIcon").GetComponent<Image>();
            text = OBJ.transform.Find("Text (TMP)").GetComponent<TMP_Text>();
        }
        public EventTriggerListener GetClickTipEvent()
        {
            if (m_EventTriggerListener==null)
            {
                m_EventTriggerListener = EventTriggerListener.Get(obj.gameObject);
            }
            return m_EventTriggerListener;
        }
        public GameObject SelectOBJState(bool isShow)
        {
             m_SelectObj.SetActive(isShow);
            return m_SelectObj;
        }
        public void RefreshSelectOBJState(int ItemID)
        {
            m_SelectObj.SetActive(ItemID== item.item.itemID);
        }
        public void RefreshData(S_Item Item)
        {
            item = Item;
            var ItemBase = ConfigLoader.Instance.Tables.TbItem.Get(Item.item.itemID);
            var ItemBgBase = ConfigLoader.Instance.Tables.TbItemBgGround.Get(ItemBase.Quality);//////
            if (imageLoad==null)
            {               
                imageLoad = ImageLoad.Create(ItemBase.Icon, IconImg);
            }
            else
            {
                imageLoad.LoadSprite(ItemBase.Icon, IconImg);
            }
            if (IconBgimageLoad == null)
            {
                IconBgimageLoad = ImageLoad.Create(ItemBgBase.ItemBoxLine, m_IconBGImg);
            }
            else
            {
                IconBgimageLoad.LoadSprite(ItemBgBase.ItemBoxLine, m_IconBGImg);
            }
            Color color;
            ColorUtility.TryParseHtmlString(ItemBgBase.ItemColor, out color);
            m_IconColorImg.color = color;
            text.text = Item.item.count.ToString();
        }
        public void OnSpawn()
        {
            
        }

        public void OnUnspawn()
        {
            IconBgimageLoad.Clear();
            imageLoad.Clear();
            IconBgimageLoad = null;
            imageLoad = null;
            m_EventTriggerListener?.RemoveUIListener();
            m_EventTriggerListener = null;
            Log.Debug("正在退出对象池");
        }
    }
}
