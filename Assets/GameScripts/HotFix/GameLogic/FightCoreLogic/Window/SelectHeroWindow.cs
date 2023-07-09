using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectHeroWindow : MonoBehaviour
{
    public GameObject opHerosObjs;
    public Camera camera3D;
    public GameObject mCurSelectHero;
    public Transform[] seatTransArr;
 
    private bool mIsPress;
    public void OnEnable()
    { 
        opHerosObjs.SetActive(true);
        for (int i = 0; i < seatTransArr.Length; i++)
        {
            GameObject heroObj =null;// ResourcesManager.Instance.LoadObject("Prefabs/Hero/"+(100+i+1), seatTransArr[i],true,true);
           heroObj.name = heroObj.name.Replace("(Clone)","");
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mIsPress = true;
 
            Ray ray = camera3D.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debuger.Log("hit.collider.gameobjecNmae:" + hit.collider.gameObject.name);
                if (mCurSelectHero == null && hit.collider != null && hit.collider.transform.parent.childCount>=2)
                    mCurSelectHero = hit.collider.transform.parent.GetChild(1).gameObject;
            }
        }
        if (Input.GetMouseButton(0))
        {
            if (mIsPress && mCurSelectHero != null)
            {
                Debuger.Log("GetMouseButton mCurSelectHero.position:" + mCurSelectHero.transform.position);
                Vector3 pos = camera3D.ScreenToWorldPoint(Input.mousePosition);
                pos.y = 1f;
                pos.z += 12.6f;
                pos.z *= 3;
                mCurSelectHero.transform.position = pos;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            Debuger.Log("GetMouseButtonUp:");
            mIsPress = false;

            if (mCurSelectHero != null)
            {
                for (int i = 0; i < seatTransArr.Length; i++)
                {
                    if (Vector3.Distance(mCurSelectHero.transform.position, seatTransArr[i].position) <= 3)
                    {
                        Transform parent = seatTransArr[i];
                        //如表目标点已经存在了英雄就进行交换 
                        if (parent.childCount>=2)
                        {
                            //交换英雄位置
                            Transform targetSeatHeroTrans = parent.GetChild(1);
                            targetSeatHeroTrans.SetParent(mCurSelectHero.transform.parent);
                            targetSeatHeroTrans.localPosition = Vector3.zero;
                        }
                        mCurSelectHero.transform.parent = seatTransArr[i];
                        break;
                    }
                }
                mCurSelectHero.transform.localPosition = Vector3.zero;
            }
            mCurSelectHero = null;
        }
    }

    public void OnStartBattleButtonClick()
    {
        List<HeroSeatDataPb> heroSeatList = new List<HeroSeatDataPb>();
        for (int i = 0; i < seatTransArr.Length; i++)
        {
            int id =int.Parse(seatTransArr[i].GetChild(1).name);
            int seatid = i;
            HeroSeatDataPb heroSeatData = new HeroSeatDataPb { id = id, seatid = seatid };
            heroSeatList.Add(heroSeatData);
        }
        HallMsgHandlerConter.Instance.SendStatBattleRequest(heroSeatList);
        //gameObject.SetActive(false);
        //opHerosObjs.SetActive(false);
        //List<HeroData> heroList = new List<HeroData>();
        //for (int i = 0; i < seatTransArr.Length; i++)
        //{
        //    int heroid = int.Parse(seatTransArr[i].GetChild(1).name);
        //    //英雄数据  一般来说都是服务器给与
        //    HeroData heroData= ConfigConter.GetHeroData(heroid);
        //    heroData.seatid = i;
        //    heroList.Add(heroData);

        //}
        //WorldManager.CreateBattleWord(heroList,ConfigConter.EnemyDataList,Random.Range(1,101));
    }
}
