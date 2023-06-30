using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroLogicCtrl : ILogicBehaviour
{

    public List<HeroLogic> AllHeroList { get; private set; } = new List<HeroLogic>();
    public List<HeroLogic> mHeroList;
    private List<HeroLogic> mEnemyList;
    public void OnCreate()
    {

    }
    public void OnCreate(List<HeroData> herolist, List<HeroData> enemylist)
    {
        mHeroList = new List<HeroLogic>();
        mEnemyList = new List<HeroLogic>();
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
    public void CreateHerosByList(List<HeroData> herolist, List<HeroLogic> herologiclist, Transform[] parents, HeroTeamEnum heroTeam)
    {
        //初始化英雄
        foreach (var heroData in herolist)
        {
            Debuger.Log("Heroid:"+heroData.id);
            HeroLogic heroLogic = new HeroLogic(heroData, heroTeam);
#if RENDER_LOGIC
            GameObject obj = Resources.Load<GameObject>("Prefabs/Hero/" + heroData.id);
            GameObject heroObj = GameObject.Instantiate<GameObject>(obj, parents[heroData.seatid]);
            heroObj.transform.localPosition = Vector3.zero;
            heroObj.transform.localRotation = Quaternion.identity;
            HeroRender heroRender = heroObj.GetComponent<HeroRender>();
            heroLogic.SetRenderObject(heroRender);
            heroRender.SetLogicObject(heroLogic);
            heroRender.SetHeroData(heroTeam);
#endif
            heroLogic.OnCreate();
            herologiclist.Add(heroLogic);
            AllHeroList.Add(heroLogic);
        }

    }
    /// <summary>
    /// 计算攻击顺序
    /// </summary>
    public Queue<HeroLogic> CalcuAttackSort()
    {
        Queue<HeroLogic> heroLogicQueue = new Queue<HeroLogic>();
        AllHeroList.Sort((x,y)=> { return y.Agl.CompareTo(x.Agl); });
        foreach (var item in AllHeroList)
        {
            heroLogicQueue.Enqueue(item);
            //Debuger.Log(" agl" + item.Agl);
        }
  
        return heroLogicQueue;
    }
    /// <summary>
    /// 通过队伍获取英雄列表
    /// </summary>
    /// <param name="team"></param>
    /// <returns></returns>
    public List<HeroLogic> GetHeroListByTeam(HeroLogic attacker, HeroTeamEnum attackTeam)
    {
        switch (attacker.HeroTeam)
        {
            case HeroTeamEnum.Self:
                return attackTeam== HeroTeamEnum.Self?mHeroList: mEnemyList;

            case HeroTeamEnum.Enemy:
                return attackTeam == HeroTeamEnum.Enemy ? mHeroList : mEnemyList ;
        }
        return null;
        }


    public bool HerosIsDeath(HeroTeamEnum team)
    {
        Debuger.Log("HeroIsDeath:"+ "mHeroList.Count" + mHeroList.Count +"  enemyCount:"+mEnemyList.Count);
        List<HeroLogic> herolist = team == HeroTeamEnum.Self ? mHeroList : mEnemyList;
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
