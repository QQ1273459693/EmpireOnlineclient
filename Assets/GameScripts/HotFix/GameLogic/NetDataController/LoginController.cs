using BestHTTP.SecureProtocol.Org.BouncyCastle.Ocsp;
using System;
using TEngine;
using TEngine.Core;
using TEngine.Core.Network;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 玩家信息网络模块。
    /// </summary>
    public class LoginController:DataCenterModule<LoginController>
    {
        /// <summary>
        /// 网络模块初始化。
        /// </summary>
        public override void Init()
        {
            base.Init();
            ////注册注册账号消息回调。
            //GameClient.Instance.RegisterMsgHandler(OuterOpcode.L2C_Login, OnRegisterRes);
        }

        #region 登陆模块

        /// <summary>
        /// 登录消息请求。
        /// </summary>
        /// <param name="userName">用户名。</param>
        /// <param name="passWord">用户密码。</param>
        public async void ReqLogin(string userName, string passWord)
        {
            
            if (GameClient.Instance.Status == GameClientStatus.StatusEnter)
            {
                Log.Info("当前已经登录成功。");
                return;
            }
            GameClient.Instance.Status = GameClientStatus.StatusLogin;

            //OnLoginButtonClick(userName, passWord).Coroutine();

            C2L_Login loginRequest = new C2L_Login()
            {
                UserName = userName,
                Password = passWord
            };




            var Response = (L2C_Login)await GameClient.Instance.Call(loginRequest);

            if (Response.ErrorCode==0)
            {
                Log.Info($"游戏角色登陆成功!你的UID是:{Response.UID}");
                GameClient.Instance.Status = GameClientStatus.StatusEnter;
                GameApp.Instance.Scene.Session.Send(new H_C2G_PushMessageToClient()
                {
                    Message = "请推送角色数据给我"
                });
            }
            else
            {
                Log.Info("登陆失败:"+Response.ErrorCode);
            }
            //GameClient.Instance.Send(loginRequest);

        }
        //private async FTask OnLoginButtonClick(string userName, string passWord)
        //{
        //    var response = (L2C_Login)await Scene.Session.Call(new C2L_Login()
        //    {
        //        UserName = userName,
        //        Password = passWord
        //    });

        //    if (response.ErrorCode == 0)
        //    {
        //        LogDebug($"游戏角色登陆成功!你的UID是:{response.UID}");
        //        return;
        //    }
        //}

        /// <summary>
        /// 登录消息回调。
        /// </summary>
        /// <param name="response">网络回复消息包。</param>
        //public void OnLoginRes(IResponse response)
        //{
        //    if (NetworkUtils.CheckError(response))
        //    {
        //        Log.Info("登录失败！");
        //        GameClient.Instance.Status = GameClientStatus.StatusConnected;
        //        return;
        //    }
        //    L2C_Login ret = (L2C_Login)response;
        //    Log.Info("登陆成功!角色是:"+ret.UID);
        //    //H_G2C_LoginResponse ret = (H_G2C_LoginResponse)response;
        //    //Log.Debug(ret.ToJson());
        //    GameClient.Instance.Status = GameClientStatus.StatusEnter;
        //}
        #endregion


        #region Register
        /// <summary>
        /// 注册消息请求。
        /// </summary>
        /// <param name="userName">用户名。</param>
        /// <param name="passWord">用户密码。</param>
        public async void ReqRegister(string userName,string passWord)
        {
            C2L_CreateRole registerQuest =new C2L_CreateRole()
            {
                UserName = userName,
                Password = passWord
            };

            var Response = (L2C_CreateRole)await GameClient.Instance.Call(registerQuest);
            if (NetworkUtils.CheckError(Response))
            {
                return;
            }
            Log.Debug("注册成功!"+ Response.ToJson());
            //GameClient.Instance.Send(registerQuest);
        }
        #endregion
        
    }
}