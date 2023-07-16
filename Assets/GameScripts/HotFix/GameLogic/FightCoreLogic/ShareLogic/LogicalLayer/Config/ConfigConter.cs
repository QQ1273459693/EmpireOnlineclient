using Newtonsoft.Json;
using Sirenix.Utilities;
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
        LoadHeroConfig();
    }

    public static void LoadHeroConfig()
    {
#if CLIENT_LOGIC
        var EnemyData = ConfigLoader.Instance.Tables.TbEnemyConfig.DataList;
        for (int i = 0; i < EnemyData.Count; i++)
        {
            var Data = EnemyData[i];
            HeroData heroData = new();
            heroData.id = Data.Id;
            heroData.hp = Data.Hp;
            heroData.atk = Data.Attack;
            heroData.def = Data.Defence;
            heroData.Name = Data.Name;
            heroData.maxRage = Data.MaxRage;
            heroData.atkRange = Data.AtkRage;
            heroData.agl = Data.Age;
            heroData.seatid = 0;//这个座位是根据自己设置的
            heroData.takeDamageRange = Data.TakeDamageRage;
            heroData.skillidArr.AddRange(Data.SkillidList);
            HeroDatalist.Add(heroData);
        }
        //TextAsset text = ResourceManager.Instance.LoadAsset<TextAsset>("Config/Hero");
        //HeroDatalist = JsonConvert.DeserializeObject<List<HeroData>>(text.text);//
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
            if (item.id == heroid)
            {
                return item;
            }
        }
        return null;
    }
}
