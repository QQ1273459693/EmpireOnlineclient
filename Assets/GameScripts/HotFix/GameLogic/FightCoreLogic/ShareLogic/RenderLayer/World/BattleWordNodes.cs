using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleWordNodes : SingletonMono<BattleWordNodes>
{
      
    public Transform[] heroTransArr;
    public Transform[] enemyTransArr;
    public Transform HUDWindow;
    public Camera Camera3D;
    public Camera UiCamera;
    public RectTransform CanvasRect;
    public StartWindow startWidnow;
    public RoundWindow roundWindow;
    public SelectHeroWindow selectHeroWidnow;
    public BattleResultWindow battleResultWindow;
    public SkillWindow skillWindow;

    public Transform enemysConter;
    public Transform herosConter;
    public Transform conterTrans;
}
