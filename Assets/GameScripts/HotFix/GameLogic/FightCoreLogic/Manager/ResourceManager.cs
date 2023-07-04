using GameBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager: Singleton<ResourceManager>
{
    /// <summary>
    /// 加载对象
    /// </summary>
    /// <param name="path"></param>
    /// <param name="parent"></param>
    /// <param name="restPos"></param>
    /// <param name="restScale"></param>
    /// <param name="restRoatate"></param>
    /// <returns></returns>
    public GameObject LoadObject(string path,Transform parent=null,bool restPos=false,bool restScale=false,bool restRoatate=false)
    {
        GameObject obj = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(path),parent);
        if (restPos)
        {
            obj.transform.localPosition = Vector3.zero;
        }
        if (restScale)
        {
            obj.transform.localScale = Vector3.zero;
        }
        if (restRoatate)
        {
            obj.transform.localRotation = Quaternion.identity;
        }
        return obj;
    }
    public T LoadObject<T>(string path,Transform parent = null)
    {
        GameObject obj = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(path),parent);
        T t = obj.GetComponent<T>();
        return t;
    }
    public T LoadAsset<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }
}
