
using UnityEngine;

namespace SmoothCam
{
    public enum FilteringMode
    {
        point = 0,
        bilinear = 1
    }

    public static class FilteringModeUtils
    {

        public static FilterMode ToFilterMode(this FilteringMode mode)
        {
            switch (mode)
            {
                case FilteringMode.point: return FilterMode.Point;
                case FilteringMode.bilinear: return FilterMode.Bilinear;
                default: return FilterMode.Point;
            }
        }

    }
}
