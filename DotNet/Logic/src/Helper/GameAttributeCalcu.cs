using static TEngine.Logic.LoginHDController;

namespace TEngine.Helper;

public static class GameAttributeCalculate
{
    static UnitAttr unit;
    static int[] m_UnitAttrValue;
    /// <summary>
    /// 计算装备全部属性
    /// </summary>
    public static UnitAttr CalculateEquip(List<CharEquipSlotData> charEquip)
    {
        if (unit==null)
        {
            var InitRoleBase = ConfigLoader.Instance.Tables.TbInitialRoleAttrieBase.DataList[0];
            //初始创建
            m_UnitAttrValue = new int[24] { InitRoleBase.Hp, InitRoleBase.Mp, InitRoleBase.MaxHp, InitRoleBase.MaxMp, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, InitRoleBase.EleMagicHit, InitRoleBase.CurseMagicHit, 0, 0, 0, 0, 0, 0, 0, 0 };

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
            };
        }






        Dictionary<int, int[]> m_Unitru = new Dictionary<int, int[]>(); 
        if (charEquip!=null)
        {
            for (int i = 0; i < charEquip.Count; i++) 
            {
                if (charEquip[i].slot!=null)
                {
                    //表明有装备
                    int EquipID = charEquip[i].slot.itemData.item.itemId;
                    var EquipAttriBute = ConfigLoader.Instance.Tables.TbEquipmentBase.DataMap[EquipID].Attribute;
                    for (int j = 0; j < EquipAttriBute.Count; j++)
                    {
                        var Bute = EquipAttriBute[j];
                        int[] ValueOrPercen;
                        if (m_Unitru.TryGetValue(Bute.AttriId,out ValueOrPercen))
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
                            m_Unitru.Add(Bute.AttriId, ints);
                        }
                    }
                }
               
            }
        }
        UnitAttr BackUnitAttr = new UnitAttr();
        BackUnitAttr = InitUnitAttrValue(BackUnitAttr);
        foreach (var item in m_Unitru)
        {
            int BaseValue = item.Value[0];
            float PercentValue = item.Value[1]/100;
            int PercentBaseValue = (int)(BaseValue + Math.Round(BaseValue * PercentValue));
            switch (item.Key)
            {
                case 0:
                    BackUnitAttr.Hp = PercentBaseValue;
                    break;
                case 1:
                    BackUnitAttr.Mp = PercentBaseValue;
                    break;
                case 2:
                    BackUnitAttr.MaxHp = PercentBaseValue;
                    break;
                case 3:
                    BackUnitAttr.MaxMp = PercentBaseValue;
                    break;
                case 4:
                    BackUnitAttr.MeleeAk = PercentBaseValue;
                    break;
                case 5:
                    BackUnitAttr.RangeAk = PercentBaseValue;
                    break;
                case 6:
                    BackUnitAttr.MagicAk = PercentBaseValue;
                    break;
                case 7:
                    BackUnitAttr.MeDEF = PercentBaseValue;
                    break;
                case 8:
                    BackUnitAttr.RGDEF = PercentBaseValue;
                    break;
                case 9:
                    BackUnitAttr.MGDEF = PercentBaseValue;
                    break;
                case 10:
                    BackUnitAttr.ELMRES = PercentBaseValue;
                    break;
                case 11:
                    BackUnitAttr.CurseMgRES = PercentBaseValue;
                    break;
                case 12:
                    BackUnitAttr.Shield = PercentBaseValue;
                    break;
                case 13:
                    BackUnitAttr.PhysicalHit = PercentBaseValue;
                    break;
                case 14:
                    BackUnitAttr.EleMagicHit = PercentBaseValue;
                    break;
                case 15:
                    BackUnitAttr.CurseMagicHit = PercentBaseValue;
                    break;
                case 16:
                    BackUnitAttr.MagicPenetration = PercentBaseValue;
                    break;
                case 17:
                    BackUnitAttr.Evade = PercentBaseValue;
                    break;
                case 18:
                    BackUnitAttr.Speed = PercentBaseValue;
                    break;
                case 19:
                    BackUnitAttr.CriticalHit = PercentBaseValue;
                    break;
                case 20:
                    BackUnitAttr.MixDamage = PercentBaseValue;
                    break;
                case 21:
                    BackUnitAttr.MaxDamage = PercentBaseValue;
                    break;
                case 22:
                    BackUnitAttr.Tough = PercentBaseValue;
                    break;
                case 23:
                    BackUnitAttr.ArmorBreakingAT = PercentBaseValue;
                    break;
            }
            Log.Info($"属性ID:{item.Key},基础加成是:{BaseValue},百分比加成:{PercentValue},结算值是:{PercentBaseValue}");
        }
        Log.Info($"看下护甲值{BackUnitAttr.Shield}");
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