using GameConfig.item;
using GameLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public class TestSaveEasy : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            //S_Item s_Item = new S_Item();
            //s_Item.item.itemID = 100;
            //s_Item.item.count = 55;
            //List<S_Item> BagItemList = new List<S_Item>();
            //for (int i = 0; i < 255; i++)
            //{
            //    S_Item s_Item1 = new S_Item();
            //    s_Item1.item.itemID = i + 1;
            //    s_Item1.item.count = i + 2;
            //    BagItemList.Add(s_Item1);
            //}
            //ES3.Save("背包道具Key", BagItemList);

            L_BagSystemDate.Instance.m_Curlong = 1;
            List<S_Character> BagItemList = new List<S_Character>();
            for (int i = 0; i < 1; i++)
            {
                S_Character s_Item1 = new S_Character();
                s_Item1.Name = "角色"+i+1;
                s_Item1.Id = 1;
                s_Item1.attribute.Attack = 10;
                s_Item1.attribute.HP = 100;
                s_Item1.attribute.MP = 100;
                List<S_Item> BagList = new List<S_Item>();
                var List = ConfigLoader.Instance.Tables.TbItem.DataList;
                Debug.Log("看下列表大小:"+ List.Count);
                for (int J = 0; J < List.Count; J++)
                {
                    var Item = List[J];
                    S_Item s_Item = new S_Item();
                    s_Item.item.itemID = Item.Id;
                    if (!Item.Overlapping)
                    {
                        //是装备
                        s_Item.item.equipmentData = new S_Item.Item.EquipmentData();
                        var EqAttriDict = ConfigLoader.Instance.Tables.TbEquipment.Get(Item.Id);//
                        Dictionary<int, int> AttriDict = new Dictionary<int, int>();
                        for (int k = 0; k < EqAttriDict.Attribute.Count; k++)
                        {
                            AttriDict.Add(EqAttriDict.Attribute[k].AttriId, EqAttriDict.Attribute[k].Value);
                        }
                        s_Item.item.equipmentData.Lv = 1;
                        s_Item.item.equipmentData.EquipID = EqAttriDict.Id;
                        s_Item.item.equipmentData.EquipIndex = (int)EqAttriDict.EquipType;
                        s_Item.item.equipmentData.m_Attribute = AttriDict;
                    }
                    s_Item.item.isEquipment = !Item.Overlapping;
                    s_Item.item.count = Item.Overlapping ? J + 1 : 1;
                    BagList.Add(s_Item);
                }
                L_BagSystemDate.Instance.SaveCilentData(BagList);
                BagItemList.Add(s_Item1);
            }
            
            LocalSaveManager.Save("CharacterListData", BagItemList);
            L_BagSystemDate.Instance.SaveLocalBagItemList();


            //Log.Debug("道具ID:"+s_Item.item.itemID+",数量是:"+s_Item.item.count);
        }
    }
}
