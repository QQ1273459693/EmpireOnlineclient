using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRule
{
    public static VInt CalculaDamage(SkillConfig skillCfg,HeroLogic attacker,HeroLogic attackTarget)
    {
        VInt rawDamage = new VInt(0);
        switch (skillCfg.damageType)
        {
            case DamageType.NomalDamage:
                //�˺�����=����/(�˺�+����)*100%; atk=1000 def=300 300/(1000+300)==300/1300=0.23
                //�˺�-(������*�˺��������)
                VInt damageRate = attackTarget.Def / (attacker.ATK + attackTarget.Def);
                rawDamage = attacker.ATK-(attacker.ATK*damageRate);
                break;
            case DamageType.RealDamage://��ʵ�˺�:���ӻ��׺ͼ���Ч��
                rawDamage = attacker.ATK;
                break;
            case DamageType.AtkPercentage:
                //�˺��������
                damageRate = attacker.Def / (attacker.ATK + attackTarget.Def);
                //�������ٷֱ�
                VInt atkMutiple = skillCfg.damagePercentage / new VInt(100);
                VInt totalDamage = attacker.ATK * atkMutiple;
                rawDamage = (totalDamage - (totalDamage * damageRate));
                break;
            case DamageType.HPPercentage:
                //Ѫ���ٷֱ��˺�
                damageRate = attacker.Def / (attacker.ATK + attackTarget.Def);
                //�������ٷֱ�
                VInt hpMutiple = skillCfg.damagePercentage / new VInt(100);
                VInt HptotalDamage = attacker.Hp * hpMutiple;
                rawDamage = (HptotalDamage - (HptotalDamage * damageRate));
                break;
        }
        return rawDamage;
    }
    public static VInt CalculaDamage(BuffConfig buffCfg, HeroLogic attacker, HeroLogic attackTarget)
    {
        VInt rawDamage = new VInt(0);
        switch (buffCfg.damageType)
        {
            case BuffDamageType.NomalAttackDamage:
                //�˺�����=����/(�˺�+����)*100%; atk=1000 def=300 300/(1000+300)==300/1300=0.23
                //�˺�-(������*�˺��������)
                VInt damageRate = attackTarget.Def / (attacker.ATK + attackTarget.Def);
                rawDamage = attacker.ATK - (attacker.ATK * damageRate);
                break;
            case BuffDamageType.RealDamage://��ʵ�˺�:���ӻ��׺ͼ���Ч��
                rawDamage = attacker.ATK;
                break;
            case BuffDamageType.AtkPercentage:
                //�˺��������
                damageRate = attacker.Def / (attacker.ATK + attackTarget.Def);
                //�������ٷֱ�
                VInt atkMutiple = buffCfg.damageOrPercentage / new VInt(100);
                VInt totalDamage = attacker.ATK * atkMutiple;
                rawDamage = (totalDamage - (totalDamage * damageRate));
                break;
            case BuffDamageType.HPPercentage:
                //Ѫ���ٷֱ��˺�
                damageRate = attacker.Def / (attacker.ATK + attackTarget.Def);
                //�������ٷֱ�
                VInt hpMutiple = buffCfg.damageOrPercentage / new VInt(100);
                VInt HptotalDamage = attacker.Hp * hpMutiple;
                rawDamage = (HptotalDamage - (HptotalDamage * damageRate));
                break;
        }
        return rawDamage;
    }
    /// <summary>
    /// ͨ���������ͻ�ȡ����Ŀ��
    /// </summary>
    /// <returns></returns>
    public static List<HeroLogic> GetAttackListByAttackType(SkillAttackType attackType,List<HeroLogic> herolist,int attackSeatid)
    {
        List<HeroLogic> attackList = new List<HeroLogic>();
        switch (attackType)
        {
            case SkillAttackType.SingTarget:
                attackList.Add(GetNormalAttackTarget(herolist,attackSeatid));
                return attackList;
            case SkillAttackType.AllHero:
                return GetHeroSurvivalList(herolist);
            case SkillAttackType.BackRowHero:
                attackList=GetBackRowHeroList(herolist);
                attackList = GetHeroSurvivalList(attackList);
                //�������Ӣ��ȫ������,Ĭ��ѡ��ǰ��Ӣ��
                if (attackList.Count==0)
                {
                    //����Ӣ��û��ȥ����ǰ��
                    attackList=GetForntRowHeroList(herolist);
                }
                return GetHeroSurvivalList(attackList);
            case SkillAttackType.ForntRowHero:
                attackList = GetForntRowHeroList(herolist);
                attackList = GetHeroSurvivalList(attackList);
                //���ǰ��û��ѡ�����
                if (attackList.Count==0)
                {
                    //ǰ��û��Ӣ�۴��
                    attackList = GetBackRowHeroList(attackList);
                }
                return GetHeroSurvivalList(attackList);
            case SkillAttackType.SameColumnHero:
                int[] targetArr = GetAttackSeatArr(attackSeatid);
                attackList.Add(herolist[targetArr[0]]);
                attackList.Add(herolist[targetArr[1]]);
                attackList = GetHeroSurvivalList(attackList);
                if (attackList.Count==0)
                {
                    attackList.Add(herolist[targetArr[2]]);
                    attackList.Add(herolist[targetArr[3]]);
                    attackList = GetHeroSurvivalList(attackList);
                    if (attackList.Count==0)
                    {
                        attackList.Add(herolist[targetArr[4]]);
                    }
                }
                return attackList;
        }
        Debuger.LogError("û���ҵ���Ч����Ŀ��");
        return attackList;
    }
    public static List<HeroLogic> GetForntRowHeroList(List<HeroLogic> herolist)
    {
        List<HeroLogic> attackList = new List<HeroLogic>
        {
            herolist[0],
            herolist[1],
            herolist[2]
        };
        return attackList;
    }
    /// <summary>
    /// ��ȡ����Ӣ��
    /// </summary>
    /// <param name="herolist"></param>
    /// <returns></returns>
    public static List<HeroLogic> GetBackRowHeroList(List<HeroLogic> herolist)
    {
        List<HeroLogic> attackList = new List<HeroLogic>();
        if (herolist.Count > 0 && herolist.Count > 1)
        {
            attackList.Add(herolist[herolist.Count - 1]);
            attackList.Add(herolist[herolist.Count - 2]);
        } else if (herolist.Count==1)
        {
            attackList.Add(herolist[herolist.Count - 1]);
        }
        
        return attackList;
    }
    /// <summary>
    /// ��ȡ����Ӣ��
    /// </summary>
    /// <param name="herolist"></param>
    /// <returns></returns>
    public static List<HeroLogic> GetHeroSurvivalList(List<HeroLogic> herolist)
    {
        List<HeroLogic> attackList = new List<HeroLogic>();
        foreach (var item in herolist)
        {
            if (item.objectState== LogicObjectState.Survival)
            {
                attackList.Add(item);
            }
        }
        return attackList;
    }
    /// <summary>
    /// ��ȡĬ�Ϲ���Ŀ��,Ĭ�Ϲ�������,���ȹ���ǰ��M������ȹ����к���
    /// </summary>
    /// <param name="herolist"></param>
    /// <param name="heroSeatid"></param>
    /// <returns></returns>
    public static HeroLogic GetNormalAttackTarget(List<HeroLogic> herolist,int heroSeatid)
    {
        //���ǰ�ŵ�һ�����,��ֱ�ӹ���
        if (herolist[0].objectState==LogicObjectState.Survival)
        {
            return herolist[0];
        }
        //ѡ���к���
        int[] attackOrderArr = GetAttackSeatArr(heroSeatid);
        for (int i = 0; i < attackOrderArr.Length; i++)
        {
            int heroIndex = attackOrderArr[i];
            if (herolist[heroIndex].objectState== LogicObjectState.Survival)
            {
                return herolist[heroIndex];
            }
        }
        return null;
    }
    public static int[] GetAttackSeatArr(int startSeatid)
    {
        if (startSeatid==0)
        {
            return new int[] { 0, 1, 2, 3, 4};
        }else if (startSeatid==1||startSeatid==4)
        {
            return new int[] { 1, 3, 2, 4, 0 };
        }
        else if (startSeatid == 2 || startSeatid == 3)
        {
            return new int[] { 2, 4, 1, 3, 0 };
        }
        return null;
    }
}
