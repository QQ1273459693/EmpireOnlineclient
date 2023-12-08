using System;
using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;

/// <summary>
/// 战斗单位数据
/// </summary>
public class FightUnitData
{
    public List<FightUnitSkill> m_PassSkillList;//被动技能数据
    public List<FightUnitSkill> m_ActiveSkillList;//主动技能数据
    public FightUnitData()
    {
        m_PassSkillList=new List<FightUnitSkill>();
        m_ActiveSkillList=new List<FightUnitSkill>();
    }
    public int ID { get; set; }
    /// <summary>
    /// 位置 座位 id
    /// </summary>
    public int SeatId { get; set; }
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 武器类型
    /// </summary>
    public int WeaponType { get; set; }
    /// <summary>
    /// 生命值
    /// </summary>
    public int Hp { get; set; }
    /// <summary>
    /// 法力值
    /// </summary>
    public int Mp { get; set; }
    /// <summary>
    /// 最大生命值
    /// </summary>
    public int MaxHp { get; set; }
    /// <summary>
    /// 最大法力值
    /// </summary>
    public int MaxMp { get; set; }
    /// <summary>
    /// 近战攻击力
    /// </summary>
    public int MeleeAk { get; set; }
    /// <summary>
    /// 远程攻击力
    /// </summary>
    public int RangeAk { get; set; }
    /// <summary>
    /// 魔法攻击力
    /// </summary>
    public int MagicAk { get; set; }
    /// <summary>
    /// 近战防御力
    /// </summary>
    public int MeDEF { get; set; }
    /// <summary>
    /// 远程防御力
    /// </summary>
    public int RGDEF { get; set; }
    /// <summary>
    /// 魔法防御力
    /// </summary>
    public int MGDEF { get; set; }
    /// <summary>
    /// 元素魔法抗性
    /// </summary>
    public int ELMRES { get; set; }
    /// <summary>
    /// 诅咒魔法抗性
    /// </summary>
    public int CurseMgRES { get; set; }
    /// <summary>
    /// 护甲
    /// </summary>
    public int Shield { get; set; }
    /// <summary>
    /// 物理攻击命中
    /// </summary>
    public int PhysicalHit { get; set; }
    /// <summary>
    /// 元素魔法命中
    /// </summary>
    public int EleMagicHit { get; set; }
    /// <summary>
    /// 诅咒魔法命中
    /// </summary>
    public int CurseMagicHit { get; set; }
    /// <summary>
    /// 魔法穿透力
    /// </summary>
    public int MagicPenetration { get; set; }
    /// <summary>
    /// 闪避
    /// </summary>
    public int Evade { get; set; }
    /// <summary>
    /// 出手速度
    /// </summary>
    public int Speed { get; set; }
    /// <summary>
    /// 暴击
    /// </summary>
    public int CriticalHit { get; set; }
    /// <summary>
    /// 最低武器伤害
    /// </summary>
    public int MixDamage { get; set; }
    /// <summary>
    /// 最高武器伤害
    /// </summary>
    public int MaxDamage { get; set; }
    /// <summary>
    /// 强韧
    /// </summary>
    public int Tough { get; set; }
    /// <summary>
    /// 破甲能力
    /// </summary>
    public int ArmorBreakingAT { get; set; }
}
