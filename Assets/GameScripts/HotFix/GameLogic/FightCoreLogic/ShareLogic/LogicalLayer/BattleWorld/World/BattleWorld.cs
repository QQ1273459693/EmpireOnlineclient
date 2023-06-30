using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleWorld
{
    /// <summary>
    /// 战斗id
    /// </summary>
    public int BattleId { get; private set; }
    /// <summary>
    /// 随机种子
    /// </summary>
    public int RandomSite { get; private set; }
    /// <summary>
    /// 是否胜利
    /// </summary>
    public bool IsWin { get;private set; }
    /// <summary>
    /// 战斗结束回调
    /// </summary>

    private System.Action<BattleWorld> OnBattleEndListener;

    public static BattleWorld Instance;
    public HeroLogicCtrl heroLogic;
    public RoundLogicCtrl roundLoigc;

    public bool IsPlayBack { get; private set; }//是否是回放
    public bool pauseBattle;//暂停游戏
    public bool battleEnd;//战斗结束
    public int quickenMultiple = 1;//加速战斗
    private const int MAXQUICKENCOUNT = 3;//最大加速倍数
    private float mAccumulatedLoigcRunTime;//累计运行逻辑的时间
    private float mNextLogicFrameTime;//下一个逻辑帧开始的时间
    public float DeltaTime;
    public void OnCreateWorld(List<HeroData> herolist, List<HeroData> enemylist, int randomSite, int battleid, System.Action<BattleWorld> battleEndCallback = null,bool isPlayBack=false)
    {
        BattleId = battleid;
        RandomSite = randomSite;
        IsPlayBack = isPlayBack;
        OnBattleEndListener = battleEndCallback;
#if CLIENT_LOGIC
        //保存战斗快照，用于战斗回放
        BattleDataModel battleData = new BattleDataModel { battleSite = randomSite, enemylist = enemylist, herolist = herolist };
        string battleJson = Newtonsoft.Json.JsonConvert.SerializeObject(battleData);
        PlayerPrefs.SetString(BattleDataModel.Key, battleJson);
#endif
        Instance = this;
        battleEnd = false;
        Time.timeScale =  quickenMultiple=1;
        FrameSyncConfig.LogicFrameid = 0;
        LogicRandom.Instance.InitRandom(randomSite);


        heroLogic = new HeroLogicCtrl();
        roundLoigc = new RoundLogicCtrl();

        heroLogic.OnCreate(herolist, enemylist);
        roundLoigc.OnCreate();

    }
    public void Update()
    {
        //游戏暂停
        if (pauseBattle || battleEnd)
        {
            return;
        }

#if CLIENT_LOGIC

        mAccumulatedLoigcRunTime += Time.deltaTime;
        //控制帧数 保证所有的设备下帧数的一致性，并进行追帧操作
        while (mAccumulatedLoigcRunTime > mNextLogicFrameTime)
        {
            //更新逻辑帧显示
            BattleWordNodes.Instance.roundWindow.UpdateLogicFrameCount();

            //一个逻辑帧
            LogicFrameUpdate();
            //计算下一个逻辑帧的时间
            mNextLogicFrameTime += FrameSyncConfig.LogicFrameLen;
            //逻辑帧id自增
            FrameSyncConfig.LogicFrameid++;
        }
        // mAccumulatedLoigcRunTime < mNextLogicFrameTime  当前累计运行时间，不到下一帧的时间
        // (mAccumulatedLoigcRunTime + FrameSyncConfig.LogicFrameLen - mNextLogicFrameTime) 计算到下一帧剩余的时间 
        // mNextLogicFrameTime=0.066  mAccumulatedLoigcRunTime=0.020 
        // mAccumulatedLoigcRunTime + FrameSyncConfig.LogicFrameLen=0.086
        // (mAccumulatedLoigcRunTime + FrameSyncConfig.LogicFrameLen - mNextLogicFrameTime) => 0.086-0.066=0.20   0.20/0.066=0.3
        //下一个Update 加如 mAccumulatedLoigcRunTime=0.040  运算结果=0.040+0.066-0.066=0.040/0.066=0.6
        //2帧时
        //下一个Update 加如 mAccumulatedLoigcRunTime=0.080  运算结果=(0.080+0.066=0.140)-0.132=0.008/0.066=0.012
        //下一个Update 加如 mAccumulatedLoigcRunTime=0.1  运算结果=(0.1+0.066=0.166)-0.132=0.034/0.066=0.515
        DeltaTime = (mAccumulatedLoigcRunTime + FrameSyncConfig.LogicFrameLen - mNextLogicFrameTime) / FrameSyncConfig.LogicFrameLen;
#else
        //一个逻辑帧
        LogicFrameUpdate();

#endif
    }
    public void LogicFrameUpdate()
    {
        //Debuger.Log("LogicFrameid:"+FrameSyncConfig.LogicFrameid + " mNextLogicFrameTime:"+ mNextLogicFrameTime);
        heroLogic?.OnLogicFrameUpdate();
        ActionManager.Instance.OnLogicFrameUpdate();
        LogicTimeManager.Instance.OnLogicFrameUpdate();
        BulletManager.Instance.OnLogicFrameUpdate();
        BuffsManager.Instance.OnLogicFrameUpdate();
    }
    public void PauseBattle()
    {
        Debuger.Log("游戏暂停");
#if CLIENT_LOGIC
        pauseBattle = !pauseBattle;
        Time.timeScale = pauseBattle ? 0 : quickenMultiple;
#endif
    }
    public void QuickenBattle()
    {
#if CLIENT_LOGIC
        quickenMultiple++;
        if (quickenMultiple > MAXQUICKENCOUNT)
        {
            quickenMultiple = 1;
        }
        Time.timeScale = quickenMultiple;
#endif
    }
    public void BattleEnd(BattleResultResponse result)
    {
        IsWin = result.isWin;
        Debuger.Log("战斗结束:" + (IsWin?"胜利":"失败"));

        string heroHPStr = " ";
        for (int i = 0; i < heroLogic.AllHeroList.Count; i++)
            heroHPStr += heroLogic.AllHeroList[i].id + " 英雄血量:" + heroLogic.AllHeroList[i].HP + "    怒气值:" + heroLogic.AllHeroList[i].Rage + " 是否被控制" + heroLogic.AllHeroList[i].IsBeContrl() + "\n";

        Debuger.Log("战斗结束 战斗数据：  \n所有英雄生命值：\n" + heroHPStr);

        battleEnd = true;
      
        OnBattleEndListener?.Invoke(this);
#if CLIENT_LOGIC
        //显示战斗界面
        BattleWordNodes.Instance.battleResultWindow.SetBattleRsult(result.isWin);
#endif
        DestroyWorld();
        BuffsManager.Instance.OnDestroy();
        SkillManager.Instance.OnDestroy();
    }
 
    public void DestroyWorld()
    {

        heroLogic.OnDestroy();
        roundLoigc.OnDestroy();
        BuffsManager.Instance.OnDestroy();
        SkillManager.Instance.OnDestroy();
        LogicTimeManager.Instance.OnDestroy();
    }
}
