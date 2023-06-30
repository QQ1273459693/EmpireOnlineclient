using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRule
{
    /// <summary>
    /// 通过攻击类型获取攻击目标
    /// </summary>
    /// <param name="skillAttackType"></param>
    /// <param name="herolist">攻击目标列表</param>
    /// <param name="attackerSeatid">攻击者座位id</param>
    /// <returns></returns>
    public static List<HeroLogic> GetAttackListByAttackType(SkillAttackType skillAttackType, List<HeroLogic> herolist, int attackerSeatid)
    {
        List<HeroLogic> attackList = new List<HeroLogic>();
        switch (skillAttackType)
        {
            case SkillAttackType.SingTarget:
                attackList.Add(GetNomalAttackTarget(herolist, attackerSeatid));
                return attackList;
            case SkillAttackType.AllHero:
                return GetHeroSurvivalList(herolist);
            case SkillAttackType.BackRowHero:
                attackList = GetBackRowHero(herolist);
                //后排英雄全部阵亡则攻击前排英雄
                if (attackList.Count == 0)
                    attackList = GetForntRowHero(herolist);
                return GetHeroSurvivalList(attackList);
            case SkillAttackType.ForntRowHero:
                attackList = GetForntRowHero(herolist);
                attackList = GetHeroSurvivalList(attackList);
                //如果前排英雄已经全部阵亡，就攻击后排
                if (attackList.Count == 0)
                    attackList = GetBackRowHero(herolist);
                return GetHeroSurvivalList(attackList); ;
            case SkillAttackType.SameColumnHero:
                int[] targetArr = GetAttackSeatArr(attackerSeatid);
                attackList.Add(herolist[targetArr[0]]);
                attackList.Add(herolist[targetArr[1]]);
                attackList = GetHeroSurvivalList(attackList);
                if (attackList.Count == 0)
                {
                    attackList.Add(herolist[targetArr[2]]);
                    attackList.Add(herolist[targetArr[3]]);
                    attackList = GetHeroSurvivalList(attackList);
                    if (attackList.Count == 0)
                        attackList.Add(herolist[targetArr[4]]);
                }
                return GetHeroSurvivalList(attackList);
        }
        Debuger.LogError("没有查询到有效攻击目标");
        return attackList;
    }
    public static List<HeroLogic> GetForntRowHero(List<HeroLogic> herolist)
    {
        List<HeroLogic> attackList = new List<HeroLogic>();
        attackList.Add(herolist[0]);
        attackList.Add(herolist[1]);
        attackList.Add(herolist[2]);
        return attackList;
    }
    public static List<HeroLogic> GetBackRowHero(List<HeroLogic> herolist)
    {
        List<HeroLogic> attackList = new List<HeroLogic>();
        attackList.Add(herolist[herolist.Count - 1]);
        attackList.Add(herolist[herolist.Count - 2]);
        return attackList;
    }
    /// <summary>
    /// 获取存活列表
    /// </summary>
    private static List<HeroLogic> GetHeroSurvivalList(List<HeroLogic> herolist)
    {
        List<HeroLogic> attackList = new List<HeroLogic>();
        foreach (var item in herolist)
        {
            if (item.objectState == LogicObjectState.Survival)
            {
                attackList.Add(item);
            }
        }
        return attackList;
    }

    /// <summary>
    /// 获取默认攻击目标
    /// </summary>
    public static HeroLogic GetNomalAttackTarget(List<HeroLogic> herolist, int heroSeatid)
    {
        //普通攻击规则，优先攻击前排，其次优先攻击对边的中后排
        if (herolist[0].objectState == LogicObjectState.Survival)
        {
            //Debuger.Log("GetNomalAttackTarget : 前排存活 heroid:" + herolist[0].id);
            return herolist[0];
        }
        //选择中后排
        int[] attackOrderArr = GetAttackSeatArr(heroSeatid);
        for (int i = 0; i < attackOrderArr.Length; i++)
        {
            int heroindex = attackOrderArr[i];
            if (herolist[heroindex].objectState == LogicObjectState.Survival)
            {
                if (herolist[heroindex].id==104)
                {
                    Debuger.Log(123);
                }
                return herolist[heroindex];
            }
        }
        return null;
    }
    private static int[] GetAttackSeatArr(int startSeatid)
    {
        if (startSeatid == 0)
            return new int[] { 0, 1, 2, 3, 4 };
        else if (startSeatid == 1 || startSeatid == 3)
            return new int[] { 1, 3, 2, 4, 0 };
        else if (startSeatid == 2 || startSeatid == 4)
            return new int[] { 2, 4, 1, 3, 0 };
        return null;
    }
}
