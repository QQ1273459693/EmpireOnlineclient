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
using TEngine.Core;

namespace TEngine
{
    class Normal_BagViewData
    {
        GameObject mBtnExit;
        RecycleView VerticalScroll;
        

        //数据层
        List<Slot> m_BagSlotList;//背包格子数据
        Dictionary<GameObject,ItemBox> dictionaryPool = new Dictionary<GameObject,ItemBox>();//背包数据
        Dictionary<int, common_BagEquipBox> m_BagEquipBoxList = new Dictionary<int, common_BagEquipBox>();//装备栏专用
        List<GameObject> m_EquipBoxObjList=new List<GameObject>();//装备栏物体


        List<CharEquipSlotData> m_CharEquipSlot;//角色身上的装备栏槽属性


        long m_CurSlotIdx=-99;//当前选择的格子ID


        public void Init(GameObject obj)
        {
            Transform LeftEquipBox = obj.transform.Find("Mid/1/GameObject/Left");
            Transform RightEquipBox = obj.transform.Find("Mid/1/GameObject/Right");

            for (int i = 0; i < LeftEquipBox.childCount; i++)
            {
                m_EquipBoxObjList.Add(LeftEquipBox.GetChild(i).gameObject);
            }
            for (int i = 0; i < RightEquipBox.childCount; i++)
            {
                m_EquipBoxObjList.Add(RightEquipBox.GetChild(i).gameObject);
            }


            VerticalScroll = obj.transform.Find("GameObject/BagLoop").GetComponent<RecycleView>();
            mBtnExit = obj.transform.Find("Top/1/Close").gameObject;
            VerticalScroll.Init(NormalCallBack);
            //EventTriggerListener.Get(mBtnExit).onClick.AddListener(OnExitClick);
            
        }
        //void OnExitClick(GameObject obj, PointerEventData eventData)
        //{
        //    Close();
        //}
        void RefreshBagCountNum()
        {
            m_BagSlotList = BagDataController.Instance.m_SortBagSlotList;
            Log.Debug("看下长度:"+ m_BagSlotList.Count);
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
                        TipsWnd_ItemTips.ShowTip(itemPool.mSlot);
                        m_CurSlotIdx = -1;
                        Log.Debug("点击的物品ID:" + itemPool.item.item.itemId);
                    }
                    else
                    {
                        Log.Info("已经点击了");
                    }
                    
                });
                dictionaryPool.Add(cell,itemPool);
                //Log.Debug("添加预制体后的字典大小:"+ dictionaryPool.Count);
            }
            itemPool.RefreshData(m_BagSlotList[index]);
        }
        /// <summary>
        /// 刷新装备栏
        /// </summary>
        void RefreshEquipBoxSlot()
        {
            for (int i = 0; i < m_EquipBoxObjList.Count; i++)
            {
                common_BagEquipBox itemPool= GameModule.ObjectPool.GetObjectPool<common_BagEquipBox>().Spawn();
                itemPool.IntObj(m_EquipBoxObjList[i]);
                itemPool.GetClickTipEvent().onClick.AddListener((go, arg) =>
                {
                    Log.Debug("没点击到?:");
                    if (itemPool.mSlot!=null&& itemPool.mIdx<0)
                    {
                        TipsWnd_ItemTips.ShowTip(itemPool.mSlot);
                    }
                    
                });
                m_BagEquipBoxList.Add(i, itemPool);
            }
            foreach (var item in m_BagEquipBoxList)
            {
                for (int i = 0; i < m_CharEquipSlot.Count; i++)
                {
                    if (item.Key== m_CharEquipSlot[i].Pos)
                    {
                        //发现相同槽位置
                        item.Value.RefreshData(m_CharEquipSlot[i].slot);
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// 背包更新
        /// </summary>
        private void UpdateBagSlot()
        {
            RefreshBagCountNum();
            VerticalScroll.ShowList(m_BagSlotList.Count);
            Log.Info("没更新背包?");
        }
        /// <summary>
        /// 装备栏更新
        /// </summary>
        private void UpdateEquipSlot()
        {
            Log.Info("装备栏没更新?");
            m_CharEquipSlot = GameDataController.Instance.m_CharacterData.EquipslotDat;
            foreach (var item in m_BagEquipBoxList)
            {
                for (int i = 0; i < m_CharEquipSlot.Count; i++)
                {
                    if (item.Key == m_CharEquipSlot[i].Pos)
                    {
                        //发现相同槽位置
                        item.Value.RefreshData(m_CharEquipSlot[i].slot);
                        break;
                    }
                }
            }
        }
        public void AfterShow()
        {
            GameEvent.AddEventListener(BagWndEvent.UpdateBagSlotEvent.EventId, UpdateBagSlot);
            GameEvent.AddEventListener(PlayerDataUpdateWndEvent.UpdateEquipSlot.EventId, UpdateEquipSlot);
            m_CurSlotIdx = -1;
            m_CharEquipSlot = GameDataController.Instance.m_CharacterData.EquipslotDat;
            //GameEvent.AddEventListener<int>("BagEquipWear", EquipWearEvent);
            RefreshBagCountNum();
            RefreshEquipBoxSlot();
            //Log.Info("背包长度是:"+ m_BagSlotList.Count);
            VerticalScroll.ShowList(m_BagSlotList.Count);

            var Base = ConfigLoader.Instance.Tables.TbSwordSkillBase.DataList[0];

            Log.Info($"技能名大小:{Base.Name.Count},描述大小:{Base.Des.Count}");
            for (int i = 0; i < Base.Name.Count; i++)
            {
                Log.Info($"技能名:{Base.Name[i]},技能效果:{Base.Des[i]}");
            }


        }
        public  void BeforeClose()
        {
            GameEvent.RemoveEventListener(BagWndEvent.UpdateBagSlotEvent.EventId, UpdateBagSlot);
            GameEvent.RemoveEventListener(PlayerDataUpdateWndEvent.UpdateEquipSlot.EventId, UpdateEquipSlot);
            //GameEvent.RemoveEventListener<int>("BagEquipWear", EquipWearEvent);
            //Log.Debug("退出前的字典大小是:"+ dictionaryPool.Count);
            // var UnSpawnPool = GameModule.ObjectPool.GetObjectPoolByType<ItemBox>();// GameModule.ObjectPool.m_ObjectPoolManager.GetObjectPoolByType(typeof(ItemPool));
            foreach (var item in dictionaryPool.Values)
            {
                GameModule.ObjectPool.GetObjectPoolByType<ItemBox>().Unspawn(item);
                //UnSpawnPool.Unspawn(item);
            }
            foreach (var item in m_BagEquipBoxList.Values)
            {
                GameModule.ObjectPool.GetObjectPoolByType<common_BagEquipBox>().Unspawn(item);
            }
            m_BagEquipBoxList.Clear();

            dictionaryPool.Clear();
            VerticalScroll.ClearGameObject();
        }
    }
}
