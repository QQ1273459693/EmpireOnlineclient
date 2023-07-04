using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 只负责最基础的属性和接口.不负责实现
/// </summary>
public class LogicBehaviour
{
    public RenderObject RenderObj { get;protected set; }
    public VInt3 LogicPosition { get; set; }//逻辑位置
    public Action OnActionEndListener { get; set; }

    public virtual void OnCreate() { }

    public virtual void OnLogicFrameUpdate() { }

    public virtual void OnDestroy() { }
    
    /// <summary>
    /// 回合开始事件
    /// </summary>
    /// <param name="round"></param>
    public virtual void RoundStartEvent(int round) { }
    /// <summary>
    /// 回合结束事件
    /// </summary>
    public virtual void RoundEndEvent() { }
    /// <summary>
    /// 开始行动
    /// </summary>
    public virtual void BeginAction() { }
    /// <summary>
    /// 行动结束
    /// </summary>
    public virtual void ActionEnd() { }
}
