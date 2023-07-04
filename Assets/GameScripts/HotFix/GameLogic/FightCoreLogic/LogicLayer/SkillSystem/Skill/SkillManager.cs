using GameBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : Singleton<SkillManager>, ILogicBehaviour
{
    public void OnCreate()
    {
       
    }
    public Skill ReleaseSkill(int skillid, LogicObject skillOwner, bool isNormalAttack)
    {
        Skill skill = new Skill(skillid,skillOwner,isNormalAttack);
        skill.ReleaseSkill();
        return skill;
    }
    public void OnDestroy()
    {
       
    }

    public void OnLogicFrameUpdate()
    {
        
    }
}
