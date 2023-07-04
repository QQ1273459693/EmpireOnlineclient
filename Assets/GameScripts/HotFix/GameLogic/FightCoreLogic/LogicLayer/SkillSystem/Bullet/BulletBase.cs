using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase:LogicObject
{
    protected LogicObject mAttacker;//������
    protected LogicObject mAttackTarget;//����Ŀ��
    protected VInt mFlightTime;//
    protected Action OnHitComplete;//�ӵ����лص�

    public BulletBase(LogicObject attacker,LogicObject bulletTarget,VInt flightTime,Action onhitComplete)
    {
        mAttacker = attacker;
        mAttackTarget = bulletTarget;
        mFlightTime = flightTime;
        OnHitComplete = onhitComplete;
    }
}
