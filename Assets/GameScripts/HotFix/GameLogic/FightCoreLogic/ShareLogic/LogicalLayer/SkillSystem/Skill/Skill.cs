using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillState
{
    None,
    ShakeBefore,//技能前摇
    ShakeAfter,//技能后摇

}

public class Skill
{

    public int Skillid { get; private set; }

    private HeroLogic mSkillOwner;//技能拥有者

    private SkillConfig mSkillCfg;

    private LogicObject mSkillTarget;

    private bool mIsNormalAtk; //是否是普通攻击

    public Skill(int skillid, LogicObject skillOwner, bool isNoramlAtk)
    {
        Skillid = skillid;
        mSkillOwner = (HeroLogic)skillOwner;
        mSkillCfg = SkillConfigConter.LoadSkillConfig(skillid);
        mIsNormalAtk = isNoramlAtk;
        if (mSkillCfg == null)
        {
            Debuger.LogError("技能配置不存在 技能id：" + skillid);
        }

    }
    public void ReleaseSkill()
    {
        if (mSkillCfg == null)
            return;


        Debuger.Log("释放技能" + mSkillCfg.skillid);
        PlayAnimation();
        if (mSkillCfg.skillType == SkillType.MoveToAttack || mSkillCfg.skillType == SkillType.MoveToEnemyConter || mSkillCfg.skillType == SkillType.MoveToConter)//移动攻击形技能
        {
            MoveTarget(TriggerSkill);
        }
        else if (mSkillCfg.skillType == SkillType.Chant) //吟唱技能
        {
            LogicTimeManager.Instance.DelayCall((VInt)mSkillCfg.skillShakeBeforeTimeMS, TriggerSkill);
        }
        else if (mSkillCfg.skillType == SkillType.Ballistic)//子弹技能
        {
            LogicTimeManager.Instance.DelayCall((VInt)mSkillCfg.skillShakeBeforeTimeMS, CreateBullet);
        }
    }
    public void PlayAnimation()
    {
#if CLIENT_LOGIC
        if (!mIsNormalAtk)
            BattleWordNodes.Instance.skillWindow.PlayAnim(mSkillCfg, mSkillOwner.id);
#endif
        mSkillOwner.PlayAnim(mSkillCfg.skillAnim);
    }
    public void CreateBullet()
    {
        mSkillTarget = BattleRule.GetNomalAttackTarget(BattleWorld.Instance.heroLogic.GetHeroListByTeam(mSkillOwner, (HeroTeamEnum)mSkillCfg.roleTragetType), mSkillOwner.HeroData.seatid);
        BulletManager.Instance.CreateBullet(mSkillCfg.bullet, mSkillOwner, mSkillTarget, mSkillCfg.skillAttackDurationMS, TriggerSkill);
    }
    public void MoveTarget(Action moveFinish)
    {
        VInt3 targetPos = VInt3.zero;
#if CLIENT_LOGIC
        //获取目标位置
        if (mSkillCfg.skillType == SkillType.MoveToAttack)
        {
            mSkillTarget = BattleRule.GetNomalAttackTarget(BattleWorld.Instance.heroLogic.GetHeroListByTeam(mSkillOwner, (HeroTeamEnum)mSkillCfg.roleTragetType), mSkillOwner.HeroData.seatid);
            targetPos = new VInt3(mSkillTarget.LogicPosition.x, mSkillTarget.LogicPosition.y, mSkillTarget.LogicPosition.z);
            VInt z = mSkillOwner.HeroTeam == HeroTeamEnum.Enemy ? new VInt(-3).Int : new VInt(3).Int;
            targetPos.z -= z.RawInt;
        }
        else if (mSkillCfg.skillType == SkillType.MoveToEnemyConter)
        {
            targetPos = new VInt3(mSkillOwner.HeroTeam == HeroTeamEnum.Enemy ? BattleWordNodes.Instance.herosConter.position : BattleWordNodes.Instance.enemysConter.position);
        }
        else if (mSkillCfg.skillType == SkillType.MoveToConter)
        {
            targetPos = new VInt3(BattleWordNodes.Instance.conterTrans.position);
        }
#endif
        //Debuger.Log("MoveAttackTarget : targetPos:" + targetPos.vec3 + " 移动目标id：" + ((HeroLogic)mSkillTarget).id + " 目标状态:" + ((HeroLogic)mSkillTarget).objectState);
        MoveToAction action = new MoveToAction(mSkillOwner, targetPos, (VInt)mSkillCfg.skillShakeBeforeTimeMS, moveFinish);
        ActionManager.Instance.RunAction(action);
    }
    public void MoveToSeat(Action moveFinish)
    {
        VInt3 seatPos = VInt3.zero;
#if CLIENT_LOGIC
        Transform[] transformArr = mSkillOwner.HeroTeam == HeroTeamEnum.Enemy ? BattleWordNodes.Instance.enemyTransArr : BattleWordNodes.Instance.heroTransArr;
        seatPos = new VInt3(transformArr[mSkillOwner.HeroData.seatid].position);
#endif
        MoveToAction action = new MoveToAction(mSkillOwner, seatPos, (VInt)mSkillCfg.skillShakeAfterTimeMS, moveFinish);
        ActionManager.Instance.RunAction(action);
    }
    //触发技能
    public void TriggerSkill()
    {
        //释放技能不回复怒气值
        if (mIsNormalAtk)
            mSkillOwner.AttackUpdateAnger();    //增加怒气值


#if CLIENT_LOGIC
        CreateSkillEffect();//创建技能特效
#endif
        List<HeroLogic> attackTargetlist = CalculationAndCauseDamage(); //计算伤害并造成伤害

#if CLIENT_LOGIC
        CreateSKillHitEffect(attackTargetlist);//创建技能击中特效
#endif
        AdditionBuffToTargets(attackTargetlist); //添加buff到攻击目标
        if (mSkillCfg.skillAttackDurationMS > 0)
            LogicTimeManager.Instance.DelayCall((VInt)mSkillCfg.skillAttackDurationMS, () => { MoveToSeat(SkillEnd); });
        else
            MoveToSeat(SkillEnd);
    }
    //创建技能特效
    public void CreateSkillEffect()
    {
#if CLIENT_LOGIC
        if (!string.IsNullOrEmpty(mSkillCfg.skillEffect))
        {
            GameObject skillEffect = ResourcesManager.Instance.LoadObject("Prefabs/SkillEffect/" + mSkillCfg.skillEffect);
            if (mSkillOwner.HeroTeam == HeroTeamEnum.Enemy)
            {
                Vector3 angle = skillEffect.transform.eulerAngles;
                angle.y = 180;
                skillEffect.transform.eulerAngles = angle;
            }
            if (mSkillCfg.skillAttackType == SkillAttackType.AllHero)
            {
                skillEffect.GetComponent<SkillEffect>().SetEffectPos(VInt3.zero);
            }
            else
            {
                skillEffect.GetComponent<SkillEffect>().SetEffectPos(mSkillOwner.LogicPosition);
            }
        }
#endif
    }
    public void CreateSKillHitEffect(List<HeroLogic> logiclist)
    {
#if CLIENT_LOGIC
        if (!string.IsNullOrEmpty(mSkillCfg.skillHitEffect))
        {
            foreach (var item in logiclist)
            {
                GameObject skillEffect = ResourcesManager.Instance.LoadObject("Prefabs/SkillEffect/" + mSkillCfg.skillHitEffect);
                skillEffect.GetComponent<SkillEffect>().SetEffectPos(item.LogicPosition);
            }
        }
#endif
    }
    //计算并造成伤害
    public List<HeroLogic> CalculationAndCauseDamage()
    {
        List<HeroLogic> logicslist = BattleRule.GetAttackListByAttackType(mSkillCfg.skillAttackType
            , BattleWorld.Instance.heroLogic.GetHeroListByTeam(mSkillOwner, (HeroTeamEnum)mSkillCfg.roleTragetType), mSkillOwner.HeroData.seatid);

        foreach (var item in logicslist)
        {
            VInt damage = BattleDataCalculatConter.CalculatDamage(mSkillCfg, mSkillOwner, (HeroLogic)item);
            item.TakeDamageRage();
            if (damage != 0)
            {
                if (mSkillCfg.roleTragetType == RoleTargetType.Teammate)
                {
                    item.DamageHP(-damage);
                }
                else
                {
                    item.DamageHP(damage);
                }
                Debuger.Log("damage:" + damage.RawInt);
            }
        }

        return logicslist;
    }
    //附加buff
    public void AdditionBuffToTargets(List<HeroLogic> attackTargetlist)
    {
        if (mSkillCfg.addBuffs != null && mSkillCfg.addBuffs.Length > 0)
        {
            for (int i = 0; i < attackTargetlist.Count; i++)
            {
                for (int j = 0; j < mSkillCfg.addBuffs.Length; j++)
                {
                    if (Skillid == 1041 || Skillid == 1021)
                    {
                        Debuger.Log("CreateBuff skillid:" + Skillid + "    buffid:" + mSkillCfg.addBuffs[j] + "    addTarget:" + (attackTargetlist[i] as HeroLogic).id + " ReleaseHero:" + mSkillOwner.id);
                    }
                    BuffsManager.Instance.CreateBuff(mSkillCfg.addBuffs[j], attackTargetlist[i], mSkillOwner);
                }
            }
        }
    }
    public void SkillEnd()
    {
        HeroLogicCtrl mHeroLogicCtrl = BattleWorld.Instance.heroLogic;
        string heroHPStr = "";
        for (int i = 0; i < mHeroLogicCtrl.AllHeroList.Count; i++)
        {
            heroHPStr += mHeroLogicCtrl.AllHeroList[i].id + " Hero HP:" + mHeroLogicCtrl.AllHeroList[i].HP + "    怒气值:" + mHeroLogicCtrl.AllHeroList[i].Rage + "\n";
        }
        Debuger.Log("技能释放完成：" + mSkillCfg.skillid + "\n所有英雄怒气值：\n" + heroHPStr);

        //Debuger.Log("技能释放完成：" + mSkillCfg.skillid);
        mSkillOwner.OnMoveActionEnd();
    }
}
