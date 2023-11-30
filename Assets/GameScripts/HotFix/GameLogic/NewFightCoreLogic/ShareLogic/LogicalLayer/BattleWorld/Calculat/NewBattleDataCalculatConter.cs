using System;
/// <summary>
/// 战斗数据计算中心
/// </summary>
public class NewBattleDataCalculatConter
{
    /// <summary>
    /// 查看是否能被攻击,被攻击的类型是什么
    /// </summary>
    public enum BeAttackEnum
    {
        /// <summary>
        /// 可以直接进行攻击
        /// </summary>
        None = 1,
        /// <summary>
        /// 无法攻击,无敌状态
        /// </summary>
        Invalid = 2,
        /// <summary>
        /// 闪避
        /// </summary>
        Evade = 3,
    }
    public static VInt CalculatDamage(NewSkillConfig skilCfg, FightUnitLogic attacker, FightUnitLogic attackTarget)
    {
        VInt RealDAG = new VInt(100);
        switch (skilCfg.SkillReleaseType)
        {
            case SkillReleaseType.SwordType://剑类攻击,实际是近战攻击
                VInt WeaponDamge = RandomNum(attacker.MixDamage,attacker.MaxDamage);//武器伤害
                VInt RealATK = WeaponDamge+attacker.MeleeAk;//近战攻击力=武器伤害+攻击者的近战攻击
                VInt TargetDEF = Math.Min(0, (int)attackTarget.MeDEF - (int)attacker.ArmorBreakingAT);//被攻击目标的防御力=目标防御力-攻击者的破甲能力,最小值是0
                VInt MeATK= Math.Min(1, (int)RealATK - (int)TargetDEF);//攻击减去防御力结果伤害
                RealDAG = MeATK - MeATK * (attackTarget.Tough/1000);//减去强韧值

                break;
            case SkillReleaseType.Close_Combat://近战攻击
                break;
            case SkillReleaseType.Magic_Attack://魔法攻击
                break;
            case SkillReleaseType.Curse://诅咒攻击
                break;
            case SkillReleaseType.CURE://治疗
                break;
            case SkillReleaseType.SUBSIDIARY://辅助
                break;
        }
        //switch (skilCfg.damageType)
        //{
        //    case DamageType.NomalDamage:
        //        //伤害减免=护甲/(伤害+护甲)*100%;
        //        VInt damageReduceRate = attackTarget.Def / (attacker.ATK + attackTarget.Def);
        //        rawDamge = (attacker.ATK - (attacker.ATK * damageReduceRate));
        //        break;
        //    case DamageType.RealDamage:
        //        //真实伤害无视护甲、护盾
        //        rawDamge = attacker.ATK;
        //        break;
        //    case DamageType.AtkPercentage:
        //        //伤害减免=护甲/(伤害+护甲)*100%;
        //        //damageReduceRate = attackTarget.Def / (attacker.ATK + attackTarget.Def);
        //        //攻击力百分比伤害
        //        VInt atkmutiple = (VInt)skilCfg.damagePercentage / new VInt(100);
        //        VInt totalDamge = attacker.ATK * atkmutiple;
        //        rawDamge = totalDamge;//(totalDamge - (totalDamge * damageReduceRate));
        //        break;
        //    case DamageType.HPPercentage:
        //        //血量百分比伤害
        //        VInt hpmutiple = (VInt)skilCfg.damagePercentage / new VInt(100);
        //        rawDamge = (attackTarget.MAXHP * hpmutiple);
        //        break;

        //}

        return RealDAG;
    }

    public static VInt CalculatDamage(BuffConfig buffCfg, HeroLogic attacker, HeroLogic attackTarget)
    {
        VInt rawDamge = new VInt(0);
        switch (buffCfg.damageType)
        {
            case BuffDamageType.NomalAttackDamage:
                //伤害减免=护甲/(伤害+护甲)*100%;
                VInt damageReduceRate = attackTarget.Def / (attacker.ATK + attackTarget.Def);
                rawDamge = (attacker.ATK - (attacker.ATK * damageReduceRate));
                break;
            case BuffDamageType.RealDamage:
                //真实伤害无视护甲、护盾、减伤buff
                rawDamge = attacker.ATK;
                break;
            case BuffDamageType.AtkPercentage:
                //伤害减免=护甲/(伤害+护甲)*100%;
                //damageReduceRate = attackTarget.Def / (attacker.ATK + attackTarget.Def);
                //攻击力百分比伤害
                VInt atkmutiple = (VInt)buffCfg.damageOrPercentage / new VInt(100);
                VInt totalDamge = attacker.ATK * atkmutiple;
                rawDamge = totalDamge; //(totalDamge - (totalDamge * damageReduceRate));
                if (buffCfg.damageOrPercentage==0)
                {
                    Debuger.LogError("Buff Config DamageOrPercentage ==0 Buffid:"+buffCfg.buffid);
                }
                break;
            case BuffDamageType.HPPercentage:                //生命值百分比伤害
                //获取伤害系数
                VInt hpmutiple = (VInt)buffCfg.damageOrPercentage / new VInt(100);
                rawDamge = (attackTarget.MAXHP * hpmutiple);
                if (buffCfg.damageOrPercentage == 0)
                {
                    Debuger.LogError("Buff Config DamageOrPercentage ==0 Buffid:" + buffCfg.buffid);
                }
                break;
        }

        return rawDamge;
    }
    public static BeAttackEnum IsCanBeAttack(SkillReleaseType ReleaseType, FightUnitLogic attacker, FightUnitLogic attackTarget)
    {
        BeAttackEnum beAttack= BeAttackEnum.None;
        switch (ReleaseType)
        {
            case SkillReleaseType.SwordType://剑类攻击,实际是近战攻击
                //先查看被攻击者是否是无敌状态
                if (attackTarget.GetBuffCount)
                {

                }

                //VInt WeaponDamge = RandomNum(attacker.MixDamage, attacker.MaxDamage);//武器伤害
                //int 


                break;
            case SkillReleaseType.Close_Combat://近战攻击


                break;
            case SkillReleaseType.Magic_Attack://魔法攻击
                break;
        }



        return beAttack;
    }
    /// <summary>
    /// 随机范围取值
    /// </summary>
    /// <returns></returns>
    static VInt RandomNum(VInt Mix, VInt Max)
    {
        Random rand = new Random();
        return rand.Next((int)Mix,(int)Max);
    }

}

