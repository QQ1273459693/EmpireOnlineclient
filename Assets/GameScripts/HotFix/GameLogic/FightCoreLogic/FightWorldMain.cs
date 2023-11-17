using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;

[Update]
public class FightWorldMain : BehaviourSingleton<FightWorldMain>
{
    public override void Start()
    {
        WorldManager.Initialize();
        Log.Info("开始了吗");
    }

    public override void Update()
    {
        base.Update();
        WorldManager.Update();
    }
    public override void Destroy()
    {
        base.Destroy();
        WorldManager.DestroyWorld();
    }
}
