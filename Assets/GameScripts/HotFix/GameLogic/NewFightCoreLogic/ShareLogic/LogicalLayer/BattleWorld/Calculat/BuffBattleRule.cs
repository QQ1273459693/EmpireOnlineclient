public class BuffBattleRule
{
    /// <summary>
    /// 通过buff类型获取buff状态,比如 输入buff定身 获得是减益Buff
    /// </summary>
    /// <returns></returns>
    public static NewBuffState GetBuffStateByBuffType(NewBuffType buffType)
    {
        NewBuffState buffState = NewBuffState.None;
        switch (buffType)
        {
            case NewBuffType.None:
                buffState = NewBuffState.None;
                break;
            case NewBuffType.INVINCIBLE:
                buffState = NewBuffState.Buff;
                break;
            case NewBuffType.PHY_ATK_NOT:
                buffState = NewBuffState.Buff;
                break;
            case NewBuffType.MAG_ATK_NOT:
                buffState = NewBuffState.Buff;
                break;
            case NewBuffType.IMTY_LMBE_CHAOS_SKLL:
                buffState = NewBuffState.Buff;
                break;
            case NewBuffType.DEBUFF_FIRE:
                buffState = NewBuffState.DeBuff;
                break;
            case NewBuffType.SKILL_SILENT:
                buffState = NewBuffState.DeBuff;
                break;
            case NewBuffType.IMMOBILIZE:
                buffState = NewBuffState.DeBuff;
                break;
            case NewBuffType.CHAOS:
                buffState = NewBuffState.DeBuff;
                break;
            case NewBuffType.PONED:
                buffState = NewBuffState.DeBuff;
                break;
        }
        return buffState;
    }
    /// <summary>
    /// 通过buff类型获取buff控制状态类型,比如 输入buff定身 获得是有控制
    /// </summary>
    /// <returns></returns>
    public static BuffControlState GetBuffControlStateByBuffType(NewBuffType buffType)
    {
        BuffControlState buffState = BuffControlState.None;
        switch (buffType)
        {
            case NewBuffType.None:
                buffState = BuffControlState.None;
                break;
            case NewBuffType.INVINCIBLE:
                buffState = BuffControlState.None;
                break;
            case NewBuffType.PHY_ATK_NOT:
                buffState = BuffControlState.None;
                break;
            case NewBuffType.MAG_ATK_NOT:
                buffState = BuffControlState.None;
                break;
            case NewBuffType.IMTY_LMBE_CHAOS_SKLL:
                buffState = BuffControlState.None;
                break;
            case NewBuffType.DEBUFF_FIRE:
                buffState = BuffControlState.None;
                break;
            case NewBuffType.SKILL_SILENT:
                buffState = BuffControlState.Control;
                break;
            case NewBuffType.IMMOBILIZE:
                buffState = BuffControlState.Control;
                break;
            case NewBuffType.CHAOS:
                buffState = BuffControlState.Control;
                break;
            case NewBuffType.PONED:
                buffState = BuffControlState.None;
                break;
        }
        return buffState;
    }
}
