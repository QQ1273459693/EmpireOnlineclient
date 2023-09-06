namespace TEngine.Logic;

/// <summary>
/// 账号信息
/// </summary>
public class BagModelInfo : Entity
{
    /// <summary>
    /// 用户唯一ID。
    /// </summary>
    public uint UID { get; set; }
    /// <summary>
    /// 物品ID
    /// </summary>
    public List<Slot> Slot { get; set; }

    public override void OnCreate()
    {
        base.OnCreate();
        Slot=new List<Slot>();
    }
    public override void Dispose()
    {
        base.Dispose();
    }
}