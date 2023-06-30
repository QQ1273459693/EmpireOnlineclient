using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderObject : RenderBehaviour
{
    public virtual void SetLogicObject(LogicObject logicObj)
    {
        LogicObject = logicObj;
    }
 
    public virtual void Update()
    {
        if (LogicObject == null)
        {
            return;
        }
        transform.position = Vector3.Lerp(transform.position, LogicObject.LogicPosition.vec3, BattleWorld.Instance.DeltaTime);
    }
    public override void OnRelease()
    {
        base.OnRelease();
        GameObject.Destroy(gameObject);
    }
}
