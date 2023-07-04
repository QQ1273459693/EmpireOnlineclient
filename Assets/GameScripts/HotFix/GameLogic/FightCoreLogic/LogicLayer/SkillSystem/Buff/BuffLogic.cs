using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffLogic :LogicObject
{
    public BuffConfig BuffConfig { get; private set; }
    public int BuffID { get; private set; }
    protected LogicObject mOwnwer;//buffӵ����
    public HeroLogic mOwnwerHero;//buff�ͷ���
    public HeroLogic targetHero;//buff�ͷ���
    protected LogicObject mAttackeTarget;//buff����Ŀ��

    private int mCurAccTime;//��ǰBuff�ۼ���Чʱ��
    private int mCurRealTime;//��ǰbuffʵʱ����ʱ��

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
                case BuffTriggerType.OneDamage_RealTime://һ���� ʵʱ���˺�
                    if (BuffConfig.buffDurationTimeMs==0&&BuffConfig.buffTriggerIntervalMs==0)
                    {
                        TriggerBuff();
                        AddBuffAndEffect();
                        //����Ƕ�غ��˺�,��Ҫ��Buff״̬���ó�δ����״̬
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
                        //��ʱBuff
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
                case BuffTriggerType.MultisegmentDamage_RealTime://��ʱ�� ����˺�
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
        Debuger.Log("Buff����:"+BuffID,2);
        objectState = LogicObjectState.Death;
        RenderObj?.OnRelease();
        BuffManager.Instance.DestroyBuff(this);
    }
}
