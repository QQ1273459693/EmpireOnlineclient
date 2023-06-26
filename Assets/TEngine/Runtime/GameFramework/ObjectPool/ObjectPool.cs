using System;

namespace TEngine
{
    public class ObjectPool<T> : ObjectPoolBase, IObjectPool<T> where T : IPoolObject
    {
        private readonly EQueue<T> mQueue = new EQueue<T>();
        private Type mObjectType;

        public override Type ObjectType
        {
            get { return null == mObjectType ? mObjectType = typeof(T) : mObjectType; }
        }
        int mCount = 0;

        public override IPoolObject Spawn2()
        {
            return Spawn();
        }

        public T Spawn()
        {
            T tmpPoolObj;

            if (mQueue.Count > 0)
            {
                tmpPoolObj = mQueue.Dequeue();
            }
            else
            {
                tmpPoolObj = Activator.CreateInstance<T>();
                tmpPoolObj.OnInit();
            }

            tmpPoolObj.IsFromPool = true;
            tmpPoolObj.Id = IdGenerater.GenerateId();
            tmpPoolObj.OnSpawn();
            mCount++;
            return tmpPoolObj;
        }

        public override void Unspawn(IPoolObject obj)
        {
            if (null == obj)
                return;

            obj.OnUnspawn();
            mQueue.Enqueue((T)obj);
            mCount--;
        }

        public override void Release()
        {
            while (mQueue.Count > 0)
            {
                T tmpPoolObj = mQueue.Dequeue();
                tmpPoolObj.OnRelease();
            }
        }
    }
}
