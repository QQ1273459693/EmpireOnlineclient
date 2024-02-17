using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace GameLogic
{
    public class FightRoundWindow : SingletonMono<FightRoundWindow>
    {
        public Camera Camera3D;
        public Camera UiCamera;

        public Transform AttackCenter;
        public float Distance;
        public float YDistance;

        public List<Transform> LeftFightList;
        public List<Transform> LeftFightList_1;
        public List<Transform> RightFightList;
        public List<Transform> RightFightList_1;
        public Transform FightUIRoot;
        public RectTransform Canvas;
        public GameObject ParticleSystemRoot;
        public GameObject HUDTextSystemRoot;
        public GameObject m_FightUI;
        public Text m_RoundText;
        public Text m_BattleEndText;
        private int mMaxRoundid;
        // Start is called before the first frame update
        void Start()
        {
            return; 
        }
        /// <summary>
        /// ½øÈëÕ½¶·
        /// </summary>
        public void EnterFight()
        {
            Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("DeCoration"));
            Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("Player"));
            Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("NPC"));
            NewWorldManager.Initialize();
            m_FightUI.SetActive(true);
            m_BattleEndText.text = "";
        }
        public void ExitFight()
        {
            Camera.main.cullingMask |= (1 << LayerMask.NameToLayer("DeCoration"));
            Camera.main.cullingMask |= (1 << LayerMask.NameToLayer("Player"));
            Camera.main.cullingMask |= (1 << LayerMask.NameToLayer("NPC"));
            m_FightUI.SetActive(false);
            for (int i = 0; i < LeftFightList.Count; i++)
            {
                LeftFightList[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < RightFightList.Count; i++)
            {
                RightFightList[i].gameObject.SetActive(false);
            }
        }
        public void SwitchFightPos(int LeftFightCount,int RightFightCount)
        {
            Vector2 min = Camera.main.ViewportToWorldPoint(Vector2.zero);
            Vector2 max = Camera.main.ViewportToWorldPoint(Vector2.one);
            float LeftX = min.x + Distance;
            float RightX = max.x - Distance;
            float Height = max.y - (YDistance * 2);
            float MidY = (max.y + min.y) / 2;//ÖÐÎ»Êý
            float TopAllY = max.y - MidY;
            float CenterPos = max.y;

            Debug.Log($"ÆÁÄ»¿í:{Screen.width},¸ß:{Screen.height},×óÏÂ½Ç:{min},ÓÒÉÏ½Ç:{max}");
            for (int i = 0; i < LeftFightList.Count; i++)
            {
                LeftFightList[i].gameObject.SetActive(true);
            }
            for (int i = 0; i < RightFightList.Count; i++)
            {
                RightFightList[i].gameObject.SetActive(true);
            }
            switch (LeftFightCount)
            {
                case 1:
                    LeftFightList[0].position = new Vector3(LeftX, MidY);
                    //Òþ²Ø
                    LeftFightList[1].gameObject.SetActive(false);
                    LeftFightList[2].gameObject.SetActive(false);
                    LeftFightList[3].gameObject.SetActive(false);
                    LeftFightList[4].gameObject.SetActive(false);
                    break;
                case 2:
                    float YD = Height / (float)3;
                    LeftFightList[0].position = new Vector3(LeftX, MidY+ YDistance);
                    LeftFightList[1].position = new Vector3(LeftX, MidY- YDistance);
                    //Òþ²Ø
                    LeftFightList[2].gameObject.SetActive(false);
                    LeftFightList[3].gameObject.SetActive(false);
                    LeftFightList[4].gameObject.SetActive(false);
                    break;
                case 3:
                    float YD1 = (Height / (float)2) + YDistance;
                    LeftFightList[0].position = new Vector3(LeftX, MidY);
                    LeftFightList[1].position = new Vector3(LeftX, MidY + YDistance);
                    LeftFightList[2].position = new Vector3(LeftX, MidY - YDistance);
                    //Òþ²Ø
                    LeftFightList[3].gameObject.SetActive(false);
                    LeftFightList[4].gameObject.SetActive(false);
                    break;
                case 4:
                    float YD2 = TopAllY / (float)2;
                    LeftFightList[0].position = new Vector3(LeftX, MidY+ YD2);
                    LeftFightList[1].position = new Vector3(LeftX, MidY+ YD2 *2);
                    LeftFightList[2].position = new Vector3(LeftX, MidY- YD2);
                    LeftFightList[3].position = new Vector3(LeftX, MidY -(YD2* 2));
                    //Òþ²Ø
                    LeftFightList[4].gameObject.SetActive(false);
                    break;
                case 5:
                    float YD3 = TopAllY / (float)3;
                    LeftFightList[0].position = new Vector3(LeftX, MidY);
                    LeftFightList[1].position = new Vector3(LeftX, MidY + YD3);
                    LeftFightList[2].position = new Vector3(LeftX, MidY + YD3 * 2);
                    LeftFightList[3].position = new Vector3(LeftX, MidY - YD3);
                    LeftFightList[4].position = new Vector3(LeftX, MidY - (YD3 * 2));
                    break;
            }

            //ÓÒ±ß
            switch (RightFightCount)
            {
                case 1:
                    RightFightList[0].position = new Vector3(RightX, MidY);
                    //Òþ²Ø
                    RightFightList[1].gameObject.SetActive(false);
                    RightFightList[2].gameObject.SetActive(false);
                    RightFightList[3].gameObject.SetActive(false);
                    RightFightList[4].gameObject.SetActive(false);
                    break;
                case 2:
                    float YD = Height / (float)3;
                    RightFightList[0].position = new Vector3(RightX, MidY + YDistance);
                    RightFightList[1].position = new Vector3(RightX, MidY - YDistance);
                    //Òþ²Ø
                    RightFightList[2].gameObject.SetActive(false);
                    RightFightList[3].gameObject.SetActive(false);
                    RightFightList[4].gameObject.SetActive(false);
                    break;
                case 3:
                    float YD1 = (Height / (float)2) + YDistance;
                    RightFightList[0].position = new Vector3(RightX, MidY);
                    RightFightList[1].position = new Vector3(RightX, MidY + YDistance);
                    RightFightList[2].position = new Vector3(RightX, MidY - YDistance);
                    //Òþ²Ø
                    RightFightList[3].gameObject.SetActive(false);
                    RightFightList[4].gameObject.SetActive(false);
                    break;
                case 4:
                    float YD2 = TopAllY / (float)2;
                    RightFightList[0].position = new Vector3(RightX, MidY + YD2);
                    RightFightList[1].position = new Vector3(RightX, MidY + YD2 * 2);
                    RightFightList[2].position = new Vector3(RightX, MidY - YD2);
                    RightFightList[3].position = new Vector3(RightX, MidY - (YD2 * 2));
                    //Òþ²Ø
                    RightFightList[4].gameObject.SetActive(false);
                    break;
                case 5:
                    float YD3 = TopAllY / (float)3;
                    RightFightList[0].position = new Vector3(RightX, MidY);
                    RightFightList[1].position = new Vector3(RightX, MidY + YD3);
                    RightFightList[2].position = new Vector3(RightX, MidY + YD3 * 2);
                    RightFightList[3].position = new Vector3(RightX, MidY - YD3);
                    RightFightList[4].position = new Vector3(RightX, MidY - (YD3 * 2));
                    break;
            }
        }
        public void RoundStart()
        {
            mMaxRoundid = NewBattleWorld.Instance.roundLoigc.MaxRoundID;
            m_RoundText.text = $"»ØºÏÊý:{NewBattleWorld.Instance.roundLoigc.RoundId}/{mMaxRoundid}";
        }
        public void NextRound(int roundid)
        {
            m_RoundText.text = $"»ØºÏÊý:{roundid}/{mMaxRoundid}";
        }
        public void BattleEnd(bool isWin)
        {
            m_BattleEndText.text = isWin ? "<color=#00ADFF>Õ½¶·Ê¤Àû</color>" : "<color=#FF000A>Õ½¶·Ê§°Ü</color>";
        }
        // Update is called once per frame
        void Update()
        {
            NewWorldManager.Update();
        }
        private void OnDestroy()
        {
            NewWorldManager.DestroyWorld();
        }
    }
}
