using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultWindow : MonoBehaviour
{
    public Text resultText;

    void Start()
    {

    }

    public void SetBattleRsult(bool iswin)
    {
        gameObject.SetActive(true);
        resultText.text = iswin ? "胜利" : "失败";
    }
    /// <summary>
    /// 回放
    /// </summary>
    public void BackPlayButtonClick()
    {
        gameObject.SetActive(false);
        string json = PlayerPrefs.GetString(BattleDataModel.Key);
        BattleDataModel battleData = Newtonsoft.Json.JsonConvert.DeserializeObject<BattleDataModel>(json);
        //WorldManager.CreateBattleWord(battleData.herolist,battleData.enemylist,battleData.battleSite);
        WorldManager.CreateBattleWord(WorldManager.lastheroDataList, WorldManager.lastEnemyDataList, WorldManager.lastRandomSiteid, WorldManager.lastBattleid,null, true);
    }
    /// <summary>
    /// 重新开始
    /// </summary>
    public void ResetPlayButtonClick()
    {
        gameObject.SetActive(false);

        ////创建英雄
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
        //WorldManager.CreateBattleWord(herolist, enemylist, Random.Range(0, 100));

        WorldManager.CreateBattleWord(WorldManager.lastheroDataList,WorldManager.lastEnemyDataList,WorldManager.lastRandomSiteid,WorldManager.lastBattleid);
    }
}
