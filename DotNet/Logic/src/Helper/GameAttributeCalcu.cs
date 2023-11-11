using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;
using GameConfig.item;
using GameConfig.item1;
using System;
using static TEngine.Logic.LoginHDController;

namespace TEngine.Helper;

public static class GameAttributeCalculate
{
    static UnitAttr unit;
    static int[] m_UnitAttrValue;

    /// <summary>
    /// 初始化属性
    /// </summary>
    static void InitUnit()
    {
        if (unit == null)
        {
            var InitRoleBase = ConfigLoader.Instance.Tables.TbInitialRoleAttrieBase.DataList[0];
            //初始创建
            m_UnitAttrValue = new int[26] { InitRoleBase.Hp, InitRoleBase.Mp, InitRoleBase.MaxHp, InitRoleBase.MaxMp, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, InitRoleBase.EleMagicHit, InitRoleBase.CurseMagicHit, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            unit = new UnitAttr
            {
                Hp = m_UnitAttrValue[0],
                Mp = m_UnitAttrValue[1],
                MaxHp = m_UnitAttrValue[2],
                MaxMp = m_UnitAttrValue[3],
                MeleeAk = m_UnitAttrValue[4],
                RangeAk = m_UnitAttrValue[5],
                MagicAk = m_UnitAttrValue[6],
                MeDEF = m_UnitAttrValue[7],
                RGDEF = m_UnitAttrValue[8],
                MGDEF = m_UnitAttrValue[9],
                ELMRES = m_UnitAttrValue[10],
                CurseMgRES = m_UnitAttrValue[11],
                Shield = m_UnitAttrValue[12],
                PhysicalHit = m_UnitAttrValue[13],
                EleMagicHit = m_UnitAttrValue[14],
                CurseMagicHit = m_UnitAttrValue[15],
                MagicPenetration = m_UnitAttrValue[16],
                Evade = m_UnitAttrValue[17],
                Speed = m_UnitAttrValue[18],
                CriticalHit = m_UnitAttrValue[19],
                MixDamage = m_UnitAttrValue[20],
                MaxDamage = m_UnitAttrValue[21],
                Tough = m_UnitAttrValue[22],
                ArmorBreakingAT = m_UnitAttrValue[23],
                SwordDamageAdd = m_UnitAttrValue[24],
                KnifeBreakingAT = m_UnitAttrValue[25],
            };
        }
    }
    /// <summary>
    /// 计算装备全部属性
    /// </summary>
    public static UnitAttr CalculateEquip(UnitAttr BackUnitAttr, List<CharEquipSlotData> charEquip)
    {

        Dictionary<int, int[]> m_Unitru = new Dictionary<int, int[]>();
        int m_WeaponType = -1;//当前穿戴的武器类型
        if (charEquip!=null)
        {
            for (int i = 0; i < charEquip.Count; i++) 
            {
                if (charEquip[i].slot!=null)
                {
                    //表明有装备,主属性计算
                    int EquipID = charEquip[i].slot.itemData.item.itemId;
                    var EquipBse = ConfigLoader.Instance.Tables.TbEquipmentBase.DataMap[EquipID];
                    if (EquipBse.SlotPos==0)
                    {
                        //是武器部位
                        m_WeaponType = (int)EquipBse.EquipType;
                    }
                    var EquipAttriBute = EquipBse.Attribute.MainAttribute;
                    for (int j = 0; j < EquipAttriBute.Count; j++)
                    {
                        var Bute = EquipAttriBute[j];
                        int[] ValueOrPercen;
                        if (m_Unitru.TryGetValue(Bute.AttriID,out ValueOrPercen))
                        {
                            if (Bute.Percent==0)
                            {
                                //不是百分比
                                ValueOrPercen[0] += Bute.Value;
                            }
                            else
                            {
                                //是百分比
                                ValueOrPercen[1] += Bute.Value;
                            }
                        }
                        else
                        {
                            int[] ints = new int[2];
                            if (Bute.Percent == 0)
                            {
                                //不是百分比
                                ints[0]= Bute.Value;
                            }
                            else
                            {
                                //是百分比
                                ints[1]= Bute.Value;
                            }
                            m_Unitru.Add(Bute.AttriID, ints);
                        }
                    }

                    //基础属性计算
                    var BaseAttriBute = ConfigLoader.Instance.Tables.TbEquipmentBase.DataMap[EquipID].BaseAttriBute.BaseAttribute;
                    for (int k = 0; k < BaseAttriBute.Count; k++)
                    {
                        var Bute = BaseAttriBute[k];
                        int[] Value;
                        if (m_Unitru.TryGetValue(Bute.AttriID, out Value))
                        {
                            if (Bute.AttriID==20)//这里写死是武器伤害
                            {
                                Value[0] += Bute.MixValue;
                                Value[1] += Bute.MaxValue;
                            }
                            else
                            {
                                Value[0]+= Bute.MixValue;
                            }
                        }
                        else
                        {
                            int[] ints = new int[2];
                            if (Bute.AttriID == 20)//这里写死是武器伤害
                            {
                                ints[0]= Bute.MixValue;
                                ints[1]= Bute.MaxValue;
                            }
                            else
                            {
                                ints[0] = Bute.MixValue;
                                ints[1] = 0;
                            }
                            Log.Info($"加载的基本属性ID:{Bute.AttriID}");
                            m_Unitru.Add(Bute.AttriID, ints);
                        }
                    }
                }
               
            }
        }
        if (BackUnitAttr==null)
        {
            Log.Error("错误的属性赋值!!!");
            return null;
        }
        foreach (var item in m_Unitru)
        {
            int BaseValue = item.Value[0];
            float PercentValue = item.Value[1]/100;
            int PercentBaseValue = (int)(BaseValue + Math.Round(BaseValue * PercentValue));
            switch (item.Key)
            {
                case 0:
                    BackUnitAttr.Hp += PercentBaseValue;
                    break;
                case 1:
                    BackUnitAttr.Mp += PercentBaseValue;
                    break;
                case 2:
                    BackUnitAttr.MaxHp += PercentBaseValue;
                    break;
                case 3:
                    BackUnitAttr.MaxMp += PercentBaseValue;
                    break;
                case 4:
                    BackUnitAttr.MeleeAk += PercentBaseValue;
                    break;
                case 5:
                    BackUnitAttr.RangeAk += PercentBaseValue;
                    break;
                case 6:
                    BackUnitAttr.MagicAk += PercentBaseValue;
                    break;
                case 7:
                    BackUnitAttr.MeDEF += PercentBaseValue;
                    break;
                case 8:
                    BackUnitAttr.RGDEF += PercentBaseValue;
                    break;
                case 9:
                    BackUnitAttr.MGDEF += PercentBaseValue;
                    break;
                case 10:
                    BackUnitAttr.ELMRES += PercentBaseValue;
                    break;
                case 11:
                    BackUnitAttr.CurseMgRES += PercentBaseValue;
                    break;
                case 12:
                    BackUnitAttr.Shield += PercentBaseValue;
                    break;
                case 13:
                    BackUnitAttr.PhysicalHit += PercentBaseValue;
                    break;
                case 14:
                    BackUnitAttr.EleMagicHit += PercentBaseValue;
                    break;
                case 15:
                    BackUnitAttr.CurseMagicHit += PercentBaseValue;
                    break;
                case 16:
                    BackUnitAttr.MagicPenetration += PercentBaseValue;
                    break;
                case 17:
                    BackUnitAttr.Evade += PercentBaseValue;
                    break;
                case 18:
                    BackUnitAttr.Speed += PercentBaseValue;
                    break;
                case 19:
                    BackUnitAttr.CriticalHit += PercentBaseValue;
                    break;
                case 20:
                    BackUnitAttr.MixDamage += item.Value[0];//额外写,这里单独加上 武器伤害属性
                    BackUnitAttr.MaxDamage += item.Value[1];//额外写法,这里单独加上 武器伤害属性
                    break;
                case 21:
                    //BackUnitAttr.MaxDamage = item.Value[1];//额外写法
                    break;
                case 22:
                    BackUnitAttr.Tough += PercentBaseValue;
                    break;
                case 23:
                    BackUnitAttr.ArmorBreakingAT = PercentBaseValue;
                    break;
                case 101:
                    BackUnitAttr.SwordDamageAdd += PercentBaseValue;
                    break;
                case 102:
                    BackUnitAttr.KnifeBreakingAT += PercentBaseValue;
                    break;
            }
            Log.Info($"属性ID:{item.Key},基础加成是:{BaseValue},百分比加成:{PercentValue},结算值是:{PercentBaseValue}");
        }
        //这里单独拉出来再重新加上武器伤害百分比
        if (m_WeaponType>0)
        {
            //证明部位有武器
            switch (m_WeaponType)
            {
                case 1://大剑
                    int MixValue = BackUnitAttr.MixDamage;
                    int MAXValue = BackUnitAttr.MaxDamage;
                    Log.Info($"原先武器的最低伤害:{MixValue},最高伤害:{MAXValue},剑类武器加成值:{BackUnitAttr.SwordDamageAdd}");
                    float PercentValue = (float)BackUnitAttr.SwordDamageAdd / 100;
                    int PercentBaseValue = (int)(MixValue + Math.Round(MixValue * PercentValue));
                    int PercentBaseValue1 = (int)(MAXValue + Math.Round(MAXValue * PercentValue));

                    Log.Info($"计算后的武器的最低伤害:{PercentBaseValue},最高伤害:{PercentBaseValue1}");
                    BackUnitAttr.MixDamage = PercentBaseValue;
                    BackUnitAttr.MaxDamage = PercentBaseValue1;
                    Log.Info("到这里了吗");
                    break;
            }
        }
        

        return BackUnitAttr;
    }
    /// <summary>
    /// 计算被动技能加成
    /// </summary>
    /// <returns></returns>
    public static UnitAttr CalculatePassSkill(UnitAttr BackUnitAttr, List<SkillData> SkillList)
    {
        InitUnit();
        if (BackUnitAttr==null)
        {
            BackUnitAttr = new UnitAttr();
            BackUnitAttr = InitUnitAttrValue(BackUnitAttr);
        }



        Dictionary<int, int[]> m_Unitru = new Dictionary<int, int[]>();
        for (int i = 0; i < SkillList.Count; i++)
        {
            var SkillData = SkillList[i];
            if (SkillData.SkillType==0)
            {
                //是被动技能
                var SkillBase = ConfigLoader.Instance.Tables.TbSwordSkillBase.Get(SkillData.SkID).Attribute[SkillData.Lv].SkillAttrs;
                for (int j = 0; j < SkillBase.Length; j++)
                {
                    var Parma = SkillBase[j];
                    Log.Info($"技能属性ID:{Parma.AttriID},技能值:{Parma.Value}");
                    int[] Value;
                    if (m_Unitru.TryGetValue(Parma.AttriID, out Value))
                    {
                        if (Parma.Percent == 1)
                        {
                            //是百分比
                            Value[1] += Parma.Value;
                        }
                        else
                        {
                            //非百分比
                            Value[0] += Parma.Value;
                        }
                    }
                    else
                    {
                        int[] ints = new int[2];
                        if (Parma.Percent==1)
                        {
                            //是百分比
                            ints[0] = 0;
                            ints[1] = Parma.Value;
                        }
                        else
                        {
                            //非百分比
                            ints[0] = Parma.Value;
                            ints[1] = 0;
                        }
                        m_Unitru.Add(Parma.AttriID, ints);
                    }
                }
            }
        }
        foreach (var item in m_Unitru)
        {
            int BaseValue = item.Value[0];
            float PercentValue = item.Value[1] / 100;
            int PercentBaseValue = (int)(BaseValue + Math.Round(BaseValue * PercentValue));
            switch (item.Key)
            {
                case 0:
                    //BackUnitAttr.Hp = PercentBaseValue;
                    break;
                case 1:
                    //BackUnitAttr.Mp = PercentBaseValue;
                    break;
                case 2:
                    BackUnitAttr.MaxHp+= PercentBaseValue;
                    break;
                case 3:
                    BackUnitAttr.MaxMp += PercentBaseValue;
                    break;
                case 4:
                    BackUnitAttr.MeleeAk += PercentBaseValue;
                    break;
                case 5:
                    BackUnitAttr.RangeAk += PercentBaseValue;
                    break;
                case 6:
                    BackUnitAttr.MagicAk += PercentBaseValue;
                    break;
                case 7:
                    BackUnitAttr.MeDEF += PercentBaseValue;
                    break;
                case 8:
                    BackUnitAttr.RGDEF += PercentBaseValue;
                    break;
                case 9:
                    BackUnitAttr.MGDEF += PercentBaseValue;
                    break;
                case 10:
                    BackUnitAttr.ELMRES += PercentBaseValue;
                    break;
                case 11:
                    BackUnitAttr.CurseMgRES += PercentBaseValue;
                    break;
                case 12:
                    BackUnitAttr.Shield += PercentBaseValue;
                    break;
                case 13:
                    BackUnitAttr.PhysicalHit += PercentBaseValue;
                    break;
                case 14:
                    BackUnitAttr.EleMagicHit += PercentBaseValue;
                    break;
                case 15:
                    BackUnitAttr.CurseMagicHit += PercentBaseValue;
                    break;
                case 16:
                    BackUnitAttr.MagicPenetration += PercentBaseValue;
                    break;
                case 17:
                    BackUnitAttr.Evade += PercentBaseValue;
                    break;
                case 18:
                    BackUnitAttr.Speed += PercentBaseValue;
                    break;
                case 19:
                    BackUnitAttr.CriticalHit += PercentBaseValue;
                    break;
                case 20:
                    break;
                case 21:
                    break;
                case 22:
                    BackUnitAttr.Tough += PercentBaseValue;
                    break;
                case 23:
                    BackUnitAttr.ArmorBreakingAT += PercentBaseValue;
                    break;
                case 101://剑类武器伤害,应该都是百分比
                    BackUnitAttr.SwordDamageAdd+= item.Value[1];//额外写
                    Log.Info("刀类添加了,现在值是:"+ BackUnitAttr.SwordDamageAdd);
                    break;
                case 102://刀类武器伤害,应该都是百分比
                    BackUnitAttr.KnifeBreakingAT += item.Value[1];//额外写
                    break;
            }
        }

        return BackUnitAttr;
    }
    /// <summary>
    /// 初始化属性值
    /// </summary>
    static UnitAttr InitUnitAttrValue(UnitAttr Attr)
    {
        Attr.Hp = unit.Hp;
        Attr.Mp = unit.Mp;
        Attr.MaxHp = unit.MaxHp;
        Attr.MaxMp = unit.MaxMp;
        Attr.MeleeAk = unit.MeleeAk;
        Attr.RangeAk = unit.RangeAk;
        Attr.MagicAk = unit.MagicAk;
        Attr.MeDEF = unit.MeDEF;
        Attr.RGDEF = unit.RGDEF;
        Attr.MGDEF = unit.MGDEF;
        Attr.ELMRES = unit.ELMRES;
        Attr.CurseMgRES = unit.CurseMgRES;
        Attr.Shield = unit.Shield;
        Attr.PhysicalHit = unit.PhysicalHit;
        Attr.EleMagicHit = unit.EleMagicHit;
        Attr.CurseMagicHit = unit.CurseMagicHit;
        Attr.MagicPenetration = unit.MagicPenetration;
        Attr.Evade = unit.Evade;
        Attr.Speed = unit.Speed;
        Attr.CriticalHit = unit.CriticalHit;
        Attr.MixDamage = unit.MixDamage;
        Attr.MaxDamage = unit.MaxDamage;
        Attr.Tough = unit.Tough;
        Attr.ArmorBreakingAT = unit.ArmorBreakingAT;
        return Attr;
    }
}