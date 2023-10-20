using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using TEngine;
using TEngine.Core;
using TEngine.Core.Network;
using TEngine.Demo;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 玩家信息网络模块。
    /// </summary>
    public class BagDataController : DataCenterModule<BagDataController>
    {

        public Dictionary<long,Slot> m_BagSlotDict=new Dictionary<long,Slot>();
        public List<Slot> m_SortBagSlotList = new List<Slot>();//排序后的List

        /// <summary>
        /// 网络模块初始化。
        /// </summary>
        public override void Init()
        {
            base.Init();
            m_BagSlotDict.Clear();
            m_SortBagSlotList.Clear();
        }
        /// <summary>
        /// 请求背包上行
        /// </summary>
        public async void ReqGetBagInfo()
        {
            C2L_BagInfo Request = new C2L_BagInfo();

            var Response = (L2C_BagInfo)await GameClient.Instance.Call(Request);

            if (Response.ErrorCode == 0)
            {
                m_BagSlotDict.Clear();
                m_SortBagSlotList.Clear();
                for (int i = 0; i < Response.slot.Count; i++)
                {
                    var Slot = Response.slot[i];
                    if (!m_BagSlotDict.TryAdd(Slot.idx, Slot))
                    {
                        Log.Error("服务器有相同的物品索引!");
                    }
                    
                }
                m_SortBagSlotList.AddRange(m_BagSlotDict.Values);
                Log.Info($"获取背包数据成功!长度是:{m_BagSlotDict.Count}");
            }
            else
            {
                Log.Info("获取背包数据失败:" + Response.ErrorCode);
            }
            GameModule.UI.ShowUI<Normal_PlayerVisitWindow>();

        }
        /// <summary>
        /// 穿戴卸载装备上行
        /// </summary>
        public async void ReqEquipWear(bool OnWear,int Pos,long SlotIdx=0)
        {
            C2L_EquipWear Request = new C2L_EquipWear();
            Request.OnWear = OnWear;
            Request.WearIdx= Pos;
            Request.idx = SlotIdx;
            Log.Info($"卸载或者穿戴的ID是:{SlotIdx}");
            var Response = (L2C_EquipWear)await GameClient.Instance.Call(Request);

            if (Response.ErrorCode == 0)
            {
                Log.Info($"装备穿戴卸载成功!");
            }
            else
            {
                Log.Info("装备穿戴卸载失败:" + Response.ErrorCode);
            }
        }
        /// <summary>
        /// 背包格子更新,更新类型,0:添加,1:删除,2:变更
        /// </summary>
        public class OnBagSlotUpdate_Handler : Message<L2C_BagUpdate>
        {
            protected override async FTask Run(Session session, L2C_BagUpdate message)
            {
                for (int i = 0; i < message.info.Count; i++)
                {
                    var UpdateSlot = message.info[i];
                    //三种情况,物品ID没有则为新增,如果有还要判断Count==0则为删除,否则则为变更
                    if (UpdateSlot.itemData.count==0)
                    {
                        //删除
                        Log.Info("删除!--------------");
                        Instance.m_BagSlotDict.Remove(UpdateSlot.idx);
                        for (int j = 0; j < Instance.m_SortBagSlotList.Count; j++)
                        {
                            if (Instance.m_SortBagSlotList[j].idx== UpdateSlot.idx)
                            {
                                Instance.m_SortBagSlotList.RemoveAt(j);
                                break;
                            }
                        }
                    }
                    else if (Instance.m_BagSlotDict.TryAdd(UpdateSlot.idx, UpdateSlot))
                    {
                        Log.Info("新增!--------------");
                        //新增成功
                        Instance.m_SortBagSlotList.Add(UpdateSlot);
                    }
                    else
                    {
                        Log.Info("修改物品属性!--------------");
                        //没有添加新的索引,说明已经有只是修改属性数量
                        Instance.m_BagSlotDict[UpdateSlot.idx] = UpdateSlot;
                        for (int j = 0; j < Instance.m_SortBagSlotList.Count; j++)
                        {
                            if (Instance.m_SortBagSlotList[j].idx == UpdateSlot.idx)
                            {
                                Instance.m_SortBagSlotList[j] = UpdateSlot;
                                break;
                            }
                        }
                    }
                }



                Log.Info($"更新完背部后的数据大小:"+ Instance.m_BagSlotDict.Count);

                GameEvent.Send(BagWndEvent.UpdateBagSlotEvent.EventId);
                await FTask.CompletedTask;
            }
        }
    }
}