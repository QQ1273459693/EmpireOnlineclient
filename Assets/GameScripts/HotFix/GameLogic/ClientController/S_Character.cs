using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public class S_Character
    {
        public string Name;//����
        public long Id;//��ɫID

        public Attribute attribute=new Attribute();
        public List<S_Item.Item.EquipmentData> equipmentDatas = new List<S_Item.Item.EquipmentData>(); 
        /// <summary>
        /// ��ɫ����ֵ
        /// </summary>
        public class Attribute
        {
            public int HP;//����
            public int MP;//ħ��ֵ
            public int Attack;//������
        }
    }
}
