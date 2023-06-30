using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConfigConter
{
    public static List<HeroData> HeroDatalist { get; private set; } = new List<HeroData>();

    public static List<HeroData> EnemyDataList { get; private set; } = new List<HeroData>();

    public static void Initialized()
    {
        SkillConfigConter.Initialized();
        LoadHeroConfig();
        for (int i = 0; i < 5; i++)
        {
            HeroData heroData= GetHeroData(500 + i+1);
            heroData.seatid = i;
            EnemyDataList.Add(heroData);
        }
  
    }

    public static void LoadHeroConfig()
    {
#if CLIENT_LOGIC
        return;
        TextAsset text = null;//ResourcesManager.Instance.LoadAsset<TextAsset>("Config/Hero");
        HeroDatalist = JsonConvert.DeserializeObject<List<HeroData>>(text.text);
#else
        string heroCfgPath =  PathConfig.SERVER_CONFIG_PATH+"Hero.json";
        string herollJson = File.ReadAllText(heroCfgPath);
        HeroDatalist = JsonConvert.DeserializeObject<List<HeroData>>(herollJson);
#endif

        Debuger.Log("HeroConfig Load Finish ! Hero Count:" + HeroDatalist.Count);
    }


    public static HeroData GetHeroData(int heroid)
    {
        foreach (var item in HeroDatalist)
        {
            if (item.id==heroid)
            {
                return item;
            }
        }
        return null;
    }
}
