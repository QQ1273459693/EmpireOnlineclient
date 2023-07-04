using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;

public class RenderObject : BehaviourSingleton<RenderObject>
{
    public LogicObject LogicObj { get; private set; }
    public GameObject gameObject;
    public virtual void SetLogicObject(LogicObject logicobj,GameObject obj)
    {
        LogicObj = logicobj;
        gameObject = obj;
    }
    public override void Update()
    {
        base.Update();
        if (LogicObj == null)
        {
            return;
        }
        //做一个帧同步动画,来达到画面流畅性
        //transform.position = Vector3.Lerp(transform.position,LogicObj.LogicPosition.vec3,BattleWorld.DeltaTime);
    }
    public virtual void OnRelease()
    {
        //GameObject.Destroy(gameObject,3);
    }
}
