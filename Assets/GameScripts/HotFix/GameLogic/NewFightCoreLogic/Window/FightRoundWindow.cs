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
        public List<Transform> LeftPosRect;
        public List<Transform> RightPosRect;
        public Transform Fight;
        public List<Transform> FightList;
        public List<GameObject> LeftFightUnitList;
        public List<GameObject> RightFightUnitList;
        public RectTransform m_TestUI;

        public Transform AttackCenter;
        public float Distance;
        public float YDistance;
        public Transform FightWorldCanvas;

        public Transform LeftFightRoot;
        public Transform LeftFightRoot_1;
        public Transform RightFightRoot;
        public Transform RightFightRoot_1;
        public List<Transform> LeftFightList;
        public List<Transform> LeftFightList_1;
        public List<Transform> RightFightList;
        public List<Transform> RightFightList_1;
        public Transform FightUIRoot;
        public RectTransform Canvas;
        public GameObject ParticleSystemRoot;
        public GameObject HUDTextSystemRoot;

        // Start is called before the first frame update
        void Start()
        {
            
            NewWorldManager.Initialize();
            return;
            Vector3 ScreenPosition = RectTransformUtility.WorldToScreenPoint(Camera3D, m_TestUI.position);
            //LeftFightUnitList[i].transform.position = worldPosition;
            Vector3 vector3 = new Vector3(544.68F, 467.85F,0);
            bool A = RectTransformUtility.ScreenPointToLocalPointInRectangle(m_TestUI.GetComponent<RectTransform>(), vector3, UiCamera, out Vector2 localPoint);
            var localPoint1 = RectTransformUtility.WorldToScreenPoint(Camera3D, Fight.position);




            // _worldPosTrans：3D世界中的某物件，将其位置作为起点
            Vector2 screenPos = Camera.main.WorldToScreenPoint(Fight.position);
            // 第一个参数是一个RectTransform，可以将UI中要挡在UI前的3D元素的父节点的RectTransform传入
            // 相机参数要传入UI相机。
            //RectTransformUtility.ScreenPointToWorldPointInRectangle(m_TestUI.TRAN, screenPos, UiCamera, out Vector3 beginPos);
            // 终点的z应小于实际的终点物件位置的Z坐标，以便能挡在终点物件之前。


            Vector3 ptScreen = RectTransformUtility.WorldToScreenPoint(Camera3D, m_TestUI.position);
            ptScreen.z = 0;
            Vector3 ptWorld = Camera.main.ScreenToWorldPoint(ptScreen);


            //var DF = ScreenPointToWorldPointInRectangle(m_TestUI, ScreenPosition, out ScreenPosition);

            //Vector3 worldPoint = Camera3D.ScreenToWorldPoint(ScreenPosition);
            Debug.Log("坐标是:" + localPoint + ",坐标:" + localPoint1 + ",坐标FF:" + ptWorld);



            //for (int i = 0; i < LeftFightUnitList.Count; i++)
            //{
            //    Transform HpObj = LeftPosRect[i];

            //    Vector3 tmpVec31 = RectTransformUtility.WorldToScreenPoint(Camera3D, HpObj.position);

            //    Vector3 ScreenPosition = RectTransformUtility.WorldToScreenPoint(Camera3D, HpObj.position);
            //    //LeftFightUnitList[i].transform.position = worldPosition;

            //    bool A=RectTransformUtility.ScreenPointToLocalPointInRectangle(HpObj.GetComponent<RectTransform>(), ScreenPosition, UiCamera,out Vector2 localPoint);
            //    bool AB= RectTransformUtility.ScreenPointToWorldPointInRectangle(HpObj.GetComponent<RectTransform>(), ScreenPosition, UiCamera, out Vector3 localPoint1);




            //    // _worldPosTrans：3D世界中的某物件，将其位置作为起点
            //    Vector2 screenPos = Camera.main.WorldToScreenPoint(HpObj.position);
            //    // 第一个参数是一个RectTransform，可以将UI中要挡在UI前的3D元素的父节点的RectTransform传入
            //    // 相机参数要传入UI相机。
            //    //RectTransformUtility.ScreenPointToWorldPointInRectangle(m_TestUI.TRAN, screenPos, UiCamera, out Vector3 beginPos);
            //    // 终点的z应小于实际的终点物件位置的Z坐标，以便能挡在终点物件之前。





            //    //var DF = ScreenPointToWorldPointInRectangle(m_TestUI, ScreenPosition, out ScreenPosition);

            //    //Vector3 worldPoint = Camera3D.ScreenToWorldPoint(ScreenPosition);
            //    Debug.Log("坐标是:"+ localPoint+",坐标:"+ localPoint1+",坐标FF:"+ HpObj.position);
            //    HpObj.transform.position = tmpVec31;
            //    //if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_TestUI, ScreenPosition, UiCamera, out ScreenPosition))
            //    //{
            //    //    HpObj.transform.position = SS;
            //    //}
            //}   
        }
        public void SwitchFightPos(int LeftFightCount,int RightFightCount)
        {
            Vector2 min = Camera.main.ViewportToWorldPoint(Vector2.zero);
            Vector2 max = Camera.main.ViewportToWorldPoint(Vector2.one);
            float LeftX = min.x + Distance;
            float RightX = max.x - Distance;
            float Height = max.y - (YDistance * 2);
            float YDis = max.y * 2 / FightList.Count;
            float CenterPos = max.y;

            Debug.Log($"屏幕宽:{Screen.width},高:{Screen.height},左下角:{min},右上角:{max}");
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
                    LeftFightList[0].position = new Vector3(LeftX, 0);
                    //隐藏
                    LeftFightList[1].gameObject.SetActive(false);
                    LeftFightList[2].gameObject.SetActive(false);
                    LeftFightList[3].gameObject.SetActive(false);
                    LeftFightList[4].gameObject.SetActive(false);
                    break;
                case 2:
                    float YD = Height / (float)3;
                    LeftFightList[0].position = new Vector3(LeftX, YD);
                    LeftFightList[1].position = new Vector3(LeftX, -YD);
                    //隐藏
                    LeftFightList[2].gameObject.SetActive(false);
                    LeftFightList[3].gameObject.SetActive(false);
                    LeftFightList[4].gameObject.SetActive(false);
                    break;
                case 3:
                    float YD1 = (Height / (float)2) + YDistance;
                    LeftFightList[0].position = new Vector3(LeftX, 0);
                    LeftFightList[1].position = new Vector3(LeftX, YD1);
                    LeftFightList[2].position = new Vector3(LeftX, -YD1);
                    //隐藏
                    LeftFightList[3].gameObject.SetActive(false);
                    LeftFightList[4].gameObject.SetActive(false);
                    break;
                case 4:
                    float YD2 = Height / (float)3;
                    LeftFightList[0].position = new Vector3(LeftX, YD2);
                    LeftFightList[1].position = new Vector3(LeftX, YD2 * 3);
                    LeftFightList[2].position = new Vector3(LeftX, -YD2);
                    LeftFightList[3].position = new Vector3(LeftX, -YD2 * 3);
                    //隐藏
                    LeftFightList[4].gameObject.SetActive(false);
                    break;
                case 5:
                    float YD3 = Height / (float)3;
                    LeftFightList[0].position = new Vector3(LeftX, 0);
                    LeftFightList[1].position = new Vector3(LeftX, YD3 * 2);
                    LeftFightList[2].position = new Vector3(LeftX, YD3 * 4);
                    LeftFightList[3].position = new Vector3(LeftX, -YD3 * 2);
                    LeftFightList[4].position = new Vector3(LeftX, -YD3 * 4);
                    break;
            }

            //右边
            switch (RightFightCount)
            {
                case 1:
                    RightFightList[0].position = new Vector3(RightX, 0);
                    //隐藏
                    RightFightList[1].gameObject.SetActive(false);
                    RightFightList[2].gameObject.SetActive(false);
                    RightFightList[3].gameObject.SetActive(false);
                    RightFightList[4].gameObject.SetActive(false);
                    break;
                case 2:
                    float YD = Height / (float)3;
                    RightFightList[0].position = new Vector3(RightX, YD);
                    RightFightList[1].position = new Vector3(RightX, -YD);
                    //隐藏
                    RightFightList[2].gameObject.SetActive(false);
                    RightFightList[3].gameObject.SetActive(false);
                    RightFightList[4].gameObject.SetActive(false);
                    break;
                case 3:
                    float YD1 = (Height / (float)2) + YDistance;
                    RightFightList[0].position = new Vector3(RightX, 0);
                    RightFightList[1].position = new Vector3(RightX, YD1);
                    RightFightList[2].position = new Vector3(RightX, -YD1);
                    //隐藏
                    RightFightList[3].gameObject.SetActive(false);
                    RightFightList[4].gameObject.SetActive(false);
                    break;
                case 4:
                    float YD2 = Height / (float)3;
                    RightFightList[0].position = new Vector3(RightX, YD2);
                    RightFightList[1].position = new Vector3(RightX, YD2 * 3);
                    RightFightList[2].position = new Vector3(RightX, -YD2);
                    RightFightList[3].position = new Vector3(RightX, -YD2 * 3);
                    //隐藏
                    RightFightList[4].gameObject.SetActive(false);
                    break;
                case 5:
                    float YD3 = Height / (float)3;
                    RightFightList[0].position = new Vector3(RightX, 0);
                    RightFightList[1].position = new Vector3(RightX, YD3 * 2);
                    RightFightList[2].position = new Vector3(RightX, YD3 * 4);
                    RightFightList[3].position = new Vector3(RightX, -YD3 * 2);
                    RightFightList[4].position = new Vector3(RightX, -YD3 * 4);
                    break;
            }
        }
        public Vector2 WorldToScreenPoint(Vector3 worldPoint)
        {
            return RectTransformUtility.WorldToScreenPoint(Camera3D, worldPoint);
        }
        public  bool ScreenPointToWorldPointInRectangle(RectTransform rect, Vector2 screenPoint, out Vector3 worldPoint)
        {
            return RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, screenPoint, UiCamera, out worldPoint);
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
