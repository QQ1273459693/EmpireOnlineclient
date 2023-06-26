using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public class S_Character
    {
        public string Name;//名字
        public long Id;//角色ID

        public Attribute attribute=new Attribute();
        public List<S_Item.Item.EquipmentData> equipmentDatas = new List<S_Item.Item.EquipmentData>(); 
        /// <summary>
        /// 角色属性值
        /// </summary>
        public class Attribute
        {
            public int HP;//生命
            public int MP;//魔法值
            public int Attack;//攻击力
        }
    }
}
