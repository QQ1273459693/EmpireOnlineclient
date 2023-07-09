
using System.Collections.Generic;
using ProtoBuf;
[ProtoContract]
public class Packet
{
    [ProtoMember(1)]
    public int resultCode;
}
/// <summary>
/// 登录请求
/// </summary>
[ProtoContract]
public class LoginRequest : Packet
{
    [ProtoMember(1)]
    public string deviceid;
}
/// <summary>
/// 登录响应
/// </summary>
[ProtoContract]
public class LoginResponse : Packet
{
    [ProtoMember(1)]
    public int id;
    [ProtoMember(2)]
    public string name;

}


/// <summary>
/// 开始战斗请求
/// </summary>
[ProtoContract]
public class StartBattleRequest : Packet
{
    //英雄座位数据
    [ProtoMember(1)]
    public List<HeroSeatDataPb> heroSeatDataList;
}
/// <summary>
/// 开始战斗响应
/// </summary>
[ProtoContract]
public class StartBattleResponse : Packet
{
    //战斗id
    [ProtoMember(1)]
    public int battleid;
    //随机种子
    [ProtoMember(2)]
    public int randomSite;
    //英雄战斗数据
    [ProtoMember(3)]
    public List<BattleHeroDataPb> heroDatalist;
    //敌方英雄战斗数据
    [ProtoMember(4)]
    public List<BattleHeroDataPb> enemyDatalist;
}
/// <summary>
/// 英雄座位数据
/// </summary>
[ProtoContract]
public class HeroSeatDataPb
{
    [ProtoMember(1)]
    public int id;
    [ProtoMember(2)]
    public int seatid;//位置 座位 id
}
/// <summary>
/// 英雄战斗数据
/// </summary>
[ProtoContract]
public class BattleHeroDataPb
{
    [ProtoMember(1)] public int id;
    [ProtoMember(2)] public int seatid;//位置 座位 id
    [ProtoMember(3)] public List<int> skillidArr=new List<int>();//技能数组
    [ProtoMember(4)] public int hp;//声明值
    [ProtoMember(5)] public int atk;//攻击力
    [ProtoMember(6)] public int def;//防御力
    [ProtoMember(7)] public int agl;//敏捷
    [ProtoMember(8)] public int atkRange;
    [ProtoMember(9)] public int takeDamageRange;//受伤回怒
    [ProtoMember(10)] public int maxRage;//最大怒气

    public HeroData ToHeroData()
    {
        HeroData heroData = new HeroData();
        heroData.id = id;
        heroData.seatid = seatid;
        heroData.skillidArr = skillidArr;
        heroData.hp = hp;
        heroData.atk = atk;
        heroData.def = def;
        heroData.agl = agl;
        heroData.atkRange = atkRange;
        heroData.takeDamageRange = takeDamageRange;
        heroData.maxRage = maxRage;
        return heroData;
    }
}

/// <summary>
/// 战斗结果请求
/// </summary>
[ProtoContract]
public class BattleResultRequest : Packet
{
    [ProtoMember(1)]
    public int battleid;
}

/// <summary>
/// 战斗结果响应
/// </summary>
[ProtoContract]
public class BattleResultResponse : Packet
{
    //是否胜利
    [ProtoMember(1)]
    public bool isWin;
    [ProtoMember(2)]
    public List<RewardDataPb> rewardLsit;
}
[ProtoContract]
public class RewardDataPb
{
    [ProtoMember(1)]
    public int itemid;
    [ProtoMember(2)]
    public int count;
}
