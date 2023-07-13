using GabrielBigardi.SpriteAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public class FightTest : MonoBehaviour
    {
        public bool Spine;
        public List<GameObject> OBJ;
        public GameObject HudDamage;
        public bool isTips;
        public List<GameObject> HP;

        public SpriteAnimator spriteAnimator;

        // Start is called before the first frame update
        void Start()
        {
            //Debuger.Log("查看长度:" + BattleWorldNodes.Instance.heroTransArr.Length);
        }

        // Update is called once per frame
        void Update()
        {
            if (Spine)
            {
                var Data=ConfigLoader.Instance.Tables.TbEnemySpine.DataList;
                for (int i = 0; i < Data.Count; i++)
                {
                    SpineAnimBox spineAnimBox = GameModule.ObjectPool.GetObjectPool<SpineAnimBox>().Spawn();
                    spineAnimBox.IntObj(OBJ[i]);
                    spineAnimBox.RefreshData(Data[i].SpineResName,false);
                }
                Spine = false;
            }
            if (isTips)
            {
                DamageTips();
                isTips = false;
            }
        }
        void DamageTips()
        {
            //Vector3 FFF = new Vector3(15.20F,-7.3F,0);

            //Debuger.Log(OBJ[1].transform.position);
            //Vector3 tmpVec3 = RectTransformUtility.WorldToScreenPoint(BattleWorldNodes.Instance.Camera3D,OBJ[1].transform.position);

            //if (RectTransformUtility.ScreenPointToWorldPointInRectangle(HudDamage.GetComponent<RectTransform>(), tmpVec3, BattleWorldNodes.Instance.UICamera, out tmpVec3))
            //{
            //    HudDamage.transform.position = tmpVec3;
            //    HudDamage.SetActive(true);
            //}


            //for (int i = 0; i < OBJ.Count; i++)
            //{
            //    Vector3 tmpVec31 = RectTransformUtility.WorldToScreenPoint(BattleWorldNodes.Instance.Camera3D, OBJ[i].transform.position);

            //    if (RectTransformUtility.ScreenPointToWorldPointInRectangle(HP[i].GetComponent<RectTransform>(), tmpVec31, BattleWorldNodes.Instance.UICamera, out tmpVec31))
            //    {
            //        HP[i].transform.position = tmpVec31;
            //    }
            //}

        }
    }
}
