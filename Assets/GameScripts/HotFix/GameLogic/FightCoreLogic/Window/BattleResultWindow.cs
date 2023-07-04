using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultWindow : MonoBehaviour
{
    public Text resultText;
    public void SetBattleResult(bool isWin)
    {
        gameObject.SetActive(true);
        resultText.text = isWin ? "Ê¤Àû" : "Ê§°Ü";
    }
    public void BackPlayButtonClick()
    {
        gameObject.SetActive(false);
        string json = PlayerPrefs.GetString(BattleDataModel.Key);
        BattleDataModel battleData= Newtonsoft.Json.JsonConvert.DeserializeObject<BattleDataModel>(json);
        WorldManager.CreateBattleWorld(battleData.herolist, battleData.enemyList);
    }
    public void OnResetGameButtonClick()
    {
        gameObject.SetActive(false);
        List<HeroData> playerHeroList = new List<HeroData>();
        List<HeroData> enemyHeroList = new List<HeroData>();
        List<int> heroidlist = new List<int> { 101, 102, 103, 104, 105, 501, 502, 503, 504, 505 };

        for (int i = 0; i < heroidlist.Count; i++)
        {
            HeroData hero = ConfigConter.GetHeroData(heroidlist[i]);
            if (i < 5)
            {
                hero.seatid = i;
                playerHeroList.Add(hero);
            }
            else
            {
                //µÐ·½Ó¢ÐÛ
                hero.seatid = i - 5;
                enemyHeroList.Add(hero);
            }
        }
        
        WorldManager.CreateBattleWorld(playerHeroList, enemyHeroList);
    }
}
