using System;
using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;
using static NewBattleDataCalculatConter;

public enum NewAnimState
{
    StopAnim,
    RePlayAnim,
}
/// <summary>
/// 战斗单位队伍
/// </summary>

public enum FightUnitTeamEnum
{
    ALL,//全部队伍
    Self,//友方
    Enemy,//敌方
}
/// <summary>
/// 单位行动状态
/// </summary>
public enum UnitActionEnum
{
    /// <summary>
    /// 普通攻击
    /// </summary>
    NormalAttack=1,
    /// <summary>
    /// 释放主动技能
    /// </summary>
    Skill=2,
    /// <summary>
    /// 防御
    /// </summary>
    Defense = 3,
    /// <summary>
    /// 逃跑
    /// </summary>
    Escape=4,
}
public class FightUnitLogic : LogicObject
{
    protected int hp;
    protected int mp;
    protected int meleeak;
    protected int rangeak;
    protected int magicak;
    protected int medef;
    protected int rgdef;
    protected int mgdef;
    protected int elmres;
    protected int curseMgRES;
    protected int shield;
    protected int physicalHit;
    protected int eleMagicHit;
    protected int curseMagicHit;
    protected int magicPenetration;
    protected int evade;
    protected int speed;
    protected int criticalHit;
    protected int tough;
    protected int armorBreakingAT;

    public int HP { get { return hp; } }
    public int MAXHP { get; protected set; }
    public int MP { get { return mp; } }
    public int MAXMP { get; protected set; }
    public int MeleeAk { get { return meleeak; } }
    public int RangeAk { get { return rangeak; } }
    public int MagicAk { get { return magicak; } }
    public int MeDEF { get { return medef; } }
    public int RGDEF { get { return rgdef; } }
    public int MGDEF { get { return mgdef; } }
    public int ELMRES { get { return elmres; } }
    public int CurseMgRES { get { return curseMgRES; } }
    public int Shield { get { return shield; } }
    public int PhysicalHit { get { return physicalHit; } }
    public int EleMagicHit { get { return eleMagicHit; } }
    public int CurseMagicHit { get { return curseMagicHit; } }
    public int MagicPenetration { get { return magicPenetration; } }
    public int Evade { get { return evade; } }
    public int Speed { get { return speed; } }
    public int CriticalHit { get { return criticalHit; } }
    public int MixDamage { get; protected set; }
    public int MaxDamage { get; protected set; }
    public int Tough { get { return tough; } }
    public int ArmorBreakingAT { get { return armorBreakingAT; } }
    public int ID => HeroData.ID;
    public int SeatID => HeroData.SeatId;
    public FightUnitData HeroData { get; private set; }
    public FightUnitTeamEnum UnitTeam { get; private set; }
    public string Name { get; private set; }
    public int WeaponType { get; private set; }
#if RENDER_LOGIC
    public FightUnitRender HeroRender { get { return (FightUnitRender)RenderObj; } }
#endif

    public List<NewBuffLogic> DeBuffList = new List<NewBuffLogic>(3);//减益Debuff列表
    public List<NewBuffLogic> BuffList = new List<NewBuffLogic>(3);//增益BUFF列表
    public List<NewBuffLogic> NoneBuffList = new List<NewBuffLogic>();//无状态Buff列表
    /// <summary>
    /// 技能数组
    /// </summary>
    protected List<FightUnitSkill> mPassSkillArr=> HeroData.m_PassSkillList;
    protected List<FightUnitSkill> mActiveSkillArr => HeroData.m_ActiveSkillList;
    //当前我自身的回合数
    protected int RoundID;
    public int TargetSeatID;//本回合点击目标单位ID
    public int MaxBUFFCount=3;//减益和增益buff列表最大数量

    public FightUnitLogic(FightUnitData heroData, FightUnitTeamEnum heroTeam)
    {
        HeroData = heroData;
        Name=heroData.Name;
        WeaponType=heroData.WeaponType;
        UnitTeam = heroTeam;
        MAXHP = heroData.MaxHp;
        MAXMP = heroData.MaxMp;
        hp = heroData.Hp;
        mp=heroData.Mp;
        meleeak = heroData.MeleeAk;
        rangeak=heroData.RangeAk;
        magicak=heroData.MagicAk;
        medef = heroData.MeDEF;
        rgdef = heroData.MeDEF;
        mgdef=heroData.MGDEF;
        elmres = heroData.ELMRES;
        curseMgRES = heroData.CurseMgRES;
        shield=heroData.Shield;
        physicalHit=heroData.PhysicalHit;
        eleMagicHit=heroData.EleMagicHit;
        curseMagicHit=heroData.CurseMagicHit;
        magicPenetration=heroData.MagicPenetration;
        evade=heroData.Evade;
        speed=heroData.Speed;
        criticalHit=heroData.CriticalHit;
        MixDamage=heroData.MixDamage;
        MaxDamage=heroData.MaxDamage;
        tough=heroData.Tough;
        armorBreakingAT=heroData.ArmorBreakingAT;

        RoundID = 0;//重置回合数

    }
    public override void OnCreate()
    {
        base.OnCreate();
#if RENDER_LOGIC
        //HeroRender.UpdateAnger_HUD(rage.RawFloat);
#endif
    }
    public override void BeginAction(bool isAutoSkill, UnitActionEnum unitActionEnum)
    {
        base.BeginAction(isAutoSkill, unitActionEnum);
        //如果处于被控状态中，跳过当前回合动作
        //先看有没有负面BUFF,这里负面BUFF优先级是定身最高,有定身和混乱的前提下会优先定身
        if (objectState == LogicObjectState.Death|| GetBuffTypeByEnum(NewBuffType.IMMOBILIZE))
        {
            ActionEnd();
            return;
        }
        else if(GetBuffTypeByEnum(NewBuffType.CHAOS))
        {
            //混乱中,全屏目标随机一个
            List<FightUnitLogic> attackList = NewBattleWorld.Instance.heroLogic.GetHeroListByTeam(FightUnitTeamEnum.ALL);
            attackList = NewBattleRule.GetHeroSurvivalList(attackList);
            if (attackList.Count>0)
            {
                int Index=LogicRandom.Instance.Range(0, attackList.Count - 1);
                CauseDamageFightUnitList(attackList[Index], (SkillReleaseType)WeaponType);
            }
            else
            {
                Log.Error("一个存活的也没有了");
            }
            ActionEnd();
            return;
        }
        bool SkillSilent = GetBuffTypeByEnum(NewBuffType.SKILL_SILENT);
        if (!isAutoSkill)
        {
            //说明还是自动技能回合,并且没有被封魔的情况下
            if (mPassSkillArr.Count>0&& !SkillSilent)
            {
                var SkillData = mPassSkillArr[RoundID % mPassSkillArr.Count];
                NewSkillManager.Instance.ReleaseSkill(SkillData, this);
            }
            else
            {
                if (SkillSilent)
                {
                    //被封魔了
                }
                ActionEnd();
            }
        }
        else
        {
            //已经到主动回合了
            switch (unitActionEnum)
            {
                case UnitActionEnum.NormalAttack://普通攻击
                    List<FightUnitLogic> logicslist = NewBattleRule.GetAttackListByAttackType(SkillTarget.ALL, SkillRadiusType.SOLO,this);
                    if (logicslist.Count>0)
                    {
                        CauseDamageFightUnitList(logicslist[0], (SkillReleaseType)WeaponType);
                    }
                    else
                    {
                        Log.Error("一个存活的也没有了");
                    }
                    ActionEnd();
                    break;
                case UnitActionEnum.Skill://释放技能
                    if (!SkillSilent)
                    {
                        var SkillData = mActiveSkillArr[RoundID % mActiveSkillArr.Count];
                        if (Name== "魔法女巫")
                        {
                            Log.Info($"回合数:{RoundID},数量:{mActiveSkillArr.Count},结果:{RoundID % mActiveSkillArr.Count}");
                        }
                        NewSkillManager.Instance.ReleaseSkill(SkillData, this);
                    }
                    else
                    {
                        //被封魔了
                        ActionEnd();
                    }
                    break;
                case UnitActionEnum.Defense://防御
                    ActionEnd();
                    break;
                case UnitActionEnum.Escape://逃跑
                    ActionEnd();
                    break;
            }
        }
        
#if RENDER_LOGIC
        //更新怒气条
        //float rate = (rage / MaxRage).RawFloat;
        //HeroRender.UpdateAnger_HUD(rate);
#endif
    }
    /// <summary>
    /// 对目标施展普通伤害
    /// </summary>
    /// <returns></returns>
    void CauseDamageFightUnitList(FightUnitLogic fightUnitLogic, SkillReleaseType AttackType)
    {
        BeAttackEnum beAttack = NewBattleDataCalculatConter.IsCanBeAttack(AttackType, this, fightUnitLogic);
        int damage;
        switch (beAttack)
        {
            case BeAttackEnum.MeleeATK:
                damage = NewBattleDataCalculatConter.CalculatDamage(beAttack, this, fightUnitLogic);
                fightUnitLogic.DamageHP(damage);
                break;
            case BeAttackEnum.RangeATK:
                damage = NewBattleDataCalculatConter.CalculatDamage(beAttack, this, fightUnitLogic);
                fightUnitLogic.DamageHP(damage);
                break;
            case BeAttackEnum.MAGATK:
                damage = NewBattleDataCalculatConter.CalculatDamage(beAttack, this, fightUnitLogic);
                fightUnitLogic.DamageHP(damage);
                break;
            case BeAttackEnum.CURSEATK:
                break;
            case BeAttackEnum.Invalid:
                //无敌状态(无敌,物理无敌,魔法无敌)
                fightUnitLogic.BeInvalidByAttack();
                break;
            case BeAttackEnum.Evade:
                //被闪避了
                fightUnitLogic.BeEvade();
                break;
            case BeAttackEnum.ELEMagicPenetrationAttack://元素魔法穿透
                damage = NewBattleDataCalculatConter.CalculatDamage(beAttack, this,fightUnitLogic);
                fightUnitLogic.DamageHP(damage);
                break;
            case BeAttackEnum.CURSEMagicPenetrationAttack://诅咒魔法穿透
                break;
        }
    }
    /// <summary>
    /// 该单位行动结束
    /// </summary>
    public override void ActionEnd()
    {
        base.ActionEnd();
        OnActionEndListener?.Invoke();
    }

    public override void RoundStartEvent(int round)
    {
        base.RoundStartEvent(round);
        RoundID++;
        for (int i = 0; i < NoneBuffList.Count; i++)
        {
            NoneBuffList[i].RoundStartEvent(round);
        }
        for (int i = 0; i < DeBuffList.Count; i++)
        {
            DeBuffList[i].RoundStartEvent(round);
        }
        for (int i = 0; i < BuffList.Count; i++)
        {
            BuffList[i].RoundStartEvent(round);
        }

    }
    public override void RoundEndEvent()
    {
        base.RoundEndEvent();
        for (int i = 0; i < NoneBuffList.Count; i++)
        {
            NoneBuffList[i].RoundEndEvent();
        }
        for (int i = 0; i < BuffList.Count; i++)
        {
            BuffList[i].RoundEndEvent();
        }
        for (int i = 0; i < DeBuffList.Count; i++)
        {
            DeBuffList[i].RoundEndEvent();
        }
    }
    #region 技能相关API
    /// <summary>
    /// 添加buff
    /// </summary>
    /// <param name="buff"></param>
    public void Addbuff(NewBuffLogic buff)
    {
        Log.Info($"添加的BuffID:{buff.Buffid},被添加的战斗单位是---{HeroData.Name}---ID:{HeroData.ID}");
        int buffid = buff.Buffid;
        //如果增益buff或者减益Buff最大的叠加次数已经达到，那么就移除进队列
        switch (buff.BuffConfig.buffState)
        {
            case NewBuffState.None:
                NoneBuffList.Add(buff);
                break;
            case NewBuffState.Buff:
                if (BuffList.Count==MaxBUFFCount)
                {
                    //已经最大值移除最后一个
                    BuffList.RemoveAt(MaxBUFFCount-1);
                }
                BuffList.Insert(0, buff);
                break;
            case NewBuffState.DeBuff:
                if (DeBuffList.Count == MaxBUFFCount)
                {
                    //已经最大值移除最后一个
                    DeBuffList.RemoveAt(MaxBUFFCount - 1);
                }
                DeBuffList.Insert(0, buff);
                break;
        }
        //if (buff.BuffConfig.maxStackingNum >= 1)
        //{
        //    int count = 0;
        //    for (int i = 0; i < haveBuffList.Count; i++)
        //    {
        //        if (haveBuffList[i].Buffid == buffid)
        //        {
        //            count++;
        //        }
        //    }
        //    if (count >= buff.BuffConfig.maxStackingNum)
        //    {
        //        Log.Info($"buff以达到最大叠加个数 buffid{buffid},BUFF名称:{buff.BuffConfig.buffName}");
        //        return;
        //    }
        //    haveBuffList.Add(buff);
        //}
        //else
        //    haveBuffList.Add(buff);
#if RENDER_LOGIC
        //增益buff或减益buff都有buff图标
        //if (buff.BuffConfig.buffType != BuffType.DamageBuff)
        //{
        //    HeroRender.Add_BuffIcon(buff.BuffConfig);
        //}
#endif
    }
    public void RemoveBuff(NewBuffLogic buff)
    {
        switch (buff.BuffConfig.buffState)
        {
            case NewBuffState.None:
                NoneBuffList.Remove(buff);
                break;
            case NewBuffState.Buff:
                BuffList.Remove(buff);
                break;
            case NewBuffState.DeBuff:
                DeBuffList.Remove(buff);
                break;
        }
        
#if RENDER_LOGIC
        //HeroRender.Remove_BuffIcon(buff.BuffConfig.buffIcon);
#endif
    }

    //获取指定buff的个数
    public int GetBuffCount(int buffid)
    {
        int buffCount = 0;
        //for (int i = 0; i < haveBuffList.Count; i++)
        //{
        //    if (haveBuffList[i].Buffid == buffid)
        //    {
        //        buffCount++;
        //    }
        //}
        return buffCount;
    }
    /// <summary>
    /// 查看是否有某种Buff状态类型
    /// </summary>
    /// <returns></returns>
    public bool GetBuffTypeByEnum(NewBuffType BuffType)
    {
        bool buffHas=false;
        switch (BuffBattleRule.GetBuffStateByBuffType(BuffType))
        {
            case NewBuffState.None:
                for (int i = 0; i < NoneBuffList.Count; i++)
                {
                    if (NoneBuffList[i].BuffConfig.buffType== BuffType)
                    {
                        buffHas = true;
                        break;
                    }
                }
                break;
            case NewBuffState.DeBuff:
                for (int i = 0; i < DeBuffList.Count; i++)
                {
                    if (DeBuffList[i].BuffConfig.buffType == BuffType)
                    {
                        buffHas = true;
                        break;
                    }
                }
                break;
            case NewBuffState.Buff:
                for (int i = 0; i < BuffList.Count; i++)
                {
                    if (BuffList[i].BuffConfig.buffType == BuffType)
                    {
                        buffHas = true;
                        break;
                    }
                }
                break;
        }
        return buffHas;
    }
    /// <summary>
    /// 刷新buff持续回合
    /// </summary>
    //public void RefershtBuffDurationRound(int buffid)
    //{
    //    for (int i = 0; i < NoneBuffList.Count; i++)
    //    {
    //        if (NoneBuffList[i].Buffid == buffid)
    //        {
    //            NoneBuffList[i].ResetBuffSurvivalRoundCount();
    //        }
    //    }
    //}
    public void ClearBuff()
    {
        for (int i = NoneBuffList.Count - 1; i >= 0; i--)
        {
            NoneBuffList[i].OnDestroy();
        }
        for (int i = BuffList.Count - 1; i >= 0; i--)
        {
            BuffList[i].OnDestroy();
        }
        for (int i = DeBuffList.Count - 1; i >= 0; i--)
        {
            DeBuffList[i].OnDestroy();
        }
    }

    /// <summary>
    /// 行动结束
    /// </summary>
    public void OnMoveActionEnd()
    {
        for (int i = 0; i < NoneBuffList.Count; i++)
        {
            NoneBuffList[i].ActionEndEvent();
        }
        for (int i = 0; i < BuffList.Count; i++)
        {
            BuffList[i].ActionEndEvent();
        }
        for (int i = 0; i < DeBuffList.Count; i++)
        {
            DeBuffList[i].ActionEndEvent();
        }
        ActionEnd();
    }
    #endregion

    #region 状态相关
    /// <summary>
    /// 是否被控制
    /// </summary>
    /// <returns></returns>
    public bool IsBeContrl()
    {
        //for (int i = 0; i < DeBuffList.Count; i++)
        //{
        //    if (DeBuffList[i].BuffConfig.buffState)
        //    {
        //        Debug.Log("BeContrl objectState:" + haveBuffList[i].objectState + "  SurvivalRound:" + haveBuffList[i].mCurBuffSurvivalRoundCount);
        //        return true;
        //    }
        //}
        return false;
    }
    #endregion

    public void AttackUpdateAnger()
    {
        //UpdateAnger(HeroData.atkRange);
    }
    public void UpdateAnger(VInt anger)
    {
//        //更新怒气值
//        if (Rage >= MaxRage)
//            rage = MaxRage;
//        rage += HeroData.atkRange;

//#if RENDER_LOGIC
//        //更新怒气条
//        float rate = (rage / MaxRage).RawFloat;
//        //HeroRender.UpdateAnger_HUD(rate);
//#endif
    }
    /// <summary>
    ///受伤回怒
    /// </summary>
    public void TakeDamageRage()
    {
        //UpdateAnger(HeroData.atkRange);
    }
    public void BuffDamage(int hp, BuffConfig buffCfg)
    {
        DamageHP(hp);
    }
    /// <summary>
    /// 属性值增删
    /// </summary>
    public void AttriAddBuff(BUFFATKType BuffState,int Value,bool isPercent)
    {
        int RealValue;
        switch (BuffState)
        {
            case BUFFATKType.HP:
                //先看是不是百分比,再看是否是增删
                if (isPercent)
                {
                    //血量百分比只能是最大值
                    RealValue= MAXHP * (int)((float)Value / 100);
                }
                else
                {
                    //非百分比
                    RealValue = (int)Value;
                }
                if (Value < 0)
                {
                    //删
                    RealValue *= -1;
                }
                DamageHP(RealValue);
                break;
            case BUFFATKType.MP:
                if (isPercent)
                {
                    //血量百分比只能是最大值
                    RealValue = MAXMP * (int)((float)Value / 100);
                }
                else
                {
                    //非百分比
                    RealValue = (int)Value;
                }
                if (Value < 0)
                {
                    //删
                    RealValue *= -1;
                }
                DamageMP(RealValue);
                break;
            case BUFFATKType.MEATK:
                if (isPercent)
                {
                    RealValue = meleeak * (int)((float)Value / 100);
                }
                else
                {
                    //非百分比
                    RealValue = (int)Value;
                }
                if (Value > 0)
                {
                    //增
                    meleeak += RealValue;
                }
                else
                {
                    //删
                    meleeak -= RealValue;
                    if (meleeak < 0)
                    {
                        meleeak = 0;
                    }
                }
                break;
            case BUFFATKType.MAGATK:
                if (isPercent)
                {
                    RealValue = magicak * (int)((float)Value / 100);
                }
                else
                {
                    //非百分比
                    RealValue = (int)Value;
                }
                if (Value > 0)
                {
                    //增
                    magicak += RealValue;
                }
                else
                {
                    //删
                    magicak -= RealValue;
                    if (magicak < 0)
                    {
                        magicak = 0;
                    }
                }
                break;
            case BUFFATKType.MEDFS:
                if (isPercent)
                {
                    RealValue = medef * (int)((float)Value / 100);
                }
                else
                {
                    //非百分比
                    RealValue = (int)Value;
                }
                if (Value > 0)
                {
                    //增
                    medef += RealValue;
                }
                else
                {
                    //删
                    medef -= RealValue;
                    if (medef < 0)
                    {
                        medef = 0;
                    }
                }
                break;
            case BUFFATKType.MAGDFS:
                if (isPercent)
                {
                    RealValue = mgdef * (int)((float)Value / 100);
                }
                else
                {
                    //非百分比
                    RealValue = (int)Value;
                }
                if (Value > 0)
                {
                    //增
                    mgdef += RealValue;
                }
                else
                {
                    //删
                    mgdef -= RealValue;
                    if (mgdef < 0)
                    {
                        mgdef = 0;
                    }
                }
                break;
            case BUFFATKType.ELMRES:
                if (isPercent)
                {
                    RealValue = elmres * (int)((float)Value / 100);
                }
                else
                {
                    //非百分比
                    RealValue = Value;
                }
                if (Value > 0)
                {
                    //增
                    elmres += RealValue;
                }
                else
                {
                    //删
                    elmres -= RealValue;
                    if (elmres < 0)
                    {
                        elmres = 0;
                    }
                }
                break;
            case BUFFATKType.CURSERES:
                if (isPercent)
                {
                    RealValue = curseMgRES * (int)((float)Value / 100);
                }
                else
                {
                    //非百分比
                    RealValue = Value;
                }
                if (Value > 0)
                {
                    //增
                    curseMgRES += RealValue;
                }
                else
                {
                    //删
                    curseMgRES -= RealValue;
                    if (curseMgRES < 0)
                    {
                        curseMgRES = 0;
                    }
                }
                break;
            case BUFFATKType.PHYHIT:
                if (isPercent)
                {
                    RealValue = physicalHit * (int)((float)Value / 100);
                }
                else
                {
                    //非百分比
                    RealValue = Value;
                }
                if (Value > 0)
                {
                    //增
                    physicalHit += RealValue;
                }
                else
                {
                    //删
                    physicalHit -= RealValue;
                    if (physicalHit < 0)
                    {
                        physicalHit = 0;
                    }
                }
                break;
            case BUFFATKType.ELMHIT:
                if (isPercent)
                {
                    RealValue = eleMagicHit * (int)((float)Value / 100);
                }
                else
                {
                    //非百分比
                    RealValue = Value;
                }
                if (Value > 0)
                {
                    //增
                    eleMagicHit += RealValue;
                }
                else
                {
                    //删
                    eleMagicHit -= RealValue;
                    if (eleMagicHit < 0)
                    {
                        eleMagicHit = 0;
                    }
                }
                break;
            case BUFFATKType.CURSEHIT:
                if (isPercent)
                {
                    RealValue = curseMagicHit * (int)((float)Value / 100);
                }
                else
                {
                    //非百分比
                    RealValue = Value;
                }
                if (Value > 0)
                {
                    //增
                    curseMagicHit += RealValue;
                }
                else
                {
                    //删
                    curseMagicHit -= RealValue;
                    if (curseMagicHit < 0)
                    {
                        curseMagicHit = 0;
                    }
                }
                break;
            case BUFFATKType.MagicPenetration:
                if (isPercent)
                {
                    RealValue = magicPenetration * (int)((float)Value / 100);
                }
                else
                {
                    //非百分比
                    RealValue = Value;
                }
                if (Value > 0)
                {
                    //增
                    magicPenetration += RealValue;
                    if (magicPenetration>500)
                    {
                        magicPenetration = 500;
                    }
                }
                else
                {
                    //删
                    magicPenetration -= RealValue;
                    if (magicPenetration < 0)
                    {
                        magicPenetration = 0;
                    }
                }
                break;
            case BUFFATKType.EVADE:
                if (isPercent)
                {
                    RealValue = evade * (int)((float)Value / 100);
                }
                else
                {
                    //非百分比
                    RealValue = Value;
                }
                if (Value > 0)
                {
                    //增
                    evade += RealValue;
                }
                else
                {
                    //删
                    evade -= RealValue;
                    if (evade < 0)
                    {
                        evade = 0;
                    }
                }
                break;
            case BUFFATKType.SPEED:
                if (isPercent)
                {
                    RealValue = speed * (int)((float)Value / 100);
                }
                else
                {
                    //非百分比
                    RealValue = Value;
                }
                if (Value > 0)
                {
                    //增
                    speed += RealValue;
                }
                else
                {
                    //删
                    speed -= RealValue;
                    if (speed < 0)
                    {
                        speed = 0;
                    }
                }
                break;
            case BUFFATKType.CRITHIT:
                if (isPercent)
                {
                    RealValue = criticalHit * (int)((float)Value / 100);
                }
                else
                {
                    //非百分比
                    RealValue = Value;
                }
                if (Value > 0)
                {
                    //增
                    criticalHit += RealValue;
                }
                else
                {
                    //删
                    criticalHit -= RealValue;
                    if (criticalHit < 0)
                    {
                        criticalHit = 0;
                    }
                }
                break;
            case BUFFATKType.TOUGH:
                if (isPercent)
                {
                    RealValue = tough * (int)((float)Value / 100);
                }
                else
                {
                    //非百分比
                    RealValue = Value;
                }
                if (Value > 0)
                {
                    //增
                    tough += RealValue;
                }
                else
                {
                    //删
                    tough -= RealValue;
                    if (tough < 0)
                    {
                        tough = 0;
                    }
                }
                break;
            case BUFFATKType.ARMRBK:
                if (isPercent)
                {
                    RealValue = armorBreakingAT * (int)((float)Value / 100);
                }
                else
                {
                    //非百分比
                    RealValue = Value;
                }
                if (Value > 0)
                {
                    //增
                    armorBreakingAT += RealValue;
                }
                else
                {
                    //删
                    armorBreakingAT -= RealValue;
                    if (armorBreakingAT < 0)
                    {
                        armorBreakingAT = 0;
                    }
                }
                break;
        }
    }
    /// <summary>
    /// 血量增删专用
    /// </summary>
    public void DamageHP(int damagehp)
    {
        
        if (damagehp==0)
        {
            return;
        }
        hp += damagehp;
        if (damagehp>0)
        {
            //增
            if (hp > MAXHP)
            {
                hp = MAXHP;
            }
            Log.Info($"战斗单位:{Name}恢复HP:{damagehp}");
        }
        else
        {
            Log.Info($"战斗单位:{Name}受到的伤害是:{damagehp}");
            //删
            BeAttack();
            if (hp < 0)
            {
                hp = 0;
                HeroDeath();
            }
        }
        Log.Info($"战斗单位:{HeroData.Name},剩余血量:{hp}");
#if RENDER_LOGIC
        //把伤害数值、血量的百分比传给渲染层 更新渲染数据
        float hpValue = (float)hp /(float)MAXHP;
        HeroRender.UpdateHP_HUD(damagehp,hp, hpValue);
#endif
    }
    /// <summary>
    /// 法力增删专用
    /// </summary>
    public void DamageMP(int damagehp)
    {
        if (damagehp == 0)
        {
            return;
        }
        if (damagehp > 0)
        {
            //增
            mp += damagehp;
            if (mp > MAXMP)
            {
                mp = MAXMP;
            }
        }
        else
        {
            //删
            mp -= damagehp;
            BeAttack();
            if (mp < 0)
            {
                mp = 0;
            }
        }
    }
    /// <summary>
    /// 被闪避
    /// </summary>
    public void BeEvade()
    {
        HeroRender.PlayUITips(3);
    }
    /// <summary>
    /// 被攻击
    /// </summary>
    public void BeAttack()
    {
        PlayAnim("Hurt",null);
    }
    /// <summary>
    /// 被攻击无效
    /// </summary>
    public void BeInvalidByAttack()
    {
    }
    public void HeroDeath()
    {

        objectState = LogicObjectState.Death;
        List<FightUnitLogic> heroLogicList = NewBattleWorld.Instance.heroLogic.AllHeroList;
        int heroSvlCount = 0;
        foreach (var item in heroLogicList)
        {
            if (item.objectState == LogicObjectState.Survival)
            {
                heroSvlCount++;
            }
        }
        Log.Error($"战斗单位:{HeroData.Name},ID:{HeroData.ID},剩余存活个数:{heroSvlCount}");
#if RENDER_LOGIC
        HeroRender.HeroDeath();
        HeroRender.PlayDeath();
#endif
        ClearBuff();
    }
    public void PlayAnim(string animName,Action AnimFinishAction=null)
    {
#if RENDER_LOGIC
        HeroRender.PlayAnim(animName, AnimFinishAction);
#endif
    }
    /// <summary>
    /// 新播放动画
    /// </summary>
    /// <param name="animName"></param>
    public void NewPlayAnim(string animName,Action DamageAction)
    {
        HeroRender.NewPlayAnim(animName,DamageAction);
    }
    public void SetAnimState(AnimState state)
    {
#if RENDER_LOGIC
        HeroRender.SetAnimState(state);
#endif
    }
}
