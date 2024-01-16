using UnityEngine;

namespace LeafUtilsRays { 
    public static class ScreenUtils 
    {
        public static Vector2Int ScreenResolution() 
        {
            Vector2Int screenResolution;


            screenResolution = new Vector2Int(Screen.width, Screen.height);

#if UNITY_EDITOR
            System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
            Vector2 r = (Vector2)Res;
            screenResolution = r.Vector2Int();
#endif

            if (screenResolution.x == 0 || screenResolution.y == 0) screenResolution = new Vector2Int(Screen.width, Screen.height);
            if (screenResolution.x == 0 || screenResolution.y == 0) screenResolution = new Vector2Int(1920, 1080);
            return screenResolution;
        }
    }

}