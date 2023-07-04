using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillConfigConter
{
    public static SkillConfig LoadSkillConfig(int skillid)
    {
        SkillConfig skillConfig = GameModule.Resource.LoadAsset<SkillConfig>(""+ skillid);//Resources.Load<SkillConfig>(AssetPathConfig.SKILL_CONFIG+skillid);
        if (skillConfig==null)
        {
            Debuger.Log("ººƒ‹≈‰÷√∂¡»° ß∞‹:"+skillid);
        }
        return skillConfig;
    }
    public static BuffConfig LoadBuffConfig(int buffid)
    {
        BuffConfig buffConfig = GameModule.Resource.LoadAsset<BuffConfig>("" + buffid);//Resources.Load<BuffConfig>(AssetPathConfig.BUFF_CONFIG + buffid);
        if (buffConfig == null)
        {
            Debuger.Log("buff≈‰÷√∂¡»° ß∞‹:" + buffid);
        }
        return buffConfig;
    }
}
