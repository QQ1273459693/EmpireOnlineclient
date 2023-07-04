using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDataModel
{
    public const string Key = "BattleDataKey";
    public int battleSite;//战斗随机种子
    public List<HeroData> herolist;//我方英雄数据列表
    public List<HeroData> enemyList;//敌方英雄数据列表
}
