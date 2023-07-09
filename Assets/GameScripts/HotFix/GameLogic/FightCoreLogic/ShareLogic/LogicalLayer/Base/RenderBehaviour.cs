using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;

public class RenderBehaviour : BehaviourSingleton<RenderBehaviour>
{
    public GameObject gameObject { get; protected set; }
    public LogicObject LogicObject { get; protected set; }

    public virtual void OnCreate() { }
    public virtual void OnRelease() { }
}
