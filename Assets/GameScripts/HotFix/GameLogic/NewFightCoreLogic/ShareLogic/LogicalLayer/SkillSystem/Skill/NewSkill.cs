using System;
using System.Collections.Generic;
using TEngine;
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

#if CLIENT_LOGIC
        CreateSkillEffect();//创建技能特效
#endif
        //先添加自身瞬时添加的buff
        for (int j = 0; j < mSkillCfg.SelfBuffList.Count; j++)
        {
            NewBuffsManager.Instance.CreateBuff(mSkillCfg.SelfBuffList, mSkillOwner, mSkillOwner);
        }
        //先添加自身瞬时添加的buff


        List<FightUnitLogic> attackTargetlist = CalculationAndCauseDamage(); //计算伤害并造成伤害

#if CLIENT_LOGIC
        //CreateSKillHitEffect(attackTargetlist);//创建技能击中特效
#endif
        //AdditionBuffToTargets(attackTargetlist); //添加buff到攻击目标
        SkillEnd();
        //if (mSkillCfg.skillAttackDurationMS > 0)
        //    LogicTimeManager.Instance.DelayCall((VInt)mSkillCfg.skillAttackDurationMS, () => { MoveToSeat(SkillEnd); });
        //else
        //    MoveToSeat(SkillEnd);
    }
    /// <summary>
    /// 创建技能特效
    /// </summary>
    public void CreateSkillEffect()
    {
//#if CLIENT_LOGIC
//        if (!string.IsNullOrEmpty(mSkillCfg.skillEffect))
//        {
//            GameObject skillEffect = null;//ResourcesManager.Instance.LoadObject("Prefabs/SkillEffect/" + mSkillCfg.skillEffect);
//            if (mSkillOwner.HeroTeam == HeroTeamEnum.Enemy)
//            {
//                Vector3 angle = skillEffect.transform.eulerAngles;
//                angle.y = 180;
//                skillEffect.transform.eulerAngles = angle;
//            }
//            if (mSkillCfg.skillAttackType == SkillAttackType.AllHero)
//            {
//                skillEffect.GetComponent<SkillEffect>().SetEffectPos(VInt3.zero);
//            }
//            else
//            {
//                skillEffect.GetComponent<SkillEffect>().SetEffectPos(mSkillOwner.LogicPosition);
//            }
//        }
//#endif
    }
    /// <summary>
    /// 计算要施展的目标列表
    /// </summary>
    /// <returns></returns>
    public List<FightUnitLogic> CalculationAndCauseDamage()
    {
        List<FightUnitLogic> logicslist = NewBattleRule.GetAttackListByAttackType(mSkillCfg.SkillTarget, mSkillCfg.SkillRadiusType, mSkillOwner.SeatID, mSkillOwner.TargetSeatID);
        CauseDamageFightUnitList(logicslist);
        return logicslist;
    }
    /// <summary>
    /// 对目标列表施展伤害技能并附加技能BUFF
    /// </summary>
    /// <returns></returns>
    void CauseDamageFightUnitList(List<FightUnitLogic> logicslist)
    {
        foreach (var item in logicslist)
        {
            BeAttackEnum beAttack = NewBattleDataCalculatConter.IsCanBeAttack(mSkillCfg.SkillReleaseType, mSkillOwner, item);
            VInt damage;
            switch (beAttack)
            {
                case BeAttackEnum.Auxiliary:
                    AdditionBuffToTargets(item);
                    break;
                case BeAttackEnum.MeleeATK:
                    damage = NewBattleDataCalculatConter.CalculatDamage(beAttack, mSkillOwner, item);
                    item.DamageHP(damage);
                    AdditionBuffToTargets(item);
                    break;
                case BeAttackEnum.MAGATK:
                    damage = NewBattleDataCalculatConter.CalculatDamage(beAttack, mSkillOwner, item);
                    item.DamageHP(damage);
                    AdditionBuffToTargets(item);
                    break;
                case BeAttackEnum.CURSEATK:
                    AdditionBuffToTargets(item);
                    break;
                case BeAttackEnum.Invalid:
                    //无敌状态(无敌,物理无敌,魔法无敌)
                    item.BeInvalidByAttack();
                    break;
                case BeAttackEnum.Evade:
                    //被闪避了
                    item.BeEvade();
                    break;
                case BeAttackEnum.ELEMagicPenetrationAttack://元素魔法穿透
                    damage = NewBattleDataCalculatConter.CalculatDamage(beAttack, mSkillOwner, item);
                    item.DamageHP(damage);
                    break;
                case BeAttackEnum.CURSEMagicPenetrationAttack://诅咒魔法穿透
                    AdditionBuffToTargets(item);
                    break;
            }
        }
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
 