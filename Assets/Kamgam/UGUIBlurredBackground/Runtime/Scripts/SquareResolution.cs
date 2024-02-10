using UnityEngine;

namespace Kamgam.UGUIBlurredBackground
{
    public enum SquareResolution
    {
        _32, 
        _64, 
        _128, 
        _256, 
        _512, 
        _1024, 
        _2048
    };

    public static class SquareResolutionsUtils
    {
        public static Vector2Int ToResolution(this SquareResolution res)
        {
            switch (res)
            {
                case SquareResolution._32:
                    return new Vector2Int(32, 32);

                case SquareResolution._64:
                    return new Vector2Int(64, 64);
                    
                case SquareResolution._128:
                    return new Vector2Int(128, 128);
                    
                case SquareResolution._256:
                    return new Vector2Int(256, 256);
                    
                case SquareResolution._512:
                    return new Vector2Int(512, 512);
                    
                case SquareResolution._1024:
                    return new Vector2Int(1024, 1024);
                    
                case SquareResolution._2048:
                    return new Vector2Int(2048, 2048);

                default:
                    return new Vector2Int(512, 512);
            }
        }
    }
}