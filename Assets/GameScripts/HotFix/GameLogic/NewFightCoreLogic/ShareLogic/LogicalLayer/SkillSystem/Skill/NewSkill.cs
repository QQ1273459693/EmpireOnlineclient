using DG.Tweening;
using GameLogic;
using System;
using System.Collections.Generic;
using TEngine;
using UnityEngine;
using static NewBattleDataCalculatConter;

public class NewSkill
{

    public int Skillid { get; private set; }

    private FightUnitLogic mSkillOwner;//技能拥有者

    private NewSkillConfig mSkillCfg;

    private LogicObject mSkillTarget;


    public NewSkill(int SkillBaseType,int skillid,int Lv, LogicObject skillOwner)
    {
        Skillid = skillid;
        mSkillOwner = (FightUnitLogic)skillOwner;
        mSkillCfg = new NewSkillConfig(SkillBaseType, skillid, Lv);
        if (mSkillCfg == null)
        {
            Log.Error("技能配置不存在 技能id"+ skillid);
        }
        else
        {
            Log.Info("技能配置加载成功!:"+ skillid);
        }
    }
    public void ReleaseSkill()
    {
        if (mSkillCfg == null)
            return;
        Log.Info("释放技能" + mSkillCfg.SkillName);
        PlayAnimation(TriggerSkill);
        //if (mSkillCfg.skillType == SkillType.MoveToAttack || mSkillCfg.skillType == SkillType.MoveToEnemyConter || mSkillCfg.skillType == SkillType.MoveToConter)//移动攻击形技能
        //{
        //    PlayAnimation();
        //    //MoveTarget(TriggerSkill);
        //}
        //else if (mSkillCfg.skillType == SkillType.Chant) //吟唱技能
        //{
        //    LogicTimeManager.Instance.DelayCall((VInt)mSkillCfg.skillShakeBeforeTimeMS, TriggerSkill);
        //}
        //else if (mSkillCfg.skillType == SkillType.Ballistic)//子弹技能
        //{
        //    LogicTimeManager.Instance.DelayCall((VInt)mSkillCfg.skillShakeBeforeTimeMS, CreateBullet);
        //}
    }

    public void PlayAnimation(Action TrigeSkill = null)
    {
        TrigeSkill?.Invoke();
    }
    /// <summary>
    /// 触发技能
    /// </summary>
    public void TriggerSkill()
    {
        //这里当做测试目标

        List<FightUnitLogic> attackList = new List<FightUnitLogic>();
        List<FightUnitLogic> herolist = null;
        if (mSkillOwner.UnitTeam== FightUnitTeamEnum.Self)
        {
            //是自己
            herolist = NewBattleWorld.Instance.heroLogic.GetHeroListByTeam(FightUnitTeamEnum.Enemy);
        }
        else
        {
            //是敌方
            herolist = NewBattleWorld.Instance.heroLogic.GetHeroListByTeam(FightUnitTeamEnum.Self);
        }
        herolist = NewBattleRule.GetHeroSurvivalList(herolist);
        Vector3 TargetPos= herolist[0].HeroRender.gameObject.transform.position;

        //测试用


#if CLIENT_LOGIC
        //CreateSkillEffect();//创建技能特效
#endif
        //先添加自身瞬时添加的buff
        for (int j = 0; j < mSkillCfg.SelfBuffList.Count; j++)
        {
            NewBuffsManager.Instance.CreateBuff(mSkillCfg.SelfBuffList, mSkillOwner, mSkillOwner);
        }
        //先添加自身瞬时添加的buff

        switch (mSkillCfg.SkillReleaseType)
        {
            case SkillReleaseType.SwordType://剑类攻击(近战)
                switch (mSkillCfg.SkillRadiusType)
                {
                    case SkillRadiusType.ALL://移动到中心
                        MoveTarget(() => {
                            CauseDamageStart();
                        }, FightRoundWindow.Instance.AttackCenter.position);
                        break;
                    case SkillRadiusType.CROSS://移动到目标范围
                        MoveTarget(() => {
                            CauseDamageStart();
                        }, TargetPos);
                        break;
                    case SkillRadiusType.SOLO://移动到目标范围
                        MoveTarget(() => {
                            CauseDamageStart();
                        }, TargetPos);
                        break;
                }
                mSkillOwner.HeroRender.PlayUITips(1);
                break;
            case SkillReleaseType.Ranged_Combat://远程攻击
                MoveTarget(() => {
                    CauseDamageStart();
                }, Vector3.zero);
                mSkillOwner.HeroRender.PlayUITips(1);
                break;
            case SkillReleaseType.Close_Combat://近战攻击
                switch (mSkillCfg.SkillRadiusType)
                {
                    case SkillRadiusType.ALL://移动到中心
                        MoveTarget(() => {
                            CauseDamageStart();
                        }, FightRoundWindow.Instance.AttackCenter.position);
                        break;
                    case SkillRadiusType.CROSS://移动到目标范围
                        MoveTarget(() => {
                            CauseDamageStart();
                        }, TargetPos);
                        break;
                    case SkillRadiusType.SOLO://移动到目标范围
                        MoveTarget(() => {
                            CauseDamageStart();
                        }, TargetPos);
                        break;
                }
                mSkillOwner.HeroRender.PlayUITips(1);
                break;
            case SkillReleaseType.Magic_Attack:
                mSkillOwner.HeroRender.PlayUITips(2);
                mSkillOwner.PlayAnim("Attack", () =>
                {
                    CauseDamageStart();
                });
                break;
            case SkillReleaseType.Curse:
                mSkillOwner.HeroRender.PlayUITips(2);
                mSkillOwner.PlayAnim("Attack", () =>
                {
                    CauseDamageStart();
                });
                
                break;
            case SkillReleaseType.CURE:
                mSkillOwner.HeroRender.PlayUITips(2);
                mSkillOwner.PlayAnim("AuxiliarySkill");
                CauseDamageStart();
                break;
            case SkillReleaseType.SUBSIDIARY:
                mSkillOwner.HeroRender.PlayUITips(2);
                mSkillOwner.PlayAnim("AuxiliarySkill");
                CauseDamageStart();
                break;
        }

        //List<FightUnitLogic> attackTargetlist = CalculationAndCauseDamage(); //计算伤害并造成伤害

#if CLIENT_LOGIC
        //CreateSKillHitEffect(attackTargetlist);//创建技能击中特效
#endif
        //AdditionBuffToTargets(attackTargetlist); //添加buff到攻击目标

        LogicTimeManager.Instance.DelayCall(1700, () => { SkillEnd(); });
        //if (mSkillCfg.skillAttackDurationMS > 0)
        //    LogicTimeManager.Instance.DelayCall((VInt)mSkillCfg.skillAttackDurationMS, () => { MoveToSeat(SkillEnd); });
        //else
        //    MoveToSeat(SkillEnd);
    }
    /// <summary>
    /// 移动完成开始造成伤害
    /// </summary>
    public void CauseDamageStart()
    {
        CalculationAndCauseDamage();
        //SkillEnd();
    }

    public void MoveTarget(Action moveFinish,Vector3 TargetPos)
    {
#if CLIENT_LOGIC
#endif
        mSkillOwner.HeroRender.FightAnimMovePos(TargetPos, moveFinish);
        //Debuger.Log("MoveAttackTarget : targetPos:" + targetPos.vec3 + " 移动目标id：" + ((HeroLogic)mSkillTarget).id + " 目标状态:" + ((HeroLogic)mSkillTarget).objectState);
        //MoveToAction action = new MoveToAction(mSkillOwner, TargetPos, (VInt)mSkillCfg.skillShakeBeforeTimeMS, moveFinish);
        //ActionManager.Instance.RunAction(action);
    }
    /// <summary>
    /// 创建技能特效
    /// </summary>
    public void CreateSkillEffect(FightUnitRender render, BeAttackEnum BeAtk)
    {
#if CLIENT_LOGIC
        if (mSkillCfg.SkillVFXID>0)
        {
            var VFXBase=ConfigLoader.Instance.Tables.TBVFXSkillModelBase.Get(mSkillCfg.SkillVFXID);
            if (VFXBase==null)
            {
                Log.Error("没有找到特效配置文件ID:"+ mSkillCfg.SkillVFXID);
                return;
            }
            EFX_ParticleHelp.GenerParticle(VFXBase.ResName, render.m_Vector3Pos);
            switch (BeAtk)
            {
                case BeAttackEnum.Auxiliary:
                    break;
                case BeAttackEnum.CURSEATK:
                    break;
                case BeAttackEnum.MeleeATK:
                    EFX_ParticleHelp.GenerParticle("EFX_BloodSream", render.m_Vector3Pos);
                    break;
                case BeAttackEnum.RangeATK:
                    EFX_ParticleHelp.GenerParticle("EFX_BloodSream", render.m_Vector3Pos);
                    break;
                case BeAttackEnum.MAGATK:
                    EFX_ParticleHelp.GenerParticle("EFX_BloodSream", render.m_Vector3Pos);
                    break;
                case BeAttackEnum.Invalid:
                    break;
                case BeAttackEnum.Evade:
                    break;
                case BeAttackEnum.ELEMagicPenetrationAttack:
                    break;
                case BeAttackEnum.CURSEMagicPenetrationAttack:
                    break;
            }
            //Log.Info("攻击延迟------------:"+ AtackDealy);
        }
        else
        {
            switch (BeAtk)
            {
                case BeAttackEnum.Auxiliary:
                    break;
                case BeAttackEnum.CURSEATK:
                    break;
                case BeAttackEnum.MeleeATK:
                    EFX_ParticleHelp.GenerParticle("EFX_BloodSream", render.m_Vector3Pos);
                    break;
                case BeAttackEnum.RangeATK:
                    EFX_ParticleHelp.GenerParticle("EFX_BloodSream", render.m_Vector3Pos);
                    break;
                case BeAttackEnum.MAGATK:
                    EFX_ParticleHelp.GenerParticle("EFX_BloodSream", render.m_Vector3Pos);
                    break;
                case BeAttackEnum.Invalid:
                    break;
                case BeAttackEnum.Evade:
                    break;
                case BeAttackEnum.ELEMagicPenetrationAttack:
                    break;
                case BeAttackEnum.CURSEMagicPenetrationAttack:
                    break;
            }
        }
#endif
    }
    /// <summary>
    /// 计算要施展的目标列表
    /// </summary>
    /// <returns></returns>
    public List<FightUnitLogic> CalculationAndCauseDamage()
    {
        List<FightUnitLogic> logicslist = NewBattleRule.GetAttackListByAttackType(mSkillCfg.SkillTarget, mSkillCfg.SkillRadiusType,mSkillOwner);
        CauseDamageFightUnitList(logicslist);
        return logicslist;
    }
    /// <summary>
    /// 对目标列表施展伤害技能并附加技能BUFF
    /// </summary>
    /// <returns></returns>
    void CauseDamageFightUnitList(List<FightUnitLogic> logicslist)
    {
        Log.Info("目标列表大小:"+ logicslist.Count);
        //开始延迟技能命中
        EFX_ParticleHelp.DealyParticle(mSkillCfg.SkillVFXID, () =>
        {
            foreach (var item in logicslist)
            {
                BeAttackEnum beAttack = NewBattleDataCalculatConter.IsCanBeAttack(mSkillCfg.SkillReleaseType, mSkillOwner, item);
                int damage;
                CreateSkillEffect(item.HeroRender, beAttack);
                switch (beAttack)
                {
                    case BeAttackEnum.Auxiliary:
                        AdditionBuffToTargets(item);
                        Log.Info($"技能释放者:{mSkillOwner.Name},对目标:{item.Name}释放了---辅助BUFF");
                        break;
                    case BeAttackEnum.MeleeATK:
                        damage = NewBattleDataCalculatConter.CalculatDamage(beAttack, mSkillOwner, item);
                        item.DamageHP(damage);

                        Log.Info($"技能释放者:{mSkillOwner.Name},对目标:{item.Name}进行近战攻击伤害---{damage}");
                        AdditionBuffToTargets(item);
                        break;
                    case BeAttackEnum.RangeATK:
                        damage = NewBattleDataCalculatConter.CalculatDamage(beAttack, mSkillOwner, item);
                        item.DamageHP(damage);
                        Log.Info($"技能释放者:{mSkillOwner.Name},对目标:{item.Name}进行远程攻击伤害---{damage}");
                        AdditionBuffToTargets(item);
                        break;
                    case BeAttackEnum.MAGATK:
                        damage = NewBattleDataCalculatConter.CalculatDamage(beAttack, mSkillOwner, item);
                        item.DamageHP(damage);
                        Log.Info($"技能释放者:{mSkillOwner.Name},对目标:{item.Name}进行魔法攻击伤害---{damage}");
                        AdditionBuffToTargets(item);
                        break;
                    case BeAttackEnum.CURSEATK:
                        AdditionBuffToTargets(item);
                        Log.Info($"技能释放者:{mSkillOwner.Name},对目标:{item.Name}进行诅咒攻击");
                        break;
                    case BeAttackEnum.Invalid:
                        //无敌状态(无敌,物理无敌,魔法无敌)
                        item.BeInvalidByAttack();
                        Log.Info($"技能释放者:{mSkillOwner.Name},对目标:{item.Name}无敌状态");
                        break;
                    case BeAttackEnum.Evade:
                        //被闪避了
                        item.BeEvade();
                        Log.Info($"技能释放者:{mSkillOwner.Name},对目标:{item.Name}被闪避");
                        break;
                    case BeAttackEnum.ELEMagicPenetrationAttack://元素魔法穿透
                        damage = NewBattleDataCalculatConter.CalculatDamage(beAttack, mSkillOwner, item);
                        item.DamageHP(damage);
                        Log.Info($"技能释放者:{mSkillOwner.Name},对目标:{item.Name}进行元素穿透魔法攻击伤害---{damage}");
                        break;
                    case BeAttackEnum.CURSEMagicPenetrationAttack://诅咒魔法穿透
                        AdditionBuffToTargets(item);
                        Log.Info($"技能释放者:{mSkillOwner.Name},对目标:{item.Name}进行诅咒穿透魔法攻击伤害---");
                        break;
                }
            }
        });
        
    }
    /// <summary>
    /// 附加BUFF给对象列表
    /// </summary>
    public void AdditionBuffToTargets(List<FightUnitLogic> attackTargetlist)
    {
        if (mSkillCfg.BuffConfigList != null && mSkillCfg.BuffConfigList.Count > 0)
        {
            for (int i = 0; i < attackTargetlist.Count; i++)
            {
                for (int j = 0; j < mSkillCfg.BuffConfigList.Count; j++)
                {
                    NewBuffsManager.Instance.CreateBuff(mSkillCfg.BuffConfigList, attackTargetlist[i], mSkillOwner);
                }
            }
        }
    }
    /// <summary>
    /// 附加Buff给单个对象
    /// </summary>
    public void AdditionBuffToTargets(FightUnitLogic attackTarget)
    {
        if (mSkillCfg.BuffConfigList != null && mSkillCfg.BuffConfigList.Count > 0)
        {
            for (int j = 0; j < mSkillCfg.BuffConfigList.Count; j++)
            {
                NewBuffsManager.Instance.CreateBuff(mSkillCfg.BuffConfigList, attackTarget, mSkillOwner);
            }
        }
    }
    public void SkillEnd()
    {
        //FightUnitLogicCtrl mHeroLogicCtrl = NewBattleWorld.Instance.heroLogic;
        //string heroHPStr = "";
        //for (int i = 0; i < mHeroLogicCtrl.AllHeroList.Count; i++)
        //{
        //    heroHPStr += mHeroLogicCtrl.AllHeroList[i].id + " Hero HP:" + mHeroLogicCtrl.AllHeroList[i].HP + "    怒气值:" + mHeroLogicCtrl.AllHeroList[i].Rage + "\n";
        //}
        Log.Info("技能释放完成：" + mSkillCfg.SkillName);

        //Debuger.Log("技能释放完成：" + mSkillCfg.skillid);
        mSkillOwner.OnMoveActionEnd();
    }


}
 