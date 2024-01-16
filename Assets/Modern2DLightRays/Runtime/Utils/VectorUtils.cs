using UnityEngine;

namespace LeafUtilsRays
{
    public static class VectorUtils
    {
        public static Vector2Int Vector2Int(this Vector2 v) => new Vector2Int((int)v.x, (int)v.y);
        public static Vector2 Vector2(this Vector2Int v) => new Vector2(v.x, v.y);
    }

}