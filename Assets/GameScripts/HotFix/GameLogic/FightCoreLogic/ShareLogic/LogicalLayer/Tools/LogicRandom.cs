using System.Collections;
using System.Collections.Generic;
 
using System;
using GameBase;

public class LogicRandom : Singleton<LogicRandom>
{

    public int seedid;//种子
    Random random;
    public void InitRandom(int seed)
    {
        Debuger.Log("初始化随机种子：" + seed);
        //#if SERVER     
        random = new Random(seed);
//#else
//        Random.InitState(seed);
//#endif
    }
    public int Range(int min, int max)
    {
//#if SERVER
        return random.Next(min, max);

//#else
        //return Random.Range(min, max);
//#endif

    }
}
