using GameBase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : Singleton<BulletManager>, ILogicBehaviour
{
    public List<BulletLogic> bulletList = new List<BulletLogic>();
    public void OnCreate()
    {

    }
    public void CreateBullet(string bulletPfb, LogicObject attacker, LogicObject bulletTarget, VInt flightTime, Action onHitComplete)
    {
        BulletLogic bulletLoigc = new BulletLogic(attacker, bulletTarget, flightTime, onHitComplete);
#if RENDER_LOGIC
        BulletRender bulletRender = null;//ResourcesManager.Instance.LoadObject<BulletRender>("Prefabs/Bullet/" + bulletPfb);
        bulletLoigc.SetRenderObject(bulletRender);
        bulletRender.SetLogicObject(bulletLoigc,null);
        bulletRender.SetBulletTarget((HeroRender)bulletTarget.RenderObj);
#endif
        bulletLoigc.OnCreate();
        bulletList.Add(bulletLoigc);
    }
    public void OnLogicFrameUpdate()
    {
        for (int i = bulletList.Count-1; i>=0; i--)
        {
            bulletList[i].OnLogicFrameUpdate();
        }
    }
    public void RemoveBullet(BulletLogic bulletLogic)
    {
        for (int i = bulletList.Count - 1; i >= 0; i--)
        {
            if (bulletList[i]==bulletLogic)
            {
                bulletList.Remove(bulletLogic);
                bulletLogic.OnDestroy();
            }
        }
    }
    public void OnDestroy()
    {

    }


}
