using System;
using System.Collections.Generic;
using UnityEngine;

namespace TEngine
{
    /// <summary>
    /// 对象池模块。
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ObjectPoolModule : GameFrameworkModuleBase
    {
        public IObjectPoolManager m_ObjectPoolManager = null;

        /// <summary>
        /// 获取对象池数量。
        /// </summary>
        //public int Count
        //{
        //    get
        //    {
        //        return ;
        //    }
        //}

        /// <summary>
        /// 游戏框架模块初始化。
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            m_ObjectPoolManager = GameFrameworkEntry.GetModule<IObjectPoolManager>();
            if (m_ObjectPoolManager == null)
            {
                Log.Fatal("Object pool manager is invalid.");
                return;
            }
        }

        public void Clear()
        {
            m_ObjectPoolManager.Release();
        }

    }
}
