using GameBase;
using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    public class BagWndEvent
    {
        public class UpdateBagSlotEvent : IMemory
        {
            public static readonly int EventId = StringId.StringToHash("UpdateBagSlotEvent");

            public void Clear()
            {
               
            }
        }
    }
}