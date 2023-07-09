using GameBase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicTimeManager : Singleton<LogicTimeManager>,ILogicBehaviour
{
    private List<LogicTimer> mLogicTiemrList = new List<LogicTimer>();
    public void OnCreate()
    {

    }
    public void DelayCall(VInt delayTime, Action callback, int loopCount = 1)
    {
#if CLIENT_LOGIC
        LogicTimer logicTimer = new LogicTimer(delayTime, loopCount, callback);
        mLogicTiemrList.Add(logicTimer);
#else
        //服务端立即触发调用无需延迟
        for (int i = 0; i < loopCount; i++)
        {
            callback?.Invoke();
        }
#endif
    }
    public void OnLogicFrameUpdate()
    {
        for (int i = 0; i < mLogicTiemrList.Count; i++)
        {
            mLogicTiemrList[i].OnLogicFrameUpdate();
        }
        for (int i = mLogicTiemrList.Count - 1; i >= 0; i--)
        {
            if (mLogicTiemrList[i].workFinished)
                mLogicTiemrList.Remove(mLogicTiemrList[i]);
        }
    }
    public void OnDestroy()
    {
        mLogicTiemrList.Clear();
    }

   
}
