using System.Collections;
using System.Collections.Generic;

namespace TEngine
{
    public interface IPoolObject
    {
        long Id { get; set; }
        bool IsFromPool { get; set; }
        void OnInit();
        void OnSpawn();
        void OnUnspawn();
        void OnRelease();
    }
}
