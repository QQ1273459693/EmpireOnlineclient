using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : LogicObject 
{
    protected LogicObject mAttacker;//攻击者
    protected LogicObject mAttackTarget;//攻击目标;
    protected Action OnHitComplete;
    protected VInt mFlightTime;//子弹飞行时间
    public BulletBase(LogicObject attacker, LogicObject bulletTarget,VInt flightTime, Action onHitComplete)
    {
        mAttacker = attacker;
        mAttackTarget = bulletTarget;
        OnHitComplete = onHitComplete;
        mFlightTime = flightTime;
    }

}
