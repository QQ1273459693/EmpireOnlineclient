
using System;

namespace TEngine
{
    /// <summary>
    /// 对象池管理器。
    /// </summary>
    public interface IObjectPoolManager
    {
        IObjectPool<T> GetObjectPool<T>() where T : IPoolObject;

        ObjectPoolBase GetObjectPoolByType(Type type);

        bool DestroyObjectPool<T>() where T : IPoolObject;

        void Release();
    }
}
