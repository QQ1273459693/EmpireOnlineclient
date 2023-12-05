 using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;
public class NewSkillConfig
{
    public int Skillid;                     //技能ID 
    public string SkillName;                //技能名称 
    public string SkillDes;                 //技能描述
    public int Lv;
    public int NeedMp;//MP消耗
    public int Round;//持续回合
    public List<NewBuffConfig> BuffConfigList=new List<NewBuffConfig>();//非自身Buff添加列表
    public List<NewBuffConfig> SelfBuffList = new List<NewBuffConfig>();//自身Buff添加列表


    public SkillReleaseType SkillReleaseType { get; set; }
    //public SkillDamageType SkillDamageType { get; set; }
    public SkillTarget SkillTarget { get; set; }
    public SkillRadiusType SkillRadiusType { get; set; }

    /// <summary>
    /// 根据不同技能表加载技能
    /// </summary>
    public NewSkillConfig(int SkillBaseType,int ID,int SkillLv)
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
                    Log.Error("剑士表ID技能名称长度越界!:"+ Skillid);                    return;
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
                var SkillRealseData= SkillBase.SkillParam[SkillLv].SkilParams;
                BuffConfigList.Clear();
                SelfBuffList.Clear();
                for (int i = 0; i < SkillRealseData.Length; i++)
                {
                    var SkillData = SkillRealseData[i];
                    NewBuffConfig buffConfig = new NewBuffConfig(Round, SkillData.Value, SkillData.Percent,(NewBuffType)SkillData.BUFFID,(BUFFATKType)SkillData.ATTACKTP, SkillData.SKTAR, SkillData.SKRANGE);
                    if ((SkillTarget)SkillData.SKTAR== SkillTarget.SELF)
                    {
                        //属于自身BUFF
                        SelfBuffList.Add(buffConfig);
                    }
                    else
                    {
                        BuffConfigList.Add(buffConfig);
                    }
                    
                }
                //技能释放

                break;
            case 101:
                //怪物表
                var EnemySkillBase = ConfigLoader.Instance.Tables.TbEnemySkillBase.Get(Skillid);
                if (EnemySkillBase == null)
                {
                    Log.Error("怪物表读取错误,没有这个ID!:" + Skillid);
                    return;
                }
                Skillid = ID;
                Lv = SkillLv;
                SkillName = EnemySkillBase.Name;
                SkillDes = EnemySkillBase.Des;


                //技能前置
                SkillReleaseType = (SkillReleaseType)(int)EnemySkillBase.AttackType;
                var EnemyFontData = EnemySkillBase.AttackParam;
                NeedMp = EnemyFontData.MP;
                SkillTarget = (SkillTarget)EnemyFontData.Target;
                SkillRadiusType = (SkillRadiusType)EnemyFontData.RANGE;
                Round = EnemyFontData.Round;
                //技能前置


                //技能释放
                var EnemySkillRealseData = EnemySkillBase.SkillParam.SkilParams;
                BuffConfigList.Clear();
                SelfBuffList.Clear();
                for (int i = 0; i < EnemySkillRealseData.Count; i++)
                {
                    var m_SkillData = EnemySkillRealseData[i];
                    NewBuffConfig buffConfig = new NewBuffConfig(Round, m_SkillData.Value, m_SkillData.Percent,(NewBuffType)m_SkillData.BUFFID, (BUFFATKType)m_SkillData.ATTACKTP, m_SkillData.SKTAR, m_SkillData.SKRANGE);
                    if ((SkillTarget)m_SkillData.SKTAR == SkillTarget.SELF)
                    {
                        //属于自身BUFF
                        SelfBuffList.Add(buffConfig);
                    }
                    else
                    {
                        BuffConfigList.Add(buffConfig);
                    }
                }
                //技能释放

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

///// <summary>
///// 伤害类型
///// </summary>
//public enum SkillDamageType
//{ 
//    /// <summary>
//    /// 普通
//    /// </summary>
//    NomalDamage=0,
//    /// <summary>
//    /// 百分比
//    /// </summary>
//    Percentage=1,
//}

/// <summary>
/// 技能作用目标
/// </summary>
public enum SkillTarget
{
    /// <summary>
    /// 不限范围
    /// </summary>
    None=1,
    /// <summary>
    /// 友方
    /// </summary>
    Teammate=2,
    /// <summary>
    /// 敌方
    /// </summary>
    Enemy=3,
    /// <summary>
    /// 自身
    /// </summary>
    SELF = 4,
    /// <summary>
    /// 全屏
    /// </summary>
    ALL = 5,
}
/// <summary>
/// 技能范围类型
/// </summary>
public enum SkillRadiusType
{
    /// <summary>
    /// 全部
    /// </summary>
    ALL = 1,
    /// <summary>
    /// 单人
    /// </summary>
    SOLO = 2,
    /// <summary>
    /// 十字范围
    /// </summary>
    CROSS = 3,
}