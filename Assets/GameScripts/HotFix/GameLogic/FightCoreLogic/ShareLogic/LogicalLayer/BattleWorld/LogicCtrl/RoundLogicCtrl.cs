using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundLogicCtrl : ILogicBehaviour
{
    /// <summary>
    /// 回合id
    /// </summary>
    public int RoundId { get; private set; }
    /// <summary>
    /// 最大回合id
    /// </summary>
    public int MaxRoundID { get; private set; } = 15;
    /// <summary>
    /// 英雄攻击队列
    /// </summary>
    private Queue<HeroLogic> mHeroAttackQueue = null;

    private HeroLogicCtrl mHeroLogicCtrl;
    public void OnCreate()
    {
        mHeroLogicCtrl = BattleWorld.Instance.heroLogic;
        //回合开始
#if CLIENT_LOGIC
        BattleWordNodes.Instance.roundWindow.RoundStart();
#endif
        LogicTimeManager.Instance.DelayCall(2000, NextRoundStart);
    }
    /// <summary>
    /// 开始下一回合
    /// </summary>
    public void NextRoundStart()
    {
        if (BattleWorld.Instance.battleEnd)
        {
            return;
        }
        RoundId++;
#if CLIENT_LOGIC
        //显示下一关卡
        BattleWordNodes.Instance.roundWindow.NextRound(RoundId);
#endif
        for (int i = 0; i < mHeroLogicCtrl.AllHeroList.Count; i++)
        {
            mHeroLogicCtrl.AllHeroList[i].RoundStartEvent(RoundId);
        }
        mHeroAttackQueue = BattleWorld.Instance.heroLogic.CalcuAttackSort();
        StartNextHeroAttack();
    }
    public void RoundEnd()
    {
        string heroHPStr = " ";
        for (int i = 0; i < mHeroLogicCtrl.AllHeroList.Count; i++)
        {
            heroHPStr += mHeroLogicCtrl.AllHeroList[i].id + " Hero HP:" + mHeroLogicCtrl.AllHeroList[i].HP + "    怒气值:" + mHeroLogicCtrl.AllHeroList[i].Rage + " isBeCtrl" + mHeroLogicCtrl.AllHeroList[i].IsBeContrl() + "\n";
            mHeroLogicCtrl.AllHeroList[i].RoundEndEvent();
        }

        Debuger.Log("第" + RoundId + "回合结束  \n所有英雄生命值：\n" + heroHPStr);
    }
    public void StartNextHeroAttack()
    {
        if (CheckBattleIsOver() || BattleWorld.Instance.battleEnd)
        {
            return;
        }
        if (mHeroAttackQueue.Count == 0)
        {
            RoundEnd();
            NextRoundStart();
            return;
        }
        HeroLogic heroLogic = mHeroAttackQueue.Dequeue();
        Debuger.Log("开始行动 行动Heroid：" + heroLogic.HeroData.id + " heroState:" + heroLogic.objectState);
        if (RoundId == 3 && heroLogic.HeroData.id == 104)
        {
            Debuger.Log("开始行动 行动Heroid：" + heroLogic.HeroData.id + " heroState:" + heroLogic.objectState);
        }
        heroLogic.OnActionEndListener = HeroActionEnd;
        heroLogic.BeginAction();
    }
    public void HeroActionEnd()
    {

        Debuger.LogError("HeroActionEnd  Enter NextRound Roundid:" + RoundId);
        StartNextHeroAttack();
    }
    /// <summary>
    /// 检测战斗是否结束
    /// </summary>
    public bool CheckBattleIsOver()
    {
        if (mHeroLogicCtrl.HerosIsDeath(HeroTeamEnum.Self))
        {
#if CLIENT_LOGIC
            HallMsgHandlerConter.Instance.SendGetBatleResultRequest(BattleWorld.Instance.BattleId);
#endif
            //BattleWorld.Instance.BattleEnd(false);
            //enemy Win
            Debuger.Log(" BattleOver You Loos!");
            return true;
        }

        if (mHeroLogicCtrl.HerosIsDeath(HeroTeamEnum.Enemy))
        {
#if CLIENT_LOGIC
            HallMsgHandlerConter.Instance.SendGetBatleResultRequest(BattleWorld.Instance.BattleId);
#endif

            //BattleWorld.Instance.BattleEnd(true);
            Debuger.Log(" BattleOver You Win!");
            return true;
        }
        return false;
    }
    public void OnLogicFrameUpdate()
    {

    }
    public void OnDestroy()
    {

    }


}
