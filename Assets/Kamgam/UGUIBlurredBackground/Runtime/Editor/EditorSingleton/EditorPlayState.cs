#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Kamgam.UGUIBlurredBackground.EditorSingleton
{
    internal static class EditorPlayState
    {
        public enum PlayState
        {
            NotListening = 0,
            Editing = 1,
            FromEditToPlay = 2,
            Playing = 3,
            FromPlayToEdit = 4
        }

        static string _editorPlayStateKey;
        static string EditorPlayStateKey
        {
            get
            {
                if (string.IsNullOrEmpty(_editorPlayStateKey))
                {
                    _editorPlayStateKey = typeof(EditorPlayState).FullName + ".State";
                }

                return _editorPlayStateKey;
            }
        }

        static PlayState _state = PlayState.NotListening;
        public static PlayState State
        {
            get
            {
                if (_state == PlayState.NotListening)
                    StartListening();

                return _state;
            }
        }

        [DidReloadScripts]
        [InitializeOnLoadMethod]
        public static void StartListening()
        {
            if (_state != PlayState.NotListening)
                return;

            EditorApplication.playModeStateChanged -= onPlayModeChanged;
            EditorApplication.playModeStateChanged += onPlayModeChanged;

            int state = SessionState.GetInt(EditorPlayStateKey, -1);
            if (state >= 0)
            {
                _state = (PlayState)state;
            }
            else
            {
                _state = EditorApplication.isPlayingOrWillChangePlaymode ? PlayState.Playing : PlayState.Editing;
            }
        }

        private static void onPlayModeChanged(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredPlayMode)
            {
                _state = PlayState.Playing;
            }
            if (change == PlayModeStateChange.ExitingPlayMode)
            {
                _state = PlayState.FromPlayToEdit;
            }
            if (change == PlayModeStateChange.EnteredEditMode)
            {
                _state = PlayState.Editing;
            }
            if (change == PlayModeStateChange.ExitingEditMode)
            {
                _state = PlayState.FromEditToPlay;
            }

            SessionState.SetInt(EditorPlayStateKey, (int)_state);
        }

        public static void StopListening()
        {
            _state = PlayState.NotListening;
            SessionState.EraseInt(EditorPlayStateKey);

            EditorApplication.playModeStateChanged -= onPlayModeChanged;
        }

        public static bool IsInBetween
        {
            get => _state == PlayState.FromEditToPlay || _state == PlayState.FromPlayToEdit;
        }

        public static bool IsEditing
        {
            get => _state == PlayState.Editing;
        }

        public static bool IsPlaying
        {
            get => _state == PlayState.Playing;
        }
    }
}
#endif