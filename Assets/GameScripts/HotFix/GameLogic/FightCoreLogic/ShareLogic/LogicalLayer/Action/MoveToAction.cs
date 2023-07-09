using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToAction : ActionBase
{
    private LogicObject mMoveObject;
    private VInt3 mVint3TargetPos;
    private VInt mVintTimems;
    public Action OnMoveComplete;

    private VInt3 mVint3Distance;//两者之间的距离
    private VInt mVintLerpTime = 0;
    
    public MoveToAction(LogicObject moveObject,VInt3 targetPos,VInt timems,Action moveComplete)
    {
        //Debuger.Log("MoveToAction:" + moveObject.LogicPosition );
        mMoveObject = moveObject;
        mVint3TargetPos = targetPos;
        mVintTimems = timems;
        OnMoveComplete = moveComplete;
        mVint3Distance = new VInt3(targetPos.x-moveObject.LogicPosition.x,0,targetPos.z-moveObject.LogicPosition.z);
    }
    public override void OnLogicFrameUpdate()
    {
#if CLIENT_LOGIC
        mVintLerpTime += (VInt)FrameSyncConfig.LogicFrameLenms;
        VInt lerpValue = mVintLerpTime/mVintTimems;
        //计算新的位置
        mMoveObject.LogicPosition = VInt3.Lerp(mMoveObject.LogicPosition, mVint3TargetPos, lerpValue.RawFloat);
        //Debuger.Log("MoveObjec LogicPosition:" + mMoveObject.LogicPosition + "  lerpValue:"+ lerpValue);
        if (lerpValue > VInt.one)
        {
            //移动完成
            moveComplete = true;
            OnMoveComplete?.Invoke();
            //Debuger.Log("M移动完成 logicPosition:" + mMoveObject.LogicPosition  +" targetPos:"+mVint3TargetPos);
            return;
        }
#else
        moveComplete = true;
        OnMoveComplete?.Invoke();
        //Debuger.Log("M移动完成 logicPosition:" + mMoveObject.LogicPosition + " targetPos:" + mVint3TargetPos);
        return;
#endif
    }
    public override void OnDestroy()
    {
       
    }

  
}
