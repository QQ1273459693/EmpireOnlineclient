using System;
using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;

public class NewSkill
{

    public int Skillid { get; private set; }

    private HeroLogic mSkillOwner;//技能拥有者

    private SkillConfig mSkillCfg;

    private LogicObject mSkillTarget;

    private bool mIsNormalAtk; //是否是普通攻击

    public NewSkill(int skillid, LogicObject skillOwner, bool isNoramlAtk)
    {
        Skillid = skillid;
        mSkillOwner = (HeroLogic)skillOwner;
        mSkillCfg = SkillConfigConter.LoadSkillConfig(skillid);
        mIsNormalAtk = isNoramlAtk;
        if (mSkillCfg == null)
        {
            Debuger.LogError("技能配置不存在 技能id：" + skillid);
        }

    }
}
