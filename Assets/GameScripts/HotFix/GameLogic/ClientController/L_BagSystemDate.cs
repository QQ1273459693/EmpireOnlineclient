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
        public long m_Curlong;//��ǰ��ɫID
        /// <summary>
        /// ���ݽ�ɫID��ȡ����Ӧ�ı�������
        /// </summary>
        Dictionary<long,List<S_Item>> BagItemList = new Dictionary<long,List<S_Item>>();

        /// <summary>
        /// ��ʼ����������
        /// </summary>
        public void LoadBagListData()
        {
            BagItemList= (Dictionary<long, List<S_Item>>)LocalSaveManager.Load(BagSavePath);
            foreach (var item in BagItemList)
            {
                Debug.Log("��ȡ���Ĵ�С��:" + item.Value.Count);
            }
            
        }

        /// <summary>
        /// ��ȡ��ǰID�Ĵ洢�ı�������ֵ
        /// </summary>
        public List<S_Item> GetCurCharacterBagList()
        {
            return BagItemList[m_Curlong];
        }
        /// <summary>
        /// �������ݵ��ֵ���
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
        /// �������ݵ�����
        /// </summary>
        public void SaveLocalBagItemList()
        {
            LocalSaveManager.Save(BagSavePath, BagItemList);
        }
    }
}
