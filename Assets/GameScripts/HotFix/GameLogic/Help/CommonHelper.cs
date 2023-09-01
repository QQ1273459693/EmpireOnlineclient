using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace GameLogic
{
    public static class CommonHelper
    {
        public enum m_CurrencyType
        {
            Gold=1,//金币
            Diamond=2//钻石
        }
        /// <summary>
        /// 通过枚举获取货币类型
        /// </summary>
        /// <returns></returns>
        public static int GetItemDataNum(m_CurrencyType m_Currency)
        {
            int Num = 0;
            switch (m_Currency)
            {
                case m_CurrencyType.Gold:
                    Num= GameDataController.Instance.m_CharacterData.Gold;
                    break;
                case m_CurrencyType.Diamond:
                    Num = GameDataController.Instance.m_CharacterData.Diamond;
                    break;
            }
            return Num;
        }

    }
}
