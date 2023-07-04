using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundLogicCtrl : ILogicBehaviour
{
    /// <summary>
    /// 回合ID
    /// </summary>
    public int RoundId { get; private set; }
    /// <summary>
    /// 最大回合ID
    /// </summary>
    public int MaxRoundId { get; private set; }
    /// <summary>
    /// 英雄出手队列
    /// </summary>
    private Queue<HeroLogic> mHeroAttackQueue = null;
    private HeroLogicCtrl mHeroLogicCtrl;
    public void OnCreate()
    {
        mHeroLogicCtrl = WorldManager.BattleWorld.heroLogic;
        //回合开始,更新UI7

#if RENDER_LOGIC
        BattleWorldNodes.Instance.roundWindows.RoundStart(RoundId);
#endif
        LogicTimerManager.Instance.DelayCall(2000, NextRoundStart);//动画准备
    }
    public void NextRoundStart()
    {
        RoundId++;
#if RENDER_LOGIC
        BattleWorldNodes.Instance.roundWindows.NextRound(RoundId);
#endif
        foreach (var item in mHeroLogicCtrl.allheroList)
        {
            item.RoundStartEvent(RoundId);
        }
        //计算英雄出手顺序,并把英雄放到队列里面
        mHeroAttackQueue = mHeroLogicCtrl.CalcuTaackSort();
        StartNextHeroAttack();

    }
    public void StartNextHeroAttack()
    {
        //检测战斗是否结束,并进行响应的处理
        if (CheckBattleIsOver()||BattleWorld.battleEnd)
        {
            return;
        }
        //英雄已经出手完成
        if (mHeroAttackQueue.Count==0)
        {
            RoundEnd();
            NextRoundStart();
            return;
        }
        //下一个英雄攻击
        HeroLogic heroLogic=mHeroAttackQueue.Dequeue();
        heroLogic.OnActionEndListener = HeroActionEnd;
        heroLogic.BeginAction();
    }
    public void HeroActionEnd()
    {
        Debuger.Log("开始下一个英雄攻击!函数:StartNextHeroAttack");
        StartNextHeroAttack();
    }
    public bool CheckBattleIsOver()
    {
        //只要有一方英雄全部阵亡,战斗就结束
        if (mHeroLogicCtrl.HerosIsAllDeath(HeroTeamEnum.Self))
        {
            Debuger.Log("我自己的队伍已经全部阵亡!");
            WorldManager.BattleWorld.BattleEnd(false);
            return true;
        }
        if (mHeroLogicCtrl.HerosIsAllDeath(HeroTeamEnum.Enemy))
        {
            Debuger.Log("敌人已经阵亡");
            WorldManager.BattleWorld.BattleEnd(true);
            return true;
        }
        return false;
    }
    public void RoundEnd()
    {
        foreach (var item in mHeroLogicCtrl.allheroList)
        {
            item.RoundEndEvent();
        }
    }

    public void OnDestroy()
    {
        
    }

    public void OnLogicFrameUpdate()
    {
        
    }
}
