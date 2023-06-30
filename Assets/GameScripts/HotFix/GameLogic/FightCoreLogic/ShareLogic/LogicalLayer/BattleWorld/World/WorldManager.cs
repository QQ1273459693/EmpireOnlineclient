using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager
{

    private static BattleWorld mBattleWorld;
    public static List<HeroData> lastheroDataList;
    public static List<HeroData> lastEnemyDataList;
    public static int lastBattleid;
    public static int lastRandomSiteid;
    public static void Initialize()
    {
        ConfigConter.Initialized();
        SkillConfigConter.Initialized();
    }

    public static void Start()
    {
        //创建英雄
        //List<int> heroidlist = new List<int>() { 101, 102, 103, 104, 105, 501, 502, 503, 504, 505 };
        //List<HeroData> herolist = new List<HeroData>();
        //List<HeroData> enemylist = new List<HeroData>();
        //for (int i = 0; i < heroidlist.Count; i++)
        //{
        //    if (i < 5)
        //    {
        //        HeroData heroData = ConfigConter.GetHeroData(heroidlist[i]);
        //        heroData.seatid = i;
        //        herolist.Add(heroData);
        //    }
        //    else
        //    {
        //        HeroData heroData = ConfigConter.GetHeroData(heroidlist[i]);
        //        heroData.seatid = i - 5;
        //        enemylist.Add(ConfigConter.GetHeroData(heroidlist[i]));
        //    }
        //}

        //CreateBattleWord(herolist, enemylist, Random.Range(1, 101));
    }
    public static void Update()
    {
        if (mBattleWorld != null)
        {
            mBattleWorld.Update();
        }
    }

    public static void CreateBattleWord(List<HeroData> herolist, List<HeroData> enemylist, int battleSite,int battleid=1,System.Action<BattleWorld> battleEndCallBack =null,bool isPlayBack=false)
    {
        Debug.Log("CreateBattleWord  herolist.count:" + herolist.Count + " enemylist.count:" + enemylist.Count);
        WorldManager.DestroyWorld();
        mBattleWorld = new BattleWorld();
        //for (int i = 0; i < herolist.Count; i++)
        //{
        //    lastheroDataList.Add(herolist[i]);
        //}
        //for (int i = 0; i < enemylist.Count; i++)
        //{
        //    lastEnemyDataList.Add(enemylist[i]);
        //}
        lastheroDataList = herolist;
        lastEnemyDataList = enemylist;
        lastBattleid = battleid;
        lastRandomSiteid = battleSite;
        mBattleWorld.OnCreateWorld(herolist, enemylist, battleSite,battleid, battleEndCallBack);
    }

    public static void DestroyWorld()
    {
        if (mBattleWorld != null)
        {
            mBattleWorld.DestroyWorld();
        }
    }


}
