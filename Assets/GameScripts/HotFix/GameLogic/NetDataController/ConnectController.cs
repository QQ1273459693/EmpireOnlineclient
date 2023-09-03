using System;
using TEngine;
using TEngine.Core;
using TEngine.Core.Network;
using TEngine.Logic;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 玩家信息网络模块。
    /// </summary>
    public class ConnectController : DataCenterModule<ConnectController>
    {
        public Scene Scene;


        /// <summary>
        /// 网络模块初始化。
        /// </summary>
        public override void Init()
        {
            base.Init();
            Scene = GameApp.Instance.Scene;
        }
        public void OnConnectServer()
        {
            Scene.CreateSession("127.0.0.1:20000", NetworkProtocolType.KCP, OnConnectComplete, OnConnectFail, OnConnectDisconnect);
        }
        private void OnConnectComplete()
        {
            Scene.Session.AddComponent<SessionHeartbeatComponent>().Start(15000);
            Log.Info("已连接到服务器");
        }
        private void OnConnectFail()
        {
            Log.Info("无法连接到服务器");
        }
        private void OnConnectDisconnect()
        {
            Log.Info("服务器主动断开了连接");
        }

    }
}