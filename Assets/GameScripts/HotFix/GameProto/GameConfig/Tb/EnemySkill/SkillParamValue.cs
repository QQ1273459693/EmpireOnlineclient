//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using System.Collections.Generic;


namespace GameConfig.Tb.EnemySkill
{
/// <summary>
/// 技能内置参数
/// </summary>
public sealed partial class SkillParamValue :  Bright.Config.BeanBase 
{
    public SkillParamValue(ByteBuf _buf) 
    {
        SKTAR = _buf.ReadInt();
        SKRANGE = _buf.ReadInt();
        ATTACKTP = _buf.ReadInt();
        Value = _buf.ReadInt();
        Percent = _buf.ReadBool();
        BUFFID = _buf.ReadInt();
        PostInit();
    }

    public static SkillParamValue DeserializeSkillParamValue(ByteBuf _buf)
    {
        return new Tb.EnemySkill.SkillParamValue(_buf);
    }

    /// <summary>
    /// 技能目标
    /// </summary>
    public int SKTAR { get; private set; }
    /// <summary>
    /// 技能范围
    /// </summary>
    public int SKRANGE { get; private set; }
    /// <summary>
    /// 属性类型值
    /// </summary>
    public int ATTACKTP { get; private set; }
    /// <summary>
    /// 属性值
    /// </summary>
    public int Value { get; private set; }
    /// <summary>
    /// 是否是百分比
    /// </summary>
    public bool Percent { get; private set; }
    /// <summary>
    /// BuffID
    /// </summary>
    public int BUFFID { get; private set; }

    public const int __ID__ = -1393910480;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<string, object> _tables)
    {
        PostResolve();
    }

    public  void TranslateText(System.Func<string, string, string> translator)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "SKTAR:" + SKTAR + ","
        + "SKRANGE:" + SKRANGE + ","
        + "ATTACKTP:" + ATTACKTP + ","
        + "Value:" + Value + ","
        + "Percent:" + Percent + ","
        + "BUFFID:" + BUFFID + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}

}