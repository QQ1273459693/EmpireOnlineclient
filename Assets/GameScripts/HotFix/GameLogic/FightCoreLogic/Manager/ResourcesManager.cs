using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : Singleton<ResourcesManager>
{
    public GameObject LoadObject(string path, Transform parent = null, bool restPos = false, bool restScale = false,bool restRotate=false)
    {
        GameObject obj= GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(path), parent);
        if (restPos)
            obj.transform.localPosition = Vector3.zero;
        if (restScale)
            obj.transform.localScale = Vector3.one;
        if (restRotate)
            obj.transform.localRotation = Quaternion.identity;
        return obj;
    }
    public T LoadObject<T>(string path, Transform parent = null) 
    {
        GameObject obj = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(path),parent);
        T t= obj.GetComponent<T>();
        return t;
    }
    public T LoadAsset<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }


}
