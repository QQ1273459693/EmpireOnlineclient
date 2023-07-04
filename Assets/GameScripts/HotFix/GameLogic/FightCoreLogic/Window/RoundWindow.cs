using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoundWindow : MonoBehaviour
{
    public TMP_Text roundText;
    public TMP_Text logicFrameText;
    public TMP_Text quickenMultipleText;//加速倍数
    public TMP_Text FightInfoText;//场上信息文本
    private int MaxRoundId=15;
    public void Update()
    {
        UpdateLogicFrameCount();
    }
    public void RoundStart(int roundId)
    {
        roundText.text = $"{roundId}/{MaxRoundId}";
    }
    public void NextRound(int roundid)
    {
        roundText.text = $"{roundid}/{MaxRoundId}";
    }
    public void OnButtonGamePause()
    {
        WorldManager.BattleWorld.PauseBattle();
    }
    public void OnQuckenBattle()
    {
        //WorldManager.BattleWorld.QuickenBattle();
        //quickenMultipleText.text = "X" + WorldManager.BattleWorld.QuickenMultiple;
    }
    public void UpdateLogicFrameCount()
    {
        logicFrameText.text = "帧数:" + LogicFrameSyncConfig.LogicFrameid;
    }
    public void UpdateFightInfoText(string Info)
    {
        FightInfoText.text = Info;
    }
}
