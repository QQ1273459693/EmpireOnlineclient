using System;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Pool;
using GameLogic;
using DG.Tweening;
using System.Collections.Generic;

namespace TEngine
{
    class Normal_RoleInfoViewData
    {

        Dictionary<int,Text> m_AtriDict=new Dictionary<int,Text>();//属性字段文本,key值是属性值

        public void Init(GameObject obj)
        {
            Transform AttriBuildTS = obj.transform.Find("ViewContent/Content");
            for (int i = 0; i < AttriBuildTS.childCount; i++)
            {
                switch (i)
                {
                    case 0://等级文本
                        break;
                    case 1://技能点
                        break;
                    case 2://出手速度
                        break;
                    case 3://最低武器伤害
                        break;
                    case 4://最高武器伤害
                        break;
                    case 5://护甲
                        break;
                    case 6://近战攻击力
                        break;
                    case 7://远程攻击力
                        break;
                    case 8://魔法攻击力
                        break;
                    case 9://近战防御力
                        break;
                    case 10://远程防御力
                        break;
                    case 11://魔法防御力
                        break;
                    case 12://元素魔法抗性
                        break;
                    case 13://诅咒魔法抗性
                        break;
                    case 14://物理攻击命中
                        break;
                    case 15://元素魔法命中
                        break;
                    case 16://诅咒魔法命中
                        break;
                    case 17://魔法穿透力
                        break;
                    case 18://闪避
                        break;
                    case 19://出手速度
                        break;
                    case 20://暴击
                        break;
                    case 21://强韧
                        break;
                    case 22://破甲能力
                        break;

                }
            }

            
        }
        
        public void AfterShow()
        {
            //GameEvent.AddEventListener(BagWndEvent.UpdateBagSlotEvent.EventId, UpdateBagSlot);
        }
        public  void BeforeClose()
        {
            //GameEvent.RemoveEventListener(BagWndEvent.UpdateBagSlotEvent.EventId, UpdateBagSlot);
        }
    }
}
