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
    class Normal_Bag : UIWindow
    {
        GameObject mBtnExit;
        RecycleView VerticalScroll;
        Dictionary<GameObject, ItemPool> dictionaryPool = new Dictionary<GameObject, ItemPool>();
        List<S_Item> BagGridList;
        TMP_Text m_BagCountText;//背包数量文本
        GameObject m_LastSelectOBJ;//上次选择的物品
        GameObject mHeroSpineObj;
        //数据层
        Vector2 m_SelectMove = new Vector2(-2.7F,2.7F);
        const float m_SlectX = -2.7F;
        const float m_SlectY = 2.7F;
        UnitUiLoader HeroSpineLoad;
        int m_LastClickItemID;//上次点击的物品

        


        public override void ScriptGenerator()
        {
            VerticalScroll = FindChildComponent<RecycleView>("Equipment/Right_Panel/ScrollRect");
            mBtnExit = FindChild("Equipment/TopBar/Button_Back").gameObject;
            m_BagCountText = FindChildComponent<TMP_Text>("Equipment/Right_Bottom/Count/Text (TMP)");
            mHeroSpineObj = FindChild("Equipment/Left_Panel/Character/CharacterBgBottom/Character/SkeletonGraphic (Character)").gameObject;
            VerticalScroll.Init(NormalCallBack);
            BagGridList = L_BagSystemDate.Instance.GetCurCharacterBagList();
            RegisterEventClick(mBtnExit,OnExitClick);
            
        }
        void OnExitClick(GameObject obj, PointerEventData eventData)
        {
            Close();
        }
        void RefreshBagCountNum()
        {
            if (BagGridList==null)
            {
                Log.Debug("是空的报错");
            }
            //Log.Debug("看下长度:"+ BagGridList.Count);
            //m_BagCountText.text = $"{BagGridList.Count}/{200}"; 
        }
        private void NormalCallBack(GameObject cell, int index)
        {
            ItemPool itemPool; 
            if (!dictionaryPool.TryGetValue(cell,out itemPool))
            {
                itemPool = GameModule.ObjectPool.GetObjectPool<ItemPool>().Spawn(); //GameModule.ObjectPool.m_ObjectPoolManager.GetObjectPool<ItemPool>().Spawn();
                itemPool.IntObj(cell);
                itemPool.GetClickTipEvent().onClick.AddListener((go, arg) =>
                {
                    m_LastClickItemID = itemPool.item.item.itemID;
                    Log.Debug("点击的物品ID:"+ itemPool.item.item.itemID);
                    //HeroSpineLoad.Equip(itemPool.item.item.itemID);
                    //物品点击抬起
                    if (m_LastSelectOBJ!=null)
                    {
                        m_LastSelectOBJ.SetActive(false);
                    }
                    m_LastSelectOBJ = itemPool.SelectOBJState(true);
                    Log.Debug("传送进去的:"+ itemPool.item.item.itemID);
                    GameModule.UI.ShowUI<Popup_ItemTips, S_Item>(itemPool.item);
                });
                dictionaryPool.Add(cell,itemPool);
                Log.Debug("添加预制体后的字典大小:"+ dictionaryPool.Count);
            }
            itemPool.RefreshData(BagGridList[index]);
            itemPool.RefreshSelectOBJState(m_LastClickItemID);
        }
        /// <summary>
        /// 点击穿戴回调事件
        /// </summary>
        void EquipWearEvent(int EquipID)
        {
            HeroSpineLoad.Equip(EquipID);
        }
        public override void AfterShow()
        {
            base.AfterShow();
            GameEvent.AddEventListener<int>("BagEquipWear", EquipWearEvent);
            HeroSpineLoad = ReferencePool.Acquire<UnitUiLoader>();
            HeroSpineLoad.Load(mHeroSpineObj);
            RefreshBagCountNum();
            VerticalScroll.ShowList(BagGridList.Count);
        }
        public override void BeforeClose()
        {
            base.BeforeClose();
            GameEvent.RemoveEventListener<int>("BagEquipWear", EquipWearEvent);
            Log.Debug("退出前的字典大小是:"+ dictionaryPool.Count);
            var UnSpawnPool = GameModule.ObjectPool.GetObjectPoolByType<ItemPool>();// GameModule.ObjectPool.m_ObjectPoolManager.GetObjectPoolByType(typeof(ItemPool));
            foreach (var item in dictionaryPool.Values)
            {
                UnSpawnPool.Unspawn(item);
            }
            dictionaryPool.Clear();
            VerticalScroll.ClearGameObject();
            HeroSpineLoad = null;
            //HeroSpineLoad.Dispose();
        }
    }
}
