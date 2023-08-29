#if TENGINE_NET
using TEngine.Core.Network;
using TEngine.Core;
using TEngine.Core.DataBase;
using Amazon.Runtime.Internal;
using System.Security.Cryptography;
using Amazon.Runtime.Internal.Util;

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
            await FTask.CompletedTask;
        }   
    }
}
#endif
