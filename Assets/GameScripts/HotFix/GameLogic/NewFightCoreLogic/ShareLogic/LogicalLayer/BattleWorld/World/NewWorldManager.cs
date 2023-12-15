using GameLogic;
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


        for (int i = 0; i < 1; i++)
        {
            FightUnitData Data = new FightUnitData();
            Data.ID = 1000;
            FightUnitSkill Skill1 = new FightUnitSkill(101,0,1,6);
            FightUnitSkill Skill2 = new FightUnitSkill(201, 0,2,4);
            FightUnitSkill Skill3 = new FightUnitSkill(301,0,2,3);

            Data.m_ActiveSkillList.Add(Skill1);
            Data.m_PassSkillList.Add(Skill2);
            Data.m_PassSkillList.Add(Skill3);

            Data.ResName = "MainHeroAnim";
            Data.SeatId = 0;
            Data.WeaponType = 1;
            Data.MaxHp = 9999;
            Data.Hp = 9999;
            Data.MaxHp = 1000;
            Data.Mp= 1000;
            Data.ArmorBreakingAT = 30;
            Data.CriticalHit = 50;
            Data.CurseMagicHit = 50;
            Data.CurseMgRES = 50;
            Data.EleMagicHit= 50;
            Data.ELMRES= 50;
            Data.Evade = 20;
            Data.MagicAk = 200;
            Data.MagicPenetration= 200;
            Data.MaxDamage = 60;
            Data.MixDamage = 10;
            Data.MeDEF = 4;
            Data.Tough = 100;
            Data.Speed = 3000;
            Data.Shield = 100;
            Data.RGDEF = 100;
            Data.RangeAk = 300;
            Data.MeleeAk = 300;
            Data.PhysicalHit = 300;

            playerHeroList.Add(Data);


            FightRoundWindow.Instance.LeftPosRect[Data.SeatId].gameObject.SetActive(true);

        }

        var EnemyBase = ConfigLoader.Instance.Tables.TbEnemyModelBase.DataList;
        for (int i = 0; i <4; i++)
        {
            var EnemyData = EnemyBase[i];

            FightUnitData Data = new FightUnitData();


            Data.ID = EnemyData.Id;
            Data.ResName= EnemyData.ResName;
            Data.Name=EnemyData.Name;
            FightUnitSkill Skill1 = new FightUnitSkill(EnemyData.PassivitySkill[0].PassSkill,0,0,0, 101);
            FightUnitSkill Skill2 = new FightUnitSkill(EnemyData.PassivitySkill[0].ActiveSkill, 0, 0, 0, 101);




            Data.m_PassSkillList.Add(Skill1);
            Data.m_ActiveSkillList.Add(Skill2);

            var Atruite = EnemyData.Des;

            Data.SeatId = i;
            Data.WeaponType = 2;
            Data.Hp = Atruite[0];
            Data.Mp = Atruite[1];
            Data.MaxHp = Atruite[2];
            Data.MaxMp = Atruite[3];
            Data.MeleeAk = Atruite[4];
            Data.RangeAk = Atruite[5];
            Data.MagicAk = Atruite[6];
            Data.RGDEF = Atruite[7];
            Data.MGDEF = Atruite[8];
            Data.ELMRES = Atruite[9];
            Data.CurseMgRES = Atruite[10];
            Data.Shield= Atruite[11];
            Data.PhysicalHit = Atruite[12];
            Data.EleMagicHit = Atruite[13];
            Data.CurseMagicHit = Atruite[14];
            Data.MagicPenetration = Atruite[15];
            Data.Evade = Atruite[16];
            Data.Speed = Atruite[17];
            Data.CriticalHit = Atruite[18];
            Data.MixDamage = Atruite[19];
            Data.MaxDamage = Atruite[20];
            Data.Tough = Atruite[21];
            Data.ArmorBreakingAT = Atruite[22];

            enemyHeroList.Add(Data);

            FightRoundWindow.Instance.RightPosRect[Data.SeatId].gameObject.SetActive(true);
        }

        //FightRoundWindow.Instance.LeftPosRect


        CreateBattleWord(playerHeroList,enemyHeroList);


        //这里看有多少个战斗单位


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
