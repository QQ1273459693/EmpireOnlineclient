using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 只负责提供最基础的属性和接口，不参与实现
/// </summary>
public abstract class LogicBehaviour
{
    public RenderObject RenderObj { get; protected set; } //渲染对象
    public VInt3 LogicPosition { get; set; }//逻辑位置
    public int CurRound { get; protected set; }
    /// <summary>
    /// 行动结束回调
    /// </summary>
    public Action OnActionEndListener { get; set; }

    public virtual void OnCreate() { }
    public virtual void OnLogicFrameUpdate() { }
    public virtual void OnDestroy() { }

    /// <summary>
    /// 回合开始时事件
    /// </summary>
    public virtual void RoundStartEvent(int round) { }
    /// <summary>
    /// 回合结束事件
    /// </summary>
    public virtual void RoundEndEvent() { }
    /// <summary>
    /// 开始行动,分自动技能和主动技能
    /// </summary>
    public virtual void BeginAction(bool isAutoSkill) { }
    /// <summary>
    /// 行动结束
    /// </summary>
    public virtual void ActionEnd() { }

}
