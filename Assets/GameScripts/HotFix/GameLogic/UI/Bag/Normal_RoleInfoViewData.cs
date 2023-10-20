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
        Text m_LvText;
        Text m_UIDText;
        Image m_HPFill;
        Image m_MpFill;
        Image m_EXPFill;
        Text m_HpText;
        Text m_MpText;
        Text m_ExpText;


        //数据层
        Dictionary<int,Text> m_AtriDict=new Dictionary<int,Text>();//属性字段文本,key值是属性值
        CharacterData m_CharData;//角色数据
        public void Init(GameObject obj)
        {
            Transform AttriBuildTS = obj.transform.Find("ViewContent/Content");
            for (int i = 0; i < AttriBuildTS.childCount; i++)
            {
                Text m_Text = AttriBuildTS.GetChild(i).GetChild(1).GetComponent<Text>();
                m_AtriDict.Add(i,m_Text);
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
            Transform Right= DUnityUtil.FindChild(obj.transform, "Mid/1/GameObject/Right/");
            m_LvText = DUnityUtil.FindChildComponent<Text>(Right, "Lv");
            m_UIDText = DUnityUtil.FindChildComponent<Text>(Right, "UID");
            m_HPFill= DUnityUtil.FindChildComponent<Image>(Right, "HP/Image");
            m_MpFill = DUnityUtil.FindChildComponent<Image>(Right, "MP/Image");
            m_EXPFill = DUnityUtil.FindChildComponent<Image>(Right, "Exp/Image");
            m_HpText = DUnityUtil.FindChildComponent<Text>(Right, "HP/Image/Num");
            m_MpText = DUnityUtil.FindChildComponent<Text>(Right, "MP/Image/Num");
            m_ExpText = DUnityUtil.FindChildComponent<Text>(Right, "Exp/Image/Num");

        }
        /// <summary>
        /// 刷新角色数据
        /// </summary>
        void RefreshData(UnitAttr unitAttr)
        {
            foreach (var item in m_AtriDict)
            {
                switch (item.Key)
                {
                    case 0://等级文本
                        item.Value.text = m_CharData.Level.ToString();
                        break;
                    case 1://技能点
                        item.Value.text=m_CharData.SkillPoints.ToString();
                        break;
                    case 2://出手速度
                        item.Value.text=unitAttr.Speed.ToString();
                        break;
                    case 3://最低武器伤害
                        item.Value.text = unitAttr.MixDamage.ToString();
                        break;
                    case 4://最高武器伤害
                        item.Value.text = unitAttr.MaxDamage.ToString();
                        break;
                    case 5://护甲
                        item.Value.text = unitAttr.Shield.ToString();
                        break;
                    case 6://近战攻击力
                        item.Value.text=unitAttr.MeleeAk.ToString();
                        break;
                    case 7://远程攻击力
                        item.Value.text = unitAttr.RangeAk.ToString();
                        break;
                    case 8://魔法攻击力
                        item.Value.text = unitAttr.MagicAk.ToString();
                        break;
                    case 9://近战防御力
                        item.Value.text = unitAttr.MeDEF.ToString();
                        break;
                    case 10://远程防御力
                        item.Value.text = unitAttr.RGDEF.ToString();
                        break;
                    case 11://魔法防御力
                        item.Value.text = unitAttr.MGDEF.ToString();
                        break;
                    case 12://元素魔法抗性
                        item.Value.text = unitAttr.ELMRES.ToString();
                        break;
                    case 13://诅咒魔法抗性
                        item.Value.text = unitAttr.CurseMgRES.ToString();
                        break;
                    case 14://物理攻击命中
                        item.Value.text = unitAttr.PhysicalHit.ToString();
                        break;
                    case 15://元素魔法命中
                        item.Value.text = unitAttr.EleMagicHit.ToString();
                        break;
                    case 16://诅咒魔法命中
                        item.Value.text = unitAttr.CurseMagicHit.ToString();
                        break;
                    case 17://魔法穿透力
                        item.Value.text = unitAttr.MagicPenetration.ToString();
                        break;
                    case 18://闪避
                        item.Value.text = unitAttr.Evade.ToString();
                        break;
                    case 19://出手速度
                        item.Value.text = unitAttr.Speed.ToString();
                        break;
                    case 20://暴击
                        item.Value.text = unitAttr.CriticalHit.ToString();
                        break;
                    case 21://强韧
                        item.Value.text = unitAttr.Tough.ToString();
                        break;
                    case 22://破甲能力
                        item.Value.text = unitAttr.ArmorBreakingAT.ToString();
                        break;
                }
            }
        }
        
        public void AfterShow()
        {
            m_CharData = GameDataController.Instance.m_CharacterData;

            m_LvText.text = $"等级:{m_CharData.Level}";
            var Unitr = m_CharData.PlayerAttribute;
            m_HPFill.fillAmount = (float)Unitr.Hp / (float)Unitr.MaxHp;
            m_MpFill.fillAmount=(float)Unitr.Mp/(float)Unitr.MaxMp;
            m_EXPFill.fillAmount = (float)m_CharData.Exp / (float)1000;
            m_HpText.text = $"{Unitr.Hp}/{Unitr.MaxHp}";
            m_MpText.text = $"{Unitr.Mp}/{Unitr.MaxMp}";
            m_ExpText.text= $"{m_CharData.Exp}/{100}";
            RefreshData(Unitr);


            //GameEvent.AddEventListener(BagWndEvent.UpdateBagSlotEvent.EventId, UpdateBagSlot);
        }
        public  void BeforeClose()
        {
            //GameEvent.RemoveEventListener(BagWndEvent.UpdateBagSlotEvent.EventId, UpdateBagSlot);
        }
    }
}
