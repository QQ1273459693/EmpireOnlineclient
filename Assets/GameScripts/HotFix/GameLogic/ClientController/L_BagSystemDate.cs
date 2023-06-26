using GameBase;
using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    public class L_BagSystemDate : Singleton<L_BagSystemDate>
    {
        const string BagSavePath = "BagSystemDataList";
        public long m_Curlong;//当前角色ID
        /// <summary>
        /// 根据角色ID获取他对应的背包数据
        /// </summary>
        Dictionary<long,List<S_Item>> BagItemList = new Dictionary<long,List<S_Item>>();

        /// <summary>
        /// 初始化背包数据
        /// </summary>
        public void LoadBagListData()
        {
            BagItemList= (Dictionary<long, List<S_Item>>)LocalSaveManager.Load(BagSavePath);
            foreach (var item in BagItemList)
            {
                Debug.Log("获取到的大小是:" + item.Value.Count);
            }
            
        }

        /// <summary>
        /// 获取当前ID的存储的背包道具值
        /// </summary>
        public List<S_Item> GetCurCharacterBagList()
        {
            return BagItemList[m_Curlong];
        }
        /// <summary>
        /// 存入数据到字典中
        /// </summary>
        public void SaveCilentData(List<S_Item> ListData)
        {
            if (BagItemList.ContainsKey(m_Curlong))
            {
                BagItemList[m_Curlong] = ListData;
            }
            else
            {
                BagItemList.Add(m_Curlong, ListData);
            }
        }
        /// <summary>
        /// 保存数据到本地
        /// </summary>
        public void SaveLocalBagItemList()
        {
            LocalSaveManager.Save(BagSavePath, BagItemList);
        }
    }
}
