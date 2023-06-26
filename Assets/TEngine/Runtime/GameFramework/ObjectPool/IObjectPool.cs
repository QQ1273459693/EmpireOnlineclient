
namespace TEngine
{
    public interface IObjectPool<T> where T : IPoolObject
    {
        T Spawn();
    }
}
