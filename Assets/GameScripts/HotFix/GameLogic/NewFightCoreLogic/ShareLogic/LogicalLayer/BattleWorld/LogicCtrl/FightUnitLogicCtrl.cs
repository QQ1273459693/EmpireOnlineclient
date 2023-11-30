using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;

public class FightUnitLogicCtrl : ILogicBehaviour
{

    public List<FightUnitLogic> AllHeroList { get; private set; } = new List<FightUnitLogic>();
    public List<FightUnitLogic> mHeroList;
    private List<FightUnitLogic> mEnemyList;
    public void OnCreate()
    {

    }
    public void OnCreate(List<FightUnitData> herolist, List<FightUnitData> enemylist)
    {
        mHeroList = new List<FightUnitLogic>();
        mEnemyList = new List<FightUnitLogic>();
#if CLIENT_LOGIC
        CreateHerosByList(herolist, mHeroList, BattleWordNodes.Instance.heroTransArr, HeroTeamEnum.Self);
        CreateHerosByList(enemylist, mEnemyList, BattleWordNodes.Instance.enemyTransArr, HeroTeamEnum.Enemy);

#else
        CreateHerosByList(herolist, mHeroList, null, HeroTeamEnum.Self);
        CreateHerosByList(enemylist, mEnemyList, null, HeroTeamEnum.Enemy);
#endif
    }

    public void OnLogicFrameUpdate() {}

       /// <summary>
    /// 通过数据列表创建英雄
    /// </summary>
    public void CreateHerosByList(List<FightUnitData> herolist, List<FightUnitLogic> herologiclist, Transform[] parents, HeroTeamEnum heroTeam)
    {
        //初始化英雄
        foreach (var heroData in herolist)
        {
            Log.Info("初始化-------战斗单位ID:"+heroData.ID);
            FightUnitLogic heroLogic = new FightUnitLogic(heroData, heroTeam);
#if RENDER_LOGIC
            FightUnitRender heroRender = new FightUnitRender();
            heroRender.SetLogicObject(heroLogic, parents[heroData.SeatId].gameObject);
            heroLogic.SetRenderObject(heroRender);
            heroRender.SetHeroData(heroData, heroTeam);

#endif
            heroLogic.OnCreate();
            herologiclist.Add(heroLogic);
            AllHeroList.Add(heroLogic);
        }

    }
    /// <summary>
    /// 计算攻击顺序
    /// </summary>
    public Queue<FightUnitLogic> CalcuAttackSort()
    {
        Queue<FightUnitLogic> heroLogicQueue = new Queue<FightUnitLogic>();
        AllHeroList.Sort((x, y) => { return y.Speed.CompareTo(x.Speed); });
        foreach (var item in AllHeroList)
        {
            heroLogicQueue.Enqueue(item);
        }
        return heroLogicQueue;
    }
    /// <summary>
    /// 通过队伍获取英雄列表
    /// </summary>
    /// <param name="team"></param>
    /// <returns></returns>
    public List<FightUnitLogic> GetHeroListByTeam(FightUnitTeamEnum attackTeam)
    {
        switch (attackTeam)
        {
            case FightUnitTeamEnum.Self:
                return mHeroList;

            case FightUnitTeamEnum.Enemy:
                return mEnemyList ;
            case FightUnitTeamEnum.ALL:
                return AllHeroList;
        }
        return null;
    }


    public bool HerosIsDeath(HeroTeamEnum team)
    {
        Debuger.Log("HeroIsDeath:"+ "mHeroList.Count" + mHeroList.Count +"  enemyCount:"+mEnemyList.Count);
        List<FightUnitLogic> herolist = team == HeroTeamEnum.Self ? mHeroList : mEnemyList;
        for (int i = 0; i < herolist.Count; i++)
        {
            if (herolist[i].objectState!= LogicObjectState.Death)
            {
                return false;
            }
        }
        return true;
    }
    public void OnDestroy()
    {
        for (int i = 0; i < AllHeroList.Count; i++)
        {
            AllHeroList[i].OnDestroy();
        }
        AllHeroList.Clear();
        mHeroList.Clear();
        mEnemyList.Clear();
    }


}
