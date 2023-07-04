using System;
using System.Collections;
using System.Collections.Generic;
using TEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    public class SpineAnimBox : IPoolObject
    {
        GameObject GO;

        //��UIHUD���
        TextMeshPro HpText;
        TextMeshPro DamageText;


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
        /// ˢ������
        /// </summary>
        public void RefreshData(string SpineResName,bool isFlii)
        {
            m_SpineAnimRes = SpineResName;
            RefreshSpineModle(isFlii);
            HpText = m_SpineAnimLoader.m_SpineParentGO.transform.Find("HpText").GetComponent<TextMeshPro>();
            DamageText = m_SpineAnimLoader.m_SpineParentGO.transform.Find("DamageHp").GetComponent<TextMeshPro>();
        }
        public void PlayAnim(string AnimName,bool isLoop=false)
        {
            m_SpineAnimLoader.PlayAnimation(AnimName,isLoop);
        }
        /// <summary>
        /// ����Hp���HUD��ʾ
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="hpRateValue"></param>
        public void UpdateHp_Hud(int damage, float hpRateValue)
        {
            DamageText.text = (damage > 0 ? "-" : "+") + Mathf.Abs(damage);
            HpText.text = $"Hp:{hpRateValue}";
        }
        /// <summary>
        /// ˢ��Spineģ��
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
