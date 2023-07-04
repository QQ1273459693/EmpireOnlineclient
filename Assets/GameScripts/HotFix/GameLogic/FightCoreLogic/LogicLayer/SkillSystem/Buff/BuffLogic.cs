using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffLogic :LogicObject
{
    public BuffConfig BuffConfig { get; private set; }
    public int BuffID { get; private set; }
    protected LogicObject mOwnwer;//buff拥有者
    public HeroLogic mOwnwerHero;//buff释放者
    public HeroLogic targetHero;//buff释放者
    protected LogicObject mAttackeTarget;//buff攻击目标

    private int mCurAccTime;//当前Buff累计生效时间
    private int mCurRealTime;//当前buff实时运行时间

    public BuffLogic(int buffid,LogicObject owner,LogicObject attackerTarget)
    {
        BuffID = buffid;
        mOwnwer = owner;
        mOwnwerHero = owner as HeroLogic;
        mAttackeTarget = attackerTarget;
        targetHero = attackerTarget as HeroLogic;
    }
    public override void OnCreate()
    {
        base.OnCreate();
        objectState = LogicObjectState.Survival;
        BuffConfig = SkillConfigConter.LoadBuffConfig(BuffID);
    }
    public override void OnLogicFrameUpdate()
    {
        base.OnLogicFrameUpdate();
        if (objectState==LogicObjectState.Survival)
        {
            switch (BuffConfig.buffTriggerType)
            {
                case BuffTriggerType.OneDamage_RealTime://一次性 实时性伤害
                    if (BuffConfig.buffDurationTimeMs==0&&BuffConfig.buffTriggerIntervalMs==0)
                    {
                        TriggerBuff();
                        AddBuffAndEffect();
                        //如果是多回合伤害,需要把Buff状态设置成未死亡状态
                        if (BuffConfig.buffDurationRound==0)
                        {
                            objectState = LogicObjectState.Death;
                        }
                        else
                        {
                            objectState = LogicObjectState.SurvivalWiteing;
                        }
                    }
                    else
                    {
                        //延时Buff
                        mCurRealTime += LogicFrameSyncConfig.logicFrameIntervalms;
                        if (mCurRealTime>=BuffConfig.buffTriggerIntervalMs)
                        {
                            TriggerBuff();
                            AddBuffAndEffect();
                            mCurRealTime -= BuffConfig.buffTriggerIntervalMs;
                        }
                        mCurAccTime += LogicFrameSyncConfig.logicFrameIntervalms;
                        if (mCurAccTime>=BuffConfig.buffDurationTimeMs)
                        {
                            objectState = LogicObjectState.Death;
                            break;
                        }
                    }
                    break;
                case BuffTriggerType.MultisegmentDamage_RealTime://即时性 多段伤害
                    if (BuffConfig.buffDurationTimeMs > 0 && BuffConfig.buffTriggerIntervalMs > 0)
                    {
                        mCurRealTime += LogicFrameSyncConfig.logicFrameIntervalms;
                        if (mCurRealTime >= BuffConfig.buffTriggerIntervalMs)
                        {
                            TriggerBuff();
                            AddBuffAndEffect();
                            mCurRealTime -= BuffConfig.buffTriggerIntervalMs;
                        }
                        mCurAccTime += LogicFrameSyncConfig.logicFrameIntervalms;
                        if (mCurAccTime >= BuffConfig.buffDurationTimeMs)
                        {
                            objectState = LogicObjectState.Death;
                            break;
                        }
                    }
                    break;
                case BuffTriggerType.Damage_RoundStart:
                    break;
                case BuffTriggerType.Damage_ActionEnd:
                    break;
            }
        }
    }
    public void TriggerBuff()
    {
        if (BuffConfig.damageType!=BuffDamageType.None)
        {
            VInt damage= BattleRule.CalculaDamage(BuffConfig,(HeroLogic)mOwnwer,(HeroLogic)mAttackeTarget);
            HeroLogic attackTargetHero = (HeroLogic)mAttackeTarget;
            attackTargetHero.BuffDamage(damage,BuffConfig);
        }
    }
    public void AddBuffAndEffect()
    {

    }

    public override void OnDestroy()
    {
        Debuger.Log("Buff销毁:"+BuffID,2);
        objectState = LogicObjectState.Death;
        RenderObj?.OnRelease();
        BuffManager.Instance.DestroyBuff(this);
    }
}
