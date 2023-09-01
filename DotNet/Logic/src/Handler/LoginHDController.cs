#if TENGINE_NET
using TEngine.Core.Network;
using TEngine.Core;
using TEngine.Core.DataBase;
using Amazon.Runtime.Internal;
using System.Security.Cryptography;
using Amazon.Runtime.Internal.Util;
using System;
using MongoDB.Driver.Core.Servers;

namespace TEngine.Logic;

public class LoginHDController
{

    public class CreateRole_MessageHandler : MessageRPC<C2L_CreateRole,L2C_CreateRole>
    {
        protected override async FTask Run(Session session, C2L_CreateRole req, L2C_CreateRole rsp, Action reply)
        {
            Log.Debug($"接收到请求创建角色信息:{req.ToJson()}");

            var m_server = Server.Get(5120);
            IDateBase db = m_server.Scene.World.DateBase;
            List<AccountInfo> result = await db.Query<AccountInfo>(t => t.UserName == req.UserName); await db.Query<AccountInfo>(t => t.SDKUID == req.SDKUID);
            if (result.Count>0)
            {
                Log.Debug("服务器有相同的账号和SDK,创建失败!");
                rsp.ErrorCode = ErrorCode.ERR_AccountAlreadyRegisted;
                reply();
                return;
            }
            Log.Debug("数据库并没有相同的账号信息,可以创建!");
            uint uid = await GeneratorUID(db);

            AccountInfo accountInfo = Entity.Create<AccountInfo>(session.Scene);
            accountInfo.UserName = req.UserName;
            accountInfo.Password = req.Password;
            accountInfo.SDKUID = req.SDKUID;
            accountInfo.UID = uid;
            db.Save(accountInfo);

            await FTask.CompletedTask;
        }
        public async FTask<uint> GeneratorUID(IDateBase db)
        {
            var ret = await db.Last<AccountInfo>(t => t.UID != 0);
            if (ret == null)
            {
                return 100000;
            }
            return ret.UID + 1;
        }
    }
    public class Login_MessageHandler : MessageRPC<C2L_Login, L2C_Login>
    {
        protected override async FTask Run(Session session, C2L_Login request, L2C_Login response, Action reply)
        {
            var m_server = Server.Get(5120);
            IDateBase db = m_server.Scene.World.DateBase;
            List<AccountInfo> result = await db.Query<AccountInfo>(t => t.UserName == request.UserName); await db.Query<AccountInfo>(t => t.Password == request.Password);
            if (result.Count == 0)
            {
                Log.Debug("服务器没有这个账号!");
                response.ErrorCode = ErrorCode.ERR_AccountAlreadyRegisted;
                reply();
                return;
            }
            Log.Debug($"玩家名:{request.UserName},登陆成功!");

            //分割
            //List<CharacterInfo> result1 = await db.Query<CharacterInfo>(t => t.UID == response.UID);
            //CharacterInfo Info;
            //if (result1.Count == 0)
            //{
            //    Log.Debug($"服务器还没有玩家UID:{response.UID}的数据!开始创建");
            //    Info = Entity.Create<CharacterInfo>(session.Scene);
            //    Info.UID = response.UID;
            //    Info.UserName = request.UserName;
            //    //Info.Level = 1;
            //    //Info.Diamond = 55;
            //    //Info.Gold = 999;
            //    //Info.SkillPoints = 10;
            //    //Info.UnitAttr=new int[] { 1, 2, 3, 4, 5, 6, 7 };
            //    db.Save(Info);
            //}
            //else
            //{
            //    Info = result1[0];
            //}
            

            //L2C_EnterGame l2C_EnterGame = new L2C_EnterGame();
            //CharacterData data=new CharacterData();
            //data.Name = Info.UserName;
            //data.Gold= Info.Gold;
            //data.Diamond= Info.Diamond;
            //data.SkillPoints= Info.SkillPoints;
            //data.Level= Info.Level;
            //data.PlayerAttribute = new UnitAttr { 
            //    Hp = Info.UnitAttr[0],
            //    Mp= Info.UnitAttr[1],
            //    Attack= Info.UnitAttr[2],
            //    Defense= Info.UnitAttr[3],
            //    Shield= Info.UnitAttr[4],
            //    PhysicalHit= Info.UnitAttr[5],
            //    MagicPenetration= Info.UnitAttr[6],
            //    Evade= Info.UnitAttr[7],
            //    Speed= Info.UnitAttr[8],
            //    CriticalHit= Info.UnitAttr[9]
            //};
            //l2C_EnterGame.characterData = data;
            //m_server.GetSession(1024).Send(l2C_EnterGame);
            await FTask.CompletedTask;
        }   
    }
    public class H_C2G_PushMessageToClientHandler : Message<H_C2G_PushMessageToClient>
    {
        protected override async FTask Run(Session session, H_C2G_PushMessageToClient message)
        {
            Log.Debug($"接收到客户端发送给服务器的请求推送消息:{message.ToJson()}");
            // 服务器主动推送给客户端消息
            var m_server = Server.Get(5120);
            IDateBase db = m_server.Scene.World.DateBase;
            List<CharacterInfo> result1 = await db.Query<CharacterInfo>(t => t.UID == 100001);
            CharacterInfo Info;
            if (result1.Count == 0)
            {
                Log.Debug($"服务器还没有玩家UID:{100001}的数据!开始创建");
                Info = Entity.Create<CharacterInfo>(session.Scene);
                Info.UID = 100001;
                Info.UserName = "臭言哥1号";
                db.Save(Info);
            }
            else
            {
                Info = result1[0];
            }


            L2C_EnterGame l2C_EnterGame = new L2C_EnterGame();
            CharacterData data = new CharacterData();
            data.Name = Info.UserName;
            data.Gold = Info.Gold;
            data.Diamond = Info.Diamond;
            data.SkillPoints = Info.SkillPoints;
            data.Level = Info.Level;
            data.PlayerAttribute = new UnitAttr
            {
                Hp = Info.UnitAttr[0],
                Mp = Info.UnitAttr[1],
                MaxHp = Info.UnitAttr[2],
                MaxMp= Info.UnitAttr[3],
                Attack = Info.UnitAttr[4],
                Defense = Info.UnitAttr[5],
                Shield = Info.UnitAttr[6],
                PhysicalHit = Info.UnitAttr[7],
                MagicPenetration = Info.UnitAttr[8],
                Evade = Info.UnitAttr[9],
                Speed = Info.UnitAttr[10],
                CriticalHit = Info.UnitAttr[11]
            };
            l2C_EnterGame.characterData = data;
            session.Send(l2C_EnterGame);
            await FTask.CompletedTask;
        }
    }
}
#endif
