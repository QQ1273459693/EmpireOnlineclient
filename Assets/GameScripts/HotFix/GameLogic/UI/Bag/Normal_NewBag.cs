using System;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Pool;
using GameLogic;
using DG.Tweening;
using System.Collections.Generic;

namespace TEngine
{
    [Window(UILayer.Normal)]
    class Normal_NewBag : UIWindow
    {
        GameObject mBtnExit;
        RecycleView VerticalScroll;
        

        //数据层
        List<Slot> m_BagSlotList;//背包格子数据
        Dictionary<GameObject, ItemBox> dictionaryPool = new Dictionary<GameObject,ItemBox>();



        public override void ScriptGenerator()
        {
            VerticalScroll = FindChildComponent<RecycleView>("GameObject/BagLoop");
            mBtnExit = FindChild("Top/1/Close").gameObject;
            VerticalScroll.Init(NormalCallBack);
            RegisterEventClick(mBtnExit,OnExitClick);
            
        }
        void OnExitClick(GameObject obj, PointerEventData eventData)
        {
            Close();
        }
        void RefreshBagCountNum()
        {
            m_BagSlotList = BagDataController.Instance.m_BagSlotList;
            //Log.Debug("看下长度:"+ BagGridList.Count);
            //m_BagCountText.text = $"{BagGridList.Count}/{200}"; 
        }
        private void NormalCallBack(GameObject cell, int index)
        {
            ItemBox itemPool; 
            if (!dictionaryPool.TryGetValue(cell,out itemPool))
            {
                itemPool = GameModule.ObjectPool.GetObjectPool<ItemBox>().Spawn();
                itemPool.IntObj(cell);
                itemPool.GetClickTipEvent().onClick.AddListener((go, arg) =>
                {
                    Log.Debug("点击的物品ID:"+ itemPool.item.item.itemId);
                    //HeroSpineLoad.Equip(itemPool.item.item.itemID);
                    //GameModule.UI.ShowUI<Popup_ItemTips, S_Item>(itemPool.item);
                });
                dictionaryPool.Add(cell,itemPool);
                Log.Debug("添加预制体后的字典大小:"+ dictionaryPool.Count);
            }
            itemPool.RefreshData(m_BagSlotList[index].itemData);
        }
        /// <summary>
        /// 点击穿戴回调事件
        /// </summary>
        public override void AfterShow()
        {
            base.AfterShow();
            //GameEvent.AddEventListener<int>("BagEquipWear", EquipWearEvent);
            RefreshBagCountNum();
            VerticalScroll.ShowList(m_BagSlotList.Count);
        }
        public override void BeforeClose()
        {
            base.BeforeClose();
            //GameEvent.RemoveEventListener<int>("BagEquipWear", EquipWearEvent);
            Log.Debug("退出前的字典大小是:"+ dictionaryPool.Count);
            var UnSpawnPool = GameModule.ObjectPool.GetObjectPoolByType<ItemBox>();// GameModule.ObjectPool.m_ObjectPoolManager.GetObjectPoolByType(typeof(ItemPool));
            foreach (var item in dictionaryPool.Values)
            {
                UnSpawnPool.Unspawn(item);
            }
            dictionaryPool.Clear();
            VerticalScroll.ClearGameObject();
        }
    }
}
