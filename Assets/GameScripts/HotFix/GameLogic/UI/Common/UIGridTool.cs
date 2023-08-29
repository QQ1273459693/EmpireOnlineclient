﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using YooAsset;

namespace TEngine
{
    public class UIGridTool
    {
        public Transform m_RootGo;
        private string m_ResName;
        private List<GameObject> m_ElemList = new List<GameObject>();
        Dictionary<GameObject, AssetOperationHandle> ListSpawnHandle = new Dictionary<GameObject, AssetOperationHandle>();
        ResourcePackage package;

        private int m_Count;
        public int Count { get { return m_ElemList.Count; } }
        public UIGridTool(GameObject root, string resName, bool isGetChild = true)
        {
            m_RootGo = root.transform;
            m_ResName = resName;
            package = YooAssets.GetPackage("DefaultPackage");
            //GameModule.ObjectPool.CreateMultiSpawnObjectPool<GameObject>("FF");
            //Game.ResourcesMgr.LoadBundleByType(EABType.ItemPrefab, resName);
            //m_TemplateGo = Game.ResourcesMgr.GetAssetByType<GameObject>(EABType.ItemPrefab, resName);
            m_Count = 0;
            m_Count = root.transform.childCount;

            if (isGetChild)
            {
                for (int i = 0, max = m_Count; i < max; ++i)
                {
                    m_ElemList.Add(root.transform.GetChild(i).gameObject);
                }
            }
        }

        public void GenerateElem(int count)
        {
            if (count > Count)
            {
                while (count > Count)
                {
                    Add();
                }
            }
            else
            {
                while (count < Count)
                {
                    RemoveAt(Count - 1);
                }
            }
            m_Count = count;
        }

        public GameObject Add()
        {
            //if (null == m_TemplateGo)
            //{
            //    Log.Debug("TemplateGo is null!");
            //    return null;
            //}
            //GameObject tmpNewElem = Game.ResourcesMgr.ResPool.Spawn(EABType.ItemPrefab, m_ResName);
            //if (null == tmpNewElem)
            //{
            //    tmpNewElem = Hotfix.Instantiate(m_TemplateGo);
            //}
            //tmpNewElem.name = $"{m_TemplateGo.name}{m_ElemList.Count}";
            //tmpNewElem.SetActive(true);
            //tmpNewElem.transform.SetParent(m_RootGo.transform, false);
            AssetOperationHandle spawnHandle = package.LoadAssetSync<GameObject>(m_ResName); /*.SpawnSync(m_ResName, m_RootGo);*/
            GameObject go = spawnHandle.InstantiateSync(m_RootGo);
            //go.transform.localScale = Vector3.one;
            var Rect = go.GetComponent<RectTransform>();
            Rect.anchoredPosition3D = new Vector3(0, 0, 0);
            ListSpawnHandle.Add(go, spawnHandle);
            m_ElemList.Add(go);
            return go;
        }
        public void MoveToLast(int index)
        {
            if (index >= m_ElemList.Count)
            {
                return;
            }
            GameObject tmpElem = m_ElemList[index];
            m_ElemList.RemoveAt(index);
            m_ElemList.Add(tmpElem);
            tmpElem.transform.SetAsLastSibling();
        }

        public GameObject Get(int index)
        {
            return index >= m_ElemList.Count ? null : m_ElemList[index];
        }
        public GameObject GetFormIndex(int index)
        {
            return m_ElemList.Find(a => a.transform.GetSiblingIndex() == index);
        }
        public void RemoveAt(int index)
        {
            GameObject obj = m_ElemList[index];
            m_ElemList.RemoveAt(index);
            ListSpawnHandle[obj].Release();
        }

        public void RemoveAndDespawn(GameObject obj)
        {
            m_ElemList.Remove(obj);
            ListSpawnHandle[obj].Release();
        }
        public void Remove(GameObject obj)
        {
            m_ElemList.Remove(obj);
        }
        public void Clear()
        {
            foreach (var item in ListSpawnHandle.Values)
            {
                item.Release();
            }
            ListSpawnHandle.Clear();
            m_ElemList.Clear();
            m_Count = 0;
        }
    }
}
