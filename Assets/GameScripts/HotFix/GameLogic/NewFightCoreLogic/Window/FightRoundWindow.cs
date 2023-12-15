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

        public List<GameObject> LeftFightUnitList;
        public List<GameObject> RightFightUnitList;
        public Transform m_TestUI;



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
