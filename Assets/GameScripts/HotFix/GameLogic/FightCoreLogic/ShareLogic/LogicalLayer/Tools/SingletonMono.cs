using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T:SingletonMono<T> 
{
    private static T _intance;
    public static T Instance
    {
        get
        {
            if (_intance==null)
            {
                _intance = Object.FindObjectOfType<T>();
                if (_intance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    obj.AddComponent<T>();
                }
            }
            return _intance;
        }

    }

}
