using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleWorldNodes:SingletionMono<BattleWorldNodes>
{

    //敌我双方位置父节点
    public Transform[] heroTransArr;
    public Transform[] enemyTransArr;
    public Transform HUDWindowTrans;
    public Camera Camera3D;
    public Camera UICamera;

    public RoundWindow roundWindows;
    public BattleResultWindow battleResultWindow;

    public Transform conTerTrans;
    public Transform enemyConterTrans;
    public Transform slefHeroConterTrans;
}
