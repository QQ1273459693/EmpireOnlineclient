using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;


public class NewBuffLogic : LogicObject
{
    public int Buffid { get; private set; }
    public NewBuffConfig BuffConfig { get; private set; }

    protected LogicObject mOwner;//Buff拥有者
    protected LogicObject mAttacker;//进攻者 添加buff的人
    public FightUnitLogic ownerHero;

    public int mCurBuffSurvivalRoundCount;//当前buff存活的回合
    //private int mCurRealTime;//当前buff生效实时时间
    //private int mCurAccTime;//当前buff累计生效时间
 
    public NewBuffLogic(NewBuffConfig BuffConfig,LogicObject owner,LogicObject attacker)
    {
        //this.Buffid = buffid;
        this.BuffConfig = BuffConfig;
        this.mOwner = owner;
        this.mAttacker = attacker;
        this.ownerHero = (FightUnitLogic)owner;
    }
    public override void OnCreate()
    {
        objectState = LogicObjectState.Survival;
        mCurBuffSurvivalRoundCount = 1;
        //BuffConfig = ResourcesManager.Instance.LoadAsset<BuffConfig>("Buff/" + Buffid);
        //Log.Error($"开始创建Buff:{BuffConfig.buffName}----------------------");
        //伤害类型为回合开始结束的在施加buff的一瞬间就要把buff添加表现出来
        //if (BuffConfig.buffTriggerType == BuffTriggerType.Damage_RoundStart || BuffConfig.buffTriggerType == BuffTriggerType.Damage_ActionEnd)
        //{
        //    AddBuffAndEffect();
        //}

        //自身添加属性buff有点特殊,列如:猎人先攻扰乱射击技能 目标敌方全部,敌方出手减少100,自身技能命中-100,代表的是 本轮大回合内,出手减少100,包括主动技能回合,技能命中减少只在自动回合内,主动回合不触发
        //遇到是自身枚举的BUFF,默认回合结束移除,buff创建立即添加
        //if (BuffConfig.isSelfBuff)
        //{
        //    //自身瞬间buff添加
        //}
        //else
        //{
        //    AddBuffAndEffect();
        //}
        AddBuffAndEffect();




#if CLIENT_LOGIC
#else
        //服务端是没有帧数循环更新这个概念的，所有的逻辑都在一帧跑完  
        OnLogicFrameUpdate();
        if (objectState == LogicObjectState.Death)
        {
            OnDestroy();
            BuffsManager.Instance.RemoveBuff(this);
        } 
#endif
    }
    /// <summary>
    /// 逻辑帧
    /// </summary>
//    public override void OnLogicFrameUpdate()
//    {
//        if (objectState == LogicObjectState.Survival)
//        {
//            switch (BuffConfig.buffTriggerDamageType)
//            {
//                case BuffTriggerDamageType.OneDamage_RealTime:
//                    //判断是否是延时buff
//                    if (BuffConfig.buffDurationTimeMs == 0 && BuffConfig.buffTriggerIntervalMs == 0)
//                    {
//                        TriggerBuff();
//                        AddBuffAndEffect();
//                        if (BuffConfig.buffDurationRound == 0)
//                            objectState = LogicObjectState.Death;
//                        else
//                            objectState = LogicObjectState.SurvialWiteing;
//                    }
//                    else
//                    {
//#if CLIENT_LOGIC
//                        if (mCurAccTime >= BuffConfig.buffDurationTimeMs)
//                        {
//                            objectState = LogicObjectState.Death;
//                            break;
//                        }
//                        //延时buff
//                        mCurRealTime += FrameSyncConfig.LogicFrameLenms;
//                        if (mCurRealTime >= BuffConfig.buffTriggerIntervalMs)
//                        {
//                            TriggerBuff();
//                            AddBuffAndEffect();
//                            mCurRealTime -= BuffConfig.buffTriggerIntervalMs;
//                        }
//                        mCurAccTime += FrameSyncConfig.LogicFrameLenms;
//#else
//                        while (mCurAccTime < BuffConfig.buffDurationTimeMs)
//                        {
//                            //延时buff
//                            mCurRealTime += FrameSyncConfig.LogicFrameLenms;
//                            if (mCurRealTime >= BuffConfig.buffTriggerIntervalMs)
//                            {
//                                TriggerBuff();
//                                AddBuffAndEffect();
//                                mCurRealTime -= BuffConfig.buffTriggerIntervalMs;
//                            }
//                            mCurAccTime += FrameSyncConfig.LogicFrameLenms;
//                        }
//                        if (mCurAccTime >= BuffConfig.buffDurationTimeMs)
//                        {
//                            objectState = LogicObjectState.Death;
//                            break;
//                        }
//#endif


//                    }
//                    break;
//                case BuffTriggerDamageType.MultisegmentDamage_RealTime:
//#if CLIENT_LOGIC
//                    if (BuffConfig.buffDurationTimeMs > 0 && BuffConfig.buffTriggerIntervalMs > 0)
//                    {
//                        if (mCurAccTime >= BuffConfig.buffDurationTimeMs)
//                        {
//                            objectState = LogicObjectState.Death;
//                            break;
//                        }
//                        mCurRealTime += FrameSyncConfig.LogicFrameLenms;
//                        Debuger.Log($"curRealTime:{mCurRealTime}  buffTriggerInterval:{BuffConfig.buffTriggerIntervalMs}");
//                        if (mCurRealTime >= BuffConfig.buffTriggerIntervalMs)
//                        {
//                            TriggerBuff();
//                            mCurRealTime -= BuffConfig.buffTriggerIntervalMs;
//                        }
//                        mCurAccTime += FrameSyncConfig.LogicFrameLenms;
//                    }
//                    break;
//#else
//                  if (BuffConfig.buffDurationTimeMs > 0 && BuffConfig.buffTriggerIntervalMs > 0)
//                    {
//                        while (mCurAccTime < BuffConfig.buffDurationTimeMs)
//                        {
//                            mCurRealTime += FrameSyncConfig.LogicFrameLenms;
//                            Debuger.Log($"curRealTime:{mCurRealTime}  buffTriggerInterval:{BuffConfig.buffTriggerIntervalMs}");
//                            if (mCurRealTime >= BuffConfig.buffTriggerIntervalMs)
//                            {
//                                TriggerBuff();
//                                mCurRealTime -= BuffConfig.buffTriggerIntervalMs;
//                            }
//                            mCurAccTime += FrameSyncConfig.LogicFrameLenms;
//                        }

//                        //if (mCurAccTime >= BuffConfig.buffDurationTimeMs)
//                        //{
//                            objectState = LogicObjectState.Death;
//                        //}

//                    }
//                    break;
//#endif


//            }
//            //if (objectState == LogicObjectState.Death)
//            //{
//            //    OnDestroy();
//            //}
//        }
//    }
    /// <summary>
    /// 触发buff
    /// </summary>
    public void TriggerBuff()
    {
        TriggerBuffDamge();
    }
    /// <summary>
    /// 英雄行动结束时时间
    /// </summary>
    public virtual void ActionEndEvent()
    {
        //if (objectState == LogicObjectState.Survival)
        //{
        //    if (BuffConfig.buffTriggerType == BuffTriggerType.Damage_ActionEnd)
        //        TriggerBuffDamge();
        //}
    }
    /// <summary>
    /// 回合开始时事件
    /// </summary>
    public override void RoundStartEvent(int round)
    {
        base.RoundStartEvent(round);
        //新回合开始，如果当前buff已经超过了最大持续回合，就直接移除掉
        if (objectState == LogicObjectState.Survival || objectState == LogicObjectState.SurvialWiteing)
        {
            if (mCurBuffSurvivalRoundCount > BuffConfig.buffDurationRound)
            {
                //ownerHero.SetAnimState(AnimState.RePlayAnim);
                objectState = LogicObjectState.Death;
                OnDestroy();
            }
        }
        Log.Error($"回合:{round}已经开始,我的buff状态是:{objectState}");
        if (objectState == LogicObjectState.Survival|| objectState == LogicObjectState.SurvialWiteing)
        {
            TriggerBuffDamge();
        }
    }
    /// <summary>
    /// 回合结束事件
    /// </summary>
    public override void RoundEndEvent()
    {
        base.RoundEndEvent();
        mCurBuffSurvivalRoundCount++;
        //Log.Info("BUFF:" + mCurBuffSurvivalRoundCount + " Buffid:" + Buffid + " objectState:" + objectState);
        
        Log.Error($"Buff回合:{mCurBuffSurvivalRoundCount}已经结束,我的buff状态是:{objectState},buff当前的回合持续是:{BuffConfig.buffDurationRound}");
    }

    /// <summary>
    /// 触发buff伤害
    /// </summary>
    public virtual void TriggerBuffDamge()
    {
        //这里是底层设计,只有MP和HP的BUFF类型是要每回合触发的,其他走正常叠加逻辑
        if (BuffConfig.BUFFATKType== BUFFATKType.HP|| BuffConfig.BUFFATKType == BUFFATKType.MP)
        {
            ownerHero.AttriAddBuff(BuffConfig.BUFFATKType, BuffConfig.BuffValue, BuffConfig.Percent);
            //VInt damage = NewBattleDataCalculatConter.CalculatDamage(BuffConfig, (HeroLogic)mAttacker, (FightUnitLogic)mOwner);
            //FightUnitLogic hero = mOwner as FightUnitLogic;
            //Debuger.Log("Trigger Buff Damage :"+damage);
            //hero.BuffDamage(damage, BuffConfig);
        }
    }
    /// <summary>
    /// 创建buff效果和特效
    /// </summary>
    public virtual void AddBuffAndEffect()
    {
        //先看是增益buff还是减益Buff或者是无Buff单纯属性加成
        //switch (BuffConfig.buffState)
        //{
        //    case NewBuffState.None:
        //        //单纯属性增加
        //        hero.AttriAddBuff(BuffConfig.BUFFATKType,BuffConfig.BuffValue, BuffConfig.Percent);
        //        break;
        //    case NewBuffState.Buff:
        //        hero.AttriAddBuff(BuffConfig.BUFFATKType,BuffConfig.BuffValue, BuffConfig.Percent);
        //        //增益BUFF,查看队列插入情况
        //        break;
        //    case NewBuffState.DeBuff:
        //        //减益BUFF,先看是不是控制buff再看被赋予者能否被控制
        //        hero.AttriAddBuff(BuffConfig.BUFFATKType, BuffConfig.BuffValue, BuffConfig.Percent);
        //        break;
        //}
        ownerHero.AttriAddBuff(BuffConfig.BUFFATKType, BuffConfig.BuffValue, BuffConfig.Percent);
        ownerHero.Addbuff(this);

//        bool isTrigger = BuffConfig.buffTriggerProbability == 100;
//        if (BuffConfig.buffTriggerProbability > 0 && BuffConfig.buffTriggerProbability < 100)
//        {
//            int result = LogicRandom.Instance.Range(0, 100);
//            Debuger.Log("随机种子：" + result);
//            isTrigger = result <= BuffConfig.buffTriggerProbability;
            
//        }
//        if (isTrigger)
//        {

//            int alreadAddBuffCount = ownerHero.GetBuffCount(Buffid);
//#if CLIENT_LOGIC
//            //特效不为空  创建特效
//            if (!string.IsNullOrEmpty(BuffConfig.buffEffect) && alreadAddBuffCount==0)
//            {
//                //生成buff特效
//                RenderObj = null;// ResourcesManager.Instance.LoadObject<RenderObject>("Prefabs/BuffEffect/" + BuffConfig.buffEffect);

//                SetRenderObject(RenderObj);
//                RenderObj.SetLogicObject(mOwner,null);
               
//                //Debuger.Log("创建Buffeffect：" + RenderObj.gameObject.name + " pos:" + RenderObj.transform.position + "parent:" + RenderObj.transform.parent);
//            }
//#endif      
//            if (Buffid == 10081)
//            {
//                Debuger.Log("冰冻buff  id：" + Buffid + "冰冻敌人：" + isTrigger + " targetHeroid:" + ownerHero.ID);
//            }
//            //相同的buff在次添加，重置所有相同buff的持续回合
//            if (alreadAddBuffCount!=0)
//            {
//                ownerHero.RefershtBuffDurationRound(Buffid);
//            }
//            //如果是无伤害控制技能，就停止动画播放
//            if (BuffConfig.buffType == BuffType.Control)
//            {
//                ownerHero.SetAnimState(AnimState.StopAnim);
//            }
//            Log.Info("添加的BUFFID:" + Buffid);
//            //ownerHero.Addbuff(this);
//        }
//        else
//        {
//            objectState = LogicObjectState.Death;
//        }
    }



    //public void ResetBuffSurvivalRoundCount()
    //{
    //    mCurBuffSurvivalRoundCount = 0;
    //}
    public override void OnDestroy()
    {
        Debuger.Log("释放Buff资源：" + Buffid);
        objectState = LogicObjectState.Death;
        RenderObj?.OnRelease();
        NewBuffsManager.Instance.DestroyBuff(this);
    }
}
