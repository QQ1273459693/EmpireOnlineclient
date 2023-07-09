using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LogicObjectState
{
    Survival,
    Death,
    SurvialWiteing,//存活等待中
}
/// <summary>
/// 实现部分逻辑对象公用的接口和属性
/// </summary>
public class LogicObject : LogicBehaviour
{

    public LogicObjectState objectState = LogicObjectState.Survival;
    public void SetRenderObject(RenderObject renderObj)
    {
        objectState = LogicObjectState.Survival;
        RenderObj = renderObj;
        LogicPosition = new VInt3(renderObj.gameObject.transform.position);
        //LogicPosition = VInt3.zero;
    }
    public override void RoundStartEvent(int round)
    {
        base.RoundStartEvent(round);
        CurRound = round;
    }
    public override void ActionEnd()
    {
        base.ActionEnd();
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        OnActionEndListener = null;

#if RENDER_LOGIC
        RenderObj.OnRelease();
#endif
    }
}
