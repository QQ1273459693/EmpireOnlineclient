using Cysharp.Threading.Tasks;
using dnlib.Threading;
using System;
using System.Collections.Generic;
using System.Threading;
using UniFramework.Pooling;
using UnityEngine;
using UnityEngine.VFX;
using YooAsset;

namespace TEngine
{
    public static class EFX_ParticleHelp
    {
        public static Transform m_RootGo;
        private static Dictionary<int,float> m_ParticleDealyDict = new Dictionary<int, float>();
        private static Dictionary<string, List<ParticleSystem>> m_ParticleDict = new Dictionary<string, List<ParticleSystem>>();
        static Dictionary<GameObject, SpawnHandle> ListSpawnHandle = new Dictionary<GameObject, SpawnHandle>();
        static Spawner spawner;

        //public static int Count { get { return m_ElemList.Count; } }
        public static void SetParticleRoot(GameObject root)
        {
            m_RootGo = root.transform;
            spawner = UniPooling.CreateSpawner("DefaultPackage");
            //spawner.CreateGameObjectPoolSync("FightParticlePool");
        }
        /// <summary>
        /// 延迟特效命中
        /// </summary>
        public static void DealyParticle(int VFXID,Action DealyAC)
        {
            if (VFXID==0)
            {
                DealyAC?.Invoke();
                return;
            }
            float Dealy=0;
            if (!m_ParticleDealyDict.TryGetValue(VFXID,out Dealy))
            {
                var VFXBase = ConfigLoader.Instance.Tables.TBVFXSkillModelBase.Get(VFXID);
                if (VFXBase == null)
                {
                    Log.Error("没有找到特效配置文件ID:" + VFXID);
                    return;
                }
                Dealy = VFXBase.DealyHit;
                m_ParticleDealyDict.Add(VFXID, Dealy);
            }
            if (Dealy>0.0F)
            {
                _ = VFXDealyAsync(Dealy, DealyAC);
            }
            else
            {
                DealyAC?.Invoke();
            }
        }
        static async UniTask VFXDealyAsync(float Dealy,Action DealyAC)
        {
            var cts = new CancellationTokenSource();
            await UniTask.Delay((int)Dealy*1000, cancellationToken: cts.Token);
            DealyAC?.Invoke();
            cts.Cancel();
        }
        public static void GenerParticle(string ResName, Vector2 pos)
        {
            List<ParticleSystem> m_ParticleList;
            if (m_ParticleDict.TryGetValue(ResName, out m_ParticleList))
            {
                //看下有没有可以播放的粒子
                bool isCanGet = false;
                for (int i = 0; i < m_ParticleList.Count; i++)
                {
                    ParticleSystem particle = m_ParticleList[i];
                    //Log.Debug($"当前播放时间:{particle.time},持续时间:{particle.main.duration}");
                    if (particle.isStopped)
                    {
                        isCanGet = true;
                        particle.transform.position = pos;
                        particle.Play(true);
                        break;
                    }
                }
                if (!isCanGet)
                {
                    //全部都在播放
                    SpawnHandle handle = spawner.SpawnSync(ResName, m_RootGo);
                    GameObject go = handle.GameObj;
                    go.transform.localScale = Vector3.one;
                    go.transform.position = pos;
                    ParticleSystem particle = go.transform.GetChild(0).GetComponent<ParticleSystem>();
                    if (m_ParticleDict.ContainsKey(ResName))
                    {
                        //说明有
                        m_ParticleDict[ResName].Add(particle);
                    }
                    else
                    {
                        List<ParticleSystem> m_PartList = new List<ParticleSystem>();
                        m_PartList.Add(particle);
                        m_ParticleDict.Add(ResName, m_PartList);
                    }
                    ListSpawnHandle.Add(go, handle);
                }
            }
            else
            {
                SpawnHandle handle = spawner.SpawnSync(ResName, m_RootGo);
                GameObject go = handle.GameObj;
                go.transform.localScale = Vector3.one;
                go.transform.position = pos;
                ParticleSystem particle = go.transform.GetChild(0).GetComponent<ParticleSystem>();
                if (m_ParticleDict.ContainsKey(ResName))
                {
                    //说明有
                    m_ParticleDict[ResName].Add(particle);
                }
                else
                {
                    List<ParticleSystem> m_PartList = new List<ParticleSystem>();
                    m_PartList.Add(particle);
                    m_ParticleDict.Add(ResName, m_PartList);
                }
                ListSpawnHandle.Add(go, handle);
            }

        }
        public static void Clear()
        {
            foreach (var item in ListSpawnHandle.Values)
            {
                item.Discard();
            }
            ListSpawnHandle.Clear();
            m_ParticleDict.Clear();
        }
    }
}