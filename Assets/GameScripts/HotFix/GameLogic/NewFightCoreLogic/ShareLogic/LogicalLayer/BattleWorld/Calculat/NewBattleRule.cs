using System;
using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;

public class NewBattleRule
{
    static int[,] SelfArray2D = { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 }, { 9, 10 } };
    static int[,] EnemyArray2D = { { 11, 12 }, { 13, 14 }, { 15, 16 }, { 17, 18 }, { 19, 20 } };


    /// <summary>
    /// 根据十字范围选取目标位置
    /// </summary>
    /// <returns></returns>
    public static List<int> GetCrossRage(int TargetSeatID)
    {
        bool isSelf = true;
        if (TargetSeatID > 10)
        {
            //是敌方
            TargetSeatID = TargetSeatID - 10;
            isSelf = false;
        }
        List<int> result = new List<int>();
        int ArrayIndx = (int)Math.Ceiling((float)TargetSeatID / (float)2) - 1;//获取所在数组索引
        if (isSelf)
        {
            result.Add(SelfArray2D[ArrayIndx, 0]);
            result.Add(SelfArray2D[ArrayIndx, 1]);
        }
        else
        {
            result.Add(EnemyArray2D[ArrayIndx, 0]);
            result.Add(EnemyArray2D[ArrayIndx, 1]);
        }


        //获取上范围
        if (ArrayIndx != 0)
        {
            if (isSelf)
            {
                result.Add(SelfArray2D[ArrayIndx - 1, TargetSeatID % 2 == 0 ? 1 : 0]);
            }
            else
            {
                result.Add(EnemyArray2D[ArrayIndx - 1, TargetSeatID % 2 == 0 ? 1 : 0]);
            }

        }

        //取下范围
        if (ArrayIndx != 4)
        {
            if (isSelf)
            {
                result.Add(SelfArray2D[ArrayIndx + 1, TargetSeatID % 2 == 0 ? 1 : 0]);
            }
            else
            {
                result.Add(EnemyArray2D[ArrayIndx + 1, TargetSeatID % 2 == 0 ? 1 : 0]);
            }
        }

        return result;
    }

    /// <summary>
    /// 通过攻击类型获取攻击目标
    /// </summary>
    /// <returns></returns>
    public static List<FightUnitLogic> GetAttackListByAttackType(SkillTarget SkillTarget, SkillRadiusType SkillRadiusType,int AttackSeatid,int TargetSeatid)
    {
        List<FightUnitLogic> attackList = new List<FightUnitLogic>();
        List<FightUnitLogic> herolist=null;
        switch (SkillTarget)
        {
            case SkillTarget.None://真正全屏
                herolist = NewBattleWorld.Instance.heroLogic.GetHeroListByTeam(FightUnitTeamEnum.ALL);
                break;
            case SkillTarget.Teammate://友方
                herolist = NewBattleWorld.Instance.heroLogic.GetHeroListByTeam(FightUnitTeamEnum.Self);
                break;
            case SkillTarget.Enemy://敌方
                herolist = NewBattleWorld.Instance.heroLogic.GetHeroListByTeam(FightUnitTeamEnum.Enemy);
                break;
            case SkillTarget.SELF://自身
                herolist = NewBattleWorld.Instance.heroLogic.GetHeroListByTeam(FightUnitTeamEnum.Self);
                for (int i = 0; i < herolist.Count; i++)
                {
                    if (herolist[i].SeatID== AttackSeatid)
                    {
                        attackList.Add(herolist[i]);
                        break;
                    }
                }
                break;
        }
        herolist = GetHeroSurvivalList(herolist);
        if (SkillTarget!= SkillTarget.SELF)
        {
            //在自身范围的时候就已经赛选完成了
            if (herolist == null)
            {
                Log.Error("错误!获取攻击列表为空");
                return null;
            }
            switch (SkillRadiusType)
            {
                case SkillRadiusType.ALL://全部
                    attackList.AddRange(herolist);
                    break;
                case SkillRadiusType.SOLO://单人
                    for (int i = 0; i < herolist.Count; i++)
                    {
                        if (herolist[i].SeatID == TargetSeatid)
                        {
                            attackList.Add(herolist[i]);
                            break;
                        }
                    }
                    break;
                case SkillRadiusType.CROSS://十字范围
                    var CrossList=GetCrossRage(TargetSeatid);
                    for (int i = 0; i < CrossList.Count; i++)
                    {
                        int SeatID = CrossList[i];
                        for (int j = 0; j < herolist.Count; j++)
                        {
                            if (herolist[j].SeatID == SeatID)
                            {
                                attackList.Add(herolist[j]);
                                break;
                            }
                        }
                    }
                    break;
            }
        }
        //switch (skillAttackType)
        //{
        //    case SkillAttackType.SingTarget:
        //        attackList.Add(GetNomalAttackTarget(herolist, attackerSeatid));
        //        return attackList;
        //    case SkillAttackType.AllHero:
        //        return GetHeroSurvivalList(herolist);
        //    case SkillAttackType.BackRowHero:
        //        attackList = GetBackRowHero(herolist);
        //        //后排英雄全部阵亡则攻击前排英雄
        //        if (attackList.Count == 0)
        //            attackList = GetForntRowHero(herolist);
        //        return GetHeroSurvivalList(attackList);
        //    case SkillAttackType.ForntRowHero:
        //        attackList = GetForntRowHero(herolist);
        //        attackList = GetHeroSurvivalList(attackList);
        //        //如果前排英雄已经全部阵亡，就攻击后排
        //        if (attackList.Count == 0)
        //            attackList = GetBackRowHero(herolist);
        //        return GetHeroSurvivalList(attackList); ;
        //    case SkillAttackType.SameColumnHero:
        //        int[] targetArr = GetAttackSeatArr(attackerSeatid);
        //        attackList.Add(herolist[targetArr[0]]);
        //        attackList.Add(herolist[targetArr[1]]);
        //        attackList = GetHeroSurvivalList(attackList);
        //        if (attackList.Count == 0)
        //        {
        //            attackList.Add(herolist[targetArr[2]]);
        //            attackList.Add(herolist[targetArr[3]]);
        //            attackList = GetHeroSurvivalList(attackList);
        //            if (attackList.Count == 0)
        //                attackList.Add(herolist[targetArr[4]]);
        //        }
        //        return GetHeroSurvivalList(attackList);
        //}
        return attackList;
    }
    public static List<FightUnitLogic> GetForntRowHero(List<FightUnitLogic> herolist)
    {
        List<FightUnitLogic> attackList = new List<FightUnitLogic>();
        attackList.Add(herolist[0]);
        attackList.Add(herolist[1]);
        attackList.Add(herolist[2]);
        return attackList;
    }
    public static List<FightUnitLogic> GetBackRowHero(List<FightUnitLogic> herolist)
    {
        List<FightUnitLogic> attackList = new List<FightUnitLogic>();
        attackList.Add(herolist[herolist.Count - 1]);
        attackList.Add(herolist[herolist.Count - 2]);
        return attackList;
    }
    /// <summary>
    /// 获取存活列表
    /// </summary>
    public static List<FightUnitLogic> GetHeroSurvivalList(List<FightUnitLogic> herolist)
    {
        List<FightUnitLogic> attackList = new List<FightUnitLogic>();
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
    public static FightUnitLogic GetNomalAttackTarget(List<FightUnitLogic> herolist, int heroSeatid)
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
                if (herolist[heroindex].ID==104)
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
