using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public class FightTest : MonoBehaviour
    {
        public bool Spine;
        public List<GameObject> OBJ;
        // Start is called before the first frame update
        void Start()
        {
            Debuger.Log("查看长度:" + BattleWorldNodes.Instance.heroTransArr.Length);
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
        }
    }
}
