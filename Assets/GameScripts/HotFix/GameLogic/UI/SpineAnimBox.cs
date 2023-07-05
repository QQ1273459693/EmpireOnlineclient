using System;
using System.Collections;
using System.Collections.Generic;
using TEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GameLogic
{
    public class SpineAnimBox : IPoolObject
    {
        public GameObject GO;

        //非UIHUD组件
        TextMeshPro HpText;
        TextMeshPro DamageText;
        Transform AttackUI;
        SpriteRenderer AttackUiSprite;

        public long Id { get; set ; }
        public bool IsFromPool { get ; set ; }

        private SpineAnimLoader m_SpineAnimLoader;
        int m_EnemySpineBaseID;
        string m_SpineAnimRes;

        public void OnInit()
        {
            
        }

        public void OnRelease()
        {

        }
        public void IntObj(GameObject OBJ)
        {
            GO=OBJ;
        }
        /// <summary>
        /// 刷新数据
        /// </summary>
        public void RefreshData(string SpineResName,bool isFlii)
        {
            m_SpineAnimRes = SpineResName;
            RefreshSpineModle(isFlii);
            HpText = m_SpineAnimLoader.m_SpineParentGO.transform.Find("HpText").GetComponent<TextMeshPro>();
            DamageText = m_SpineAnimLoader.m_SpineParentGO.transform.Find("DamageHp").GetComponent<TextMeshPro>();
            AttackUI = m_SpineAnimLoader.m_SpineParentGO.transform.Find("AttackTipsPos/AttackUITips").transform;
            AttackUiSprite= AttackUI.GetComponent<SpriteRenderer>();
            AttackUI.gameObject.SetActive(false);
        }
        public void PlayAnim(string AnimName,bool isLoop=false)
        {
            m_SpineAnimLoader.PlayAnimation(AnimName,isLoop);
            if (AnimName=="BeAttack")
            {
                return;
            }
            AttackUI.gameObject.SetActive(true);
            AttackUiSprite.color = Color.white;
            AttackUiSprite.DOBlendableColor(Color.clear,1F);
            AttackUI.DOLocalJump(new Vector3(0, 3.5F), 2, 1, 0.9F).OnComplete(() =>
            {
                AttackUI.gameObject.SetActive(false);
            });
        }
        /// <summary>
        /// 更新Hp相关HUD显示
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="hpRateValue"></param>
        public void UpdateHp_Hud(int damage, float hpRateValue)
        {
            //DamageText.text = (damage > 0 ? "-" : "+") + Mathf.Abs(damage);
            HpText.text = $"Hp:{hpRateValue}";
        }
        /// <summary>
        /// 刷新Spine模型
        /// </summary>
        void RefreshSpineModle(bool isFill)
        {
            m_SpineAnimLoader?.Dispose();
            m_SpineAnimLoader = ReferencePool.Acquire<SpineAnimLoader>();
            m_SpineAnimLoader.Load(GO,m_SpineAnimRes,false,isFill);
        }
        public void OnSpawn()
        {
            
        }

        public void OnUnspawn()
        {
            m_SpineAnimLoader?.Dispose();
            m_SpineAnimLoader = null;
        }
    }
}
