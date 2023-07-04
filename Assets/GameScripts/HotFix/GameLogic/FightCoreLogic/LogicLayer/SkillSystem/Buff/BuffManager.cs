using GameBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : SingletionMono<BuffManager>, ILogicBehaviour
{
    private List<BuffLogic> mBuffList = new List<BuffLogic>();
    public void OnCreate()
    {
        
    }
    /// <summary>
    /// 创建buff
    /// </summary>
    /// <param name="buffid"></param>
    /// <param name="owner"></param>
    /// <param name="attacker"></param>
    /// <returns></returns>
    public BuffLogic CreateBuff(int buffid,LogicObject owner,LogicObject attacker)
    {
        Debuger.Log("创建一个buffID:"+buffid);
        BuffLogic buff = new BuffLogic(buffid,owner,attacker);
        buff.OnCreate();
        mBuffList.Add(buff);
        return buff;
    }
    public void OnLogicFrameUpdate()
    {
        for (int i = 0; i < mBuffList.Count; i++)
        {
            mBuffList[i].OnLogicFrameUpdate();
        }
        for (int i = mBuffList.Count-1; i >=0; i--)
        {
            BuffLogic buff = mBuffList[i];
            if (buff.objectState== LogicObjectState.Death)
            {
                buff.OnDestroy();
                mBuffList.Remove(buff);
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
    public void OnDestroy()
    {

    }
    /// <summary>
    /// 销毁buff
    /// </summary>
    public void DestroyBuff(BuffLogic buff)
    {
        buff.targetHero.RemoveBuff(buff);
    }
}
