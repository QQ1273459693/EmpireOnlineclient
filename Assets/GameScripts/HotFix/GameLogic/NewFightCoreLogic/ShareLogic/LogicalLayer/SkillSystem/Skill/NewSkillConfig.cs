using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;
public class NewSkillConfig
{
    public int Skillid;                     //技能ID 
    public string SkillName;                //技能名称 
    public int[] AddBuffs;                  //添加的buff
    public string SkillDes;                 //技能描述
    public int Value;//值
    public int Lv;
    public int NeedMp;//MP消耗
    public int Round;//持续回合
    public SkillReleaseType SkillReleaseType { get; set; }
    public SkillDamageType SkillDamageType { get; set; }
    public SkillATKType SkillATKType { get; set; }
    public SkillTarget SkillTarget { get; set; }
    public SkillRadiusType SkillRadiusType { get; set; }
    /// <summary>
    /// 根据不同技能表加载技能
    /// </summary>
    /// <param name="SkillBaseType">技能表类型</param>
    /// <param name="ID">技能ID</param>
    void LoadSkillConfig(int SkillBaseType,int ID,int SkillLv)
    {
        switch (SkillBaseType)
        {
            case 1:
                //剑士表
                var SkillBase = ConfigLoader.Instance.Tables.TbSwordSkillBase.Get(Skillid);
                if (SkillBase==null)
                {
                    Log.Error("剑士表读取错误,没有这个ID!:"+ Skillid);
                    return;
                }
                Skillid = ID;
                Lv = SkillLv;
                if (Lv>= SkillBase.Name.Count)
                {
                    Log.Error("剑士表ID技能名称长度越界!:"+ Skillid);
                    return;
                }
                SkillName = SkillBase.Name[SkillLv];
                if (Lv >= SkillBase.Des.Count)
                {
                    Log.Error("剑士表ID技能描述长度越界!:" + Skillid);
                    return;
                }
                SkillDes = SkillBase.Des[SkillLv];


                //技能前置
                SkillReleaseType = (SkillReleaseType)(int)SkillBase.AttackType;
                var FontData = SkillBase.AttackParam[SkillLv];
                NeedMp = FontData.MP;
                SkillTarget = (SkillTarget)FontData.Target;
                SkillRadiusType = (SkillRadiusType)FontData.RANGE;
                Round = FontData.Round;
                //技能前置


                //技能释放
                var SkillRealseData= SkillBase.SkillParam[SkillLv];
                //SkillATKType = SkillRealseData.
                //技能释放

                break;
            case 101:
                //怪物表

                break;
        }
    }
}

/// <summary>
/// 技能释放类型
/// </summary>
public enum SkillReleaseType
{
    /// <summary>
    /// 剑类
    /// </summary>
    SwordType=1,
    /// <summary>
    /// 近战攻击
    /// </summary>
    Close_Combat = 2,
    /// <summary>
    /// 魔法攻击
    /// </summary>
    Magic_Attack = 3,
    /// <summary>
    /// 诅咒魔法攻击
    /// </summary>
    Curse = 4,
    /// <summary>
    /// 治疗
    /// </summary>
    CURE = 5,
    /// <summary>
    /// 辅助(权限最高的类型)
    /// </summary>
    SUBSIDIARY = 6,
}

/// <summary>
/// 伤害类型
/// </summary>
public enum SkillDamageType
{ 
    /// <summary>
    /// 普通
    /// </summary>
    NomalDamage,
    /// <summary>
    /// 百分比
    /// </summary>
    Percentage,
}


/// <summary>
/// 技能实际攻击类型
/// </summary>
public enum SkillATKType
{
    /// <summary>
    /// 最大血量百分比或者普通血量
    /// </summary>
    HP=1,
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
    /// 闪避
    /// </summary>
    EVADE = 11,
    /// <summary>
    /// 出手速度
    /// </summary>
    SPEED = 12,
    /// <summary>
    /// 暴击
    /// </summary>
    CRITHIT = 13,
    /// <summary>
    /// 强韧
    /// </summary>
    TOUGH = 14,
    /// <summary>
    /// 破甲能力
    /// </summary>
    ARMRBK = 15,

}


/// <summary>
/// 技能作用目标
/// </summary>
public enum SkillTarget
{
    /// <summary>
    /// 不限范围
    /// </summary>
    None,
    /// <summary>
    /// 友方
    /// </summary>
    Teammate,
    /// <summary>
    /// 敌方
    /// </summary>
    Enemy,
}
/// <summary>
/// 技能范围类型
/// </summary>
public enum SkillRadiusType
{
    /// <summary>
    /// 全屏
    /// </summary>
    ALL = 1,
    /// <summary>
    /// 自身
    /// </summary>
    SELF = 2,
    /// <summary>
    /// 单人
    /// </summary>
    SOLO = 3,
    /// <summary>
    /// 十字范围
    /// </summary>
    CROSS = 4,
}