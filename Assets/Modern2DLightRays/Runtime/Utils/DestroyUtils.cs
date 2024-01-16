using UnityEngine;

namespace LeafUtilsRays
{
    public static class DestroyUtils
    {
        public static void DestroyAlways(GameObject obj)
        {
            if (Application.isPlaying) GameObject.Destroy(obj);
            else GameObject.DestroyImmediate(obj);
        }

        public static void DestroyAlways(Component obj)
        {
            if (Application.isPlaying) GameObject.Destroy(obj);
            else GameObject.DestroyImmediate(obj);
        }
    }

}