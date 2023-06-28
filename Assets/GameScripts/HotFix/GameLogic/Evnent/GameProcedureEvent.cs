using GameBase;
using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    public class GameProcedureEvent
    {
        public class LoadMainStateEvent : IMemory
        {
            public static readonly int EventId = StringId.StringToHash("LoadMainStateEvent");

            public void Clear()
            {
               
            }
        }
        public class LoadMainCityUIEvent : IMemory
        {
            public static readonly int EventId = StringId.StringToHash("LoadMainCityUIEvent");

            public void Clear()
            {

            }
        }
    }
}