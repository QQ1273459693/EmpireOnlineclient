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
}
