using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletRender : RenderObject
{
    private HeroRender mTarget;

    public void SetBulletTarget(HeroRender heroRender)
    {
        mTarget = heroRender;
    }    
    public override void Update()
    {
        base.Update();
        transform.LookAt(mTarget.transform,Vector3.forward);
    }
}
