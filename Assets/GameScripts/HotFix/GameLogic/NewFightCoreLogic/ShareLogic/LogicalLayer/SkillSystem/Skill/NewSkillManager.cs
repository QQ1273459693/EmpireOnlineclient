using GameBase;
public class NewSkillManager : Singleton<NewSkillManager>, ILogicBehaviour
{
    

    public void OnCreate()
    {

    }
    public void OnLogicFrameUpdate()
    {

    }
    public void RegisterSkill(int skillid, LogicObject skillOwner)
    {

    }
    public NewSkill ReleaseSkill(FightUnitSkill fightUnitSkill, LogicObject skillOwner)
    {
        NewSkill skill = new NewSkill(fightUnitSkill.SkillBaseTypeID, fightUnitSkill.SkID, fightUnitSkill.Lv, skillOwner);

        skill.ReleaseSkill();
        return skill;
    }
    public void OnDestroy()
    {
       
    }

}
