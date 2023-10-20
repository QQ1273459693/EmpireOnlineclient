namespace TEngine.Helper;

public static class GameAttributeCalcu
{
    /// <summary>
    /// 计算装备全部属性
    /// </summary>
    public static void CalculateEquip(List<CharEquipSlotData> charEquip)
    {
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
                            m_Unitru.Add(Bute.AttriId, ints);
                        }
                    }
                }
               
            }
        }
    }
}