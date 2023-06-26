using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public class S_Item
    {
        public Item item = new Item();
        public class Item
        {
            public int itemID;//物品ID
            public int count;//数量
            public EquipmentData equipmentData;//装备数据
            public bool isEquipment;//是否是装备
            public class EquipmentData
            {
                public int EquipIndex;//装备部位
                public int EquipID;//装备ID
                public int Lv;//装备等级
                public Dictionary<int, int> m_Attribute = new Dictionary<int, int>();
            }
        }
    }
    
}
