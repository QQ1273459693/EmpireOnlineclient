using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletionMono<T> : MonoBehaviour where T : SingletionMono<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Object.FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }
}
