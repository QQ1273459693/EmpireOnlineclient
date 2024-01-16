using UnityEngine;

namespace LeafUtils
{
    public static class DestroyUtils
    {
        public static void DestroyAlways(GameObject obj)
        {
            if (Application.isPlaying) GameObject.Destroy(obj);
            else GameObject.DestroyImmediate(obj);
        }
    }

}