using BestHTTP.SecureProtocol.Org.BouncyCastle.Ocsp;
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

        public List<Slot> m_BagSlotList=new List<Slot>();


        /// <summary>
        /// 网络模块初始化。
        /// </summary>
        public override void Init()
        {
            base.Init();
            m_BagSlotList.Clear();
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
                m_BagSlotList.Clear();
                m_BagSlotList.AddRange(Response.slot);
                Log.Info($"获取背包数据成功!长度是:{m_BagSlotList.Count}");
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
                Log.Info("背包数据,枚举是:" + message.UpdateType);
                switch (message.UpdateType)
                {
                    case 0://添加
                        Instance.m_BagSlotList.AddRange(message.info);
                        break;
                    case 1://删除
                        for (int i = 0; i < message.info.Count; i++)
                        {
                            var UpdateSlot = message.info[i].idx;
                            for (int j = Instance.m_BagSlotList.Count - 1; j >= 0; j--)
                            {
                                if (Instance.m_BagSlotList[j].idx == UpdateSlot)
                                {
                                    Instance.m_BagSlotList.RemoveAt(j);
                                    break;
                                }
                            }
                        }
                        break;
                    case 2://变更
                        int BagCount = Instance.m_BagSlotList.Count;
                        for (int i = 0; i < message.info.Count; i++)
                        {
                            var UpdateSlot = message.info[i];
                            for (int j = 0; j < BagCount; j++)
                            {
                                if (Instance.m_BagSlotList[j].idx== UpdateSlot.idx)
                                {
                                    Instance.m_BagSlotList[j] = UpdateSlot;
                                    break;
                                }
                            }
                        }
                        break;
                }
                GameEvent.Send(BagWndEvent.UpdateBagSlotEvent.EventId);
                await FTask.CompletedTask;
            }
        }
    }
}