using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillState
{
    None,
    ShakeBefore,//����ǰҡ
    ShakeAfter,//���ܺ�ҡ

}
public class Skill
{
    public SkillConfig SkillCfg { get; private set; }
    public int Skillid { get; private set; }
    private HeroLogic mSkillOwner;//����ӵ���ߡ�
    private HeroLogic mSkillTarget;//����Ŀ��
    private bool mIsNormalAtk;//�Ƿ�����ͨ����
    public SkillState SkillState { get; private set; }
    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="skillid">����ID</param>
    /// <param name="skillOwner">����ӵ����</param>
    /// <param name="isNormalAttack">�Ƿ�����ͨ����</param>
    public Skill(int skillid,LogicObject skillOwner,bool isNormalAttack)
    {
        Skillid = skillid;
        mSkillOwner = (HeroLogic)skillOwner;
        mIsNormalAtk = isNormalAttack;
        SkillCfg=SkillConfigConter.LoadSkillConfig(skillid);
    }
    /// <summary>
    /// �ͷż���
    /// </summary>
    public void ReleaseSkill()
    {
        Debuger.Log("ReleaseSkill id:"+Skillid);
        SkillShakeAfter();
        PlaySkillAnim();
        //ֻ�����ƶ��������͵������,����Ҫ�����ƶ�
        if (SkillCfg.skillType == SkillType.MoveToAttack || SkillCfg.skillType == SkillType.MoveToEnemyConter||SkillCfg.skillType==SkillType.MoveToConter)
        {
            MoveToTarget(SkillTrigger);
        }else if (SkillCfg.skillType==SkillType.Chant)//��������
        {
            Debuger.Log("��������");
            SkillChant(SkillTrigger);
        }
        else if (SkillCfg.skillType==SkillType.Ballistic)//��������
        {
            LogicTimerManager.Instance.DelayCall((VInt)SkillCfg.skillShakeAfterTimeMS,CreateBullet);
        }
    }
    
    /// <summary>
    /// ��������
    /// </summary>
    public void SkillChant(Action chantFinish)
    {
        LogicTimerManager.Instance.DelayCall((VInt)SkillCfg.skillShakeBeforeTimeMS,chantFinish);
    }
    /// <summary>
    /// ����ǰ��
    /// </summary>
    public void SkillShakeBefore()
    {
        SkillState = SkillState.ShakeBefore;
    }
    /// <summary>
    /// ���ż��ܶ���
    /// </summary>
    public void PlaySkillAnim()
    {
        if (mSkillOwner==null)
        {
            Debuger.LogError("����");
        }else if (SkillCfg==null)
        {
            Debuger.LogError("����");
        }
        if (SkillCfg.skillAnim==null)
        {
            Debuger.LogError("����Ľ�ɫ��:"+SkillCfg.name);
        }
        mSkillOwner.PlayAnim(SkillCfg.skillAnim);
    }
    /// <summary>
    /// �ƶ���Ŀ��λ��
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
        Debuger.LogError("�����ƶ�,Ŀ��λ����:"+ targetPos);
        MoveToAction action = new MoveToAction(mSkillOwner,targetPos,(VInt)SkillCfg.skillShakeBeforeTimeMS,moveFinish);
        ActionManager.Instance.RunAction(action);
    }
    /// <summary>
    /// ��������
    /// </summary>
    public void SkillTrigger()
    {
        //Ӣ����ͨ������Ҫ�ظ�һ����ŭ��ֵ
        if (mIsNormalAtk)
        {
            mSkillOwner.UpdateAnger(mSkillOwner.HeroData.atkRange);
        }

        List<HeroLogic> herolist = CauseDamage();
        CreateSkillEffect(herolist);


        AdditionBuff(herolist);
        SkillShakeAfter();
        //Ӣ�۹�����ɺ���Ҫ�ƶ���ԭ����λ��
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
    /// ���ɼ�����Ч
    /// </summary>
    public void CreateSkillEffect(List<HeroLogic> herolist)
    {
#if RENDER_LOGIC
        //������Ч
        if (!String.IsNullOrEmpty(SkillCfg.skillHitEffect))
        {
            for (int i = 0; i < herolist.Count; i++)
            {
                SkillEffect skillEffect = ResourceManager.Instance.LoadObject<SkillEffect>(AssetPathConfig.SKILLEFFECT + SkillCfg.skillHitEffect);
                skillEffect.SetFeectPos(herolist[i].LogicPosition);
            }
            
        }
        //������Ч
        if (!String.IsNullOrEmpty(SkillCfg.skillEffect))
        {
            SkillEffect skillEffect = ResourceManager.Instance.LoadObject<SkillEffect>(AssetPathConfig.SKILLEFFECT + SkillCfg.skillEffect);
            if (mSkillOwner.HeroTeam==HeroTeamEnum.Enemy)
            {
                //��Ⱦ���ֲ��漰�߼�
                Vector3 angle = skillEffect.transform.eulerAngles;
                angle.y = 180;
                skillEffect.transform.eulerAngles = angle;
            }
            //����ǹ�������Ӣ��,�Ǿ�ȫ��һ��������Ч,�ŵ���Ļ�м����
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
    /// �����˺�
    /// </summary>
    public List<HeroLogic> CauseDamage()
    {
        //Debug����
        string DamageText="�ҷ�:"+ mSkillOwner.HeroData.Name+":��";


        List<HeroLogic> herolist = WorldManager.BattleWorld.heroLogic.GetHeroListByTeam(mSkillOwner, (HeroTeamEnum)SkillCfg.roleTragetType);
        //���ݹ������ͼ����ܵ�������Ӣ��
        List<HeroLogic> attackHeroList= BattleRule.GetAttackListByAttackType(SkillCfg.skillAttackType, herolist,mSkillOwner.HeroData.seatid);
        //CreateSkillEffect(attackHeroList);
        //Debuger.Log("���¹����б�:"+attackHeroList.Count+",���õ�ö��:"+ SkillCfg.skillAttackType);
        foreach (var hero in attackHeroList)
        {
            VInt damage= BattleRule.CalculaDamage(SkillCfg, mSkillOwner,hero);
            //�ܻ��ظ�ŭ��ֵ
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
                DamageText += $"����:{hero.HeroData.Name}���:<color=#B10ADB>{damage.RawInt}</color>���˺�!";
            }
        }
        Debuger.Log(DamageText);
        return attackHeroList;
        
    }
    /// <summary>
    /// ���Buff
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
    /// ���ܺ�ҡ
    /// </summary>
    public void SkillShakeAfter()
    {
        SkillState = SkillState.ShakeAfter;
    }
    /// <summary>
    /// �ƶ�����λ
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
    /// �����ͷŽ���
    /// </summary>
    public void SkillEnd()
    {
        Debuger.Log("�����ͷ����!,ID:"+Skillid);
        mSkillOwner.ActionEnd();
    }
}
