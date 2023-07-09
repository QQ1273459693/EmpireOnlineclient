using Sirenix.Utilities;
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

        //这里用于本地不联网战斗
        Start();
    }

    public static void Start()
    {
        //创建英雄
        List<HeroData> playerHeroList = new List<HeroData>();
        List<HeroData> enemyHeroList = new List<HeroData>();
        for (int i = 0; i < ConfigConter.HeroDatalist.Count; i++)
        {
            var Hero = ConfigConter.HeroDatalist[i];
            if (i>=5)
            {
                Hero.seatid = i - 5;
                playerHeroList.Add(Hero);
            }
            else
            {
                Hero.seatid = i;
                enemyHeroList.Add(Hero);
            }
        }

        CreateBattleWord(playerHeroList, enemyHeroList,888);
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
        Debug.Log("创建英雄列表:" + herolist.Count + " 敌人列表:" + enemylist.Count);
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
