using BestHTTP.SecureProtocol.Org.BouncyCastle.Ocsp;
using System;
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
    public class GameDataController : DataCenterModule<GameDataController>
    {
        public CharacterData m_CharacterData;

        /// <summary>
        /// 网络模块初始化。
        /// </summary>
        public override void Init()
        {
            base.Init();
            ////注册进入游戏消息回调。
            //GameClient.Instance.RegisterMsgHandler(OuterOpcode.L2C_EnterGame, OnEnterGamRes);
            ////注册注册账号消息回调。
            //GameClient.Instance.RegisterMsgHandler(OuterOpcode.L2C_Login, OnRegisterRes);
        }
        /// <summary>
        /// 进入游戏
        /// </summary>
        public class OnEnterGamRes_Handler : Message<L2C_EnterGame>
        {
            protected override async FTask Run(Session session, L2C_EnterGame message)
            {
                //var networkEntryComponent = session.Scene.GetComponent<NetworkEntryComponent>();
               // networkEntryComponent.Action($"收到服务器推送的消息 message:{message.Message}");

                Log.Info("进入游戏成功,玩家数据:" + message.characterData.ToJson());
                Instance.m_CharacterData = message.characterData;
                GameEvent.Send(GameProcedureEvent.LoadMainStateEvent.EventId);
                await FTask.CompletedTask;
            }
        }
        /// <summary>
        /// 玩家角色数据更新下行
        /// </summary>
        public class OnPlayerNotifyUpdate_Handler : Message<L2C_PlayerNotifyUpdate>
        {
            protected override async FTask Run(Session session, L2C_PlayerNotifyUpdate message)
            {
                //Log.Info("更新角色数据,枚举是:" + message.UpdateCase);
                switch (message.UpdateCase)
                {
                    case 0://更新全部
                        Instance.m_CharacterData= message.characterData;
                        break;
                    case 8://更新装备栏
                        Instance.m_CharacterData.EquipslotDat = message.characterData.EquipslotDat;
                        for (int i = 0; i < Instance.m_CharacterData.EquipslotDat.Count; i++)
                        {
                            var Data = Instance.m_CharacterData.EquipslotDat[i];
                            //Log.Info($"位置{Data.Pos}的装备信息:不为空!{Data.slot!=null}");
                        }
                        break;
                }
                GameEvent.Send(PlayerDataUpdateWndEvent.UpdateEquipSlot.EventId);
                await FTask.CompletedTask;
            }
        }
    }
}