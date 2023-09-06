using BestHTTP.SecureProtocol.Org.BouncyCastle.Ocsp;
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
                m_BagSlotList.AddRange(Response.slot);
                Log.Info($"获取背包数据成功!长度是:{m_BagSlotList.Count}");
            }
            else
            {
                Log.Info("获取背包数据失败:" + Response.ErrorCode);
            }
            GameModule.UI.ShowUI<Normal_NewBag>();

        }
    }
}