//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using System.Collections.Generic;


namespace GameConfig.item1
{
public sealed partial class Item1 :  Bright.Config.BeanBase 
{
    public Item1(ByteBuf _buf) 
    {
        Id = _buf.ReadInt();
        Name = _buf.ReadString();
        Desc = _buf.ReadString();
        Icon = _buf.ReadString();
        Overlapping = _buf.ReadBool();
        Price = _buf.ReadInt();
        BatchUseable = _buf.ReadBool();
        Quality = _buf.ReadInt();
        PostInit();
    }

    public static Item1 DeserializeItem1(ByteBuf _buf)
    {
        return new item1.Item1(_buf);
    }

    /// <summary>
    /// 这是id
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// 名字
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// 描述
    /// </summary>
    public string Desc { get; private set; }
    /// <summary>
    /// 图标
    /// </summary>
    public string Icon { get; private set; }
    /// <summary>
    /// 是否可重叠
    /// </summary>
    public bool Overlapping { get; private set; }
    /// <summary>
    /// 价格
    /// </summary>
    public int Price { get; private set; }
    /// <summary>
    /// 能否批量使用
    /// </summary>
    public bool BatchUseable { get; private set; }
    /// <summary>
    /// 品质
    /// </summary>
    public int Quality { get; private set; }

    public const int __ID__ = -272648978;
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
        + "Desc:" + Desc + ","
        + "Icon:" + Icon + ","
        + "Overlapping:" + Overlapping + ","
        + "Price:" + Price + ","
        + "BatchUseable:" + BatchUseable + ","
        + "Quality:" + Quality + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}

}