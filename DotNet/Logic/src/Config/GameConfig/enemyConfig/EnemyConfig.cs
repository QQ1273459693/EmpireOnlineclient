//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using System.Collections.Generic;


namespace GameConfig.enemyConfig
{
public sealed partial class EnemyConfig :  Bright.Config.BeanBase 
{
    public EnemyConfig(ByteBuf _buf) 
    {
        Id = _buf.ReadInt();
        Name = _buf.ReadString();
        Hp = _buf.ReadInt();
        Attack = _buf.ReadInt();
        Defence = _buf.ReadInt();
        Age = _buf.ReadInt();
        AtkRage = _buf.ReadInt();
        TakeDamageRage = _buf.ReadInt();
        MaxRage = _buf.ReadInt();
        Type = _buf.ReadInt();
        SkillDes = _buf.ReadString();
        {int n0 = System.Math.Min(_buf.ReadSize(), _buf.Size);SkillidList = new System.Collections.Generic.List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = _buf.ReadInt(); SkillidList.Add(_e0);}}
        PostInit();
    }

    public static EnemyConfig DeserializeEnemyConfig(ByteBuf _buf)
    {
        return new enemyConfig.EnemyConfig(_buf);
    }

    /// <summary>
    /// 怪物ID
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// 怪物名字
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// 血量
    /// </summary>
    public int Hp { get; private set; }
    /// <summary>
    /// 攻击力
    /// </summary>
    public int Attack { get; private set; }
    /// <summary>
    /// 防御力
    /// </summary>
    public int Defence { get; private set; }
    /// <summary>
    /// 敏捷值
    /// </summary>
    public int Age { get; private set; }
    /// <summary>
    /// 攻击回复的怒气值
    /// </summary>
    public int AtkRage { get; private set; }
    /// <summary>
    /// 受击恢复的怒气值
    /// </summary>
    public int TakeDamageRage { get; private set; }
    /// <summary>
    /// 最大怒气值
    /// </summary>
    public int MaxRage { get; private set; }
    /// <summary>
    /// 怪物类型
    /// </summary>
    public int Type { get; private set; }
    /// <summary>
    /// 技能描述
    /// </summary>
    public string SkillDes { get; private set; }
    /// <summary>
    /// 技能数组
    /// </summary>
    public System.Collections.Generic.List<int> SkillidList { get; private set; }

    public const int __ID__ = 1340359110;
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
        + "Id:" + Id + ","
        + "Name:" + Name + ","
        + "Hp:" + Hp + ","
        + "Attack:" + Attack + ","
        + "Defence:" + Defence + ","
        + "Age:" + Age + ","
        + "AtkRage:" + AtkRage + ","
        + "TakeDamageRage:" + TakeDamageRage + ","
        + "MaxRage:" + MaxRage + ","
        + "Type:" + Type + ","
        + "SkillDes:" + SkillDes + ","
        + "SkillidList:" + Bright.Common.StringUtil.CollectionToString(SkillidList) + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}

}