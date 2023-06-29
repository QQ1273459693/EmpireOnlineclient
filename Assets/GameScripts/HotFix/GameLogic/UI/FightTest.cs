using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public class FightTest : MonoBehaviour
    {
        public bool Spine;
        public GameObject OBJ;
        SpineAnimBox spineAnimBox;
        // Start is called before the first frame update
        void Start()
        {
            spineAnimBox= GameModule.ObjectPool.GetObjectPool<SpineAnimBox>().Spawn();
        }

        // Update is called once per frame
        void Update()
        {
            if (Spine)
            {
                var Data=ConfigLoader.Instance.Tables.TbEnemySpine.DataList;
                spineAnimBox.IntObj(OBJ);
                spineAnimBox.RefreshData(Data[0].SpineResName);
                Spine = false;
            }
        }
    }
}
