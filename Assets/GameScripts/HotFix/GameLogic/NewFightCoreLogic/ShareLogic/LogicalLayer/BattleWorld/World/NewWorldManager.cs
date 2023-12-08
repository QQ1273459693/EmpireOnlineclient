using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewWorldManager
{

    private static NewBattleWorld mBattleWorld;
    public static List<FightUnitData> lastheroDataList;
    public static List<FightUnitData> lastEnemyDataList;
    public static int lastBattleid;
    public static int lastRandomSiteid;
    public static void Initialize()
    {

        //这里用于本地不联网战斗
        Start();
    }

    public static void Start()
    {
        //创建角色列表和敌人列表,这里测试用
        List<FightUnitData> playerHeroList = new List<FightUnitData>();
        List<FightUnitData> enemyHeroList = new List<FightUnitData>();

        //for (int i = 0; i < 3; i++)
        //{

        //}
        //for (int i = 0; i < ConfigConter.HeroDatalist.Count; i++)
        //{
        //    var Hero = ConfigConter.HeroDatalist[i];
        //    if (i>=5)
        //    {
        //        Hero.seatid = i - 5;
        //        playerHeroList.Add(Hero);
        //    }
        //    else
        //    {
        //        Hero.seatid = i;
        //        enemyHeroList.Add(Hero);
        //    }
        //}

        CreateBattleWord(playerHeroList,enemyHeroList);
    }
    public static void Update()
    {
        mBattleWorld?.Update();
    }

    public static void CreateBattleWord(List<FightUnitData> herolist, List<FightUnitData> enemylist,System.Action<NewBattleWorld> battleEndCallBack =null)
    {
        Debug.Log("创建英雄列表:" + herolist.Count + " 敌人列表:" + enemylist.Count);
        WorldManager.DestroyWorld();
        mBattleWorld = new NewBattleWorld();
        //for (int i = 0; i < herolist.Count; i++)
        //{
        //    lastheroDataList.Add(herolist[i]);
        //}
        //for (int i = 0; i < enemylist.Count; i++)
        //{
        //    lastEnemyDataList.Add(enemylist[i]);
        //}
        //lastheroDataList = herolist;
        //lastEnemyDataList = enemylist;
        mBattleWorld.OnCreateWorld(herolist, enemylist,battleEndCallBack);
    }

    public static void DestroyWorld()
    {
        if (mBattleWorld != null)
        {
            mBattleWorld.DestroyWorld();
        }
    }


}
