using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderObject : RenderBehaviour
{
    public virtual void SetLogicObject(LogicObject logicObj,GameObject RendObj)
    {
        LogicObject = logicObj;
        gameObject = RendObj;
    }
    //public virtual void Update()
    //{
    //    if (LogicObject == null)
    //    {
    //        return;
    //    }
    //    gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, LogicObject.LogicPosition.vec3, BattleWorld.Instance.DeltaTime);
    //}
    public override void OnRelease()
    {
        base.OnRelease();
        //GameObject.Destroy(gameObject);
    }
}
