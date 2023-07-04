using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager
{
    /// <summary>
    /// 战斗世界
    /// </summary>
    public static BattleWorld BattleWorld { get; private set; }
    /// <summary>
    /// 初始化
    /// </summary>
    public static void Initialize()
    {
        ConfigConter.Initialized();
    }
    /// <summary>
    /// 构建战斗世界
    /// </summary>
    public static void CreateBattleWorld(List<HeroData> playerHerolist,List<HeroData> enemyHeroList)
    {
        BattleWorld = new BattleWorld();
        BattleWorld.OnCreateWorld(playerHerolist, enemyHeroList);
    }
    public static void OnUpdate()
    {
        if (BattleWorld != null)
        {
            BattleWorld.OnUpdate();
        }
    }
    /// <summary>
    /// 销毁世界
    /// </summary>
    public static void DestroyWorld()
    {
        BattleWorld.OnDestroyWorld();
    }
}
