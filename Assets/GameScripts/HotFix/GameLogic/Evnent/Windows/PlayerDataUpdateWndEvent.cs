using GameBase;
using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    public class PlayerDataUpdateWndEvent
    {
        public class UpdateEquipSlot : IMemory
        {
            public static readonly int EventId = StringId.StringToHash("UpdateEquipSlot");

            public void Clear()
            {
               
            }
        }
        /// <summary>
        /// ���½�ɫ����
        /// </summary>
        public class UpdatePlayerUnitAttr : IMemory
        {
            public static readonly int EventId = StringId.StringToHash("UpdatePlayerUnitAttr");

            public void Clear()
            {

            }
        }
    }
}