using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TEngine;
using UnityEngine;

public enum HeroTeamEnum
{
    None,
    Self,
    Enemy
}
public class HeroLogicCtrl : ILogicBehaviour
{
    public List<HeroLogic> allheroList=new List<HeroLogic>();//所有英雄列表
    public List<HeroLogic> heroList=new List<HeroLogic>();//我方英雄列表
    public List<HeroLogic> enemyList=new List<HeroLogic>();//敌方英雄列表
    public void OnCreate()
    {
        
    }
    public void OnCreate(List<HeroData> playerHerolist, List<HeroData> enemyHeroList)
    {
#if CLIENT_LOGIC
        CreateHerosByList(playerHerolist, heroList,BattleWorldNodes.Instance.heroTransArr,HeroTeamEnum.Self);
        CreateHerosByList(enemyHeroList, enemyList, BattleWorldNodes.Instance.enemyTransArr,HeroTeamEnum.Enemy);
#else
        CreateHerosByList(playerHerolist,heroList,null,HeroTeamEnum.Self);
        CreateHerosByList(enemyHeroList,enemyList, null,HeroTeamEnum.Enemy);
#endif
    }
    public void OnDestroy()
    {
        foreach (var hero in allheroList)
        {
            hero.OnDestroy();
        }
        allheroList.Clear();
        heroList.Clear();
        enemyList.Clear();
    }
    /// <summary>
    /// 英雄数据列表
    /// </summary>
    public void CreateHerosByList(List<HeroData> herolist,List<HeroLogic> heroLogicList,Transform[] parents, HeroTeamEnum heroTeam)
    {
        var SpineList = ConfigLoader.Instance.Tables.TbEnemySpine.DataMap;
        foreach (HeroData hero in herolist)
        {
            HeroLogic heroLogic=new HeroLogic(hero,heroTeam);
#if CLIENT_LOGIC
            //生成英雄

            //GameObject heroObj = GameModule.Resource.LoadAsset<GameObject>(SpineList[hero.id].SpineResName+"Root");
            HeroRender heroRender = new HeroRender();//heroObj.GetComponent<HeroRender>();
            //heroObj.transform.SetParent(parents[hero.seatid],false);
            heroRender.SetLogicObject(heroLogic, parents[hero.seatid].gameObject);
            heroLogic.SetRenderObject(heroRender);
            heroRender.SetHeroData(hero, heroTeam);
#endif
            heroLogic.OnCreate();
            heroLogicList.Add(heroLogic);
            allheroList.Add(heroLogic);

        }
    }
    public void OnLogicFrameUpdate()
    {
        
    }
    /// <summary>
    /// 计算英雄攻击顺序
    /// </summary>
    /// <returns></returns>
    public Queue<HeroLogic> CalcuTaackSort()
    {
        Queue<HeroLogic> heroLogicQueue=new Queue<HeroLogic>();
        allheroList.Sort((x, y) =>
        {
            return y.Agl.CompareTo(x.Agl);
        });
        foreach (var item in allheroList)
        {
            heroLogicQueue.Enqueue(item);
        }
        return heroLogicQueue;
    }
    /// <summary>
    /// 通过攻击类型获取英雄列表
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="attackTeam"></param>
    /// <returns></returns>
    public List<HeroLogic> GetHeroListByTeam(HeroLogic attacker,HeroTeamEnum attackTeam)
    {
        switch (attacker.HeroTeam)
        {
            case HeroTeamEnum.Self:
                return attackTeam == HeroTeamEnum.Self ? heroList : enemyList;
            case HeroTeamEnum.Enemy:
                return attackTeam == HeroTeamEnum.Enemy ? heroList : enemyList;
        }
        return null;
    }
    /// <summary>
    /// 英雄是否全部阵亡
    /// </summary>
    public bool HerosIsAllDeath(HeroTeamEnum heroTeam)
    {
        List<HeroLogic> list= heroTeam == HeroTeamEnum.Self ? heroList : enemyList;
        foreach (var item in list)
        {
            if (item.objectState==LogicObjectState.Survival)
            {
                return false;
            }
        }
        return true;
    }
}
