//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace GameConfig.BUFF
{ 
    [System.Flags]
    public enum EFFECT
    {
        /// <summary>
        /// 无敌Buff,物理和魔法诅咒攻击无效
        /// </summary>
        BUFF_INVINCIBLE = 1,
        /// <summary>
        /// 物理攻击无效
        /// </summary>
        BUFF_PHY_ATK_NOT = 2,
        /// <summary>
        /// 魔法攻击无效
        /// </summary>
        BUFF_MAG_ATK_NOT = 3,
        /// <summary>
        /// 免疫定身,混乱,封魔状态
        /// </summary>
        BUFF_IMTY_LMBE_CHAOS_SKLL = 4,
        /// <summary>
        /// 火烧
        /// </summary>
        DEBUFF_FIRE = 5,
        /// <summary>
        /// 封魔
        /// </summary>
        DEBUFF_SKILL_NOT = 6,
        /// <summary>
        /// 定身
        /// </summary>
        DEBUFF_LMBE = 7,
        /// <summary>
        /// 混乱
        /// </summary>
        DEBUFF_CHAOS = 8,
        /// <summary>
        /// 中毒
        /// </summary>
        DEBUFF_PONED = 9,
    }

} 
