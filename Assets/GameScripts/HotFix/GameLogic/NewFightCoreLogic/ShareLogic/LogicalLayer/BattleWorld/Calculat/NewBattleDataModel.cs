using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBattleDataModel
{
    public const string Key = "BattleDataKEY";
    public int battleSite;//战斗种子
    public List<HeroData> herolist;//英雄列表
    public List<HeroData> enemylist;//敌人列表
}
