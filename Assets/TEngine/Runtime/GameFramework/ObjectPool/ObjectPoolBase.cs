using System;

namespace TEngine
{
    public abstract class ObjectPoolBase
    {
        public abstract Type ObjectType
        {
            get;
        }

        public abstract IPoolObject Spawn2();

        public abstract void Unspawn(IPoolObject obj);

        public abstract void Release();
    }
}
