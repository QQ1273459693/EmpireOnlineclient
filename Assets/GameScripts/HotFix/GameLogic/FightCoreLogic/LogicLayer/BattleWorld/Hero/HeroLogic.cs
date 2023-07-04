using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroLogic : LogicObject
{
    protected VInt hp;
    protected VInt atk;
    protected VInt def;
    protected VInt agl;
    protected VInt rage;
    public VInt Hp { get { return hp; } }
    public VInt MaxHp { get; protected set; }
    public VInt ATK { get { return atk; } }//攻击力
    public VInt Def { get { return def; } }//防御力 护甲
    public VInt Agl { get { return agl; } }//敏捷值
    public VInt Rage { get { return rage; } }//怒气值
    public VInt MaxRage { get { return 100; } }//最大怒气值 100说明可以放弃技能
    public int id=>HeroData.id;
    public HeroData HeroData { get; private set; }
    public HeroRender HeroRender { get; private set; }
    public HeroTeamEnum HeroTeam { get; private set; }
    public HeroLogic(HeroData heroData,HeroTeamEnum heroTeam)
    {
        HeroData=heroData;
        HeroTeam=heroTeam;
        hp=HeroData.hp;
        atk = HeroData.atk;
        def = HeroData.def;
        agl = HeroData.agle;
        MaxHp = hp;
        rage = 0;
    }
    public override void OnCreate()
    {
        base.OnCreate();
        HeroRender = (HeroRender)RenderObj;
        UpdateAnger(rage);
        Debuger.Log("英雄名字:"+RenderObj.gameObject.name);
    }
    public void PlayAnim(string animName)
    {
#if RENDER_LOGIC
        HeroRender.PlayAnim(animName);
#endif
    }
    public override void OnLogicFrameUpdate()
    {
        base.OnLogicFrameUpdate();
    }
    public void TryClearRage()
    {
        rage = 0;
    }
    /// <summary>
    /// 怒气值处理
    /// </summary>
    public void UpdateAnger(VInt anger)
    {
        if (Rage >= MaxRage)
        {
            this.rage = MaxRage;
        }
        this.rage += anger;
#if CLIENT_LOGIC
        //计算怒气比率
        float rate = (float)(rage / MaxRage).RawFloat;
        HeroRender.UpdateAnger_HUD(rate);
#endif
    }
    /// <summary>
    /// 开始行动,轮到该英雄攻击时,这个接口就会触发
    /// </summary>
    public override void BeginAction()
    {
        base.BeginAction();
        if (objectState== LogicObjectState.Death)
        {
            //已经死亡
            ActionEnd();
            return;
        }
        //判断当前英雄怒气值是否大于100,如果大于100 就说明可以释放技能了
        //否则就只能进行普通攻击
        bool isNormalAttack = Rage < MaxRage;
        if (Rage>MaxRage)
        {
            rage = 0;
        }
        int skillid = isNormalAttack ? HeroData.skillidArr[0] : HeroData.skillidArr[1];

        SkillManager.Instance.ReleaseSkill(skillid,this,isNormalAttack);
        UpdateAnger(0);
    }
    /// <summary>
    /// 英雄攻击结束
    /// </summary>
    public override void ActionEnd()
    {
        base.ActionEnd();
        OnActionEndListener?.Invoke();
    }
    /// <summary>
    /// 损失血量
    /// </summary>
    public void DamageHp(VInt damagehp,BuffConfig buffConfig=null)
    {
        if (damagehp==0)
        {
            return;
        }
        hp-=damagehp;
        if (hp<=0)
        {
            hp = 0;
            //英雄死亡
            HeroDeath();
            return;
        }
        else
        {
            if (damagehp>0)
            {
                //回血
                PlayAnim("BeAttack");
            }
            
        }
        Debuger.Log("英雄ID:" +id+"英雄损失血量:"+damagehp+"英雄剩余血量:"+hp);
        //把伤害数值,血量百分比传给渲染层,更新渲染数据
#if RENDER_LOGIC
        float hpValue = hp.RawInt;//hp.RawFloat / MaxHp.RawFloat;
        HeroRender.UpdateHp_Hud(damagehp.RawInt,hpValue);
#endif
    }
    public void HeroDeath()
    {
        objectState = LogicObjectState.Death;
#if RENDER_LOGIC
        HeroRender.HeroDeath();
#endif
        Debuger.Log("英雄死亡");
    }
    public void BuffDamage(VInt hp,BuffConfig buffConfig)
    {
        Debuger.Log("Buff伤害:"+hp,1);
        DamageHp(hp,buffConfig);
    }
    public void RemoveBuff(BuffLogic buff)
    {

    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        OnActionEndListener = null;
        HeroRender.OnRelease();
#if RENDER_LOGIC
        HeroRender.OnRelease();
#endif
    }
}
