using System;
using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;

public class NewSkill
{

    public int Skillid { get; private set; }

    private HeroLogic mSkillOwner;//技能拥有者

    private NewSkillConfig mSkillCfg;

    private LogicObject mSkillTarget;


    public NewSkill(int skillid, LogicObject skillOwner, bool isNoramlAtk)
    {
        Skillid = skillid;
        mSkillOwner = (HeroLogic)skillOwner;
        //mSkillCfg = NewSkillConfig.LoadSkillConfig(skillid);
        if (mSkillCfg == null)
        {
            Log.Error("技能配置不存在 技能id：" + skillid);
        }

    }
}
