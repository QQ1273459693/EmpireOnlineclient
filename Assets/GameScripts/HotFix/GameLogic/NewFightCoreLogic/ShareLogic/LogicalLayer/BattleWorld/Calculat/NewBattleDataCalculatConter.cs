using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗数据计算中心
/// </summary>
public class NewBattleDataCalculatConter
{
    private static VInt MAX = new VInt(100);
    public static VInt CalculatDamage(NewSkillConfig skilCfg, FightUnitLogic attacker, FightUnitLogic attackTarget)
    {
        VInt rawDamge = new VInt(0);
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

        return rawDamge;
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
}
