using GameBase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager: SingletionMono<BulletManager>,ILogicBehaviour
{
    public List<BulletLogic> bulletList = new List<BulletLogic>();
    public void OnCreate()
    {

    }
    /// <summary>
    /// 创建子弹
    /// </summary>
    /// <param name="bulletPfb"></param>
    /// <param name="attacker"></param>
    /// <param name="bulletTarget"></param>
    /// <param name="flightTime"></param>
    /// <param name="onHitComplet"></param>
    public void CreateBullet(string bulletPfb,LogicObject attacker,LogicObject bulletTarget,VInt flightTime,Action onHitComplet)
    {
        BulletLogic bulletlogic = new BulletLogic(attacker,bulletTarget,flightTime,onHitComplet);
#if RENDER_LOGIC
        //加载子弹预制体设置逻辑对象和渲染对象
        BulletRender bulletRender= ResourceManager.Instance.LoadObject<BulletRender>(AssetPathConfig.SKILLEFFECT + bulletPfb);
        bulletRender.SetLogicObject(bulletlogic,null);
        bulletlogic.SetRenderObject(bulletRender);
#endif
        bulletlogic.OnCreate();
        bulletList.Add(bulletlogic);

    }
    public void OnLogicFrameUpdate()
    {
        for (int i = bulletList.Count-1; i >=0 ; i--)
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
                bulletList.Remove(bulletList[i]);
                bulletLogic.OnDestroy();
            }
        }
    }
    public void OnDestroy()
    {
        bulletList.Clear();
    }
}
