/// <summary>
/// 战斗单位技能数据
/// </summary>
public class FightUnitSkill
{
    public int SkillBaseTypeID { get; set; }
    public int SkID { get; set; }
    public int RaceID { get; set; }
    public int SkillType { get; set; }
    public int Lv { get; set; }
    public FightUnitSkill(int ID,int mRaceID,int mSkillType,int mLv)
    {
        SkID=ID;
        RaceID=mRaceID;
        SkillType=mSkillType;
        Lv=mLv;
    }
}
