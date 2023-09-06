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

namespace TEngine.Logic;

public class BagHDController
{

    public class BagInfo_MessageHandler : MessageRPC<C2L_BagInfo,L2C_BagInfo>
    {
        protected override async FTask Run(Session session, C2L_BagInfo req, L2C_BagInfo rsp, Action reply)
        {
            //session.id
            //AddressManage.Get
            Log.Debug($"接收到请求角色背包信息:{req.ToJson()}");
            Unit unit = AccountManage.Get(session.Id);
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
            for (int i = 0; i < 26; i++)
            {
                Slot QQ = new Slot();
                QQ.idx = i;
                QQ.itemData = new ItemData();
                QQ.itemData.item = new Item();
                QQ.itemData.item.equipData = new EquipData();
                QQ.itemData.item.equipData.slv = 5;
                QQ.itemData.count = Index;
                QQ.itemData.item.itemId = List[i].Id;
                BagInfo.Slot.Add(QQ);
                Index++;
            }
            for (int i = 0; i < 26; i++)
            {
                Slot QQ = new Slot();
                QQ.idx = i;
                QQ.itemData = new ItemData();
                QQ.itemData.item = new Item();
                QQ.itemData.item.equipData = new EquipData();
                QQ.itemData.item.equipData.slv = 5;
                QQ.itemData.count = Index;
                QQ.itemData.item.itemId = List[i].Id;
                BagInfo.Slot.Add(QQ);
                Index++;
            }
            for (int i = 0; i < 26; i++)
            {
                Slot QQ = new Slot();
                QQ.idx = i;
                QQ.itemData = new ItemData();
                QQ.itemData.item = new Item();
                QQ.itemData.item.equipData = new EquipData();
                QQ.itemData.item.equipData.slv = 5;
                QQ.itemData.count = Index;
                QQ.itemData.item.itemId = List[i].Id;
                BagInfo.Slot.Add(QQ);
                Index++;
            }
            for (int i = 0; i < 26; i++)
            {
                Slot QQ = new Slot();
                QQ.idx = i;
                QQ.itemData = new ItemData();
                QQ.itemData.item = new Item();
                QQ.itemData.item.equipData = new EquipData();
                QQ.itemData.item.equipData.slv = 5;
                QQ.itemData.count = Index;
                QQ.itemData.item.itemId = List[i].Id;
                BagInfo.Slot.Add(QQ);
                Index++;
            }
            for (int i = 0; i < 26; i++)
            {
                Slot QQ = new Slot();
                QQ.idx = i;
                QQ.itemData = new ItemData();
                QQ.itemData.item = new Item();
                QQ.itemData.item.equipData = new EquipData();
                QQ.itemData.item.equipData.slv = 5;
                QQ.itemData.count = Index;
                QQ.itemData.item.itemId = List[i].Id;
                BagInfo.Slot.Add(QQ);
                Index++;
            }
            for (int i = 0; i < 26; i++)
            {
                Slot QQ = new Slot();
                QQ.idx = i;
                QQ.itemData = new ItemData();
                QQ.itemData.item = new Item();
                QQ.itemData.item.equipData = new EquipData();
                QQ.itemData.item.equipData.slv = 5;
                QQ.itemData.count = Index;
                QQ.itemData.item.itemId = List[i].Id;
                BagInfo.Slot.Add(QQ);
                Index++;
            }
            for (int i = 0; i < 26; i++)
            {
                Slot QQ = new Slot();
                QQ.idx = i;
                QQ.itemData = new ItemData();
                QQ.itemData.item = new Item();
                QQ.itemData.item.equipData = new EquipData();
                QQ.itemData.item.equipData.slv = 5;
                QQ.itemData.count = Index;
                QQ.itemData.item.itemId = List[i].Id;
                BagInfo.Slot.Add(QQ);
                Index++;
            }
            for (int i = 0; i < 26; i++)
            {
                Slot QQ = new Slot();
                QQ.idx = i;
                QQ.itemData = new ItemData();
                QQ.itemData.item = new Item();
                QQ.itemData.item.equipData = new EquipData();
                QQ.itemData.item.equipData.slv = 5;
                QQ.itemData.count = Index;
                QQ.itemData.item.itemId = List[i].Id;
                BagInfo.Slot.Add(QQ);
                Index++;
            }

            await db.Save(BagInfo);

            rsp.slot.AddRange(BagInfo.Slot);
            reply();

            await FTask.CompletedTask;
        }
    }
}
#endif
