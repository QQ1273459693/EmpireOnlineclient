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

    public int EXP { get; set; }

    public List<CharEquipSlotData> CharEquipSlots { get; set; }

    public List<SkillData> PassiveSkills { get; set; }
    public List<SkillData> ActiveSkills { get; set; }
    public List<SkillData> AutoSkills { get; set; }    


    //public List<Ca>
    public void Awake()
    {
        UserName = "测试用户";
        Gold= 0;
        Diamond = 999;
        Level = 1;
        SkillPoints = 55;
        var InitRoleBase = ConfigLoader.Instance.Tables.TbInitialRoleAttrieBase.DataList[0];
        UnitAttr = new int[24] { InitRoleBase.Hp, InitRoleBase.Mp, InitRoleBase.MaxHp, InitRoleBase.MaxMp,0,0,0,0,0,0,0,0,0,0, InitRoleBase.EleMagicHit, InitRoleBase.CurseMagicHit, 0,0,0,0,0,0,0,0};
        EXP = 0;
        CharEquipSlots = new List<CharEquipSlotData>(6);
        for (int i = 0; i < 6; i++)
        {
            CharEquipSlotData slotData = new CharEquipSlotData();
            slotData.Pos = i;
            CharEquipSlots.Add(slotData);
        }
        PassiveSkills = new List<SkillData>();
        ActiveSkills = new List<SkillData>();
        AutoSkills = new List<SkillData>();
    }

}