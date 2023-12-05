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
        /// 辅助功能,权限最高
        /// </summary>
        Auxiliary=0,
        /// <summary>
        /// 物理攻击
        /// </summary>
        MeleeATK = 1,
        /// <summary>
        /// 元素魔法攻击
        /// </summary>
        MAGATK = 2,
        /// <summary>
        /// 诅咒魔法攻击
        /// </summary>
        CURSEATK = 3,
        /// <summary>
        /// 无法攻击,无敌状态
        /// </summary>
        Invalid = 4,
        /// <summary>
        /// 闪避
        /// </summary>
        Evade = 5,
        /// <summary>
        /// 元素魔法穿透攻击
        /// </summary>
        ELEMagicPenetrationAttack=6,
        /// <summary>
        /// 诅咒魔法穿透攻击
        /// </summary>
        CURSEMagicPenetrationAttack = 7,
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
    public static VInt CalculatDamage(BeAttackEnum beAttackEnum,FightUnitLogic attacker, FightUnitLogic attackTarget)
    {
        VInt RealDAG = new VInt(100);
        switch (beAttackEnum)
        {
            case BeAttackEnum.MeleeATK:
                VInt WeaponDamge = RandomNum(attacker.MixDamage, attacker.MaxDamage);//武器伤害
                VInt RealATK = WeaponDamge + attacker.MeleeAk;//近战攻击力=武器伤害+攻击者的近战攻击
                VInt TargetDEF = Math.Min(0, (int)attackTarget.MeDEF - (int)attacker.ArmorBreakingAT);//被攻击目标的防御力=目标防御力-攻击者的破甲能力,最小值是0
                VInt MeATK = Math.Min(1, (int)RealATK - (int)TargetDEF);//攻击减去防御力结果伤害
                RealDAG = MeATK - MeATK * (attackTarget.Tough / new VInt(1000));//减去强韧值
                break;
            case BeAttackEnum.MAGATK:
                //魔法系统攻击力计算加成: 元素命中 - 元素抗性 = 命中率,如果命中大于100则直接采用元素伤害系统规则
                //元素伤害系统规则:如果命中的前提下 元素命中-元素抗性=剩余;(剩余/100)*魔法攻击力
                VInt EleHit=attacker.EleMagicHit - attackTarget.ELMRES;
                VInt MAGATK = (EleHit / new VInt(100)) * attacker.MagicAk;
                RealDAG = MAGATK - MAGATK * (attackTarget.Tough / new VInt(1000));//减去强韧值
                break;
            case BeAttackEnum.ELEMagicPenetrationAttack:
                //低于100命中率走随机命中,如果命中走元素伤害系统规则,否则最后一个阶段走穿透伤害系统规则
                //穿透伤害系统:穿透力的千分比就是几率,如果命中则 伤害=魔法攻击力-魔法防御力
                VInt PeneATK = attacker.MagicAk - attackTarget.MGDEF;
                RealDAG = PeneATK - PeneATK * (attackTarget.Tough / new VInt(1000));//减去强韧值
                break;
        }
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
        BeAttackEnum beAttack= BeAttackEnum.Auxiliary;
        switch (ReleaseType)
        {
            case SkillReleaseType.SwordType:
                ReleaseType = SkillReleaseType.Close_Combat;
                break;
        }


        switch (ReleaseType)
        {
            case SkillReleaseType.Close_Combat://近战攻击
                //先查看被攻击者是否是无敌状态或者物理攻击无效状态
                if (attackTarget.GetBuffTypeByEnum(NewBuffType.INVINCIBLE) || attackTarget.GetBuffTypeByEnum(NewBuffType.PHY_ATK_NOT))
                {
                    //表明有物理攻击无效的BUFF状态,不能攻击
                    beAttack = BeAttackEnum.Invalid;
                }
                else
                {
                    //都没有无敌状态,那么就看命中率
                    var HitProbability = attacker.PhysicalHit - attackTarget.Evade;
                    if (HitProbability < 0)
                    {
                        beAttack = BeAttackEnum.Evade;
                        //这里暂且不加强制命中底层处理
                    }
                    else
                    {
                        //看下命中是否超过100,超过就强制命中,否则随机处理
                        if (HitProbability >= 100)
                        {
                            //已经超过百分百几率,强制命中
                            beAttack = BeAttackEnum.MeleeATK;
                        }
                        else
                        {
                            //随机处理
                            if (CheckProbability(HitProbability))//成功命中
                            {
                                beAttack = BeAttackEnum.MeleeATK;
                            }
                            else
                            {
                                //命中失败,闪避
                                beAttack = BeAttackEnum.Evade;
                            }
                        }
                    }
                }
                break;
            case SkillReleaseType.Magic_Attack://魔法攻击
                //先查看被攻击者是否是无敌状态或者魔法攻击无效状态
                if (attackTarget.GetBuffTypeByEnum(NewBuffType.INVINCIBLE) || attackTarget.GetBuffTypeByEnum(NewBuffType.MAG_ATK_NOT))
                {
                    //表明有魔法攻击无效的BUFF状态,不能攻击
                    beAttack = BeAttackEnum.Invalid;
                }
                else
                {
                    //都没有无敌状态,那么就看命中率
                    var EleHitProbability = attacker.EleMagicHit - attackTarget.ELMRES;
                    if (EleHitProbability < 0)
                    {
                        //元素命中小于0,没有命中的可能,走魔法穿透系统规则
                        //这里暂且不加强制命中底层处理
                        //随机处理
                        if (CheckProbability(attacker.MagicPenetration,1000))//成功命中
                        {
                            //穿透命中了
                            beAttack = BeAttackEnum.ELEMagicPenetrationAttack;
                        }
                        else
                        {
                            //穿透命中失败,闪避
                            beAttack = BeAttackEnum.Evade;
                        }
                    }
                    else
                    {
                        //看下命中是否超过100,超过就强制命中,否则随机处理
                        if (EleHitProbability >= 100)
                        {
                            //已经超过百分百几率,强制命中
                            beAttack = BeAttackEnum.MAGATK;   
                        }
                        else
                        {
                            //随机处理
                            if (CheckProbability(EleHitProbability))//成功命中
                            {
                                beAttack = BeAttackEnum.MAGATK;
                            }
                            else
                            {
                                //命中失败,闪避
                                beAttack = BeAttackEnum.Evade;
                            }
                        }
                    }
                }
                break;
            case SkillReleaseType.Curse://诅咒攻击
                //先查看被攻击者是否是无敌状态或者魔法攻击无效状态
                if (attackTarget.GetBuffTypeByEnum(NewBuffType.INVINCIBLE) || attackTarget.GetBuffTypeByEnum(NewBuffType.MAG_ATK_NOT))
                {
                    //表明有魔法攻击无效的BUFF状态,不能攻击
                    beAttack = BeAttackEnum.Invalid;
                }
                else
                {
                    //都没有无敌状态,那么就看命中率
                    var CurseHitProbability = attacker.CurseMagicHit - attackTarget.CurseMgRES;
                    if (CurseHitProbability < 0)
                    {
                        //元素命中小于0,没有命中的可能,走魔法穿透系统规则
                        //这里暂且不加强制命中底层处理
                        //随机处理
                        if (CheckProbability(attacker.MagicPenetration,1000))//成功命中
                        {
                            //穿透命中了
                            beAttack = BeAttackEnum.CURSEMagicPenetrationAttack;
                        }
                        else
                        {
                            //穿透命中失败,闪避
                            beAttack = BeAttackEnum.Evade;
                        }
                    }
                    else
                    {
                        //看下命中是否超过100,超过就强制命中,否则随机处理
                        if (CurseHitProbability >= 100)
                        {
                            //已经超过百分百几率,强制命中
                            beAttack = BeAttackEnum.CURSEATK;
                        }
                        else
                        {
                            //随机处理
                            if (CheckProbability(CurseHitProbability))//成功命中
                            {
                                beAttack = BeAttackEnum.CURSEATK;
                            }
                            else
                            {
                                //命中失败,闪避
                                beAttack = BeAttackEnum.Evade;
                            }
                        }
                    }
                }
                break;
            case SkillReleaseType.SUBSIDIARY://辅助,最高权限
                beAttack = BeAttackEnum.Auxiliary;
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
    /// <summary>
    /// 输入一个概率看下是否命中这个概率
    /// </summary>
    /// <returns></returns>
    static bool CheckProbability(VInt probability,int Max=100)
    {
        VInt RandomValue = RandomNum(0, Max);
        return RandomValue <= probability;
    }

}

