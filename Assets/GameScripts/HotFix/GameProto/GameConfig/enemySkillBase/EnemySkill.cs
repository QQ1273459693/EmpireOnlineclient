//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using System.Collections.Generic;


namespace GameConfig.enemySkillBase
{
public sealed partial class EnemySkill :  Bright.Config.BeanBase 
{
    public EnemySkill(ByteBuf _buf) 
    {
        Id = _buf.ReadInt();
        Name = _buf.ReadString();
        Icon = _buf.ReadString();
        VFXID = _buf.ReadInt();
        Des = _buf.ReadString();
        AttackType = (SKILL.TYPE)_buf.ReadInt();
        AttackParam = Tb.SwordSkill.SkillFontValue.DeserializeSkillFontValue(_buf);
        SkillParam = Tb.EnemySkill.SkillParamArray.DeserializeSkillParamArray(_buf);
        PostInit();
    }

    public static EnemySkill DeserializeEnemySkill(ByteBuf _buf)
    {
        return new enemySkillBase.EnemySkill(_buf);
    }

    /// <summary>
    /// 技能ID
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// 技能名称
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// 图标
    /// </summary>
    public string Icon { get; private set; }
    /// <summary>
    /// 技能特效ID
    /// </summary>
    public int VFXID { get; private set; }
    /// <summary>
    /// 技能描述
    /// </summary>
    public string Des { get; private set; }
    /// <summary>
    /// 技能攻击类型
    /// </summary>
    public SKILL.TYPE AttackType { get; private set; }
    /// <summary>
    /// 技能前置参数
    /// </summary>
    public Tb.SwordSkill.SkillFontValue AttackParam { get; private set; }
    /// <summary>
    /// 技能执行参数
    /// </summary>
    public Tb.EnemySkill.SkillParamArray SkillParam { get; private set; }

    public const int __ID__ = 1647609885;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<string, object> _tables)
    {
        AttackParam?.Resolve(_tables);
        SkillParam?.Resolve(_tables);
        PostResolve();
    }

    public  void TranslateText(System.Func<string, string, string> translator)
    {
        AttackParam?.TranslateText(translator);
        SkillParam?.TranslateText(translator);
    }

    public override string ToString()
    {
        return "{ "
        + "Id:" + Id + ","
        + "Name:" + Name + ","
        + "Icon:" + Icon + ","
        + "VFXID:" + VFXID + ","
        + "Des:" + Des + ","
        + "AttackType:" + AttackType + ","
        + "AttackParam:" + AttackParam + ","
        + "SkillParam:" + SkillParam + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}

}