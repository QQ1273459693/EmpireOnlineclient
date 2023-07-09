using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicTimer : ILogicBehaviour
{
    public bool workFinished;
    public int loopCount;
    public VInt delayTime;//延迟时间
    private VInt mCurAccmulatTime;//当前累计时间
    public Action OnTimerComplete;
    public LogicTimer(VInt delayTime, int loopCount, Action callback)
    {
        this.delayTime = delayTime;
        this.OnTimerComplete = callback;
        this.loopCount = loopCount;
    }
    public void OnCreate()
    {

    }
    public void OnLogicFrameUpdate()
    {
        mCurAccmulatTime += (VInt)FrameSyncConfig.LogicFrameLenms;
        //Debuger.Log("mCurAccmulatTime:"+ mCurAccmulatTime);
        if (mCurAccmulatTime >= delayTime && loopCount > 0)
        {
            OnTimerComplete?.Invoke();
            mCurAccmulatTime = 0;
            loopCount--;
            if (loopCount == 0)
                workFinished = true;
        }
    }
    public void OnDestroy()
    {

    }


}
