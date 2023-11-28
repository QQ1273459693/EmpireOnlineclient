using System;
using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;
public enum NewAnimState
{
    StopAnim,
    RePlayAnim,
}

public enum NewHeroTeamEnum
{
    None,
    Self,
    Enemy,
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
    protected VInt hp;
    protected VInt mp;
    protected VInt meleeak;
    protected VInt rangeak;
    protected VInt magicak;
    protected VInt medef;
    protected VInt rgdef;
    protected VInt mgdef;
    protected VInt eLMRES;
    protected VInt curseMgRES;
    protected VInt shield;
    protected VInt physicalHit;
    protected VInt eleMagicHit;
    protected VInt curseMagicHit;
    protected VInt magicPenetration;
    protected VInt evade;
    protected VInt speed;
    protected VInt criticalHit;
    protected VInt tough;
    protected VInt armorBreakingAT;

    public VInt HP { get { return hp; } }
    public VInt MAXHP { get; protected set; }
    public VInt MP { get { return mp; } }
    public VInt MAXMP { get; protected set; }
    public VInt MeleeAk { get { return meleeak; } }
    public VInt RangeAk { get { return rangeak; } }
    public VInt MagicAk { get { return magicak; } }
    public VInt MeDEF { get { return medef; } }
    public VInt RGDEF { get { return rgdef; } }
    public VInt MGDEF { get { return mgdef; } }
    public VInt ELMRES { get { return eLMRES; } }
    public VInt CurseMgRES { get { return curseMgRES; } }
    public VInt Shield { get { return shield; } }
    public VInt PhysicalHit { get { return physicalHit; } }
    public VInt EleMagicHit { get { return eleMagicHit; } }
    public VInt CurseMagicHit { get { return curseMagicHit; } }
    public VInt MagicPenetration { get { return magicPenetration; } }
    public VInt Evade { get { return evade; } }
    public VInt Speed { get { return speed; } }
    public VInt CriticalHit { get { return criticalHit; } }
    public VInt MixDamage { get; protected set; }
    public VInt MaxDamage { get; protected set; }
    public VInt Tough { get { return tough; } }
    public VInt ArmorBreakingAT { get { return armorBreakingAT; } }
    public int ID => HeroData.ID;
    public FightUnitData HeroData { get; private set; }
    public HeroTeamEnum HeroTeam { get; private set; }
#if RENDER_LOGIC
    public FightUnitRender HeroRender { get { return (FightUnitRender)RenderObj; } }
#endif

    public List<NewBuffLogic> haveBuffList = new List<NewBuffLogic>();
    /// <summary>
    /// 技能数组
    /// </summary>
    protected List<FightUnitSkill> mPassSkillArr=> HeroData.m_PassSkillList;
    protected List<FightUnitSkill> mActiveSkillArr => HeroData.m_ActiveSkillList;
    //当前我自身的回合数
    
    protected int RoundID;


    public FightUnitLogic(FightUnitData heroData, HeroTeamEnum heroTeam)
    {
        HeroData = heroData;
        HeroTeam = heroTeam;
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
        eLMRES=heroData.ELMRES;
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
        if (objectState == LogicObjectState.Death || IsBeContrl())
        {
            //Debuger.Log("Hero Be Conterl No Release Skill Heroid:" + id + "roundid:" + CurRound);
            ActionEnd();
            return;
        }
        if (!isAutoSkill)
        {
            //说明还是自动技能回合
            if (mPassSkillArr.Count>0)
            {
                var SkillData = mPassSkillArr[RoundID % mPassSkillArr.Count];
                NewSkillManager.Instance.ReleaseSkill(SkillData, this);
            }
        }
        else
        {
            //已经到主动回合了
            switch (unitActionEnum)
            {
                case UnitActionEnum.NormalAttack://普通攻击
                    break;
                case UnitActionEnum.Skill://释放技能
                    var SkillData = mActiveSkillArr[RoundID % mActiveSkillArr.Count];
                    NewSkillManager.Instance.ReleaseSkill(SkillData, this);
                    break;
                case UnitActionEnum.Defense://防御
                    break;
                case UnitActionEnum.Escape://逃跑
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
    /// 该单位行动结束
    /// </summary>
    public override void ActionEnd()
    {
        base.ActionEnd();
        RoundID++;
        OnActionEndListener?.Invoke();
    }

    public override void RoundStartEvent(int round)
    {
        base.RoundStartEvent(round);
        for (int i = 0; i < haveBuffList.Count; i++)
        {
            haveBuffList[i].RoundStartEvent(round);
        }
    }
    public override void RoundEndEvent()
    {
        base.RoundEndEvent();
        for (int i = 0; i < haveBuffList.Count; i++)
        {
            haveBuffList[i].RoundEndEvent();
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
        //如果buff最大的叠加次数已经达到，那么就不进行叠加
        if (buff.BuffConfig.maxStackingNum >= 1)
        {
            int count = 0;
            for (int i = 0; i < haveBuffList.Count; i++)
            {
                if (haveBuffList[i].Buffid == buffid)
                {
                    count++;
                }
            }
            if (count >= buff.BuffConfig.maxStackingNum)
            {
                Log.Info($"buff以达到最大叠加个数 buffid{buffid},BUFF名称:{buff.BuffConfig.buffName}");
                return;
            }
            haveBuffList.Add(buff);
        }
        else
            haveBuffList.Add(buff);
#if RENDER_LOGIC
        //增益buff或减益buff都有buff图标
        if (buff.BuffConfig.buffType != BuffType.DamageBuff)
        {
            HeroRender.Add_BuffIcon(buff.BuffConfig);
        }
#endif
    }
    public void RemoveBuff(NewBuffLogic buff)
    {
        haveBuffList.Remove(buff);
#if RENDER_LOGIC
        HeroRender.Remove_BuffIcon(buff.BuffConfig.buffIcon);
#endif
    }

    //获取指定buff的个数
    public int GetBuffCount(int buffid)
    {
        int buffCount = 0;
        for (int i = 0; i < haveBuffList.Count; i++)
        {
            if (haveBuffList[i].Buffid == buffid)
            {
                buffCount++;
            }
        }
        return buffCount;
    }
    /// <summary>
    /// 刷新buff持续回合
    /// </summary>
    public void RefershtBuffDurationRound(int buffid)
    {
        for (int i = 0; i < haveBuffList.Count; i++)
        {
            if (haveBuffList[i].Buffid == buffid)
            {
                haveBuffList[i].ResetBuffSurvivalRoundCount();
            }
        }
    }
    public void ClearBuff()
    {
        for (int i = haveBuffList.Count - 1; i >= 0; i--)
        {
            haveBuffList[i].OnDestroy();
        }
    }
    /// <summary>
    /// 行动结束
    /// </summary>
    public void OnMoveActionEnd()
    {
        for (int i = 0; i < haveBuffList.Count; i++)
        {
            haveBuffList[i].ActionEndEvent();
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
        for (int i = 0; i < haveBuffList.Count; i++)
        {
            if (haveBuffList[i].BuffConfig.buffType == BuffType.Control)
            {
                Debug.Log("BeContrl objectState:" + haveBuffList[i].objectState + "  SurvivalRound:" + haveBuffList[i].mCurBuffSurvivalRoundCount);
                return true;
            }
        }
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
    public void BuffDamage(VInt hp, BuffConfig buffCfg)
    {
        DamageHP(hp, buffCfg);
    }
    public void DamageHP(VInt damagehp, BuffConfig buffCfg = null)
    {
        if (damagehp==0)
        {
            return;
        }

        hp -= damagehp;
        if (hp <= 0)
        {
            hp = 0;
            //英雄死亡
            HeroDeath();
            return;
        }
        else
        {
            //PlayAnim("BeAttack");
        }
        Log.Info($"战斗单位:{HeroData.Name},剩余血量:{hp}");
#if RENDER_LOGIC
        //把伤害数值、血量的百分比传给渲染层 更新渲染数据
        float hpValue = hp.RawFloat / MAXHP.RawFloat;
        HeroRender.UpdateHP_HUD(damagehp.RawInt, hp.RawInt, hpValue, buffCfg);
#endif

    }
    public void HeroDeath()
    {

        objectState = LogicObjectState.Death;
        List<HeroLogic> heroLogicList = BattleWorld.Instance.heroLogic.AllHeroList;
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
#endif
        ClearBuff();
    }
    public void PlayAnim(string animName)
    {
#if RENDER_LOGIC
        HeroRender.PlayAnim(animName);
#endif
    }
    /// <summary>
    /// 新播放动画
    /// </summary>
    /// <param name="animName"></param>
    public void NewPlayAnim(string animName,Vector3 TargetPos,Action DamageAction)
    {
        HeroRender.NewPlayAnim(animName,TargetPos,DamageAction);
    }
    public void SetAnimState(AnimState state)
    {
#if RENDER_LOGIC
        HeroRender.SetAnimState(state);
#endif
    }
}
