using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AnimState
{
    StopAnim,
    RePlayAnim,
}

public enum HeroTeamEnum
{
    None,
    Self,
    Enemy,
}
public class HeroLogic : LogicObject
{
    protected VInt hp;
    protected VInt atk;
    protected VInt def;
    protected VInt agl;
    protected VInt rage;
    public VInt HP { get { return hp; } }
    public VInt MAXHP { get; protected set; }
    public VInt ATK { get { return atk; } }
    public VInt Def { get { return def; } }
    public VInt Agl { get { return agl; } }
    public VInt Rage { get { return rage; } }
    public VInt MaxRage { get { return 100; } }
    public int id => HeroData.id;
    public HeroData HeroData { get; private set; }
    public HeroTeamEnum HeroTeam { get; private set; }
#if RENDER_LOGIC
    public HeroRender HeroRender { get { return (HeroRender)RenderObj; } }
#endif

    public List<BuffLogic> haveBuffList = new List<BuffLogic>();
    /// <summary>
    /// 技能数组
    /// </summary>
    protected List<int> mSkillArr => HeroData.skillidArr;
    public HeroLogic(HeroData heroData, HeroTeamEnum heroTeam)
    {
        HeroData = heroData;
        HeroTeam = heroTeam;
        MAXHP = hp = HeroData.hp;
        atk = HeroData.atk;
        def = HeroData.def;
        agl = HeroData.agl;
        rage = 0;
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
            Debuger.Log("Hero Be Conterl No Release Skill Heroid:" + id + "roundid:" + CurRound);
            ActionEnd();
            
            return;
        }
        int skillid = isAutoSkill ? mSkillArr[1] : mSkillArr[0];
        bool isNormalAtk = Rage < MaxRage;
        if (Rage >= MaxRage)
        {
            rage = 0;
          
        }

        SkillManager.Instance.ReleaseSkill(skillid, this, isNormalAtk);
#if RENDER_LOGIC
        //更新怒气条
        float rate = (rage / MaxRage).RawFloat;
        //HeroRender.UpdateAnger_HUD(rate);
#endif
    }
    public override void ActionEnd()
    {
        base.ActionEnd();
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
    public void Addbuff(BuffLogic buff)
    {
        Debug.Log("AddBuff Buffid:" + buff.Buffid + " Hroid:" + HeroData.id);
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
                Debuger.LogError("buff以达到最大叠加个数 buffid：" + buffid + "  buffName:" + buff.BuffConfig.buffName);
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
    public void RemoveBuff(BuffLogic buff)
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
        UpdateAnger(HeroData.atkRange);
    }
    public void UpdateAnger(VInt anger)
    {
        //更新怒气值
        if (Rage >= MaxRage)
            rage = MaxRage;
        rage += HeroData.atkRange;

#if RENDER_LOGIC
        //更新怒气条
        float rate = (rage / MaxRage).RawFloat;
        //HeroRender.UpdateAnger_HUD(rate);
#endif
    }
    /// <summary>
    ///受伤回怒
    /// </summary>
    public void TakeDamageRage()
    {
        UpdateAnger(HeroData.atkRange);
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
            PlayAnim("BeAttack");
        }
        Debuger.Log(id + "英雄剩余血量:" + hp);
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
        Debuger.LogError("GetNomalAttackTarget 英雄死亡 Heroid：" + id + "剩余存活英雄个数：" + heroSvlCount);
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
