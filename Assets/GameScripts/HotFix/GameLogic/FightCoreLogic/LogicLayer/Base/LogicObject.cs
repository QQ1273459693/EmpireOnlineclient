using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum LogicObjectState
{
    Survival,//存活中
    Death,//死亡
    SurvivalWiteing,//存活等待中

}

public class LogicObject : LogicBehaviour
{
    /// <summary>
    /// 对象状态
    /// </summary>
    public LogicObjectState objectState=LogicObjectState.Survival;
    public void SetRenderObject(RenderObject renderobj)
    {
        objectState = LogicObjectState.Survival;
        RenderObj = renderobj;
        LogicPosition = new VInt3(renderobj.gameObject.transform.position);

    }
    public override void OnDestroy()
    {
        base.OnDestroy();
#if CLIENT_LOGIC
        RenderObj.OnRelease();
#endif

    }
}
