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
            public int itemID;//��ƷID
            public int count;//����
            public EquipmentData equipmentData;//װ������
            public bool isEquipment;//�Ƿ���װ��
            public class EquipmentData
            {
                public int EquipIndex;//װ����λ
                public int EquipID;//װ��ID
                public int Lv;//װ���ȼ�
                public Dictionary<int, int> m_Attribute = new Dictionary<int, int>();
            }
        }
    }
    
}
