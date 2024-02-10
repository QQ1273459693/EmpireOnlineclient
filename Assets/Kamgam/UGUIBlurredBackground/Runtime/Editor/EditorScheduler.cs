#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kamgam.UGUIBlurredBackground
{
    public static class EditorScheduler
    {
        static bool _registeredToEditorUpdate;

        struct FunctionInfo
        {
            public double Time;
            public Action Func;
            public bool IsMonoBehaviour;
            public string Id;

            public FunctionInfo(double time, Action func, bool isMonoBehaviour, string id = null)
            {
                Time = time;
                Func = func;
                IsMonoBehaviour = isMonoBehaviour;
                Id = id;
            }
        }

        static List<FunctionInfo> _functionTable = new List<FunctionInfo>();

        public static bool HasId(string id)
        {
            if (string.IsNullOrEmpty(id))
                return false;

            foreach (var tup in _functionTable)
            {
                if (tup.Id == id)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Schedules the given function to be executed after delay in seconds.
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="func"></param>
        /// <param name="id">If specified then existing entries with the same id are replaced by the new one.</param>
        public static void Schedule(float delay, Action func, string id = null)
        {
            registerToEditorUpdate();

            // Id an id was set then check if there already is a function with that id and if yes replace it.
            bool isMonoBehaviour;
            if (!string.IsNullOrEmpty(id))
            {
                for (int i = 0; i < _functionTable.Count; i++)
                {
                    if (_functionTable[i].Id == id)
                    {
                        isMonoBehaviour = func.Target is MonoBehaviour;
                        _functionTable[i] = new FunctionInfo(EditorApplication.timeSinceStartup + delay, func, isMonoBehaviour, id);
                        return;
                    }
                }
            }

            isMonoBehaviour = func.Target is MonoBehaviour;
            _functionTable.Add(new FunctionInfo(EditorApplication.timeSinceStartup + delay, func, isMonoBehaviour, id));
        }

        static void registerToEditorUpdate()
        {
            if (_registeredToEditorUpdate)
                return;

            EditorApplication.update += update;
            _registeredToEditorUpdate = true;
        }

        static void update()
        {
            double time = EditorApplication.timeSinceStartup;
            for (int i = _functionTable.Count-1; i >= 0; i--)
            {
                if (_functionTable[i].Time <= time)
                {
                    var info = _functionTable[i];
                    _functionTable.RemoveAt(i);

                    // Some shenanegans to make sure the object we are calling func on is not destroyed.
                    var func = info.Func;
                    if(info.IsMonoBehaviour)
                    {
                        var behaviour = func.Target as MonoBehaviour;
                        if (func != null && func.Target != null && behaviour != null && behaviour.gameObject != null)
                        {
                            func.Invoke();
                        }
                    }
                    else
                    {
                        func?.Invoke();
                    }
                    
                }
            }
        }
    }
}
#endif