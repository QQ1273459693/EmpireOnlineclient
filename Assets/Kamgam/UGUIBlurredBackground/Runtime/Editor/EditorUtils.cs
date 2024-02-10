#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Kamgam.UGUIBlurredBackground
{
    public static class EditorUtils
    {
        static EditorWindow _gameView;

        public static EditorWindow GetGameView()
        {
            System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
            System.Type type = assembly.GetType("UnityEditor.GameView");
            if(type != null)
            {
                _gameView = EditorWindow.GetWindow(type);
            }

            return _gameView;
        }

        public static void RefreshGameView()
        {
            var gameView = GetGameView();
            if (gameView != null)
            {
                gameView.Repaint();
            }
        }

        public static bool WasDestroyedWhileEditing(Component comp)
        {
            return !EditorApplication.isPlayingOrWillChangePlaymode
                && comp != null
                && comp.gameObject != null
                && comp.gameObject.scene.isLoaded
                && comp.gameObject.scene.IsValid();
        }
    }
}
#endif