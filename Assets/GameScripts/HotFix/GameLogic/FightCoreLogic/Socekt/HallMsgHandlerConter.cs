using GameBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if CLIENT_LOGIC
/// <summary>
/// 战斗消息管理器
/// </summary>
public class HallMsgHandlerConter : Singleton<HallMsgHandlerConter>
{
    public void Initialize()
    {
        NetEventControl.AddEvent(Protocal.LogicResponse, OnLogicResponse);
        NetEventControl.AddEvent(Protocal.StartBattleResponse, OnStartBattleResponse);
        NetEventControl.AddEvent(Protocal.BattleResultResponse, OnGetBattleResultResponse);
    }
    /// <summary>
    /// 发送登录请求
    /// </summary>
    /// <param name="data"></param>
    public void SendLoginRequest()
    {
        LoginRequest request = new LoginRequest();
        request.deviceid = SystemInfo.deviceUniqueIdentifier;
        NetWorkManager.Instance.SendPacket(Protocal.LoignRequest, request);
    }
    public void OnLogicResponse(byte[] data)
    {
        LoginResponse response = ProtoBuffSerialize.DeSerialize<LoginResponse>(data);
        if (response.resultCode == 0)
        {
            BattleWordNodes.Instance.startWidnow.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 发送开始战斗请求
    /// </summary>
    public void SendStatBattleRequest(List<HeroSeatDataPb> herolist)
    {
        StartBattleRequest request = new StartBattleRequest();
        request.heroSeatDataList = herolist;

        NetWorkManager.Instance.SendPacket(Protocal.StartBattleReqeuest, request);
    }
    public void OnStartBattleResponse(byte[] data)
    {
        StartBattleResponse response = ProtoBuffSerialize.DeSerialize<StartBattleResponse>(data);
        if (response.resultCode == 0)
        {
            BattleWordNodes.Instance.selectHeroWidnow.opHerosObjs.SetActive(false);
            BattleWordNodes.Instance.selectHeroWidnow.gameObject.SetActive(false);

            List<HeroData> heroDataList = new List<HeroData>();
            List<HeroData> enemyHeroDataList = new List<HeroData>();
            for (int i = 0; i < response.heroDatalist.Count; i++)
            {
                heroDataList.Add(response.heroDatalist[i].ToHeroData());
            }
            for (int i = 0; i < response.enemyDatalist.Count; i++)
            {
                enemyHeroDataList.Add(response.enemyDatalist[i].ToHeroData());
            }
            WorldManager.CreateBattleWord(heroDataList, enemyHeroDataList, response.randomSite, response.battleid);
        }
    }


    /// <summary>
    /// 发送获取战斗结果请求
    /// </summary>
    /// <param name="battleid"></param>
    public void SendGetBatleResultRequest(int battleid)
    {
        Debug.Log("SendGetBatleResultRequest 获取战斗结果...");
        BattleResultRequest resultRequest = new BattleResultRequest();
        resultRequest.battleid = battleid;
        NetWorkManager.Instance.SendPacket(Protocal.BattleResultRequest, resultRequest);
    }
    public void OnGetBattleResultResponse(byte[] data)
    {
        BattleResultResponse response = ProtoBuffSerialize.DeSerialize<BattleResultResponse>(data);
        if (response.resultCode==0)
        {
            BattleWorld.Instance.BattleEnd(response);
        }
    }
    public void OnDestroy()
    {
        NetEventControl.RemoveEvent(Protocal.LogicResponse, OnLogicResponse);
        NetEventControl.RemoveEvent(Protocal.StartBattleResponse, OnStartBattleResponse);
        NetEventControl.RemoveEvent(Protocal.BattleResultResponse, OnGetBattleResultResponse);
    }


}
#endif