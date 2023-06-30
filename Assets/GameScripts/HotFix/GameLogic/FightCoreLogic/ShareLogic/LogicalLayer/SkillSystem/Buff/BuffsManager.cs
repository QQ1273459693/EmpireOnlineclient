using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffsManager : Singleton<BuffsManager>, ILogicBehaviour
{
    private List<BuffLogic> mBuffList = new List<BuffLogic>();
    public void OnCreate()
    {

    }

    public BuffLogic CreateBuff(int buffid, LogicObject buffaddTarget, LogicObject attacker)
    {
        Debuger.Log("创建一个Buff  buffid:" + buffid);
        BuffLogic buff = new BuffLogic(buffid, buffaddTarget, attacker);
        buff.OnCreate();
        mBuffList.Add(buff);

        return buff;
    }
    public void OnLogicFrameUpdate()
    {
        //这里使用正序更新 倒序移除的方法是为了在 buff逻辑帧顺序时与服务端保持一致，否则会出现buff 随机数运算错乱问题
        for (int i = 0; i < mBuffList.Count; i++)
        {
            mBuffList[i].OnLogicFrameUpdate();
        }

        for (int i = mBuffList.Count - 1; i >= 0; i--)
        {
            if (mBuffList[i].objectState == LogicObjectState.Death)
            {
                mBuffList[i].OnDestroy();
                mBuffList.Remove(mBuffList[i]);
            }
        }
    }
    public void RemoveBuff(BuffLogic buff)
    {
        if (mBuffList.Contains(buff))
        {
            mBuffList.Remove(buff);
        }
    }
    public void DestroyBuff(BuffLogic buff)
    {
        buff.ownerHero.RemoveBuff(buff);
    }
    public void OnDestroy()
    {
        for (int i = 0; i < mBuffList.Count; i++)
        {
            mBuffList[i].OnDestroy();
        }
        mBuffList.Clear();
    }
}