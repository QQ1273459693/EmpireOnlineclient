using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace TEngine
{
    internal sealed class ObjectPoolManager : GameFrameworkModule, IObjectPoolManager
    {
        private readonly Dictionary<Type, ObjectPoolBase> mObjectPools;

        public ObjectPoolManager()
        {
            mObjectPools = new Dictionary<Type, ObjectPoolBase>();
        }

        public IObjectPool<T> GetObjectPool<T>() where T : IPoolObject
        {
            ObjectPoolBase tmpObjectPool;

            if (!mObjectPools.TryGetValue(typeof(T), out tmpObjectPool))
            {
                tmpObjectPool = new ObjectPool<T>();
                mObjectPools.Add(typeof(T), tmpObjectPool);
            }

            return (IObjectPool<T>)tmpObjectPool;
        }

        public ObjectPoolBase GetObjectPoolByType(Type type)
        {
            return mObjectPools[type];
        }

        public bool DestroyObjectPool<T>() where T : IPoolObject
        {
            return mObjectPools.Remove(typeof(T));
        }

        public void Release()
        {
            Dictionary<Type, ObjectPoolBase>.Enumerator tmpItor = mObjectPools.GetEnumerator();
            while (tmpItor.MoveNext())
            {
                tmpItor.Current.Value.Release();
            }

            mObjectPools.Clear();
        }

        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {

        }

        internal override void Shutdown()
        {

        }
    }
}
