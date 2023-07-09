using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class RoundWindow : MonoBehaviour
{
    public TMP_Text roundText;
    public TMP_Text logicFrameText;
    public TMP_Text quickenMultipleText;
    private int mMaxRoundid;
  
    public void RoundStart()
    {
        mMaxRoundid = BattleWorld.Instance.roundLoigc.MaxRoundID;
        gameObject.SetActive(true);
        roundText.text = BattleWorld.Instance.roundLoigc.RoundId + "/" + mMaxRoundid;
        quickenMultipleText.text = "x" + BattleWorld.Instance.quickenMultiple;
    }
    public void NextRound(int roundid)
    {
        roundText.text = roundid.ToString() + "/" + mMaxRoundid;
    }

    public void JumpButtonClick()
    {
        HallMsgHandlerConter.Instance.SendGetBatleResultRequest(BattleWorld.Instance.BattleId);
    }
    /// <summary>
    /// 暂停游戏
    /// </summary>
    public void PauseBattleButtonClick()
    {
        BattleWorld.Instance.PauseBattle();
    }
    /// <summary>
    /// 加速游戏
    /// </summary>
    public void QuickenButtonClick()
    {
        BattleWorld.Instance.QuickenBattle();
        quickenMultipleText.text = "x"+BattleWorld.Instance.quickenMultiple;
    }
    /// <summary>
    /// 更新逻辑帧显示
    /// </summary>
    public void UpdateLogicFrameCount()
    {
        logicFrameText.text = "帧数:"+ FrameSyncConfig.LogicFrameid;
    }
}
