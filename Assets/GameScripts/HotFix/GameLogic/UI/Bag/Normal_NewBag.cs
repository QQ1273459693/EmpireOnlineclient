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
        Dictionary<GameObject,ItemBox> dictionaryPool = new Dictionary<GameObject,ItemBox>();
        long m_CurSlotIdx=-1;//当前选择的格子ID


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
                    if (m_CurSlotIdx!= itemPool.mIdx)
                    {
                        m_CurSlotIdx= itemPool.mIdx;
                        TipsWnd_ItemTips.ShowTip(itemPool.item);
                        Log.Debug("点击的物品ID:" + itemPool.item.item.itemId);
                    }
                    else
                    {
                        Log.Info("已经点击了");
                    }
                    
                });
                dictionaryPool.Add(cell,itemPool);
                Log.Debug("添加预制体后的字典大小:"+ dictionaryPool.Count);
            }
            itemPool.RefreshData(m_BagSlotList[index].itemData, m_BagSlotList[index].idx);
        }
        /// <summary>
        /// 点击穿戴回调事件
        /// </summary>
        public override void AfterShow()
        {
            base.AfterShow();
            m_CurSlotIdx = -1;
            //GameEvent.AddEventListener<int>("BagEquipWear", EquipWearEvent);
            RefreshBagCountNum();
            Log.Info("背包长度是:"+ m_BagSlotList.Count);
            VerticalScroll.ShowList(m_BagSlotList.Count);
        }
        public override void BeforeClose()
        {
            base.BeforeClose();
            //GameEvent.RemoveEventListener<int>("BagEquipWear", EquipWearEvent);
            Log.Debug("退出前的字典大小是:"+ dictionaryPool.Count);
           // var UnSpawnPool = GameModule.ObjectPool.GetObjectPoolByType<ItemBox>();// GameModule.ObjectPool.m_ObjectPoolManager.GetObjectPoolByType(typeof(ItemPool));
            foreach (var item in dictionaryPool.Values)
            {
                GameModule.ObjectPool.GetObjectPoolByType<ItemBox>().Unspawn(item);
                //UnSpawnPool.Unspawn(item);
            }
            dictionaryPool.Clear();
            VerticalScroll.ClearGameObject();
        }
    }
}
