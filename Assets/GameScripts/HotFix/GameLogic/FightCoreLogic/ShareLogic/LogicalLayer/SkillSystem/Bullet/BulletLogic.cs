using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLogic : BulletBase
{
    public BulletLogic(LogicObject attacker, LogicObject bulletTarget, VInt flightTime, Action onHitComplete) :base(attacker,bulletTarget, flightTime, onHitComplete)
    {
      
    }
    public override void OnCreate()
    {
        base.OnCreate();
        MoveToAction action = new MoveToAction(this,mAttackTarget.LogicPosition, mFlightTime, BulletMoveComplete);
        ActionManager.Instance.RunAction(action);
    }
    public void BulletMoveComplete()
    {
        OnHitComplete?.Invoke();
        BulletManager.Instance.RemoveBullet(this);
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
#if RENDER_LOGIC
        RenderObj.OnRelease();
#endif
    }
}
