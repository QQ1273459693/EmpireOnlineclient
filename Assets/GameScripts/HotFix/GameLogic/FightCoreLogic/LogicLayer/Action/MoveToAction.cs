using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveToAction : ActionBase
{
    private LogicObject mMoveObject;//移动的对象
    private VInt3 mVint3TargetPos;//移动到的位置
    private VInt mVintTimes;//整个移动需要的事件
    public Action OnMoveComplete;//移动完成的回调
    private VInt3 mOriginPos;
    private VInt mVintLerpTime = 0;
    /// <summary>
    /// 移动到目标位置的行动
    /// </summary>
    /// <param name="moveObject">移动的对象</param>
    /// <param name="targetPos">目标位置</param>
    /// <param name="time3">移动所需事件 ms</param>
    /// <param name="moveComplete">移动完成回调</param>
    public MoveToAction(LogicObject moveObject,VInt3 targetPos,VInt time3,Action moveComplete)
    {
        mMoveObject = moveObject;
        mOriginPos = moveObject.LogicPosition;
        mVint3TargetPos = targetPos;
        mVintTimes = time3;
        OnMoveComplete = moveComplete;
    }
    public override void OnLogicFrameUpdate()
    {
        base.OnLogicFrameUpdate();
#if CLIENT_LOGIC
        mVintLerpTime += (VInt)LogicFrameSyncConfig.logicFrameIntervalms;
        VInt lerpValue = mVintLerpTime / mVintTimes;
        //计算新的位置
        mMoveObject.LogicPosition = VInt3.Lerp(mOriginPos, mVint3TargetPos, lerpValue.RawFloat);
        if (lerpValue>VInt.one)
        {
            //行动完成
            OnMoveComplete?.Invoke();
            actionComplete = true;
            return;
        }

#else
            OnMoveComplete?.Invoke();
            actionComplete = true;
#endif
    }
}
