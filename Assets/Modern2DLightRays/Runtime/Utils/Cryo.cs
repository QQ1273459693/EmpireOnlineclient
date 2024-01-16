using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;

namespace LeafUtilsRays
{

#if UNITY_EDITOR

    public static class CryoDrawer
    {
        public static void DrawProp(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            var amountRect = new Rect(position.x, position.y, position.width, position.height);
            EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("_val"), GUIContent.none);
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        [CustomPropertyDrawer(typeof(Cryo<double>))]
        public class CryoDrawerDouble : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) => DrawProp(position, property, GUIContent.none);
        }

        [CustomPropertyDrawer(typeof(Cryo<float>))]
        public class CryoDrawerFloat : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) => DrawProp(position, property, GUIContent.none); 
        }

        [CustomPropertyDrawer(typeof(Cryo<int>))]
        public class CryoDrawerInt : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) => DrawProp(position, property, GUIContent.none);
        }

        [CustomPropertyDrawer(typeof(Cryo<Color>))]
        public class CryoDrawerColor : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) => DrawProp(position, property, GUIContent.none);
        }

        [CustomPropertyDrawer(typeof(Cryo<Vector4>))]
        public class CryoDrawerVector4 : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) => DrawProp(position, property, GUIContent.none);
        }

        [CustomPropertyDrawer(typeof(Cryo<Vector3>))]
        public class CryoDrawerVector3 : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) => DrawProp(position, property, GUIContent.none);
        }

        [CustomPropertyDrawer(typeof(Cryo<Vector2>))]
        public class CryoDrawerVector2 : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) => DrawProp(position, property, GUIContent.none);
        }

    }

#endif


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
                {
                    onValueChanged.Invoke();
    
                }
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