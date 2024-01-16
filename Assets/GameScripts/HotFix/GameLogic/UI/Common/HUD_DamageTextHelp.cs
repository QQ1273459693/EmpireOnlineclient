using GameLogic;
using System.Collections.Generic;
using UniFramework.Pooling;
using UnityEngine;
using UnityEngine.VFX;
using YooAsset;

namespace TEngine
{
    public static class HUD_DamageTextHelp
    {
        public static Transform m_RootGo;
        static Dictionary<Transform, SpawnHandle> ListSpawnHandle = new Dictionary<Transform, SpawnHandle>();
        static Spawner spawner;
        static List<HUD_DamageTextPool> HUD_DamageTextPools = new List<HUD_DamageTextPool>();
        public static Dictionary<int, List<Vector3>> m_HUDPosDict = new Dictionary<int, List<Vector3>>();
        static Dictionary<int,Vector3> m_HUDFightPos = new Dictionary<int, Vector3>();
        static List<float> m_IntervalPosList=new List<float>();
        static string HUDTextRES = "Fight_HUDDamageText";
        
        //public static int Count { get { return m_ElemList.Count; } }
        public static void SetHUDTextRoot(GameObject root)
        {
            m_RootGo = root.transform;
            spawner = UniPooling.CreateSpawner("DefaultPackage");

            //提前算好全部位置的落点信息
            //m_IntervalPosList.Add(1.78F);
            //m_IntervalPosList.Add(2.66F);
            //m_IntervalPosList.Add(3.08F);
            //m_IntervalPosList.Add(3.22F);

            m_IntervalPosList.Add(0.58F);
            m_IntervalPosList.Add(0.86F);
            m_IntervalPosList.Add(0.98F);
            m_IntervalPosList.Add(1.04F);

            for (int i = 0; i < 5; i++)
            {
                List<Vector3> Pos = new List<Vector3>();
                var Trs = FightRoundWindow.Instance.LeftFightList[i];
                m_HUDFightPos.Add(i, Trs.position);
                for (int j = 0; j < m_IntervalPosList.Count; j++)
                {
                    Vector3 vector3 = new Vector3(Trs.position.x- m_IntervalPosList[j], Trs.position.y-(0.01F*(i+1)));
                    Pos.Add(vector3);
                }
                m_HUDPosDict.Add(i, Pos);
            }

            for (int i = 0; i < 5; i++)
            {
                List<Vector3> Pos = new List<Vector3>();
                var Trs = FightRoundWindow.Instance.RightFightList[i];
                m_HUDFightPos.Add(5+i, Trs.position);
                for (int j = 0; j < m_IntervalPosList.Count; j++)
                {
                    Vector3 vector3 = new Vector3(Trs.position.x + m_IntervalPosList[j], Trs.position.y - (0.01F * (i + 1)));
                    Pos.Add(vector3);
                }
                m_HUDPosDict.Add(5+i, Pos);
            }
        }
        public static void GenerHUDText(int SeatID,int Count,int Damage)
        {
            bool isCan = false;
            Vector3 StartPos = m_HUDFightPos[SeatID];
            for (int i = 0; i < HUD_DamageTextPools.Count; i++)
            {
                if (!HUD_DamageTextPools[i].isCanShow())
                {
                    //可以获取
                    isCan = true;
                    HUD_DamageTextPools[i].TextJumpStart(StartPos,m_HUDPosDict[SeatID], Count, Damage,true);
                    break;
                }
            }
            if (!isCan)
            {
                //没有可用的,去对象池拿
                SpawnHandle handle = spawner.SpawnSync(HUDTextRES, m_RootGo);
                GameObject go = handle.GameObj;
                go.transform.localScale = Vector3.one;
                HUD_DamageTextPool hUD_Damage = new HUD_DamageTextPool(go);
                hUD_Damage.TextJumpStart(StartPos, m_HUDPosDict[SeatID], Count, Damage);
                //Log.Debug("拿了多少次QQ");
                HUD_DamageTextPools.Add(hUD_Damage);
            }
        }
        public static void Clear()
        {
            foreach (var item in ListSpawnHandle.Values)
            {
                item.Discard();
            }
            ListSpawnHandle.Clear();
            HUD_DamageTextPools.Clear();
        }
    }
}
