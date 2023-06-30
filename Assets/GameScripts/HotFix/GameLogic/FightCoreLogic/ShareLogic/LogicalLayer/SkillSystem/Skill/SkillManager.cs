using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : Singleton<SkillManager>, ILogicBehaviour
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
    public Skill ReleaseSkill(int skillid, LogicObject skillOwner,bool isNoramlAtk)
    {
        Skill skill = new Skill(skillid, skillOwner, isNoramlAtk);

        skill.ReleaseSkill();
        return skill;
    }
    public void OnDestroy()
    {
       
    }

}
