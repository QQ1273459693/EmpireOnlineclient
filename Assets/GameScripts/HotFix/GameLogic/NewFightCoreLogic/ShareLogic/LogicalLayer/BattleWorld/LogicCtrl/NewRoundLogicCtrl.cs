using GameLogic;
using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;

public class NewRoundLogicCtrl : ILogicBehaviour
{
    /// <summary>
    /// 回合id
    /// </summary>
    public int RoundId { get; private set; }
    /// <summary>
    /// 最大回合id
    /// </summary>
    public int MaxRoundID { get; private set; } = 30;
    /// <summary>
    /// 英雄攻击队列
    /// </summary>
    private Queue<FightUnitLogic> mHeroAttackQueue = null;

    private FightUnitLogicCtrl mHeroLogicCtrl;

    private bool isAutoSkillEnd;//自动技能回合是否结束
    public void OnCreate()
    {
        mHeroLogicCtrl = NewBattleWorld.Instance.heroLogic;
        RoundId = 0;
        Log.Info("游戏战斗回合开始!");
        //回合开始
#if CLIENT_LOGIC
        FightRoundWindow.Instance.RoundStart();
#endif
        LogicTimeManager.Instance.DelayCall(2000, NextRoundStart);
    }
    /// <summary>
    /// 开始下一回合
    /// </summary>
    public void NextRoundStart()
    {
        if (NewBattleWorld.Instance.battleEnd || RoundId >= MaxRoundID)
        {
            return;
        }
        RoundId++;
#if CLIENT_LOGIC
        //显示下一回合
        FightRoundWindow.Instance.NextRound(RoundId);
#endif
        for (int i = 0; i < mHeroLogicCtrl.AllHeroList.Count; i++)
        {
            mHeroLogicCtrl.AllHeroList[i].RoundStartEvent(RoundId);
        }
        mHeroAttackQueue = NewBattleWorld.Instance.heroLogic.CalcuAttackSort();
        isAutoSkillEnd = false;
        StartNextHeroAttack();
    }
    public void RoundEnd()
    {
        string heroHPStr = " ";
        for (int i = 0; i < mHeroLogicCtrl.AllHeroList.Count; i++)
        {
            //heroHPStr += mHeroLogicCtrl.AllHeroList[i].id + " Hero HP:" + mHeroLogicCtrl.AllHeroList[i].HP + "    怒气值:" + mHeroLogicCtrl.AllHeroList[i].Rage + " isBeCtrl" + mHeroLogicCtrl.AllHeroList[i].IsBeContrl() + "\n";
            mHeroLogicCtrl.AllHeroList[i].RoundEndEvent();
        }

        Debuger.Log("第" + RoundId + "回合结束  \n所有英雄生命值：\n" + heroHPStr);
    }
    /// <summary>
    /// 下一个战斗单位开始行动
    /// </summary>
    public void StartNextHeroAttack(bool A=false)
    {
        if (CheckBattleIsOver() || NewBattleWorld.Instance.battleEnd)
        {

            return;
        }
        if (mHeroAttackQueue.Count == 0)
        {
            if (!isAutoSkillEnd)
            {
                //到自动技能回合已过,开始进入主动技能回合
                isAutoSkillEnd = true;
                mHeroAttackQueue = NewBattleWorld.Instance.heroLogic.CalcuAttackSort();
            }
            else
            {
                //所有回合已经结束
                RoundEnd();
                NextRoundStart();
                return;
            }
        }
        FightUnitLogic heroLogic = mHeroAttackQueue.Dequeue();
        Log.Info("开始行动 行动单位：" + heroLogic.HeroData.Name + " 单位状态:" + heroLogic.objectState);
        heroLogic.OnActionEndListener = HeroActionEnd;
        heroLogic.BeginAction(isAutoSkillEnd, UnitActionEnum.Skill);
    }
    /// <summary>
    /// 战斗单位行动结束
    /// </summary>
    public void HeroActionEnd()
    {
        Log.Info("此回合结束,开始下一回合:" + RoundId);
        StartNextHeroAttack(true);
    }
    /// <summary>
    /// 检测战斗是否结束
    /// </summary>
    public bool CheckBattleIsOver()
    {
        if (mHeroLogicCtrl.HerosIsDeath(FightUnitTeamEnum.Self))
        {
#if CLIENT_LOGIC
            //HallMsgHandlerConter.Instance.SendGetBatleResultRequest(BattleWorld.Instance.BattleId);
#endif
            NewBattleWorld.Instance.BattleEnd(false);
            //enemy Win
            Debuger.Log(" BattleOver You Loos!");
            return true;
        }

        if (mHeroLogicCtrl.HerosIsDeath(FightUnitTeamEnum.Enemy))
        {
#if CLIENT_LOGIC
            //HallMsgHandlerConter.Instance.SendGetBatleResultRequest(BattleWorld.Instance.BattleId);
#endif

            NewBattleWorld.Instance.BattleEnd(true);
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
