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
        public void RefreshData(string SpineResName)
        {
            m_SpineAnimRes = SpineResName;
            RefreshSpineModle();
        }
        /// <summary>
        /// 刷新Spine模型
        /// </summary>
        public void RefreshSpineModle()
        {
            m_SpineAnimLoader?.Dispose();
            m_SpineAnimLoader = ReferencePool.Acquire<SpineAnimLoader>();
            m_SpineAnimLoader.Load(GO,m_SpineAnimRes);
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
