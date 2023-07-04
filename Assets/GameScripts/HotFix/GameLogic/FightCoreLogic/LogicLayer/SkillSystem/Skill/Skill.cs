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
    public SkillConfig SkillCfg { get; private set; }
    public int Skillid { get; private set; }
    private HeroLogic mSkillOwner;//技能拥有者、
    private HeroLogic mSkillTarget;//技能目标
    private bool mIsNormalAtk;//是否是普通攻击
    public SkillState SkillState { get; private set; }
    /// <summary>
    /// 创建技能
    /// </summary>
    /// <param name="skillid">技能ID</param>
    /// <param name="skillOwner">技能拥有者</param>
    /// <param name="isNormalAttack">是否是普通攻击</param>
    public Skill(int skillid,LogicObject skillOwner,bool isNormalAttack)
    {
        Skillid = skillid;
        mSkillOwner = (HeroLogic)skillOwner;
        mIsNormalAtk = isNormalAttack;
        SkillCfg=SkillConfigConter.LoadSkillConfig(skillid);
    }
    /// <summary>
    /// 释放技能
    /// </summary>
    public void ReleaseSkill()
    {
        Debuger.Log("ReleaseSkill id:"+Skillid);
        SkillShakeAfter();
        PlaySkillAnim();
        //只有在移动攻击类型的情况下,才需要进行移动
        if (SkillCfg.skillType == SkillType.MoveToAttack || SkillCfg.skillType == SkillType.MoveToEnemyConter||SkillCfg.skillType==SkillType.MoveToConter)
        {
            MoveToTarget(SkillTrigger);
        }else if (SkillCfg.skillType==SkillType.Chant)//吟唱技能
        {
            Debuger.Log("吟唱进入");
            SkillChant(SkillTrigger);
        }
        else if (SkillCfg.skillType==SkillType.Ballistic)//弹道技能
        {
            LogicTimerManager.Instance.DelayCall((VInt)SkillCfg.skillShakeAfterTimeMS,CreateBullet);
        }
    }
    
    /// <summary>
    /// 技能吟唱
    /// </summary>
    public void SkillChant(Action chantFinish)
    {
        LogicTimerManager.Instance.DelayCall((VInt)SkillCfg.skillShakeBeforeTimeMS,chantFinish);
    }
    /// <summary>
    /// 技能前腰
    /// </summary>
    public void SkillShakeBefore()
    {
        SkillState = SkillState.ShakeBefore;
    }
    /// <summary>
    /// 播放技能动画
    /// </summary>
    public void PlaySkillAnim()
    {
        if (mSkillOwner==null)
        {
            Debuger.LogError("出错");
        }else if (SkillCfg==null)
        {
            Debuger.LogError("出错");
        }
        if (SkillCfg.skillAnim==null)
        {
            Debuger.LogError("报错的角色是:"+SkillCfg.name);
        }
        mSkillOwner.PlayAnim(SkillCfg.skillAnim);
    }
    /// <summary>
    /// 移动到目标位置
    /// </summary>
    public void MoveToTarget(Action moveFinish)
    {
        VInt3 targetPos = VInt3.zero;
        if (SkillCfg.skillType==SkillType.MoveToAttack)
        {
            mSkillTarget=BattleRule.GetNormalAttackTarget(WorldManager.BattleWorld.heroLogic.GetHeroListByTeam(mSkillOwner,(HeroTeamEnum)SkillCfg.roleTragetType), mSkillOwner.HeroData.seatid);
            targetPos = new VInt3(mSkillTarget.LogicPosition.x,mSkillTarget.LogicPosition.y, mSkillTarget.LogicPosition.z);
            VInt z = mSkillOwner.HeroTeam == HeroTeamEnum.Enemy ? new VInt(-3).Int : new VInt(3).Int;
            targetPos.z -= z.RawInt;
        }
        else if (SkillCfg.skillType==SkillType.MoveToConter)
        {
            targetPos = new VInt3(BattleWorldNodes.Instance.conTerTrans.position);
        }else if (SkillCfg.skillType==SkillType.MoveToEnemyConter)
        {
            targetPos = new VInt3(mSkillOwner.HeroTeam==HeroTeamEnum.Enemy?BattleWorldNodes.Instance.slefHeroConterTrans.position:BattleWorldNodes.Instance.enemyConterTrans.position);
        }
        Debuger.LogError("正在移动,目标位置是:"+ targetPos);
        MoveToAction action = new MoveToAction(mSkillOwner,targetPos,(VInt)SkillCfg.skillShakeBeforeTimeMS,moveFinish);
        ActionManager.Instance.RunAction(action);
    }
    /// <summary>
    /// 触发技能
    /// </summary>
    public void SkillTrigger()
    {
        //英雄普通攻击需要回复一定的怒气值
        if (mIsNormalAtk)
        {
            mSkillOwner.UpdateAnger(mSkillOwner.HeroData.atkRange);
        }

        List<HeroLogic> herolist = CauseDamage();
        CreateSkillEffect(herolist);


        AdditionBuff(herolist);
        SkillShakeAfter();
        //英雄攻击完成后需要移动回原本的位置
        if (SkillCfg.skillAttackDurationMS>0)
        {
            LogicTimerManager.Instance.DelayCall((VInt)SkillCfg.skillAttackDurationMS, () =>
            {
                MoveToSeat(SkillEnd);
            });
        }
        else
        {
            MoveToSeat(SkillEnd);
        }
        
    }
    /// <summary>
    /// 生成技能特效
    /// </summary>
    public void CreateSkillEffect(List<HeroLogic> herolist)
    {
#if RENDER_LOGIC
        //击中特效
        if (!String.IsNullOrEmpty(SkillCfg.skillHitEffect))
        {
            for (int i = 0; i < herolist.Count; i++)
            {
                SkillEffect skillEffect = ResourceManager.Instance.LoadObject<SkillEffect>(AssetPathConfig.SKILLEFFECT + SkillCfg.skillHitEffect);
                skillEffect.SetFeectPos(herolist[i].LogicPosition);
            }
            
        }
        //技能特效
        if (!String.IsNullOrEmpty(SkillCfg.skillEffect))
        {
            SkillEffect skillEffect = ResourceManager.Instance.LoadObject<SkillEffect>(AssetPathConfig.SKILLEFFECT + SkillCfg.skillEffect);
            if (mSkillOwner.HeroTeam==HeroTeamEnum.Enemy)
            {
                //渲染表现不涉及逻辑
                Vector3 angle = skillEffect.transform.eulerAngles;
                angle.y = 180;
                skillEffect.transform.eulerAngles = angle;
            }
            //如果是攻击所有英雄,那就全屏一个技能特效,放到屏幕中间就行
            if (SkillCfg.skillAttackType==SkillAttackType.AllHero)
            {
                skillEffect.SetFeectPos(VInt3.zero);
            }
            else
            {
                skillEffect.SetFeectPos(mSkillOwner.LogicPosition);
            }
        }
#endif
    }
    /// <summary>
    /// 生成伤害
    /// </summary>
    public List<HeroLogic> CauseDamage()
    {
        //Debug测试
        string DamageText="我方:"+ mSkillOwner.HeroData.Name+":对";


        List<HeroLogic> herolist = WorldManager.BattleWorld.heroLogic.GetHeroListByTeam(mSkillOwner, (HeroTeamEnum)SkillCfg.roleTragetType);
        //根据攻击类型计算受到攻击的英雄
        List<HeroLogic> attackHeroList= BattleRule.GetAttackListByAttackType(SkillCfg.skillAttackType, herolist,mSkillOwner.HeroData.seatid);
        //CreateSkillEffect(attackHeroList);
        //Debuger.Log("看下攻击列表:"+attackHeroList.Count+",配置的枚举:"+ SkillCfg.skillAttackType);
        foreach (var hero in attackHeroList)
        {
            VInt damage= BattleRule.CalculaDamage(SkillCfg, mSkillOwner,hero);
            //受击回复怒气值
            hero.UpdateAnger(hero.HeroData.takeDamageRange);
            mSkillOwner.UpdateAnger(0);
            if (damage!=0)
            {
                if (SkillCfg.roleTragetType== RoleTargetType.Teammate)
                {
                    hero.DamageHp(-damage);
                }
                else
                {
                    hero.DamageHp(damage);
                }
                DamageText += $"敌人:{hero.HeroData.Name}造成:<color=#B10ADB>{damage.RawInt}</color>的伤害!";
            }
        }
        Debuger.Log(DamageText);
        return attackHeroList;
        
    }
    /// <summary>
    /// 添加Buff
    /// </summary>
    public void AdditionBuff(List<HeroLogic> attackTargetlist)
    {
        if (SkillCfg.addBuffs!=null&&SkillCfg.addBuffs.Length>0)
        {
            foreach (var atkTargetHero in attackTargetlist)
            {
                for (int i = 0; i < SkillCfg.addBuffs.Length; i++)
                {
                    BuffManager.Instance.CreateBuff(SkillCfg.addBuffs[i],mSkillOwner,atkTargetHero);
                }
            }
        }
    }
    public void CreateBullet()
    {
        mSkillTarget = BattleRule.GetNormalAttackTarget(WorldManager.BattleWorld.heroLogic.GetHeroListByTeam(mSkillOwner, (HeroTeamEnum)SkillCfg.roleTragetType), mSkillOwner.HeroData.seatid);
        BulletManager.Instance.CreateBullet(SkillCfg.bullet,mSkillOwner,mSkillTarget,SkillCfg.skillAttackDurationMS,SkillTrigger);
    }
    /// <summary>
    /// 技能后摇
    /// </summary>
    public void SkillShakeAfter()
    {
        SkillState = SkillState.ShakeAfter;
    }
    /// <summary>
    /// 移动到座位
    /// </summary>
    public void MoveToSeat(Action moveFinish)
    {
        if (SkillCfg.skillType == SkillType.Chant||SkillCfg.skillType==SkillType.Ballistic)
        {
            LogicTimerManager.Instance.DelayCall((VInt)SkillCfg.skillShakeAfterTimeMS,moveFinish);
        }
        else
        {
            VInt3 seatPos = VInt3.zero;
#if CLIENT_LOGIC
            Transform[] seatTransArr = mSkillOwner.HeroTeam == HeroTeamEnum.Enemy ? BattleWorldNodes.Instance.enemyTransArr : BattleWorldNodes.Instance.heroTransArr;
            seatPos = new VInt3(seatTransArr[mSkillOwner.HeroData.seatid].position);
#endif
            MoveToAction action = new MoveToAction(mSkillOwner, seatPos, (VInt)SkillCfg.skillShakeAfterTimeMS, moveFinish);
            ActionManager.Instance.RunAction(action);
        }
        
    }
    /// <summary>
    /// 技能释放结束
    /// </summary>
    public void SkillEnd()
    {
        Debuger.Log("技能释放完毕!,ID:"+Skillid);
        mSkillOwner.ActionEnd();
    }
}
