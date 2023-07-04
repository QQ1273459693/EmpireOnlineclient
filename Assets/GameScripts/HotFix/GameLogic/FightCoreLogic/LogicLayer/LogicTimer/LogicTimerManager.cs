using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameBase;

public class LogicTimerManager : Singleton<LogicTimerManager>, ILogicBehaviour
{
    private List<LogicTimer> mLogicTimerList = new List<LogicTimer>();
    public void OnCreate()
    {

    }
    public void DelayCall(VInt delayTime, Action callBat, int loop = 1)
    {
#if CLIENT_LOGIC
        LogicTimer logicTimer = new LogicTimer(delayTime,callBat,loop);
        mLogicTimerList.Add(logicTimer);
#else
        //服务器立即触发回调,无需延迟
        for (int i = 0; i < loop; i++)
        {
            callBat?.Invoke();
        }
#endif
    }
    public void OnDestroy()
    {
        mLogicTimerList.Clear();
    }

    public void OnLogicFrameUpdate()
    {
        for (int i = 0; i < mLogicTimerList.Count; i++)
        {
            mLogicTimerList[i].OnLogicFrameUpdate();
        }
        //检测是否有完成工作的计时器,如果有就进行移除
        for (int i = mLogicTimerList.Count-1; i==0; i--)
        {
            if (mLogicTimerList[i].workFinished)
            {
                mLogicTimerList.Remove(mLogicTimerList[i]);
            }
        }
    }
}
