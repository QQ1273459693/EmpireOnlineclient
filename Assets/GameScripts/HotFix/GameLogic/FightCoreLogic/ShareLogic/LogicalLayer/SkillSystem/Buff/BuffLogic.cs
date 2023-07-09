using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;


public class BuffLogic : LogicObject
{
    public int Buffid { get; private set; }
    public BuffConfig BuffConfig { get; private set; }

    protected LogicObject mOwner;//Buff拥有者
    protected LogicObject mAttacker;//进攻者 添加buff的人
    public HeroLogic ownerHero;

    public int mCurBuffSurvivalRoundCount;//当前buff存活的回合

    private int mCurRealTime;//当前buff生效实时时间
    private int mCurAccTime;//当前buff累计生效时间
 
    public BuffLogic(int buffid, LogicObject owner, LogicObject attacker)
    {
        this.Buffid = buffid;
        this.mOwner = owner;
        this.mAttacker = attacker;
        this.ownerHero = (HeroLogic)owner;
    }
    public override void OnCreate()
    {
        objectState = LogicObjectState.Survival;
        BuffConfig = SkillConfigConter.LoadBuffConfig(Buffid);
        //BuffConfig = ResourcesManager.Instance.LoadAsset<BuffConfig>("Buff/" + Buffid);
        Log.Error($"开始创建Buff:{BuffConfig.buffName}----------------------");
        //伤害类型为回合开始结束的在施加buff的一瞬间就要把buff添加表现出来
        if (BuffConfig.buffTriggerType == BuffTriggerType.Damage_RoundStart || BuffConfig.buffTriggerType == BuffTriggerType.Damage_ActionEnd)
        {
            AddBuffAndEffect();
        }

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
    public override void OnLogicFrameUpdate()
    {
        if (objectState == LogicObjectState.Survival)
        {
            switch (BuffConfig.buffTriggerDamageType)
            {
                case BuffTriggerDamageType.OneDamage_RealTime:
                    //判断是否是延时buff
                    if (BuffConfig.buffDurationTimeMs == 0 && BuffConfig.buffTriggerIntervalMs == 0)
                    {
                        TriggerBuff();
                        AddBuffAndEffect();
                        if (BuffConfig.buffDurationRound == 0)
                            objectState = LogicObjectState.Death;
                        else
                            objectState = LogicObjectState.SurvialWiteing;
                    }
                    else
                    {
#if CLIENT_LOGIC
                        if (mCurAccTime >= BuffConfig.buffDurationTimeMs)
                        {
                            objectState = LogicObjectState.Death;
                            break;
                        }
                        //延时buff
                        mCurRealTime += FrameSyncConfig.LogicFrameLenms;
                        if (mCurRealTime >= BuffConfig.buffTriggerIntervalMs)
                        {
                            TriggerBuff();
                            AddBuffAndEffect();
                            mCurRealTime -= BuffConfig.buffTriggerIntervalMs;
                        }
                        mCurAccTime += FrameSyncConfig.LogicFrameLenms;
#else
                        while (mCurAccTime < BuffConfig.buffDurationTimeMs)
                        {
                            //延时buff
                            mCurRealTime += FrameSyncConfig.LogicFrameLenms;
                            if (mCurRealTime >= BuffConfig.buffTriggerIntervalMs)
                            {
                                TriggerBuff();
                                AddBuffAndEffect();
                                mCurRealTime -= BuffConfig.buffTriggerIntervalMs;
                            }
                            mCurAccTime += FrameSyncConfig.LogicFrameLenms;
                        }
                        if (mCurAccTime >= BuffConfig.buffDurationTimeMs)
                        {
                            objectState = LogicObjectState.Death;
                            break;
                        }
#endif


                    }
                    break;
                case BuffTriggerDamageType.MultisegmentDamage_RealTime:
#if CLIENT_LOGIC
                    if (BuffConfig.buffDurationTimeMs > 0 && BuffConfig.buffTriggerIntervalMs > 0)
                    {
                        if (mCurAccTime >= BuffConfig.buffDurationTimeMs)
                        {
                            objectState = LogicObjectState.Death;
                            break;
                        }
                        mCurRealTime += FrameSyncConfig.LogicFrameLenms;
                        Debuger.Log($"curRealTime:{mCurRealTime}  buffTriggerInterval:{BuffConfig.buffTriggerIntervalMs}");
                        if (mCurRealTime >= BuffConfig.buffTriggerIntervalMs)
                        {
                            TriggerBuff();
                            mCurRealTime -= BuffConfig.buffTriggerIntervalMs;
                        }
                        mCurAccTime += FrameSyncConfig.LogicFrameLenms;
                    }
                    break;
#else
                  if (BuffConfig.buffDurationTimeMs > 0 && BuffConfig.buffTriggerIntervalMs > 0)
                    {
                        while (mCurAccTime < BuffConfig.buffDurationTimeMs)
                        {
                            mCurRealTime += FrameSyncConfig.LogicFrameLenms;
                            Debuger.Log($"curRealTime:{mCurRealTime}  buffTriggerInterval:{BuffConfig.buffTriggerIntervalMs}");
                            if (mCurRealTime >= BuffConfig.buffTriggerIntervalMs)
                            {
                                TriggerBuff();
                                mCurRealTime -= BuffConfig.buffTriggerIntervalMs;
                            }
                            mCurAccTime += FrameSyncConfig.LogicFrameLenms;
                        }

                        //if (mCurAccTime >= BuffConfig.buffDurationTimeMs)
                        //{
                            objectState = LogicObjectState.Death;
                        //}

                    }
                    break;
#endif


            }
            //if (objectState == LogicObjectState.Death)
            //{
            //    OnDestroy();
            //}
        }
    }
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
        if (objectState == LogicObjectState.Survival)
        {
            if (BuffConfig.buffTriggerType == BuffTriggerType.Damage_ActionEnd)
                TriggerBuffDamge();
        }
    }
    /// <summary>
    /// 回合开始时事件
    /// </summary>
    public override void RoundStartEvent(int round)
    {
        base.RoundStartEvent(round);
        Log.Error($"回合:{round}已经开始,我的buff状态是:{objectState},buff触发类型是:{BuffConfig.buffTriggerType}");
        if (objectState == LogicObjectState.Survival|| objectState == LogicObjectState.SurvialWiteing)
        {
            if (BuffConfig.buffTriggerType == BuffTriggerType.Damage_RoundStart)
            {
                TriggerBuffDamge();
            }
        }
    }
    /// <summary>
    /// 回合结束事件
    /// </summary>
    public override void RoundEndEvent()
    {
        base.RoundEndEvent();
        mCurBuffSurvivalRoundCount++;
        Debuger.LogError("Buff Logic RoundEnd mCurBuffSurvival:" + mCurBuffSurvivalRoundCount + " Buffid:" + Buffid + " objectState:" + objectState);
        //新回合开始，如果当前buff已经超过了最大持续回合，就直接移除掉
        if (objectState == LogicObjectState.Survival || objectState == LogicObjectState.SurvialWiteing)
        {
            if (BuffConfig.buffDurationRound >= 0 && mCurBuffSurvivalRoundCount > BuffConfig.buffDurationRound)
            {
                ownerHero.SetAnimState(AnimState.RePlayAnim);
                objectState = LogicObjectState.Death;
                OnDestroy();
            }
        }
        Log.Error($"Buff回合:{mCurBuffSurvivalRoundCount}已经结束,我的buff状态是:{objectState},buff当前的回合持续是:{BuffConfig.buffDurationRound}");
    }

    /// <summary>
    /// 触发buff伤害
    /// </summary>
    public virtual void TriggerBuffDamge()
    {
        if (BuffConfig.damageType != BuffDamageType.None)
        {
            VInt damage = BattleDataCalculatConter.CalculatDamage(BuffConfig, (HeroLogic)mAttacker, (HeroLogic)mOwner);
            HeroLogic hero = mOwner as HeroLogic;
            Debuger.Log("Trigger Buff Damage :"+damage);
            hero.BuffDamage(damage, BuffConfig);
        }
    }
    /// <summary>
    /// 创建buff特效
    /// </summary>
    public virtual void AddBuffAndEffect()
    {

        bool isTrigger = BuffConfig.buffTriggerProbability == 100;
        if (BuffConfig.buffTriggerProbability > 0 && BuffConfig.buffTriggerProbability < 100)
        {
            int result = LogicRandom.Instance.Range(0, 100);
            Debuger.Log("随机种子：" + result);
            isTrigger = result <= BuffConfig.buffTriggerProbability;
            
        }
        if (isTrigger)
        {

            int alreadAddBuffCount = ownerHero.GetBuffCount(Buffid);
#if CLIENT_LOGIC
            //特效不为空  创建特效
            if (!string.IsNullOrEmpty(BuffConfig.buffEffect) && alreadAddBuffCount==0)
            {
                //生成buff特效
                RenderObj = null;// ResourcesManager.Instance.LoadObject<RenderObject>("Prefabs/BuffEffect/" + BuffConfig.buffEffect);

                SetRenderObject(RenderObj);
                RenderObj.SetLogicObject(mOwner,null);
               
                //Debuger.Log("创建Buffeffect：" + RenderObj.gameObject.name + " pos:" + RenderObj.transform.position + "parent:" + RenderObj.transform.parent);
            }
#endif      
            if (Buffid == 10081)
            {
                Debuger.Log("冰冻buff  id：" + Buffid + "冰冻敌人：" + isTrigger + " targetHeroid:" + ownerHero.id);
            }
            //相同的buff在次添加，重置所有相同buff的持续回合
            if (alreadAddBuffCount!=0)
            {
                ownerHero.RefershtBuffDurationRound(Buffid);
            }
            //如果是无伤害控制技能，就停止动画播放
            if (BuffConfig.buffType == BuffType.Control)
            {
                ownerHero.SetAnimState(AnimState.StopAnim);
            }
            Debuger.LogError("Addbuff" + Buffid);
            ownerHero.Addbuff(this);
        }
        else
        {
            objectState = LogicObjectState.Death;
        }
    }



    public void ResetBuffSurvivalRoundCount()
    {
        mCurBuffSurvivalRoundCount = 0;
    }
    public override void OnDestroy()
    {
        Debuger.Log("释放Buff资源：" + Buffid);
        objectState = LogicObjectState.Death;
        RenderObj?.OnRelease();
        BuffsManager.Instance.DestroyBuff(this);
    }
}
