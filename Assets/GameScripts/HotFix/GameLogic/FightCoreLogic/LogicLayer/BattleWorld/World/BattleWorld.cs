using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;

public class BattleWorld
{
    public static bool battleEnd;
    public HeroLogicCtrl heroLogic;
    public RoundLogicCtrl roundLogic;

    public int QuickenMultiple = 1;//战斗加速倍数
    private int MaxQuickenMutiple = 3;//最大加速倍数
    public bool battlePause;//战斗暂停
    private float mAccLogicRuntime;//逻辑帧累计运行时间
    private float mNextLogicFrameTime;//下一个逻辑帧时间
    public static float DeltaTime;//动画缓动时间
    public void OnCreateWorld(List<HeroData> playerHerolist,List<HeroData> enemyHeroList)
    {
        heroLogic = new HeroLogicCtrl();
        roundLogic=new RoundLogicCtrl();

        heroLogic.OnCreate(playerHerolist, enemyHeroList);
        roundLogic.OnCreate();
        battleEnd = false;
        QuickenMultiple = 1;
        LogicFrameSyncConfig.LogicFrameid = 0;
        DeltaTime = 0;
#if CLIENT_LOGIC
        BattleDataModel dataModel = new BattleDataModel
        {
            herolist = playerHerolist,
            enemyList = enemyHeroList
        };
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(dataModel);
        PlayerPrefs.SetString(BattleDataModel.Key, json);
        //序列化有问题
#endif
    }
    public void OnUpdate()
    {
        if (battleEnd|| battlePause)
        {
            //战斗结束或者暂停
            return;
        }
#if CLIENT_LOGIC
        //逻辑帧运行时间累加
        mAccLogicRuntime += Time.deltaTime;
        //当前逻辑帧累计时间 如果大于下一个逻辑帧开始时间,就需要更新逻辑帧
        //控制帧数,保证所有设备的逻辑帧数的一致性,并进行追帧操作
        while (mAccLogicRuntime>mNextLogicFrameTime)
        {
            //更新逻辑帧
            OnLogicFrameUpdate();
            //计算下一个逻辑帧的时间
            mNextLogicFrameTime += LogicFrameSyncConfig.LogicFrameInterval;
            //逻辑帧id自增加
            LogicFrameSyncConfig.LogicFrameid++;

            //Debuger.Log("LogicFrameSyncID:"+LogicFrameSyncConfig.LogicFrameid);
        }
        DeltaTime = (mAccLogicRuntime + LogicFrameSyncConfig.LogicFrameInterval - mNextLogicFrameTime) / LogicFrameSyncConfig.LogicFrameInterval;
#else
        OnLogicFrameUpdate();
#endif
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debuger.Log("计时开始:"+Time.realtimeSinceStartup);
            heroLogic.heroList[0].PlayAnim("Attack");
            MoveToAction moveto = new MoveToAction(heroLogic.heroList[0], heroLogic.enemyList[0].LogicPosition, 1000, () =>
            {
                Debuger.Log("计时结束:" + Time.realtimeSinceStartup);
                Debuger.Log("移动完成测试:"+ heroLogic.heroList[0].LogicPosition);
                SkillEffect effect= ResourceManager.Instance.LoadObject<SkillEffect>("Prefabs/SkillEffect/Effect_RenMa_hit");
                effect.SetFeectPos(heroLogic.enemyList[0].LogicPosition);
                //heroLogic.enemyList[0].DamageHp(30);
            });
            ActionManager.Instance.RunAction(moveto);
            LogicTimerManager.Instance.DelayCall(700, () =>
            {
                //heroLogic.enemyList[0].DamageHp(15);
            });
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveToAction moveto = new MoveToAction(heroLogic.heroList[0],new VInt3(BattleWorldNodes.Instance.heroTransArr[0].position), 1000, () =>
            {
                Debuger.Log("移动完成测试:" + heroLogic.heroList[0].LogicPosition);
            });
            ActionManager.Instance.RunAction(moveto);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            SkillManager.Instance.ReleaseSkill(1010, heroLogic.heroList[0],true);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            heroLogic.heroList[0].TryClearRage();
            SkillManager.Instance.ReleaseSkill(1011, heroLogic.heroList[0], true);
            heroLogic.heroList[0].UpdateAnger(0);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            heroLogic.heroList[3].TryClearRage();
            SkillManager.Instance.ReleaseSkill(1041, heroLogic.heroList[3], true);
            heroLogic.heroList[3].UpdateAnger(0);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            heroLogic.heroList[3].TryClearRage();
            SkillManager.Instance.ReleaseSkill(1040, heroLogic.heroList[3], true);
            heroLogic.heroList[3].UpdateAnger(0);
        }
    }
    /// <summary>
    /// 逻辑帧更新
    /// </summary>
    public void OnLogicFrameUpdate()
    {
        heroLogic?.OnLogicFrameUpdate();
        roundLogic?.OnLogicFrameUpdate();
        ActionManager.Instance?.OnLogicFrameUpdate();
        LogicTimerManager.Instance?.OnLogicFrameUpdate();
        BulletManager.Instance?.OnLogicFrameUpdate();
        BuffManager.Instance?.OnLogicFrameUpdate();
    }
    /// <summary>
    /// 战斗暂停
    /// </summary>
    public void PauseBattle()
    {
#if CLIENT_LOGIC
        battlePause = !battlePause;
        Time.timeScale = battlePause ? 0 : QuickenMultiple;
#endif
    }
    public void QuickenBattle()
    {
#if CLIENT_LOGIC
        QuickenMultiple++;
        if (QuickenMultiple>MaxQuickenMutiple)
        {
            QuickenMultiple = 1;
        }
        Time.timeScale = QuickenMultiple;
#endif
    }
    /// <summary>
    /// 战斗结束
    /// </summary>
    /// <param name="isWin"></param>
    public void BattleEnd(bool isWin)
    {
        battleEnd = true;
# if CLIENT_LOGIC
        BattleWorldNodes.Instance.battleResultWindow.SetBattleResult(isWin);
#endif
        OnDestroyWorld();
    }
    /// <summary>
    /// 销毁世界
    /// </summary>
    public void OnDestroyWorld()
    {
        heroLogic.OnDestroy();
        roundLogic.OnDestroy();
        SkillManager.Instance.OnDestroy();
        LogicTimerManager.Instance.OnDestroy();
        ActionManager.Instance.OnDestroy();
    }

}
