using System;
using UnityEngine;
using UnityEngine.Events;

namespace LeafUtils
{

    //used for variables that don't need to be updated every frame
    [System.Serializable]
    public class Cryo<T> where T : IEquatable<T>
    {
        [SerializeField]
        private T _val;

        [SerializeField]
        public bool On = true;

        [SerializeField]
        public T value
        {
            get { return _val; }
            set
            {
                T oldValue = _val;
                _val = value;
                if (!On) return;
                if (!value.Equals(oldValue) && onValueChanged != null)
                    onValueChanged.Invoke();
            }
        }
        public UnityAction onValueChanged;

        public Cryo(T value, UnityAction onValueChanged)
        {
            this.value = value;
            this.onValueChanged = onValueChanged;
        }

        public Cryo(T value)
        {
            this.value = value;
        }
        
        public static void SetCallbacks(UnityAction onValChanged, Cryo<T>[] cryos)
        {
            foreach (var c in cryos) c.onValueChanged = onValChanged;
        }
    }

}