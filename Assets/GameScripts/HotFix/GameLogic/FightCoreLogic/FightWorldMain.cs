using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    public class FightWorldMain : BehaviourSingleton<FightWorldMain>// MonoBehaviour
    {
        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
            Log.Debug("开始构建战斗世界");
            WorldManager.Initialize();
            List<HeroData> playerHeroList = new List<HeroData>();
            List<HeroData> enemyHeroList = new List<HeroData>();
            enemyHeroList.AddRange(ConfigConter.HeroDatalist);
            playerHeroList.Add(ConfigConter.GetHeroData(5));

            WorldManager.CreateBattleWorld(playerHeroList, enemyHeroList);
        }
        //void Start()
        //{
        //    WorldManager.Initialize();
        //    List<HeroData> playerHeroList = new List<HeroData>();
        //    List<HeroData> enemyHeroList = new List<HeroData>();
        //    enemyHeroList.AddRange(ConfigConter.HeroDatalist);
        //    playerHeroList.Add(ConfigConter.GetHeroData(5));
        //    //List<int> heroidlist = new List<int> { 101, 102, 103, 104, 105, 501, 502, 503, 504, 505 };

        //    //for (int i = 0; i < heroidlist.Count; i++)
        //    //{
        //    //    HeroData hero = ConfigConter.GetHeroData(heroidlist[i]);
        //    //    if (i < 5)
        //    //    {
        //    //        hero.seatid = i;
        //    //        playerHeroList.Add(hero);
        //    //    }
        //    //    else
        //    //    {
        //    //        敌方英雄
        //    //        hero.seatid = i - 5;
        //    //        enemyHeroList.AddRange(ConfigConter.HeroDatalist); .Add(hero);
        //    //    }
        //    //}

        //    WorldManager.CreateBattleWorld(playerHeroList, enemyHeroList);
        //}

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
            Debug.Log("开始进行循环帧");
            WorldManager.OnUpdate();
        }
        //void Update()
        //{
        //    WorldManager.OnUpdate();
        //}
        public void OnDestroy()
        {
            WorldManager.DestroyWorld();
        }
    }
}
