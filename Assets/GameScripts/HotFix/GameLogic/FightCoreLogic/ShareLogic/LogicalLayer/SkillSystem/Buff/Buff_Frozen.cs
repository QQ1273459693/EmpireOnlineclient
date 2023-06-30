using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 冰冻buff
/// </summary>
public class Buff_Frozen : BuffLogic
{

    public Buff_Frozen(int buffid, LogicObject owner, LogicObject attacker) : base(buffid, owner, attacker) { }
    public override void OnCreate()
    {
        base.OnCreate();
        ownerHero.SetAnimState(AnimState.StopAnim);
    }
    public override void OnLogicFrameUpdate()
    {

    }
    public override void RoundEndEvent()
    {
        //base.RoundEndEvent();
        ////新回合开始，如果当前buff已经超过了最大持续回合，就直接移除掉
        //if (objectState == LogicObjectState.Survival)
        //{
        //    if (mCurBuffSurvival >= BuffConfig.buffDurationRound)
        //    {
        //        ownerHero.SetAnimState(AnimState.RePlayAnim);
        //        OnDestroy();
        //    }
        //}
    }
}
