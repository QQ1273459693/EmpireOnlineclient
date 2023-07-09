using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TEngine;
using UnityEngine;

public class SkillConfigConter
{
#if CLIENT_LOGIC

#else
    public static List<SkillConfig> SkillConfigList { get; private set; } = new List<SkillConfig>();
     public static List<BuffConfig> BuffConfigList { get; private set; } = new List<BuffConfig>();
#endif
 
    public static void Initialized()
    {
#if CLIENT_LOGIC

#else
        string skillCfgPath =PathConfig.SERVER_CONFIG_PATH+ "SkillJsonCfg.txt";
        string skillJson = File.ReadAllText(skillCfgPath);
        SkillConfigList = JsonConvert.DeserializeObject<List<SkillConfig>>(skillJson);


        string buffCfgPath = PathConfig.SERVER_CONFIG_PATH+ "BuffJsonCfg.txt";
        string buffJson = File.ReadAllText(buffCfgPath);
        BuffConfigList = JsonConvert.DeserializeObject<List<BuffConfig>>(buffJson);
#endif
    }
    public static BuffConfig LoadBuffConfig(int buffid)
    {
#if CLIENT_LOGIC
        BuffConfig buffConfig = GameModule.Resource.LoadAsset<BuffConfig>("" + buffid);//Resources.Load<BuffConfig>(AssetPathConfig.BUFF_CONFIG + buffid);
        if (buffConfig == null)
        {
            Log.Error("buff配置读取失败:" + buffid);
        }
        return buffConfig;
#else

        for (int i = 0; i < BuffConfigList.Count; i++)
        {
            if (BuffConfigList[i].buffid == buffid)
            {
                return BuffConfigList[i];
            }
        }
        return null;
#endif
    }
    public static SkillConfig LoadSkillConfig(int skillid)
    {
#if CLIENT_LOGIC

        SkillConfig skillConfig = GameModule.Resource.LoadAsset<SkillConfig>("" + skillid);//Resources.Load<SkillConfig>(AssetPathConfig.SKILL_CONFIG+skillid);
        if (skillConfig == null)
        {
            Log.Error("技能配置读取失败:" + skillid);
        }
        return skillConfig;
#else
        for (int i = 0; i < SkillConfigList.Count; i++)
        {
            if (SkillConfigList[i].skillid == skillid)
            {
                return SkillConfigList[i];
            }
        }
        return null;
#endif
    }
}
