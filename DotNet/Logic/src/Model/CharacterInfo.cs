namespace TEngine.Logic;

public class CharacterInfoAwakeSystem : AwakeSystem<CharacterInfo>
{
    protected override void Awake(CharacterInfo self)
    {
        self.Awake();
    }
}

/// <summary>
/// 角色信息。
/// </summary>
public class CharacterInfo : Entity
{
    /// <summary>
    /// 用户唯一ID。
    /// </summary>
    public uint UID { get; set; }
    //昵称
    public string UserName { get; set; }

    //金币
    public int Gold { get; set; }
    //钻石
    public int Diamond { get; set; }
    //等级
    public int Level { get; set; }
    //技能点
    public int SkillPoints { get; set; }

    //属性
    public int[] UnitAttr { get; set; }

    //public List<Ca>
    public void Awake()
    {
        UserName = "测试用户";
        Gold= 0;
        Diamond = 999;
        Level = 1;
        SkillPoints = 55;
        UnitAttr = new int[12] { 1, 2,500,540, 3, 4, 5 ,6,7,8,9,10};
    }

}