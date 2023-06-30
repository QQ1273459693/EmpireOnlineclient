using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroData
{
    public HeroData()
    {

    }
    public int id;
    public int seatid;//位置 座位 id
    
    public int[] skillidArr;//技能数组
    public int hp;//声明值
    public int atk;//攻击力
    public int def;//防御力
    public int agl;//敏捷
    public int atkRange;
    public int takeDamageRange;
    public int maxRage;//最大怒气
    public BattleHeroDataPb ToBattleHeroData()
    {
        BattleHeroDataPb heroDataPb = new BattleHeroDataPb();
        heroDataPb.id = id;
        heroDataPb.seatid = seatid;
        heroDataPb.skillidArr = skillidArr;
        heroDataPb.hp = hp;
        heroDataPb.atk = atk;
        heroDataPb.def = def;
        heroDataPb.agl = agl;
        heroDataPb.atkRange = atkRange;
        heroDataPb.takeDamageRange = takeDamageRange;
        heroDataPb.maxRage = maxRage;
        return heroDataPb;
    }
}
