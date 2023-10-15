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
    }
}