using System;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using GameLogic;
using TMPro;
using System.Collections.Generic;

namespace TEngine
{
    [Window(UILayer.Tips)]
    class TipsWnd_TipsInfo : UIWindow,UIWindow.IInitData<SkillData>
    {
        Text m_DesText;
        SkillData m_SkillData;
        public override void ScriptGenerator()
        {
            m_DesText = FindChildComponent<Text>("Bg/Tips/RectScroll/Mid/DesText");


            RegisterEventClick(FindChild("BlackBg").gameObject, CancleBtn);
        }

        void CancleBtn(GameObject obj, PointerEventData eventData)
        {
            Close();
        }

        public override void AfterShow()
        {
            base.AfterShow();
            RefreshData();
        }
        void RefreshData()
        {
            var SkillBase = ConfigLoader.Instance.Tables.TbSwordSkillBase.Get(m_SkillData.SkID);
            string Des = SkillBase.Name[m_SkillData.Lv];
            var DesSpit = SkillBase.Des[m_SkillData.Lv].Split(',');
             
            switch (SkillBase.SkillType)
            {
                case 0://被动技能
                    var SkillData = SkillBase.Attribute[m_SkillData.Lv].SkillAttrs;
                    for (int i = 0; i < DesSpit.Length; i++)
                    {
                        string Precent = SkillData[i].Percent == 0 ? "" : "%";
                        Des += "\n"+"效果:"+string.Format(DesSpit[i], $"+{SkillData[i].Value}{Precent}");
                    }
                    break;
                case 1://主动技能
                    var SkillData1 = SkillBase.SkillParam[m_SkillData.Lv].SkilParams;
                    var SkillParma = SkillBase.AttackParam[m_SkillData.Lv];
                    Des += $"\nMP消耗:{SkillParma.MP}";
                    Des += $"\n种类:{SkillTypeName(SkillBase.AttackType)}";
                    Des += $"\n目标范围:{SkillRadiusName(SkillParma.ATKRAG)}";
                    for (int i = 0; i < DesSpit.Length; i++)
                    {
                        var Data = SkillData1[i];
                        string Precent = Data.Percent ? "%" : "";
                        Des += "\n" + "效果:" + string.Format(DesSpit[i], $"+{Data.Value}{Precent}")+ SkillBuffName(Data.BUFFID);
                    }
                    Des += $"\n持续回合:{SkillParma.Round}";
                    break;
                case 2://自动技能
                    var SkillData2 = SkillBase.SkillParam[m_SkillData.Lv].SkilParams;
                    var SkillParma2 = SkillBase.AttackParam[m_SkillData.Lv];
                    Des += $"\nMP消耗:{SkillParma2.MP}";
                    Des += $"\n种类:{SkillTypeName(SkillBase.AttackType)}";
                    Des += $"\n目标范围:{SkillRadiusName(SkillParma2.ATKRAG)}";
                    for (int i = 0; i < DesSpit.Length; i++)
                    {
                        var Data = SkillData2[i];
                        string Precent = Data.Percent ? "%" : "";
                        Des += "\n" + "效果:" + string.Format(DesSpit[i], $"+{Data.Value}{Precent}") + SkillBuffName(Data.BUFFID);
                    }
                    Des += $"\n持续回合:{SkillParma2.Round}";
                    break;
            }
            m_DesText.text = Des;
        }
        string SkillTypeName(GameConfig.SKILL.TYPE AttackType)
        {
            string Name = string.Empty;
            switch (AttackType)
            {
                case GameConfig.SKILL.TYPE.SwordType:
                    Name = "剑类";
                    break;
                case GameConfig.SKILL.TYPE.Close_Combat:
                    Name = "近战";
                    break;
                case GameConfig.SKILL.TYPE.Magic_Attack:
                    Name = "魔法";
                    break;
                case GameConfig.SKILL.TYPE.Curse:
                    Name = "诅咒";
                    break;
                case GameConfig.SKILL.TYPE.CURE:
                    Name = "治疗";
                    break;
                case GameConfig.SKILL.TYPE.SUBSIDIARY:
                    Name = "辅助";
                    break;
            }
            return Name;
        }
        string SkillRadiusName(int Index)
        {
            int AbsIndex = (int)MathF.Abs(Index);
            string Name = string.Empty;
            switch (AbsIndex)
            {
                case 1:
                    Name = Index>0?"敌方单人":"我方单人";
                    break;
                case 2:
                    Name = Index > 0 ? "敌方十字" : "我方十字";
                    break;
                case 3:
                    Name = Index > 0 ? "敌方全部" : "我方全部";
                    break;
                case 4:
                    Name = "全体范围";
                    break;
                case 5:
                    Name = "自身";
                    break;
            }
            return Name;
        }
        /// <summary>
        /// BuffName
        /// </summary>
        /// <returns></returns>
        string SkillBuffName(int Index)
        {
            string Name = string.Empty;
            switch (Index)
            {
                case 1:
                    Name = "<color=#ffe233>(无敌)</color>";
                    break;
                case 2:
                    Name = "<color=#095f86>(物理攻击无效)</color>";
                    break;
                case 3:
                    Name = "<color=#4d0986>(魔法攻击无效)</color>";
                    break;
                case 4:
                    Name = "<color=#7dbdcf>(免疫定身、混乱、封魔状态)</color>";
                    break;
                case 5:
                    Name = "<color=#ba3c3b>(火烧)</color>";
                    break;
                case 6:
                    Name = "<color=#ba3c3b>(封魔)</color>";
                    break;
                case 7:
                    Name = "<color=#ba3c3b>(定身)</color>";
                    break;
                case 8:
                    Name = "<color=#ba3c3b>(混乱)</color>";
                    break;
                case 9:
                    Name = "<color=#0a6b43>(中毒)</color>";
                    break;
            }
            return Name;
        }
        public override void BeforeClose()
        {
            base.BeforeClose();
        }

        public void InitData(SkillData a)
        {
            m_SkillData = a;
        }
    }
}
