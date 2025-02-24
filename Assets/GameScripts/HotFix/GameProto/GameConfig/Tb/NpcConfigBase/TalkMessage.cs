//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using System.Collections.Generic;


namespace GameConfig.Tb.NpcConfigBase
{
/// <summary>
/// 对话文本组
/// </summary>
public sealed partial class TalkMessage :  Bright.Config.BeanBase 
{
    public TalkMessage(ByteBuf _buf) 
    {
        TxtLine = _buf.ReadString();
        TXTContent = _buf.ReadString();
        Int = _buf.ReadInt();
        PostInit();
    }

    public static TalkMessage DeserializeTalkMessage(ByteBuf _buf)
    {
        return new Tb.NpcConfigBase.TalkMessage(_buf);
    }

    /// <summary>
    /// 对话行
    /// </summary>
    public string TxtLine { get; private set; }
    /// <summary>
    /// 内容
    /// </summary>
    public string TXTContent { get; private set; }
    /// <summary>
    /// 跳转类型
    /// </summary>
    public int Int { get; private set; }

    public const int __ID__ = -517378527;
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
        + "TxtLine:" + TxtLine + ","
        + "TXTContent:" + TXTContent + ","
        + "Int:" + Int + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}

}