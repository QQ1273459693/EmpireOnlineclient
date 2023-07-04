using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase:LogicObject
{
    protected LogicObject mAttacker;//攻击者
    protected LogicObject mAttackTarget;//攻击目标
    protected VInt mFlightTime;//
    protected Action OnHitComplete;//子弹击中回调

    public BulletBase(LogicObject attacker,LogicObject bulletTarget,VInt flightTime,Action onhitComplete)
    {
        mAttacker = attacker;
        mAttackTarget = bulletTarget;
        mFlightTime = flightTime;
        OnHitComplete = onhitComplete;
    }
}
