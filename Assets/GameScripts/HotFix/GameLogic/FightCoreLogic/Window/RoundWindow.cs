using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class RoundWindow : MonoBehaviour
{
    public GameObject roundStartAnim;
    public Text roundText;
    public Text logicFrameText;
    public Text quickenMultipleText;
    private int mMaxRoundid;
  
    public void RoundStart()
    {
        roundStartAnim.SetActive(true);
        mMaxRoundid = BattleWorld.Instance.roundLoigc.MaxRoundID;
        gameObject.SetActive(true);
        roundText.text = BattleWorld.Instance.roundLoigc.RoundId + "/" + mMaxRoundid;
        roundStartAnim.transform.DOScale(1, 0.3f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            roundStartAnim.transform.DOScale(0, 0f).SetDelay(0.6f);
        });
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
        logicFrameText.text = "LogicFrame:"+ FrameSyncConfig.LogicFrameid;
    }
}
