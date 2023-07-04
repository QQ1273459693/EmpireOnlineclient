using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LogicTimer : ILogicBehaviour
{
    public VInt delayTime;
    public Action OnTimerComplete;
    public int loopCount;
    public bool workFinished;//工作是否完成
    private VInt mCurAccmulatTime;//当前累计时间

    public LogicTimer(VInt delayTime,Action callback,int loop=1)
    {
        this.delayTime = delayTime;
        this.loopCount = loop;
        this.OnTimerComplete = callback;
    }
    public void OnCreate()
    {
        
    }
    public void OnLogicFrameUpdate()
    {
        mCurAccmulatTime += (VInt)LogicFrameSyncConfig.logicFrameIntervalms;
        if (mCurAccmulatTime>=delayTime&&loopCount>0)
        {
            OnTimerComplete?.Invoke();
            mCurAccmulatTime = 0;
            loopCount--;
            if (loopCount==0)
            {
                workFinished = true;
            }
        }
    }
    public void OnDestroy()
    {
        
    }

    
}
