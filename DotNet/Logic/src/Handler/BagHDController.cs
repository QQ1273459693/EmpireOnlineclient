#if TENGINE_NET
using TEngine.Core.Network;
using TEngine.Core;
using TEngine.Core.DataBase;
using Amazon.Runtime.Internal;
using System.Security.Cryptography;
using Amazon.Runtime.Internal.Util;
using System;
using MongoDB.Driver.Core.Servers;
using static TEngine.Logic.LoginHDController;
using static System.Collections.Specialized.BitVector32;

namespace TEngine.Logic;

public class BagHDController
{
    /// <summary>
    /// 背包信息协议
    /// </summary>
    public class BagInfo_MessageHandler : MessageRPC<C2L_BagInfo,L2C_BagInfo>
    {
        protected override async FTask Run(Session session, C2L_BagInfo req, L2C_BagInfo rsp, Action reply)
        {
            //session.id
            //AddressManage.Get
            Log.Debug($"接收到请求角色背包信息:{req.ToJson()}");
            Unit unit = AccountManage.Get(session.Id)!;
            if (unit==null)
            {
                Log.Error($"错误!链接:{session.Id}没有玩家信息!");
                return;
            }



            var m_server = Server.Get(5120);
            IDateBase db = m_server.Scene.World.DateBase;
            List<BagModelInfo> result = await db.Query<BagModelInfo>(t => t.UID == unit.UID);
            rsp.slot = new List<Slot>();
            if (result.Count > 0)
            {
                Log.Debug("服务器有相同的背包数据");
                
                var Slot = result[0].Slot;
                rsp.slot.AddRange(Slot);
                reply();
                return;
            }
            Log.Debug("背包没有这个玩家的背包信息,创建下!");
            BagModelInfo BagInfo = Entity.Create<BagModelInfo>(session.Scene);
            BagInfo.UID= unit.UID;
            var List=ConfigLoader.Instance.Tables.TbItem1.DataList;

            int Index = 1;


            for (int i = 29; i >= 0; i--)
            {
                Slot QQ = new Slot();
                QQ.idx = i;
                bool isEquip = List[i].Type == 4;
                QQ.itemData = new ItemData();
                QQ.itemData.item = new Item();
                QQ.itemData.item.itemId = List[i].Id;
                if (isEquip)
                {
                    //是装备
                    QQ.itemData.item.equipData = new EquipData();
                    Random ran = new Random();
                    QQ.itemData.item.equipData.GemAttributeID = ran.Next(0,10);
                    QQ.itemData.item.equipData.slv = 4;
                    QQ.itemData.count = 1;
                }
                else
                {
                    QQ.itemData.count = Index;
                }
                BagInfo.Slot.Add(QQ);
                Index++;
            }


            await db.Save(BagInfo);

            rsp.slot.AddRange(BagInfo.Slot);
            reply();

            await FTask.CompletedTask;
        }
    }

    /// <summary>
    /// 装备穿戴协议
    /// </summary>
    public class EquipWear_MessageHandler : MessageRPC<C2L_EquipWear, L2C_EquipWear>
    {
        protected override async FTask Run(Session session, C2L_EquipWear req, L2C_EquipWear rsp, Action reply)
        {
            //session.id
            //AddressManage.Get
            Log.Debug($"接收到请求穿戴装备协议信息:{req.ToJson()}");
            Unit unit = AccountManage.Get(session.Id)!;
            if (unit == null)
            {
                Log.Error($"错误!链接:{session.Id}没有玩家信息!");
                return;
            }



            var m_server = Server.Get(5120);
            IDateBase db = m_server.Scene.World.DateBase;
            List<BagModelInfo> Bagresult = await db.Query<BagModelInfo>(t => t.UID == unit.UID);
            Log.Debug("打开前背包数据ID:" + Bagresult[0].Id);
            List<CharacterInfo> CharSlotResult = await db.Query<CharacterInfo>(t => t.UID == unit.UID);
            if (CharSlotResult.Count==0)
            {
                Log.Error("错误!,没有找到用户ID的角色属性:"+ unit.UID);
                return;
            }
            var CharResult = CharSlotResult[0];
            var CharSlot = CharResult.CharEquipSlots;
            if (req.OnWear)
            {
                //穿戴,找寻背包装备
                
                if (Bagresult.Count > 0)
                {
                    Log.Debug("服务器有背包数据");
                    var mSlot = Bagresult[0].Slot;
                    for (int i = 0; i < mSlot.Count; i++)
                    {
                        if (mSlot[i].idx==req.idx)
                        {
                            Slot slot = mSlot[i];
                            //找到对应的装备穿戴
                            for (int j = 0; j < CharSlot.Count; j++)
                            {
                                if (CharSlot[j].Pos==req.WearIdx)
                                {
                                    //找到穿戴对应位置
                                    CharSlot[j].slot = slot;
                                    Bagresult[0].Slot.Remove(slot);
                                    Log.Debug("打开后背包数据ID:" + Bagresult[0].Id);
                                    await db.Save(Bagresult[0]);
                                    await db.Save(CharResult);
                                    SendUpdateBagSlot(session,1,slot);
                                    UpdateSendCharEquipSlot(session, CharSlot);
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    reply();
                    return;
                }
            }
            else
            {
                //卸载,找寻对应装备位置
                
                if (CharSlotResult.Count > 0)
                {
                    Log.Debug("服务器有背包数据");
                    
                    for (int i = 0; i < CharSlot.Count; i++)
                    {
                        if (CharSlot[i].Pos== req.WearIdx)
                        {
                            if (CharSlot[i].slot!=null)
                            {
                                Slot slot = CharSlot[i].slot;
                                slot.idx = Bagresult[0].Slot.Count+1;
                                Bagresult[0].Slot.Add(slot);
                                CharSlot[i].slot = null!;
                                Log.Debug("打开后背包数据ID:" + Bagresult[0].Id);
                                await db.Save(Bagresult[0]);
                                await db.Save(CharResult);
                                SendUpdateBagSlot(session,0,slot);
                                UpdateSendCharEquipSlot(session, CharSlot);
                            }
                            else
                            {
                                rsp.ErrorCode = 1001;
                            }
                            
                            break;
                        }
                    }
                    reply();
                    return;
                }
            }
            

            await FTask.CompletedTask;
        }
        /// <summary>
        /// 刷新角色装备栏并发送更新协议
        /// </summary>
        void UpdateSendCharEquipSlot(Session session,List<CharEquipSlotData> datas)
        {
            L2C_PlayerNotifyUpdate l2C_ = new L2C_PlayerNotifyUpdate();
            CharacterData data = new CharacterData();
            data.EquipslotDat = datas;
            l2C_.characterData = data;
            l2C_.UpdateCase = 8;
            session.Send(l2C_);
        }
        /// <summary>
        /// 根据枚举值更新背包信息--更新类型,0:添加,1:删除,2:变更
        /// </summary>
        /// <param name="session"></param>
        /// <param name="datas"></param>
        void SendUpdateBagSlot(Session session, int UpdateType,List<Slot> UpdateSlot)
        {
            L2C_BagUpdate l2C_ = new L2C_BagUpdate();
            l2C_.UpdateType= UpdateType;
            l2C_.info = new List<Slot>();
            l2C_.info.AddRange(UpdateSlot);
            session.Send(l2C_);
        }
        void SendUpdateBagSlot(Session session, int UpdateType,Slot UpdateSlot)
        {
            L2C_BagUpdate l2C_ = new L2C_BagUpdate();
            l2C_.UpdateType = UpdateType;
            l2C_.info = new List<Slot>();
            l2C_.info.Add(UpdateSlot);
            session.Send(l2C_);
        }
    }
}
#endif
