using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 攻击力百分比伤害buff
/// </summary>
public class Buff_AtkPercentage : BuffLogic
{

    public Buff_AtkPercentage(int buffid, LogicObject owner, LogicObject attacker) : base(buffid, owner, attacker)
    {

    }
    public override void OnLogicFrameUpdate()
    {
        base.OnLogicFrameUpdate();
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}

