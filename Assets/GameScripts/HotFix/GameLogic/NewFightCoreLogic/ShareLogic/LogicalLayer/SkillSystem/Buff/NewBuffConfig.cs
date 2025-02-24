﻿using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NewBuffConfig
{
    public int buffDurationRound { get; set; }         //Buff持续回合
    public NewBuffType buffType { get; set; }                //Buff类型
    public NewBuffState buffState { get; set; }              //buff状态
    public BUFFATKType BUFFATKType { get; set; }
    public int BuffValue;//技能BUFF值
    public bool Percent { get; set; }//是否是百分比
    public int BUFFTAG { get; set; }//BUFF目标
    public int BUFFRAG { get; set; }//BUFF范围
    public bool isSelfBuff { get; set; }//是否是对自身施加Buff


    public NewBuffConfig(int Round,int m_BuffValue,bool percent,NewBuffType m_buffType, BUFFATKType m_bUFFATKType,int Bufftarget,int BuffRange)
    {
        buffDurationRound=Round;
        buffType = m_buffType;
        BUFFATKType=m_bUFFATKType;
        BuffValue = m_BuffValue;
        Percent = percent;
        BUFFTAG=Bufftarget;
        BUFFRAG = BuffRange;
        isSelfBuff =(SkillTarget)BUFFTAG== SkillTarget.SELF;
        buffState = BuffBattleRule.GetBuffStateByBuffType(m_buffType);

    } 
}
public enum NewBuffType
{
    /// <summary>
    /// 无
    /// </summary>
    None,
    /// <summary>
    /// 无敌状态,物理和魔法诅咒攻击无效
    /// </summary>
    INVINCIBLE,
    /// <summary>
    /// 物理攻击无效
    /// </summary>
    PHY_ATK_NOT,
    /// <summary>
    /// 魔法攻击无效
    /// </summary>
    MAG_ATK_NOT,
    /// <summary>
    /// 免疫定身,混乱,封魔状态
    /// </summary>
    IMTY_LMBE_CHAOS_SKLL,
    /// <summary>
    /// 火烧
    /// </summary>
    DEBUFF_FIRE,
    /// <summary>
    /// 封魔(技能沉默)
    /// </summary>
    SKILL_SILENT,
    /// <summary>
    /// 定身
    /// </summary>
    IMMOBILIZE,
    /// <summary>
    /// 混乱
    /// </summary>
    CHAOS,
    /// <summary>
    /// 中毒
    /// </summary>
    PONED,

}
public enum NewBuffState
{
    /// <summary>
    /// 无类型buff
    /// </summary>
    None=0,
    /// <summary>
    /// 增益BUFF
    /// </summary>
    Buff=1,
    /// <summary>
    /// 减益Buff
    /// </summary>
    DeBuff=2,
}
/// <summary>
/// Buff控制类型
/// </summary>
public enum BuffControlState
{
    /// <summary>
    /// 无控制
    /// </summary>
    None = 0,
    /// <summary>
    /// 有控制(目前只有定身,封魔,混乱)
    /// </summary>
    Control = 1,
}

/// <summary>
/// 技能BUFF实际攻击类型
/// </summary>
public enum BUFFATKType
{
    /// <summary>
    /// 最大血量百分比或者普通血量
    /// </summary>
    HP = 1,
    /// <summary>
    /// 最大MP百分比或者普通MP
    /// </summary>
    MP = 2,
    /// <summary>
    /// 近战攻击力
    /// </summary>
    MEATK = 3,
    /// <summary>
    /// 魔法攻击力
    /// </summary>
    MAGATK = 4,
    /// <summary>
    /// 近战防御力
    /// </summary>
    MEDFS = 5,
    /// <summary>
    /// 魔法防御力
    /// </summary>
    MAGDFS = 6,
    /// <summary>
    /// 元素抗性
    /// </summary>
    ELMRES = 7,
    /// <summary>
    /// 诅咒抗性
    /// </summary>
    CURSERES = 8,
    /// <summary>
    /// 物理攻击命中
    /// </summary>
    PHYHIT = 9,
    /// <summary>
    /// 元素命中
    /// </summary>
    ELMHIT = 10,
    /// <summary>
    /// 诅咒魔法命中
    /// </summary>
    CURSEHIT = 11,
    /// <summary>
    /// 魔法穿透力
    /// </summary>
    MagicPenetration = 12,
    /// <summary>
    /// 闪避
    /// </summary>
    EVADE = 13,
    /// <summary>
    /// 出手速度
    /// </summary>
    SPEED = 14,
    /// <summary>
    /// 暴击
    /// </summary>
    CRITHIT = 15,
    /// <summary>
    /// 强韧
    /// </summary>
    TOUGH = 16,
    /// <summary>
    /// 破甲能力
    /// </summary>
    ARMRBK = 17,

}

//public enum NewBuffDamageType
//{
//    None,//无伤害，增加buff
//    NomalAttackDamage,  //普通伤害  (普通攻击)
//    RealDamage,         //真实伤害 (无视护盾、减伤)
//    AtkPercentage,      //攻击力百分比伤害
//    HPPercentage,       //生命值白分比伤害
//    NoneDamageControl,  //无伤害控制
//}